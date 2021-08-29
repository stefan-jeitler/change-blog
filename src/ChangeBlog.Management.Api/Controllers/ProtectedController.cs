using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Management.Api.Controllers
{
    [ApiController]
    [Route("api/protected")]
    [Produces(MediaTypeNames.Application.Json)]
    public class ProtectedController : ControllerBase
    {
        [HttpGet]
        public string GetProtectedString() => "Protected-String";
    }
}