using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Application.UseCases.AssignPendingLineToVersion.Models;
using ChangeTracker.Domain.ChangeLog;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;

// ReSharper disable UseDeconstructionOnParameter

namespace ChangeTracker.Application.UseCases.AssignPendingLineToVersion
{
    public class AssignPendingLogToVersionInteractor : IAssignPendingLogToVersion
    {
        private readonly IChangeLogDao _changeLogDao;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVersionDao _versionDao;

        public AssignPendingLogToVersionInteractor(IVersionDao versionDao, IChangeLogDao changeLogDao, IUnitOfWork unitOfWork)
        {
            _versionDao = versionDao ?? throw new ArgumentNullException(nameof(versionDao));
            _changeLogDao = changeLogDao ?? throw new ArgumentNullException(nameof(changeLogDao));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task ExecuteAsync(IAssignPendingLineOutputPort output, VersionIdAssignmentRequestModel requestModel)
        {
            _unitOfWork.Start();

            var version = await _versionDao.FindAsync(requestModel.ProjectId, requestModel.VersionId);
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

            var version = await _versionDao.FindAsync(requestModel.ProjectId, versionValue);
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
            var changeLogsMetadata = await _changeLogDao.GetChangeLogsMetadataAsync(version.ProjectId, version.Id);
            if (!changeLogsMetadata.IsPositionAvailable)
            {
                output.MaxChangeLogLinesReached(ChangeLogsMetadata.MaxChangeLogLines);
                return;
            }

            var existingLine = await _changeLogDao.FindAsync(pendingLineId);
            if (existingLine.HasNoValue)
            {
                output.ChangeLogLineDoesNotExist();
                return;
            }

            var assignedLine = existingLine.Value.AssignToVersion(version.Id, changeLogsMetadata.NextFreePosition);

            await SaveLineAsync(output, assignedLine);
        }

        private async Task SaveLineAsync(IAssignPendingLineOutputPort output, ChangeLogLine assignedLine)
        {
            await _changeLogDao.UpdateLineAsync(assignedLine)
                .Match(Finish, c => output.Conflict(c.Reason));

            void Finish(int count)
            {
                _unitOfWork.Commit();
                output.Assigned(assignedLine.VersionId!.Value, assignedLine.Id);
            }
        }
    }
}