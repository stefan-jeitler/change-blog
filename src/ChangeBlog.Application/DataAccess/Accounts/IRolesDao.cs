using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeBlog.Domain;

namespace ChangeBlog.Application.DataAccess.Accounts;

public interface IRolesDao
{
    Task<IList<Role>> GetRolesAsync();
}