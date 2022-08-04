using System;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.Tests.TestDoubles;
using ChangeBlog.Application.UseCases.ChangeLogs.DeleteChangeLogLine;
using ChangeBlog.Application.UseCases.ChangeLogs.UpdateChangeLogLine;
using ChangeBlog.Domain.ChangeLog;
using Moq;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.Commands.UpdateChangeLogLine;

public class UpdateChangeLogLineInteractorTests
{
    private readonly FakeChangeLogDao _fakeChangeLogDao;
    private readonly Mock<IUpdateChangeLogLineOutputPort> _outputPortMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public UpdateChangeLogLineInteractorTests()
    {
        _outputPortMock = new Mock<IUpdateChangeLogLineOutputPort>(MockBehavior.Strict);
        _fakeChangeLogDao = new FakeChangeLogDao();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
    }

    private UpdateChangeLogLineInteractor CreateInteractor()
    {
        return new UpdateChangeLogLineInteractor(_fakeChangeLogDao, _fakeChangeLogDao, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task UpdateLine_HappyPath_Successful()
    {
        // arrange
        var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
        const string text = "feature added";
        var requestModel =
            new UpdateChangeLogLineRequestModel(lineId,
                ChangeLogLineType.Pending,
                text,
                Array.Empty<string>(),
                Array.Empty<string>());
        var updateLineInteractor = CreateInteractor();

        _fakeChangeLogDao.ChangeLogs.Add(new ChangeLogLine(lineId, null, TestAccount.Product.Id,
            ChangeLogText.Parse("some feature added"), 0, DateTime.Parse("2021-04-17"), Array.Empty<Label>(),
            Array.Empty<Issue>(), TestAccount.UserId));

        _outputPortMock.Setup(m => m.Updated(It.IsAny<Guid>()));
        _unitOfWorkMock.Setup(m => m.Start());
        _unitOfWorkMock.Setup(m => m.Commit());

        // act
        await updateLineInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

        // assert
        _outputPortMock.Verify(m => m.Updated(It.Is<Guid>(x => x == lineId)), Times.Once);
        _unitOfWorkMock.Verify(m => m.Start(), Times.Once);
        _unitOfWorkMock.Verify(m => m.Commit(), Times.Once);
    }

    [Fact]
    public async Task UpdateLine_PendingLinePassed_LineIsPendingOutput()
    {
        // arrange
        var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
        const string text = "feature added";
        var requestModel =
            new UpdateChangeLogLineRequestModel(lineId,
                ChangeLogLineType.NotPending,
                text,
                Array.Empty<string>(),
                Array.Empty<string>());
        var updateLineInteractor = CreateInteractor();

        _fakeChangeLogDao.ChangeLogs.Add(new ChangeLogLine(lineId, null, TestAccount.Product.Id,
            ChangeLogText.Parse("some feature added"), 0, DateTime.Parse("2021-04-17"), Array.Empty<Label>(),
            Array.Empty<Issue>(), TestAccount.UserId));

        _outputPortMock.Setup(m => m.RequestedLineIsPending(It.IsAny<Guid>()));

        // act
        await updateLineInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

        // assert
        _outputPortMock.Verify(m => m.RequestedLineIsPending(It.Is<Guid>(x => x == lineId)), Times.Once);
    }

    [Fact]
    public async Task UpdateLine_ConflictWhileSaving_ConflictOutput()
    {
        // arrange
        var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
        const string text = "feature added";
        var requestModel =
            new UpdateChangeLogLineRequestModel(lineId,
                ChangeLogLineType.Pending,
                text,
                Array.Empty<string>(),
                Array.Empty<string>());
        var updateLineInteractor = CreateInteractor();

        _fakeChangeLogDao.ChangeLogs.Add(new ChangeLogLine(lineId, null, TestAccount.Product.Id,
            ChangeLogText.Parse("some feature added"), 0, DateTime.Parse("2021-04-17"), Array.Empty<Label>(),
            Array.Empty<Issue>(), TestAccount.UserId));

        _fakeChangeLogDao.Conflict = new ConflictStub();

        _outputPortMock.Setup(m => m.Conflict(It.IsAny<Conflict>()));

        // act
        await updateLineInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

        // assert
        _outputPortMock.Verify(m => m.Conflict(It.IsAny<Conflict>()), Times.Once);
    }

    [Fact]
    public async Task UpdateLine_WithTextThatAlreadyExist_Output()
    {
        // arrange
        var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
        const string text = "feature added";
        var requestModel =
            new UpdateChangeLogLineRequestModel(lineId,
                ChangeLogLineType.Pending,
                text,
                Array.Empty<string>(),
                Array.Empty<string>());
        var updateLineInteractor = CreateInteractor();

        _fakeChangeLogDao.ChangeLogs.Add(new ChangeLogLine(lineId, null, TestAccount.Product.Id,
            ChangeLogText.Parse("some feature added"), 0, DateTime.Parse("2021-04-17"), Array.Empty<Label>(),
            Array.Empty<Issue>(), TestAccount.UserId));
        _fakeChangeLogDao.ChangeLogs.Add(new ChangeLogLine(null, TestAccount.Product.Id,
            ChangeLogText.Parse("feature added"), 0, TestAccount.UserId));

        _outputPortMock.Setup(m => m.LineWithSameTextAlreadyExists(It.IsAny<string>()));

        // act
        await updateLineInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

        // assert
        _outputPortMock.Verify(m => m.LineWithSameTextAlreadyExists(It.Is<string>(x => x == text)), Times.Once);
    }

    [Fact]
    public async Task UpdateLine_NotExistingLine_ChangeLogLineDoesNotExistOutput()
    {
        // arrange
        var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
        const string text = "some features added";
        var requestModel =
            new UpdateChangeLogLineRequestModel(lineId,
                ChangeLogLineType.Pending,
                text,
                Array.Empty<string>(),
                Array.Empty<string>());
        var updateLineInteractor = CreateInteractor();

        _outputPortMock.Setup(m => m.ChangeLogLineDoesNotExist());

        // act
        await updateLineInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

        // assert
        _outputPortMock.Verify(m => m.ChangeLogLineDoesNotExist(), Times.Once);
    }
}