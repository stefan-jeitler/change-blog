using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.Projects;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Domain;
using ChangeTracker.Domain.Version;

namespace ChangeTracker.Application.UseCases.CreateVersion
{
    public class CreateVersionUseCase : ICreateVersionUseCase
    {
        private readonly IProjectDao _projectDao;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVersionDao _versionDao;

        public CreateVersionUseCase(IVersionDao versionDao, IProjectDao projectDao, IUnitOfWork unitOfWork)
        {
            _versionDao = versionDao ?? throw new ArgumentNullException(nameof(versionDao));
            _projectDao = projectDao ?? throw new ArgumentNullException(nameof(projectDao));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task ExecuteAsync(ICreateVersionOutputPort output, CreateVersionDto versionDto)
        {
            var (projectId, v) = versionDto;
            if (!ClVersion.TryParse(v, out var version))
            {
                output.InvalidVersionFormat(v);
                return;
            }

            _unitOfWork.Start();
            var project = await _projectDao.FindAsync(projectId);
            if (project.HasNoValue)
            {
                output.ProjectDoesNotExist();
                return;
            }

            if (!version.Match(project.Value.VersioningScheme))
            {
                output.VersionDoesNotMatchScheme(version.Value);
                return;
            }

            await SaveVersionAsync(output, project.Value, version);
            _unitOfWork.Commit();
        }

        private async Task SaveVersionAsync(ICreateVersionOutputPort output, Project project, ClVersion version)
        {
            var versionInfo = new ClVersionInfo(Guid.NewGuid(), project.Id, version, null,
                DateTime.UtcNow, null);

            var result = await _versionDao.AddAsync(versionInfo);
            result.Switch(x => output.Created(x.Id), output.Conflict);
        }
    }
}