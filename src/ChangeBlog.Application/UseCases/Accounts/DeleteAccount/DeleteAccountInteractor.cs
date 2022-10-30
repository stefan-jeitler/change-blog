using System;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess.Accounts;
using CSharpFunctionalExtensions;

namespace ChangeBlog.Application.UseCases.Accounts.DeleteAccount;

public class DeleteAccountInteractor : IDeleteAccount
{
    private readonly IAccountDao _accountDao;

    public DeleteAccountInteractor(IAccountDao accountDao)
    {
        _accountDao = accountDao;
    }

    public async Task ExecuteAsync(IDeleteAccountOutputPort output, Guid accountId)
    {
        var account = await _accountDao.GetAccountAsync(accountId);

        if (account.DeletedAt.HasValue)
        {
            output.AccountDeleted(account.Id);
            return;
        }

        await _accountDao.DeleteAsync(accountId)
            .Match(output.AccountDeleted, output.Conflict);
    }
}