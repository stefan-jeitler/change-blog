using System;
using System.Threading.Tasks;
using ChangeBlog.Application.DataAccess;
using ChangeBlog.Application.DataAccess.ExternalIdentity;
using ChangeBlog.Application.Tests.TestDoubles;
using ChangeBlog.Application.UseCases.Commands.AddExternalIdentity;
using ChangeBlog.Domain;
using ChangeBlog.Domain.Common;
using CSharpFunctionalExtensions;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.Commands.AddExternalIdentity
{
    public class AddExternalIdentityInteractorTests
    {

        private readonly ExternalUserInfoDaoStub _externalUserInfoDaoStub;
        private readonly UserDaoStub _userDaoStub;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public AddExternalIdentityInteractorTests()
        {
            _externalUserInfoDaoStub = new ExternalUserInfoDaoStub();
            _userDaoStub = new UserDaoStub();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
        }


        private AddExternalIdentityInteractor CreateInteractor() =>
            new(_externalUserInfoDaoStub, _userDaoStub, _unitOfWorkMock.Object);

        [Fact]
        public async Task AddExternalIdentity_ExternalUserIdIsNull_Failure()
        {
            var interactor = CreateInteractor();

            var result = await interactor.ExecuteAsync(null);

            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public async Task AddExternalIdentity_UserAlreadyExistsWithoutExternalIdentityAndIsDeleted_Failure()
        {
            // arrange
            const string externalUserId = "123456";
            var testUser = new User(TestAccount.UserId, 
                TestAccount.User.Email,
                TestAccount.User.FirstName,
                TestAccount.User.LastName,
                Name.Parse("UTC"),
                DateTime.Parse("2021-09-05"),
                DateTime.Parse("2021-09-05"));

            _externalUserInfoDaoStub.UserInfo = new UserInfo(externalUserId,
                testUser.FirstName,
                testUser.FirstName,
                testUser.LastName,
                testUser.Email);
            _userDaoStub.Users.Add(testUser);

            var interactor = CreateInteractor();

            // act
            var result = await interactor.ExecuteAsync(externalUserId);

            // assert
            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public async Task AddExternalIdentity_UserAlreadyExists_ReturnsIdWithoutImportingUser()
        {
            // arrange
            const string externalUserId = "123456";
            _userDaoStub.ExternalIdentities.Add(new UserDaoStub.ExternalIdentity(externalUserId, TestAccount.UserId));
            _userDaoStub.Users.Add(TestAccount.User);
            var interactor = CreateInteractor();

            // act
            var (isSuccess, _, userId) = await interactor.ExecuteAsync(externalUserId);

            // assert
            isSuccess.Should().BeTrue();
            userId.Should().Be(TestAccount.UserId);
        }

        [Fact]
        public async Task AddExternalIdentity_UserAlreadyExistsButExternalIdentityNot_ExternalIdentityAdded()
        {
            // arrange
            const string externalUserId = "123456";
            var testUser = new User(TestAccount.UserId, 
                TestAccount.User.Email,
                TestAccount.User.FirstName,
                TestAccount.User.LastName,
                Name.Parse("UTC"),
                null,
                DateTime.Parse("2021-09-05"));

            _externalUserInfoDaoStub.UserInfo = new UserInfo(externalUserId,
                testUser.FirstName,
                testUser.FirstName,
                testUser.LastName,
                testUser.Email);
            _userDaoStub.Users.Add(testUser);

            var interactor = CreateInteractor();

            // act
            var (isSuccess, _, _) = await interactor.ExecuteAsync(externalUserId);

            // assert
            isSuccess.Should().BeTrue();
            _userDaoStub.ExternalIdentities.Should().Contain(x => x.ExternalUserId == externalUserId);
        }

        [Fact]
        public async Task AddExternalIdentity_UserAlreadyExistsWithExternalIdentityButIsDeleted_ExternalIdentityAdded()
        {
            // arrange
            const string externalUserId = "123456";
            var testUser = new User(TestAccount.UserId, 
                TestAccount.User.Email,
                TestAccount.User.FirstName,
                TestAccount.User.LastName,
                Name.Parse("UTC"),
                DateTime.Parse("2021-09-05"),
                DateTime.Parse("2021-09-05"));

            _externalUserInfoDaoStub.UserInfo = new UserInfo(externalUserId,
                testUser.FirstName,
                testUser.FirstName,
                testUser.LastName,
                testUser.Email);
            _userDaoStub.Users.Add(testUser);
            _userDaoStub.ExternalIdentities.Add(new UserDaoStub.ExternalIdentity(externalUserId, testUser.Id));

            var interactor = CreateInteractor();

            // act
            var (_, isFailure, _, _) = await interactor.ExecuteAsync(externalUserId);

            // assert
            isFailure.Should().BeTrue();
        }

        [Fact]
        public async Task AddExternalIdentity_UserDoesNotExist_SuccessfullyImported()
        {
            // arrange
            const string externalUserId = "123456";
            var testUser = new User(TestAccount.UserId, 
                TestAccount.User.Email,
                TestAccount.User.FirstName,
                TestAccount.User.LastName,
                Name.Parse("UTC"),
                DateTime.Parse("2021-09-05"),
                DateTime.Parse("2021-09-05"));

            _externalUserInfoDaoStub.UserInfo = new UserInfo(externalUserId,
                testUser.FirstName,
                testUser.FirstName,
                testUser.LastName,
                testUser.Email);

            var interactor = CreateInteractor();

            // act
            var (isSuccess, _, userId, _) = await interactor.ExecuteAsync(externalUserId);

            // assert
            isSuccess.Should().BeTrue();
            var importedUser = await _userDaoStub.FindByExternalUserIdAsync(externalUserId);
            importedUser.HasValue.Should().BeTrue();
            importedUser.Value.Email.Should().Be(testUser.Email);
            importedUser.Value.TimeZone.Value.Should().Be("Etc/UTC");
            userId.Should().Be(importedUser.Value.Id);

            _unitOfWorkMock.Verify(m => m.Start(), Times.Once);
            _unitOfWorkMock.Verify(m => m.Commit(), Times.Once);
        }
        [Fact]
        public async Task AddExternalIdentity_ErrorWhileInsertingUser_ReturnsFailure()
        {
            // arrange
            const string externalUserId = "123456";
            var testUser = new User(TestAccount.UserId, 
                TestAccount.User.Email,
                TestAccount.User.FirstName,
                TestAccount.User.LastName,
                Name.Parse("UTC"),
                DateTime.Parse("2021-09-05"),
                DateTime.Parse("2021-09-05"));

            _externalUserInfoDaoStub.UserInfo = new UserInfo(externalUserId,
                testUser.FirstName,
                testUser.FirstName,
                testUser.LastName,
                testUser.Email);

            _userDaoStub.ProduceFailureWhileImporting = true;
            var interactor = CreateInteractor();

            // act
            var result = await interactor.ExecuteAsync(externalUserId);

            // assert
            result.IsFailure.Should().BeTrue();
        }
    }
}
