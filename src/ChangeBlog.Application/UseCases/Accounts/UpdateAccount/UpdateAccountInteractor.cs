using System;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.Boundaries.DataAccess.Accounts;
using ChangeBlog.Domain.Miscellaneous;
using CSharpFunctionalExtensions;

namespace ChangeBlog.Application.UseCases.Accounts.UpdateAccount;

public class UpdateAccountInteractor : IUpdateAccount
{
    private readonly IAccountDao _accountDao;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAccountInteractor(IAccountDao accountDao, IUnitOfWork unitOfWork)
    {
        _accountDao = accountDao;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(IUpdateAccountOutputPort output, UpdateAccountRequestModel requestModel)
    {
        if (!Name.TryParse(requestModel.Name, out var name))
        {
            output.Updated(requestModel.AccountId);
            return;
        }

        await UpdateInternalAsync(output, requestModel.AccountId, name);
    }

    private async Task UpdateInternalAsync(IUpdateAccountOutputPort output,
        Guid accountId,
        Name name)
    {
        _unitOfWork.Start();
        var account = await _accountDao.FindAccountAsync(name);
        if (account.HasValue)
        {
            output.NewNameAlreadyTaken(name);
            return;
        }

        await _accountDao.UpdateName(accountId, name)
            .Match(Finish, output.Conflict);

        void Finish(Guid id)
        {
            _unitOfWork.Commit();
            output.Updated(accountId);
        }
    }
}