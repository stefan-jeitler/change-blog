using System;
using System.Collections.Generic;
using ChangeBlog.Api.Localization.Resources;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.Presenters;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.UseCases.Users.UpdateApiKey;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Management.Api.Presenters.V1;

public class UpdateApiKeyApiPresenter : BaseApiPresenter, IUpdateApiKeyOutputPort
{
    public void ApiKeyNotFound(Guid apiKeyId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.ApiKeyId] = apiKeyId.ToString()
        };

        var responseMessage = ErrorResponse.Create(ChangeBlogStrings.ApiKeyNotFound, resourceIds);
        Response = new NotFoundObjectResult(responseMessage);
    }

    public void ExpirationTooShort(TimeSpan expiresIn, TimeSpan minExpiration)
    {
        Response = new BadRequestObjectResult(
            ErrorResponse.Create(ChangeBlogStrings.ApiKeyExpirationTooShort, "expiresAt"));
    }

    public void ExpirationTooLong(TimeSpan expiresIn, TimeSpan maxExpiration)
    {
        Response = new BadRequestObjectResult(ErrorResponse.Create(ChangeBlogStrings.ApiKeyExpirationTooLong,
            "expiresAt"));
    }

    public void ExpirationDateInThePast(DateTime expiresAt)
    {
        Response = new BadRequestObjectResult(ErrorResponse.Create(ChangeBlogStrings.ExpirationDateInThePast,
            "expiresAt"));
    }

    public void Conflict(Conflict conflict)
    {
        Response = conflict.ToResponse();
    }

    public void InvalidTitle(string title)
    {
        Response = new BadRequestObjectResult(ErrorResponse.Create(ChangeBlogStrings.InvalidApiKeyTitle, "title"));
    }

    public void Updated(Guid apiKeyId)
    {
        Response = new OkObjectResult(SuccessResponse.Create(ChangeBlogStrings.ApiKeyUpdated));
    }
}