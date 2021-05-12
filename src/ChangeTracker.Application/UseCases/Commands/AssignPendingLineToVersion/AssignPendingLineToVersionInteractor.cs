using System;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.ChangeLogs;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Application.UseCases.Commands.AssignPendingLineToVersion.Models;
using ChangeTracker.Domain.ChangeLog;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;

// ReSharper disable UseDeconstructionOnParameter

namespace ChangeTracker.Application.UseCases.Commands.AssignPendingLineToVersion
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

        public async Task ExecuteAsync(IAssignPendingLineOutputPort output,
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

        public async Task ExecuteAsync(IAssignPendingLineOutputPort output, VersionAssignmentRequestModel requestModel)
        {
            if (!ClVersionValue.TryParse(requestModel.Version, out var versionValue))
            {
                output.InvalidVersionFormat(requestModel.Version);
                return;
            }

            _unitOfWork.Start();

            var version = await _versionDao.FindVersionAsync(requestModel.ProjectId, versionValue);
            if (version.HasNoValue)
            {
                output.VersionDoesNotExist();
                return;
            }

            await AssignToVersionAsync(output, version.Value, requestModel.ChangeLogLineId);
        }

        private async Task AssignToVersionAsync(IAssignPendingLineOutputPort output, ClVersion version,
            Guid pendingLineId)
        {
            var changeLogs = await _changeLogQueries.GetChangeLogsAsync(version.ProjectId, version.Id);
            if (!changeLogs.IsPositionAvailable)
            {
                output.MaxChangeLogLinesReached(ChangeLogs.MaxLines);
                return;
            }

            var pendingLine = await _changeLogQueries.FindLineAsync(pendingLineId);
            if (pendingLine.HasNoValue)
            {
                output.ChangeLogLineDoesNotExist();
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

        private async Task SaveAssignmentAsync(IAssignPendingLineOutputPort output, ChangeLogLine assignedLine)
        {
            await _changeLogCommands.MoveLineAsync(assignedLine)
                .Match(Finish, c => output.Conflict(c.Reason));

            void Finish(ChangeLogLine l)
            {
                _unitOfWork.Commit();
                output.Assigned(l.VersionId!.Value, l.Id);
            }
        }
    }
}