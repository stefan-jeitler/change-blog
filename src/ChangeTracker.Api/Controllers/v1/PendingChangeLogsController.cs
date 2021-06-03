using System;
using System.Collections.Generic;
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
    [Route("api/v1")]
    [Produces(MediaTypeNames.Application.Json)]
    [SwaggerControllerOrder(5)]
    public class PendingChangeLogsController : ControllerBase
    {
        [HttpGet("products/{productId:Guid}/pending-changelogs")]
        [NeedsPermission(Permission.ViewPendingChangeLogLines)]
        public async Task<ActionResult<List<ChangeLogLineDto>>> GetPendingChangeLogsAsync(Guid productId)
        {
            await Task.Yield();

            return Ok(productId);
        }

        [HttpGet("pending-changelogs/{changeLogLineId:Guid}")]
        [NeedsPermission(Permission.ViewPendingChangeLogLines)]
        public async Task<ActionResult<List<ChangeLogLineDto>>> GetPendingChangeLogLineAsync(Guid productId)
        {
            await Task.Yield();

            return Ok(productId);
        }

        [HttpPut("products/{productId:Guid}/pending-changelogs")]
        [NeedsPermission(Permission.AddOrUpdateChangeLogLine)]
        public async Task<ActionResult> AddOrUpdatePendingChangeLogLineAsync(Guid productId,
            [FromBody] AddOrUpdateChangeLogLineDto changeLogLineDto)
        {
            await Task.Yield();

            return Ok(productId);
        }

        [HttpPost("pending-changelogs/{changeLogLineId:Guid}/move")]
        [NeedsPermission(Permission.MoveChangeLogLines)]
        public async Task<ActionResult> MovePendingChangeLogLineAsync(Guid productId,
            Guid changeLogLineId,
            [FromBody] MoveChangeLogLineDto moveChangeLogLineDto)
        {
            await Task.Yield();

            return Ok(productId);
        }

        [HttpPost("products/{productId:Guid}/pending-changelogs/move")]
        [NeedsPermission(Permission.MoveChangeLogLines)]
        public async Task<ActionResult> MoveAllPendingChangeLogLineAsync(Guid productId,
            [FromBody] MoveChangeLogLineDto moveChangeLogLineDto)
        {
            await Task.Yield();

            return Ok(productId);
        }

        [HttpDelete("pending-changelogs/{changeLogLineId:Guid}")]
        [NeedsPermission(Permission.DeleteChangeLogLine)]
        public async Task<ActionResult> DeletePendingChangeLogLineAsync(Guid productId,
            Guid changeLogLineId)
        {
            await Task.Yield();

            return Ok(productId);
        }

        [HttpDelete("products/{productId:Guid}/pending-changelogs")]
        [NeedsPermission(Permission.DeleteChangeLogLine)]
        public async Task<ActionResult> DeleteAllPendingChangeLogLineAsync(Guid productId)
        {
            await Task.Yield();

            return Ok(productId);
        }
    }
}