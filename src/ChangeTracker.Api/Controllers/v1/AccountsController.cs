using System;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Api.Authorization;
using ChangeTracker.Api.DTOs.v1.Project;
using ChangeTracker.Api.Extensions;
using ChangeTracker.Application.UseCases.Queries.GetProjects;
using ChangeTracker.Domain;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly IGetProjects _getProjects;

        public AccountsController(IGetProjects getProjects)
        {
            _getProjects = getProjects;
        }

        [HttpGet("{accountId:Guid}/projects")]
        [NeedsPermission(Permission.ViewProjects)]
        public async Task<ActionResult> GetProjectAsync(Guid accountId,
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
    }
}