using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess.Accounts;
using ChangeTracker.Domain;
using Dapper;

namespace ChangeTracker.DataAccess.Postgres.DataAccessObjects
{
    public class RolesDao : IRolesDao
    {
        private readonly IDbAccessor _dbAccessor;

        public RolesDao(IDbAccessor dbAccessor)
        {
            _dbAccessor = dbAccessor;
        }

        public async Task<IList<Role>> GetRolesAsync()
        {
            const string getRolePermissionsSql = @"
                select r.id,
                       r.name,
                       r.description,
                       rp.permission,
                       r.created_at AS createdAt
                from role r
                join role_permission rp on r.id = rp.role_id
                order by r.name";

            var roles = await _dbAccessor.DbConnection.QueryAsync<Role>(getRolePermissionsSql);

            return roles.GroupBy(x => x.Id)
                .Select(x =>
                {
                    var f = x.First();
                    return new Role(x.Key, f.Name, f.Description, f.CreatedAt, x.SelectMany(p => p.Permissions));
                })
                .ToList();
        }
    }
}