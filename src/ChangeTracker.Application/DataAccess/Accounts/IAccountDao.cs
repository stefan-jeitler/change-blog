using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeTracker.Domain;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.DataAccess.Accounts
{
    public interface IAccountDao
    {
        Task<Maybe<Account>> FindAccountAsync(Guid accountId);
        Task<Account> GetAccountAsync(Guid accountId);
        Task<IList<Account>> GetAccountsAsync(Guid userId);
        Task<IList<Account>> GetAccountsAsync(IList<Guid> accountIds);
    }
}