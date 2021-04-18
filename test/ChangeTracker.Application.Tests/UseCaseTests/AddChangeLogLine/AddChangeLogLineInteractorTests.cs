﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.AddChangeLogLine;
using ChangeTracker.Domain;
using ChangeTracker.Domain.ChangeLog;
using ChangeTracker.Domain.Version;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.AddChangeLogLine
{
    public class AddChangeLogLineInteractorTests
    {
        private readonly ChangeLogDaoStub _changeLogDaoStub;
        private readonly Mock<IAddLineOutputPort> _outputPortMock;
        private readonly ProjectDaoStub _projectDaoStub;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly VersionDaoStub _versionDaoStub;

        public AddChangeLogLineInteractorTests()
        {
            _projectDaoStub = new ProjectDaoStub();
            _versionDaoStub = new VersionDaoStub();
            _changeLogDaoStub = new ChangeLogDaoStub();
            _outputPortMock = new Mock<IAddLineOutputPort>(MockBehavior.Strict);
            _unitOfWorkMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
        }

        private AddChangeLogLineInteractor CreateInteractor() => new(_changeLogDaoStub, _unitOfWorkMock.Object,
            _versionDaoStub, _projectDaoStub);

        [Fact]
        public async Task AddChangeLogLine_InvalidVersion_InvalidVersionFormatOutput()
        {
            // arrange
            const string changeLogLine = "Bug fixed.";
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            var issues = new List<string> {"#1234", "#12345"};
            var changeLogLineRequestModel =
                new ChangeLogLineRequestModel(TestAccount.Project.Id, "1. .3", changeLogLine, labels, issues);

            _projectDaoStub.Projects.Add(new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, TestAccount.CreationDate, null));

            var addLineInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.InvalidVersionFormat());
            _unitOfWorkMock.Setup(m => m.Start());

            // act
            await addLineInteractor.ExecuteAsync(_outputPortMock.Object, changeLogLineRequestModel);

            // assert
            _outputPortMock.Verify(m => m.InvalidVersionFormat(), Times.Once);
        }

        [Fact]
        public async Task AddChangeLogLine_NotExistingProject_ProjectDoesNotExistOutput()
        {
            // arrange
            const string changeLogLine = "Bug fixed.";
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            var issues = new List<string> {"#1234", "#12345"};
            var changeLogLineRequestModel =
                new ChangeLogLineRequestModel(TestAccount.Project.Id, "1.2.3", changeLogLine, labels, issues);

            var addLineInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.ProjectDoesNotExist());

            // act
            await addLineInteractor.ExecuteAsync(_outputPortMock.Object, changeLogLineRequestModel);

            // assert
            _outputPortMock.Verify(m => m.ProjectDoesNotExist(), Times.Once);
        }

        [Fact]
        public async Task AddChangeLogLine_ConflictWhileSaving_ConflictOutput()
        {
            // arrange
            const string changeLogLine = "Some Bug fixed";
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            var issues = new List<string> {"#1234", "#12345"};
            var changeLogLineRequestModel =
                new ChangeLogLineRequestModel(TestAccount.Project.Id, "1.2", changeLogLine, labels, issues);

            _projectDaoStub.Projects.Add(new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, TestAccount.CreationDate, null));

            var version = new ClVersion(TestAccount.Project.Id, ClVersionValue.Parse("1.2"));
            _versionDaoStub.Versions.Add(version);

            _changeLogDaoStub.ProduceConflict = true;

            var addLineInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.Conflict(It.IsAny<string>()));
            _unitOfWorkMock.Setup(m => m.Start());

            // act
            await addLineInteractor.ExecuteAsync(_outputPortMock.Object, changeLogLineRequestModel);

            // assert
            _outputPortMock.Verify(m => m.Conflict(It.IsAny<string>()), Times.Once);
            _unitOfWorkMock.Verify(m => m.Start(), Times.Once);
        }

        [Fact]
        public async Task AddChangeLogLine_NotExistingVersion_VersionDoesNotExistOutput()
        {
            // arrange
            const string changeLogLine = "Some Bug fixed";
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            var issues = new List<string> {"#1234", "#12345"};
            var changeLogLineRequestModel =
                new ChangeLogLineRequestModel(TestAccount.Project.Id, "1.2", changeLogLine, labels, issues);

            _projectDaoStub.Projects.Add(new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, TestAccount.CreationDate, null));

            var addLineInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.VersionDoesNotExist(It.IsAny<string>()));
            _unitOfWorkMock.Setup(m => m.Start());

            // act
            await addLineInteractor.ExecuteAsync(_outputPortMock.Object, changeLogLineRequestModel);

            // assert
            _outputPortMock.Verify(m => m.VersionDoesNotExist(It.Is<string>(x => x == "1.2")), Times.Once);
        }

        [Fact]
        public async Task AddChangeLogLine_ValidLine_CreatedOutputAndCommittedUow()
        {
            // arrange
            const string changeLogLine = "Some Bug fixed";
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            var issues = new List<string> {"#1234", "#12345"};
            var changeLogLineRequestModel =
                new ChangeLogLineRequestModel(TestAccount.Project.Id, "1.2", changeLogLine, labels, issues);

            _projectDaoStub.Projects.Add(new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, TestAccount.CreationDate, null));

            var versionId = Guid.Parse("1d7831d5-32fb-437f-a9d5-bf5a7dd34b10");
            var version = new ClVersion(versionId,
                TestAccount.Project.Id,
                ClVersionValue.Parse("1.2"),
                null,
                DateTime.Parse("2021-04-09"),
                null);
            _versionDaoStub.Versions.Add(version);

            var addLineInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.Created(It.IsAny<Guid>()));
            _unitOfWorkMock.Setup(m => m.Start());
            _unitOfWorkMock.Setup(m => m.Commit());

            // act
            await addLineInteractor.ExecuteAsync(_outputPortMock.Object, changeLogLineRequestModel);

            // assert
            _outputPortMock.Verify(m => m.Created(It.IsAny<Guid>()), Times.Once);
            _unitOfWorkMock.Verify(m => m.Start(), Times.Once);
            _unitOfWorkMock.Verify(m => m.Commit(), Times.Once);
        }

        [Fact]
        public async Task AddChangeLogLine_MaxLinesReached_TooManyLinesOutput()
        {
            // arrange
            const string changeLogLine = "Some Bug fixed";
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            var issues = new List<string> {"#1234", "#12345"};
            var changeLogLineRequestModel =
                new ChangeLogLineRequestModel(TestAccount.Project.Id, "1.2", changeLogLine, labels, issues);

            _projectDaoStub.Projects.Add(new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, TestAccount.CreationDate, null));

            var versionId = Guid.Parse("1d7831d5-32fb-437f-a9d5-bf5a7dd34b10");
            var version = new ClVersion(versionId,
                TestAccount.Project.Id, ClVersionValue.Parse("1.2"), null, DateTime.Parse("2021-04-09"), null);
            _versionDaoStub.Versions.Add(version);

            _changeLogDaoStub.ChangeLogs.AddRange(Enumerable.Range(0, 100)
                .Select(x =>
                    new ChangeLogLine(versionId, TestAccount.Project.Id, ChangeLogText.Parse($"{x:D5}"), (uint) x)));

            var addLineInteractor = CreateInteractor();
            _unitOfWorkMock.Setup(m => m.Start());
            _outputPortMock.Setup(m => m.TooManyLines(It.IsAny<int>()));

            // act
            await addLineInteractor.ExecuteAsync(_outputPortMock.Object, changeLogLineRequestModel);

            // assert
            _outputPortMock.Verify(m => m.TooManyLines(It.Is<int>(x => x == ChangeLogsMetadata.MaxChangeLogLines)),
                Times.Once);
        }

        [Fact]
        public async Task AddChangeLogLine_VersionAlreadyReleased_VersionAlreadyReleasedOutput()
        {
            // arrange
            const string changeLogLine = "Some Bug fixed";
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            var issues = new List<string> {"#1234", "#12345"};
            var changeLogLineRequestModel =
                new ChangeLogLineRequestModel(TestAccount.Project.Id, "1.2", changeLogLine, labels, issues);

            _projectDaoStub.Projects.Add(new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, TestAccount.CreationDate, null));

            var versionId = Guid.Parse("1d7831d5-32fb-437f-a9d5-bf5a7dd34b10");
            var version = new ClVersion(versionId,
                TestAccount.Project.Id,
                ClVersionValue.Parse("1.2"),
                DateTime.Parse("2021-04-09"),
                DateTime.Parse("2021-04-09"),
                null);
            _versionDaoStub.Versions.Add(version);

            var addLineInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.RelatedVersionAlreadyReleased(It.IsAny<string>()));
            _unitOfWorkMock.Setup(m => m.Start());

            // act
            await addLineInteractor.ExecuteAsync(_outputPortMock.Object, changeLogLineRequestModel);

            // assert
            _outputPortMock.Verify(m => m.RelatedVersionAlreadyReleased(It.Is<string>(x => x == "1.2")), Times.Once);
        }

        [Fact]
        public async Task AddChangeLogLine_VersionDeleted_VersionDeletedOutput()
        {
            // arrange
            const string changeLogLine = "Some Bug fixed";
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            var issues = new List<string> {"#1234", "#12345"};
            var changeLogLineRequestModel =
                new ChangeLogLineRequestModel(TestAccount.Project.Id, "1.2", changeLogLine, labels, issues);

            _projectDaoStub.Projects.Add(new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, TestAccount.CreationDate, null));

            var versionId = Guid.Parse("1d7831d5-32fb-437f-a9d5-bf5a7dd34b10");
            var version = new ClVersion(versionId,
                TestAccount.Project.Id,
                ClVersionValue.Parse("1.2"),
                null,
                DateTime.Parse("2021-04-09"),
                DateTime.Parse("2021-04-09"));
            _versionDaoStub.Versions.Add(version);

            var addLineInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.RelatedVersionDeleted(It.IsAny<string>()));
            _unitOfWorkMock.Setup(m => m.Start());

            // act
            await addLineInteractor.ExecuteAsync(_outputPortMock.Object, changeLogLineRequestModel);

            // assert
            _outputPortMock.Verify(m => m.RelatedVersionDeleted(It.Is<string>(x => x == "1.2")), Times.Once);
        }

        [Fact]
        public async Task AddChangeLogLine_ValidLine_ProperlySaved()
        {
            // arrange
            const string changeLogLine = "Some Bug fixed";
            var labels = new List<string> {"Bugfix"};
            var issues = new List<string> {"#1234", "#12345"};
            var changeLogLineRequestModel =
                new ChangeLogLineRequestModel(TestAccount.Project.Id, "1.2", changeLogLine, labels, issues);

            _projectDaoStub.Projects.Add(new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, TestAccount.CreationDate, null));

            var versionId = Guid.Parse("1d7831d5-32fb-437f-a9d5-bf5a7dd34b10");
            var version = new ClVersion(versionId,
                TestAccount.Project.Id,
                ClVersionValue.Parse("1.2"),
                null,
                DateTime.Parse("2021-04-09"),
                null);
            _versionDaoStub.Versions.Add(version);

            var addLineInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.Created(It.IsAny<Guid>()));
            _unitOfWorkMock.Setup(m => m.Start());
            _unitOfWorkMock.Setup(m => m.Commit());

            // act
            await addLineInteractor.ExecuteAsync(_outputPortMock.Object, changeLogLineRequestModel);

            // assert
            var savedLine = _changeLogDaoStub.ChangeLogs.Single(x =>
                x.ProjectId == TestAccount.Project.Id && x.VersionId!.Value == versionId);

            savedLine.Position.Should().Be(0);
            savedLine.IsPending.Should().BeFalse();
            savedLine.DeletedAt.Should().BeNull();
            savedLine.Issues.Count.Should().Be(2);
            savedLine.Labels.Count.Should().Be(1);
        }
    }
}