using System;
using System.Collections.Generic;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Application.UseCases.Queries.GetChangeLogLine;
using ChangeTracker.Application.UseCases.Queries.SharedModels;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Presenters.V1.ChangeLogs
{
    public class GetChangeLogLineApiPresenter : BaseApiPresenter, IGetChangeLogLineOutputPort
    {
        public void LineDoesNotExists(Guid changeLogLineId)
        {
            var resourceIds = new Dictionary<string, string>
            {
                [nameof(changeLogLineId)] = changeLogLineId.ToString()
            };

            Response = new NotFoundObjectResult(DefaultResponse.Create("ChangeLogLine not found.", resourceIds));
        }

        public void LineIsPending(Guid changeLogLineId)
        {
            var resourceIds = new Dictionary<string, string>
            {
                [nameof(changeLogLineId)] = changeLogLineId.ToString()
            };

            Response = new NotFoundObjectResult(DefaultResponse.Create("The requested change log line is pending.", resourceIds));
        }

        public void LineFound(ChangeLogLineResponseModel responseModel)
        {
            Response = new OkObjectResult(responseModel);
        }
    }
}