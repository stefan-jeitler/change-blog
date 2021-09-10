﻿namespace ChangeBlog.Application.DataAccess.ExternalIdentity
{
    public record UserInfo(string Id,
        string FullName,
        string FirstName,
        string LastName,
        string Email,
        string IdentityProvider);
}