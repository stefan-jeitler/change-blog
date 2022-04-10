using System;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess.ExternalIdentity;
using CSharpFunctionalExtensions;

namespace ChangeBlog.DataAccess.MicrosoftIdentity;

public class ExternalUserInfoNotSupported : IExternalUserInfoDao
{
    public Task<UserInfo> GetUserInfoAsync()
    {
        throw new NotSupportedException("External UserInfo is not yet supported.");
    }

    public Task<Maybe<UserPhoto>> GetUserPhotoAsync()
    {
        throw new NotSupportedException("External UserInfo Photo is not yet supported.");
    }
}