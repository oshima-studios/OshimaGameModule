using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Milimoe.FunGame.Core.Api.Utility;

namespace Oshima.Core.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController(ILogger<TestController> logger) : ControllerBase
    {
        private readonly ILogger<TestController> _logger = logger;

        [HttpGet("gethmacsha512")]
        public string UseHMACSHA512(string msg, string key)
        {
            return msg.Encrypt(key);
        }
    }
}
