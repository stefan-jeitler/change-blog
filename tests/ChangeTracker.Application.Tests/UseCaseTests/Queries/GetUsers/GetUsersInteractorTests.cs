using System.Threading.Tasks;
using ChangeTracker.Application.Extensions;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.Queries.GetUsers;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.Queries.GetUsers
{
    public class GetUsersInteractorTests
    {
        private readonly UserDaoStub _userDaoStub;

        public GetUsersInteractorTests()
        {
            _userDaoStub = new UserDaoStub();
        }

        private GetUsersInteractor CreateInteractor()
        {
            return new(_userDaoStub);
        }

        [Fact]
        public async Task GetUsers_HappyPath_Successful()
        {
            // arrange
            var interactor = CreateInteractor();
            _userDaoStub.Users.Add(TestAccount.User);
            var requestModel = new UsersQueryRequestModel(TestAccount.UserId, TestAccount.Id);

            // act
            var users = await interactor.ExecuteAsync(requestModel);

            // assert
            users.Should().HaveCount(1);
            users.Should().Contain(x => x.Id == TestAccount.UserId);
        }

        [Fact]
        public async Task GetUser_CreateAt_ProperlyConvertedToLocalTime()
        {
            // arrange
            var interactor = CreateInteractor();
            _userDaoStub.Users.Add(TestAccount.User);
            var createdAtLocal = TestAccount.User.CreatedAt.ToLocal(TestAccount.User.TimeZone);

            // act
            var user = await interactor.ExecuteAsync(TestAccount.UserId);

            // assert
            user.CreatedAt.Should().Be(createdAtLocal);
        }
    }
}