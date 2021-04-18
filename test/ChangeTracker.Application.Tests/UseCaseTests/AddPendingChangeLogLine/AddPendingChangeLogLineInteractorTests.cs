using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.AddPendingChangeLogLine;
using ChangeTracker.Domain;
using ChangeTracker.Domain.ChangeLog;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.AddPendingChangeLogLine
{
    public class AddPendingChangeLogLineInteractorTests
    {
        private readonly ChangeLogDaoStub _changeLogDaoStub;
        private readonly Mock<IAddPendingLineOutputPort> _outputPortMock;
        private readonly ProjectDaoStub _projectDaoStub;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public AddPendingChangeLogLineInteractorTests()
        {
            _projectDaoStub = new ProjectDaoStub();
            _changeLogDaoStub = new ChangeLogDaoStub();
            _outputPortMock = new Mock<IAddPendingLineOutputPort>(MockBehavior.Strict);
            _unitOfWorkMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
        }

        private AddPendingChangeLogLineInteractor CreateInteractor() =>
            new(_projectDaoStub, _changeLogDaoStub, _changeLogDaoStub, _unitOfWorkMock.Object);

        [Fact]
        public async Task AddChangeLogLine_NotExistingProject_ProjectDoesNotExistOutput()
        {
            // arrange
            const string changeLogLine = "Some Bug fixed";
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            var issues = new List<string> {"#1234", "#12345"};
            var lineRequestModel =
                new PendingLineRequestModel(TestAccount.Project.Id, changeLogLine, labels, issues);

            var addPendingLineInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.ProjectDoesNotExist());

            // act
            await addPendingLineInteractor.ExecuteAsync(_outputPortMock.Object, lineRequestModel);

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
            var lineRequestModel =
                new PendingLineRequestModel(TestAccount.Project.Id, changeLogLine, labels, issues);

            _projectDaoStub.Projects.Add(new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, TestAccount.CreationDate, null));

            _changeLogDaoStub.ProduceConflict = true;
            _changeLogDaoStub.ChangeLogs.Add(new ChangeLogLine(Guid.NewGuid(),
                null,
                TestAccount.Project.Id,
                ChangeLogText.Parse("some-release"),
                0,
                DateTime.Parse("2021-04-09")));

            var addPendingLineInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.Conflict(It.IsAny<string>()));
            _unitOfWorkMock.Setup(m => m.Start());

            // act
            await addPendingLineInteractor.ExecuteAsync(_outputPortMock.Object, lineRequestModel);

            // assert
            _outputPortMock.Verify(m => m.Conflict(It.IsAny<string>()), Times.Once);
            _unitOfWorkMock.Verify(m => m.Start(), Times.Once);
        }

        [Fact]
        public async Task AddChangeLogLine_MaxLinesReached_TooManyLinesOutput()
        {
            // arrange
            const string changeLogLine = "Some Bug fixed";
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            var issues = new List<string> {"#1234", "#12345"};
            var lineRequestModel =
                new PendingLineRequestModel(TestAccount.Project.Id, changeLogLine, labels, issues);

            _projectDaoStub.Projects.Add(new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, TestAccount.CreationDate, null));

            _changeLogDaoStub.ChangeLogs.AddRange(Enumerable.Range(0, 100)
                .Select(x =>
                    new ChangeLogLine(null, TestAccount.Project.Id, ChangeLogText.Parse($"{x:D5}"), (uint) x)));

            var addPendingLineInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.TooManyLines(It.IsAny<int>()));
            _unitOfWorkMock.Setup(m => m.Start());

            // act
            await addPendingLineInteractor.ExecuteAsync(_outputPortMock.Object, lineRequestModel);

            // assert
            _outputPortMock.Verify(m => m.TooManyLines(It.Is<int>(x => x == ChangeLogsMetadata.MaxChangeLogLines)),
                Times.Once);
        }

        [Fact]
        public async Task AddChangeLogLine_ValidLine_CreatedOutputAndCommittedUow()
        {
            // arrange
            const string changeLogLine = "Some Bug fixed";
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            var issues = new List<string> {"#1234", "#12345"};
            var lineRequestModel =
                new PendingLineRequestModel(TestAccount.Project.Id, changeLogLine, labels, issues);

            _projectDaoStub.Projects.Add(new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, TestAccount.CreationDate, null));

            _changeLogDaoStub.ChangeLogs.Add(new ChangeLogLine(Guid.NewGuid(),
                null,
                TestAccount.Project.Id,
                ChangeLogText.Parse("some-release"),
                0,
                DateTime.Parse("2021-04-09")));

            var addPendingLineInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.Created(It.IsAny<Guid>()));
            _unitOfWorkMock.Setup(m => m.Start());
            _unitOfWorkMock.Setup(m => m.Commit());

            // act
            await addPendingLineInteractor.ExecuteAsync(_outputPortMock.Object, lineRequestModel);

            // assert
            _outputPortMock.Verify(m => m.Created(It.IsAny<Guid>()), Times.Once);
            _unitOfWorkMock.Verify(m => m.Start(), Times.Once);
            _unitOfWorkMock.Verify(m => m.Commit(), Times.Once);
        }
    }
}