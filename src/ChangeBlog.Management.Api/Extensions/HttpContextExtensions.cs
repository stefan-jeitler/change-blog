﻿using System;
using System.Security.Claims;
using ChangeBlog.Api.Shared;
using Microsoft.AspNetCore.Http;

namespace ChangeBlog.Management.Api.Extensions;

public static class HttpContextExtensions
{
    public static Guid GetUserId(this HttpContext httpContext)
    {
        var userIdValue = httpContext
            .User.FindFirstValue(Constants.AppClaims.UserId);

        return Guid.TryParse(userIdValue, out var userId)
            ? userId
            : Guid.Empty;
    }
}