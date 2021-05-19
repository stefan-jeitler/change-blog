using System;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Application.UseCases.Commands.CloseProduct;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Presenters.v1.Project
{
    public class CloseProductApiPresenter : BasePresenter, ICloseProductOutputPort
    {
        public void ProductAlreadyClosed(Guid productId)
        {
            Response = new OkObjectResult(DefaultResponse.Create($"Product with Id {productId} closed."));
        }

        public void ProductDoesNotExist()
        {
            Response = new NotFoundObjectResult(DefaultResponse.Create("Product not found."));
        }

        public void ProductClosed(Guid productId)
        {
            Response = new OkObjectResult(DefaultResponse.Create($"Product with Id {productId} closed."));
        }

        public void Conflict(string reason)
        {
            Response = new ConflictObjectResult(DefaultResponse.Create(reason));
        }
    }
}