using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Api.DTOs.V1.Version;
using ChangeTracker.Application.UseCases.Queries.GetLatestVersion;
using ChangeTracker.Application.UseCases.Queries.SharedModels;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Presenters.V1.Version
{
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
                [nameof(productId)] = productId.ToString()
            };

            Response = new NotFoundObjectResult(DefaultResponse.Create("Product does not contain any version.", resourceIds));
        }

        public void ProductDoesNotExist()
        {
            Response = new NotFoundObjectResult(DefaultResponse.Create("Product not found."));
        }
    }
}
