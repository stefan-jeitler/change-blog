using System;
using System.Threading.Tasks;
using ChangeTracker.Api.Authorization;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Api.DTOs.v1.Project;
using ChangeTracker.Api.Extensions;
using ChangeTracker.Api.Presenters.v1.Project;
using ChangeTracker.Application.UseCases.Commands.AddProject;
using ChangeTracker.Application.UseCases.Commands.CloseProject;
using ChangeTracker.Application.UseCases.Queries.GetProjects;
using ChangeTracker.Domain;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/projects")]
    public class ProjectsController : ControllerBase
    {
        private readonly IAddProject _addProject;
        private readonly ICloseProject _closeProject;
        private readonly IGetProjects _getProjects;

        public ProjectsController(IAddProject addProject, ICloseProject closeProject, IGetProjects getProjects)
        {
            _addProject = addProject;
            _closeProject = closeProject;
            _getProjects = getProjects;
        }

        [HttpGet("{projectId:Guid}")]
        [NeedsPermission(Permission.ViewProjects)]
        public async Task<ActionResult> GetProjectAsync(Guid projectId)
        {
            var project = await _getProjects.ExecuteAsync(HttpContext.GetUserId(), projectId);

            if (project.HasNoValue)
            {
                return NotFound(DefaultResponse.Create("Project not found"));
            }

            return Ok(ProjectDto.FromResponseModel(project.Value));
        }

        [HttpPost]
        [NeedsPermission(Permission.AddProject)]
        public async Task<ActionResult> CreateProjectAsync([FromBody] AddProjectDto addProjectDto)
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

        [HttpPut("{projectId:Guid}/close")]
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