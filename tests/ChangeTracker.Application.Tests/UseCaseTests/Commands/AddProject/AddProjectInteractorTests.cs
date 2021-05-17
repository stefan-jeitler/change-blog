using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.Commands.AddProject;
using ChangeTracker.Domain;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.Commands.AddProject
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

        private AddProjectInteractor CreateInteractor()
        {
            return new(_accountDaoStub,
                _versioningSchemeDaoStub,
                _projectDaoStub,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task CreateProject_Successful()
        {
            // arrange
            var account = new Account(TestAccount.Id, TestAccount.Name, null, TestAccount.CreationDate, null);
            _accountDaoStub.Accounts.Add(account);
            _versioningSchemeDaoStub.VersioningSchemes.Add(TestAccount.CustomVersioningScheme);
            var projectRequestModel = new ProjectRequestModel(TestAccount.Id, TestAccount.Name.Value,
                TestAccount.CustomVersioningScheme.Id, TestAccount.UserId);
            var createProjectInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.Created(It.IsAny<Guid>(), It.IsAny<Guid>()));

            // act
            await createProjectInteractor.ExecuteAsync(_outputPortMock.Object, projectRequestModel);

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
            var projectRequestModel =
                new ProjectRequestModel(TestAccount.Id, TestAccount.Name.Value, null, TestAccount.UserId);
            var createProjectInteractor = CreateInteractor();
            _outputPortMock.Setup(m => m.AccountDoesNotExist());

            // act
            await createProjectInteractor.ExecuteAsync(_outputPortMock.Object, projectRequestModel);

            // assert
            _outputPortMock.Verify(m => m.AccountDoesNotExist(), Times.Once);
        }

        [Fact]
        public async Task CreateProject_DeletedAccount_AccountDeletedOutput()
        {
            // arrange
            var deletedAccount = new Account(TestAccount.Id, TestAccount.Name, null, TestAccount.CreationDate,
                DateTime.Parse("2021-04-04"));
            _accountDaoStub.Accounts.Add(deletedAccount);

            var projectRequestModel =
                new ProjectRequestModel(TestAccount.Id, TestAccount.Name.Value, null, TestAccount.UserId);
            var createProjectInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.AccountDeleted(It.IsAny<Guid>()));

            // act
            await createProjectInteractor.ExecuteAsync(_outputPortMock.Object, projectRequestModel);

            // assert
            _outputPortMock.Verify(m
                => m.AccountDeleted(It.Is<Guid>(x => x == TestAccount.Id)), Times.Once);
        }

        [Fact]
        public async Task CreateProject_InvalidName_InvalidNameOutput()
        {
            // arrange
            _accountDaoStub.Accounts.Add(new Account(TestAccount.Id, TestAccount.Name, null, TestAccount.CreationDate,
                null));

            var projectRequestModel = new ProjectRequestModel(TestAccount.Id, "", null, TestAccount.UserId);
            var createProjectInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.InvalidName(It.IsAny<string>()));

            // act
            await createProjectInteractor.ExecuteAsync(_outputPortMock.Object, projectRequestModel);

            // assert
            _outputPortMock.Verify(m => m.InvalidName(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task CreateProject_ProjectExists_ProjectAlreadyExistsOutput()
        {
            // arrange
            _accountDaoStub.Accounts.Add(new Account(TestAccount.Id, TestAccount.Name, null, TestAccount.CreationDate,
                null));
            _projectDaoStub.Projects.Add(new Project(TestAccount.Id, TestAccount.Name,
                TestAccount.Project.VersioningScheme, TestAccount.UserId,
                DateTime.Parse("2021-04-04")));

            var projectRequestModel =
                new ProjectRequestModel(TestAccount.Id, TestAccount.Name.Value, null, TestAccount.UserId);
            var createProjectInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.ProjectAlreadyExists(It.IsAny<Guid>()));

            // act
            await createProjectInteractor.ExecuteAsync(_outputPortMock.Object, projectRequestModel);

            // assert
            _outputPortMock.Verify(m => m.ProjectAlreadyExists(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task CreateProject_NotExistingVersioningScheme_VersioningSchemeDoesNotExistOutput()
        {
            // arrange
            _accountDaoStub.Accounts.Add(new Account(TestAccount.Id, TestAccount.Name, null, TestAccount.CreationDate,
                null));
            var notExistingVersioningSchemeId = Guid.Parse("3984bcf2-9930-4d41-984e-b72ccc6d6c87");

            var projectRequestModel =
                new ProjectRequestModel(TestAccount.Id, TestAccount.Name.Value, notExistingVersioningSchemeId,
                    TestAccount.UserId);
            var createProjectInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.VersioningSchemeDoesNotExist());

            // act
            await createProjectInteractor.ExecuteAsync(_outputPortMock.Object, projectRequestModel);

            // assert
            _outputPortMock.Verify(m => m.VersioningSchemeDoesNotExist(), Times.Once);
        }

        [Fact]
        public async Task CreateProject_ConflictWhenSaving_ConflictOutput()
        {
            // arrange
            _accountDaoStub.Accounts.Add(new Account(TestAccount.Id, TestAccount.Name, null, TestAccount.CreationDate,
                null));

            _versioningSchemeDaoStub.VersioningSchemes.Add(TestAccount.CustomVersioningScheme);
            _projectDaoStub.ProduceConflict = true;

            var projectRequestModel = new ProjectRequestModel(TestAccount.Id, TestAccount.Name.Value,
                TestAccount.CustomVersioningScheme.Id, TestAccount.UserId);
            var createProjectInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.Conflict(It.IsAny<string>()));

            // act
            await createProjectInteractor.ExecuteAsync(_outputPortMock.Object, projectRequestModel);

            // assert
            _outputPortMock.Verify(m => m.Conflict(It.IsAny<string>()), Times.Once);
        }
    }
}