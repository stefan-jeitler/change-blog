using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.Queries.GetAccounts
{
    public interface IGetAccounts
    {
        Task<IList<AccountResponseModel>> ExecuteAsync(Guid userId);
        Task<AccountResponseModel> ExecuteAsync(Guid userId, Guid accountId);
    }
}
