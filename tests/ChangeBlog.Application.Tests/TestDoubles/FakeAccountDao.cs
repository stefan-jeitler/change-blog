using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess.Accounts;
using ChangeBlog.Domain;
using CSharpFunctionalExtensions;

namespace ChangeBlog.Application.Tests.TestDoubles;

public class FakeAccountDao : IAccountDao
{
    public List<Account> Accounts { get; } = new();

    public async Task<Maybe<Account>> FindAccountAsync(Guid accountId)
    {
        await Task.Yield();

        return Accounts.TryFirst(x => x.Id == accountId);
    }

    public async Task<Account> GetAccountAsync(Guid accountId)
    {
        await Task.Yield();

        return Accounts.Single(x => x.Id == accountId);
    }

    /// <summary>
    ///     Not properly implemented, but should be enough for use-case tests
    ///     The actual implementation of IAccountDao is tested separately.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<IList<Account>> GetAccountsAsync(Guid userId)
    {
        await Task.Yield();

        return Accounts;
    }

    public async Task<IList<Account>> GetAccountsAsync(IList<Guid> accountIds)
    {
        await Task.Yield();

        return Accounts
            .Where(x => accountIds.Any(y => x.Id == y))
            .ToList();
    }
}