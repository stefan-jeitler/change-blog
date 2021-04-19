using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.Projects;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Domain;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.UseCases.AddVersion
{
    public class AddVersionInteractor : IAddVersion
    {
        private readonly IProjectDao _projectDao;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVersionDao _versionDao;

        public AddVersionInteractor(IVersionDao versionDao, IProjectDao projectDao, IUnitOfWork unitOfWork)
        {
            _versionDao = versionDao ?? throw new ArgumentNullException(nameof(versionDao));
            _projectDao = projectDao ?? throw new ArgumentNullException(nameof(projectDao));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task ExecuteAsync(IAddVersionOutputPort output, VersionRequestModel versionRequestModel)
        {
            var (projectId, v) = versionRequestModel;
            if (!ClVersionValue.TryParse(v, out var version))
            {
                output.InvalidVersionFormat(v);
                return;
            }

            _unitOfWork.Start();
            var project = await _projectDao.FindProjectAsync(projectId);
            if (project.HasNoValue)
            {
                output.ProjectDoesNotExist();
                return;
            }

            var clVersion = await _versionDao.FindVersionAsync(project.Value.Id, version);
            if (clVersion.HasValue)
            {
                output.VersionAlreadyExists(version.Value);
                return;
            }

            if (!version.Match(project.Value.VersioningScheme))
            {
                output.VersionDoesNotMatchScheme(version.Value);
                return;
            }

            await SaveVersionAsync(output, project.Value, version);
        }

        private async Task SaveVersionAsync(IAddVersionOutputPort output, Project project,
            ClVersionValue versionValue)
        {
            var version = new ClVersion(Guid.NewGuid(),
                project.Id,
                versionValue,
                null,
                DateTime.UtcNow,
                null);

            await _versionDao
                .AddVersionAsync(version)
                .Match(Finish, c => output.Conflict(c));

            void Finish(ClVersion v)
            {
                _unitOfWork.Commit();
                output.Created(v.Id);
            }
        }
    }
}