using System;
using System.Data;
using System.Threading.Tasks;
using ChangeTracker.Application.UseCases;
using Dapper;

namespace ChangeTracker.DataAccess.Postgres
{
    public class UserAccessDao
    {
        private readonly Func<IDbConnection> _acquireDbConnection;

        public UserAccessDao(Func<IDbConnection> acquireDbConnection)
        {
            _acquireDbConnection = acquireDbConnection;
        }

        public async Task<Guid?> FindUserIdAsync(string apiKey)
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

        public async Task<bool> HasAccountPermissionAsync(Guid userId, Guid accountId, Permission permission)
        {
            const string hasAccountPermissionSql = @"
                SELECT EXISTS(SELECT NULL
                FROM account_user au
                JOIN ""role"" r ON r.id = au.role_id
                JOIN role_permission rp ON rp.role_id = r.id              
                WHERE au.account_id = @accountId
                AND au.user_id = @userId
                AND rp.permission = @permission)";

            using var dbConnection = _acquireDbConnection();

            return await dbConnection.ExecuteScalarAsync<bool>(hasAccountPermissionSql, new
            {
                accountId,
                userId,
                permission = permission.ToString()
            });
        }

        public async Task<bool> HasProjectPermissionAsync(Guid userId, Guid projectId, Permission permission)
        {
            const string hasProjectPermissionSql = @"
                SELECT EXISTS(SELECT NULL
                FROM project_user pu
                JOIN ""role"" r ON r.id = pu.role_id
                JOIN role_permission rp on rp.role_id = r.id
                WHERE pu.user_id = @userId
                AND pu.project_id = @projectId
                AND rp.permission = @permission)";

            var dbConnection = _acquireDbConnection();

            return await dbConnection.ExecuteScalarAsync<bool>(hasProjectPermissionSql, new
            {
                projectId,
                userId,
                permission = permission.ToString()
            });
        }

        public async Task<bool> HasVersionPermissionAsync(Guid userId, Guid versionId, Permission permission)
        {
            const string hasVersionPermissionSql = @"SELECT EXISTS(SELECT NULL
                FROM version v
                JOIN project p on v.project_id = p.id
                JOIN project_user pu on p.id = pu.project_id
                JOIN role r on pu.role_id = r.id
                JOIN role_permission rp on r.id = rp.role_id
                WHERE v.id = @versionId
                AND pu.user_id = @userId
                AND rp.permission = @permission)";

            var dbConnection = _acquireDbConnection();

            return await dbConnection.ExecuteScalarAsync<bool>(hasVersionPermissionSql, new
            {
                userId,
                versionId,
                permission = permission.ToString()
            });
        }

        public async Task<bool> HasChangeLogLinePermissionAsync(Guid userId, Guid changeLogLineId, Permission permission)
        {
            const string hasChangeLogLinePermissionSql = @"
                SELECT EXISTS(SELECT NULL
                FROM changelog_line chl
                JOIN project p on chl.project_id = p.id
                JOIN project_user pu on p.id = pu.project_id
                JOIN role r on pu.role_id = r.id
                JOIN role_permission rp on r.id = rp.role_id
                WHERE chl.id = @changeLogLineId
                AND pu.user_id = @userId
                AND rp.permission = @permission)";

            var dbConnection = _acquireDbConnection();

            return await dbConnection.ExecuteScalarAsync<bool>(hasChangeLogLinePermissionSql, new
            {
                userId,
                changeLogLineId,
                permission = permission.ToString()
            });
        }
    }
}