using System;
using System.Collections.Generic;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Application.UseCases.Queries.GetPendingChangeLogLine;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Presenters.V1.ChangeLogs
{
    public class GetPendingChangeLogLineApiPresenter : BaseApiPresenter, IGetPendingChangeLogLineOutputPort
    {
        public void LineDoesNotExists(Guid changeLogLineId)
        {
            var resourceIds = new Dictionary<string, string>
            {
                [nameof(changeLogLineId)] = changeLogLineId.ToString()
            };

            Response = new NotFoundObjectResult(DefaultResponse.Create("ChangeLogLine not found.", resourceIds));
        }

        public void LineIsNotPending(Guid changeLogLineId)
        {
            var resourceIds = new Dictionary<string, string>
            {
                [nameof(changeLogLineId)] = changeLogLineId.ToString()
            };

            Response = new NotFoundObjectResult(DefaultResponse.Create("The requested change log line is not pending.", resourceIds));
        }

        public void LineFound(PendingChangeLogLineResponseModel responseModel)
        {
            Response = new OkObjectResult(responseModel);
        }
    }
}