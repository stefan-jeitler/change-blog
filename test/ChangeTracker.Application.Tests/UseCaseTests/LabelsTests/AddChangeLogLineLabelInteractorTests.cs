using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.Labels.AddChangeLogLineLabel;
using ChangeTracker.Application.UseCases.Labels.SharedModels;
using ChangeTracker.Domain.ChangeLog;
using ChangeTracker.Domain.Version;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.LabelsTests
{
    public class AddChangeLogLineLabelInteractorTests
    {
        private readonly ChangeLogDaoStub _changeLogDaoStub;
        private readonly Mock<IAddChangeLogLineLabelOutputPort> _outputPortMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly VersionDaoStub _versionDaoStub;

        public AddChangeLogLineLabelInteractorTests()
        {
            _outputPortMock = new Mock<IAddChangeLogLineLabelOutputPort>(MockBehavior.Strict);
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _changeLogDaoStub = new ChangeLogDaoStub();
            _versionDaoStub = new VersionDaoStub();
        }

        private AddChangeLogLineLabelInteractor CreateInteractor() =>
            new(_unitOfWorkMock.Object, _changeLogDaoStub, _versionDaoStub);

        [Fact]
        public async Task AddLabel_HappyPath_Successful()
        {
            // arrange
            var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
            var requestModel = new ChangeLogLineLabelRequestModel(lineId, "SomeLabel");
            var addLabelInteractor = CreateInteractor();

            _changeLogDaoStub.ChangeLogs.Add(new ChangeLogLine(lineId, null, TestAccount.Project.Id,
                ChangeLogText.Parse("Some text"), 0U, DateTime.Parse("2021-04-17")));
            _outputPortMock.Setup(m => m.Added(It.IsAny<Guid>()));

            // act
            await addLabelInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

            // assert
            _outputPortMock.Verify(m => m.Added(It.Is<Guid>(x => x == lineId)));
            _unitOfWorkMock.Verify(m => m.Start(), Times.Once);
            _unitOfWorkMock.Verify(m => m.Commit(), Times.Once);
        }

        [Fact]
        public async Task AddLabel_InvalidLabel_InvalidLabelOutput()
        {
            // arrange
            var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
            var requestModel = new ChangeLogLineLabelRequestModel(lineId, "Some Label");
            var addLabelInteractor = CreateInteractor();

            _changeLogDaoStub.ChangeLogs.Add(new ChangeLogLine(lineId, null, TestAccount.Project.Id,
                ChangeLogText.Parse("Some text"), 0U, DateTime.Parse("2021-04-17")));
            _outputPortMock.Setup(m => m.InvalidLabel(It.IsAny<string>()));

            // act
            await addLabelInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

            // assert
            _outputPortMock.Verify(m => m.InvalidLabel(It.Is<string>(x => x == "Some Label")));
        }

        [Fact]
        public async Task AddLabel_ConflictWhileSaving_ConflictOutput()
        {
            // arrange
            var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
            var requestModel = new ChangeLogLineLabelRequestModel(lineId, "SomeLabel");
            var addLabelInteractor = CreateInteractor();

            _changeLogDaoStub.ChangeLogs.Add(new ChangeLogLine(lineId, null, TestAccount.Project.Id,
                ChangeLogText.Parse("Some text"), 0U, DateTime.Parse("2021-04-17")));
            _outputPortMock.Setup(m => m.Conflict(It.IsAny<string>()));

            _changeLogDaoStub.ProduceConflict = true;

            // act
            await addLabelInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

            // assert
            _outputPortMock.Verify(m => m.Conflict(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task AddLabel_ExistingLineWithMaxLabels_MaxLabelsReachedOutput()
        {
            // arrange
            var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
            var requestModel = new ChangeLogLineLabelRequestModel(lineId, "SomeLabel");
            var addLabelInteractor = CreateInteractor();

            var existingLabels =
                new List<string> {"Feature", "Bug", "Security", "Deprecated", "Added"}.Select(Label.Parse);
            _changeLogDaoStub.ChangeLogs.Add(new ChangeLogLine(lineId, null, TestAccount.Project.Id,
                ChangeLogText.Parse("Some text"), 0U, DateTime.Parse("2021-04-17"), existingLabels, Array.Empty<Issue>()));
            _outputPortMock.Setup(m => m.MaxLabelsReached(It.IsAny<int>()));

            _changeLogDaoStub.ProduceConflict = true;

            // act
            await addLabelInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

            // assert
            _outputPortMock.Verify(m => m.MaxLabelsReached(It.Is<int>(x => x == ChangeLogLine.MaxLabels)), Times.Once);
        }

        [Fact]
        public async Task AddLabel_NotExistingChangeLogLine_ChangeLogLineDoesNotExistOutput()
        {
            // arrange
            var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
            var requestModel = new ChangeLogLineLabelRequestModel(lineId, "SomeLabel");
            var addLabelInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.ChangeLogLineDoesNotExist());

            // act
            await addLabelInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

            // assert
            _outputPortMock.Verify(m => m.ChangeLogLineDoesNotExist());
        }

        [Fact]
        public async Task AddLabel_LineBelongsToReleasedVersion_RelatedVersionAlreadyReleasedOutput()
        {
            // arrange
            var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
            var versionId = Guid.Parse("1d7831d5-32fb-437f-a9d5-bf5a7dd34b10");
            var requestModel = new ChangeLogLineLabelRequestModel(lineId, "SomeLabel");
            var addLabelInteractor = CreateInteractor();

            _changeLogDaoStub.ChangeLogs.Add(new ChangeLogLine(lineId, versionId, TestAccount.Project.Id,
                ChangeLogText.Parse("Some text"), 0U, DateTime.Parse("2021-04-17")));
            _versionDaoStub.Versions.Add(new ClVersion(versionId, TestAccount.Project.Id, ClVersionValue.Parse("1.2.3"),
                DateTime.Parse("2021-04-18"), DateTime.Parse("2021-04-18"), null));

            _outputPortMock.Setup(m => m.RelatedVersionAlreadyReleased());

            // act
            await addLabelInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

            // assert
            _outputPortMock.Verify(m => m.RelatedVersionAlreadyReleased());
        }

        [Fact]
        public async Task AddLabel_LineBelongsToDeletedVersion_RelatedVersionDeletedOutput()
        {
            // arrange
            var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
            var versionId = Guid.Parse("1d7831d5-32fb-437f-a9d5-bf5a7dd34b10");
            var requestModel = new ChangeLogLineLabelRequestModel(lineId, "SomeLabel");
            var addLabelInteractor = CreateInteractor();

            _changeLogDaoStub.ChangeLogs.Add(new ChangeLogLine(lineId, versionId, TestAccount.Project.Id,
                ChangeLogText.Parse("Some text"), 0U, DateTime.Parse("2021-04-17")));
            _versionDaoStub.Versions.Add(new ClVersion(versionId, TestAccount.Project.Id, ClVersionValue.Parse("1.2.3"),
                null, DateTime.Parse("2021-04-18"), DateTime.Parse("2021-04-18")));

            _outputPortMock.Setup(m => m.RelatedVersionDeleted());

            // act
            await addLabelInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

            // assert
            _outputPortMock.Verify(m => m.RelatedVersionDeleted());
        }
    }
}