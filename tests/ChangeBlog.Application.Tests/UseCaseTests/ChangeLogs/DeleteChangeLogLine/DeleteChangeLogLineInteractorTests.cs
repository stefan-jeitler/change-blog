using System;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.Tests.TestDoubles;
using ChangeBlog.Application.UseCases.ChangeLogs.DeleteChangeLogLine;
using ChangeBlog.Domain.ChangeLog;
using Moq;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.ChangeLogs.DeleteChangeLogLine;

public class DeleteChangeLogLineInteractorTests
{
    private readonly FakeChangeLogDao _fakeChangeLogDao;
    private readonly Mock<IDeleteChangeLogLineOutputPort> _outputPortMock;

    public DeleteChangeLogLineInteractorTests()
    {
        _fakeChangeLogDao = new FakeChangeLogDao();
        _outputPortMock = new Mock<IDeleteChangeLogLineOutputPort>(MockBehavior.Strict);
    }

    private DeleteChangeLogLineInteractor CreateInteractor()
    {
        return new DeleteChangeLogLineInteractor(_fakeChangeLogDao, _fakeChangeLogDao);
    }

    [Fact]
    public async Task DeleteLine_LinesDoesNotExist_LineDoesNotExistOutput()
    {
        // arrange
        var notExistingLineId = Guid.Parse("11cbae99-5cc0-4268-8d3f-31961822133c");
        var interactor = CreateInteractor();
        _outputPortMock.Setup(m => m.LineDoesNotExist(It.IsAny<Guid>()));

        var requestModel = new DeleteChangeLogLineRequestModel(notExistingLineId, ChangeLogLineType.NotPending);

        // act
        await interactor.ExecuteAsync(_outputPortMock.Object, requestModel);

        // assert
        _outputPortMock.Verify(m => m.LineDoesNotExist(It.Is<Guid>(x => x == notExistingLineId)), Times.Once);
    }

    [Fact]
    public async Task DeletePendingLine_ExistingLine_LineDeletedOutput()
    {
        // arrange
        var existingLineId = Guid.Parse("11cbae99-5cc0-4268-8d3f-31961822133c");
        var interactor = CreateInteractor();
        _outputPortMock.Setup(m => m.LineDeleted(It.IsAny<Guid>()));

        _fakeChangeLogDao.ChangeLogs.Add(new ChangeLogLine(existingLineId, null, TestAccount.Product.Id,
            ChangeLogText.Parse("some feature added"), 0, TestAccount.UserId, DateTime.Parse("2021-05-13")));

        var requestModel = new DeleteChangeLogLineRequestModel(existingLineId, ChangeLogLineType.Pending);

        // act
        await interactor.ExecuteAsync(_outputPortMock.Object, requestModel);

        // assert
        _outputPortMock.Verify(m => m.LineDeleted(It.Is<Guid>(x => x == existingLineId)), Times.Once);
    }

    [Fact]
    public async Task DeletePendingLine_ConflictWhenDelete_ConflictOutput()
    {
        // arrange
        var existingLineId = Guid.Parse("11cbae99-5cc0-4268-8d3f-31961822133c");
        var interactor = CreateInteractor();
        _outputPortMock.Setup(m => m.Conflict(It.IsAny<Conflict>()));

        _fakeChangeLogDao.ChangeLogs.Add(new ChangeLogLine(existingLineId, null, TestAccount.Product.Id,
            ChangeLogText.Parse("some feature added"), 0, TestAccount.UserId, DateTime.Parse("2021-05-13")));
        _fakeChangeLogDao.Conflict = new ConflictStub();

        var requestModel = new DeleteChangeLogLineRequestModel(existingLineId, ChangeLogLineType.Pending);

        // act
        await interactor.ExecuteAsync(_outputPortMock.Object, requestModel);

        // assert
        _outputPortMock.Verify(m => m.Conflict(It.IsAny<Conflict>()), Times.Once);
    }

    [Fact]
    public async Task DeletePendingLine_LineIsNotPending_RequestedLineIsNotPendingOutput()
    {
        // arrange
        var existingLineId = Guid.Parse("11cbae99-5cc0-4268-8d3f-31961822133c");
        var interactor = CreateInteractor();
        _outputPortMock.Setup(m => m.RequestedLineIsNotPending(It.IsAny<Guid>()));

        _fakeChangeLogDao.ChangeLogs.Add(new ChangeLogLine(existingLineId, Guid.NewGuid(), TestAccount.Product.Id,
            ChangeLogText.Parse("some feature added"), 0, TestAccount.UserId, DateTime.Parse("2021-05-13")));
        _fakeChangeLogDao.Conflict = new ConflictStub();

        var requestModel = new DeleteChangeLogLineRequestModel(existingLineId, ChangeLogLineType.Pending);

        // act
        await interactor.ExecuteAsync(_outputPortMock.Object, requestModel);

        // assert
        _outputPortMock.Verify(m => m.RequestedLineIsNotPending(It.Is<Guid>(x => x == existingLineId)), Times.Once);
    }

    [Fact]
    public async Task DeleteLine_LineIsPending_RequestedLineIsNotPendingOutput()
    {
        // arrange
        var existingLineId = Guid.Parse("11cbae99-5cc0-4268-8d3f-31961822133c");
        var interactor = CreateInteractor();
        _outputPortMock.Setup(m => m.RequestedLineIsPending(It.IsAny<Guid>()));

        _fakeChangeLogDao.ChangeLogs.Add(new ChangeLogLine(existingLineId, null, TestAccount.Product.Id,
            ChangeLogText.Parse("some feature added"), 0, TestAccount.UserId, DateTime.Parse("2021-05-13")));
        _fakeChangeLogDao.Conflict = new ConflictStub();

        var requestModel = new DeleteChangeLogLineRequestModel(existingLineId, ChangeLogLineType.NotPending);

        // act
        await interactor.ExecuteAsync(_outputPortMock.Object, requestModel);

        // assert
        _outputPortMock.Verify(m => m.RequestedLineIsPending(It.Is<Guid>(x => x == existingLineId)), Times.Once);
    }
}