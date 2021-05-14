using System;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Application.UseCases.Commands.CloseProject;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Presenters.v1.Project
{
    public class CloseProjectApiPresenter : BasePresenter, ICloseProjectOutputPort
    {
        public void ProjectAlreadyClosed(Guid projectId)
        {
            Response = new OkObjectResult(DefaultResponse.Create($"Project with Id {projectId} closed."));
        }

        public void ProjectDoesNotExist()
        {
            Response = new NotFoundObjectResult(DefaultResponse.Create("Project not found."));
        }

        public void ProjectClosed(Guid projectId)
        {
            Response = new OkObjectResult(DefaultResponse.Create($"Project with Id {projectId} closed."));
        }

        public void Conflict(string reason)
        {
            Response = new ConflictObjectResult(DefaultResponse.Create(reason));
        }
    }
}