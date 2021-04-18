using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.UpdateChangeLogLine;
using ChangeTracker.Domain.ChangeLog;
using ChangeTracker.Domain.Version;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.UpdateChangeLogLine
{
    public class UpdateChangeLogLineInteractorTests
    {
        private readonly ChangeLogDaoStub _changeLogDaoStub;
        private readonly Mock<IUpdateLineOutputPort> _outputPortMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly VersionDaoStub _versionDaoStub;

        public UpdateChangeLogLineInteractorTests()
        {
            _outputPortMock = new Mock<IUpdateLineOutputPort>(MockBehavior.Strict);
            _changeLogDaoStub = new ChangeLogDaoStub();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _versionDaoStub = new VersionDaoStub();
        }

        private UpdateChangeLogLineInteractor CreateInteractor() =>
            new(_changeLogDaoStub, _unitOfWorkMock.Object, _versionDaoStub);

        [Fact]
        public async Task UpdateLine_HappyPath_Successful()
        {
            // arrange
            var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
            const string text = "feature added";
            var requestModel =
                new ChangeLogLineRequestModel(lineId, text, Array.Empty<string>(), Array.Empty<string>());
            var updateLineInteractor = CreateInteractor();

            _changeLogDaoStub.ChangeLogs.Add(new ChangeLogLine(lineId, null, TestAccount.Project.Id,
                ChangeLogText.Parse("some feature added"), 0, DateTime.Parse("2021-04-17"), Array.Empty<Label>(),
                Array.Empty<Issue>()));

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

            _changeLogDaoStub.ChangeLogs.Add(new ChangeLogLine(lineId, null, TestAccount.Project.Id,
                ChangeLogText.Parse("some feature added"), 0, DateTime.Parse("2021-04-17"), Array.Empty<Label>(),
                Array.Empty<Issue>()));

            _changeLogDaoStub.ProduceConflict = true;

            _outputPortMock.Setup(m => m.Conflict(It.IsAny<string>()));

            // act
            await updateLineInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

            // assert
            _outputPortMock.Verify(m => m.Conflict(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task UpdateLine_NotExistingLine_ChangeLogLineDoesNotExistOutput()
        {
            // arrange
            var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
            const string text = "some features added";
            var requestModel = new ChangeLogLineRequestModel(lineId, text, Array.Empty<string>(), Array.Empty<string>());
            var updateLineInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.ChangeLogLineDoesNotExist());

            // act
            await updateLineInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

            // assert
            _outputPortMock.Verify(m => m.ChangeLogLineDoesNotExist(), Times.Once);
        }

        [Fact]
        public async Task UpdateLine_WithoutChanges_NotModifiedOutput()
        {
            // arrange
            var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
            const string text = "some features added";
            var labels = new List<string> {"FirstLabel", "SecondLabel"};
            var requestModel = new ChangeLogLineRequestModel(lineId, text, labels, Array.Empty<string>());
            var updateLineInteractor = CreateInteractor();

            _changeLogDaoStub.ChangeLogs.Add(new ChangeLogLine(lineId, null, TestAccount.Project.Id,
                ChangeLogText.Parse(text), 0, DateTime.Parse("2021-04-17"), labels.Select(Label.Parse),
                Array.Empty<Issue>()));

            _outputPortMock.Setup(m => m.NotModified());

            // act
            await updateLineInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

            // assert
            _outputPortMock.Verify(m => m.NotModified(), Times.Once);
        }

        [Fact]
        public async Task UpdateLine_VersionAlreadyReleased_VersionAlreadyReleasedOutput()
        {
            // arrange
            var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
            var versionId = Guid.Parse("db9a0e15-ed35-428b-acbb-ab0e769a601b");
            const string text = "some features added";
            var labels = new List<string> {"FirstLabel", "SecondLabel"};
            var requestModel = new ChangeLogLineRequestModel(lineId, text, labels, Array.Empty<string>());
            var updateLineInteractor = CreateInteractor();

            _changeLogDaoStub.ChangeLogs.Add(new ChangeLogLine(lineId, versionId, TestAccount.Project.Id,
                ChangeLogText.Parse(text), 0, DateTime.Parse("2021-04-17"), labels.Select(Label.Parse),
                Array.Empty<Issue>()));

            _versionDaoStub.Versions.Add(new ClVersion(versionId, TestAccount.Project.Id, ClVersionValue.Parse("1.2.3"),
                DateTime.Parse("2021-04-17"), DateTime.Parse("2021-04-07"), null));

            _outputPortMock.Setup(m => m.RelatedVersionAlreadyReleased());

            // act
            await updateLineInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

            // assert
            _outputPortMock.Verify(m => m.RelatedVersionAlreadyReleased(), Times.Once);
        }

        [Fact]
        public async Task UpdateLine_VersionDeleted_VersionDeletedOutput()
        {
            // arrange
            var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
            var versionId = Guid.Parse("db9a0e15-ed35-428b-acbb-ab0e769a601b");
            const string text = "some features added";
            var labels = new List<string> {"FirstLabel", "SecondLabel"};
            var requestModel = new ChangeLogLineRequestModel(lineId, text, labels, Array.Empty<string>());
            var updateLineInteractor = CreateInteractor();

            _changeLogDaoStub.ChangeLogs.Add(new ChangeLogLine(lineId, versionId, TestAccount.Project.Id,
                ChangeLogText.Parse(text), 0, DateTime.Parse("2021-04-17"), labels.Select(Label.Parse),
                Array.Empty<Issue>()));

            _versionDaoStub.Versions.Add(new ClVersion(versionId, TestAccount.Project.Id, ClVersionValue.Parse("1.2.3"),
                null, DateTime.Parse("2021-04-07"), DateTime.Parse("2021-01-17")));

            _outputPortMock.Setup(m => m.RelatedVersionDeleted());

            // act
            await updateLineInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

            // assert
            _outputPortMock.Verify(m => m.RelatedVersionDeleted(), Times.Once);
        }
    }
}