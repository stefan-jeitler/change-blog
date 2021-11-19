using System;
using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.Queries.GetAccounts;

public interface IGetAccount
{
    Task<AccountResponseModel> ExecuteAsync(Guid userId, Guid accountId);
}