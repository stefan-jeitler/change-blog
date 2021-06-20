using System;
using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.Queries.GetAccounts
{
    public interface IGetAccount
    {
        Task<AccountResponseModel> ExecuteAsync(Guid userId, Guid accountId);
    }
}