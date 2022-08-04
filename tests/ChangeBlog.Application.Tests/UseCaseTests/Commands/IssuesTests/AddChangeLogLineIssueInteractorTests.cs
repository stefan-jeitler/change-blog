using System;
using System.Linq;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.Tests.TestDoubles;
using ChangeBlog.Application.UseCases.ChangeLogs.Issues.AddChangeLogLineIssue;
using ChangeBlog.Application.UseCases.ChangeLogs.Issues.SharedModels;
using ChangeBlog.Domain.ChangeLog;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.Commands.IssuesTests;

public class AddChangeLogLineIssueInteractorTests
{
    private readonly FakeChangeLogDao _fakeChangeLogDao;
    private readonly Mock<IAddChangeLogLineIssueOutputPort> _outputPortMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public AddChangeLogLineIssueInteractorTests()
    {
        _outputPortMock = new Mock<IAddChangeLogLineIssueOutputPort>(MockBehavior.Strict);
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _fakeChangeLogDao = new FakeChangeLogDao();
    }

    private AddChangeLogLineIssueInteractor CreateInteractor()
    {
        return new AddChangeLogLineIssueInteractor(_unitOfWorkMock.Object, _fakeChangeLogDao, _fakeChangeLogDao);
    }

    [Fact]
    public async Task AddIssue_HappyPath_IssueAddedAndUowCommitted()
    {
        // arrange
        var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
        var issue = Issue.Parse("#1234");
        var requestModel = new ChangeLogLineIssueRequestModel(lineId, issue.Value);
        var addLabelInteractor = CreateInteractor();

        _fakeChangeLogDao.ChangeLogs.Add(new ChangeLogLine(lineId, null, TestAccount.Product.Id,
            ChangeLogText.Parse("Some text"), 0U, TestAccount.UserId, DateTime.Parse("2021-04-17")));
        _outputPortMock.Setup(m => m.Added(It.IsAny<Guid>()));

        // act
        await addLabelInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

        // assert
        _outputPortMock.Verify(m => m.Added(It.Is<Guid>(x => x == lineId)));
        _unitOfWorkMock.Verify(m => m.Start(), Times.Once);
        _unitOfWorkMock.Verify(m => m.Commit(), Times.Once);
        _fakeChangeLogDao.ChangeLogs.Single().Issues.Should().ContainSingle(x => x == issue);
    }

    [Fact]
    public async Task AddIssue_InvalidIssue_InvalidLabelOutput()
    {
        // arrange
        var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
        var requestModel = new ChangeLogLineIssueRequestModel(lineId, "# 234");
        var addLabelInteractor = CreateInteractor();

        _fakeChangeLogDao.ChangeLogs.Add(new ChangeLogLine(lineId, null, TestAccount.Product.Id,
            ChangeLogText.Parse("Some text"), 0U, TestAccount.UserId, DateTime.Parse("2021-04-17")));
        _outputPortMock.Setup(m => m.InvalidIssue(It.IsAny<string>()));

        // act
        await addLabelInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

        // assert
        _outputPortMock.Verify(m => m.InvalidIssue(It.Is<string>(x => x == "# 234")));
    }

    [Fact]
    public async Task AddIssue_ConflictWhileSaving_ConflictOutput()
    {
        // arrange
        var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
        var requestModel = new ChangeLogLineIssueRequestModel(lineId, "#1234");
        var addLabelInteractor = CreateInteractor();

        _fakeChangeLogDao.ChangeLogs.Add(new ChangeLogLine(lineId, null, TestAccount.Product.Id,
            ChangeLogText.Parse("Some text"), 0U, TestAccount.UserId, DateTime.Parse("2021-04-17")));
        _outputPortMock.Setup(m => m.Conflict(It.IsAny<Conflict>()));

        _fakeChangeLogDao.Conflict = new ConflictStub();

        // act
        await addLabelInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

        // assert
        _outputPortMock.Verify(m => m.Conflict(It.IsAny<Conflict>()), Times.Once);
    }

    [Fact]
    public async Task AddIssue_ExistingLineWithMaxLabels_MaxLabelsReachedOutput()
    {
        // arrange
        var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
        var requestModel = new ChangeLogLineIssueRequestModel(lineId, "#1234");
        var addLabelInteractor = CreateInteractor();

        var existingIssues = Enumerable.Range(0, 10).Select(x => $"{x:D5}").Select(Issue.Parse);
        _fakeChangeLogDao.ChangeLogs.Add(new ChangeLogLine(lineId, null, TestAccount.Product.Id,
            ChangeLogText.Parse("Some text"), 0U, DateTime.Parse("2021-04-17"), Array.Empty<Label>(),
            existingIssues, TestAccount.UserId));
        _outputPortMock.Setup(m => m.MaxIssuesReached(It.IsAny<int>()));

        // act
        await addLabelInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

        // assert
        _outputPortMock.Verify(m => m.MaxIssuesReached(It.Is<int>(x => x == ChangeLogLine.MaxIssues)), Times.Once);
    }

    [Fact]
    public async Task AddIssue_NotExistingChangeLogLine_ChangeLogLineDoesNotExistOutput()
    {
        // arrange
        var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
        var requestModel = new ChangeLogLineIssueRequestModel(lineId, "#1234");
        var addLabelInteractor = CreateInteractor();

        _outputPortMock.Setup(m => m.ChangeLogLineDoesNotExist());

        // act
        await addLabelInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

        // assert
        _outputPortMock.Verify(m => m.ChangeLogLineDoesNotExist());
    }
}