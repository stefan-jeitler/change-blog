using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ChangeTracker.Api.Authorization;
using ChangeTracker.Api.DTOs;
using ChangeTracker.Application.UseCases;
using ChangeTracker.Application.UseCases.Queries.GetRoles;
using ChangeTracker.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

// ReSharper disable InconsistentNaming

namespace ChangeTracker.Api.Controllers
{
    [ApiController]
    [Route("api")]
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
        public ActionResult ChangeLogs() => Ok("coming soon ...");
    }
}