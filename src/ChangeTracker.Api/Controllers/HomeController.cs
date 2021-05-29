using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Threading.Tasks;
using ChangeTracker.Api.Authorization;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Api.DTOs.V1.Version;
using ChangeTracker.Api.Extensions;
using ChangeTracker.Api.SwaggerUI;
using ChangeTracker.Application.Constants;
using ChangeTracker.Application.UseCases;
using ChangeTracker.Application.UseCases.Queries.GetCompleteVersions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

// ReSharper disable InconsistentNaming

namespace ChangeTracker.Api.Controllers
{
    [ApiController]
    [Route("api")]
    [Produces(MediaTypeNames.Application.Json)]
    [SwaggerControllerOrder(0)]
    public class HomeController : ControllerBase
    {
        private readonly IGetCompleteVersions _getCompleteVersions;

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

        private readonly IHostEnvironment _hostEnvironment;

        public HomeController(IHostEnvironment hostEnvironment, IGetCompleteVersions getCompleteVersions)
        {
            _hostEnvironment = hostEnvironment;
            _getCompleteVersions = getCompleteVersions;
        }

        [HttpGet("info")]
        [AllowAnonymous]
        public ActionResult Info()
        {
            var apiInfo = new ApiInfo(AssemblyName.Value,
                AssemblyVersion.Value,
                _hostEnvironment.EnvironmentName);

            return Ok(apiInfo);
        }

        [HttpGet("changeLogs")]
        [AllowAnonymous]
        public async Task<ActionResult<List<CompleteVersionDto>>> GetCompleteProductVersionsAsync(
            [MaxLength(VersionsQueryRequestModel.MaxSearchTermLength)]
            string searchTerm = null,
            Guid? lastVersionId = null,
            ushort limit = VersionsQueryRequestModel.MaxLimit)
        {
            var requestModel = new VersionsQueryRequestModel(AppChanges.ProductId,
                lastVersionId,
                AppChanges.UserId,
                searchTerm,
                limit);

            var versions = await _getCompleteVersions.ExecuteAsync(requestModel);

            return Ok(versions.Select(CompleteVersionDto.FromResponseModel));
        }
    }
}