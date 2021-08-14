using System;
using System.Collections.Generic;
using ChangeBlog.Api.DTOs;
using ChangeBlog.Application.DataAccess;
using ChangeBlog.Application.UseCases.Commands.DeleteChangeLogLine;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Api.Presenters.V1.ChangeLogs
{
    public class DeleteChangeLogLineApiPresenter : BaseApiPresenter, IDeleteChangeLogLineOutputPort
    {
        public void LineDoesNotExist(Guid changeLogLineId)
        {
            var resourceIds = new Dictionary<string, string>
            {
                [nameof(changeLogLineId)] = changeLogLineId.ToString()
            };

            Response = new NotFoundObjectResult(DefaultResponse.Create("The requested ChangeLogLine does not exist.",
                resourceIds));
        }

        public void LineDeleted(Guid changeLogLineId)
        {
            var resourceIds = new Dictionary<string, string>
            {
                [nameof(changeLogLineId)] = changeLogLineId.ToString()
            };

            Response = new OkObjectResult(DefaultResponse.Create("Line successfully deleted.", resourceIds));
        }

        public void Conflict(Conflict conflict)
        {
            Response = conflict.ToResponse();
        }

        public void RequestedLineIsNotPending(Guid changeLogLineId)
        {
            var resourceIds = new Dictionary<string, string>
            {
                [nameof(changeLogLineId)] = changeLogLineId.ToString()
            };

            Response = new ConflictObjectResult(DefaultResponse.Create("The requested change log line is not pending.",
                resourceIds));
        }

        public void RequestedLineIsPending(Guid changeLogLineId)
        {
            var resourceIds = new Dictionary<string, string>
            {
                [nameof(changeLogLineId)] = changeLogLineId.ToString()
            };

            Response = new ConflictObjectResult(DefaultResponse.Create("The requested change log line is pending.",
                resourceIds));
        }
    }
}
