using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess.Accounts;
using ChangeTracker.Domain;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.Tests.TestDoubles
{
    public class AccountDaoMock : IAccountDao
    {
        public Account Account { get; set; }

        public Task<Maybe<Account>> FindAsync(Guid accountId) =>
            Task.FromResult(accountId == Account?.Id
                ? Maybe<Account>.From(Account)
                : Maybe<Account>.None);
    }
}