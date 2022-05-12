using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeBlog.Application.Models;
using CSharpFunctionalExtensions;

namespace ChangeBlog.Application.Boundaries.DataAccess.Users;

public interface IApiKeysDao
{
    Task<IList<ApiKey>> GetApiKeysAsync(Guid userId);
    Task<Result<Guid, Conflict>> AddAsync(ApiKey apiKey);
    Task DeleteApiKeyAsync(Guid userId, Guid apiKeyId);
    Task<Maybe<ApiKey>> GetApiKeyAsync(Guid apiKeyId);
    Task<int> GetApiKeysCountAsync(Guid userId);
    Task<Result<Guid, Conflict>> UpdateApiKeyAsync(ApiKey apiKey);
}