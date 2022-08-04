using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.Accounts.GetRoles;

public interface IGetRoles
{
    Task<IList<RoleResponseModel>> ExecuteAsync(string role);
}