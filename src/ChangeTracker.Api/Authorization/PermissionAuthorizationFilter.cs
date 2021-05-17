using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Api.Extensions;
using ChangeTracker.Application.UseCases;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace ChangeTracker.Api.Authorization
{
    public class PermissionAuthorizationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            var permission = context.ActionDescriptor.EndpointMetadata.OfType<NeedsPermissionAttribute>().TryFirst();

            if (permission.HasValue && await IsAuthorizedAsync(context, permission.Value.Permission))
            {
                await next();
                return;
            }

            if (permission.HasNoValue && allowAnonymous)
            {
                await next();
                return;
            }

            context.Result =
                new ObjectResult(DefaultResponse.Create("You don't have permission to access this resource."))
                {
                    StatusCode = 403
                };
        }

        private static async Task<bool> IsAuthorizedAsync(ActionExecutingContext context, Permission permission)
        {
            var userId = context.HttpContext.GetUserId();
            var permissionCheck = context
                .HttpContext.RequestServices
                .GetRequiredService<PermissionCheck>();

            return await permissionCheck.HasPermission(context, userId, permission);
        }
    }
}