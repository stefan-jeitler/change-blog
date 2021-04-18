using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.Labels.RemoveChangeLogLineLabel;
using Moq;

namespace ChangeTracker.Application.Tests.UseCaseTests.LabelsTests
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

        private RemoveChangeLogLineLabelInteractor CreateInteractor() => new(_unitOfWorkMock.Object, _changeLogDaoStub);

        //[Fact]
        //public async Task RemoveLabel_HappyPath_Successful()
        //{
        //    // arrange
        //    var lineId = Guid.Parse("0683e1e1-0e0d-405c-b77e-a6d0d5141b67");
        //    var requestModel = new ChangeLogLineLabelRequestModel(lineId, "SomeLabel");
        //    var removeLabelInteractor = CreateInteractor();

        //    _outputPortMock.Setup(m => m.Removed(It.IsAny<Guid>()));

        //    // act
        //    await removeLabelInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

        //    // assert
        //    _outputPortMock.Verify(m => m.Removed(It.Is<Guid>(x => x == lineId)), Times.Once);
        //}
    }
}