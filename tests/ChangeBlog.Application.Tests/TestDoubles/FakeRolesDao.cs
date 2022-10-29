using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess.Accounts;
using ChangeBlog.Domain;

namespace ChangeBlog.Application.Tests.TestDoubles;

public class FakeRolesDao : IRolesDao
{
    public List<Role> Roles { get; } = new();

    public Task<IList<Role>> GetRolesAsync() => Task.FromResult(Roles as IList<Role>);
}