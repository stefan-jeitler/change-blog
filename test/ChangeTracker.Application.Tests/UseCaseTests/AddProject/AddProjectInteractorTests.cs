using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.AddProject;
using ChangeTracker.Domain;
using ChangeTracker.Domain.Version;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.AddProject
{
    public class AddProjectInteractorTests
    {
        private readonly AccountDaoStub _accountDaoStub;
        private readonly Mock<IAddProjectOutputPort> _outputPortMock;
        private readonly ProjectDaoStub _projectDaoStub;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly VersioningSchemeDaoStub _versioningSchemeDaoStub;

        public AddProjectInteractorTests()
        {
            _accountDaoStub = new AccountDaoStub();
            _versioningSchemeDaoStub = new VersioningSchemeDaoStub();
            _projectDaoStub = new ProjectDaoStub();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _outputPortMock = new Mock<IAddProjectOutputPort>(MockBehavior.Strict);
        }

        [Fact]
        public async Task CreateProject_Successful()
        {
            // arrange
            var account = new Account(TestAccount.Id, TestAccount.Name, null, TestAccount.CreationDate, null);
            _accountDaoStub.Account = account;
            _versioningSchemeDaoStub.VersioningScheme = TestAccount.CustomVersioningScheme;
            var createProjectUseCase = new AddProjectInteractor(_accountDaoStub,
                _versioningSchemeDaoStub,
                _projectDaoStub,
                _unitOfWorkMock.Object);
            var projectRequestModel = new ProjectRequestModel(TestAccount.Id, TestAccount.Name.Value,
                TestAccount.CustomVersioningScheme.Id);

            _outputPortMock.Setup(m => m.Created(It.IsAny<Guid>(), It.IsAny<Guid>()));

            // act
            await createProjectUseCase.ExecuteAsync(_outputPortMock.Object, projectRequestModel);

            // assert
            _outputPortMock.Verify(
                m => m.Created(
                    It.Is<Guid>(x => x == TestAccount.Id),
                    It.IsAny<Guid>()), Times.Once);

            _unitOfWorkMock.Verify(m => m.Start(), Times.Once);
            _unitOfWorkMock.Verify(m => m.Commit(), Times.Once);
        }

        [Fact]
        public async Task CreateProject_NotExistingAccount_AccountDoesNotExistsOutput()
        {
            // arrange
            var createProjectUseCase = new AddProjectInteractor(_accountDaoStub,
                _versioningSchemeDaoStub,
                _projectDaoStub,
                _unitOfWorkMock.Object);
            var projectRequestModel = new ProjectRequestModel(TestAccount.Id, TestAccount.Name.Value, null);
            _outputPortMock.Setup(m => m.AccountDoesNotExist());

            // act
            await createProjectUseCase.ExecuteAsync(_outputPortMock.Object, projectRequestModel);

            // assert
            _outputPortMock.Verify(m => m.AccountDoesNotExist(), Times.Once);
        }

        [Fact]
        public async Task CreateProject_DeletedAccount_AccountDoesNotExistsOutput()
        {
            // arrange
            var deletedAccount = new Account(TestAccount.Id, TestAccount.Name, null, TestAccount.CreationDate,
                DateTime.Parse("2021-04-04"));
            _accountDaoStub.Account = deletedAccount;
            var createProjectUseCase = new AddProjectInteractor(_accountDaoStub,
                _versioningSchemeDaoStub,
                _projectDaoStub,
                _unitOfWorkMock.Object);

            var projectRequestModel = new ProjectRequestModel(TestAccount.Id, TestAccount.Name.Value, null);
            _outputPortMock.Setup(m => m.AccountDeleted(It.IsAny<Guid>()));

            // act
            await createProjectUseCase.ExecuteAsync(_outputPortMock.Object, projectRequestModel);

            // assert
            _outputPortMock.Verify(m
                => m.AccountDeleted(It.Is<Guid>(x => x == TestAccount.Id)), Times.Once);
        }

        [Fact]
        public async Task CreateProject_InvalidName_InvalidNameOutput()
        {
            // arrange
            _accountDaoStub.Account =
                new Account(TestAccount.Id, TestAccount.Name, null, TestAccount.CreationDate, null);
            var createProjectUseCase = new AddProjectInteractor(_accountDaoStub,
                _versioningSchemeDaoStub,
                _projectDaoStub,
                _unitOfWorkMock.Object);

            var projectRequestModel = new ProjectRequestModel(TestAccount.Id, "", null);
            _outputPortMock.Setup(m => m.InvalidName(It.IsAny<string>()));

            // act
            await createProjectUseCase.ExecuteAsync(_outputPortMock.Object, projectRequestModel);

            // assert
            _outputPortMock.Verify(m => m.InvalidName(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task CreateProject_ProjectExists_InvalidNameOutput()
        {
            // arrange
            _accountDaoStub.Account =
                new Account(TestAccount.Id, TestAccount.Name, null, TestAccount.CreationDate, null);
            _projectDaoStub.Projects.Add(new Project(TestAccount.Id, TestAccount.Name, Defaults.VersioningScheme,
                DateTime.Parse("2021-04-04")));
            var createProjectUseCase = new AddProjectInteractor(_accountDaoStub,
                _versioningSchemeDaoStub,
                _projectDaoStub,
                _unitOfWorkMock.Object);

            var projectRequestModel = new ProjectRequestModel(TestAccount.Id, TestAccount.Name.Value, null);
            _outputPortMock.Setup(m => m.ProjectAlreadyExists());

            // act
            await createProjectUseCase.ExecuteAsync(_outputPortMock.Object, projectRequestModel);

            // assert
            _outputPortMock.Verify(m => m.ProjectAlreadyExists(), Times.Once);
        }

        [Fact]
        public async Task CreateProject_NotExistingVersioningScheme_VersioningSchemeDoesNotExistOutput()
        {
            // arrange
            _accountDaoStub.Account =
                new Account(TestAccount.Id, TestAccount.Name, null, TestAccount.CreationDate, null);
            var createProjectUseCase = new AddProjectInteractor(_accountDaoStub,
                _versioningSchemeDaoStub,
                _projectDaoStub,
                _unitOfWorkMock.Object);

            var notExistingVersioningSchemeId = Guid.Parse("3984bcf2-9930-4d41-984e-b72ccc6d6c87");
            var projectRequestModel =
                new ProjectRequestModel(TestAccount.Id, TestAccount.Name.Value, notExistingVersioningSchemeId);
            _outputPortMock.Setup(m => m.VersioningSchemeDoesNotExist());

            // act
            await createProjectUseCase.ExecuteAsync(_outputPortMock.Object, projectRequestModel);

            // assert
            _outputPortMock.Verify(m => m.VersioningSchemeDoesNotExist(), Times.Once);
        }

        [Fact]
        public async Task CreateProject_ConflictWhenSaving_ConflictOutput()
        {
            // arrange
            _accountDaoStub.Account =
                new Account(TestAccount.Id, TestAccount.Name, null, TestAccount.CreationDate, null);

            _versioningSchemeDaoStub.VersioningScheme = TestAccount.CustomVersioningScheme;
            _projectDaoStub.ProduceConflict = true;

            var createProjectUseCase = new AddProjectInteractor(_accountDaoStub, _versioningSchemeDaoStub,
                _projectDaoStub, _unitOfWorkMock.Object);

            var projectRequestModel = new ProjectRequestModel(TestAccount.Id, TestAccount.Name.Value,
                TestAccount.CustomVersioningScheme.Id);

            _outputPortMock.Setup(m => m.Conflict(It.IsAny<string>()));

            // act
            await createProjectUseCase.ExecuteAsync(_outputPortMock.Object, projectRequestModel);

            // assert
            _outputPortMock.Verify(m => m.Conflict(It.IsAny<string>()), Times.Once);
        }
    }
}