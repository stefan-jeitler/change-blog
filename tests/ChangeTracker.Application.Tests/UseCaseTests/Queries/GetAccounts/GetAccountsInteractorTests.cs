using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChangeTracker.Application.Extensions;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.Queries.GetAccounts;
using ChangeTracker.Domain;
using ChangeTracker.Domain.Common;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.Queries.GetAccounts
{
    public class GetAccountsInteractorTests
    {

        private readonly AccountDaoStub _accountDaoStub;
        private readonly UserDaoStub _userDaoStub;
        private readonly VersioningSchemeDaoStub _versioningSchemeDaoStub;

        public GetAccountsInteractorTests()
        {
            _accountDaoStub = new AccountDaoStub();
            _userDaoStub = new UserDaoStub();
            _versioningSchemeDaoStub = new VersioningSchemeDaoStub();
        }

        private GetAccountsInteractor CreateInteractor() =>
            new(_accountDaoStub, _userDaoStub, _versioningSchemeDaoStub);

        [Fact]
        public async Task GetAccounts_HappyPath_Successful()
        {
            // arrange
            _userDaoStub.Users.Add(TestAccount.User);
            _accountDaoStub.Accounts.Add(TestAccount.Account);
            _versioningSchemeDaoStub.VersioningSchemes.Add(TestAccount.CustomVersioningScheme);
            _versioningSchemeDaoStub.VersioningSchemes.Add(TestAccount.DefaultScheme);
            var interactor = CreateInteractor();

            // act 
            var accounts = await interactor.ExecuteAsync(TestAccount.UserId);

            // assert
            accounts.Should().HaveCount(1);
            accounts.Should().Contain(x => x.Id == TestAccount.Id);
        }

        [Fact]
        public async Task GetAccounts_CreatedAt_ConvertedToLocalTime()
        {
            // arrange
            _userDaoStub.Users.Add(TestAccount.User);
            _accountDaoStub.Accounts.Add(TestAccount.Account);
            _versioningSchemeDaoStub.VersioningSchemes.Add(TestAccount.CustomVersioningScheme);
            _versioningSchemeDaoStub.VersioningSchemes.Add(TestAccount.DefaultScheme);
            var interactor = CreateInteractor();
            var createdAtLocal = TestAccount.CreationDate.ToLocal(TestAccount.User.TimeZone);

            // act 
            var accounts = await interactor.ExecuteAsync(TestAccount.UserId);

            // assert
            accounts.Should().HaveCount(1);
            accounts.Should().Contain(x => x.CreatedAt == createdAtLocal);
        }

        [Fact]
        public async Task GetAccount_HappyPath_Successful()
        {
            // arrange
            _userDaoStub.Users.Add(TestAccount.User);
            _accountDaoStub.Accounts.Add(TestAccount.Account);
            _versioningSchemeDaoStub.VersioningSchemes.Add(TestAccount.CustomVersioningScheme);
            var interactor = CreateInteractor();
            var createdAtLocal = TestAccount.CreationDate.ToLocal(TestAccount.User.TimeZone);

            // act 
            var account = await interactor.ExecuteAsync(TestAccount.UserId, TestAccount.Id);

            // assert
            account.Id.Should().Be(TestAccount.Id);
        }

        [Fact]
        public async Task GetAccount_AccountWithoutDefaultScheme_UsesGlobalDefaultScheme()
        {
            // arrange
            _userDaoStub.Users.Add(TestAccount.User);
            _accountDaoStub.Accounts.Add(new Account(TestAccount.Id, TestAccount.Name, null, TestAccount.CreationDate, null));
            _versioningSchemeDaoStub.VersioningSchemes.Add(TestAccount.DefaultScheme);
            var interactor = CreateInteractor();

            // act 
            var account = await interactor.ExecuteAsync(TestAccount.UserId, TestAccount.Id);

            // assert
            account.DefaultVersioningSchemeId.Should().Be(TestAccount.DefaultScheme.Id);
        }
    }
}
