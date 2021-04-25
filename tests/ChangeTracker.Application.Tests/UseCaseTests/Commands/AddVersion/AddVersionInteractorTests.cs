using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.Command.AddVersion;
using ChangeTracker.Domain;
using ChangeTracker.Domain.Version;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.Commands.AddVersion
{
    public class AddVersionInteractorTests
    {
        private readonly Mock<IAddVersionOutputPort> _outputPortMock;
        private readonly ProjectDaoStub _projectDaoStub;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly VersionDaoStub _versionDaoStub;

        public AddVersionInteractorTests()
        {
            _projectDaoStub = new ProjectDaoStub();
            _versionDaoStub = new VersionDaoStub();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _outputPortMock = new Mock<IAddVersionOutputPort>(MockBehavior.Strict);
        }

        private AddVersionInteractor CreateInteractor() =>
            new(_versionDaoStub, _projectDaoStub, _unitOfWorkMock.Object);

        [Fact]
        public async Task CreateVersion_HappyPath_Successful()
        {
            // arrange
            _projectDaoStub.Projects.Add(TestAccount.Project);
            var versionRequestModel = new VersionRequestModel(TestAccount.Project.Id, "1.2.3");
            var createVersionInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.Created(It.IsAny<Guid>()));

            // act
            await createVersionInteractor.ExecuteAsync(_outputPortMock.Object, versionRequestModel);

            // assert
            _outputPortMock.Verify(m => m.Created(It.Is<Guid>(x => x != Guid.Empty)));
        }

        [Fact]
        public async Task CreateVersion_VersionWithWhitespaceInTheMiddle_InvalidVersionFormatOutput()
        {
            // arrange
            _projectDaoStub.Projects.Add(TestAccount.Project);
            var versionRequestModel = new VersionRequestModel(TestAccount.Project.Id, "1. .3");
            var createVersionInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.InvalidVersionFormat(It.IsAny<string>()));

            // act
            await createVersionInteractor.ExecuteAsync(_outputPortMock.Object, versionRequestModel);

            // assert
            _outputPortMock.Verify(m => m.InvalidVersionFormat(It.Is<string>(x => x == "1. .3")));
        }

        [Fact]
        public async Task CreateVersion_NoProjectExists_ProjectDoesNotExistOutput()
        {
            // arrange
            var versionRequestModel = new VersionRequestModel(TestAccount.Project.Id, "1.2.3");
            var createVersionInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.ProjectDoesNotExist());

            // act
            await createVersionInteractor.ExecuteAsync(_outputPortMock.Object, versionRequestModel);

            // assert
            _outputPortMock.Verify(m => m.ProjectDoesNotExist());
        }

        [Fact]
        public async Task CreateVersion_VersionSchemeMismatch_VersionDoesNotMatchSchemeOutput()
        {
            // arrange
            _projectDaoStub.Projects.Add(new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, DateTime.Parse("2021-04-04"), DateTime.Parse("2021-04-05")));

            var versionRequestModel = new VersionRequestModel(TestAccount.Project.Id, "12*");
            var createVersionInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.VersionDoesNotMatchScheme(It.IsAny<string>()));

            // act
            await createVersionInteractor.ExecuteAsync(_outputPortMock.Object, versionRequestModel);

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

            var versionRequestModel = new VersionRequestModel(TestAccount.Project.Id, version);
            var createVersionInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.VersionAlreadyExists(It.IsAny<string>()));

            // act
            await createVersionInteractor.ExecuteAsync(_outputPortMock.Object, versionRequestModel);

            // assert
            _outputPortMock.Verify(m => m.VersionAlreadyExists(It.Is<string>(x => x == version.Value)));
        }

        [Fact]
        public async Task CreateVersion_ConflictWhileSaving_ConflictOutput()
        {
            // arrange
            _projectDaoStub.Projects.Add(TestAccount.Project);
            var versionRequestModel = new VersionRequestModel(TestAccount.Project.Id, "1.2.3");
            var createVersionInteractor = CreateInteractor();
            _versionDaoStub.ProduceConflict = true;
            _outputPortMock.Setup(m => m.Conflict(It.IsAny<string>()));

            // act
            await createVersionInteractor.ExecuteAsync(_outputPortMock.Object, versionRequestModel);

            // assert
            _outputPortMock.Verify(m => m.Conflict(It.IsAny<string>()));
        }
    }
}