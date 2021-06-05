using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.Commands.Labels.AddChangeLogLineLabel;
using ChangeTracker.Application.UseCases.Commands.Labels.SharedModels;
using ChangeTracker.Domain.ChangeLog;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.Commands.LabelsTests
{
    public class AddChangeLogLineLabelInteractorTests
    {
        private readonly ChangeLogDaoStub _changeLogDaoStub;
        private readonly Mock<IAddChangeLogLineLabelOutputPort> _outputPortMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public AddChangeLogLineLabelInteractorTests()
        {
            _outputPortMock = new Mock<IAddChangeLogLineLabelOutputPort>(MockBehavior.Strict);
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _changeLogDaoStub = new ChangeLogDaoStub();
        }

        private AddChangeLogLineLabelInteractor CreateInteractor() =>
            new(_unitOfWorkMock.Object, _changeLogDaoStub, _changeLogDaoStub);

        [Fact]
        public async Task AddLabel_HappyPath_LabelAddedAndUowCommitted()
        {
            // arrange
            var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
            var label = Label.Parse("SomeLabel");
            var requestModel = new ChangeLogLineLabelRequestModel(lineId, label.Value);
            var addLabelInteractor = CreateInteractor();

            _changeLogDaoStub.ChangeLogs.Add(new ChangeLogLine(lineId, null, TestAccount.Product.Id,
                ChangeLogText.Parse("Some text"), 0U, TestAccount.UserId, DateTime.Parse("2021-04-17")));
            _outputPortMock.Setup(m => m.Added(It.IsAny<Guid>()));

            // act
            await addLabelInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

            // assert
            _outputPortMock.Verify(m => m.Added(It.Is<Guid>(x => x == lineId)));
            _unitOfWorkMock.Verify(m => m.Start(), Times.Once);
            _unitOfWorkMock.Verify(m => m.Commit(), Times.Once);
            _changeLogDaoStub.ChangeLogs.Single().Labels.Should().ContainSingle(x => x == label);
        }

        [Fact]
        public async Task AddLabel_InvalidLabel_InvalidLabelOutput()
        {
            // arrange
            var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
            var requestModel = new ChangeLogLineLabelRequestModel(lineId, "Some Label");
            var addLabelInteractor = CreateInteractor();

            _changeLogDaoStub.ChangeLogs.Add(new ChangeLogLine(lineId, null, TestAccount.Product.Id,
                ChangeLogText.Parse("Some text"), 0U, TestAccount.UserId, DateTime.Parse("2021-04-17")));
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

            _changeLogDaoStub.ChangeLogs.Add(new ChangeLogLine(lineId, null, TestAccount.Product.Id,
                ChangeLogText.Parse("Some text"), 0U, TestAccount.UserId, DateTime.Parse("2021-04-17")));
            _outputPortMock.Setup(m => m.Conflict(It.IsAny<Conflict>()));

            _changeLogDaoStub.Conflict = new ConflictStub();

            // act
            await addLabelInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

            // assert
            _outputPortMock.Verify(m => m.Conflict(It.IsAny<Conflict>()), Times.Once);
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
            _changeLogDaoStub.ChangeLogs.Add(new ChangeLogLine(lineId, null, TestAccount.Product.Id,
                ChangeLogText.Parse("Some text"), 0U, DateTime.Parse("2021-04-17"), existingLabels,
                Array.Empty<Issue>(), TestAccount.UserId));
            _outputPortMock.Setup(m => m.MaxLabelsReached(It.IsAny<int>()));

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
    }
}