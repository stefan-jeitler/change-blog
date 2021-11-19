using System;
using System.Collections.Generic;
using ChangeBlog.Api.DTOs.V1.Version;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Application.UseCases.Queries.GetLatestVersion;
using ChangeBlog.Application.UseCases.Queries.SharedModels;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Api.Presenters.V1.Version;

public class GetLatestVersionApiPresenter : BaseApiPresenter, IGetLatestVersionOutputPort
{
    public void VersionFound(VersionResponseModel versionResponseModel)
    {
        Response = new OkObjectResult(VersionDto.FromResponseModel(versionResponseModel));
    }

    public void NoVersionExists(Guid productId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.ProductId] = productId.ToString()
        };

        Response = new NotFoundObjectResult(DefaultResponse.Create("Product does not contain any version.", resourceIds));
    }

    public void ProductDoesNotExist()
    {
        Response = new NotFoundObjectResult(DefaultResponse.Create("Product not found."));
    }
}