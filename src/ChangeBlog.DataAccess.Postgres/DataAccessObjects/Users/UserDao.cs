using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.Boundaries.DataAccess.Users;
using ChangeBlog.Application.Models;
using ChangeBlog.Domain;
using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.Extensions.Logging;

namespace ChangeBlog.DataAccess.Postgres.DataAccessObjects.Users;

public class UserDao : IUserDao
{
    private readonly IDbAccessor _dbAccessor;
    private readonly ILogger<UserDao> _logger;

    public UserDao(IDbAccessor dbAccessor, ILogger<UserDao> logger)
    {
        _dbAccessor = dbAccessor;
        _logger = logger;
    }

    public async Task<User> GetUserAsync(Guid userId)
    {
        const string getUserSql = @"
                SELECT id,
                       email,
                       first_name AS firstName,
                       last_name  AS lastName,
                       timezone,
                       culture,
                       deleted_at AS deletedAt,
                       created_at AS createdAt
                FROM ""user""
                WHERE id = @userId";

        return await _dbAccessor.DbConnection
            .QuerySingleAsync<User>(getUserSql, new
            {
                userId
            });
    }

    public async Task<IList<User>> GetUsersAsync(IList<Guid> userIds)
    {
        const string getUsersSql = @"
                SELECT id,
                       email,
                       first_name AS firstName,
                       last_name  AS lastName,
                       timezone,
                       culture,
                       deleted_at AS deletedAt,
                       created_at AS createdAt
                FROM ""user""
                WHERE id = ANY (@userIds)";

        var users = await _dbAccessor.DbConnection
            .QueryAsync<User>(getUsersSql, new
            {
                userIds
            });

        return users.AsList();
    }

    public async Task<IList<User>> GetUsersAsync(Guid accountId, ushort limit, Guid? lastUserId)
    {
        var pagingFilter = lastUserId.HasValue
            ? "AND u.email > (SELECT us.email FROM \"user\" us where us.id = @lastUserId)"
            : string.Empty;

        var getAccountUsersSql = @$"
                SELECT DISTINCT u.id,
                                u.email,
                                u.first_name AS firstName,
                                u.last_name  AS lastName,
                                u.timezone,
                                u.culture,
                                u.deleted_at AS deletedAt,
                                u.created_at AS createdAt
                FROM ""user"" u
                         JOIN account_user au on u.id = au.user_id
                WHERE au.account_id = @accountId
                  {pagingFilter}
                ORDER BY u.email
                FETCH FIRST (@limit) ROWS ONLY";

        var users = await _dbAccessor.DbConnection
            .QueryAsync<User>(getAccountUsersSql, new
            {
                accountId,
                lastUserId,
                limit = (int)limit
            });

        return users.AsList();
    }

    public async Task<Maybe<User>> FindByExternalUserIdAsync(string externalUserId)
    {
        if (string.IsNullOrEmpty(externalUserId))
        {
            return Maybe<User>.None;
        }

        const string getUserSql = @"
                SELECT u.id,
                       u.email,
                       u.first_name AS firstName,
                       u.last_name  AS lastName,
                       u.timezone,
                       u.culture,
                       u.deleted_at AS deletedAt,
                       u.created_at AS createdAt
                FROM ""user"" u
                JOIN external_identity ei on u.id = ei.user_id
                WHERE ei.external_user_id = @externalUserId";

        var user = await _dbAccessor.DbConnection
            .QuerySingleOrDefaultAsync<User>(getUserSql, new
            {
                externalUserId
            });

        return user == default
            ? Maybe<User>.None
            : Maybe<User>.From(user);
    }

    public async Task<Maybe<User>> FindByEmailAsync(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return Maybe<User>.None;
        }

        const string getUserSql = @"
                SELECT u.id,
                       u.email,
                       u.first_name AS firstName,
                       u.last_name  AS lastName,
                       u.timezone,
                       u.culture,
                       u.deleted_at AS deletedAt,
                       u.created_at AS createdAt
                FROM ""user"" u
                WHERE LOWER(u.email) = LOWER(@email)";

        var user = await _dbAccessor.DbConnection
            .QuerySingleOrDefaultAsync<User>(getUserSql, new
            {
                email
            });

        return user == default
            ? Maybe<User>.None
            : Maybe<User>.From(user);
    }

    public async Task<Result> AddExternalIdentity(ExternalIdentity externalIdentity)
    {
        const string insertExternalIdentitySql = @"
                INSERT INTO external_identity (id, user_id, external_user_id, created_at, identity_provider) 
                VALUES (@id, @userId, @externalUserId, @createdAt, @identityProvider)";

        try
        {
            await _dbAccessor.DbConnection
                .ExecuteAsync(insertExternalIdentitySql, new
                {
                    id = externalIdentity.Id,
                    userId = externalIdentity.UserId,
                    externalUserId = externalIdentity.ExternalUserId,
                    createdAt = externalIdentity.CreatedAt,
                    identityProvider = externalIdentity.IdentityProvider
                });

            return Result.Success();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error while adding external identity");
            return Result.Failure(exception.Message);
        }
    }

    public async Task<Result<User, Conflict>> UpdateCultureAndTimezoneAsync(User user)
    {
        const string updateSql = @"UPDATE ""user"" SET timezone = @timezone, culture = @culture WHERE id = @userId";

        await _dbAccessor.DbConnection
            .ExecuteAsync(updateSql, new
            {
                userId = user.Id,
                timezone = user.TimeZone,
                culture = user.Culture
            });
        
        return Result.Success<User, Conflict>(await GetUserAsync(user.Id));
    }

    public async Task<Result> AddAsync(User user)
    {
        const string insertUserSql = @"
                INSERT INTO ""user"" (id, email, first_name, last_name, timezone, culture, created_at)
                VALUES (@userId, @email, @firstName, @lastName, @timeZone, @culture, @createdAt)
                ";

        try
        {
            await _dbAccessor.DbConnection
                .ExecuteAsync(insertUserSql, new
                {
                    userId = user.Id,
                    email = user.Email.Value,
                    firstName = user.FirstName.Value,
                    lastName = user.LastName.Value,
                    timeZone = user.TimeZone.Value,
                    culture = user.Culture.Value,
                    createdAt = user.CreatedAt
                });

            return Result.Success();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error while adding user");
            return Result.Failure(exception.Message);
        }
    }
}