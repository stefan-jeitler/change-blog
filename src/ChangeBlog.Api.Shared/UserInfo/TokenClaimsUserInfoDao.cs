using System;
using System.Security.Claims;
using ChangeBlog.Application.Boundaries.DataAccess.ExternalIdentity;
using Microsoft.AspNetCore.Http;

namespace ChangeBlog.Api.Shared.UserInfo;

public class TokenClaimsUserInfoDao : IExternalUserInfoDao
{
    private const string EmailsClaim = "emails";
    private const string IdentityProviderClaim = "http://schemas.microsoft.com/identity/claims/identityprovider";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenClaimsUserInfoDao(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    // TODO: use userinfo endpoint instead of extracting from claims
    public ExternalUserInfo GetUserInfo()
    {
        if (_httpContextAccessor.HttpContext is null)
            throw new Exception("Missing HttpContext");

        var user = _httpContextAccessor.HttpContext.User;

        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier) ??
                     throw new ArgumentNullException(nameof(ClaimTypes.NameIdentifier));
        
        var email = user.FindFirstValue(EmailsClaim) ?? throw new ArgumentNullException(nameof(EmailsClaim));
        var firstName = user.FindFirstValue(ClaimTypes.GivenName) ??
                        throw new ArgumentNullException(nameof(ClaimTypes.GivenName));
        
        var lastName = user.FindFirstValue(ClaimTypes.Surname) ??
                       throw new ArgumentNullException(nameof(ClaimTypes.Surname));
        
        var identityProvider = user.FindFirstValue(IdentityProviderClaim);

        return new ExternalUserInfo(userId, email, firstName, lastName, identityProvider);
    }
}