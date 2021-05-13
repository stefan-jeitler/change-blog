using System;
using System.Threading.Tasks;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.Commands.ReleaseVersion;
using ChangeTracker.Domain;
using ChangeTracker.Domain.Version;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.Commands.ReleaseVersion
{
    public class ReleaseVersionInteractorTests
    {
        private readonly Mock<IReleaseVersionOutputPort> _outputPortMock;
        private readonly ProjectDaoStub _projectDaoStub;
        private readonly VersionDaoStub _versionDaoStub;

        public ReleaseVersionInteractorTests()
        {
            _versionDaoStub = new VersionDaoStub();
            _projectDaoStub = new ProjectDaoStub();
            _outputPortMock = new Mock<IReleaseVersionOutputPort>(MockBehavior.Strict);
        }

        private ReleaseVersionInteractor CreateInteractor() => new(_versionDaoStub, _projectDaoStub);

        [Fact]
        public async Task ReleaseVersion_NotExistingVersion_VersionDoesNotExistOutput()
        {
            // arrange
            var notExistingVersion = Guid.Parse("7a9fef4d-cf90-4eaf-9c7e-ee639b88939b");
            var interactor = CreateInteractor();
            _outputPortMock.Setup(m => m.VersionDoesNotExist());

            // act
            await interactor.ExecuteAsync(_outputPortMock.Object, notExistingVersion);

            // assert
            _outputPortMock.Verify(m => m.VersionDoesNotExist(), Times.Once);
        }

        [Fact]
        public async Task ReleaseVersion_DeletedVersion_VersionDeletedOutput()
        {
            // arrange
            var version = new ClVersion(TestAccount.Project.Id, ClVersionValue.Parse("1.23"), null,
                DateTime.Parse("2021-05-13"));
            var interactor = CreateInteractor();
            _outputPortMock.Setup(m => m.VersionDeleted());
            _versionDaoStub.Versions.Add(version);

            // act
            await interactor.ExecuteAsync(_outputPortMock.Object, version.Id);

            // assert
            _outputPortMock.Verify(m => m.VersionDeleted(), Times.Once);
        }

        [Fact]
        public async Task ReleaseVersion_ReleasedVersion_VersionAlreadyReleasedOutput()
        {
            // arrange
            var version = new ClVersion(TestAccount.Project.Id, ClVersionValue.Parse("1.23"),
                DateTime.Parse("2021-05-13"));
            var interactor = CreateInteractor();
            _outputPortMock.Setup(m => m.VersionAlreadyReleased());
            _versionDaoStub.Versions.Add(version);

            // act
            await interactor.ExecuteAsync(_outputPortMock.Object, version.Id);

            // assert
            _outputPortMock.Verify(m => m.VersionAlreadyReleased(), Times.Once);
        }

        [Fact]
        public async Task ReleaseVersion_RelatedProjectClosed_RelatedProjectClosedOutput()
        {
            // arrange
            var version = new ClVersion(TestAccount.Project.Id, ClVersionValue.Parse("1.23"));
            var project = new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, TestAccount.UserId, TestAccount.Project.CreatedAt, DateTime.Parse("2021-05-13"));
            var interactor = CreateInteractor();
            _outputPortMock.Setup(m => m.RelatedProjectClosed(It.IsAny<Guid>()));
            _versionDaoStub.Versions.Add(version);
            _projectDaoStub.Projects.Add(project);

            // act
            await interactor.ExecuteAsync(_outputPortMock.Object, version.Id);

            // assert
            _outputPortMock.Verify(m => m.RelatedProjectClosed(It.Is<Guid>(x => x == project.Id)), Times.Once);
        }

        [Fact]
        public async Task ReleaseVersion_ConflictWhenRelease_ConflictOutput()
        {
            // arrange
            var version = new ClVersion(TestAccount.Project.Id, ClVersionValue.Parse("1.23"));
            var project = new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, TestAccount.UserId, TestAccount.Project.CreatedAt, null);
            var interactor = CreateInteractor();
            _outputPortMock.Setup(m => m.Conflict(It.IsAny<string>()));
            _versionDaoStub.Versions.Add(version);
            _versionDaoStub.ProduceConflict = true;
            _projectDaoStub.Projects.Add(project);

            // act
            await interactor.ExecuteAsync(_outputPortMock.Object, version.Id);

            // assert
            _outputPortMock.Verify(m => m.Conflict(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ReleaseVersion_HappyPath_ReleasedOutput()
        {
            // arrange
            var version = new ClVersion(TestAccount.Project.Id, ClVersionValue.Parse("1.23"));
            var project = new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, TestAccount.UserId, TestAccount.Project.CreatedAt, null);
            var interactor = CreateInteractor();
            _outputPortMock.Setup(m => m.VersionReleased(It.IsAny<Guid>()));
            _versionDaoStub.Versions.Add(version);
            _projectDaoStub.Projects.Add(project);

            // act
            await interactor.ExecuteAsync(_outputPortMock.Object, version.Id);

            // assert
            _outputPortMock.Verify(m => m.VersionReleased(It.Is<Guid>(x => x == version.Id)), Times.Once);
        }
    }
}