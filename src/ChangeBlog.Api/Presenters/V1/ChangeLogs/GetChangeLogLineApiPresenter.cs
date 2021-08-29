using System;
using System.Collections.Generic;
using ChangeBlog.Api.DTOs;
using ChangeBlog.Api.DTOs.V1.ChangeLog;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Application.UseCases.Queries.GetChangeLogLine;
using ChangeBlog.Application.UseCases.Queries.SharedModels;
using ChangeBlog.Domain.ChangeLog;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Api.Presenters.V1.ChangeLogs
{
    public class GetChangeLogLineApiPresenter : BaseApiPresenter, IGetChangeLogLineOutputPort
    {
        public void LineDoesNotExists(Guid changeLogLineId)
        {
            var resourceIds = new Dictionary<string, string>
            {
                [KnownIdentifiers.ChangeLogLineId] = changeLogLineId.ToString()
            };

            Response = new NotFoundObjectResult(DefaultResponse.Create("ChangeLogLine not found.", resourceIds));
        }

        public void LineIsPending(Guid changeLogLineId)
        {
            var resourceIds = new Dictionary<string, string>
            {
                [KnownIdentifiers.ChangeLogLineId] = changeLogLineId.ToString()
            };

            Response = new NotFoundObjectResult(DefaultResponse.Create("The requested change log line is pending.",
                resourceIds));
        }

        public void LineFound(ChangeLogLineResponseModel responseModel)
        {
            Response = new OkObjectResult(ChangeLogLineDto.FromResponseModel(responseModel));
        }
    }
}
