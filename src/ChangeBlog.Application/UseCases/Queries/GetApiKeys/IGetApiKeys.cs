using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.Queries.GetApiKeys;

public interface IGetApiKeys
{
    Task<IList<ApiKeyResponseModel>> ExecuteAsync(Guid userId);
}