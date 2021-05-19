using System;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.Commands.MakeChangeLogLinePending;
using ChangeTracker.Domain.ChangeLog;
using ChangeTracker.Domain.Version;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.Commands.MakeChangeLogLinePending
{
    public class MakeChangeLogLinePendingInteractorTests
    {
        private readonly ChangeLogDaoStub _changeLogDaoStub;
        private readonly Mock<IMakeChangeLogLinePendingOutputPort> _outputPortMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly VersionDaoStub _versionDaoStub;

        public MakeChangeLogLinePendingInteractorTests()
        {
            _versionDaoStub = new VersionDaoStub();
            _changeLogDaoStub = new ChangeLogDaoStub();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _outputPortMock = new Mock<IMakeChangeLogLinePendingOutputPort>(MockBehavior.Strict);
        }

        private MakeChangeLogLinePendingInteractor CreateInteractor()
        {
            return new(_versionDaoStub, _changeLogDaoStub,
                _changeLogDaoStub, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task MakeLinePending_HappyPath_SuccessfullyAndUowStartedAndCommitted()
        {
            // arrange
            var clVersion = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.2.3"));
            _versionDaoStub.Versions.Add(clVersion);

            var changeLogLine = new ChangeLogLine(clVersion.Id, TestAccount.Product.Id,
                ChangeLogText.Parse("some text"), 0);
            _changeLogDaoStub.ChangeLogs.Add(changeLogLine);

            var makeLinePendingInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.WasMadePending(It.IsAny<Guid>()));

            // act
            await makeLinePendingInteractor.ExecuteAsync(_outputPortMock.Object, changeLogLine.Id);

            // assert
            _unitOfWorkMock.Verify(x => x.Start(), Times.Once);
            _unitOfWorkMock.Verify(x => x.Commit(), Times.Once);
            _changeLogDaoStub.ChangeLogs.Single().VersionId.Should().NotHaveValue();
        }

        [Fact]
        public void MakeLinePending_LineIdIsEmpty_ArgumentException()
        {
            var makeLinePendingInteractor = CreateInteractor();

            Func<Task> act = () => makeLinePendingInteractor.ExecuteAsync(_outputPortMock.Object, Guid.Empty);

            act.Should().ThrowExactly<ArgumentException>();
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
                ChangeLogText.Parse("some text"), 0);
            _changeLogDaoStub.ChangeLogs.Add(changeLogLine);

            var makeLinePendingInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.ChangeLogLineIsAlreadyPending());

            // act
            await makeLinePendingInteractor.ExecuteAsync(_outputPortMock.Object, changeLogLine.Id);

            // assert
            _outputPortMock.Verify(m => m.ChangeLogLineIsAlreadyPending(), Times.Once);
        }

        [Fact]
        public async Task MakeLinePending_VersionIsAlreadyReleased_VersionAlreadyReleasedOutput()
        {
            // arrange
            var clVersion = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.2.3"),
                DateTime.Parse("2021-04-18"));
            _versionDaoStub.Versions.Add(clVersion);

            var changeLogLine = new ChangeLogLine(clVersion.Id, TestAccount.Product.Id,
                ChangeLogText.Parse("some text"), 0);
            _changeLogDaoStub.ChangeLogs.Add(changeLogLine);

            var makeLinePendingInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.VersionAlreadyReleased());

            // act
            await makeLinePendingInteractor.ExecuteAsync(_outputPortMock.Object, changeLogLine.Id);

            // assert
            _outputPortMock.Verify(m => m.VersionAlreadyReleased(), Times.Once);
        }

        [Fact]
        public async Task MakeLinePending_VersionIsClosed_VersionClosedOutput()
        {
            // arrange
            var clVersion = new ClVersion(Guid.NewGuid(), TestAccount.Product.Id,
                ClVersionValue.Parse("1.2.3"), null, DateTime.Parse("2021-04-18"),
                DateTime.Parse("2021-04-18"));

            _versionDaoStub.Versions.Add(clVersion);

            var changeLogLine = new ChangeLogLine(clVersion.Id, TestAccount.Product.Id,
                ChangeLogText.Parse("some text"), 0);
            _changeLogDaoStub.ChangeLogs.Add(changeLogLine);

            var makeLinePendingInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.VersionClosed());

            // act
            await makeLinePendingInteractor.ExecuteAsync(_outputPortMock.Object, changeLogLine.Id);

            // assert
            _outputPortMock.Verify(m => m.VersionClosed(), Times.Once);
        }

        [Fact]
        public async Task MakeLinePending_TooManyPendingLinesExists_TooManyPendingLinesOutput()
        {
            // arrange
            var clVersion = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.2.3"));
            _versionDaoStub.Versions.Add(clVersion);

            var changeLogLine = new ChangeLogLine(clVersion.Id, TestAccount.Product.Id,
                ChangeLogText.Parse("some text"), 0);
            _changeLogDaoStub.ChangeLogs.Add(changeLogLine);

            _changeLogDaoStub.ChangeLogs.AddRange(Enumerable.Range(0, 100)
                .Select(x => new ChangeLogLine(null, TestAccount.Product.Id,
                    ChangeLogText.Parse($"{x:D5}"), 0)));

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
            var clVersion = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.2.3"));
            _versionDaoStub.Versions.Add(clVersion);

            var changeLogLine = new ChangeLogLine(clVersion.Id, TestAccount.Product.Id,
                ChangeLogText.Parse("some text"), 0);
            _changeLogDaoStub.ChangeLogs.Add(changeLogLine);

            var makeLinePendingInteractor = CreateInteractor();
            _changeLogDaoStub.ProduceConflict = true;

            _outputPortMock.Setup(m => m.Conflict(It.IsAny<string>()));

            // act
            await makeLinePendingInteractor.ExecuteAsync(_outputPortMock.Object, changeLogLine.Id);

            // assert
            _outputPortMock.Verify(m => m.Conflict(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task MakeLinePending_PendingLineWithSameTextExists_ConflictOutput()
        {
            // arrange
            var clVersion = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.2.3"));
            _versionDaoStub.Versions.Add(clVersion);

            var changeLogLine = new ChangeLogLine(clVersion.Id, TestAccount.Product.Id,
                ChangeLogText.Parse("some text"), 0);
            _changeLogDaoStub.ChangeLogs.Add(changeLogLine);
            _changeLogDaoStub.ChangeLogs.Add(new ChangeLogLine(null, TestAccount.Product.Id,
                ChangeLogText.Parse("some text"), 0));

            var makeLinePendingInteractor = CreateInteractor();
            _changeLogDaoStub.ProduceConflict = true;

            _outputPortMock.Setup(m => m.LineWithSameTextAlreadyExists(It.IsAny<string>()));

            // act
            await makeLinePendingInteractor.ExecuteAsync(_outputPortMock.Object, changeLogLine.Id);

            // assert
            _outputPortMock.Verify(m => m.LineWithSameTextAlreadyExists(It.Is<string>(x => x == "some text")),
                Times.Once);
        }
    }
}