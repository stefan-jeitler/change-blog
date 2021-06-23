using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Api.Extensions;
using ChangeTracker.Application.UseCases;
using ChangeTracker.Domain.Authorization;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
// ReSharper disable InconsistentNaming

namespace ChangeTracker.Api.Authorization
{
    public class AuthorizationFilter : IAsyncActionFilter
    {
        private static readonly ActionResult UnauthorizedResult = 
                new ObjectResult(DefaultResponse.Create("You don't have permission to access this resource."))
                {
                    StatusCode = 403
                };

        private static readonly ActionResult NotFoundResult = new NotFoundObjectResult(DefaultResponse.Create("Requested resource not found."));

        private static readonly ActionResult InternalServerError =
            new StatusCodeResult(StatusCodes.Status500InternalServerError);

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            var permission = context.ActionDescriptor.EndpointMetadata.OfType<NeedsPermissionAttribute>().TryFirst();

            if (permission.HasNoValue && allowAnonymous)
            {
                await next();
                return;
            }

            var authState = await GetAuthorizationStateAsync(context, permission.Value.Permission);

            switch (authState)
            {
                case AuthorizationState.Authorized:
                    await next();
                    break;
                case AuthorizationState.Unauthorized:
                    context.Result = UnauthorizedResult;
                    break;
                case AuthorizationState.Inaccessible:
                    context.Result = NotFoundResult;
                    break;
                default:
                    context.Result = InternalServerError;
                    break;
            }
        }

        private static async Task<AuthorizationState> GetAuthorizationStateAsync(ActionExecutingContext context, Permission permission)
        {
            var userId = context.HttpContext.GetUserId();
            var permissionCheck = context
                .HttpContext.RequestServices
                .GetRequiredService<AuthorizationHandler>();

            return await permissionCheck.GetAuthorizationState(context, userId, permission);
        }
    }
}