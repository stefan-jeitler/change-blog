using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Application.UseCases.AssignAllPendingLinesToVersion.Models;
using ChangeTracker.Domain.ChangeLog;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.UseCases.AssignAllPendingLinesToVersion
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

            _unitOfWork.Start();

            var clVersion = await _versionDao.FindVersionAsync(projectId, version);
            if (clVersion.HasNoValue)
            {
                output.VersionDoesNotExist();
                return;
            }

            var assignedLines = await AssignLinesAsync(output, projectId, clVersion.Value);
            if (assignedLines.HasNoValue)
                return;

            await MoveLinesAsync(output, assignedLines.Value, clVersion.Value);
        }

        private async Task<Maybe<IEnumerable<ChangeLogLine>>> AssignLinesAsync(IAssignAllPendingLinesToVersionOutputPort output, 
            Guid projectId, ClVersion version)
        {
            var versionChangeLogs = await _changeLogQueries.GetChangeLogsMetadataAsync(projectId, version.Id);
            var pendingChangeLogs = await _changeLogQueries.GetChangeLogsMetadataAsync(projectId);

            if (pendingChangeLogs.Count > versionChangeLogs.RemainingPositionsToAdd)
            {
                output.TooManyLinesToAdd(versionChangeLogs.RemainingPositionsToAdd);
                return Maybe<IEnumerable<ChangeLogLine>>.None;
            }

            var lines = await _changeLogQueries.GetPendingLines(projectId);

            var assignedLines = lines
                .Select((x, i) => new {Line = x, Position = versionChangeLogs.NextFreePosition + i})
                .Select(x => x.Line.AssignToVersion(version.Id, (uint) x.Position));

            return Maybe<IEnumerable<ChangeLogLine>>.From(assignedLines);
        }

        private async Task MoveLinesAsync(IAssignAllPendingLinesToVersionOutputPort output, IEnumerable<ChangeLogLine> assignedLines,
            ClVersion version)
        {
            await _changeLogCommands.MoveLinesAsync(assignedLines)
                .Match(Finish, c => output.Conflict(c.Reason));

            void Finish(int count)
            {
                _unitOfWork.Commit();
                output.Assigned(version.Id);
            }
        }
    }
}