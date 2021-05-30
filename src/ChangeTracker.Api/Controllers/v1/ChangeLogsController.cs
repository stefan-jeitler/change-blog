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
    [Route("api/v1/change-logs")]
    [Produces(MediaTypeNames.Application.Json)]
    [SwaggerControllerOrder(6)]
    public class ChangeLogsController : ControllerBase
    {
        [HttpGet("{changeLogLineId:Guid}")]
        [NeedsPermission(Permission.ViewChangeLogLines)]
        public async Task<ActionResult<List<ChangeLogLineDto>>> GetChangeLogLinesAsync(Guid changeLogLineId)
        {
            await Task.Yield();
            return Ok();
        }

        [HttpPut]
        [NeedsPermission(Permission.AddOrUpdateChangeLogLine)]
        public async Task<ActionResult<List<ChangeLogLineDto>>> AddOrUpdateChangeLogLinesAsync(Guid changeLogLineId,
            [FromBody] AddOrUpdateChangeLogLineDto changeLogLineDto)
        {
            await Task.Yield();
            return Ok();
        }

        [HttpDelete("{changeLogLineId:Guid}")]
        [NeedsPermission(Permission.DeleteChangeLogLine)]
        public async Task<ActionResult<List<ChangeLogLineDto>>> DeleteChangeLogLinesAsync(Guid changeLogLineId)
        {
            await Task.Yield();
            return Ok();
        }
    }
}