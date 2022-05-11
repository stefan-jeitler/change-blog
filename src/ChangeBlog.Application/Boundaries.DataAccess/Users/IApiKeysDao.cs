using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeBlog.Application.Models;
using CSharpFunctionalExtensions;

namespace ChangeBlog.Application.Boundaries.DataAccess.Users;

public interface IApiKeysDao
{
    Task<IList<ApiKey>> GetUserApiKeysAsync(Guid userId);
    Task<Result<Guid, Conflict>> AddAsync(ApiKey apiKey);
    Task DeleteApiKeyAsync(Guid userId, Guid apiKeyId);
}