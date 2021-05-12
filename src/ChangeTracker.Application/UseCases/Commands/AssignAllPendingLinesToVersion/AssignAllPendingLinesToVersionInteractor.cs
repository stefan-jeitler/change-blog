﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.ChangeLogs;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Application.UseCases.Commands.AssignAllPendingLinesToVersion.Models;
using ChangeTracker.Domain.ChangeLog;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.UseCases.Commands.AssignAllPendingLinesToVersion
{
    public class AssignAllPendingLinesToVersionInteractor : IAssignAllPendingLinesToVersion
    {
        private readonly IChangeLogCommandsDao _changeLogCommands;
        private readonly IChangeLogQueriesDao _changeLogQueries;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVersionDao _versionDao;

        public AssignAllPendingLinesToVersionInteractor(IVersionDao versionDao, IUnitOfWork unitOfWork,
            IChangeLogCommandsDao changeLogCommands, IChangeLogQueriesDao changeLogQueries)
        {
            _versionDao = versionDao;
            _unitOfWork = unitOfWork;
            _changeLogCommands = changeLogCommands;
            _changeLogQueries = changeLogQueries;
        }

        public async Task ExecuteAsync(IAssignAllPendingLinesToVersionOutputPort output,
            VersionIdAssignmentRequestModel requestModel)
        {
            var clVersion = await _versionDao.FindVersionAsync(requestModel.VersionId);

            if (clVersion.HasNoValue)
            {
                output.VersionDoesNotExist();
                return;
            }

            await AssignAllPendingLinesToVersionAsync(output, clVersion.Value);
        }

        public async Task ExecuteAsync(IAssignAllPendingLinesToVersionOutputPort output,
            VersionAssignmentRequestModel requestModel)
        {
            var projectId = requestModel.ProjectId;

            if (!ClVersionValue.TryParse(requestModel.Version, out var version))
            {
                output.InvalidVersionFormat(requestModel.Version);
                return;
            }

            var clVersion = await _versionDao.FindVersionAsync(projectId, version);
            if (clVersion.HasNoValue)
            {
                output.VersionDoesNotExist();
                return;
            }

            await AssignAllPendingLinesToVersionAsync(output, clVersion.Value);
        }

        private async Task AssignAllPendingLinesToVersionAsync(IAssignAllPendingLinesToVersionOutputPort output,
            ClVersion clVersion)
        {
            _unitOfWork.Start();

            var assignedLines = await AssignLinesAsync(output, clVersion);
            if (assignedLines.HasNoValue)
                return;

            await SaveAssignmentsAsync(output, assignedLines.Value, clVersion);
        }

        private async Task<Maybe<IEnumerable<ChangeLogLine>>> AssignLinesAsync(
            IAssignAllPendingLinesToVersionOutputPort output, ClVersion clVersion)
        {
            var projectId = clVersion.ProjectId;
            var versionChangeLogs = await _changeLogQueries.GetChangeLogsAsync(projectId, clVersion.Id);
            var pendingChangeLogs = await _changeLogQueries.GetChangeLogsAsync(projectId);

            if (pendingChangeLogs.Count == 0)
            {
                output.NoPendingChangeLogLines();
                return Maybe<IEnumerable<ChangeLogLine>>.None;
            }

            if (pendingChangeLogs.Count > versionChangeLogs.RemainingPositionsToAdd)
            {
                output.TooManyLinesToAdd(versionChangeLogs.RemainingPositionsToAdd);
                return Maybe<IEnumerable<ChangeLogLine>>.None;
            }

            var lines = await _changeLogQueries.GetPendingLinesAsync(projectId);

            var duplicates = versionChangeLogs
                .FindDuplicateTexts(pendingChangeLogs.Lines)
                .ToList();

            if (duplicates.Any())
            {
                output.LineWithSameTextAlreadyExists(duplicates.Select(x => x.Text.Value).ToList());
                return Maybe<IEnumerable<ChangeLogLine>>.None;
            }

            var assignedLines = lines
                .Select((x, i) => new {Line = x, Position = versionChangeLogs.NextFreePosition + i})
                .Select(x => x.Line.AssignToVersion(clVersion.Id, (uint) x.Position));

            return Maybe<IEnumerable<ChangeLogLine>>.From(assignedLines);
        }

        private async Task SaveAssignmentsAsync(IAssignAllPendingLinesToVersionOutputPort output,
            IEnumerable<ChangeLogLine> assignedLines, ClVersion clVersion)
        {
            await _changeLogCommands.MoveLinesAsync(assignedLines)
                .Match(Finish, c => output.Conflict(c.Reason));

            void Finish(int count)
            {
                _unitOfWork.Commit();
                output.Assigned(clVersion.Id);
            }
        }
    }
}