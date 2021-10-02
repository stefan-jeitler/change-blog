using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeBlog.Application.DataAccess;
using ChangeBlog.Application.DataAccess.ChangeLog;
using ChangeBlog.Application.DataAccess.Versions;
using ChangeBlog.Application.UseCases.Commands.AssignAllPendingLinesToVersion.Models;
using ChangeBlog.Domain.ChangeLog;
using ChangeBlog.Domain.Version;
using CSharpFunctionalExtensions;

namespace ChangeBlog.Application.UseCases.Commands.AssignAllPendingLinesToVersion
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
            _versionDao = versionDao ?? throw new ArgumentNullException(nameof(versionDao));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _changeLogCommands = changeLogCommands ?? throw new ArgumentNullException(nameof(changeLogCommands));
            _changeLogQueries = changeLogQueries ?? throw new ArgumentNullException(nameof(changeLogQueries));
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

            if (requestModel.ProductId != clVersion.GetValueOrThrow().ProductId)
            {
                output.TargetVersionBelongsToDifferentProduct(requestModel.ProductId, clVersion.GetValueOrThrow().ProductId);
                return;
            }

            await AssignAllPendingLinesToVersionAsync(output, clVersion.GetValueOrThrow());
        }

        public async Task ExecuteAsync(IAssignAllPendingLinesToVersionOutputPort output,
            VersionAssignmentRequestModel requestModel)
        {
            var productId = requestModel.ProductId;

            if (!ClVersionValue.TryParse(requestModel.Version, out var version))
            {
                output.InvalidVersionFormat(requestModel.Version);
                return;
            }

            var clVersion = await _versionDao.FindVersionAsync(productId, version);
            if (clVersion.HasNoValue)
            {
                output.VersionDoesNotExist();
                return;
            }

            await AssignAllPendingLinesToVersionAsync(output, clVersion.GetValueOrThrow());
        }

        private async Task AssignAllPendingLinesToVersionAsync(IAssignAllPendingLinesToVersionOutputPort output,
            ClVersion clVersion)
        {
            _unitOfWork.Start();

            var assignedLines = await AssignLinesAsync(output, clVersion);
            if (assignedLines.HasNoValue)
                return;

            await SaveAssignmentsAsync(output, assignedLines.GetValueOrThrow(), clVersion);
        }

        private async Task<Maybe<IEnumerable<ChangeLogLine>>> AssignLinesAsync(
            IAssignAllPendingLinesToVersionOutputPort output, ClVersion clVersion)
        {
            var productId = clVersion.ProductId;
            var versionChangeLogs = await _changeLogQueries.GetChangeLogsAsync(productId, clVersion.Id);
            var pendingChangeLogs = await _changeLogQueries.GetChangeLogsAsync(productId);

            if (pendingChangeLogs.Count == 0)
            {
                output.NoPendingChangeLogLines(clVersion.ProductId);
                return Maybe<IEnumerable<ChangeLogLine>>.None;
            }

            if (pendingChangeLogs.Count > versionChangeLogs.RemainingPositionsToAdd)
            {
                output.TooManyLinesToAdd(versionChangeLogs.RemainingPositionsToAdd);
                return Maybe<IEnumerable<ChangeLogLine>>.None;
            }

            var duplicates = versionChangeLogs
                .FindDuplicateTexts(pendingChangeLogs.Lines)
                .ToList();

            if (duplicates.Any())
            {
                output.LineWithSameTextAlreadyExists(duplicates.Select(x => x.Text.Value).ToList());
                return Maybe<IEnumerable<ChangeLogLine>>.None;
            }

            var assignedLines = pendingChangeLogs.Lines
                .Select((x, i) => new {Line = x, Position = versionChangeLogs.NextFreePosition + i})
                .Select(x => x.Line.AssignToVersion(clVersion.Id, (uint) x.Position));

            return Maybe<IEnumerable<ChangeLogLine>>.From(assignedLines);
        }

        private async Task SaveAssignmentsAsync(IAssignAllPendingLinesToVersionOutputPort output,
            IEnumerable<ChangeLogLine> assignedLines, ClVersion clVersion)
        {
            await _changeLogCommands.MoveLinesAsync(assignedLines)
                .Match(Finish, output.Conflict);

            void Finish(int count)
            {
                _unitOfWork.Commit();
                output.Assigned(clVersion.Id);
            }
        }
    }
}
