using System;
using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.Accounts.GetAccounts;

public interface IGetAccount
{
    Task<AccountResponseModel> ExecuteAsync(Guid userId, Guid accountId);
}