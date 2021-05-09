using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ChangeTracker.Api.Extensions
{
    public static class HttpContextExtensions
    {
        public static Guid GetUserId(this HttpContext httpContext)
        {
            var userIdValue = httpContext
                .User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(userIdValue, out var userId) 
                ? userId 
                : Guid.Empty;
        }
    }
}
