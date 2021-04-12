using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.CreateVersion;
using ChangeTracker.Domain;
using ChangeTracker.Domain.Version;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests
{
    public class CreateVersionUseCaseTests
    {
        private readonly Mock<ICreateVersionOutputPort> _outputPortMock;
        private readonly ProjectDaoMock _projectDaoMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly VersionDaoMock _versionDaoMock;

        public CreateVersionUseCaseTests()
        {
            _projectDaoMock = new ProjectDaoMock();
            _versionDaoMock = new VersionDaoMock();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _outputPortMock = new Mock<ICreateVersionOutputPort>(MockBehavior.Strict);
        }

        [Fact]
        public async Task CreateVersion_Successful()
        {
            // arrange
            _projectDaoMock.Projects.Add(TestAccount.Project);
            var createVersionDto = new VersionDto(TestAccount.Project.Id, "1.2.3");
            var createVersionUseCase =
                new CreateVersionUseCase(_versionDaoMock, _projectDaoMock, _unitOfWorkMock.Object);

            _outputPortMock.Setup(m => m.Created(It.IsAny<Guid>()));

            // act
            await createVersionUseCase.ExecuteAsync(_outputPortMock.Object, createVersionDto);

            // assert
            _outputPortMock.Verify(m => m.Created(It.Is<Guid>(x => x != Guid.Empty)));
        }

        [Fact]
        public async Task CreateVersion_VersionWithWhitespaceInTheMiddle_InvalidVersionFormatOutput()
        {
            // arrange
            _projectDaoMock.Projects.Add(TestAccount.Project);
            var createVersionDto = new VersionDto(TestAccount.Project.Id, "1. .3");
            var createVersionUseCase =
                new CreateVersionUseCase(_versionDaoMock, _projectDaoMock, _unitOfWorkMock.Object);

            _outputPortMock.Setup(m => m.InvalidVersionFormat(It.IsAny<string>()));

            // act
            await createVersionUseCase.ExecuteAsync(_outputPortMock.Object, createVersionDto);

            // assert
            _outputPortMock.Verify(m => m.InvalidVersionFormat(It.Is<string>(x => x == "1. .3")));
        }

        [Fact]
        public async Task CreateVersion_NoProjectExists_ProjectDoesNotExistOutput()
        {
            // arrange
            var createVersionDto = new VersionDto(TestAccount.Project.Id, "1.2.3");
            var createVersionUseCase =
                new CreateVersionUseCase(_versionDaoMock, _projectDaoMock, _unitOfWorkMock.Object);

            _outputPortMock.Setup(m => m.ProjectDoesNotExist());

            // act
            await createVersionUseCase.ExecuteAsync(_outputPortMock.Object, createVersionDto);

            // assert
            _outputPortMock.Verify(m => m.ProjectDoesNotExist());
        }

        [Fact]
        public async Task CreateVersion_WithInvalidVersionValue_VersionSchemeMismatchOutput()
        {
            // arrange
            _projectDaoMock.Projects.Add(new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, DateTime.Parse("2021-04-04"), DateTime.Parse("2021-04-05")));

            var createVersionDto = new VersionDto(TestAccount.Project.Id, "12*");
            var createVersionUseCase =
                new CreateVersionUseCase(_versionDaoMock, _projectDaoMock, _unitOfWorkMock.Object);

            _outputPortMock.Setup(m => m.VersionDoesNotMatchScheme(It.IsAny<string>()));

            // act
            await createVersionUseCase.ExecuteAsync(_outputPortMock.Object, createVersionDto);

            // assert
            _outputPortMock.Verify(m => m.VersionDoesNotMatchScheme(It.Is<string>(x => x == "12*")));
        }

        [Fact]
        public async Task CreateVersion_VersionAlreadyExists_VersionAlreadyExistsOutput()
        {
            // arrange
            _projectDaoMock.Projects.Add(new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, DateTime.Parse("2021-04-04"), DateTime.Parse("2021-04-05")));

            var versionId = Guid.Parse("1d7831d5-32fb-437f-a9d5-bf5a7dd34b10");
            var version = ClVersionValue.Parse("1.2");
            _versionDaoMock.Versions.Add(new ClVersion(versionId, TestAccount.Project.Id, 
                version, DateTime.Parse("2021-04-12"),
                DateTime.Parse("2021-04-12"), DateTime.Parse("2021-04-12")));

            var createVersionDto = new VersionDto(TestAccount.Project.Id, version);
            var createVersionUseCase =
                new CreateVersionUseCase(_versionDaoMock, _projectDaoMock, _unitOfWorkMock.Object);

            _outputPortMock.Setup(m => m.VersionAlreadyExists(It.IsAny<string>()));

            // act
            await createVersionUseCase.ExecuteAsync(_outputPortMock.Object, createVersionDto);

            // assert
            _outputPortMock.Verify(m => m.VersionAlreadyExists(It.Is<string>(x => x == version.Value)));
        }
    }
}