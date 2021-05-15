using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess.Projects;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;

// ReSharper disable InvertIf

namespace ChangeTracker.Application.UseCases.Commands.ReleaseVersion
{
    public class ReleaseVersionInteractor : IReleaseVersion
    {
        private readonly IProjectDao _project;
        private readonly IVersionDao _versionDao;

        public ReleaseVersionInteractor(IVersionDao versionDao, IProjectDao project)
        {
            _versionDao = versionDao ?? throw new ArgumentNullException(nameof(versionDao));
            _project = project ?? throw new ArgumentNullException(nameof(project));
        }

        public async Task ExecuteAsync(IReleaseVersionOutputPort output, Guid versionId)
        {
            var clVersion = await GetVersionAsync(output, versionId);
            if (clVersion.HasNoValue)
                return;

            var project = await _project.GetProjectAsync(clVersion.Value.ProjectId);
            if (project.IsClosed)
            {
                output.RelatedProjectClosed(project.Id);
                return;
            }

            var releaseVersion = clVersion.Value.Release();
            await _versionDao.ReleaseVersionAsync(releaseVersion)
                .Match(
                    v => output.VersionReleased(v.Id),
                    c => output.Conflict(c.Reason));
        }

        private async Task<Maybe<ClVersion>> GetVersionAsync(IReleaseVersionOutputPort output, Guid versionId)
        {
            var clVersion = await _versionDao.FindVersionAsync(versionId);
            if (clVersion.HasNoValue)
            {
                output.VersionDoesNotExist();
                return Maybe<ClVersion>.None;
            }

            if (clVersion.Value.IsDeleted)
            {
                output.VersionDeleted();
                return Maybe<ClVersion>.None;
            }

            if (clVersion.Value.IsReleased)
            {
                output.VersionAlreadyReleased();
                return Maybe<ClVersion>.None;
            }

            return clVersion;
        }
    }
}