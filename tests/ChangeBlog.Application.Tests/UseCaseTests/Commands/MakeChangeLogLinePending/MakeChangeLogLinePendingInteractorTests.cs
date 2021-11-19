using System;
using System.Linq;
using System.Threading.Tasks;
using ChangeBlog.Application.DataAccess;
using ChangeBlog.Application.Tests.TestDoubles;
using ChangeBlog.Application.UseCases.Commands.MakeChangeLogLinePending;
using ChangeBlog.Domain.ChangeLog;
using ChangeBlog.Domain.Miscellaneous;
using ChangeBlog.Domain.Version;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.Commands.MakeChangeLogLinePending;

public class MakeChangeLogLinePendingInteractorTests
{
    private readonly FakeChangeLogDao _fakeChangeLogDao;
    private readonly Mock<IMakeChangeLogLinePendingOutputPort> _outputPortMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly FakeVersionDao _fakeVersionDao;

    public MakeChangeLogLinePendingInteractorTests()
    {
        _fakeVersionDao = new FakeVersionDao();
        _fakeChangeLogDao = new FakeChangeLogDao();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _outputPortMock = new Mock<IMakeChangeLogLinePendingOutputPort>(MockBehavior.Strict);
    }

    private MakeChangeLogLinePendingInteractor CreateInteractor() =>
        new(_fakeVersionDao, _fakeChangeLogDao,
            _fakeChangeLogDao, _unitOfWorkMock.Object);

    [Fact]
    public async Task MakeLinePending_HappyPath_SuccessfullyAndUowStartedAndCommitted()
    {
        // arrange
        var clVersion = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.2.3"), OptionalName.Empty,
            TestAccount.UserId);
        _fakeVersionDao.Versions.Add(clVersion);

        var changeLogLine = new ChangeLogLine(clVersion.Id, TestAccount.Product.Id,
            ChangeLogText.Parse("some text"), 0, TestAccount.UserId);
        _fakeChangeLogDao.ChangeLogs.Add(changeLogLine);

        var makeLinePendingInteractor = CreateInteractor();

        _outputPortMock.Setup(m => m.WasMadePending(It.IsAny<Guid>()));

        // act
        await makeLinePendingInteractor.ExecuteAsync(_outputPortMock.Object, changeLogLine.Id);

