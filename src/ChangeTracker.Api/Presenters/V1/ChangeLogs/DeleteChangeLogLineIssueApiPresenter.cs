using System;
using System.Collections.Generic;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.UseCases.Commands.Issues.DeleteChangeLogLineIssue;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Presenters.V1.ChangeLogs
{
    public class DeleteChangeLogLineIssueApiPresenter : BaseApiPresenter, IDeleteChangeLogLineIssueOutputPort
    {
        public void Removed(Guid changeLogLineId)
        {
            var resourceIds = new Dictionary<string, string>
            {
                [nameof(changeLogLineId)] = changeLogLineId.ToString()
            };

            Response = new OkObjectResult(DefaultResponse.Create("Issue successfully deleted.", resourceIds));
        }

        public void Conflict(Conflict conflict)
        {
            Response = conflict.ToResponse();
        }

        public void ChangeLogLineDoesNotExist()
        {
            Response = new NotFoundObjectResult(DefaultResponse.Create("ChangeLogLine not found."));
        }

        public void InvalidIssue(string issue)
        {
            Response = new BadRequestObjectResult(
                DefaultResponse.Create($"Invalid issue '{issue}'."));
        }
    }
}