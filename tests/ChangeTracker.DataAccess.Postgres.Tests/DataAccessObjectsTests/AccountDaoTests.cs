using System;
using System.Threading.Tasks;
using ChangeTracker.DataAccess.Postgres.DataAccessObjects;
using ChangeTracker.Domain;
using FluentAssertions;
using Npgsql;
using Xunit;

// ReSharper disable InconsistentNaming

namespace ChangeTracker.DataAccess.Postgres.Tests.DataAccessObjectsTests
{
    public class AccountDaoTests : IDisposable
    {
        private readonly LazyDbConnection _lazyDbConnection;

        public AccountDaoTests()
        {
            _lazyDbConnection = new LazyDbConnection(() => new NpgsqlConnection(Configuration.ConnectionString));
        }

        public void Dispose()
        {
            _lazyDbConnection?.Dispose();
        }

        private AccountDao CreateDao()
        {
            return new(new DbSession(_lazyDbConnection));
        }

        [Fact]
        public async Task FindAccount_ExistingAccount_ResultHasValue()
        {
            var accountDao = CreateDao();
            var t_ua_account_01 = Guid.Parse("ec3a44cc-0ba4-4c97-ad7f-911e9f6a73bc");

            var account = await accountDao.FindAccountAsync(t_ua_account_01);

            account.HasValue.Should().BeTrue();
        }

        [Fact]
        public async Task FindAccount_NotExistingAccount_ResultHasNoValue()
        {
            var accountDao = CreateDao();
            var notExistingAccount = Guid.Parse("d6f4d5cd-e0b0-43e8-b05b-46b55eb0d529");

            var account = await accountDao.FindAccountAsync(notExistingAccount);

            account.HasValue.Should().BeFalse();
        }

        [Fact]
        public async Task GetAccount_ExistingAccount_ReturnsAccount()
        {
            var accountDao = CreateDao();
            var t_ua_account_01 = Guid.Parse("ec3a44cc-0ba4-4c97-ad7f-911e9f6a73bc");

            var account = await accountDao.GetAccountAsync(t_ua_account_01);

            account.Id.Should().Be(t_ua_account_01);
        }

        [Fact]
        public void GetAccount_NotExistingAccount_Exception()
        {
            var accountDao = CreateDao();
            var notExistingAccountId = Guid.Parse("d6f4d5cd-e0b0-43e8-b05b-46b55eb0d529");

            Func<Task<Account>> act = () => accountDao.GetAccountAsync(notExistingAccountId);

            act.Should().ThrowExactly<Exception>();
        }

        [Fact]
        public async Task GetAccounts_ForUserWithOnlyDefaultUserRole_ReturnsAccount()
        {
            var accountDao = CreateDao();
            var t_ua_account_01_user_01 = Guid.Parse("f575503e-4eee-4d6d-b2c1-f11d8fc3da76");

            var accounts = await accountDao.GetAccountsAsync(t_ua_account_01_user_01);

            accounts.Should().HaveCount(1);
        }
    }
}