using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess.Accounts;
using ChangeTracker.Domain;
using ChangeTracker.Domain.Common;
using Dapper;
using Microsoft.Extensions.Logging;

namespace ChangeTracker.DataAccess.Postgres.DataAccessObjects.Users
{
    public class RolesDao : IRolesDao
    {
        private const string GetRolePermissionsSql = @"
                select '' as type,
                       r.id,
                       r.name,
                       r.description,
                       rp.permission,
                       r.created_at AS createdAt
                from role r
                         left join role_permission rp on r.id = rp.role_id
                order by r.name";

        private readonly IDbAccessor _dbAccessor;
        private readonly ILogger<RolesDao> _logger;

        public RolesDao(IDbAccessor dbAccessor, ILogger<RolesDao> logger)
        {
            _dbAccessor = dbAccessor;
            _logger = logger;
        }

        public async Task<IList<Role>> GetRolesAsync()
        {
            var permissions = (await _dbAccessor.DbConnection.QueryAsync<RolePermissionDto>(GetRolePermissionsSql))
                .AsList();

            var notSupportedPermissions = permissions
                .Where(x => x.Permission is null)
                .ToList();

            if (notSupportedPermissions.Any())
            {
                _logger.LogWarning("There are permissions that are not supported by the app.", string.Join(", ", notSupportedPermissions));
            }

            return permissions
                .Where(x => x.Permission is not null)
                .GroupBy(x => x.Id)
                .Select(x =>
                {
                    var f = x.First();

                    return new Role(x.Key,
                        Name.Parse(f.Name),
                        Text.Parse(f.Description),
                        f.CreatedAt,
                        x.Select(p => p.Permission!.Value));
                })
                .ToList();
        }
    }
}