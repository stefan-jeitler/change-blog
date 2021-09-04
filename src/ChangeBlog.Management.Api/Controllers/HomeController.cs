using System;
using System.Net.Mime;
using System.Reflection;
using ChangeBlog.Api.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace ChangeBlog.Management.Api.Controllers
{
    [ApiController]
    [Route("api")]
    [Produces(MediaTypeNames.Application.Json)]
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

        private readonly IHostEnvironment _hostEnvironment;

        public HomeController(IHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
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
    }
}