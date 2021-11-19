using System;
using System.Threading.Tasks;
using ChangeBlog.Application.Tests.TestDoubles;
using ChangeBlog.Application.UseCases.Queries.GetChangeLogLine;
using ChangeBlog.Application.UseCases.Queries.SharedModels;
using ChangeBlog.Domain.ChangeLog;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.Queries.GetChangeLogLine
{
    public class GetChangeLogLineInteractorTests
    {
        private readonly FakeChangeLogDao _changeLogQueriesStub;
        private readonly Mock<IGetChangeLogLineOutputPort> _outputPortMock;
        private readonly FakeUserDao _fakeUserDao;

        public GetChangeLogLineInteractorTests()
        {
            _changeLogQueriesStub = new FakeChangeLogDao();
            _fakeUserDao = new FakeUserDao();
            _outputPortMock = new Mock<IGetChangeLogLineOutputPort>(MockBehavior.Strict);
        }

        private GetChangeLogLineInteractor CreateInteractor() => new(_changeLogQueriesStub, _fakeUserDao);

        [Fact]
        public async Task GetChangeLogLine_HappyPath_Successful()
        {
            // arrange
            var interactor = CreateInteractor();
            _fakeUserDao.Users.Add(TestAccount.User);
            var changeLogLineId = Guid.Parse("bf621860-3fa3-40d4-92ac-530cc57a1a98");
            var versionId = Guid.Parse("d8d1ab5d-670d-4370-84d5-2a260755e45c");
            var line = new ChangeLogLine(changeLogLineId, versionId, TestAccount.Product.Id,
                ChangeLogText.Parse("Test line."), 0, TestAccount.UserId, DateTime.Parse("2021-07-26"));
            _changeLogQueriesStub.ChangeLogs.Add(line);

            _outputPortMock.Setup(m => m.LineFound(It.IsAny<ChangeLogLineResponseModel>()));

            // act
            await interactor.ExecuteAsync(_outputPortMock.Object, TestAccount.UserId, changeLogLineId);

            // assert
            _outputPortMock.Verify(
                m => m.LineFound(It.Is<ChangeLogLineResponseModel>(r => r.Id == changeLogLineId)), Times.Once);
        }

        [Fact]
        public async Task GetChangeLogLine_UserTimezone2HoursAheadOfUtc_CreatedAtProperlyConverted()
        {
            // arrange
            var interactor = CreateInteractor();
            _fakeUserDao.Users.Add(TestAccount.User);
            var changeLogLineId = Guid.Parse("bf621860-3fa3-40d4-92ac-530cc57a1a98");
            var versionId = Guid.Parse("d8d1ab5d-670d-4370-84d5-2a260755e45c");
            var line = new ChangeLogLine(changeLogLineId, versionId, TestAccount.Product.Id,
                ChangeLogText.Parse("Test line."), 0, TestAccount.UserId, DateTime.Parse("2021-07-26"));
            _changeLogQueriesStub.ChangeLogs.Add(line);

            _outputPortMock.Setup(m => m.LineFound(It.IsAny<ChangeLogLineResponseModel>()));

            // act
            await interactor.ExecuteAsync(_outputPortMock.Object, TestAccount.UserId, changeLogLineId);

            // assert
            var expectedCreatedAt = DateTimeOffset.Parse("2021-07-26T02:00:00+02:00");
            _outputPortMock.Verify(
                m => m.LineFound(It.Is<ChangeLogLineResponseModel>(r => r.CreatedAt.LocalDateTime == expectedCreatedAt)), Times.Once);
        }

        [Fact]
        public async Task GetChangeLogLine_LineIsPending_LineIsPendingOutput()
        {
            // arrange
            var interactor = CreateInteractor();
            _fakeUserDao.Users.Add(TestAccount.User);
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
            _fakeUserDao.Users.Add(TestAccount.User);
            var changeLogLineId = Guid.Parse("bf621860-3fa3-40d4-92ac-530cc57a1a98");
            _outputPortMock.Setup(m => m.LineDoesNotExists(It.IsAny<Guid>()));

            // act
            await interactor.ExecuteAsync(_outputPortMock.Object, TestAccount.UserId, changeLogLineId);

            // assert
            _outputPortMock.Verify(m => m.LineDoesNotExists(It.Is<Guid>(x => x == changeLogLineId)), Times.Once);
        }


        [Fact]
        public async Task GetChangeLogLine_EmptyProductId_ArgumentException()
        {
            var interactor = CreateInteractor();
            var changeLogLineId = Guid.Parse("bf621860-3fa3-40d4-92ac-530cc57a1a98");

            var act = () => interactor.ExecuteAsync(_outputPortMock.Object, Guid.Empty, changeLogLineId);

            await act.Should().ThrowExactlyAsync<ArgumentException>();
        }

        [Fact]
        public async Task GetChangeLogLine_EmptyChangeLogLineId_ArgumentException()
        {
            var interactor = CreateInteractor();

            var act = () => interactor.ExecuteAsync(_outputPortMock.Object, TestAccount.Product.Id, Guid.Empty);

            await act.Should().ThrowExactlyAsync<ArgumentException>();
        }
    }
}
