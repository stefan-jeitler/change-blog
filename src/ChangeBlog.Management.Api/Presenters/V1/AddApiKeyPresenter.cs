using System;
using System.Collections.Generic;
using ChangeBlog.Api.Localization.Resources;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.Presenters;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.UseCases.Commands.AddApiKey;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Management.Api.Presenters.V1;

public class AddApiKeyPresenter : BaseApiPresenter, IAddApiKeyOutputPort
{
    public void ExpirationTooShort(TimeSpan expiresIn, TimeSpan minExpiration)
    {
        Response = new BadRequestObjectResult(
            ErrorResponse.Create(ChangeBlogStrings.ApiKeyExpirationTooShort));
    }

    public void ExpirationTooLong(TimeSpan expiresIn, TimeSpan maxExpiration)
    {
        Response = new BadRequestObjectResult(
            ErrorResponse.Create(ChangeBlogStrings.ApiKeyExpirationTooLong));
    }

    public void Conflict(Conflict conflict)
    {
        Response = conflict.ToResponse();
    }

    public void ApiKeyAdded(Guid apiKeyId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.ApiKeyId] = apiKeyId.ToString()
        };

        Response = new OkObjectResult(SuccessResponse.Create(ChangeBlogStrings.ApiKeyAdded, resourceIds));
    }

    public void InvalidTitle(string title)
    {
        Response = new BadRequestObjectResult(ErrorResponse.Create(ChangeBlogStrings.InvalidApiKeyTitle));
    }

    public void MaxApiKeyCountReached(ushort maxApiKeys)
    {
        Response = new UnprocessableEntityObjectResult(ErrorResponse.Create(string.Format(ChangeBlogStrings.MaxApiKeysCountReached, maxApiKeys)));
    }

    public void ExpirationDateInThePast(DateTime expiresAt)
    {
        Response = new BadRequestObjectResult(ErrorResponse.Create(ChangeBlogStrings.ExpirationDateInThePast));
    }
}