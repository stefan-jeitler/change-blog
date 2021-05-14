using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.Queries.GetRoles
{
    public interface IGetRoles
    {
        Task<IList<RoleResponseModel>> ExecuteAsync(string role);
    }
}