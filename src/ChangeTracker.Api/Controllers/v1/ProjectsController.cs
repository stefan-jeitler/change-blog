using System;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Api.Authorization;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Api.DTOs.v1.Project;
using ChangeTracker.Api.Extensions;
using ChangeTracker.Api.Presenters.v1.Project;
using ChangeTracker.Application.UseCases;
using ChangeTracker.Application.UseCases.Commands.AddProject;
using ChangeTracker.Application.UseCases.Commands.CloseProject;
using ChangeTracker.Application.UseCases.Queries.GetProjects;
using Microsoft.AspNetCore.Mvc;
using ProjectRequestModel = ChangeTracker.Application.UseCases.Commands.AddProject.ProjectRequestModel;

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

        [HttpGet]
        [NeedsPermission(Permission.ViewProjects)]
        public async Task<ActionResult> GetProjectsAsync(Guid? accountId = null,
            ushort count = ProjectsQueryRequestModel.MaxChunkCount, 
            Guid? lastProjectId = null)
        {
            var requestModel = new ProjectsQueryRequestModel(HttpContext.GetUserId(),
                accountId,
                lastProjectId,
                count
            );

            var projects = await _getProjects.ExecuteAsync(requestModel);

            return Ok(projects.Select(ProjectDto.FromResponseModel));
        }

        [HttpPost]
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