using System;
using System.Threading.Tasks;
using ChangeBlog.Application.Tests.TestDoubles;
using ChangeBlog.Application.UseCases.Queries.GetPendingChangeLogLine;
using ChangeBlog.Domain.ChangeLog;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.Queries.GetPendingChangeLogLine;

public class GetPendingChangeLogLineInteractorTests
{
    private readonly FakeChangeLogDao _fakeChangeLogDao;
    private readonly FakeProductDao _fakeProductDao;
    private readonly FakeUserDao _fakeUserDao;
    private readonly Mock<IGetPendingChangeLogLineOutputPort> _outputPortMock;

    public GetPendingChangeLogLineInteractorTests()
    {
        _fakeChangeLogDao = new FakeChangeLogDao();
        _fakeProductDao = new FakeProductDao();
        _fakeUserDao = new FakeUserDao();
        _outputPortMock = new Mock<IGetPendingChangeLogLineOutputPort>(MockBehavior.Strict);
    }

    private GetPendingChangeLogLineInteractor CreateInteractor()
    {
        return new GetPendingChangeLogLineInteractor(_fakeChangeLogDao, _fakeUserDao, _fakeProductDao);
    }

    [Fact]
    public async Task GetPendingChangeLogLine_HappyPath_Successful()
    {
        // arrange
        var interactor = CreateInteractor();
        _fakeUserDao.Users.Add(TestAccount.User);
        _fakeProductDao.Products.Add(TestAccount.Product);
        var changeLogLineId = Guid.Parse("bf621860-3fa3-40d4-92ac-530cc57a1a98");
        _fakeChangeLogDao.ChangeLogs.Add(new ChangeLogLine(changeLogLineId, null, TestAccount.Product.Id,
            ChangeLogText.Parse("Test line."), 0, TestAccount.UserId, DateTime.Parse("2021-07-26")));

        _outputPortMock.Setup(m => m.LineFound(It.IsAny<PendingChangeLogLineResponseModel>()));

        // act
        await interactor.ExecuteAsync(_outputPortMock.Object, TestAccount.UserId, changeLogLineId);

        // assert
        _outputPortMock.Verify(
            m => m.LineFound(It.Is<PendingChangeLogLineResponseModel>(r => r.ChangeLogLine.Id == changeLogLineId)),
            Times.Once);
    }


    [Fact]
    public async Task GetChangeLogLine_UserTimezone2HoursAheadOfUtc_CreatedAtProperlyConverted()
    {
        // arrange
        var interactor = CreateInteractor();
        _fakeUserDao.Users.Add(TestAccount.User);
        _fakeProductDao.Products.Add(TestAccount.Product);
        var changeLogLineId = Guid.Parse("bf621860-3fa3-40d4-92ac-530cc57a1a98");
        _fakeChangeLogDao.ChangeLogs.Add(new ChangeLogLine(changeLogLineId, null, TestAccount.Product.Id,
            ChangeLogText.Parse("Test line."), 0, TestAccount.UserId, DateTime.Parse("2021-07-26")));

        _outputPortMock.Setup(m => m.LineFound(It.IsAny<PendingChangeLogLineResponseModel>()));

        // act
        await interactor.ExecuteAsync(_outputPortMock.Object, TestAccount.UserId, changeLogLineId);

        // assert
        var expectedCreatedAt = DateTimeOffset.Parse("2021-07-26T02:00:00+02:00");
        _outputPortMock.Verify(
            m => m.LineFound(It.Is<PendingChangeLogLineResponseModel>(r =>
                r.ChangeLogLine.CreatedAt.LocalDateTime == expectedCreatedAt)),
            Times.Once);
    }

    [Fact]
    public async Task GetPendingChangeLogLine_LineIsNotPending_LineNotPendingOutput()
    {
        // arrange
        var interactor = CreateInteractor();
        _fakeUserDao.Users.Add(TestAccount.User);
        _fakeProductDao.Products.Add(TestAccount.Product);
        var versionId = Guid.Parse("d8d1ab5d-670d-4370-84d5-2a260755e45c");
        var changeLogLineId = Guid.Parse("bf621860-3fa3-40d4-92ac-530cc57a1a98");
        _fakeChangeLogDao.ChangeLogs.Add(new ChangeLogLine(changeLogLineId, versionId, TestAccount.Product.Id,
            ChangeLogText.Parse("Test line."), 0, TestAccount.UserId, DateTime.Parse("2021-07-26")));

        _outputPortMock.Setup(m => m.LineIsNotPending(It.IsAny<Guid>()));

        // act
        await interactor.ExecuteAsync(_outputPortMock.Object, TestAccount.UserId, changeLogLineId);

        // assert
        _outputPortMock.Verify(m => m.LineIsNotPending(It.Is<Guid>(x => x == changeLogLineId)),
            Times.Once);
    }

    [Fact]
    public async Task GetPendingChangeLogLine_NotExistingLine_LineDoesNotExistOutput()
    {
        // arrange
        var interactor = CreateInteractor();
        _fakeUserDao.Users.Add(TestAccount.User);
        _fakeProductDao.Products.Add(TestAccount.Product);
        var changeLogLineId = Guid.Parse("bf621860-3fa3-40d4-92ac-530cc57a1a98");

        _outputPortMock.Setup(m => m.LineDoesNotExist(It.IsAny<Guid>()));

        // act
        await interactor.ExecuteAsync(_outputPortMock.Object, TestAccount.UserId, changeLogLineId);

        // assert
        _outputPortMock.Verify(m => m.LineDoesNotExist(It.Is<Guid>(x => x == changeLogLineId)),
            Times.Once);
    }

    [Fact]
    public async Task GetPendingChangeLogLine_EmptyProductId_ArgumentException()
    {
        var interactor = CreateInteractor();
        var changeLogLineId = Guid.Parse("bf621860-3fa3-40d4-92ac-530cc57a1a98");

        var act = () => interactor.ExecuteAsync(_outputPortMock.Object, Guid.Empty, changeLogLineId);

        await act.Should().ThrowExactlyAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetPendingChangeLogLine_EmptyChangeLogLineId_ArgumentException()
    {
        var interactor = CreateInteractor();

        var act = () => interactor.ExecuteAsync(_outputPortMock.Object, TestAccount.Product.Id, Guid.Empty);

        await act.Should().ThrowExactlyAsync<ArgumentException>();
    }
}