using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.CreateProject;
using ChangeTracker.Domain;
using ChangeTracker.Domain.Version;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests
{
    public class CreateProjectUseCaseTests
    {
        private readonly AccountDaoMock _accountDaoMock;
        private readonly Mock<ICreateProjectOutputPort> _outputPortMock;
        private readonly ProjectDaoMock _projectDaoMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly VersioningSchemeDaoMock _versioningSchemeDaoMock;

        public CreateProjectUseCaseTests()
        {
            _accountDaoMock = new AccountDaoMock();
            _versioningSchemeDaoMock = new VersioningSchemeDaoMock();
            _projectDaoMock = new ProjectDaoMock();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _outputPortMock = new Mock<ICreateProjectOutputPort>(MockBehavior.Strict);
        }

        [Fact]
        public async Task CreateProject_Successful()
        {
            // arrange
            var account = new Account(TestAccount.Id, TestAccount.Name, null, TestAccount.CreationDate, null);
            _accountDaoMock.Account = account;
            _versioningSchemeDaoMock.VersioningScheme = TestAccount.CustomVersioningScheme;
            var createProjectUseCase = new CreateProjectUseCase(_accountDaoMock,
                _versioningSchemeDaoMock,
                _projectDaoMock,
                _unitOfWorkMock.Object);
            var createProjectDto = new CreateProjectDto(TestAccount.Id, TestAccount.Name.Value,
                TestAccount.CustomVersioningScheme.Id);

            _outputPortMock.Setup(m => m.Created(It.IsAny<Guid>(), It.IsAny<Guid>()));

            // act
            await createProjectUseCase.ExecuteAsync(_outputPortMock.Object, createProjectDto);

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
            var createProjectUseCase = new CreateProjectUseCase(_accountDaoMock,
                _versioningSchemeDaoMock,
                _projectDaoMock,
                _unitOfWorkMock.Object);
            var createProjectDto = new CreateProjectDto(TestAccount.Id, TestAccount.Name.Value, null);
            _outputPortMock.Setup(m => m.AccountDoesNotExist());

            // act
            await createProjectUseCase.ExecuteAsync(_outputPortMock.Object, createProjectDto);

            // assert
            _outputPortMock.Verify(m => m.AccountDoesNotExist(), Times.Once);
        }

        [Fact]
        public async Task CreateProject_DeletedAccount_AccountDoesNotExistsOutput()
        {
            // arrange
            var deletedAccount = new Account(TestAccount.Id, TestAccount.Name, null, TestAccount.CreationDate,
                DateTime.Parse("2021-04-04"));
            _accountDaoMock.Account = deletedAccount;
            var createProjectUseCase = new CreateProjectUseCase(_accountDaoMock,
                _versioningSchemeDaoMock,
                _projectDaoMock,
                _unitOfWorkMock.Object);

            var createProjectDto = new CreateProjectDto(TestAccount.Id, TestAccount.Name.Value, null);
            _outputPortMock.Setup(m => m.AccountDeleted(It.IsAny<Guid>()));

            // act
            await createProjectUseCase.ExecuteAsync(_outputPortMock.Object, createProjectDto);

            // assert
            _outputPortMock.Verify(m
                => m.AccountDeleted(It.Is<Guid>(x => x == TestAccount.Id)), Times.Once);
        }

        [Fact]
        public async Task CreateProject_InvalidName_InvalidNameOutput()
        {
            // arrange
            _accountDaoMock.Account =
                new Account(TestAccount.Id, TestAccount.Name, null, TestAccount.CreationDate, null);
            var createProjectUseCase = new CreateProjectUseCase(_accountDaoMock,
                _versioningSchemeDaoMock,
                _projectDaoMock,
                _unitOfWorkMock.Object);

            var createProjectDto = new CreateProjectDto(TestAccount.Id, null, null);
            _outputPortMock.Setup(m => m.InvalidName(It.IsAny<string>()));

            // act
            await createProjectUseCase.ExecuteAsync(_outputPortMock.Object, createProjectDto);

            // assert
            _outputPortMock.Verify(m => m.InvalidName(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task CreateProject_ProjectExists_InvalidNameOutput()
        {
            // arrange
            _accountDaoMock.Account =
                new Account(TestAccount.Id, TestAccount.Name, null, TestAccount.CreationDate, null);
            _projectDaoMock.Projects.Add(new Project(TestAccount.Id, TestAccount.Name, Defaults.VersioningScheme,
                DateTime.Parse("2021-04-04")));
            var createProjectUseCase = new CreateProjectUseCase(_accountDaoMock,
                _versioningSchemeDaoMock,
                _projectDaoMock,
                _unitOfWorkMock.Object);

            var createProjectDto = new CreateProjectDto(TestAccount.Id, TestAccount.Name.Value, null);
            _outputPortMock.Setup(m => m.ProjectAlreadyExists());

            // act
            await createProjectUseCase.ExecuteAsync(_outputPortMock.Object, createProjectDto);

            // assert
            _outputPortMock.Verify(m => m.ProjectAlreadyExists(), Times.Once);
        }

        [Fact]
        public async Task CreateProject_NotExistingVersioningScheme_VersioningSchemeDoesNotExistOutput()
        {
            // arrange
            _accountDaoMock.Account =
                new Account(TestAccount.Id, TestAccount.Name, null, TestAccount.CreationDate, null);
            var createProjectUseCase = new CreateProjectUseCase(_accountDaoMock,
                _versioningSchemeDaoMock,
                _projectDaoMock,
                _unitOfWorkMock.Object);

            var notExistingVersioningSchemeId = Guid.Parse("3984bcf2-9930-4d41-984e-b72ccc6d6c87");
            var createProjectDto =
                new CreateProjectDto(TestAccount.Id, TestAccount.Name.Value, notExistingVersioningSchemeId);
            _outputPortMock.Setup(m => m.VersioningSchemeDoesNotExist());

            // act
            await createProjectUseCase.ExecuteAsync(_outputPortMock.Object, createProjectDto);

            // assert
            _outputPortMock.Verify(m => m.VersioningSchemeDoesNotExist(), Times.Once);
        }

        [Fact]
        public async Task CreateProject_ConflictWhenSaving_ConflictOutput()
        {
            // arrange
            _accountDaoMock.Account =
                new Account(TestAccount.Id, TestAccount.Name, null, TestAccount.CreationDate, null);

            _versioningSchemeDaoMock.VersioningScheme = TestAccount.CustomVersioningScheme;
            _projectDaoMock.ProduceConflict = true;

            var createProjectUseCase = new CreateProjectUseCase(_accountDaoMock, _versioningSchemeDaoMock,
                _projectDaoMock, _unitOfWorkMock.Object);

            var createProjectDto = new CreateProjectDto(TestAccount.Id, TestAccount.Name.Value,
                TestAccount.CustomVersioningScheme.Id);

            _outputPortMock.Setup(m => m.Conflict(It.IsAny<Conflict>()));

            // act
            await createProjectUseCase.ExecuteAsync(_outputPortMock.Object, createProjectDto);

            // assert
            _outputPortMock.Verify(m => m.Conflict(It.IsAny<Conflict>()), Times.Once);
        }
    }
}