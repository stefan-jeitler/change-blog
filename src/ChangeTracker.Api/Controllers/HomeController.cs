using System;
using System.Reflection;
using ChangeTracker.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace ChangeTracker.Api.Controllers
{
    [ApiController]
    [Route("")]
    [ApiExplorerSettings(IgnoreApi = true)]
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

        [HttpGet("")]
        [AllowAnonymous]
        public ActionResult Index()
        {
            var swagger = CreateLinkTo("swagger");
            var changeLog = CreateLinkTo("changeLog");

            var apiInfo = new ApiInfo(AssemblyName.Value,
                AssemblyVersion.Value,
                _hostEnvironment.EnvironmentName,
                new[]
                {
                    swagger.AbsoluteUri,
                    changeLog.AbsoluteUri
                });

            return Ok(apiInfo);
        }

        [HttpGet("changeLog")]
        public ActionResult ChangeLog() => Ok("coming soon ...");

        private Uri CreateLinkTo(string relativePath)
        {
            var request = HttpContext.Request;
            var scheme = _hostEnvironment.IsDevelopment() ? request.Scheme : "https";

            var uriBuilder = new UriBuilder
            {
                Scheme = scheme,
                Host = request.Host.Host,
                Path = relativePath,
                Port = request.Host.Port ?? -1
            };

            return uriBuilder.Uri;
        }
    }
}