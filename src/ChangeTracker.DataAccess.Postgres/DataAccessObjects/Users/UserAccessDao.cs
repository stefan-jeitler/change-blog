using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess.Users;
using ChangeTracker.Application.UseCases;
using ChangeTracker.Domain;
using ChangeTracker.Domain.Authorization;
using ChangeTracker.Domain.Common;
using Dapper;
using Microsoft.Extensions.Logging;
using static ChangeTracker.DataAccess.Postgres.DataAccessObjects.Users.UserAccessDaoSqlStatements;

namespace ChangeTracker.DataAccess.Postgres.DataAccessObjects.Users
{
    public class UserAccessDao : IUserAccessDao
    {
        private readonly Func<IDbConnection> _acquireDbConnection;
        private readonly ILogger<UserAccessDao> _logger;

        public UserAccessDao(Func<IDbConnection> acquireDbConnection, ILogger<UserAccessDao> logger)
        {
            _acquireDbConnection = acquireDbConnection;
            _logger = logger;
        }

        public async Task<Guid?> FindUserIdAsync(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey)) return null;

            using var dbConnection = _acquireDbConnection();
            return await dbConnection
                .QueryFirstOrDefaultAsync<Guid?>(FindUserIdByApiKeySql,
                    new {apiKey});
        }

        public async Task<IEnumerable<Role>> GetAccountRolesAsync(Guid accountId, Guid userId)
        {
            using var dbConnection = _acquireDbConnection();

            var rolePermissions =  await dbConnection.QueryAsync<RolePermissionDto>(GetAccountPermissionsSql, new
            {
                userId,
                accountId
            });

            return ParseRoleDtos(rolePermissions.AsList());
        }

        public async Task<IEnumerable<Role>> GetAccountsRolesAsync(Guid userId)
        {
            using var dbConnection = _acquireDbConnection();

            var rolePermissions = await dbConnection.QueryAsync<RolePermissionDto>(GetUserAccountsPermissionsSql, new
            {
                userId
            });

            return ParseRoleDtos(rolePermissions.AsList());
        }

        public async Task<AccountProductRolesDto> GetRolesByProductIdAsync(Guid userId, Guid productId)
        {
            using var dbConnection = _acquireDbConnection();

            var rolePermissions = await dbConnection.QueryAsync<RolePermissionDto>(GetPermissionsByProductIdSql, new
            {
                userId,
                productId
            });

            return ParseAccountAndProductRoles(rolePermissions.AsList());
        }

        public async Task<AccountProductRolesDto> GetRolesByVersionIdAsync(Guid userId, Guid versionId)
        {
            using var dbConnection = _acquireDbConnection();

            var rolePermissions = await dbConnection.QueryAsync<RolePermissionDto>(GetPermissionsByVersionIdSql, new
            {
                userId,
                versionId
            });

            return ParseAccountAndProductRoles(rolePermissions.AsList());
        }

        public async Task<AccountProductRolesDto> GetRolesByChangeLogLineIdAsync(Guid userId, Guid changeLogLineId)
        {
            using var dbConnection = _acquireDbConnection();

            var rolePermissions = await dbConnection.QueryAsync<RolePermissionDto>(GetPermissionsByChangeLogLineIdSql, new
            {
                userId,
                changeLogLineId
            });

            return ParseAccountAndProductRoles(rolePermissions.AsList());
        }

        private AccountProductRolesDto ParseAccountAndProductRoles(IReadOnlyCollection<RolePermissionDto> rolePermissions)
        {
            var accountRoles = rolePermissions
                .Where(x => x.Type == "Account")
                .ToList();

            var productRoles = rolePermissions
                .Where(x => x.Type == "Product")
                .ToList();

            return new AccountProductRolesDto(ParseRoleDtos(accountRoles), ParseRoleDtos(productRoles));
        }

        private IEnumerable<Role> ParseRoleDtos(IReadOnlyCollection<RolePermissionDto> rolePermissions)
        {
            var notSupportedRoles = rolePermissions
                .Where(x => !x.Permission.HasValue)
                .ToList();

            if (notSupportedRoles.Any())
            {
                _logger.LogWarning("There are permissions that are not supported by the app.", string.Join(", ", notSupportedRoles));
            }

            return rolePermissions
                .Where(x => x.Permission is not null)
                .GroupBy(x => x.Id)
                .Select(x =>
                {
                    var f = x.First();
                    var permissions = x.Select(p => p.Permission!.Value);

                    return new Role(x.Key,
                        Name.Parse(f.Name),
                        Text.Parse(f.Description),
                        f.CreatedAt,
                        permissions);
                })
                .ToList();
        }
    }
}