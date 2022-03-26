using System;
using System.Security.Claims;
using ChangeBlog.Api.Shared;
using Microsoft.AspNetCore.Http;

namespace ChangeBlog.Api.Extensions;

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

    public static Uri CreateLinkTo(this HttpContext ctx, string relativePath)
    {
        var request = ctx.Request;

        var uriBuilder = new UriBuilder
        {
            Scheme = "https",
            Host = request.Host.Host,
            Path = relativePath,
            Port = request.Host.Port ?? -1
        };

        return uriBuilder.Uri;
    }
}