using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    [Route("api/v1/products/{productId:Guid}")]
    [Produces(MediaTypeNames.Application.Json)]
    [SwaggerControllerOrder(5)]
    public class PendingChangeLogsController : ControllerBase
    {
        [HttpGet("pending-changelogs")]
        [NeedsPermission(Permission.ViewPendingChangeLogLines)]
        public async Task<ActionResult<List<ChangeLogLineDto>>> GetPendingChangeLogLineAsync(Guid productId)
        {
            await Task.Yield();

            return Ok(productId);
        }

        [HttpPut("pending-changelogs")]
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

        [HttpPost("pending-changelogs/move")]
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

        [HttpDelete("pending-changelogs")]
        [NeedsPermission(Permission.DeleteChangeLogLine)]
        public async Task<ActionResult> DeleteAllPendingChangeLogLineAsync(Guid productId)
        {
            await Task.Yield();

            return Ok(productId);
        }
    }
}