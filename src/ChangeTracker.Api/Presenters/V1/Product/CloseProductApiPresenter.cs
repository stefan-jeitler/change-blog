using System;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Application.UseCases.Commands.CloseProduct;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Presenters.V1.Product
{
    public class CloseProductApiPresenter : BasePresenter, ICloseProductOutputPort
    {
        public void ProductAlreadyClosed(Guid productId)
        {
            Response = new NoContentResult();
        }

        public void ProductDoesNotExist()
        {
            Response = new NotFoundObjectResult(DefaultResponse.Create("Product not found."));
        }

        public void ProductClosed(Guid productId)
        {
            Response = new NoContentResult();
        }
    }
}