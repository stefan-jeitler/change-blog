﻿using System;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.AssignPendingLineToVersion;
using ChangeTracker.Application.UseCases.AssignPendingLineToVersion.DTOs;
using ChangeTracker.Domain;
using ChangeTracker.Domain.ChangeLog;
using ChangeTracker.Domain.Common;
using ChangeTracker.Domain.Version;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests
{
    public class AssignPendingLineUseCaseTests
    {
        private readonly ChangeLogDaoStub _changeLogDaoStub;
        private readonly Mock<IAssignPendingLineOutputPort> _outputPortMock;
        private readonly ProjectDaoStub _projectDaoStub;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly VersionDaoStub _versionDaoStub;


        public AssignPendingLineUseCaseTests()
        {
            _projectDaoStub = new ProjectDaoStub();
            _versionDaoStub = new VersionDaoStub();
            _changeLogDaoStub = new ChangeLogDaoStub();
            _outputPortMock = new Mock<IAssignPendingLineOutputPort>(MockBehavior.Strict);
            _unitOfWorkMock = new Mock<IUnitOfWork>();
        }

        [Fact]
        public async Task AssignPendingLineByVersionId_HappyPath_SuccessfullyAssigned()
        {
            // arrange
            _projectDaoStub.Projects.Add(new Project(TestAccount.Id,
                Name.Parse("Test Project"),
                TestAccount.CustomVersioningScheme,
                TestAccount.CreationDate));

            var clVersion = new ClVersion(TestAccount.Project.Id, ClVersionValue.Parse("1.2"));
            _versionDaoStub.Versions.Add(clVersion);

            var line = new ChangeLogLine(Guid.NewGuid(), null, TestAccount.Project.Id,
                ChangeLogText.Parse("Some new changes"), 0, DateTime.Parse("2021-04-12"));
            _changeLogDaoStub.ChangeLogs.Add(line);

            var pendingLineDto = new PendingLineByVersionIdDto(TestAccount.Project.Id, clVersion.Id, line.Id);
            var assignToVersionUseCase =
                new AssignPendingLineUseCase(_versionDaoStub, _changeLogDaoStub, _unitOfWorkMock.Object);

            _outputPortMock.Setup(m => m.Assigned(It.IsAny<Guid>(), It.IsAny<Guid>()));

            // act
            await assignToVersionUseCase.ExecuteAsync(_outputPortMock.Object, pendingLineDto);

            // assert
            _changeLogDaoStub.ChangeLogs.Single().VersionId.Value.Should().Be(clVersion.Id);
            _outputPortMock.Verify(m => m.Assigned(It.Is<Guid>(x => x == clVersion.Id), It.Is<Guid>(x => x == line.Id)),
                Times.Once);
            _unitOfWorkMock.Verify(m => m.Start(), Times.Once);
            _unitOfWorkMock.Verify(m => m.Commit(), Times.Once);
        }

        [Fact]
        public async Task AssignPendingLineByVersionValue_InvalidVersionFormat_InvalidVersionFormatOutput()
        {
            // arrange
            _projectDaoStub.Projects.Add(new Project(TestAccount.Id,
                Name.Parse("Test Project"),
                TestAccount.CustomVersioningScheme,
                TestAccount.CreationDate));

            var line = new ChangeLogLine(Guid.NewGuid(), null, TestAccount.Project.Id,
                ChangeLogText.Parse("Some new changes"), 0, DateTime.Parse("2021-04-12"));
            _changeLogDaoStub.ChangeLogs.Add(line);

            var pendingLineDto = new PendingLineByVersionDto(TestAccount.Project.Id, "1. .2", line.Id);
            var assignToVersionUseCase =
                new AssignPendingLineUseCase(_versionDaoStub, _changeLogDaoStub, _unitOfWorkMock.Object);

            _outputPortMock.Setup(m => m.InvalidVersionFormat(It.IsAny<string>()));

            // act
            await assignToVersionUseCase.ExecuteAsync(_outputPortMock.Object, pendingLineDto);

            // assert
            _outputPortMock.Verify(m => m.InvalidVersionFormat(It.Is<string>(x => x == "1. .2")), Times.Once);
        }

        [Fact]
        public async Task AssignPendingLineByVersionValue_NotExistingVersion_VersionDoesNotExistOutput()
        {
            // arrange
            _projectDaoStub.Projects.Add(new Project(TestAccount.Id,
                Name.Parse("Test Project"),
                TestAccount.CustomVersioningScheme,
                TestAccount.CreationDate));

            var clVersion = new ClVersion(TestAccount.Project.Id, ClVersionValue.Parse("1.2"));
            _versionDaoStub.Versions.Add(clVersion);

            var line = new ChangeLogLine(Guid.NewGuid(), null, TestAccount.Project.Id,
                ChangeLogText.Parse("Some new changes"), 0, DateTime.Parse("2021-04-12"));
            _changeLogDaoStub.ChangeLogs.Add(line);

            var pendingLineDto = new PendingLineByVersionDto(TestAccount.Project.Id, "1.3", line.Id);
            var assignToVersionUseCase =
                new AssignPendingLineUseCase(_versionDaoStub, _changeLogDaoStub, _unitOfWorkMock.Object);

            _outputPortMock.Setup(m => m.VersionDoesNotExist());

            // act
            await assignToVersionUseCase.ExecuteAsync(_outputPortMock.Object, pendingLineDto);

            // assert
            _outputPortMock.Verify(m => m.VersionDoesNotExist(), Times.Once);
        }

        [Fact]
        public async Task AssignPendingLine_NotExistingVersion_VersionDoesNotExistOutput()
        {
            // arrange
            _projectDaoStub.Projects.Add(new Project(TestAccount.Id,
                Name.Parse("Test Project"),
                TestAccount.CustomVersioningScheme,
                TestAccount.CreationDate));

            var versionId = Guid.Parse("1d7831d5-32fb-437f-a9d5-bf5a7dd34b10");

            var line = new ChangeLogLine(Guid.NewGuid(), null, TestAccount.Project.Id,
                ChangeLogText.Parse("Some new changes"), 0, DateTime.Parse("2021-04-12"));
            _changeLogDaoStub.ChangeLogs.Add(line);

            var pendingLineDto = new PendingLineByVersionIdDto(TestAccount.Project.Id, versionId, line.Id);
            var assignToVersionUseCase =
                new AssignPendingLineUseCase(_versionDaoStub, _changeLogDaoStub, _unitOfWorkMock.Object);

            _outputPortMock.Setup(m => m.VersionDoesNotExist());

            // act
            await assignToVersionUseCase.ExecuteAsync(_outputPortMock.Object, pendingLineDto);

            // assert
            _outputPortMock.Verify(m => m.VersionDoesNotExist(), Times.Once);
        }

        [Fact]
        public async Task AssignPendingLine_NotExistingChangeLogLine_ChangeLogLineDoesNotExistOutput()
        {
            // arrange
            _projectDaoStub.Projects.Add(new Project(TestAccount.Id,
                Name.Parse("Test Project"),
                TestAccount.CustomVersioningScheme,
                TestAccount.CreationDate));

            var clVersion = new ClVersion(TestAccount.Project.Id, ClVersionValue.Parse("1.2"));
            _versionDaoStub.Versions.Add(clVersion);

            var lineId = Guid.Parse("2b4b147a-9ebd-4350-a45b-aaae5d8d63de");

            var pendingLineDto = new PendingLineByVersionIdDto(TestAccount.Project.Id, clVersion.Id, lineId);
            var assignToVersionUseCase =
                new AssignPendingLineUseCase(_versionDaoStub, _changeLogDaoStub, _unitOfWorkMock.Object);

            _outputPortMock.Setup(m => m.ChangeLogLineDoesNotExist());

            // act
            await assignToVersionUseCase.ExecuteAsync(_outputPortMock.Object, pendingLineDto);

            // assert
            _outputPortMock.Verify(m => m.ChangeLogLineDoesNotExist(), Times.Once);
        }

        [Fact]
        public async Task AssignPendingLine_TooManyLinesAssigned_ChangeLogLineDoesNotExistOutput()
        {
            // arrange
            _projectDaoStub.Projects.Add(new Project(TestAccount.Id,
                Name.Parse("Test Project"),
                TestAccount.CustomVersioningScheme,
                TestAccount.CreationDate));

            var clVersion = new ClVersion(TestAccount.Project.Id, ClVersionValue.Parse("1.2"));
            _versionDaoStub.Versions.Add(clVersion);

            var lineId = Guid.Parse("2b4b147a-9ebd-4350-a45b-aaae5d8d63de");

            _changeLogDaoStub.ChangeLogs.AddRange(Enumerable.Range(0, 100).Select(x =>
                new ChangeLogLine(clVersion.Id, TestAccount.Project.Id, ChangeLogText.Parse($"{x:D5}"), (uint) x)));

            var pendingLineDto = new PendingLineByVersionIdDto(TestAccount.Project.Id, clVersion.Id, lineId);
            var assignToVersionUseCase =
                new AssignPendingLineUseCase(_versionDaoStub, _changeLogDaoStub, _unitOfWorkMock.Object);

            _outputPortMock.Setup(m => m.MaxChangeLogLinesReached(It.IsAny<int>()));

            // act
            await assignToVersionUseCase.ExecuteAsync(_outputPortMock.Object, pendingLineDto);

            // assert
            _outputPortMock.Verify(
                m => m.MaxChangeLogLinesReached(It.Is<int>(x => x == ChangeLogsMetadata.MaxChangeLogLines)),
                Times.Once);
        }

        [Fact]
        public async Task AssignPendingLine_ConflictWhileSaving_ConflictOutput()
        {
            // arrange
            _projectDaoStub.Projects.Add(new Project(TestAccount.Id,
                Name.Parse("Test Project"),
                TestAccount.CustomVersioningScheme,
                TestAccount.CreationDate));

            var clVersion = new ClVersion(TestAccount.Project.Id, ClVersionValue.Parse("1.2"));
            _versionDaoStub.Versions.Add(clVersion);

            var line = new ChangeLogLine(Guid.NewGuid(), null, TestAccount.Project.Id,
                ChangeLogText.Parse("Some new changes"), 0, DateTime.Parse("2021-04-12"));
            _changeLogDaoStub.ChangeLogs.Add(line);

            _changeLogDaoStub.ProduceConflict = true;

            var pendingLineDto = new PendingLineByVersionIdDto(TestAccount.Project.Id, clVersion.Id, line.Id);
            var assignToVersionUseCase =
                new AssignPendingLineUseCase(_versionDaoStub, _changeLogDaoStub, _unitOfWorkMock.Object);

            _outputPortMock.Setup(m => m.Conflict(It.IsAny<string>()));

            // act
            await assignToVersionUseCase.ExecuteAsync(_outputPortMock.Object, pendingLineDto);

            // assert
            _unitOfWorkMock.Verify(m => m.Commit(), Times.Never);
            _outputPortMock.Verify(m => m.Conflict(It.IsAny<string>()));
        }
    }
}