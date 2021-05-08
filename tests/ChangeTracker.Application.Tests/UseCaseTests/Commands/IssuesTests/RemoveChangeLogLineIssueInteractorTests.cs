using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.Commands.Issues.RemoveChangeLogLineIssue;
using ChangeTracker.Application.UseCases.Commands.Issues.SharedModels;
using ChangeTracker.Domain.ChangeLog;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.Commands.IssuesTests
{
    public class RemoveChangeLogLineIssueInteractorTests
    {
        private readonly ChangeLogDaoStub _changeLogDaoStub;
        private readonly Mock<IRemoveChangeLogLineIssueOutputPort> _outputPortMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public RemoveChangeLogLineIssueInteractorTests()
        {
            _changeLogDaoStub = new ChangeLogDaoStub();
            _outputPortMock = new Mock<IRemoveChangeLogLineIssueOutputPort>(MockBehavior.Strict);
            _unitOfWorkMock = new Mock<IUnitOfWork>();
        }

        private RemoveChangeLogLineIssueInteractor CreateInteractor() =>
            new(_changeLogDaoStub, _changeLogDaoStub, _unitOfWorkMock.Object);

        [Fact]
        public async Task RemoveIssue_HappyPath_IssueAddedAndUowCommitted()
        {
            // arrange
            var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
            var issue = Issue.Parse("#1234");
            var requestModel = new ChangeLogLineIssueRequestModel(lineId, issue.Value);
            var removeIssueInteractor = CreateInteractor();

            var line = new ChangeLogLine(lineId, null, TestAccount.Project.Id, ChangeLogText.Parse("some valid text"),
                0U, DateTime.Parse("2021-04-19"), Array.Empty<Label>(), new List<Issue>(1) {issue});
            _changeLogDaoStub.ChangeLogs.Add(line);

            _outputPortMock.Setup(m => m.Removed(It.IsAny<Guid>()));

            // act
            await removeIssueInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

            // assert
            _unitOfWorkMock.Verify(m => m.Start(), Times.Once);
            _unitOfWorkMock.Verify(m => m.Commit(), Times.Once);
            _outputPortMock.Verify(m => m.Removed(It.Is<Guid>(x => x == lineId)));
            _changeLogDaoStub.ChangeLogs.Single().Issues.Should().BeEmpty();
        }

        [Fact]
        public async Task RemoveIssue_ConflictWhileSaving_ConflictOutput()
        {
            // arrange
            var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
            var issue = Issue.Parse("#1234");
            var requestModel = new ChangeLogLineIssueRequestModel(lineId, issue.Value);
            var removeIssueInteractor = CreateInteractor();

            var line = new ChangeLogLine(lineId, null, TestAccount.Project.Id, ChangeLogText.Parse("some valid text"),
                0U, DateTime.Parse("2021-04-19"), Array.Empty<Label>(), new List<Issue>(1) {issue});
            _changeLogDaoStub.ChangeLogs.Add(line);
            _changeLogDaoStub.ProduceConflict = true;

            _outputPortMock.Setup(m => m.Conflict(It.IsAny<string>()));

            // act
            await removeIssueInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

            // assert
            _outputPortMock.Verify(m => m.Conflict(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task RemoveIssue_InvalidIssue_InvalidIssueOutput()
        {
            // arrange
            var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
            const string issue = "# 234";
            var requestModel = new ChangeLogLineIssueRequestModel(lineId, issue);
            var removeIssueInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.InvalidIssue(It.IsAny<string>()));

            // act
            await removeIssueInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

            // assert
            _outputPortMock.Verify(m => m.InvalidIssue(It.Is<string>(x => x == issue)), Times.Once);
        }

        [Fact]
        public async Task RemoveIssue_NotExistingLine_ChangeLogLineDoesNotExistOutput()
        {
            // arrange
            var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
            const string issue = "#1234";
            var requestModel = new ChangeLogLineIssueRequestModel(lineId, issue);
            var removeIssueInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.ChangeLogLineDoesNotExist());

            // act
            await removeIssueInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

            // assert
            _outputPortMock.Verify(m => m.ChangeLogLineDoesNotExist(), Times.Once);
        }
    }
}