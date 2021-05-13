using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess.Users;
using ChangeTracker.Domain;
using Dapper;

namespace ChangeTracker.DataAccess.Postgres.DataAccessObjects
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
                       last_name AS lastName,
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
                       last_name AS lastName,
                       timezone,
                       deleted_at AS deletedAt,
                       created_at AS createdAt
                FROM ""user""
                WHERE id = ANY(@userIds)";

            var users = await _dbAccessor.DbConnection
                .QueryAsync<User>(getUsersSql, new
                {
                    userIds
                });

            return users.ToList();
        }
    }
}
