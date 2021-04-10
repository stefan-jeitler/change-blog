using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.Accounts;
using ChangeTracker.Application.DataAccess.Projects;
using ChangeTracker.Domain;
using ChangeTracker.Domain.ChangeLog.Services;
using ChangeTracker.Domain.Common;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;

// ReSharper disable InvertIf

namespace ChangeTracker.Application.UseCases.CreateProject
{
    public class CreateProjectUseCase : ICreateProjectUseCase
    {
        private readonly IAccountDao _accountDao;
        private readonly IProjectDao _projectDao;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVersioningSchemeDao _versioningSchemeDao;

        public CreateProjectUseCase(IAccountDao accountDao, IVersioningSchemeDao versioningSchemeDao,
            IProjectDao projectDao, IUnitOfWork unitOfWork)
        {
            _accountDao = accountDao ?? throw new ArgumentNullException(nameof(accountDao));
            _versioningSchemeDao = versioningSchemeDao ?? throw new ArgumentNullException(nameof(versioningSchemeDao));
            _projectDao = projectDao ?? throw new ArgumentNullException(nameof(projectDao));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task ExecuteAsync(ICreateProjectOutputPort output, CreateProjectDto projectDto)
        {
            _unitOfWork.Start();

            var account = await GetAccountAsync(output, projectDto.AccountId);
            if (account.HasNoValue)
                return;

            if (!Name.TryParse(projectDto.Name, out var name))
            {
                output.InvalidName(projectDto.Name);
                return;
            }

            var existingProject = await _projectDao.FindAsync(account.Value.Id, name);
            if (existingProject.HasValue)
            {
                output.ProjectAlreadyExists();
                return;
            }

            var versioningSchemeId = await GetVersioningSchemeId(output, projectDto, account.Value);
            if (versioningSchemeId.HasNoValue)
            {
                output.VersioningSchemeDoesNotExist();
                return;
            }

            var project = new Project(account.Value.Id, name, versioningSchemeId.Value, DateTime.UtcNow);
            await SaveProjectAsync(output, project);
        }

        private async Task<Maybe<Account>> GetAccountAsync(ICreateProjectOutputPort output, Guid accountId)
        {
            var account = await _accountDao.FindAsync(accountId);
            if (account.HasNoValue)
            {
                output.AccountDoesNotExist();
                return Maybe<Account>.None;
            }

            if (account.Value.DeletedAt.HasValue)
            {
                output.AccountDeleted(account.Value.Id);
                return Maybe<Account>.None;
            }

            return account;
        }

        private async Task<Maybe<VersioningScheme>> GetVersioningSchemeId(ICreateProjectOutputPort output,
            CreateProjectDto createProjectDto, Account account)
        {
            var versioningSchemeService = new VersioningSchemeService(account);
            var customSchemeId = createProjectDto.VersioningSchemeId;
            var versioningSchemeId = versioningSchemeService.FindSchemeIdForProject(customSchemeId);

            return await _versioningSchemeDao.FindAsync(versioningSchemeId);
        }

        private async Task SaveProjectAsync(ICreateProjectOutputPort output, Project newProject)
        {
            var result = await _projectDao.AddAsync(newProject);

            result.Switch(x =>
            {
                output.Created(x.AccountId, x.Id);
                _unitOfWork.Commit();
            }, output.Conflict);
        }
    }
}