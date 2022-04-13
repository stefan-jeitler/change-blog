using System;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess.ExternalIdentity;
using CSharpFunctionalExtensions;

namespace ChangeBlog.DataAccess.MicrosoftIdentity;

/// <summary>
/// ChangeBlog.Api does not use JWT Auth exclusively while ChangeBlog.Management.Api does.
/// If API-KEY Auth is used, user infos can't be fetched from the identity provider due to the missing JsonWebToken.
/// So, external user info is inherently not supported. 
/// </summary>
public class ExternalUserInfoNotSupported : IExternalUserInfoDao
{
    public Task<UserInfo> GetUserInfoAsync()
    {
        throw new NotSupportedException("External UserInfo is not yet supported in this api.");
    }

    public Task<Maybe<UserPhoto>> GetUserPhotoAsync()
    {
        throw new NotSupportedException("External UserInfo Photo is not yet supported in this api.");
    }
}