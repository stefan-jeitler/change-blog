using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess.Projects;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;

// ReSharper disable InvertIf

namespace ChangeTracker.Application.UseCases.Commands.DeleteVersion
{
    public class DeleteVersionInteractor : IDeleteVersion
    {
        private readonly IProjectDao _projectDao;
        private readonly IVersionDao _versionDao;

        public DeleteVersionInteractor(IVersionDao versionDao, IProjectDao projectDao)
        {
            _versionDao = versionDao;
            _projectDao = projectDao;
        }

        public async Task ExecuteAsync(IDeleteVersionOutputPort output, Guid versionId)
        {
            var clVersion = await GetVersionAsync(output, versionId);
            if (clVersion.HasNoValue)
                return;

            var project = await _projectDao.GetProjectAsync(clVersion.Value.ProjectId);
            if (project.IsClosed)
            {
                output.ProjectClosed(project.Id);
                return;
            }

            await _versionDao.DeleteVersionAsync(clVersion.Value)
                .Match(
                    v => output.VersionDeleted(v.Id),
                    c => output.Conflict(c.Reason));
        }

        private async Task<Maybe<ClVersion>> GetVersionAsync(IDeleteVersionOutputPort output, Guid versionId)
        {
            var clVersion = await _versionDao.FindVersionAsync(versionId);
            if (clVersion.HasNoValue)
            {
                output.VersionDoesNotExist();
                return Maybe<ClVersion>.None;
            }

            if (clVersion.Value.IsDeleted)
            {
                output.VersionAlreadyDeleted();
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