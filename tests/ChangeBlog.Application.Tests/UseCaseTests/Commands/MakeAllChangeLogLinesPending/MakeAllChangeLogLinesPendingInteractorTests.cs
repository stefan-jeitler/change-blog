using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ChangeBlog.Application.DataAccess;
using ChangeBlog.Application.Tests.TestDoubles;
using ChangeBlog.Application.UseCases.Commands.MakeAllChangeLogLinesPending;
using ChangeBlog.Domain.ChangeLog;
using ChangeBlog.Domain.Common;
using ChangeBlog.Domain.Version;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.Commands.MakeAllChangeLogLinesPending
{
    public class MakeAllChangeLogLinesPendingInteractorTests
    {
        private readonly ChangeLogDaoStub _changeLogDaoStub;
        private readonly Mock<IMakeAllChangeLogLinesPendingOutputPort> _outputPortMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly VersionDaoStub _versionDaoStub;

        public MakeAllChangeLogLinesPendingInteractorTests()
        {
            _outputPortMock = new Mock<IMakeAllChangeLogLinesPendingOutputPort>(MockBehavior.Strict);
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _versionDaoStub = new VersionDaoStub();
            _changeLogDaoStub = new ChangeLogDaoStub();
        }

        private MakeAllChangeLogLinesPendingInteractor CreateInteractor() => new(_versionDaoStub, _changeLogDaoStub,
            _changeLogDaoStub, _unitOfWorkMock.Object);

        [Fact]
        public void MakeAllLinesPending_EmptyVersionId_ArgumentException()
        {
            var makeAllLinesPendingInteractor = CreateInteractor();

            Func<Task> act = () => makeAllLinesPendingInteractor.ExecuteAsync(_outputPortMock.Object, Guid.Empty);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void MakeAllLinesPending_EmptyProductId_ArgumentException()
        {
            var makeAllLinesPendingInteractor = CreateInteractor();

            Func<Task> act = () =>
                makeAllLinesPendingInteractor.ExecuteAsync(_outputPortMock.Object, Guid.Empty, "1.2.3");

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void MakeAllLinesPending_VersionIsNull_ArgumentNullException()
        {
            var makeAllLinesPendingInteractor = CreateInteractor();

            Func<Task> act = () =>
                makeAllLinesPendingInteractor.ExecuteAsync(_outputPortMock.Object, TestAccount.Product.Id, null);

            act.Should().ThrowExactly<ArgumentNullException>();
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

            _versionDaoStub.Versions.Add(clVersion);
            _outputPortMock.Setup(m => m.VersionAlreadyReleased(It.IsAny<Guid>()));

            // act
            await makeAllLinesPendingInteractor.ExecuteAsync(_outputPortMock.Object, clVersion.Id);

            // assert
            _outputPortMock.Verify(m => m.VersionAlreadyReleased(It.Is<Guid>(x => x == clVersion.Id)), Times.Once);
        }

        [Fact]
        public async Task MakeAllLinesPending_DeletedClosed_VersionDeletedOutput()
        {
            // arrange
            var makeAllLinesPendingInteractor = CreateInteractor();

            var clVersion = new ClVersion(Guid.NewGuid(), TestAccount.Product.Id, ClVersionValue.Parse("1.2.3"),
                OptionalName.Empty,
                null, TestAccount.UserId, DateTime.Parse("2021-04-24"), DateTime.Parse("2021-04-24"));

            _versionDaoStub.Versions.Add(clVersion);
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
            _changeLogDaoStub.ChangeLogs.Add(versionLine);
            var pendingLines = Enumerable.Range(0, 100)
                .Select(x => new ChangeLogLine(null, TestAccount.Product.Id, ChangeLogText.Parse($"{x:D5}"), (uint) x,
                    TestAccount.UserId));
            _changeLogDaoStub.ChangeLogs.AddRange(pendingLines);

            _versionDaoStub.Versions.Add(clVersion);
            _outputPortMock.Setup(m => m.TooManyPendingLines(It.IsAny<int>()));

            // act
            await makeAllLinesPendingInteractor.ExecuteAsync(_outputPortMock.Object, clVersion.Id);

            // assert
            _outputPortMock.Verify(
                m => m.TooManyPendingLines(It.Is<int>(x => x == ChangeLogs.MaxLines)),
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
            _changeLogDaoStub.ChangeLogs.Add(versionLine1);
            _changeLogDaoStub.ChangeLogs.Add(versionLine2);
            var pendingLines = Enumerable.Range(0, 98)
                .Select(x => new ChangeLogLine(null, TestAccount.Product.Id, ChangeLogText.Parse($"{x:D5}"), (uint) x,
                    TestAccount.UserId));
            _changeLogDaoStub.ChangeLogs.AddRange(pendingLines);

            _versionDaoStub.Versions.Add(clVersion);
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
            _changeLogDaoStub.ChangeLogs.Add(versionLine1);
            _changeLogDaoStub.Conflict = new ConflictStub();
            _versionDaoStub.Versions.Add(clVersion);
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
            _changeLogDaoStub.ChangeLogs.Add(versionLine);
            _versionDaoStub.Versions.Add(clVersion);
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
}
