using System;
using System.Linq;
using System.Threading.Tasks;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Domain.Authorization;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

// ReSharper disable InconsistentNaming
// ReSharper disable ConvertIfStatementToSwitchStatement

namespace ChangeBlog.Api.Shared.Authorization;

public class AuthorizationFilter : IAsyncActionFilter
{
    private static readonly ActionResult UnauthorizedResult =
        new ObjectResult(ErrorResponse.Create("You don't have permission to access this resource."))
        {
            StatusCode = StatusCodes.Status403Forbidden
        };

    private static readonly ActionResult NotFoundResult =
        new NotFoundObjectResult(ErrorResponse.Create("Requested resource not found."));

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
        var skipAuthorization = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any() ||
                                context.ActionDescriptor.EndpointMetadata.OfType<SkipAuthorizationAttribute>()
                                    .Any();

        var permission = context.ActionDescriptor.EndpointMetadata.OfType<NeedsPermissionAttribute>().TryFirst();

        if (permission.HasNoValue && skipAuthorization)
        {
            await next();
            return;
        }

        if (permission.HasNoValue && !skipAuthorization)
        {
            var actionName = context.ActionDescriptor.DisplayName;
            _logger.LogError(
                "The requested action '{ActionName}' hasn't one of the attributes: {PermissionAttribute}, {AllowAnonymous} or {SkipAuthorization}",
                actionName, nameof(NeedsPermissionAttribute), nameof(AllowAnonymousAttribute),
                nameof(SkipAuthorizationAttribute));

            context.Result = InternalServerError;
            return;
        }

        await HandleAuthorizationAsync(context, next, permission);
    }

    private async Task HandleAuthorizationAsync(ActionExecutingContext context, ActionExecutionDelegate next,
        Maybe<NeedsPermissionAttribute> permission)
    {
        var userId = context.HttpContext.GetUserId();
        var authState =
            await _authorizationHandler.GetAuthorizationState(context, userId, permission.GetValueOrThrow().Permission);

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
                _logger.LogCritical("AuthorizationState is not supported. {AuthState}", authState);
                context.Result = InternalServerError;
                break;
        }
    }
}