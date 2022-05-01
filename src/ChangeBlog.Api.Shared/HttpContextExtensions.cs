using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ChangeBlog.Api.Shared;

public static class HttpContextExtensions
{
    public static Guid GetUserId(this HttpContext httpContext)
    {
        var userIdValue = httpContext
            .User.FindFirstValue(ApiConstants.AppClaims.UserId);

        return Guid.TryParse(userIdValue, out var userId)
            ? userId
            : Guid.Empty;
    }

    public static string GetEmail(this HttpContext httpContext)
    {
        return httpContext.User.FindFirstValue(ClaimTypes.Email) ??
               throw new Exception("Missing email address in claims.");
    }

    public static string GetFirstName(this HttpContext httpContext)
    {
        return httpContext.User.FindFirstValue(ClaimTypes.GivenName) ??
               throw new Exception("Missing email address in claims.");
    }

    public static string GetLastName(this HttpContext httpContext)
    {
        return httpContext.User.FindFirstValue(ClaimTypes.Surname) ??
               throw new Exception("Missing email address in claims.");
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