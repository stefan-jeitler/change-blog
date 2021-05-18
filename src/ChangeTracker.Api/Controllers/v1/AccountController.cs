using System;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Api.Authorization;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Api.DTOs.v1.Account;
using ChangeTracker.Api.DTOs.v1.Project;
using ChangeTracker.Api.Extensions;
using ChangeTracker.Application.UseCases;
using ChangeTracker.Application.UseCases.Queries.GetAccounts;
using ChangeTracker.Application.UseCases.Queries.GetProjects;
using ChangeTracker.Application.UseCases.Queries.GetRoles;
using ChangeTracker.Application.UseCases.Queries.GetUsers;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/accounts")]
    public class AccountController : ControllerBase
    {
        private readonly IGetAccounts _getAccounts;
        private readonly IGetProjects _getProjects;
        private readonly IGetUsers _getUsers;

        public AccountController(IGetProjects getProjects, IGetAccounts getAccounts, IGetUsers getUsers)
        {
            _getProjects = getProjects;
            _getAccounts = getAccounts;
            _getUsers = getUsers;
        }

        [HttpGet]
        [NeedsPermission(Permission.ViewAccount)]
        public async Task<ActionResult> GetAccountsAsync()
        {
            var userId = HttpContext.GetUserId();
            var accounts = await _getAccounts.ExecuteAsync(userId);

            return Ok(accounts.Select(AccountDto.FromResponseModel));
        }

        [HttpGet("{accountId:Guid}")]
        [NeedsPermission(Permission.ViewAccount)]
        public async Task<ActionResult> GetAccountAsync(Guid accountId)
        {
            var userId = HttpContext.GetUserId();
            var account = await _getAccounts.ExecuteAsync(userId, accountId);

            return Ok(AccountDto.FromResponseModel(account));
        }

        [HttpGet("{accountId:Guid}/users")]
        [NeedsPermission(Permission.ViewUsers)]
        public async Task<ActionResult> GetUsersAsync(Guid accountId,
            ushort limit = UsersQueryRequestModel.MaxChunkCount,
            Guid? lastUserId = null)
        {
            var requestModel = new UsersQueryRequestModel(HttpContext.GetUserId(),
                accountId,
                lastUserId,
                limit
            );

            var projects = await _getUsers.ExecuteAsync(requestModel);

            return Ok(projects.Select(UserDto.FromResponseModel));
        }

        [HttpGet("{accountId:Guid}/projects")]
        [NeedsPermission(Permission.ViewAccountProjects)]
        public async Task<ActionResult> GetProjectsAsync(Guid accountId,
            ushort limit = ProjectsQueryRequestModel.MaxLimit,
            Guid? lastProjectId = null,
            bool includeClosedProjects = false)
        {
            var requestModel = new ProjectsQueryRequestModel(HttpContext.GetUserId(),
                accountId,
                lastProjectId,
                limit,
                includeClosedProjects
            );

            var projects = await _getProjects.ExecuteAsync(requestModel);

            return Ok(projects.Select(ProjectDto.FromResponseModel));
        }

        [HttpGet("roles")]
        [NeedsPermission(Permission.ViewRoles)]
        public async Task<ActionResult> GetRolesAsync([FromServices] IGetRoles getRoles,
            string role = null,
            bool includePermissions = false)
        {
            var roles = await getRoles.ExecuteAsync(role);

            return Ok(roles.Select(x => RoleDto.FromResponseModel(x, includePermissions)));
        }
    }
}