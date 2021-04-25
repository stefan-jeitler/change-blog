using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess.Accounts;
using ChangeTracker.Domain;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.Tests.TestDoubles
{
    public class AccountDaoStub : IAccountDao
    {
        public Account Account { get; set; }

        public Task<Maybe<Account>> FindAccountAsync(Guid accountId) =>
            Task.FromResult(accountId == Account?.Id
                ? Maybe<Account>.From(Account)
                : Maybe<Account>.None);
    }
}