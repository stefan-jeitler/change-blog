using System;
using System.Collections.Generic;
using ChangeBlog.Api.Localization.Resources;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.Presenters;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.UseCases.Commands.UpdateApiKey;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Management.Api.Presenters.V1;

public class UpdateApiKeyPresenter : BaseApiPresenter, IUpdateApiKeyOutputPort
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
            ErrorResponse.Create(ChangeBlogStrings.ApiKeyExpirationTooShort));
    }

    public void ExpirationTooLong(TimeSpan expiresIn, TimeSpan maxExpiration)
    {
        Response = new BadRequestObjectResult(ErrorResponse.Create(ChangeBlogStrings.ApiKeyExpirationTooLong));
    }
    
    public void ExpirationDateInThePast(DateTime expiresAt)
    {
        Response = new BadRequestObjectResult(ErrorResponse.Create(ChangeBlogStrings.ExpirationDateInThePast));
    }

    public void Conflict(Conflict conflict)
    {
        Response = conflict.ToResponse();
    }

    public void InvalidTitle(string title)
    {
        Response = new BadRequestObjectResult(ErrorResponse.Create(ChangeBlogStrings.InvalidApiKeyTitle));
    }

    public void Updated(Guid apiKeyId)
    {
        Response = new OkObjectResult(SuccessResponse.Create(ChangeBlogStrings.ApiKeyUpdated));
    }
}