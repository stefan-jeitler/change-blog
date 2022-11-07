using System;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess.Accounts;

namespace ChangeBlog.Application.UseCases.Accounts.AccountExists;

public class AccountExistsInteractor : IAccountExists
{
    private readonly IAccountDao _accountDao;

    public AccountExistsInteractor(IAccountDao accountDao)
    {
        _accountDao = accountDao;
    }

    public async Task<bool> ExecuteAsync(Guid accountId)
    {
        var account = await _accountDao.FindAccountAsync(accountId);
        return account.HasValue;
    }
}