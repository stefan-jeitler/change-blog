using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeBlog.Application.DataAccess;
using ChangeBlog.Application.Tests.TestDoubles;
using ChangeBlog.Application.UseCases.Commands.Labels.DeleteChangeLogLineLabel;
using ChangeBlog.Application.UseCases.Commands.Labels.SharedModels;
using ChangeBlog.Domain.ChangeLog;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.Commands.LabelsTests
{
    public class RemoveChangeLogLineLabelInteractorTests
    {
        private readonly FakeChangeLogDao _fakeChangeLogDao;
        private readonly Mock<IDeleteChangeLogLineLabelOutputPort> _outputPortMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public RemoveChangeLogLineLabelInteractorTests()
        {
            _outputPortMock = new Mock<IDeleteChangeLogLineLabelOutputPort>(MockBehavior.Strict);
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _fakeChangeLogDao = new FakeChangeLogDao();
        }

        private DeleteChangeLogLineLabelInteractor CreateInteractor() =>
            new(_unitOfWorkMock.Object, _fakeChangeLogDao, _fakeChangeLogDao);

        [Fact]
        public async Task RemoveLabel_HappyPath_LabelRemovedAndUowCommitted()
        {
            // arrange
            var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
            var label = Label.Parse("SomeLabel");
            var requestModel = new ChangeLogLineLabelRequestModel(lineId, label.Value);
            var removeLabelInteractor = CreateInteractor();

            var line = new ChangeLogLine(lineId, null, TestAccount.Product.Id, ChangeLogText.Parse("some valid text"),
                0U, DateTime.Parse("2021-04-19"), new List<Label> {label}, Array.Empty<Issue>(), TestAccount.UserId);
            _fakeChangeLogDao.ChangeLogs.Add(line);
            _outputPortMock.Setup(m => m.Deleted(It.IsAny<Guid>()));

            // act
            await removeLabelInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

            // assert
            _outputPortMock.Verify(m => m.Deleted(It.Is<Guid>(x => x == lineId)), Times.Once);
            _unitOfWorkMock.Verify(m => m.Start(), Times.Once);
            _unitOfWorkMock.Verify(m => m.Commit(), Times.Once);
            _fakeChangeLogDao.ChangeLogs.Single().Labels.Should().BeEmpty();
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

            _fakeChangeLogDao.ChangeLogs.Add(new ChangeLogLine(lineId, null, TestAccount.Product.Id,
                ChangeLogText.Parse("Some text"), 0U, TestAccount.UserId, DateTime.Parse("2021-04-17")));
            _outputPortMock.Setup(m => m.Conflict(It.IsAny<Conflict>()));

            _fakeChangeLogDao.Conflict = new ConflictStub();

            // act
            await addLabelInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

            // assert
            _outputPortMock.Verify(m => m.Conflict(It.IsAny<Conflict>()), Times.Once);
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
