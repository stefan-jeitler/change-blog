using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess.Users;
using ChangeBlog.Domain;
using ChangeBlog.Domain.Miscellaneous;
using Dapper;
using Microsoft.Extensions.Logging;
using static ChangeBlog.DataAccess.Postgres.DataAccessObjects.Users.UserAccess.UserAccessDaoSqlStatements;

namespace ChangeBlog.DataAccess.Postgres.DataAccessObjects.Users.UserAccess;

public class UserAccessDao : IUserAccessDao
{
    private readonly Func<IDbConnection> _acquireDbConnection;
    private readonly ILogger<UserAccessDao> _logger;

    public UserAccessDao(Func<IDbConnection> acquireDbConnection, ILogger<UserAccessDao> logger)
    {
        _acquireDbConnection = acquireDbConnection;
        _logger = logger;
    }

    public async Task<IEnumerable<Role>> GetAccountRolesAsync(Guid userId, Guid accountId)
    {
        using var dbConnection = _acquireDbConnection();

        var rolePermissions = await dbConnection.QueryAsync<RolePermissionDto>(GetAccountPermissionsSql, new
        {
            userId,
            accountId
        });

        return ParseRoleDtos(rolePermissions.AsList());
    }

    public async Task<AccountProductRolesDto> GetRolesByProductIdAsync(Guid userId, Guid productId)
    {
        using var dbConnection = _acquireDbConnection();

        var rolePermissions = await dbConnection.QueryAsync<RolePermissionDto>(PermissionsByProductIdSql, new
        {
            userId,
            productId
        });

        return ParseAccountAndProductRoles(rolePermissions.AsList());
    }

    public async Task<AccountProductRolesDto> GetRolesByVersionIdAsync(Guid userId, Guid versionId)
    {
        using var dbConnection = _acquireDbConnection();

        var rolePermissions = await dbConnection.QueryAsync<RolePermissionDto>(PermissionsByVersionIdSql, new
        {
            userId,
            versionId
        });

        return ParseAccountAndProductRoles(rolePermissions.AsList());
    }

    public async Task<AccountProductRolesDto> GetRolesByChangeLogLineIdAsync(Guid userId, Guid changeLogLineId)
    {
        using var dbConnection = _acquireDbConnection();

        var rolePermissions = await dbConnection.QueryAsync<RolePermissionDto>(PermissionsByChangeLogLineIdSql,
            new
            {
                userId,
                changeLogLineId
            });

        return ParseAccountAndProductRoles(rolePermissions.AsList());
    }

    public async Task<Guid?> FindActiveUserIdByApiKeyAsync(string apiKey)
    {
        if (string.IsNullOrEmpty(apiKey)) return null;

        using var dbConnection = _acquireDbConnection();
        return await dbConnection
            .QueryFirstOrDefaultAsync<Guid?>(FindActiveUserIdByApiKeySql,
                new {apiKey});
    }

    public async Task<Guid?> FindActiveUserByExternalUserId(string externalUserId)
    {
        if (string.IsNullOrEmpty(externalUserId)) return null;

        using var dbConnection = _acquireDbConnection();
        return await dbConnection
            .QueryFirstOrDefaultAsync<Guid?>(FindActiveUserIdByExternalUserIdSql,
                new {externalUserId});
    }

    private AccountProductRolesDto ParseAccountAndProductRoles(
        IReadOnlyCollection<RolePermissionDto> rolePermissions)
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
        var notSupportedPermissionsExists = rolePermissions
            .Any(x => !x.Permission.HasValue);

        if (notSupportedPermissionsExists)
            _logger.LogWarning("There are permissions that are not supported by the app");

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