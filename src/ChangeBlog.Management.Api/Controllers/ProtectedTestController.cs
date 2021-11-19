﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading.Tasks;
using ChangeBlog.Management.Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

namespace ChangeBlog.Management.Api.Controllers;

[ApiController]
[Route("api/protected")]
[Produces(MediaTypeNames.Application.Json)]
public class ProtectedTestController : ControllerBase
{
    private readonly ITokenAcquisition _tokenAcquisition;

    public ProtectedTestController(ITokenAcquisition tokenAcquisition)
    {
        _tokenAcquisition = tokenAcquisition;
    }

    [HttpGet]
    public string GetProtectedString() => "Protected-String";

}