using System.Threading.Tasks;
using ChangeBlog.Application.DataAccess.ExternalIdentity;

namespace ChangeBlog.Application.Tests.TestDoubles;

public class FakeExternalUserInfoDao : IExternalUserInfoDao
{
    public UserInfo UserInfo { get; set; }

    public Task<UserInfo> GetAsync()
    {
        return Task.FromResult(UserInfo);
    }
}