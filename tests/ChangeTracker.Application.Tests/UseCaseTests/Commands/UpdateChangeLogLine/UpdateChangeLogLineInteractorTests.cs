using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.Commands.UpdateChangeLogLine;
using ChangeTracker.Domain.ChangeLog;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.Commands.UpdateChangeLogLine
{
    public class UpdateChangeLogLineInteractorTests
    {
        private readonly ChangeLogDaoStub _changeLogDaoStub;
        private readonly Mock<IUpdateLineOutputPort> _outputPortMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public UpdateChangeLogLineInteractorTests()
        {
            _outputPortMock = new Mock<IUpdateLineOutputPort>(MockBehavior.Strict);
            _changeLogDaoStub = new ChangeLogDaoStub();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
        }

        private UpdateChangeLogLineInteractor CreateInteractor() =>
            new(_changeLogDaoStub, _changeLogDaoStub, _unitOfWorkMock.Object);

        [Fact]
        public async Task UpdateLine_HappyPath_Successful()
        {
            // arrange
            var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
            const string text = "feature added";
            var requestModel =
                new ChangeLogLineRequestModel(lineId, text, Array.Empty<string>(), Array.Empty<string>());
            var updateLineInteractor = CreateInteractor();

            _changeLogDaoStub.ChangeLogs.Add(new ChangeLogLine(lineId, null, TestAccount.Product.Id,
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
        public async Task UpdateLine_ConflictWhileSaving_ConflictOutput()
        {
            // arrange
            var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
            const string text = "feature added";
            var requestModel =
                new ChangeLogLineRequestModel(lineId, text, Array.Empty<string>(), Array.Empty<string>());
            var updateLineInteractor = CreateInteractor();

            _changeLogDaoStub.ChangeLogs.Add(new ChangeLogLine(lineId, null, TestAccount.Product.Id,
                ChangeLogText.Parse("some feature added"), 0, DateTime.Parse("2021-04-17"), Array.Empty<Label>(),
                Array.Empty<Issue>(), TestAccount.UserId));

            _changeLogDaoStub.ProduceConflict = true;

            _outputPortMock.Setup(m => m.Conflict(It.IsAny<string>()));

            // act
            await updateLineInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

            // assert
            _outputPortMock.Verify(m => m.Conflict(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task UpdateLine_WithTextThatAlreadyExist_Output()
        {
            // arrange
            var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
            const string text = "feature added";
            var requestModel =
                new ChangeLogLineRequestModel(lineId, text, Array.Empty<string>(), Array.Empty<string>());
            var updateLineInteractor = CreateInteractor();

            _changeLogDaoStub.ChangeLogs.Add(new ChangeLogLine(lineId, null, TestAccount.Product.Id,
                ChangeLogText.Parse("some feature added"), 0, DateTime.Parse("2021-04-17"), Array.Empty<Label>(),
                Array.Empty<Issue>(), TestAccount.UserId));
            _changeLogDaoStub.ChangeLogs.Add(new ChangeLogLine(null, TestAccount.Product.Id,
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
                new ChangeLogLineRequestModel(lineId, text, Array.Empty<string>(), Array.Empty<string>());
            var updateLineInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.ChangeLogLineDoesNotExist());

            // act
            await updateLineInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

            // assert
            _outputPortMock.Verify(m => m.ChangeLogLineDoesNotExist(), Times.Once);
        }
    }
}