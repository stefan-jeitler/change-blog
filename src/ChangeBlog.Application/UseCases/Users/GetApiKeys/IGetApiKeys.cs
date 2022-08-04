using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.Users.GetApiKeys;

public interface IGetApiKeys
{
    Task<IList<ApiKeyResponseModel>> ExecuteAsync(Guid userId);
}