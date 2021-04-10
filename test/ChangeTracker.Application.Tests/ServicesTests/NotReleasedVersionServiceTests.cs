using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.Services.NotReleasedVersion;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.AddChangeLogLine;
using ChangeTracker.Domain;
using ChangeTracker.Domain.ChangeLog;
using ChangeTracker.Domain.Version;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.ServicesTests
{
    public class NotReleasedVersionServiceTests
    {
        private readonly ChangeLogDaoMock _changeLogDaoMock;
        private readonly Mock<INotReleasedVersionOutputPort> _outputPortMock;
        private readonly ProjectDaoMock _projectDaoMock;
        private readonly VersionDaoMock _versionDaoMock;


        public NotReleasedVersionServiceTests()
        {
            _projectDaoMock = new ProjectDaoMock();
            _versionDaoMock = new VersionDaoMock();
            _changeLogDaoMock = new ChangeLogDaoMock();
            _outputPortMock = new Mock<INotReleasedVersionOutputPort>(MockBehavior.Strict);
        }

        [Fact]
        public async Task Find_NotExistingProject_ProjectDoesNotExistOutput()
        {
            // arrange
            var notReleasedVersion = new NotReleasedVersionService(_projectDaoMock, _versionDaoMock);
            _outputPortMock.Setup(m => m.ProjectDoesNotExist());

            // act
            var version = await notReleasedVersion.FindAsync(_outputPortMock.Object, TestAccount.Project.Id, ClVersion.Parse("1.2"));

            // assert
            _outputPortMock.Verify(m => m.ProjectDoesNotExist(), Times.Once);
            version.HasValue.Should().BeFalse();
        }


        [Fact]
        public async Task Find_VersionDoesNotExist_VersionDoesNotExistOutput()
        {
            // arrange
            var projectId = TestAccount.Project.Id;
            _projectDaoMock.Projects.Add(new Project(projectId, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, TestAccount.CreationDate, null));

            var notReleasedVersion =
                    new NotReleasedVersionService(_projectDaoMock, _versionDaoMock);

            _outputPortMock.Setup(m => m.VersionDoesNotExist());

            // act
            var version = await notReleasedVersion.FindAsync(_outputPortMock.Object, projectId, ClVersion.Parse("1.23"));

            // assert
            _outputPortMock.Verify(m => m.VersionDoesNotExist(), Times.Once);
            version.HasValue.Should().BeFalse();
        }


        [Fact]
        public async Task Find_AlreadyReleasedVersion_VersionAlreadyReleasedOutput()
        {
            // arrange
            var projectId = TestAccount.Project.Id;
            _projectDaoMock.Projects.Add(new Project(projectId, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, TestAccount.CreationDate, null));

            var releasedAt = DateTime.Parse("2021-04-09");
            var versionInfo = new ClVersionInfo(projectId, ClVersion.Parse("1.2"), releasedAt);
            _versionDaoMock.VersionInfo.Add(versionInfo);

            _changeLogDaoMock.ChangeLog.Add(new ChangeLogLine(Guid.NewGuid(),
                versionInfo.Id,
                projectId,
                ChangeLogText.Parse("some-release"),
                0,
                DateTime.Parse("2021-04-09")));

            var notReleasedVersion = new NotReleasedVersionService(_projectDaoMock, _versionDaoMock);
            _outputPortMock.Setup(m => m.VersionAlreadyReleased(It.IsAny<DateTime>()));

            // act
            var version = await notReleasedVersion.FindAsync(_outputPortMock.Object, projectId, ClVersion.Parse("1.2"));

            // assert
            _outputPortMock.Verify(m => m.VersionAlreadyReleased(
                It.Is<DateTime>(x => x == releasedAt)), Times.Once);
            version.HasValue.Should().BeFalse();
        }

        [Fact]
        public async Task Find_DeletedVersion_VersionDeletedOutput()
        {
            // arrange
            var projectId = TestAccount.Project.Id;
            var deletedAt = DateTime.Parse("2021-04-09");
            _projectDaoMock.Projects.Add(new Project(projectId, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, TestAccount.CreationDate, null));

            var versionInfo = new ClVersionInfo(Guid.NewGuid(),
                projectId,
                ClVersion.Parse("1.2"),
                null,
                DateTime.Parse("2021-04-09T12:00"),
                deletedAt);
            _versionDaoMock.VersionInfo.Add(versionInfo);

            _changeLogDaoMock.ChangeLog.Add(new ChangeLogLine(Guid.NewGuid(),
                versionInfo.Id,
                projectId,
                ChangeLogText.Parse("some-release"),
                0,
                DateTime.Parse("2021-04-09")));

            var notReleaseVersion = new NotReleasedVersionService(_projectDaoMock, _versionDaoMock);

            _outputPortMock.Setup(m => m.VersionDeleted(It.IsAny<DateTime>()));

            // act
            var version = await notReleaseVersion.FindAsync(_outputPortMock.Object, projectId, ClVersion.Parse("1.2"));

            // assert
            _outputPortMock.Verify(m => m.VersionDeleted(
                It.Is<DateTime>(x => x == deletedAt)), Times.Once);
            version.HasValue.Should().BeFalse();
        }

        [Fact]
        public async Task Find_NotReleasedVersion_NoOutputAndReturnsVersion()
        {
            // arrange
            var projectId = TestAccount.Project.Id;
            var deletedAt = DateTime.Parse("2021-04-09");
            _projectDaoMock.Projects.Add(new Project(projectId, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, TestAccount.CreationDate, null));

            var versionInfo = new ClVersionInfo(Guid.NewGuid(),
                projectId,
                ClVersion.Parse("1.2"),
                null,
                DateTime.Parse("2021-04-09T12:00"),
                null);
            _versionDaoMock.VersionInfo.Add(versionInfo);

            _changeLogDaoMock.ChangeLog.Add(new ChangeLogLine(Guid.NewGuid(),
                versionInfo.Id,
                projectId,
                ChangeLogText.Parse("some-release"),
                0,
                DateTime.Parse("2021-04-09")));

            var notReleaseVersion = new NotReleasedVersionService(_projectDaoMock, _versionDaoMock);

            // act
            var version = await notReleaseVersion.FindAsync(_outputPortMock.Object, projectId, ClVersion.Parse("1.2"));

            // assert
            version.HasValue.Should().BeTrue();
            version.Value.Value.Should().Be(ClVersion.Parse("1.2"));
        }
    }
}