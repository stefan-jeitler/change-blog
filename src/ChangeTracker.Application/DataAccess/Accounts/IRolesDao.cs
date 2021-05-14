using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeTracker.Domain;

namespace ChangeTracker.Application.DataAccess.Accounts
{
    public interface IRolesDao
    {
        Task<IList<Role>> GetRolesAsync();
    }
}