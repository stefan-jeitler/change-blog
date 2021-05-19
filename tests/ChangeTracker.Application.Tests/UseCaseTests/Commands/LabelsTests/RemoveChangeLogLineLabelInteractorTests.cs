using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.Commands.Labels.RemoveChangeLogLineLabel;
using ChangeTracker.Application.UseCases.Commands.Labels.SharedModels;
using ChangeTracker.Domain.ChangeLog;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.Commands.LabelsTests
{
    public class RemoveChangeLogLineLabelInteractorTests
    {
        private readonly ChangeLogDaoStub _changeLogDaoStub;
        private readonly Mock<IRemoveChangeLogLineLabelOutputPort> _outputPortMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public RemoveChangeLogLineLabelInteractorTests()
        {
            _outputPortMock = new Mock<IRemoveChangeLogLineLabelOutputPort>(MockBehavior.Strict);
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _changeLogDaoStub = new ChangeLogDaoStub();
        }

        private RemoveChangeLogLineLabelInteractor CreateInteractor()
        {
            return new(_unitOfWorkMock.Object, _changeLogDaoStub, _changeLogDaoStub);
        }

        [Fact]
        public async Task RemoveLabel_HappyPath_LabelRemovedAndUowCommitted()
        {
            // arrange
            var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
            var label = Label.Parse("SomeLabel");
            var requestModel = new ChangeLogLineLabelRequestModel(lineId, label.Value);
            var removeLabelInteractor = CreateInteractor();

            var line = new ChangeLogLine(lineId, null, TestAccount.Product.Id, ChangeLogText.Parse("some valid text"),
                0U, DateTime.Parse("2021-04-19"), new List<Label> {label}, Array.Empty<Issue>());
            _changeLogDaoStub.ChangeLogs.Add(line);
            _outputPortMock.Setup(m => m.Removed(It.IsAny<Guid>()));

            // act
            await removeLabelInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

            // assert
            _outputPortMock.Verify(m => m.Removed(It.Is<Guid>(x => x == lineId)), Times.Once);
            _unitOfWorkMock.Verify(m => m.Start(), Times.Once);
            _unitOfWorkMock.Verify(m => m.Commit(), Times.Once);
            _changeLogDaoStub.ChangeLogs.Single().Labels.Should().BeEmpty();
        }

        [Fact]
        public async Task RemoveLabel_InvalidLabel_InvalidLabelOutput()
        {
            // arrange
            var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
            var requestModel = new ChangeLogLineLabelRequestModel(lineId, "Some Label");
            var removeLabelInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.InvalidLabel(It.IsAny<string>()));

            // act
            await removeLabelInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

            // assert
            _outputPortMock.Verify(m => m.InvalidLabel(It.Is<string>(x => x == "Some Label")));
        }

        [Fact]
        public async Task RemoveLabel_NoLabelExists_NotModifiedOutput()
        {
            // arrange
            var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
            var requestModel = new ChangeLogLineLabelRequestModel(lineId, "Some Label");
            var removeLabelInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.InvalidLabel(It.IsAny<string>()));

            // act
            await removeLabelInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

            // assert
            _outputPortMock.Verify(m => m.InvalidLabel(It.Is<string>(x => x == "Some Label")));
        }

        [Fact]
        public async Task RemoveLabel_ConflictWhileSaving_ConflictOutput()
        {
            // arrange
            var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
            var requestModel = new ChangeLogLineLabelRequestModel(lineId, "SomeLabel");
            var addLabelInteractor = CreateInteractor();

            _changeLogDaoStub.ChangeLogs.Add(new ChangeLogLine(lineId, null, TestAccount.Product.Id,
                ChangeLogText.Parse("Some text"), 0U, DateTime.Parse("2021-04-17")));
            _outputPortMock.Setup(m => m.Conflict(It.IsAny<string>()));

            _changeLogDaoStub.ProduceConflict = true;

            // act
            await addLabelInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

            // assert
            _outputPortMock.Verify(m => m.Conflict(It.IsAny<string>()), Times.Once);
        }


        [Fact]
        public async Task RemoveLabel_NotExistingChangeLogLine_ChangeLogLineDoesNotExistOutput()
        {
            // arrange
            var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
            var requestModel = new ChangeLogLineLabelRequestModel(lineId, "SomeLabel");
            var removeLabelInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.ChangeLogLineDoesNotExist());

            // act
            await removeLabelInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

            // assert
            _outputPortMock.Verify(m => m.ChangeLogLineDoesNotExist(), Times.Once);
        }
    }
}