using System;
using System.Net.Mime;
using System.Threading.Tasks;
using ChangeTracker.Api.Authorization;
using ChangeTracker.Api.DTOs.V1.ChangeLog;
using ChangeTracker.Api.SwaggerUI;
using ChangeTracker.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/changeLogLines")]
    [Produces(MediaTypeNames.Application.Json)]
    [SwaggerControllerOrder(5)]
    public class ChangeLogLinesController : ControllerBase
    {
        [HttpGet("{changeLogLineId:Guid}")]
        [NeedsPermission(Permission.ViewChangeLogLines)]
        public async Task<ActionResult<ChangeLogLineDto>> GetChangeLogLineAsync(
            Guid changeLogLineId)
        {
            await Task.Yield();

            return Ok(changeLogLineId);
        }

        [HttpGet("{productId:Guid}/pending")]
        [NeedsPermission(Permission.ViewChangeLogLines)]
        public async Task<ActionResult<ChangeLogLineDto>> GetPendingChangeLogLinesAsync(
            Guid productId)
        {
            await Task.Yield();

            return Ok(productId);
        }
    }
}