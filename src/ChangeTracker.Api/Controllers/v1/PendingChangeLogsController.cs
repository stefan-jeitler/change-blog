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
    [Route("api/v1/products/{productId:Guid}")]
    [Produces(MediaTypeNames.Application.Json)]
    [SwaggerControllerOrder(4)]
    public class PendingChangeLogsController : ControllerBase
    {
        [HttpGet("pending")]
        [NeedsPermission(Permission.ViewPendingChangeLogLines)]
        public async Task<ActionResult<List<ChangeLogLineDto>>> GetPendingChangeLogLineAsync(Guid productId)
        {
            await Task.Yield();

            return Ok(productId);
        }
    }
}