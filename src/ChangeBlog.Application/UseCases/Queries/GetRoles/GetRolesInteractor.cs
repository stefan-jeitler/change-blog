using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeBlog.Application.DataAccess.Accounts;

namespace ChangeBlog.Application.UseCases.Queries.GetRoles
{
    public class GetRolesInteractor : IGetRoles
    {
        private readonly IRolesDao _rolesDao;

        public GetRolesInteractor(IRolesDao rolesDao)
        {
            _rolesDao = rolesDao ?? throw new ArgumentNullException(nameof(rolesDao));
        }

        public async Task<IList<RoleResponseModel>> ExecuteAsync(string role)
        {
            var roles = await _rolesDao.GetRolesAsync();

            var result = role is null
                ? roles
                : roles.Where(x => x.Name.Value.Equals(role.Trim(), StringComparison.OrdinalIgnoreCase));

            return result.Select(x =>
                    new RoleResponseModel(x.Name,
                        x.Permissions.Select(v => v.ToString())
                            .ToList()))
                .ToList();
        }
    }
}
