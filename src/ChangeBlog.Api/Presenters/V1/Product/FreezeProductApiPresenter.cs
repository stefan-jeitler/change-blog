using System;
using System.Collections.Generic;
using ChangeBlog.Api.Localization.Resources;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.Presenters;
using ChangeBlog.Application.UseCases.Products.CloseProduct;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Api.Presenters.V1.Product;

public class FreezeProductApiPresenter : BaseApiPresenter, IFreezeProductOutputPort
{
    public void ProductAlreadyFreezed(Guid productId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.ProductId] = productId.ToString()
        };

        Response = new OkObjectResult(SuccessResponse.Create(ChangeBlogStrings.ProductHasBeenClosed, resourceIds));
    }

    public void ProductDoesNotExist(Guid productId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.ProductId] = productId.ToString()
        };

        Response = new NotFoundObjectResult(ErrorResponse.Create(ChangeBlogStrings.ProductNotFound, resourceIds));
    }

    public void ProductFreezed(Guid productId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.ProductId] = productId.ToString()
        };

        Response = new OkObjectResult(SuccessResponse.Create(ChangeBlogStrings.ProductHasBeenClosed, resourceIds));
    }
}