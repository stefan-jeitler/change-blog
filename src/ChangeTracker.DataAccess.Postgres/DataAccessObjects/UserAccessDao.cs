using System;
using System.Data;
using System.Threading.Tasks;
using ChangeTracker.Application.UseCases;
using Dapper;

namespace ChangeTracker.DataAccess.Postgres.DataAccessObjects
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
            if (string.IsNullOrEmpty(apiKey)) return null;

            using var dbConnection = _acquireDbConnection();
            return await dbConnection
                .QueryFirstOrDefaultAsync<Guid?>(UserAccessDaoSqlStatements.FindUserIdByApiKeySql,
                    new {apiKey});
        }

        public async Task<bool> HasAccountPermissionAsync(Guid userId, Guid accountId, Permission permission)
        {
            using var dbConnection = _acquireDbConnection();

            return await dbConnection.ExecuteScalarAsync<bool>(UserAccessDaoSqlStatements.AccountPermissionSql, new
            {
                accountId,
                userId,
                permission = permission.ToString()
            });
        }

        public async Task<bool> HasProjectPermissionAsync(Guid userId, Guid projectId, Permission permission)
        {
            var query = UserAccessDaoSqlStatements.ProjectPermissionsSql;
            using var dbConnection = _acquireDbConnection();

            return await dbConnection.ExecuteScalarAsync<bool>(query, new
            {
                projectId,
                userId,
                permission = permission.ToString()
            });
        }

        public async Task<bool> HasVersionPermissionAsync(Guid userId, Guid versionId, Permission permission)
        {
            var query = UserAccessDaoSqlStatements.VersionPermissionSql;
            using var dbConnection = _acquireDbConnection();

            return await dbConnection.ExecuteScalarAsync<bool>(query, new
            {
                userId,
                versionId,
                permission = permission.ToString()
            });
        }

        public async Task<bool> HasChangeLogLinePermissionAsync(Guid userId, Guid changeLogLineId,
            Permission permission)
        {
            var query = UserAccessDaoSqlStatements.ChangeLogLinePermissionSql;
            using var dbConnection = _acquireDbConnection();

            return await dbConnection
                .ExecuteScalarAsync<bool>(query, new
                {
                    userId,
                    changeLogLineId,
                    permission = permission.ToString()
                });
        }

        public async Task<bool> HasUserPermissionAsync(Guid userId, Permission permission)
        {
            using var dbConnection = _acquireDbConnection();

            return await dbConnection.ExecuteScalarAsync<bool>(UserAccessDaoSqlStatements.AccountUserPermission, new
            {
                userId,
                permission = permission.ToString()
            });
        }
    }
}