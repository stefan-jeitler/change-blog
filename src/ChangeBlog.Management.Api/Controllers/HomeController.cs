﻿using System;
using System.Net.Mime;
using System.Reflection;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.Swagger;
using ChangeBlog.Management.Api.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ChangeBlog.Management.Api.Controllers;

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

    private readonly IConfiguration _configuration;

    private readonly IHostEnvironment _hostEnvironment;

    public HomeController(IHostEnvironment hostEnvironment, IConfiguration configuration)
    {
        _hostEnvironment = hostEnvironment;
        _configuration = configuration;
    }


    [HttpGet("info", Name = "GetAppInfo")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [AllowAnonymous]
    public ActionResult<ApiInfo> Info()
    {
        var apiInfo = new ApiInfo(AssemblyName.Value,
            AssemblyVersion.Value,
            _hostEnvironment.EnvironmentName);

        return Ok(apiInfo);
    }

    [HttpGet("appsettings", Name = "GetAppSettings")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [AllowAnonymous]
    public ActionResult AppSettings()
    {
        var appSettings = _configuration
            .GetSection(nameof(Configuration.ClientAppSettings))
            .Get<ClientAppSettings>();

        return Ok(appSettings);
    }
}