using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChangeBlog.Application.Tests.TestDoubles;
using ChangeBlog.Application.UseCases.Queries.GetPendingChangeLogs;
using ChangeBlog.Domain.ChangeLog;
using FluentAssertions;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.Queries.GetPendingChangeLogs
{
    public class GetPendingChangeLogsInteractorTest
    {

        private readonly ChangeLogDaoStub _changeLogDaoStub;
        private readonly UserDaoStub _userDaoStub;
        private readonly ProductDaoStub _productDaoStub;

        public GetPendingChangeLogsInteractorTest()
        {
            _changeLogDaoStub = new ChangeLogDaoStub();
            _userDaoStub = new UserDaoStub();
            _productDaoStub = new ProductDaoStub();
        }

        private GetPendingChangeLogsInteractor CreateInteractor() =>
            new(_changeLogDaoStub, _userDaoStub, _productDaoStub);

        [Fact]
        public async Task GetPendingChangeLogs_HappyPath_Successful()
        {
            // arrange
            var interactor = CreateInteractor();
            _userDaoStub.Users.Add(TestAccount.User);

            _productDaoStub.Products.Add(TestAccount.Product);
            var changeLogLineId = Guid.Parse("bf621860-3fa3-40d4-92ac-530cc57a1a98");
            _changeLogDaoStub.ChangeLogs.Add(new ChangeLogLine(changeLogLineId, null, TestAccount.Product.Id,
                ChangeLogText.Parse("Test line."), 0, TestAccount.UserId, DateTime.Parse("2021-07-26")));

            // act
            var changeLogs = await interactor.ExecuteAsync(TestAccount.UserId, TestAccount.Product.Id);

            // assert
            changeLogs.ProductId.Should().Be(TestAccount.Product.Id);
            changeLogs.ChangeLogs.Should().ContainSingle(x => x.Id == changeLogLineId);
        }

        [Fact]
        public async Task GetPendingChangeLogs_UserTimeZone2HoursAheadOfUtc_CreatedAtProperlyConverted()
        {
            // arrange
            var interactor = CreateInteractor();
            _userDaoStub.Users.Add(TestAccount.User);

            _productDaoStub.Products.Add(TestAccount.Product);
            var changeLogLineId = Guid.Parse("bf621860-3fa3-40d4-92ac-530cc57a1a98");
            _changeLogDaoStub.ChangeLogs.Add(new ChangeLogLine(changeLogLineId, null, TestAccount.Product.Id,
                ChangeLogText.Parse("Test line."), 0, TestAccount.UserId, DateTime.Parse("2021-07-26")));

            // act
            var changeLogs = await interactor.ExecuteAsync(TestAccount.UserId, TestAccount.Product.Id);

            // assert
            var expectedCreatedAt = DateTimeOffset.Parse("2021-07-26T02:00:00+02:00");
            changeLogs.ProductId.Should().Be(TestAccount.Product.Id);
            changeLogs.ChangeLogs.Should().HaveCount(1);
            changeLogs.ChangeLogs.Should().ContainSingle(x => x.CreatedAt.LocalDateTime == expectedCreatedAt);
        }

        [Fact]
        public async Task GetPendingChangeLogs_EmptyProductId_ArgumentException()
        {
            var interactor = CreateInteractor();

            Func<Task<PendingChangeLogsResponseModel>> act = () => interactor.ExecuteAsync(TestAccount.UserId, Guid.Empty);

            await act.Should().ThrowExactlyAsync<ArgumentException>();
        }

        [Fact]
        public async Task GetPendingChangeLogs_EmptyUserId_ArgumentException()
        {
            var interactor = CreateInteractor();

            Func<Task<PendingChangeLogsResponseModel>> act = () => interactor.ExecuteAsync(Guid.Empty, TestAccount.Product.Id);

            await act.Should().ThrowExactlyAsync<ArgumentException>();
        }
    }
}