        // assert
        _unitOfWorkMock.Verify(x => x.Start(), Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(), Times.Once);
        _fakeChangeLogDao.ChangeLogs.Single().VersionId.Should().NotHaveValue();
    }

    [Fact]
    public async Task MakeLinePending_LineIdIsEmpty_ArgumentException()
    {
        var makeLinePendingInteractor = CreateInteractor();

        var act = () => makeLinePendingInteractor.ExecuteAsync(_outputPortMock.Object, Guid.Empty);

        await act.Should().ThrowExactlyAsync<ArgumentException>();
    }

    [Fact]
    public async Task MakeLinePending_NotExistingLine_ChangeLogLineDoesNotExistOutput()
    {
        // arrange
        var notExistingLineId = Guid.Parse("f18d5027-4439-441d-be59-506b9bd3af6d");

        var makeLinePendingInteractor = CreateInteractor();

        _outputPortMock.Setup(m => m.ChangeLogLineDoesNotExist());

        // act
        await makeLinePendingInteractor.ExecuteAsync(_outputPortMock.Object, notExistingLineId);

        // assert
        _outputPortMock.Verify(m => m.ChangeLogLineDoesNotExist(), Times.Once);
    }

    [Fact]
    public async Task MakeLinePending_LineIsPending_ChangeLogLineDoesNotExistOutput()
    {
        // arrange
        var changeLogLine = new ChangeLogLine(null, TestAccount.Product.Id,
            ChangeLogText.Parse("some text"), 0, TestAccount.UserId);
        _fakeChangeLogDao.ChangeLogs.Add(changeLogLine);

        var makeLinePendingInteractor = CreateInteractor();

        _outputPortMock.Setup(m => m.ChangeLogLineIsAlreadyPending(It.IsAny<Guid>()));

        // act
        await makeLinePendingInteractor.ExecuteAsync(_outputPortMock.Object, changeLogLine.Id);

        // assert
        _outputPortMock.Verify(m => m.ChangeLogLineIsAlreadyPending(It.Is<Guid>(x => x == changeLogLine.Id)),
            Times.Once);
    }

    [Fact]
    public async Task MakeLinePending_VersionIsAlreadyReleased_VersionAlreadyReleasedOutput()
    {
        // arrange
        var clVersion = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.2.3"), OptionalName.Empty,
            TestAccount.UserId, DateTime.Parse("2021-04-18"));
        _fakeVersionDao.Versions.Add(clVersion);

        var changeLogLine = new ChangeLogLine(clVersion.Id, TestAccount.Product.Id,
            ChangeLogText.Parse("some text"), 0, TestAccount.UserId);
        _fakeChangeLogDao.ChangeLogs.Add(changeLogLine);

        var makeLinePendingInteractor = CreateInteractor();

        _outputPortMock.Setup(m => m.VersionAlreadyReleased(It.IsAny<Guid>()));

        // act
        await makeLinePendingInteractor.ExecuteAsync(_outputPortMock.Object, changeLogLine.Id);

        // assert
        _outputPortMock.Verify(m => m.VersionAlreadyReleased(It.Is<Guid>(x => x == clVersion.Id)), Times.Once);
    }

    [Fact]
    public async Task MakeLinePending_VersionIsClosed_VersionClosedOutput()
    {
        // arrange
        var clVersion = new ClVersion(Guid.NewGuid(), TestAccount.Product.Id,
            ClVersionValue.Parse("1.2.3"), OptionalName.Empty, null, TestAccount.UserId,
            DateTime.Parse("2021-04-18"),
            DateTime.Parse("2021-04-18"));

        _fakeVersionDao.Versions.Add(clVersion);

        var changeLogLine = new ChangeLogLine(clVersion.Id, TestAccount.Product.Id,
            ChangeLogText.Parse("some text"), 0, TestAccount.UserId);
        _fakeChangeLogDao.ChangeLogs.Add(changeLogLine);

        var makeLinePendingInteractor = CreateInteractor();

        _outputPortMock.Setup(m => m.VersionDeleted(It.IsAny<Guid>()));

        // act
        await makeLinePendingInteractor.ExecuteAsync(_outputPortMock.Object, changeLogLine.Id);

        // assert
        _outputPortMock.Verify(m => m.VersionDeleted(It.Is<Guid>(x => x == clVersion.Id)), Times.Once);
    }

    [Fact]
    public async Task MakeLinePending_TooManyPendingLinesExists_TooManyPendingLinesOutput()
    {
        // arrange
        var clVersion = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.2.3"), OptionalName.Empty,
            TestAccount.UserId);
        _fakeVersionDao.Versions.Add(clVersion);

        var changeLogLine = new ChangeLogLine(clVersion.Id, TestAccount.Product.Id,
            ChangeLogText.Parse("some text"), 0, TestAccount.UserId);
        _fakeChangeLogDao.ChangeLogs.Add(changeLogLine);

        _fakeChangeLogDao.ChangeLogs.AddRange(Enumerable.Range(0, 100)
            .Select(x => new ChangeLogLine(null, TestAccount.Product.Id,
                ChangeLogText.Parse($"{x:D5}"), 0, TestAccount.UserId)));

        var makeLinePendingInteractor = CreateInteractor();

        _outputPortMock.Setup(m => m.TooManyPendingLines(It.IsAny<int>()));

        // act
        await makeLinePendingInteractor.ExecuteAsync(_outputPortMock.Object, changeLogLine.Id);

        // assert
        _outputPortMock.Verify(m => m.TooManyPendingLines(It.IsAny<int>()));
    }

    [Fact]
    public async Task MakeLinePending_ConflictWhileSaving_ConflictOutput()
    {
        // arrange
        var clVersion = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.2.3"), OptionalName.Empty,
            TestAccount.UserId);
        _fakeVersionDao.Versions.Add(clVersion);

        var changeLogLine = new ChangeLogLine(clVersion.Id, TestAccount.Product.Id,
            ChangeLogText.Parse("some text"), 0, TestAccount.UserId);
        _fakeChangeLogDao.ChangeLogs.Add(changeLogLine);

        var makeLinePendingInteractor = CreateInteractor();
        _fakeChangeLogDao.Conflict = new ConflictStub();

        _outputPortMock.Setup(m => m.Conflict(It.IsAny<Conflict>()));

        // act
        await makeLinePendingInteractor.ExecuteAsync(_outputPortMock.Object, changeLogLine.Id);

        // assert
        _outputPortMock.Verify(m => m.Conflict(It.IsAny<Conflict>()), Times.Once);
    }

    [Fact]
    public async Task MakeLinePending_PendingLineWithSameTextExists_ConflictOutput()
    {
        // arrange
        var clVersion = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.2.3"), OptionalName.Empty,
            TestAccount.UserId);
        _fakeVersionDao.Versions.Add(clVersion);

        var changeLogLine = new ChangeLogLine(clVersion.Id, TestAccount.Product.Id,
            ChangeLogText.Parse("some text"), 0, TestAccount.UserId);
        _fakeChangeLogDao.ChangeLogs.Add(changeLogLine);
        var pendingLine = new ChangeLogLine(null, TestAccount.Product.Id,
            ChangeLogText.Parse("some text"), 0, TestAccount.UserId);
        _fakeChangeLogDao.ChangeLogs.Add(pendingLine);

        var makeLinePendingInteractor = CreateInteractor();

        _outputPortMock.Setup(m => m.LineWithSameTextAlreadyExists(It.IsAny<Guid>(), It.IsAny<string>()));

        // act
        await makeLinePendingInteractor.ExecuteAsync(_outputPortMock.Object, changeLogLine.Id);

        // assert
        _outputPortMock.Verify(
            m => m.LineWithSameTextAlreadyExists(It.Is<Guid>(x => x == pendingLine.Id),
                It.Is<string>(x => x == "some text")),
            Times.Once);
    }
}