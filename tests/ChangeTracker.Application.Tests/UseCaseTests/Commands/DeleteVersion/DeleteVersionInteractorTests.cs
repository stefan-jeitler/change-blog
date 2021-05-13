using System;
using System.Threading.Tasks;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.Commands.DeleteVersion;
using ChangeTracker.Domain;
using ChangeTracker.Domain.Common;
using ChangeTracker.Domain.Version;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.Commands.DeleteVersion
{
    public class DeleteVersionInteractorTests
    {
        private readonly Mock<IDeleteVersionOutputPort> _outputPortMock;
        private readonly ProjectDaoStub _projectDaoStub;
        private readonly VersionDaoStub _versionDaoStub;

        public DeleteVersionInteractorTests()
        {
            _versionDaoStub = new VersionDaoStub();
            _projectDaoStub = new ProjectDaoStub();
            _outputPortMock = new Mock<IDeleteVersionOutputPort>(MockBehavior.Strict);
        }

        private DeleteVersionInteractor CreateInteractor() => new(_versionDaoStub, _projectDaoStub);

        [Fact]
        public async Task DeleteVersion_VersionDoesNotExist_VersionDoesNotExistOutput()
        {
            // arrange
            var notExistingVersionId = Guid.Parse("11cbae99-5cc0-4268-8d3f-31961822133c");
            var interactor = CreateInteractor();
            _outputPortMock.Setup(m => m.VersionDoesNotExist());

            // act
            await interactor.ExecuteAsync(_outputPortMock.Object, notExistingVersionId);

            // assert
            _outputPortMock.Verify(m => m.VersionDoesNotExist(), Times.Once);
        }

        [Fact]
        public async Task DeleteVersion_DeletedVersion_VersionAlreadyDeletedOutput()
        {
            // arrange
            var version = new ClVersion(TestAccount.Project.Id, ClVersionValue.Parse("1.23"), null,
                DateTime.Parse("2021-05-13"));
            var interactor = CreateInteractor();
            _outputPortMock.Setup(m => m.VersionAlreadyDeleted());
            _versionDaoStub.Versions.Add(version);

            // act
            await interactor.ExecuteAsync(_outputPortMock.Object, version.Id);

            // assert
            _outputPortMock.Verify(m => m.VersionAlreadyDeleted(), Times.Once);
        }

        [Fact]
        public async Task DeleteVersion_ReleasedVersion_VersionAlreadyReleasedOutput()
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
        public async Task DeleteVersion_ProjectClosedExist_ProjectClosedOutput()
        {
            // arrange
            var version = new ClVersion(TestAccount.Project.Id, ClVersionValue.Parse("1.23"));
            var interactor = CreateInteractor();
            _outputPortMock.Setup(m => m.ProjectClosed(It.IsAny<Guid>()));

            _projectDaoStub.Projects.Add(new Project(TestAccount.Project.Id, TestAccount.Id, Name.Parse("test project"),
                TestAccount.CustomVersioningScheme, TestAccount.UserId, DateTime.Parse("2021-05-13"), DateTime.Parse("2021-05-13")));
            _versionDaoStub.Versions.Add(version);

            // act
            await interactor.ExecuteAsync(_outputPortMock.Object, version.Id);

            // assert
            _outputPortMock.Verify(m => m.ProjectClosed(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task DeleteVersion_ConflictWhenDeleting_ConflictOutput()
        {
            // arrange
            var version = new ClVersion(TestAccount.Project.Id, ClVersionValue.Parse("1.23"));
            var interactor = CreateInteractor();
            _outputPortMock.Setup(m => m.Conflict(It.IsAny<string>()));

            _projectDaoStub.Projects.Add(TestAccount.Project);
            _versionDaoStub.Versions.Add(version);
            _versionDaoStub.ProduceConflict = true;

            // act
            await interactor.ExecuteAsync(_outputPortMock.Object, version.Id);

            // assert
            _outputPortMock.Verify(m => m.Conflict(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task DeleteVersion_HappyPath_VersionDeletedOutput()
        {
            // arrange
            var version = new ClVersion(TestAccount.Project.Id, ClVersionValue.Parse("1.23"));
            var interactor = CreateInteractor();
            _outputPortMock.Setup(m => m.VersionDeleted(It.IsAny<Guid>()));

            _projectDaoStub.Projects.Add(TestAccount.Project);
            _versionDaoStub.Versions.Add(version);

            // act
            await interactor.ExecuteAsync(_outputPortMock.Object, version.Id);

            // assert
            _outputPortMock.Verify(m => m.VersionDeleted(It.Is<Guid>(x => x == version.Id)), Times.Once);
        }
    }
}