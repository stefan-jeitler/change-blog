using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.Accounts;
using ChangeTracker.Application.DataAccess.Projects;
using ChangeTracker.Domain;
using ChangeTracker.Domain.Common;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;

// ReSharper disable InvertIf

namespace ChangeTracker.Application.UseCases.AddProject
{
    public class AddProjectInteractor : IAddProject
    {
        private readonly IAccountDao _accountDao;
        private readonly IProjectDao _projectDao;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVersioningSchemeDao _versioningSchemeDao;

        public AddProjectInteractor(IAccountDao accountDao, IVersioningSchemeDao versioningSchemeDao,
            IProjectDao projectDao, IUnitOfWork unitOfWork)
        {
            _accountDao = accountDao ?? throw new ArgumentNullException(nameof(accountDao));
            _versioningSchemeDao = versioningSchemeDao ?? throw new ArgumentNullException(nameof(versioningSchemeDao));
            _projectDao = projectDao ?? throw new ArgumentNullException(nameof(projectDao));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task ExecuteAsync(IAddProjectOutputPort output, ProjectRequestModel projectRequestModel)
        {
            _unitOfWork.Start();

            var account = await GetAccountAsync(output, projectRequestModel.AccountId);
            if (account.HasNoValue)
                return;

            if (!Name.TryParse(projectRequestModel.Name, out var name))
            {
                output.InvalidName(projectRequestModel.Name);
                return;
            }

            var existingProject = await _projectDao.FindProjectAsync(account.Value.Id, name);
            if (existingProject.HasValue)
            {
                output.ProjectAlreadyExists();
                return;
            }

            var versioningSchemeId = await GetVersioningSchemeIdAsync(output, projectRequestModel, account.Value);
            if (versioningSchemeId.HasNoValue)
            {
                output.VersioningSchemeDoesNotExist();
                return;
            }

            var project = new Project(account.Value.Id, name, versioningSchemeId.Value, DateTime.UtcNow);
            await SaveProjectAsync(output, project);
        }

        private async Task<Maybe<Account>> GetAccountAsync(IAddProjectOutputPort output, Guid accountId)
        {
            var account = await _accountDao.FindAccountAsync(accountId);
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

        private async Task<Maybe<VersioningScheme>> GetVersioningSchemeIdAsync(IAddProjectOutputPort output,
            ProjectRequestModel projectRequestModel, Account account)
        {
            var versioningSchemeService = new VersioningSchemeService(account);
            var customSchemeId = projectRequestModel.VersioningSchemeId;
            var versioningSchemeId = versioningSchemeService.FindSchemeIdForProject(customSchemeId);

            return await _versioningSchemeDao.FindAsync(versioningSchemeId);
        }

        private async Task SaveProjectAsync(IAddProjectOutputPort output, Project newProject)
        {
            await _projectDao
                .AddProjectAsync(newProject)
                .Match(Finish, c => output.Conflict(c));

            void Finish(Project x)
            {
                _unitOfWork.Commit();
                output.Created(x.AccountId, x.Id);
            }
        }
    }
}