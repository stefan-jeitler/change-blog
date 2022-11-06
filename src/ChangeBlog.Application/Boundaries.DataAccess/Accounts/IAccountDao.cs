using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeBlog.Domain;
using ChangeBlog.Domain.Miscellaneous;
using CSharpFunctionalExtensions;

namespace ChangeBlog.Application.Boundaries.DataAccess.Accounts;

public interface IAccountDao
{
    Task<Maybe<Account>> FindAccountAsync(Guid accountId);
    Task<Maybe<Account>> FindAccountAsync(Name accountName);
    Task<IList<Account>> FindByCreator(Guid creatorId);
    Task<Account> GetAccountAsync(Guid accountId);
    Task<IList<Account>> GetAccountsAsync(Guid userId);
    Task<IList<AccountStats>> GetAccountsStatsAsync(IList<Guid> accountIds);
    Task<AccountStats> GetAccountStatsAsync(Guid accountId);
    Task<IList<Account>> GetAccountsAsync(IList<Guid> accountIds);
    Task<Result<Guid, Conflict>> AddAccount(Account accountToAdd, AccountUser accountUserToAdd);
    Task<Result<Guid, Conflict>> DeleteAsync(Guid accountId);
    Task<Result<Guid, Conflict>> UpdateName(Guid accountId, Name newName);
}