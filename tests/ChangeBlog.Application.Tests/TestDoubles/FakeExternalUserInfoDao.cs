using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess.ExternalIdentity;
using CSharpFunctionalExtensions;

namespace ChangeBlog.Application.Tests.TestDoubles;

public class FakeExternalUserInfoDao : IExternalUserInfoDao
{
    public UserInfo UserInfo { get; set; }

    public Task<UserInfo> GetUserInfoAsync()
    {
        return Task.FromResult(UserInfo);
    }

    public Task<Maybe<UserPhoto>> GetUserPhotoAsync()
    {
        return Task.FromResult(Maybe<UserPhoto>.None);
    }
}