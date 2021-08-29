using System;
using System.Collections.Generic;
using ChangeBlog.Api.DTOs;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Application.DataAccess;
using ChangeBlog.Application.UseCases.Commands.Issues.DeleteChangeLogLineIssue;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Api.Presenters.V1.ChangeLogs
{
    public class DeleteChangeLogLineIssueApiPresenter : BaseApiPresenter, IDeleteChangeLogLineIssueOutputPort
    {
        public void Removed(Guid changeLogLineId)
        {
            var resourceIds = new Dictionary<string, string>
            {
                [KnownIdentifiers.ChangeLogLineId] = changeLogLineId.ToString()
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
