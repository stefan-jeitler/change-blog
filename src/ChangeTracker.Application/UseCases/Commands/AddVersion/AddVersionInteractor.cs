using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.Projects;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Domain;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;

// ReSharper disable InvertIf

namespace ChangeTracker.Application.UseCases.Commands.AddVersion
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
            if (!ClVersionValue.TryParse(v, out var versionValue))
            {
                output.InvalidVersionFormat(v);
                return;
            }

            _unitOfWork.Start();
            var project = await GetProjectAsync(output, projectId);
            if (project.HasNoValue)
                return;

            var clVersion = await CreateVersionAsync(output, project.Value, versionValue);
            if (clVersion.HasNoValue)
                return;

            await SaveVersionAsync(output, clVersion.Value);
        }

        public async Task<Maybe<ClVersion>> CreateVersionAsync(IAddVersionOutputPort output, Project project,
            ClVersionValue versionValue)
        {
            var existingClVersion = await _versionDao.FindVersionAsync(project.Id, versionValue);
            if (existingClVersion.HasValue)
            {
                output.VersionAlreadyExists(versionValue);
                return Maybe<ClVersion>.None;
            }

            if (!versionValue.Match(project.VersioningScheme))
            {
                output.VersionDoesNotMatchScheme(versionValue);
                return Maybe<ClVersion>.None;
            }

            var clVersion = new ClVersion(Guid.NewGuid(),
                project.Id, versionValue,
                null, DateTime.UtcNow, null);

            return Maybe<ClVersion>.From(clVersion);
        }

        private async Task<Maybe<Project>> GetProjectAsync(IAddVersionOutputPort output, Guid projectId)
        {
            var project = await _projectDao.FindProjectAsync(projectId);
            if (project.HasNoValue)
            {
                output.ProjectDoesNotExist();
                return Maybe<Project>.None;
            }

            if (project.Value.IsClosed)
            {
                output.ProjectClosed();
                return Maybe<Project>.None;
            }

            return project;
        }

        private async Task SaveVersionAsync(IAddVersionOutputPort output, ClVersion clVersion)
        {
            await _versionDao
                .AddVersionAsync(clVersion)
                .Match(Finish, c => output.Conflict(c));

            void Finish(ClVersion v)
            {
                _unitOfWork.Commit();
                output.Created(v.Id);
            }
        }
    }
}