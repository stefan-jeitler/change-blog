using System;
using System.Collections.Generic;
using ChangeBlog.Api.DTOs.V1.Version;
using ChangeBlog.Api.Localization.Resources;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.Presenters;
using ChangeBlog.Application.UseCases.SharedModels;
using ChangeBlog.Application.UseCases.Versions.GetLatestVersion;
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

        Response = new NotFoundObjectResult(
            ErrorResponse.Create(ChangeBlogStrings.ProductWithoutVersions, resourceIds));
    }

    public void ProductDoesNotExist()
    {
        Response = new NotFoundObjectResult(ErrorResponse.Create(ChangeBlogStrings.ProductNotFound));
    }
}