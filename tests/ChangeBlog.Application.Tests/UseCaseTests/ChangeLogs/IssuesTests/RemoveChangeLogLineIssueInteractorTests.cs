using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.Tests.TestDoubles;
using ChangeBlog.Application.UseCases.ChangeLogs.Issues.DeleteChangeLogLineIssue;
using ChangeBlog.Application.UseCases.ChangeLogs.Issues.SharedModels;
using ChangeBlog.Domain.ChangeLog;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.ChangeLogs.IssuesTests;

public class RemoveChangeLogLineIssueInteractorTests
{
    private readonly FakeChangeLogDao _fakeChangeLogDao;
    private readonly Mock<IDeleteChangeLogLineIssueOutputPort> _outputPortMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public RemoveChangeLogLineIssueInteractorTests()
    {
        _fakeChangeLogDao = new FakeChangeLogDao();
        _outputPortMock = new Mock<IDeleteChangeLogLineIssueOutputPort>(MockBehavior.Strict);
        _unitOfWorkMock = new Mock<IUnitOfWork>();
    }

    private DeleteChangeLogLineIssueInteractor CreateInteractor()
    {
        return new DeleteChangeLogLineIssueInteractor(_fakeChangeLogDao, _fakeChangeLogDao, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task RemoveIssue_HappyPath_IssueAddedAndUowCommitted()
    {
        // arrange
        var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
        var issue = Issue.Parse("#1234");
        var requestModel = new ChangeLogLineIssueRequestModel(lineId, issue.Value);
        var removeIssueInteractor = CreateInteractor();

        var line = new ChangeLogLine(lineId, null, TestAccount.Product.Id, ChangeLogText.Parse("some valid text"),
            0U, DateTime.Parse("2021-04-19"), Array.Empty<Label>(), new List<Issue>(1) { issue }, TestAccount.UserId);
        _fakeChangeLogDao.ChangeLogs.Add(line);

        _outputPortMock.Setup(m => m.Removed(It.IsAny<Guid>()));

        // act
        await removeIssueInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

        // assert
        _unitOfWorkMock.Verify(m => m.Start(), Times.Once);
        _unitOfWorkMock.Verify(m => m.Commit(), Times.Once);
        _outputPortMock.Verify(m => m.Removed(It.Is<Guid>(x => x == lineId)));
        _fakeChangeLogDao.ChangeLogs.Single().Issues.Should().BeEmpty();
    }

    [Fact]
    public async Task RemoveIssue_ConflictWhileSaving_ConflictOutput()
    {
        // arrange
        var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
        var issue = Issue.Parse("#1234");
        var requestModel = new ChangeLogLineIssueRequestModel(lineId, issue.Value);
        var removeIssueInteractor = CreateInteractor();

        var line = new ChangeLogLine(lineId, null, TestAccount.Product.Id, ChangeLogText.Parse("some valid text"),
            0U, DateTime.Parse("2021-04-19"), Array.Empty<Label>(), new List<Issue>(1) { issue }, TestAccount.UserId);
        _fakeChangeLogDao.ChangeLogs.Add(line);
        _fakeChangeLogDao.Conflict = new ConflictStub();

        _outputPortMock.Setup(m => m.Conflict(It.IsAny<Conflict>()));

        // act
        await removeIssueInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

        // assert
        _outputPortMock.Verify(m => m.Conflict(It.IsAny<Conflict>()), Times.Once);
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