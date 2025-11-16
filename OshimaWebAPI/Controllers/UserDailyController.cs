using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Oshima.Core.Configs;
using Oshima.FunGame.WebAPI.Models;
using Oshima.FunGame.WebAPI.Services;

namespace Oshima.FunGame.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserDailyController(ILogger<UserDailyController> logger) : ControllerBase
    {
        private readonly ILogger<UserDailyController> _logger = logger;

        [HttpPost("get/{user_id}", Name = "GetUserDaily")]
        public UserDaily Get(long user_id)
        {
            return UserDailyService.GetUserDaily(user_id);
        }

        [HttpGet("view/{user_id}", Name = "ViewUserDaily")]
        public UserDaily View(long user_id)
        {
            return UserDailyService.ViewUserDaily(user_id);
        }

        [HttpPost("open/{open_id}", Name = "GetOpenUserDaily")]
        public OpenUserDaily Open(string open_id)
        {
            if (QQOpenID.QQAndOpenID.TryGetValue(open_id, out long qq) && qq != 0)
            {
                UserDaily daily = UserDailyService.GetUserDaily(qq);
                return new(open_id, daily.type, daily.daily);
            }
            else
            {
                return UserDailyService.GetOpenUserDaily(open_id);
            }
        }

        [HttpPost("remove/{user_id}", Name = "RemoveUserDaily")]
        public string Remove(long user_id)
        {
            return UserDailyService.RemoveDaily(user_id);
        }

        [HttpGet("img/{type}", Name = "GetTypeImage")]
        public string GetTypeImage(int type)
        {
            string img = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/images/zi/";
            img += type switch
            {
                1 => "dj" + (Random.Shared.Next(3) + 1) + ".png",
                2 => "zj" + (Random.Shared.Next(2) + 1) + ".png",
                3 => "j" + (Random.Shared.Next(4) + 1) + ".png",
                4 => "mj" + (Random.Shared.Next(2) + 1) + ".png",
                5 => "x" + (Random.Shared.Next(2) + 1) + ".png",
                6 => "dx" + (Random.Shared.Next(2) + 1) + ".png",
                _ => ""
            };
            return img;
        }
    }
}
