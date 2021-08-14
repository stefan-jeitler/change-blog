using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeBlog.Application.DataAccess.Users;
using ChangeBlog.Domain;
using Dapper;

namespace ChangeBlog.DataAccess.Postgres.DataAccessObjects.Users
{
    public class UserDao : IUserDao
    {
        private readonly IDbAccessor _dbAccessor;

        public UserDao(IDbAccessor dbAccessor)
        {
            _dbAccessor = dbAccessor;
        }

        public async Task<User> GetUserAsync(Guid userId)
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
                    limit = (int) limit
                });

            return users.AsList();
        }
    }
}
