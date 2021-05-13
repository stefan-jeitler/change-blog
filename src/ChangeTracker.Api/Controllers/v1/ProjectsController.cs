using System;
using System.Threading.Tasks;
using ChangeTracker.Api.Authorization;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Api.DTOs.v1.Project;
using ChangeTracker.Api.Extensions;
using ChangeTracker.Api.Presenters.v1.Project;
using ChangeTracker.Application.UseCases;
using ChangeTracker.Application.UseCases.Commands.AddProject;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/projects")]
    public class ProjectsController : ControllerBase
    {
        private readonly IAddProject _addProject;

        public ProjectsController(IAddProject addProject)
        {
            _addProject = addProject;
        }

        [HttpPost]
        [NeedsPermission(Permission.AddProject)]
        public async Task<ActionResult> AddProjectAsync([FromBody] AddProjectDto addProjectDto)
        {
            if (addProjectDto.Name is null)
            {
                return BadRequest(DefaultResponse.Create("Missing name"));
            }

            if (addProjectDto.AccountId == Guid.Empty)
            {
                return BadRequest(DefaultResponse.Create("Missing AccountId"));
            }

            if (addProjectDto.VersioningSchemeId == Guid.Empty)
            {
                return BadRequest(DefaultResponse.Create("VersioningSchemeId cannot be empty."));
            }

            var presenter = new AddProjectApiPresenter(HttpContext);
            var requestModel = new ProjectRequestModel(addProjectDto.AccountId, addProjectDto.Name,
                addProjectDto.VersioningSchemeId, HttpContext.GetUserId());

            await _addProject.ExecuteAsync(presenter, requestModel);

            return presenter.Response;
        }
    }
}