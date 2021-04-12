using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Application.UseCases.AssignPendingLineToVersion.DTOs;
using ChangeTracker.Domain.ChangeLog;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;

// ReSharper disable UseDeconstructionOnParameter

namespace ChangeTracker.Application.UseCases.AssignPendingLineToVersion
{
    public class AssignPendingLineUseCase : IAssignPendingLineUseCase
    {
        private readonly IChangeLogDao _changeLogDao;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVersionDao _versionDao;

        public AssignPendingLineUseCase(IVersionDao versionDao, IChangeLogDao changeLogDao, IUnitOfWork unitOfWork)
        {
            _versionDao = versionDao;
            _changeLogDao = changeLogDao;
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync(IAssignPendingLineOutputPort output, PendingLineByVersionIdDto assignmentDto)
        {
            _unitOfWork.Start();

            var versionInfo = await _versionDao.FindAsync(assignmentDto.ProjectId, assignmentDto.VersionId);
            if (versionInfo.HasNoValue)
            {
                output.VersionDoesNotExist();
                return;
            }

            await AssignToVersionAsync(output, versionInfo.Value, assignmentDto.ChangeLogLineId);
        }

        public async Task ExecuteAsync(IAssignPendingLineOutputPort output, PendingLineByVersionDto assignmentDto)
        {
            if (!ClVersionValue.TryParse(assignmentDto.Version, out var versionValue))
            {
                output.InvalidVersionFormat(assignmentDto.Version);
                return;
            }

            _unitOfWork.Start();

            var version = await _versionDao.FindAsync(assignmentDto.ProjectId, versionValue);
            if (version.HasNoValue)
            {
                output.VersionDoesNotExist();
                return;
            }

            await AssignToVersionAsync(output, version.Value, assignmentDto.ChangeLogLineId);
        }

        private async Task AssignToVersionAsync(IAssignPendingLineOutputPort output, ClVersion version,
            Guid pendingLineId)
        {
            var changeLogInfo = await _changeLogDao.GetChangeLogsMetadataAsync(version.ProjectId, version.Id);
            if (!changeLogInfo.IsPositionAvailable)
            {
                output.MaxChangeLogLinesReached(ChangeLogsMetadata.MaxChangeLogLines);
                return;
            }

            var existingLine = await _changeLogDao.GetAsync(pendingLineId);
            if (existingLine.HasNoValue)
            {
                output.ChangeLogLineDoesNotExist();
                return;
            }

            var assignedLine = existingLine.Value.AssignToVersion(version.Id, changeLogInfo.NextFreePosition);

            await SaveLineAsync(output, assignedLine);
        }

        private async Task SaveLineAsync(IAssignPendingLineOutputPort output, ChangeLogLine assignedLine)
        {
            await _changeLogDao.UpdateLineAsync(assignedLine)
                .Match(Finish, c => output.Conflict(c.Reason));

            void Finish(int count)
            {
                output.Assigned(assignedLine.VersionId!.Value, assignedLine.Id);
                _unitOfWork.Commit();
            }
        }
    }
}