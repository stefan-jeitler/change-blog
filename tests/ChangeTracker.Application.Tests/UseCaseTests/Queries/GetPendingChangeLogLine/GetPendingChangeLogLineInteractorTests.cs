using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.Queries.GetPendingChangeLogLine;
using ChangeTracker.Domain.ChangeLog;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.Queries.GetPendingChangeLogLine
{
    public class GetPendingChangeLogLineInteractorTests
    {
        private readonly ChangeLogDaoStub _changeLogDaoStub;
        private readonly ProductDaoStub _productDaoStub;
        private readonly UserDaoStub _userDaoStub;
        private readonly Mock<IGetPendingChangeLogLineOutputPort> _outputPortMock;

        public GetPendingChangeLogLineInteractorTests()
        {
            _changeLogDaoStub = new ChangeLogDaoStub();
            _productDaoStub = new ProductDaoStub();
            _userDaoStub = new UserDaoStub();
            _outputPortMock = new Mock<IGetPendingChangeLogLineOutputPort>(MockBehavior.Strict);
        }

        private GetPendingChangeLogLineInteractor CreateInteractor() => new(_changeLogDaoStub, _userDaoStub, _productDaoStub);

        [Fact]
        public async Task GetPendingChangeLogLine_HappyPath_Successful()
        {
            // arrange
            var interactor = CreateInteractor();
            _userDaoStub.Users.Add(TestAccount.User);
            _productDaoStub.Products.Add(TestAccount.Product);
            var changeLogLineId = Guid.Parse("bf621860-3fa3-40d4-92ac-530cc57a1a98");
            _changeLogDaoStub.ChangeLogs.Add(new ChangeLogLine(changeLogLineId, null, TestAccount.Product.Id,
                ChangeLogText.Parse("Test line."), 0, TestAccount.UserId, DateTime.Parse("2021-07-26")));

            _outputPortMock.Setup(m => m.LineFound(It.IsAny<PendingChangeLogLineResponseModel>()));

            // act
            await interactor.ExecuteAsync(_outputPortMock.Object, TestAccount.UserId, changeLogLineId);

            // assert
            _outputPortMock.Verify(m => m.LineFound(It.Is<PendingChangeLogLineResponseModel>(r => r.ChangeLogLine.Id == changeLogLineId)),
                Times.Once);
        }

        [Fact]
        public async Task GetPendingChangeLogLine_LineIsNotPending_LineNotPendingOutput()
        {
            // arrange
            var interactor = CreateInteractor();
            _userDaoStub.Users.Add(TestAccount.User);
            _productDaoStub.Products.Add(TestAccount.Product);
            var versionId = Guid.Parse("d8d1ab5d-670d-4370-84d5-2a260755e45c");
            var changeLogLineId = Guid.Parse("bf621860-3fa3-40d4-92ac-530cc57a1a98");
            _changeLogDaoStub.ChangeLogs.Add(new ChangeLogLine(changeLogLineId, versionId, TestAccount.Product.Id,
                ChangeLogText.Parse("Test line."), 0, TestAccount.UserId, DateTime.Parse("2021-07-26")));

            _outputPortMock.Setup(m => m.LineIsNotPending(It.IsAny<Guid>()));

            // act
            await interactor.ExecuteAsync(_outputPortMock.Object, TestAccount.UserId, changeLogLineId);

            // assert
            _outputPortMock.Verify(m => m.LineIsNotPending(It.Is<Guid>(x => x == changeLogLineId)),
                Times.Once);
        }

        [Fact]
        public async Task GetPendingChangeLogLine_NotExistingLine_LineDoesNotExistOutput()
        {
            // arrange
            var interactor = CreateInteractor();
            _userDaoStub.Users.Add(TestAccount.User);
            _productDaoStub.Products.Add(TestAccount.Product);
            var changeLogLineId = Guid.Parse("bf621860-3fa3-40d4-92ac-530cc57a1a98");

            _outputPortMock.Setup(m => m.LineDoesNotExist(It.IsAny<Guid>()));

            // act
            await interactor.ExecuteAsync(_outputPortMock.Object, TestAccount.UserId, changeLogLineId);

            // assert
            _outputPortMock.Verify(m => m.LineDoesNotExist(It.Is<Guid>(x => x == changeLogLineId)),
                Times.Once);
        }

        [Fact]
        public void GetPendingChangeLogLine_EmptyProductId_ArgumentException()
        {
            var interactor = CreateInteractor();
            var changeLogLineId = Guid.Parse("bf621860-3fa3-40d4-92ac-530cc57a1a98");

            Func<Task> act = () => interactor.ExecuteAsync(_outputPortMock.Object, Guid.Empty, changeLogLineId);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void GetPendingChangeLogLine_EmptyChangeLogLineId_ArgumentException()
        {
            var interactor = CreateInteractor();

            Func<Task> act = () => interactor.ExecuteAsync(_outputPortMock.Object, TestAccount.Product.Id, Guid.Empty);
            
            act.Should().ThrowExactly<ArgumentException>();
        }
    }
}
