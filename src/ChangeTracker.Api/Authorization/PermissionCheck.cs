using System;
using System.Threading.Tasks;
using ChangeTracker.Application.UseCases;
using Microsoft.AspNetCore.Http;

namespace ChangeTracker.Api.Authorization
{
    public abstract class PermissionCheck
    {
        public abstract Task<bool> HasPermission(HttpContext httpContext, Guid userId, Permission permission);

        protected Guid? TryFindIdInRouteValues(HttpContext httpContext, string routeValueKey)
        {
            if (httpContext.Request.RouteValues.TryGetValue(routeValueKey, out var routeValue) &&
                Guid.TryParse(routeValue?.ToString(), out var id))
            {
                return id;
            }

            return null;
        }
    }
}