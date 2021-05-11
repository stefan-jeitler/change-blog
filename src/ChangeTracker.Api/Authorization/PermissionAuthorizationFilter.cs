using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Api.Extensions;
using ChangeTracker.Application.UseCases;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;

namespace ChangeTracker.Api.Authorization
{
    public class PermissionAuthorizationFilter : IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var permission = context.ActionDescriptor.EndpointMetadata.OfType<RequiresAttribute>().TryFirst();
            if (permission.HasValue && await IsAuthorizedAsync(context, permission.Value.Permission))
            {
                return;
            }

            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous)
            {
                return;
            }

            context.Result =
                new ObjectResult(NonSuccessResponse.Create("You don't have permission to access this resource."))
                {
                    StatusCode = 403
                };
        }

        private static async Task<bool> IsAuthorizedAsync(ActionContext context, Permission permission)
        {
            var userId = context.HttpContext.GetUserId();
            var permissionCheck = context
                .HttpContext.RequestServices
                .GetRequiredService<IPermissionCheck>();

            return await permissionCheck.HasPermission(context.HttpContext, userId, permission);
        }
    }
}