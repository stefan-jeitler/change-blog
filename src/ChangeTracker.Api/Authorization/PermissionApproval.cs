using System;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.UseCases;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ChangeTracker.Api.Authorization
{
    public abstract class PermissionApproval
    {
        public abstract Task<bool> HasPermission(ActionExecutingContext context, Guid userId, Permission permission);

        protected static Guid? TryFindIdInHeader(HttpContext httpContext, string key)
        {
            if (httpContext.Request.RouteValues.TryGetValue(key, out var routeValue) &&
                Guid.TryParse(routeValue?.ToString(), out var idInRoute))
                return idInRoute;

            if (httpContext.Request.Query.TryGetValue(key, out var queryParameter) &&
                Guid.TryParse(queryParameter, out var idInQueryString))
                return idInQueryString;

            return null;
        }

        protected static T TryFindInBody<T>(ActionExecutingContext context)
        {
            var id = context.ActionArguments
                .Values.SingleOrDefault(x => x is T);

            return (T) id;
        }
    }
}