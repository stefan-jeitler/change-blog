﻿using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.ChangeLogs;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Domain.ChangeLog;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;
// ReSharper disable InvertIf

namespace ChangeTracker.Application.UseCases.MakeChangeLogLinePending
{
    public class MakeChangeLogLinePendingInteractor : IMakeChangeLogLinePending
    {
        private readonly IChangeLogCommandsDao _changeLogCommands;
        private readonly IChangeLogQueriesDao _changeLogQueries;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVersionDao _versionDao;

        public MakeChangeLogLinePendingInteractor(IVersionDao versionDao, IChangeLogQueriesDao changeLogQueries,
            IChangeLogCommandsDao changeLogCommands, IUnitOfWork unitOfWork)
        {
            _versionDao = versionDao;
            _changeLogQueries = changeLogQueries;
            _unitOfWork = unitOfWork;
            _changeLogCommands = changeLogCommands;
        }

        public async Task ExecuteAsync(IMakeChangeLogLinePendingOutputPort output, Guid changeLogLineId)
        {
            if (changeLogLineId == Guid.Empty)
                throw new ArgumentException("ChangeLogLineId cannot be empty.");

            _unitOfWork.Start();

            var line = await GetLineAsync(output, changeLogLineId);
            if (line.HasNoValue)
                return;

            var clVersion = await GetVersionAsync(output, line.Value);
            if (clVersion.HasNoValue)
                return;

            var pendingChangeLogMetadata = await GetChangeLogsMetadataAsync(output, line.Value);
            if (pendingChangeLogMetadata.HasNoValue)
                return;

            await MoveLineAsync(output, line, pendingChangeLogMetadata.Value);
        }

        private async Task<Maybe<ChangeLogLine>> GetLineAsync(IMakeChangeLogLinePendingOutputPort output, Guid changeLogLineId)
        {
            var line = await _changeLogQueries.FindLineAsync(changeLogLineId);
            if (line.HasNoValue)
            {
                output.ChangeLogLineDoesNotExist();
                return Maybe<ChangeLogLine>.None;
            }

            if (line.Value.IsPending)
            {
                output.ChangeLogLineIsAlreadyPending();
                return Maybe<ChangeLogLine>.None;
            }

            return line;
        }

        private async Task<Maybe<ClVersion>> GetVersionAsync(IMakeChangeLogLinePendingOutputPort output, ChangeLogLine line)
        {
            var versionId = line.VersionId!.Value;
            var clVersion = await _versionDao.GetVersionAsync(versionId);

            if (clVersion.IsReleased)
            {
                output.VersionAlreadyReleased();
                return Maybe<ClVersion>.None;
            }

            if (clVersion.IsDeleted)
            {
                output.VersionDeleted();
                return Maybe<ClVersion>.None;
            }

            return Maybe<ClVersion>.From(clVersion);
        }

        private async Task<Maybe<ChangeLogsMetadata>> GetChangeLogsMetadataAsync(IMakeChangeLogLinePendingOutputPort output,
            ChangeLogLine line)
        {
            var pendingChangeLogMetadata = await _changeLogQueries.GetChangeLogsMetadataAsync(line.ProjectId);
            if (!pendingChangeLogMetadata.IsPositionAvailable)
            {
                output.TooManyPendingLines(ChangeLogsMetadata.MaxChangeLogLines);
                return Maybe<ChangeLogsMetadata>.None;
            }

            if (pendingChangeLogMetadata.Texts.Contains(line.Text))
            {
                output.LineWithSameTextAlreadyExists(line.Text);
                return Maybe<ChangeLogsMetadata>.None;
            }

            return Maybe<ChangeLogsMetadata>.From(pendingChangeLogMetadata);
        }

        private async Task MoveLineAsync(IMakeChangeLogLinePendingOutputPort output, Maybe<ChangeLogLine> line,
            ChangeLogsMetadata pendingChangeLogMetadata)
        {
            var l = line.Value;
            var pendingLine = new ChangeLogLine(l.Id, null, l.ProjectId, l.Text,
                pendingChangeLogMetadata.NextFreePosition, l.CreatedAt, l.Labels, l.Issues, l.DeletedAt);

            await _changeLogCommands.MoveLineAsync(pendingLine)
                .Match(Finish, c => output.Conflict(c.Reason));

            void Finish(ChangeLogLine m)
            {
                _unitOfWork.Commit();
                output.WasMadePending(m.Id);
            }
        }
    }
}