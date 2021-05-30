using System;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Application.UseCases.Commands.CloseProduct;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Presenters.V1.Product
{
    public class CloseProductApiPresenter : BaseApiPresenter, ICloseProductOutputPort
    {
        public void ProductAlreadyClosed(Guid productId)
        {
            Response = new NoContentResult();
        }

        public void ProductDoesNotExist(Guid productId)
        {
            Response = new NotFoundObjectResult(DefaultResponse.Create("Product not found.", productId));
        }

        public void ProductClosed(Guid productId)
        {
            Response = new NoContentResult();
        }
    }
}