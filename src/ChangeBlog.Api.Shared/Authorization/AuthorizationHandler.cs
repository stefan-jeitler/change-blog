using System;
using System.Linq;
using System.Threading.Tasks;
using ChangeBlog.Domain.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ChangeBlog.Api.Shared.Authorization;

public abstract class AuthorizationHandler
{
    public abstract Task<AuthorizationState> GetAuthorizationStateAsync(
        ActionExecutingContext context,
        Guid userId,
        Permission permission);

    protected static Guid? TryFindIdInRoute(HttpContext httpContext, string key)
    {
        if (httpContext.Request.RouteValues.TryGetValue(key, out var routeValue) &&
            Guid.TryParse(routeValue?.ToString(), out var idInRoute))
            return idInRoute;

        return null;
    }

    protected static T TryFindInBody<T>(ActionExecutingContext context)
    {
        var id = context.ActionArguments.Values.SingleOrDefault(x => x is T);

        return (T) id;
    }
}