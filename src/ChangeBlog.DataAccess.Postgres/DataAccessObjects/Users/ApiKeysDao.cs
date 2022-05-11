using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.Boundaries.DataAccess.Users;
using ChangeBlog.Application.Models;
using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.Extensions.Logging;

namespace ChangeBlog.DataAccess.Postgres.DataAccessObjects.Users;

public class ApiKeysDao : IApiKeysDao
{
    private readonly IDbAccessor _dbAccessor;
    private readonly ILogger<ApiKeysDao> _logger;

    public ApiKeysDao(IDbAccessor dbAccessor, ILogger<ApiKeysDao> logger)
    {
        _dbAccessor = dbAccessor;
        _logger = logger;
    }

    public async Task<IList<UserApiKey>> GetUserApiKeysAsync(Guid userId)
    {
        const string sql = @"SELECT user_id    AS userId,
                                       id         as apiKeyId,
                                       key        as apiKey,
                                       expires_at as expiresAt
                                FROM api_key
                                WHERE user_id = @userId";

        var result = await _dbAccessor.DbConnection.QueryAsync<UserApiKey>(sql, new
        {
            userId
        });

        return result.AsList();
    }

    public async Task<Result<Guid, Conflict>> AddAsync(UserApiKey userApiKey)
    {
        
        const string insertNewApiKeySql = @"
            INSERT INTO api_key (id, user_id, key, expires_at, deleted_at, created_at)
            VALUES (@id, @userId, @apiKey, @expiresAt, null, now())";

        try
        {
            await _dbAccessor.DbConnection
                .ExecuteAsync(insertNewApiKeySql, new
                {
                    id = userApiKey.ApiKeyId,
                    userId = userApiKey.UserId,
                    apiKey = userApiKey.ApiKey,
                    expiresAt = userApiKey.ExpiresAt
                });

            return Result.Success<Guid, Conflict>(userApiKey.ApiKeyId);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error while adding api key.");
            throw;
        }
    }

    public Task DeleteApiKeyAsync(Guid userId, Guid apiKeyId)
    {
        return _dbAccessor.DbConnection.ExecuteAsync(@"DELETE FROM api_key WHERE user_id = @userId AND id = @apiKeyId",
            new
            {
                userId,
                apiKeyId
            });
    }
}