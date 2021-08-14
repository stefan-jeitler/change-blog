using System;
using System.Linq;
using System.Threading.Tasks;
using ChangeBlog.Api.DTOs;
using ChangeBlog.Api.Extensions;
using ChangeBlog.Domain.Authorization;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
// ReSharper disable InconsistentNaming
// ReSharper disable ConvertIfStatementToSwitchStatement

namespace ChangeBlog.Api.Authorization
{
    public class AuthorizationFilter : IAsyncActionFilter
    {
        private static readonly ActionResult UnauthorizedResult =
            new ObjectResult(DefaultResponse.Create("You don't have permission to access this resource."))
            {
                StatusCode = StatusCodes.Status403Forbidden
            };

        private static readonly ActionResult NotFoundResult =
            new NotFoundObjectResult(DefaultResponse.Create("Requested resource not found."));

        private static readonly ActionResult InternalServerError =
            new StatusCodeResult(StatusCodes.Status500InternalServerError);

        private readonly AuthorizationHandler _authorizationHandler;
        private readonly ILogger<AuthorizationFilter> _logger;

        public AuthorizationFilter(ILogger<AuthorizationFilter> logger, AuthorizationHandler authorizationHandler)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _authorizationHandler =
                authorizationHandler ?? throw new ArgumentNullException(nameof(authorizationHandler));
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            var permission = context.ActionDescriptor.EndpointMetadata.OfType<NeedsPermissionAttribute>().TryFirst();

            if (permission.HasNoValue && allowAnonymous)
            {
                await next();
                return;
            }

            if (permission.HasNoValue && !allowAnonymous)
            {
                var actionName = context.ActionDescriptor.DisplayName;
                _logger.LogError(
                    $"The requested action '{actionName}' has no {nameof(NeedsPermissionAttribute)} and no {nameof(AllowAnonymousAttribute)}.");

                context.Result = InternalServerError;
                return;
            }

            await HandleAuthorizationAsync(context, next, permission);
        }

        private async Task HandleAuthorizationAsync(ActionExecutingContext context, ActionExecutionDelegate next,
            Maybe<NeedsPermissionAttribute> permission)
        {
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
                    _logger.LogCritical("AuthorizationState is not supported.", authState);
                    context.Result = InternalServerError;
                    break;
            }
        }

        private async Task<AuthorizationState> GetAuthorizationStateAsync(ActionExecutingContext context,
            Permission permission)
        {
            var userId = context.HttpContext.GetUserId();
            return await _authorizationHandler.GetAuthorizationState(context, userId, permission);
        }
    }
}
