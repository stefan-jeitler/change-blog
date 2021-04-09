using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.AddChangeLogLine;
using ChangeTracker.Domain;
using ChangeTracker.Domain.ChangeLog;
using ChangeTracker.Domain.ChangeLog.Services;
using ChangeTracker.Domain.Version;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests
{
    public class AddChangeLogLineUseCaseTests
    {
        private readonly ChangeLogDaoMock _changeLogDaoMock;
        private readonly Mock<IAddChangeLogLineOutputPort> _outputPortMock;
        private readonly ProjectDaoMock _projectDaoMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly VersionDaoMock _versionDaoMock;

        public AddChangeLogLineUseCaseTests()
        {
            _projectDaoMock = new ProjectDaoMock();
            _versionDaoMock = new VersionDaoMock();
            _changeLogDaoMock = new ChangeLogDaoMock();
            _outputPortMock = new Mock<IAddChangeLogLineOutputPort>(MockBehavior.Strict);
            _unitOfWorkMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
        }

        [Fact]
        public async Task AddChangeLogLine_InvalidChangeLogLabel_InvalidLabelsOutput()
        {
            // arrange
            const string changeLogLine = "Bug fixed.";
            var labels = new List<string> {"Bugfix", "ProxyIssue", "invalid label"};
            var issues = new List<string> {"#1234", "#12345"};
            var changeLogLineDto =
                new AddChangeLogLineDto(TestAccount.Project.Id, "1.2", changeLogLine, labels, issues);

            _projectDaoMock.Projects.Add(new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, TestAccount.CreationDate, null));

            _versionDaoMock.VersionInfo.Add(new ClVersionInfo(TestAccount.Project.Id, ClVersion.Parse("1.2")));

            var addChangeLogLineUseCase =
                new AddChangeLogLineUseCase(_projectDaoMock, _versionDaoMock, _changeLogDaoMock, _unitOfWorkMock.Object);

            _outputPortMock.Setup(m => m.InvalidLabels(It.IsAny<List<string>>()));

            // act
            await addChangeLogLineUseCase.ExecuteAsync(_outputPortMock.Object, changeLogLineDto);

            // assert
            _outputPortMock.Verify(m
                => m.InvalidLabels(
                    It.Is<List<string>>(x => x.Count == 1 &&
                                             x.First() == "invalid label")), Times.Once);
        }

        [Fact]
        public async Task AddChangeLogLine_TooManyChangeLogLabels_TooManyLabelsOutput()
        {
            // arrange
            const string changeLogLine = "Bug fixed.";
            var labels = new List<string>
                {"Bugfix", "ProxyIssue", "Security", "ProxyStrikesBack", "Deprecated", "Feature"};
            var issues = new List<string> {"#1234", "#12345"};
            var changeLogLineDto =
                new AddChangeLogLineDto(TestAccount.Project.Id, "1.2", changeLogLine, labels, issues);

            _projectDaoMock.Projects.Add(new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, TestAccount.CreationDate, null));

            _versionDaoMock.VersionInfo.Add(new ClVersionInfo(TestAccount.Project.Id, ClVersion.Parse("1.2")));

            var addChangeLogLineUseCase =
                new AddChangeLogLineUseCase(_projectDaoMock, _versionDaoMock, _changeLogDaoMock, _unitOfWorkMock.Object);

            _outputPortMock.Setup(m => m.TooManyLabels(It.IsAny<int>()));

            // act
            await addChangeLogLineUseCase.ExecuteAsync(_outputPortMock.Object, changeLogLineDto);

            // assert
            _outputPortMock.Verify(m
                => m.TooManyLabels(It.Is<int>(x => x == ChangeLogLine.MaxLabels)), Times.Once);
        }

        [Fact]
        public async Task AddChangeLogLine_InvalidChangeLogIssue_InvalidIssuesOutput()
        {
            // arrange
            const string changeLogLine = "Bug fixed.";
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            var issues = new List<string> {"#1234", "# 345"};
            var changeLogLineDto =
                new AddChangeLogLineDto(TestAccount.Project.Id, "1.2", changeLogLine, labels, issues);

            _projectDaoMock.Projects.Add(new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, TestAccount.CreationDate, null));

            _versionDaoMock.VersionInfo.Add(new ClVersionInfo(TestAccount.Project.Id, ClVersion.Parse("1.2")));

            var addChangeLogLineUseCase =
                new AddChangeLogLineUseCase(_projectDaoMock, _versionDaoMock, _changeLogDaoMock, _unitOfWorkMock.Object);

            _outputPortMock.Setup(m => m.InvalidIssues(It.IsAny<List<string>>()));

            // act
            await addChangeLogLineUseCase.ExecuteAsync(_outputPortMock.Object, changeLogLineDto);

            // assert
            _outputPortMock.Verify(m
                => m.InvalidIssues(It.Is<List<string>>(x => x.Count == 1 &&
                                                            x.First() == "# 345")), Times.Once);
        }

        [Fact]
        public async Task AddChangeLogLine_TooManyChangeLogIssues_TooManyIssuesOutput()
        {
            // arrange
            const string changeLogLine = "Bug fixed.";
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            var issues = new List<string> {"#1", "#2", "#3", "#4", "#5", "#6", "#7", "#8", "#9", "#10", "#11"};
            var changeLogLineDto =
                new AddChangeLogLineDto(TestAccount.Project.Id, "1.2", changeLogLine, labels, issues);

            _projectDaoMock.Projects.Add(new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, TestAccount.CreationDate, null));

            _versionDaoMock.VersionInfo.Add(new ClVersionInfo(TestAccount.Project.Id, ClVersion.Parse("1.2")));

            var addChangeLogLineUseCase =
                new AddChangeLogLineUseCase(_projectDaoMock, _versionDaoMock, _changeLogDaoMock, _unitOfWorkMock.Object);

            _outputPortMock.Setup(m => m.TooManyIssues(It.IsAny<int>()));

            // act
            await addChangeLogLineUseCase.ExecuteAsync(_outputPortMock.Object, changeLogLineDto);

            // assert
            _outputPortMock.Verify(m
                => m.TooManyIssues(It.Is<int>(x => x == ChangeLogLine.MaxIssues)), Times.Once);
        }

        [Fact]
        public async Task AddChangeLogLine_NotExistingProject_ProjectDoesNotExistOutput()
        {
            // arrange
            const string changeLogLine = "Bug fixed.";
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            var issues = new List<string> {"#1234", "#12345"};
            var addChangeLogLineDto =
                new AddChangeLogLineDto(TestAccount.Project.Id, "1.2", changeLogLine, labels, issues);

            var addChangeLogLineUseCase =
                new AddChangeLogLineUseCase(_projectDaoMock, _versionDaoMock, _changeLogDaoMock, _unitOfWorkMock.Object);

            _outputPortMock.Setup(m => m.ProjectDoesNotExist());
            _unitOfWorkMock.Setup(m => m.Start());

            // act
            await addChangeLogLineUseCase.ExecuteAsync(_outputPortMock.Object, addChangeLogLineDto);

            // assert
            _outputPortMock.Verify(m => m.ProjectDoesNotExist(), Times.Once);
        }

        [Fact]
        public async Task AddChangeLogLine_InvalidVersion_InvalidVersionFormatOutput()
        {
            // arrange
            const string changeLogLine = "Bug fixed.";
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            var issues = new List<string> {"#1234", "#12345"};
            var changeLogLineDto =
                new AddChangeLogLineDto(TestAccount.Project.Id, "1. .3", changeLogLine, labels, issues);

            _projectDaoMock.Projects.Add(new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, TestAccount.CreationDate, null));

            var addChangeLogLineUseCase =
                new AddChangeLogLineUseCase(_projectDaoMock, _versionDaoMock, _changeLogDaoMock, _unitOfWorkMock.Object);

            _outputPortMock.Setup(m => m.InvalidVersionFormat());
            _unitOfWorkMock.Setup(m => m.Start());

            // act
            await addChangeLogLineUseCase.ExecuteAsync(_outputPortMock.Object, changeLogLineDto);

            // assert
            _outputPortMock.Verify(m => m.InvalidVersionFormat(), Times.Once);
        }

        [Fact]
        public async Task AddChangeLogLine_VersionDoesNotExist_VersionDoesNotExistOutput()
        {
            // arrange
            const string changeLogLine = "Bug fixed.";
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            var issues = new List<string> {"#1234", "#12345"};
            var changeLogLineDto =
                new AddChangeLogLineDto(TestAccount.Project.Id, "1.23", changeLogLine, labels, issues);

            _projectDaoMock.Projects.Add(new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, TestAccount.CreationDate, null));

            var addChangeLogLineUseCase =
                new AddChangeLogLineUseCase(_projectDaoMock, _versionDaoMock, _changeLogDaoMock, _unitOfWorkMock.Object);

            _outputPortMock.Setup(m => m.VersionDoesNotExist());
            _unitOfWorkMock.Setup(m => m.Start());

            // act
            await addChangeLogLineUseCase.ExecuteAsync(_outputPortMock.Object, changeLogLineDto);

            // assert
            _outputPortMock.Verify(m => m.VersionDoesNotExist(), Times.Once);
        }

        [Fact]
        public async Task AddChangeLogLine_InvalidLineText_InvalidChangeLogLineOutput()
        {
            // arrange
            const string changeLogLine = "a";
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            var issues = new List<string> {"#1234", "#12345"};
            var changeLogLineDto =
                new AddChangeLogLineDto(TestAccount.Project.Id, "1.2", changeLogLine, labels, issues);

            _projectDaoMock.Projects.Add(new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, TestAccount.CreationDate, null));

            _versionDaoMock.VersionInfo.Add(new ClVersionInfo(TestAccount.Project.Id, ClVersion.Parse("1.2")));
            var addChangeLogLineUseCase =
                new AddChangeLogLineUseCase(_projectDaoMock, _versionDaoMock, _changeLogDaoMock, _unitOfWorkMock.Object);

            _outputPortMock.Setup(m => m.InvalidChangeLogLine(It.IsAny<string>()));
            _unitOfWorkMock.Setup(m => m.Start());

            // act
            await addChangeLogLineUseCase.ExecuteAsync(_outputPortMock.Object, changeLogLineDto);

            // assert
            _outputPortMock.Verify(m
                => m.InvalidChangeLogLine(It.Is<string>(x => x == "a")), Times.Once);
        }

        [Fact]
        public async Task AddChangeLogLine_NoMoreLinesAvailable_TooManyChangeLogLinesOutput()
        {
            // arrange
            const string changeLogLine = "Some Bug fixed";
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            var issues = new List<string> {"#1234", "#12345"};
            var changeLogLineDto =
                new AddChangeLogLineDto(TestAccount.Project.Id, "1.2", changeLogLine, labels, issues);

            _projectDaoMock.Projects.Add(new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, TestAccount.CreationDate, null));

            var versionInfo = new ClVersionInfo(TestAccount.Project.Id, ClVersion.Parse("1.2"));
            _versionDaoMock.VersionInfo.Add(versionInfo);

            _changeLogDaoMock.ChangeLog.AddRange(Enumerable.Range(0, 100)
                .Select(x => new ChangeLogLine(Guid.NewGuid(),
                    versionInfo.Id,
                    TestAccount.Project.Id,
                    ChangeLogText.Parse($"{x:D5}"),
                    (uint) x,
                    DateTime.Parse("2021-04-09"))));

            var addChangeLogLineUseCase =
                new AddChangeLogLineUseCase(_projectDaoMock, _versionDaoMock, _changeLogDaoMock, _unitOfWorkMock.Object);

            _outputPortMock.Setup(m => m.MaxChangeLogLinesReached(It.IsAny<int>()));
            _unitOfWorkMock.Setup(m => m.Start());

            // act
            await addChangeLogLineUseCase.ExecuteAsync(_outputPortMock.Object, changeLogLineDto);

            // assert
            _outputPortMock.Verify(m => m.MaxChangeLogLinesReached(
                It.Is<int>(x => x == ChangeLogInfo.MaxChangeLogLines)), Times.Once);
        }

        [Fact]
        public async Task AddChangeLogLine_ToAlreadyReleasedVersion_VersionAlreadyReleasedOutput()
        {
            // arrange
            const string changeLogLine = "Some Bug fixed";
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            var issues = new List<string> {"#1234", "#12345"};
            var changeLogLineDto =
                new AddChangeLogLineDto(TestAccount.Project.Id, "1.2", changeLogLine, labels, issues);

            _projectDaoMock.Projects.Add(new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, TestAccount.CreationDate, null));

            var releasedAt = DateTime.Parse("2021-04-09");
            var versionInfo = new ClVersionInfo(TestAccount.Project.Id, ClVersion.Parse("1.2"), releasedAt);
            _versionDaoMock.VersionInfo.Add(versionInfo);

            _changeLogDaoMock.ChangeLog.Add(new ChangeLogLine(Guid.NewGuid(),
                versionInfo.Id,
                TestAccount.Project.Id,
                ChangeLogText.Parse("some-release"),
                0,
                DateTime.Parse("2021-04-09")));

            var addChangeLogLineUseCase =
                new AddChangeLogLineUseCase(_projectDaoMock, _versionDaoMock, _changeLogDaoMock, _unitOfWorkMock.Object);

            _outputPortMock.Setup(m => m.VersionAlreadyReleased(It.IsAny<DateTime>()));
            _unitOfWorkMock.Setup(m => m.Start());

            // act
            await addChangeLogLineUseCase.ExecuteAsync(_outputPortMock.Object, changeLogLineDto);

            // assert
            _outputPortMock.Verify(m => m.VersionAlreadyReleased(
                It.Is<DateTime>(x => x == releasedAt)), Times.Once);
        }

        [Fact]
        public async Task AddChangeLogLine_ToDeletedVersion_VersionDeletedOutput()
        {
            // arrange
            const string changeLogLine = "Some Bug fixed";
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            var issues = new List<string> {"#1234", "#12345"};
            var changeLogLineDto =
                new AddChangeLogLineDto(TestAccount.Project.Id, "1.2", changeLogLine, labels, issues);

            var deletedAt = DateTime.Parse("2021-04-09");
            _projectDaoMock.Projects.Add(new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, TestAccount.CreationDate, null));

            var versionInfo = new ClVersionInfo(Guid.NewGuid(),
                TestAccount.Project.Id,
                ClVersion.Parse("1.2"),
                null,
                DateTime.Parse("2021-04-09T12:00"),
                deletedAt);
            _versionDaoMock.VersionInfo.Add(versionInfo);

            _changeLogDaoMock.ChangeLog.Add(new ChangeLogLine(Guid.NewGuid(),
                versionInfo.Id,
                TestAccount.Project.Id,
                ChangeLogText.Parse("some-release"),
                0,
                DateTime.Parse("2021-04-09")));

            var addChangeLogLineUseCase =
                new AddChangeLogLineUseCase(_projectDaoMock, _versionDaoMock, _changeLogDaoMock, _unitOfWorkMock.Object);

            _outputPortMock.Setup(m => m.VersionDeleted(It.IsAny<DateTime>()));
            _unitOfWorkMock.Setup(m => m.Start());

            // act
            await addChangeLogLineUseCase.ExecuteAsync(_outputPortMock.Object, changeLogLineDto);

            // assert
            _outputPortMock.Verify(m => m.VersionDeleted(
                It.Is<DateTime>(x => x == deletedAt)), Times.Once);
        }

        [Fact]
        public async Task AddChangeLogLine_ConflictWhileSaving_ConflictOutput()
        {
            // arrange
            const string changeLogLine = "Some Bug fixed";
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            var issues = new List<string> {"#1234", "#12345"};
            var changeLogLineDto =
                new AddChangeLogLineDto(TestAccount.Project.Id, "1.2", changeLogLine, labels, issues);

            _projectDaoMock.Projects.Add(new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, TestAccount.CreationDate, null));

            var versionInfo = new ClVersionInfo(TestAccount.Project.Id, ClVersion.Parse("1.2"));
            _versionDaoMock.VersionInfo.Add(versionInfo);

            _changeLogDaoMock.ProduceConflict = true;
            _changeLogDaoMock.ChangeLog.Add(new ChangeLogLine(Guid.NewGuid(),
                versionInfo.Id,
                TestAccount.Project.Id,
                ChangeLogText.Parse("some-release"),
                0,
                DateTime.Parse("2021-04-09")));

            var addChangeLogLineUseCase =
                new AddChangeLogLineUseCase(_projectDaoMock, _versionDaoMock, _changeLogDaoMock, _unitOfWorkMock.Object);

            _outputPortMock.Setup(m => m.Conflict(It.IsAny<string>()));
            _unitOfWorkMock.Setup(m => m.Start());

            // act
            await addChangeLogLineUseCase.ExecuteAsync(_outputPortMock.Object, changeLogLineDto);

            // assert
            _outputPortMock.Verify(m => m.Conflict(It.IsAny<string>()), Times.Once);
            _unitOfWorkMock.Verify(m => m.Start(), Times.Once);
        }

        [Fact]
        public async Task AddChangeLogLine_ValidLine_CreatedOutputAndCommittedUow()
        {
            // arrange
            const string changeLogLine = "Some Bug fixed";
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            var issues = new List<string> {"#1234", "#12345"};
            var changeLogLineDto =
                new AddChangeLogLineDto(TestAccount.Project.Id, "1.2", changeLogLine, labels, issues);

            _projectDaoMock.Projects.Add(new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, TestAccount.CreationDate, null));

            var versionId = Guid.Parse("1d7831d5-32fb-437f-a9d5-bf5a7dd34b10");
            var versionInfo = new ClVersionInfo(versionId,
                TestAccount.Project.Id, 
                ClVersion.Parse("1.2"),
                null,
                DateTime.Parse("2021-04-09"),
                null);
            _versionDaoMock.VersionInfo.Add(versionInfo);

            _changeLogDaoMock.ChangeLog.Add(new ChangeLogLine(Guid.NewGuid(),
                versionInfo.Id,
                TestAccount.Project.Id,
                ChangeLogText.Parse("some-release"),
                0,
                DateTime.Parse("2021-04-09")));

            var addChangeLogLineUseCase =
                new AddChangeLogLineUseCase(_projectDaoMock, _versionDaoMock, _changeLogDaoMock, _unitOfWorkMock.Object);

            _outputPortMock.Setup(m => m.Created(It.IsAny<Guid>()));
            _unitOfWorkMock.Setup(m => m.Start());
            _unitOfWorkMock.Setup(m => m.Commit());

            // act
            await addChangeLogLineUseCase.ExecuteAsync(_outputPortMock.Object, changeLogLineDto);

            // assert
            _outputPortMock.Verify(m => m.Created(It.IsAny<Guid>()), Times.Once);
            _unitOfWorkMock.Verify(m => m.Start(), Times.Once);
            _unitOfWorkMock.Verify(m => m.Commit(), Times.Once);
        }
    }
}