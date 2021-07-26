using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.Queries.GetChangeLogLine;
using ChangeTracker.Application.UseCases.Queries.SharedModels;
using ChangeTracker.Domain.ChangeLog;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.Queries.GetChangeLogLine
{
    public class GetChangeLogLineInteractorTests
    {
        private readonly ChangeLogDaoStub _changeLogQueriesStub;
        private readonly UserDaoStub _userDaoStub;
        private readonly Mock<IGetChangeLogLineOutputPort> _outputPortMock;

        public GetChangeLogLineInteractorTests()
        {
            _changeLogQueriesStub = new ChangeLogDaoStub();
            _userDaoStub = new UserDaoStub();
            _outputPortMock = new Mock<IGetChangeLogLineOutputPort>(MockBehavior.Strict);
        }

        private GetChangeLogLineInteractor CreateInteractor() => new(_changeLogQueriesStub, _userDaoStub);

        [Fact]
        public async Task GetChangeLogLine_HappyPath_Successful()
        {
            // arrange
            var interactor = CreateInteractor();
            _userDaoStub.Users.Add(TestAccount.User);
            var changeLogLineId = Guid.Parse("bf621860-3fa3-40d4-92ac-530cc57a1a98");
            var versionId = Guid.Parse("d8d1ab5d-670d-4370-84d5-2a260755e45c");
            var line = new ChangeLogLine(changeLogLineId, versionId, TestAccount.Product.Id,
                ChangeLogText.Parse("Test line."), 0, TestAccount.UserId, DateTime.Parse("2021-07-26"));
            _changeLogQueriesStub.ChangeLogs.Add(line);

            _outputPortMock.Setup(m => m.LineFound(It.IsAny<ChangeLogLineResponseModel>()));

            // act
            await interactor.ExecuteAsync(_outputPortMock.Object, TestAccount.UserId, changeLogLineId);

            // assert
            _outputPortMock.Verify(m => m.LineFound(It.Is<ChangeLogLineResponseModel>(r => r.Id == changeLogLineId)), Times.Once);
        }

        [Fact]
        public async Task GetChangeLogLine_LineIsPending_LineIsPendingOutput()
        {
            // arrange
            var interactor = CreateInteractor();
            _userDaoStub.Users.Add(TestAccount.User);
            var changeLogLineId = Guid.Parse("bf621860-3fa3-40d4-92ac-530cc57a1a98");
            var line = new ChangeLogLine(changeLogLineId, null, TestAccount.Product.Id,
                ChangeLogText.Parse("Test line."), 0, TestAccount.UserId, DateTime.Parse("2021-07-26"));
            _changeLogQueriesStub.ChangeLogs.Add(line);

            _outputPortMock.Setup(m => m.LineIsPending(It.IsAny<Guid>()));

            // act
            await interactor.ExecuteAsync(_outputPortMock.Object, TestAccount.UserId, changeLogLineId);

            // assert
            _outputPortMock.Verify(m => m.LineIsPending(It.Is<Guid>(x => x == changeLogLineId)), Times.Once);
        }

        [Fact]
        public async Task GetChangeLogLine_LineDoesNotExist_LineNotFoundOutput()
        {
            // arrange
            var interactor = CreateInteractor();
            _userDaoStub.Users.Add(TestAccount.User);
            var changeLogLineId = Guid.Parse("bf621860-3fa3-40d4-92ac-530cc57a1a98");
            _outputPortMock.Setup(m => m.LineDoesNotExists(It.IsAny<Guid>()));

            // act
            await interactor.ExecuteAsync(_outputPortMock.Object, TestAccount.UserId, changeLogLineId);

            // assert
            _outputPortMock.Verify(m => m.LineDoesNotExists(It.Is<Guid>(x => x == changeLogLineId)), Times.Once);
        }
    }
}
