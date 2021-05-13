using System;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Api.Extensions;
using ChangeTracker.Application.UseCases.Commands.AddProject;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Presenters.v1.Project
{
    public class AddProjectApiPresenter : IAddProjectOutputPort
    {
        private readonly HttpContext _httpContext;

        public AddProjectApiPresenter(HttpContext httpContext)
        {
            _httpContext = httpContext;
        }

        public ActionResult Response { get; private set; } = new StatusCodeResult(500);

        public void AccountDoesNotExist()
        {
            Response = new NotFoundObjectResult(DefaultResponse.Create("Account does not exist"));
        }

        public void AccountDeleted(Guid accountId)
        {
            Response = new StatusCodeResult(410);
        }

        public void InvalidName(string name)
        {
            Response = new BadRequestObjectResult(DefaultResponse.Create($"Invalid name {name}"));
        }

        public void ProjectAlreadyExists()
        {
            Response = new ConflictObjectResult(DefaultResponse.Create("Project already exists"));
        }

        public void VersioningSchemeDoesNotExist()
        {
            Response = new NotFoundObjectResult(DefaultResponse.Create("VersioningScheme does not exist"));
        }

        public void Conflict(string reason)
        {
            Response = new ConflictObjectResult(DefaultResponse.Create(reason));
        }

        public void Created(Guid accountId, Guid projectId)
        {
            var location = _httpContext.CreateLinkTo($"api/v1/projects/{projectId}");
            Response = new CreatedResult(location, DefaultResponse.Create($"Project with id {projectId} added"));
        }
    }
}