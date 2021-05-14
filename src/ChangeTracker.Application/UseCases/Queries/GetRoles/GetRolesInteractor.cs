using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess.Accounts;

namespace ChangeTracker.Application.UseCases.Queries.GetRoles
{
    public class GetRolesInteractor : IGetRoles
    {
        private readonly IRolesDao _rolesDao;

        public GetRolesInteractor(IRolesDao rolesDao)
        {
            _rolesDao = rolesDao;
        }

        public async Task<IList<RoleResponseModel>> ExecuteAsync(string role)
        {
            var roles = await _rolesDao.GetRolesAsync();

            var filtered = role is null
                ? roles
                : roles.Where(x => x.Name.Value.Equals(role.Trim(), StringComparison.OrdinalIgnoreCase));

            return filtered.Select(x =>
                    new RoleResponseModel(x.Name,
                        x.Description,
                        x.Permissions.Select(v => v.Value)
                            .ToList()))
                .ToList();
        }
    }
}