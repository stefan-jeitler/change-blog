﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess.Users;
using Dapper;

namespace ChangeTracker.DataAccess.Postgres.DataAccessObjects.User
{
    public class UserDao : IUserDao
    {
        private readonly IDbAccessor _dbAccessor;

        public UserDao(IDbAccessor dbAccessor)
        {
            _dbAccessor = dbAccessor;
        }

        public async Task<Domain.User> GetUserAsync(Guid userId)
        {
            const string getUserSql = @"
                SELECT id,
                       email,
                       first_name AS firstName,
                       last_name  AS lastName,
                       timezone,
                       deleted_at AS deletedAt,
                       created_at AS createdAt
                FROM ""user""
                WHERE id = @userId";

            return await _dbAccessor.DbConnection
                .QuerySingleAsync<Domain.User>(getUserSql, new
                {
                    userId
                });
        }

        public async Task<IList<Domain.User>> GetUsersAsync(IList<Guid> userIds)
        {
            const string getUsersSql = @"
                SELECT id,
                       email,
                       first_name AS firstName,
                       last_name  AS lastName,
                       timezone,
                       deleted_at AS deletedAt,
                       created_at AS createdAt
                FROM ""user""
                WHERE id = ANY (@userIds)";

            var users = await _dbAccessor.DbConnection
                .QueryAsync<Domain.User>(getUsersSql, new
                {
                    userIds
                });

            return users.AsList();
        }

        public async Task<IList<Domain.User>> GetUsersAsync(Guid accountId, ushort limit, Guid? lastUserId)
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
                                u.deleted_at AS deletedAt,
                                u.created_at AS createdAt
                FROM ""user"" u
                         JOIN account_user au on u.id = au.user_id
                WHERE au.account_id = @accountId
                  {pagingFilter}
                ORDER BY u.email
                    FETCH FIRST (@limit) ROWS ONLY";

            var users = await _dbAccessor.DbConnection
                .QueryAsync<Domain.User>(getAccountUsersSql, new
                {
                    accountId,
                    lastUserId,
                    limit = (int) limit
                });

            return users.AsList();
        }
    }
}