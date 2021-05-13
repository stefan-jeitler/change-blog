using System;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.UseCases;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ChangeTracker.Api.Authorization
{
    public abstract class PermissionCheck
    {
        public abstract Task<bool> HasPermission(ActionExecutingContext httpContext, Guid userId, Permission permission);

        protected Guid? TryFindIdInRouteValues(HttpContext httpContext, string routeValueKey)
        {
            if (httpContext.Request.RouteValues.TryGetValue(routeValueKey, out var routeValue) &&
                Guid.TryParse(routeValue?.ToString(), out var id))
            {
                return id;
            }

            return null;
        }

        protected T TryFindInBody<T>(ActionExecutingContext context)
        {
            var id = context.ActionArguments
                .Values.SingleOrDefault(x => x is T);

            return (T)id;
        }
    }
}