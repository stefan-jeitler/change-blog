using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess.Projects;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;

// ReSharper disable InvertIf

namespace ChangeTracker.Application.Services.NotReleasedVersion
{
    public class NotReleasedVersionService
    {
        private readonly IProjectDao _projectDao;
        private readonly IVersionDao _versionDao;

        public NotReleasedVersionService(IProjectDao projectDao, IVersionDao versionDao)
        {
            _projectDao = projectDao;
            _versionDao = versionDao;
        }

        public async Task<Maybe<ClVersion>> FindAsync(INotReleasedVersionOutputPort output, Guid projectId,
            ClVersionValue versionValue)
        {
            var noVersionInfo = Maybe<ClVersion>.None;

            var project = await _projectDao.FindAsync(projectId);
            if (project.HasNoValue)
            {
                output.ProjectDoesNotExist();
                return noVersionInfo;
            }

            var versionInfo = await _versionDao.FindAsync(project.Value.Id, versionValue);
            if (versionInfo.HasNoValue)
            {
                output.VersionDoesNotExist();
                return noVersionInfo;
            }

            if (versionInfo.Value.IsReleased)
            {
                output.VersionAlreadyReleased(versionInfo.Value.ReleasedAt!.Value);
                return noVersionInfo;
            }

            if (versionInfo.Value.IsDeleted)
            {
                output.VersionDeleted(versionInfo.Value.DeletedAt!.Value);
                return noVersionInfo;
            }

            return versionInfo;
        }
    }
}