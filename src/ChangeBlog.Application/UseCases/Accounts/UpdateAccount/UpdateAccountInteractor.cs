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
        var newName = Name.Parse(requestModel.Name);
        await UpdateNameInternalAsync(output, requestModel.AccountId, newName);
    }

    private async Task UpdateNameInternalAsync(IUpdateAccountOutputPort output,
        Guid accountId,
        Name newName)
    {
        _unitOfWork.Start();

        var account = await _accountDao.FindAccountAsync(newName);

        if (account.HasValue)
        {
            output.NewNameAlreadyTaken(newName.Value);
            return;
        }

        await _accountDao.UpdateName(accountId, newName)
            .Match(Finish, output.Conflict);

        void Finish(Guid id)
        {
            _unitOfWork.Commit();
            output.Updated(accountId);
        }
    }
}