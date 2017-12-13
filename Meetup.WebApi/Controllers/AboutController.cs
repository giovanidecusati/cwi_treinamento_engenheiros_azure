using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;

namespace Meetup.WebApi.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class AboutController : ControllerBase
    {
        static string ApiVersion;

        static AboutController()
        {
            ApiVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        [HttpGet]
        [Route("Version")]
        public IActionResult GetVersion()
        {
            return Ok(ApiVersion);
        }

        [HttpGet]
        [Route("ServerName")]
        public IActionResult GetServerName()
        {
            return Ok(Environment.MachineName);
        }
    }
}
