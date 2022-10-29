using System;
using System.Collections.Generic;
using ChangeBlog.Api.Localization.Resources;
using ChangeBlog.Api.Shared;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.Presenters;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.UseCases.Accounts.CreateAccount;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Management.Api.Presenters.V1;

public class CreateAccountPresenter : BaseApiPresenter, ICreateAccountOutputPort
{
    private readonly HttpContext _httpContext;

    public CreateAccountPresenter(HttpContext httpContext)
    {
        _httpContext = httpContext;
    }

    public void Created(Guid accountId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.AccountId] = accountId.ToString()
        };

        var resource = _httpContext.CreateLinkTo($"api/v1/accounts/{accountId}");
        Response = new CreatedResult(resource, SuccessResponse.Create(ChangeBlogStrings.AccountCreated, resourceIds));
    }

    public void InvalidName(string name)
    {
        var message = string.Format(ChangeBlogStrings.InvalidName, name);
        Response = new BadRequestObjectResult(ErrorResponse.Create(message, nameof(name)));
    }

    public void Conflict(Conflict conflict)
    {
        Response = conflict.ToResponse();
    }

    public void AccountAlreadyExists(Guid accountId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.ApiKeyId] = accountId.ToString()
        };

        Response = new UnprocessableEntityObjectResult(ErrorResponse.Create(ChangeBlogStrings.NameAlreadyTaken,
            resourceIds));
    }

    public void TooManyAccountsCreated(ushort limit)
    {
        var message = string.Format(ChangeBlogStrings.MaxAccountsCreatedByUserReached, limit);
        Response = new UnprocessableEntityObjectResult(ErrorResponse.Create(message));
    }
}