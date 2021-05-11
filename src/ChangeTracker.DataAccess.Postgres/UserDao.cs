using System;
using System.Data;
using System.Threading.Tasks;
using ChangeTracker.Application.UseCases;
using Dapper;

namespace ChangeTracker.DataAccess.Postgres
{
    public class UserDao
    {
        private readonly Func<IDbConnection> _acquireDbConnection;

        public UserDao(Func<IDbConnection> acquireDbConnection)
        {
            _acquireDbConnection = acquireDbConnection;
        }

        public async Task<Guid?> FindUserId(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                return null;
            }

            const string getApiKeySql = @"
                SELECT user_id
                FROM api_key
                WHERE key = @apiKey";

            using var dbConnection = _acquireDbConnection();
            return await dbConnection
                .QuerySingleOrDefaultAsync<Guid?>(getApiKeySql, new {apiKey});
        }

        public async Task<bool> HasAccountPermission(Guid userId, Guid accountId, Permission permission)
        {
            const string hasAccountPermissionQuery = @"
                SELECT EXISTS(SELECT 1 
                FROM account_user au 
                JOIN account_user_role aur ON aur.account_user_id = au.id
                JOIN ""role"" r ON r.id = aur.role_id
                JOIN role_permission rp ON rp.role_id = r.id
                WHERE au.account_id = @accountId
                AND au.user_id = @userId
                AND rp.permission = @permission)";

            using var dbConnection = _acquireDbConnection();

            return await dbConnection.ExecuteScalarAsync<bool>(hasAccountPermissionQuery, new
            {
                accountId,
                userId,
                permission = permission.ToString()
            });
        }
    }
}