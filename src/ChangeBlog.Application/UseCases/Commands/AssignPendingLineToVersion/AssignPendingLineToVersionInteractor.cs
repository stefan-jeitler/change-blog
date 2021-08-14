using System;
using System.Threading.Tasks;
using ChangeBlog.Application.DataAccess;
using ChangeBlog.Application.DataAccess.ChangeLog;
using ChangeBlog.Application.DataAccess.Versions;
using ChangeBlog.Application.UseCases.Commands.AssignPendingLineToVersion.Models;
using ChangeBlog.Domain.ChangeLog;
using ChangeBlog.Domain.Version;
using CSharpFunctionalExtensions;

// ReSharper disable UseDeconstructionOnParameter

namespace ChangeBlog.Application.UseCases.Commands.AssignPendingLineToVersion
{
    public class AssignPendingLineToVersionInteractor : IAssignPendingLineToVersion
    {
        private readonly IChangeLogCommandsDao _changeLogCommands;
        private readonly IChangeLogQueriesDao _changeLogQueries;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVersionDao _versionDao;

        public AssignPendingLineToVersionInteractor(IVersionDao versionDao, IChangeLogQueriesDao changeLogQueriesDao,
            IChangeLogCommandsDao changeLogCommands, IUnitOfWork unitOfWork)
        {
            _versionDao = versionDao ?? throw new ArgumentNullException(nameof(versionDao));
            _changeLogQueries = changeLogQueriesDao ?? throw new ArgumentNullException(nameof(changeLogQueriesDao));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _changeLogCommands = changeLogCommands ?? throw new ArgumentNullException(nameof(changeLogCommands));
        }

        public async Task ExecuteAsync(IAssignPendingLineToVersionOutputPort output,
            VersionIdAssignmentRequestModel requestModel)
        {
            _unitOfWork.Start();

            var version = await _versionDao.FindVersionAsync(requestModel.VersionId);
            if (version.HasNoValue)
            {
                output.VersionDoesNotExist();
                return;
            }

            await AssignToVersionAsync(output, version.Value, requestModel.ChangeLogLineId);
        }

        public async Task ExecuteAsync(IAssignPendingLineToVersionOutputPort output,
            VersionAssignmentRequestModel requestModel)
        {
            if (!ClVersionValue.TryParse(requestModel.Version, out var versionValue))
            {
                output.InvalidVersionFormat(requestModel.Version);
                return;
            }

            _unitOfWork.Start();

            var version = await _versionDao.FindVersionAsync(requestModel.ProductId, versionValue);
            if (version.HasNoValue)
            {
                output.VersionDoesNotExist();
                return;
            }

            await AssignToVersionAsync(output, version.Value, requestModel.ChangeLogLineId);
        }

        private async Task AssignToVersionAsync(IAssignPendingLineToVersionOutputPort output, ClVersion version,
            Guid pendingLineId)
        {
            var changeLogs = await _changeLogQueries.GetChangeLogsAsync(version.ProductId, version.Id);
            if (!changeLogs.IsPositionAvailable)
            {
                output.MaxChangeLogLinesReached(ChangeLogs.MaxLines);
                return;
            }

            var pendingLine = await _changeLogQueries.FindLineAsync(pendingLineId);
            if (pendingLine.HasNoValue)
            {
                output.ChangeLogLineDoesNotExist(pendingLineId);
                return;
            }

            if (pendingLine.Value.ProductId != version.ProductId)
            {
                output.TargetVersionBelongsToDifferentProduct(pendingLine.Value.ProductId, version.ProductId);
                return;
            }

            if (!pendingLine.Value.IsPending)
            {
                output.ChangeLogLineIsNotPending(pendingLine.Value.Id);
                return;
            }

            if (changeLogs.ContainsText(pendingLine.Value.Text))
            {
                output.LineWithSameTextAlreadyExists(pendingLine.Value.Text);
                return;
            }

            var assignedLine = pendingLine.Value.AssignToVersion(version.Id, changeLogs.NextFreePosition);
            await SaveAssignmentAsync(output, assignedLine);
        }

        private async Task SaveAssignmentAsync(IAssignPendingLineToVersionOutputPort output, ChangeLogLine assignedLine)
        {
            await _changeLogCommands.MoveLineAsync(assignedLine)
                .Match(Finish, output.Conflict);

            void Finish(ChangeLogLine l)
            {
                _unitOfWork.Commit();
                output.Assigned(l.VersionId!.Value, l.Id);
            }
        }
    }
}
