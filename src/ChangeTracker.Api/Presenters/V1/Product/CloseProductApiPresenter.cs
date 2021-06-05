﻿using System;
using System.Collections.Generic;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Application.UseCases.Commands.CloseProduct;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Presenters.V1.Product
{
    public class CloseProductApiPresenter : BaseApiPresenter, ICloseProductOutputPort
    {
        public void ProductAlreadyClosed(Guid productId)
        {
            var resourceIds = new Dictionary<string, string>
            {
                [nameof(productId)] = productId.ToString()
            };

            Response = new OkObjectResult(DefaultResponse.Create("Project successfully closed.", resourceIds));
        }

        public void ProductDoesNotExist(Guid productId)
        {
            var resourceIds = new Dictionary<string, string>
            {
                [nameof(productId)] = productId.ToString()
            };

            Response = new NotFoundObjectResult(DefaultResponse.Create("Product not found.", resourceIds));
        }

        public void ProductClosed(Guid productId)
        {
            var resourceIds = new Dictionary<string, string>
            {
                [nameof(productId)] = productId.ToString()
            };

            Response = new OkObjectResult(DefaultResponse.Create("Project successfully closed.", resourceIds));
        }
    }
}