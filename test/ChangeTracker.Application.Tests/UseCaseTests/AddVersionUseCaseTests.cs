using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.AddVersion;
using ChangeTracker.Domain;
using ChangeTracker.Domain.Version;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests
{
    public class AddVersionUseCaseTests
    {
        private readonly Mock<IAddVersionOutputPort> _outputPortMock;
        private readonly ProjectDaoStub _projectDaoStub;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly VersionDaoStub _versionDaoStub;

        public AddVersionUseCaseTests()
        {
            _projectDaoStub = new ProjectDaoStub();
            _versionDaoStub = new VersionDaoStub();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _outputPortMock = new Mock<IAddVersionOutputPort>(MockBehavior.Strict);
        }

        [Fact]
        public async Task CreateVersion_Successful()
        {
            // arrange
            _projectDaoStub.Projects.Add(TestAccount.Project);
            var createVersionDto = new VersionDto(TestAccount.Project.Id, "1.2.3");
            var createVersionUseCase =
                new AddVersionUseCase(_versionDaoStub, _projectDaoStub, _unitOfWorkMock.Object);

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
            _projectDaoStub.Projects.Add(TestAccount.Project);
            var createVersionDto = new VersionDto(TestAccount.Project.Id, "1. .3");
            var createVersionUseCase =
                new AddVersionUseCase(_versionDaoStub, _projectDaoStub, _unitOfWorkMock.Object);

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
                new AddVersionUseCase(_versionDaoStub, _projectDaoStub, _unitOfWorkMock.Object);

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
            _projectDaoStub.Projects.Add(new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, DateTime.Parse("2021-04-04"), DateTime.Parse("2021-04-05")));

            var createVersionDto = new VersionDto(TestAccount.Project.Id, "12*");
            var createVersionUseCase =
                new AddVersionUseCase(_versionDaoStub, _projectDaoStub, _unitOfWorkMock.Object);

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
            _projectDaoStub.Projects.Add(new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, DateTime.Parse("2021-04-04"), DateTime.Parse("2021-04-05")));

            var versionId = Guid.Parse("1d7831d5-32fb-437f-a9d5-bf5a7dd34b10");
            var version = ClVersionValue.Parse("1.2");
            _versionDaoStub.Versions.Add(new ClVersion(versionId, TestAccount.Project.Id,
                version, DateTime.Parse("2021-04-12"),
                DateTime.Parse("2021-04-12"), DateTime.Parse("2021-04-12")));

            var createVersionDto = new VersionDto(TestAccount.Project.Id, version);
            var createVersionUseCase =
                new AddVersionUseCase(_versionDaoStub, _projectDaoStub, _unitOfWorkMock.Object);

            _outputPortMock.Setup(m => m.VersionAlreadyExists(It.IsAny<string>()));

            // act
            await createVersionUseCase.ExecuteAsync(_outputPortMock.Object, createVersionDto);

            // assert
            _outputPortMock.Verify(m => m.VersionAlreadyExists(It.Is<string>(x => x == version.Value)));
        }

        [Fact]
        public async Task CreateVersion_ConflictWhileSaving_ConflictOutput()
        {
            // arrange
            _projectDaoStub.Projects.Add(TestAccount.Project);
            var createVersionDto = new VersionDto(TestAccount.Project.Id, "1.2.3");
            var createVersionUseCase = new AddVersionUseCase(_versionDaoStub, _projectDaoStub, _unitOfWorkMock.Object);
            _versionDaoStub.ProduceConflict = true;
            _outputPortMock.Setup(m => m.Conflict(It.IsAny<string>()));

            // act
            await createVersionUseCase.ExecuteAsync(_outputPortMock.Object, createVersionDto);

            // assert
            _outputPortMock.Verify(m => m.Conflict(It.IsAny<string>()));
        }
    }
}