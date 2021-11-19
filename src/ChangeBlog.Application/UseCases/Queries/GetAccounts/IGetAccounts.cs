using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.Queries.GetAccounts;

public interface IGetAccounts
{
    Task<IList<AccountResponseModel>> ExecuteAsync(Guid userId);
}