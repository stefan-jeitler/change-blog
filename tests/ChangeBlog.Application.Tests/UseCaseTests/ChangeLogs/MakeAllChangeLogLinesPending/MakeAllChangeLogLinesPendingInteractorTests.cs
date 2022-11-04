using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.Tests.TestDoubles;
using ChangeBlog.Application.UseCases.ChangeLogs.MakeAllChangeLogLinesPending;
using ChangeBlog.Domain.ChangeLog;
using ChangeBlog.Domain.Miscellaneous;
using ChangeBlog.Domain.Version;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.ChangeLogs.MakeAllChangeLogLinesPending;

public class MakeAllChangeLogLinesPendingInteractorTests
{
    private readonly FakeChangeLogDao _fakeChangeLogDao;
    private readonly FakeVersionDao _fakeVersionDao;
    private readonly Mock<IMakeAllChangeLogLinesPendingOutputPort> _outputPortMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public MakeAllChangeLogLinesPendingInteractorTests()
    {
        _outputPortMock = new Mock<IMakeAllChangeLogLinesPendingOutputPort>(MockBehavior.Strict);
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _fakeVersionDao = new FakeVersionDao();
        _fakeChangeLogDao = new FakeChangeLogDao();
    }

    private MakeAllChangeLogLinesPendingInteractor CreateInteractor() =>
        new MakeAllChangeLogLinesPendingInteractor(_fakeVersionDao, _fakeChangeLogDao,
            _fakeChangeLogDao, _unitOfWorkMock.Object);

    [Fact]
    public async Task MakeAllLinesPending_EmptyVersionId_ArgumentException()
    {
        var makeAllLinesPendingInteractor = CreateInteractor();

        var act = () => makeAllLinesPendingInteractor.ExecuteAsync(_outputPortMock.Object, Guid.Empty);

        await act.Should().ThrowExactlyAsync<ArgumentException>();
    }

    [Fact]
    public async Task MakeAllLinesPending_EmptyProductId_ArgumentException()
    {
        var makeAllLinesPendingInteractor = CreateInteractor();

        var act = () =>
            makeAllLinesPendingInteractor.ExecuteAsync(_outputPortMock.Object, Guid.Empty, "1.2.3");

        await act.Should().ThrowExactlyAsync<ArgumentException>();
    }

    [Fact]
    public async Task MakeAllLinesPending_VersionIsNull_ArgumentNullException()
    {
        var makeAllLinesPendingInteractor = CreateInteractor();

        var act = () =>
            makeAllLinesPendingInteractor.ExecuteAsync(_outputPortMock.Object, TestAccount.Product.Id, null);

        await act.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task MakeAllLinesPending_InvalidVersionFormat_InvalidVersionFormatOutput()
    {
        // arrange
        var makeAllLinesPendingInteractor = CreateInteractor();
        _outputPortMock.Setup(m => m.InvalidVersionFormat(It.IsAny<string>()));

        // act
        await makeAllLinesPendingInteractor.ExecuteAsync(_outputPortMock.Object, TestAccount.Product.Id, "1. .3");

        // assert
        _outputPortMock.Verify(m => m.InvalidVersionFormat(It.Is<string>(x => x == "1. .3")));
    }

    [Fact]
    public async Task MakeAllLinesPending_NotExistingVersion_VersionDoesNotExistOutput()
    {
        // arrange
        var makeAllLinesPendingInteractor = CreateInteractor();
        var notExistingVersionId = Guid.Parse("7b591021-073a-4b7d-8bcb-fccc4d4db17a");

        _outputPortMock.Setup(m => m.VersionDoesNotExist());

        // act
        await makeAllLinesPendingInteractor.ExecuteAsync(_outputPortMock.Object, notExistingVersionId);

        // assert
        _outputPortMock.Verify(m => m.VersionDoesNotExist(), Times.Once);
    }

    [Fact]
    public async Task MakeAllLinesPendingByVersionValue_NotExistingVersion_VersionDoesNotExistOutput()
    {
        // arrange
        var makeAllLinesPendingInteractor = CreateInteractor();
        _outputPortMock.Setup(m => m.VersionDoesNotExist());

        // act
        await makeAllLinesPendingInteractor.ExecuteAsync(_outputPortMock.Object, TestAccount.Product.Id, "1.2.3");

        // assert
        _outputPortMock.Verify(m => m.VersionDoesNotExist(), Times.Once);
    }

    [Fact]
    public async Task MakeAllLinesPending_VersionIsAlreadyReleased_VersionAlreadyReleasedOutput()
    {
        // arrange
        var makeAllLinesPendingInteractor = CreateInteractor();

        var clVersion = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.2.3"), OptionalName.Empty,
            TestAccount.UserId,
            DateTime.Parse("2021-04-24"));

        _fakeVersionDao.Versions.Add(clVersion);
        _outputPortMock.Setup(m => m.VersionAlreadyReleased(It.IsAny<Guid>()));

        // act
        await makeAllLinesPendingInteractor.ExecuteAsync(_outputPortMock.Object, clVersion.Id);

        // assert
        _outputPortMock.Verify(m => m.VersionAlreadyReleased(It.Is<Guid>(x => x == clVersion.Id)), Times.Once);
    }

