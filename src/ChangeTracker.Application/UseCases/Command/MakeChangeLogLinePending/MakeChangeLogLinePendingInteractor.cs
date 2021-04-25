using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.ChangeLogs;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Domain.ChangeLog;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;

// ReSharper disable InvertIf

namespace ChangeTracker.Application.UseCases.Command.MakeChangeLogLinePending
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

            var pendingLine = MakeLinePending(line.Value, pendingChangeLogMetadata.Value);
            await SaveAssignmentAsync(output, pendingLine);
        }

        private async Task<Maybe<ChangeLogLine>> GetLineAsync(IMakeChangeLogLinePendingOutputPort output,
            Guid changeLogLineId)
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

        private async Task<Maybe<ClVersion>> GetVersionAsync(IMakeChangeLogLinePendingOutputPort output,
            ChangeLogLine line)
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
                output.VersionClosed();
                return Maybe<ClVersion>.None;
            }

            return Maybe<ClVersion>.From(clVersion);
        }

        private async Task<Maybe<ChangeLogsMetadata>> GetChangeLogsMetadataAsync(
            IMakeChangeLogLinePendingOutputPort output,
            ChangeLogLine line)
        {
            var pendingChangeLogs = await _changeLogQueries.GetChangeLogsMetadataAsync(line.ProjectId);
            if (!pendingChangeLogs.IsPositionAvailable)
            {
                output.TooManyPendingLines(ChangeLogsMetadata.MaxChangeLogLines);
                return Maybe<ChangeLogsMetadata>.None;
            }

            if (pendingChangeLogs.Texts.Contains(line.Text))
            {
                output.LineWithSameTextAlreadyExists(line.Text);
                return Maybe<ChangeLogsMetadata>.None;
            }

            return Maybe<ChangeLogsMetadata>.From(pendingChangeLogs);
        }

        private static ChangeLogLine MakeLinePending(ChangeLogLine line, ChangeLogsMetadata pendingChangeLogMetadata) =>
            new(line.Id, null,
                line.ProjectId, line.Text,
                pendingChangeLogMetadata.NextFreePosition,
                line.CreatedAt, line.Labels,
                line.Issues, line.DeletedAt);

        private async Task SaveAssignmentAsync(IMakeChangeLogLinePendingOutputPort output, ChangeLogLine line)
        {
            await _changeLogCommands.AssignLineToVersionAsync(line)
                .Match(Finish, c => output.Conflict(c.Reason));

            void Finish(ChangeLogLine m)
            {
                _unitOfWork.Commit();
                output.WasMadePending(m.Id);
            }
        }
    }
}