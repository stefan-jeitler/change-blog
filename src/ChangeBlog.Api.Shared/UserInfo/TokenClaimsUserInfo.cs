using System;
using System.Security.Claims;
using ChangeBlog.Application.Boundaries.DataAccess.ExternalIdentity;
using Microsoft.AspNetCore.Http;

namespace ChangeBlog.Api.Shared.UserInfo;

public class TokenClaimsUserInfo : IExternalUserInfoDao
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenClaimsUserInfo(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public ExternalUserInfo GetUserInfo()
    {
        if (_httpContextAccessor.HttpContext is null)
            throw new Exception("Missing HttpContext");

        var user = _httpContextAccessor.HttpContext.User;
        
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = user.FindFirstValue("emails");
        var firstName = user.FindFirstValue(ClaimTypes.GivenName);
        var lastName = user.FindFirstValue(ClaimTypes.Surname);

        return new ExternalUserInfo(userId, email, firstName, lastName);
    }
}