    [Fact]
    public async Task MakeAllLinesPending_VersionIsDeleted_VersionDeletedOutput()
    {
        // arrange
        var makeAllLinesPendingInteractor = CreateInteractor();

        var clVersion = new ClVersion(Guid.NewGuid(), TestAccount.Product.Id, ClVersionValue.Parse("1.2.3"),
            OptionalName.Empty,
            null, TestAccount.UserId, DateTime.Parse("2021-04-24"), DateTime.Parse("2021-04-24"));

        _fakeVersionDao.Versions.Add(clVersion);
        _outputPortMock.Setup(m => m.VersionDeleted(It.IsAny<Guid>()));

        // act
        await makeAllLinesPendingInteractor.ExecuteAsync(_outputPortMock.Object, clVersion.Id);

        // assert
        _outputPortMock.Verify(m => m.VersionDeleted(It.Is<Guid>(x => x == clVersion.Id)), Times.Once);
    }

    [Fact]
    public async Task MakeAllLinesPending_TooManyPendingLines_TooManyPendingLinesOutput()
    {
        // arrange
        var makeAllLinesPendingInteractor = CreateInteractor();

        var clVersion = new ClVersion(Guid.NewGuid(), TestAccount.Product.Id, ClVersionValue.Parse("1.2.3"),
            OptionalName.Empty,
            null, TestAccount.UserId, DateTime.Parse("2021-04-24"), null);

        var versionLine =
            new ChangeLogLine(clVersion.Id, TestAccount.Product.Id, ChangeLogText.Parse("some text"), 0,
                TestAccount.UserId);
        _fakeChangeLogDao.ChangeLogs.Add(versionLine);
        var pendingLines = Enumerable.Range(0, 100)
            .Select(x => new ChangeLogLine(null, TestAccount.Product.Id, ChangeLogText.Parse($"{x:D5}"), (uint) x,
                TestAccount.UserId));
        _fakeChangeLogDao.ChangeLogs.AddRange(pendingLines);

        _fakeVersionDao.Versions.Add(clVersion);
        _outputPortMock.Setup(m => m.TooManyPendingLines(It.IsAny<int>()));

        // act
        await makeAllLinesPendingInteractor.ExecuteAsync(_outputPortMock.Object, clVersion.Id);

        // assert
        _outputPortMock.Verify(
            m => m.TooManyPendingLines(It.Is<int>(x => x == Domain.ChangeLog.ChangeLogs.MaxLines)),
            Times.Once);
    }

