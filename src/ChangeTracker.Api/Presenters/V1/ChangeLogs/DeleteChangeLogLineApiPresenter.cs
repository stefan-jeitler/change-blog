using System;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Application.UseCases.Commands.DeleteChangeLogLine;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Presenters.V1.ChangeLogs
{
    public class DeleteChangeLogLineApiPresenter : BaseApiPresenter, IDeleteChangeLogLineOutputPort
    {
        public void LineDoesNotExist(Guid changeLogLineId)
        {
            Response = new NotFoundObjectResult(DefaultResponse.Create("The requested ChangeLogLine does not exist.", changeLogLineId));
        }

        public void LineDeleted(Guid lineId)
        {
            Response = new OkObjectResult(DefaultResponse.Create("Line successfully deleted.", lineId));
        }

        public void Conflict(string reason)
        {
            Response = new ConflictObjectResult(DefaultResponse.Create(reason));
        }
    }
}