using System;
using System.Collections.Generic;
using ChangeBlog.Api.DTOs;
using ChangeBlog.Api.Extensions;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Application.DataAccess;
using ChangeBlog.Application.UseCases.Commands.AddProduct;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Api.Presenters.V1.Product;

public class AddProductApiPresenter : BaseApiPresenter, IAddProductOutputPort
{
    private readonly HttpContext _httpContext;

    public AddProductApiPresenter(HttpContext httpContext)
    {
        _httpContext = httpContext;
    }

    public void AccountDoesNotExist(Guid accountId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.AccountId] = accountId.ToString()
        };

        Response = new NotFoundObjectResult(DefaultResponse.Create("Account not found.", resourceIds));
    }

    public void AccountDeleted(Guid accountId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.AccountId] = accountId.ToString()
        };

        Response = new UnprocessableEntityObjectResult(
            DefaultResponse.Create("The requested account has been deleted.", resourceIds));
    }

    public void InvalidName(string name)
    {
        Response = new BadRequestObjectResult(DefaultResponse.Create($"Invalid name {name}."));
    }

    public void ProductAlreadyExists(Guid productId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.ProductId] = productId.ToString()
        };

        Response = new ConflictObjectResult(
            DefaultResponse.Create("Product already exists.", resourceIds));
    }

    public void VersioningSchemeDoesNotExist(Guid versioningSchemeId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [nameof(versioningSchemeId)] = versioningSchemeId.ToString()
        };

        Response = new NotFoundObjectResult(DefaultResponse.Create("VersioningScheme not found.",
            resourceIds));
    }

    public void Conflict(Conflict conflict)
    {
        Response = conflict.ToResponse();
    }

    public void Created(Guid accountId, Guid productId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.AccountId] = accountId.ToString(),
            [KnownIdentifiers.ProductId] = productId.ToString()
        };

        var location = _httpContext.CreateLinkTo($"api/v1/products/{productId}");
        Response = new CreatedResult(location, DefaultResponse.Create("Product added.", resourceIds));
    }

    public void NotSupportedLanguageCode(string languageCode, IList<string> supportedLangCodes)
    {
        Response = new UnprocessableEntityObjectResult(
            DefaultResponse.Create(
                $"The given LanguageCode {languageCode} is not supported. Supported Codes are {string.Join(", ", supportedLangCodes)}"));
    }
}