    [Fact]
    public async Task MakeAllLinesPending_LineWithSameText_LineWithSameTextAlreadyExistsOutput()
    {
        // arrange
        var makeAllLinesPendingInteractor = CreateInteractor();

        var clVersion = new ClVersion(Guid.NewGuid(), TestAccount.Product.Id, ClVersionValue.Parse("1.2.3"),
            OptionalName.Empty,
            null, TestAccount.UserId, DateTime.Parse("2021-04-24"), null);

        var versionLine1 = new ChangeLogLine(clVersion.Id, TestAccount.Product.Id, ChangeLogText.Parse("00000"), 0,
            TestAccount.UserId);
        var versionLine2 = new ChangeLogLine(clVersion.Id, TestAccount.Product.Id, ChangeLogText.Parse("00001"), 0,
            TestAccount.UserId);
        _fakeChangeLogDao.ChangeLogs.Add(versionLine1);
        _fakeChangeLogDao.ChangeLogs.Add(versionLine2);
        var pendingLines = Enumerable.Range(0, 98)
            .Select(x => new ChangeLogLine(null, TestAccount.Product.Id, ChangeLogText.Parse($"{x:D5}"), (uint) x,
                TestAccount.UserId));
        _fakeChangeLogDao.ChangeLogs.AddRange(pendingLines);

        _fakeVersionDao.Versions.Add(clVersion);
        _outputPortMock.Setup(m => m.LineWithSameTextAlreadyExists(It.IsAny<List<string>>()));

        // act
        await makeAllLinesPendingInteractor.ExecuteAsync(_outputPortMock.Object, clVersion.Id);

        // assert
        Expression<Func<List<string>, bool>> verify = x =>
            x.Count == 2 && x.Contains("00000") && x.Contains("00001");

        _outputPortMock.Verify(m => m.LineWithSameTextAlreadyExists(It.Is(verify)), Times.Once);
    }

    [Fact]
    public async Task MakeAllLinesPending_ConflictWhileSaving_ConflictOutput()
    {
        // arrange
        var makeAllLinesPendingInteractor = CreateInteractor();

        var clVersion = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.2.3"), OptionalName.Empty,
            TestAccount.UserId);

        var versionLine1 =
            new ChangeLogLine(clVersion.Id, TestAccount.Product.Id, ChangeLogText.Parse("some text"), 0,
                TestAccount.UserId);
        _fakeChangeLogDao.ChangeLogs.Add(versionLine1);
        _fakeChangeLogDao.Conflict = new ConflictStub();
        _fakeVersionDao.Versions.Add(clVersion);
        _outputPortMock.Setup(m => m.Conflict(It.IsAny<Conflict>()));

        // act
        await makeAllLinesPendingInteractor.ExecuteAsync(_outputPortMock.Object, clVersion.Id);

        // assert
        _outputPortMock.Verify(m => m.Conflict(It.IsAny<Conflict>()), Times.Once);
    }

    [Fact]
    public async Task MakeAllLinesPending_HappyPatch_MadePendingOutputAndStartedCommittedUow()
    {
        // arrange
        var makeAllLinesPendingInteractor = CreateInteractor();

        var clVersion = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.2.3"), OptionalName.Empty,
            TestAccount.UserId);

        var versionLine =
            new ChangeLogLine(clVersion.Id, TestAccount.Product.Id, ChangeLogText.Parse("some text"), 0,
                TestAccount.UserId);
        _fakeChangeLogDao.ChangeLogs.Add(versionLine);
        _fakeVersionDao.Versions.Add(clVersion);
        _outputPortMock.Setup(m => m.MadePending(It.IsAny<Guid>(), It.IsAny<int>()));

        // act
        await makeAllLinesPendingInteractor.ExecuteAsync(_outputPortMock.Object, clVersion.Id);

        // assert
        _outputPortMock.Verify(
            m => m.MadePending(It.Is<Guid>(x => x == TestAccount.Product.Id), It.Is<int>(x => x == 1)), Times.Once);
        _unitOfWorkMock.Verify(m => m.Start(), Times.Once);
        _unitOfWorkMock.Verify(m => m.Commit(), Times.Once);
    }
}