using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.Services.NotReleasedVersion;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.AddChangeLogLine;
using ChangeTracker.Application.UseCases.AddPendingChangeLogLine;
using ChangeTracker.Domain;
using ChangeTracker.Domain.ChangeLog;
using ChangeTracker.Domain.Version;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests
{
    public class AddPendingReleaseNoteLineUseCaseTests
    {
        private readonly ChangeLogDaoMock _changeLogDaoMock;
        private readonly Mock<IAddPendingChangeLogLineOutputPort> _outputPortMock;
        private readonly ProjectDaoMock _projectDaoMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public AddPendingReleaseNoteLineUseCaseTests()
        {
            _projectDaoMock = new ProjectDaoMock();
            _changeLogDaoMock = new ChangeLogDaoMock();
            _outputPortMock = new Mock<IAddPendingChangeLogLineOutputPort>(MockBehavior.Strict);
            _unitOfWorkMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
        }

        [Fact]
        public async Task AddChangeLogLine_InvalidLineText_InvalidChangeLogLineOutput()
        {
            // arrange
            const string changeLogLine = "a";
            var labels = new List<string> { "Bugfix", "ProxyIssue" };
            var issues = new List<string> { "#1234", "#12345" };
            var changeLogLineDto = new PendingChangeLogLineDto(TestAccount.Project.Id, changeLogLine, labels, issues);

            var addPendingChangeLogLineUseCase =
                new AddPendingChangeLogLineUseCase(_projectDaoMock, _changeLogDaoMock, _unitOfWorkMock.Object);

            _outputPortMock.Setup(m => m.InvalidChangeLogLineText(It.IsAny<string>()));
            _unitOfWorkMock.Setup(m => m.Start());

            // act
            await addPendingChangeLogLineUseCase.ExecuteAsync(_outputPortMock.Object, changeLogLineDto);

            // assert
            _outputPortMock.Verify(m
                => m.InvalidChangeLogLineText(It.Is<string>(x => x == "a")), Times.Once);
        }

        [Fact]
        public async Task AddChangeLogLine_NoMoreLinesAvailable_TooManyChangeLogLinesOutput()
        {
            // arrange
            const string changeLogLine = "Some Bug fixed";
            var labels = new List<string> { "Bugfix", "ProxyIssue" };
            var issues = new List<string> { "#1234", "#12345" };
            var changeLogLineDto =
                new PendingChangeLogLineDto(TestAccount.Project.Id, changeLogLine, labels, issues);

            _projectDaoMock.Projects.Add(new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, TestAccount.CreationDate, null));

            _changeLogDaoMock.ChangeLogs.AddRange(Enumerable.Range(0, 100)
                .Select(x => new ChangeLogLine(Guid.NewGuid(),
                    null,
                    TestAccount.Project.Id,
                    ChangeLogText.Parse($"{x:D5}"),
                    (uint)x,
                    DateTime.Parse("2021-04-09"))));

            var addPendingChangeLogLineUseCase =
                new AddPendingChangeLogLineUseCase(_projectDaoMock, _changeLogDaoMock, _unitOfWorkMock.Object);

            _outputPortMock.Setup(m => m.MaxChangeLogLinesReached(It.IsAny<int>()));
            _unitOfWorkMock.Setup(m => m.Start());

            // act
            await addPendingChangeLogLineUseCase.ExecuteAsync(_outputPortMock.Object, changeLogLineDto);

            // assert
            _outputPortMock.Verify(m => m.MaxChangeLogLinesReached(
                It.Is<int>(x => x == ChangeLogsMetadata.MaxChangeLogLines)), Times.Once);
        }

        [Fact]
        public async Task AddChangeLogLine_ConflictWhileSaving_ConflictOutput()
        {
            // arrange
            const string changeLogLine = "Some Bug fixed";
            var labels = new List<string> { "Bugfix", "ProxyIssue" };
            var issues = new List<string> { "#1234", "#12345" };
            var changeLogLineDto =
                new PendingChangeLogLineDto(TestAccount.Project.Id, changeLogLine, labels, issues);

            _projectDaoMock.Projects.Add(new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, TestAccount.CreationDate, null));

            _changeLogDaoMock.ProduceConflict = true;
            _changeLogDaoMock.ChangeLogs.Add(new ChangeLogLine(Guid.NewGuid(),
                null,
                TestAccount.Project.Id,
                ChangeLogText.Parse("some-release"),
                0,
                DateTime.Parse("2021-04-09")));

            var addPendingChangeLogLineUseCase =
                new AddPendingChangeLogLineUseCase(_projectDaoMock, _changeLogDaoMock, _unitOfWorkMock.Object);

            _outputPortMock.Setup(m => m.Conflict(It.IsAny<string>()));
            _unitOfWorkMock.Setup(m => m.Start());

            // act
            await addPendingChangeLogLineUseCase.ExecuteAsync(_outputPortMock.Object, changeLogLineDto);

            // assert
            _outputPortMock.Verify(m => m.Conflict(It.IsAny<string>()), Times.Once);
            _unitOfWorkMock.Verify(m => m.Start(), Times.Once);
        }

        [Fact]
        public async Task AddChangeLogLine_ValidLine_CreatedOutputAndCommittedUow()
        {
            // arrange
            const string changeLogLine = "Some Bug fixed";
            var labels = new List<string> { "Bugfix", "ProxyIssue" };
            var issues = new List<string> { "#1234", "#12345" };
            var changeLogLineDto =
                new PendingChangeLogLineDto(TestAccount.Project.Id, changeLogLine, labels, issues);

            _projectDaoMock.Projects.Add(new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, TestAccount.CreationDate, null));

            _changeLogDaoMock.ChangeLogs.Add(new ChangeLogLine(Guid.NewGuid(),
                null,
                TestAccount.Project.Id,
                ChangeLogText.Parse("some-release"),
                0,
                DateTime.Parse("2021-04-09")));

            var addPendingChangeLogLineUseCase =
                new AddPendingChangeLogLineUseCase(_projectDaoMock, _changeLogDaoMock, _unitOfWorkMock.Object);

            _outputPortMock.Setup(m => m.Created(It.IsAny<Guid>()));
            _unitOfWorkMock.Setup(m => m.Start());
            _unitOfWorkMock.Setup(m => m.Commit());

            // act
            await addPendingChangeLogLineUseCase.ExecuteAsync(_outputPortMock.Object, changeLogLineDto);

            // assert
            _outputPortMock.Verify(m => m.Created(It.IsAny<Guid>()), Times.Once);
            _unitOfWorkMock.Verify(m => m.Start(), Times.Once);
            _unitOfWorkMock.Verify(m => m.Commit(), Times.Once);
        }

    }
}
