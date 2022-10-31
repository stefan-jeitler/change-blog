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

    public async Task<IList<ApiKey>> GetApiKeysAsync(Guid userId)
    {
        const string sql = @"SELECT user_id    AS userId,
                                       id         as apiKeyId,
                                       name      as name,
                                       key        as apiKey,
                                       expires_at as expiresAt
                                FROM api_key
                                WHERE user_id = @userId";

        var result = await _dbAccessor.DbConnection.QueryAsync<ApiKey>(sql, new
        {
            userId
        });

        return result.AsList();
    }

    public async Task<Result<Guid, Conflict>> AddAsync(ApiKey apiKey)
    {
        const string insertNewApiKeySql = @"
            INSERT INTO api_key (id, user_id, name, key, expires_at, deleted_at, created_at)
            VALUES (@id, @userId, @name, @apiKey, @expiresAt, null, now())";

        try
        {
            await _dbAccessor.DbConnection
                .ExecuteAsync(insertNewApiKeySql, new
                {
                    id = apiKey.ApiKeyId,
                    userId = apiKey.UserId,
                    name = apiKey.Name,
                    apiKey = apiKey.Key,
                    expiresAt = apiKey.ExpiresAt
                });

            return Result.Success<Guid, Conflict>(apiKey.ApiKeyId);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error while adding api key.");
            throw;
        }
    }

    public Task DeleteApiKeyAsync(Guid userId, Guid apiKeyId) =>
        _dbAccessor.DbConnection.ExecuteAsync(@"DELETE FROM api_key WHERE user_id = @userId AND id = @apiKeyId",
            new
            {
                userId,
                apiKeyId
            });

    public async Task<Maybe<ApiKey>> GetApiKeyAsync(Guid apiKeyId)
    {
        const string sql = @"SELECT user_id    AS userId,
                                       id         as apiKeyId,
                                       name      as name,
                                       key        as apiKey,
                                       expires_at as expiresAt
                                FROM api_key
                                WHERE id = @apiKeyId";

        var apiKey = await _dbAccessor.DbConnection.QuerySingleOrDefaultAsync<ApiKey>(sql,
            new {apiKeyId});

        return apiKey == default
            ? Maybe<ApiKey>.None
            : Maybe.From(apiKey);
    }

    public Task<int> GetApiKeysCountAsync(Guid userId)
    {
        const string sql = "SELECT COUNT(*) FROM api_key WHERE user_id = @userId";

        return _dbAccessor.DbConnection.ExecuteScalarAsync<int>(sql, new {userId});
    }

    public async Task<Result<Guid, Conflict>> UpdateApiKeyAsync(ApiKey apiKey)
    {
        const string updateApiKeySql = @"
            UPDATE api_key SET name = @name, expires_at = @expiresAt 
            WHERE id = @id";

        try
        {
            await _dbAccessor.DbConnection
                .ExecuteAsync(updateApiKeySql, new
                {
                    id = apiKey.ApiKeyId,
                    name = apiKey.Name,
                    expiresAt = apiKey.ExpiresAt
                });

            return Result.Success<Guid, Conflict>(apiKey.ApiKeyId);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error while adding api key.");
            throw;
        }
    }
}