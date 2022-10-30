using System;
using System.Collections.Generic;
using ChangeBlog.Api.Localization.Resources;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.Presenters;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.UseCases.Users.AddApiKey;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Management.Api.Presenters.V1;

public class AddApiKeyApiPresenter : BaseApiPresenter, IAddApiKeyOutputPort
{
    public void ExpirationTooShort(DateTimeOffset expiresAt, TimeSpan minExpiration)
    {
        Response = new BadRequestObjectResult(
            ErrorResponse.Create(ChangeBlogStrings.ApiKeyExpirationTooShort, "expiresAt"));
    }

    public void ExpirationTooLong(DateTimeOffset expiresAt, TimeSpan maxExpiration)
    {
        Response = new BadRequestObjectResult(
            ErrorResponse.Create(ChangeBlogStrings.ApiKeyExpirationTooLong, "expiresAt"));
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

    public void MaxApiKeyCountReached(ushort maxApiKeys)
    {
        Response = new UnprocessableEntityObjectResult(
            ErrorResponse.Create(string.Format(ChangeBlogStrings.MaxApiKeysCountReached, maxApiKeys)));
    }

    public void ExpirationDateInThePast(DateTime expiresAt)
    {
        Response = new BadRequestObjectResult(ErrorResponse.Create(ChangeBlogStrings.ExpirationDateInThePast,
            "expiresAt"));
    }
}