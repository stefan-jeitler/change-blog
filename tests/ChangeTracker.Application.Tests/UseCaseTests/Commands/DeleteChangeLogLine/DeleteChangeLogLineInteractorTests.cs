using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.Commands.DeleteChangeLogLine;
using ChangeTracker.Domain.ChangeLog;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.Commands.DeleteChangeLogLine
{
    public class DeleteChangeLogLineInteractorTests
    {
        private readonly ChangeLogDaoStub _changeLogDaoStub;
        private readonly Mock<IDeleteChangeLogLineOutputPort> _outputPortMock;

        public DeleteChangeLogLineInteractorTests()
        {
            _changeLogDaoStub = new ChangeLogDaoStub();
            _outputPortMock = new Mock<IDeleteChangeLogLineOutputPort>(MockBehavior.Strict);
        }

        private DeleteChangeLogLineInteractor CreateInteractor() => new(_changeLogDaoStub, _changeLogDaoStub);

        [Fact]
        public async Task DeleteLine_LinesDoesNotExist_LineDoesNotExistOutput()
        {
            var notExistingLineId = Guid.Parse("11cbae99-5cc0-4268-8d3f-31961822133c");
            var interactor = CreateInteractor();
            _outputPortMock.Setup(m => m.LineDoesNotExist(It.IsAny<Guid>()));

            await interactor.ExecuteAsync(_outputPortMock.Object, notExistingLineId);

            _outputPortMock.Verify(m => m.LineDoesNotExist(It.Is<Guid>(x => x == notExistingLineId)), Times.Once);
        }

        [Fact]
        public async Task DeleteLine_ExistingLine_LineDeletedOutput()
        {
            var existingLineId = Guid.Parse("11cbae99-5cc0-4268-8d3f-31961822133c");
            var interactor = CreateInteractor();
            _outputPortMock.Setup(m => m.LineDeleted(It.IsAny<Guid>()));

            _changeLogDaoStub.ChangeLogs.Add(new ChangeLogLine(existingLineId, null, TestAccount.Product.Id,
                ChangeLogText.Parse("some feature added"), 0, TestAccount.UserId, DateTime.Parse("2021-05-13")));

            await interactor.ExecuteAsync(_outputPortMock.Object, existingLineId);

            _outputPortMock.Verify(m => m.LineDeleted(It.Is<Guid>(x => x == existingLineId)), Times.Once);
        }

        [Fact]
        public async Task DeleteLine_ConflictWhenDelete_ConflictOutput()
        {
            var existingLineId = Guid.Parse("11cbae99-5cc0-4268-8d3f-31961822133c");
            var interactor = CreateInteractor();
            _outputPortMock.Setup(m => m.Conflict(It.IsAny<Conflict>()));

            _changeLogDaoStub.ChangeLogs.Add(new ChangeLogLine(existingLineId, null, TestAccount.Product.Id,
                ChangeLogText.Parse("some feature added"), 0, TestAccount.UserId, DateTime.Parse("2021-05-13")));
            _changeLogDaoStub.Conflict = new ConflictStub();

            await interactor.ExecuteAsync(_outputPortMock.Object, existingLineId);

            _outputPortMock.Verify(m => m.Conflict(It.IsAny<Conflict>()), Times.Once);
        }
    }
}