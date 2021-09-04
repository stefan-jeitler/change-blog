using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Threading.Tasks;
using ChangeBlog.Api.DTOs;
using ChangeBlog.Api.DTOs.V1.Version;
using ChangeBlog.Api.Shared;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.SwaggerUI;
using ChangeBlog.Application.UseCases.Queries.GetVersions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

// ReSharper disable InconsistentNaming

namespace ChangeBlog.Api.Controllers
{
    [ApiController]
    [Route("api")]
    [Produces(MediaTypeNames.Application.Json)]
    [SwaggerControllerOrder(0)]
    public class HomeController : ControllerBase
    {
        private static readonly Lazy<string> AssemblyVersion =
            new(() =>
            {
                var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
                var assemblyVersionAttribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

                return assemblyVersionAttribute is null
                    ? assembly.GetName().Version?.ToString()
                    : assemblyVersionAttribute.InformationalVersion;
            });

        private static readonly Lazy<string> AssemblyName =
            new(() => Assembly.GetEntryAssembly()?.GetName().Name);

        private readonly IGetVersions _getVersions;

        private readonly IHostEnvironment _hostEnvironment;

        public HomeController(IHostEnvironment hostEnvironment, IGetVersions getVersions)
        {
            _hostEnvironment = hostEnvironment;
            _getVersions = getVersions;
        }

        [HttpGet("info")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public ActionResult<ApiInfo> Info()
        {
            var apiInfo = new ApiInfo(AssemblyName.Value,
                AssemblyVersion.Value,
                _hostEnvironment.EnvironmentName);

            return Ok(apiInfo);
        }

        [HttpGet("changes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<ActionResult<List<VersionDto>>> GetChangesAsync(
            [MaxLength(VersionsQueryRequestModel.MaxSearchTermLength)]
            string searchTerm = null,
            Guid? lastVersionId = null,
            [Range(1, VersionsQueryRequestModel.MaxLimit)]
            ushort limit = VersionsQueryRequestModel.MaxLimit)
        {
            var requestModel = new VersionsQueryRequestModel(Application.Constants.ChangeBlog.ProductId,
                lastVersionId,
                Application.Constants.ChangeBlog.UserId,
                searchTerm,
                limit);

            var versions = await _getVersions.ExecuteAsync(requestModel);

            return Ok(versions.Select(VersionDto.FromResponseModel));
        }
    }
}
