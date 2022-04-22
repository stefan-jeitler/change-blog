using ChangeBlog.Application.Boundaries.DataAccess.ExternalIdentity;

namespace ChangeBlog.Application.Tests.TestDoubles;

public class FakeExternalUserInfoDao : IExternalUserInfoDao
{
    public ExternalUserInfo ExternalUserInfo { get; set; }

    public ExternalUserInfo GetUserInfo()
    {
        return ExternalUserInfo;
    }
}