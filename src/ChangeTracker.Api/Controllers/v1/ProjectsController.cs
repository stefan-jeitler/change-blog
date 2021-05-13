using System;
using System.Threading.Tasks;
using ChangeTracker.Api.Authorization;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Api.DTOs.v1.Project;
using ChangeTracker.Api.Extensions;
using ChangeTracker.Api.Presenters.v1.Project;
using ChangeTracker.Application.UseCases;
using ChangeTracker.Application.UseCases.Commands.AddProject;
using ChangeTracker.Application.UseCases.Commands.CloseProject;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1")]
    public class ProjectsController : ControllerBase
    {
        private readonly IAddProject _addProject;
        private readonly ICloseProject _closeProject;

        public ProjectsController(IAddProject addProject, ICloseProject closeProject)
        {
            _addProject = addProject;
            _closeProject = closeProject;
        }

        [HttpGet("accounts/{accountId:Guid}")]
        [NeedsPermission(Permission.ViewProjects)]
        public async Task<ActionResult> GetProjectsAsync(Guid accountId)
        {
            await Task.Yield();
            return Ok("");
        }

        [HttpPost("projects")]
        [NeedsPermission(Permission.AddProject)]
        public async Task<ActionResult> AddProjectAsync([FromBody] AddProjectDto addProjectDto)
        {
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

        [HttpPut("{projects/projectId:Guid}/close")]
        [NeedsPermission(Permission.CloseProject)]
        public async Task<ActionResult> CloseProjectAsync(Guid projectId)
        {
            if (projectId == Guid.Empty)
                return BadRequest(DefaultResponse.Create("Missing projectId."));

            var presenter = new CloseProjectApiPresenter();
            await _closeProject.ExecuteAsync(presenter, projectId);

            return presenter.Response;
        }
    }
}