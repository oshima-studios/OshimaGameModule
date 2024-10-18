using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Milimoe.FunGame.Core.Api.Utility;
using Oshima.Core.Configs;
using Oshima.Core.Models;
using Oshima.Core.Utils;

namespace Oshima.Core.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserDailyController(ILogger<UserDailyController> logger) : ControllerBase
    {
        private readonly ILogger<UserDailyController> _logger = logger;

        [HttpPost("get/{user_id}", Name = "GetUserDaily")]
        public UserDaily Get(long user_id)
        {
            return UserDailyUtil.GetUserDaily(user_id);
        }

        [HttpGet("view/{user_id}", Name = "ViewUserDaily")]
        public UserDaily View(long user_id)
        {
            return UserDailyUtil.ViewUserDaily(user_id);
        }

        [HttpPost("open/{open_id}", Name = "GetOpenUserDaily")]
        public UserDaily Open(string open_id)
        {
            if (QQOpenID.QQAndOpenID.TryGetValue(open_id, out long qq) && qq != 0)
            {
                return UserDailyUtil.GetUserDaily(qq);
            }
            return new(0, 0, "你似乎没有绑定QQ呢，请先发送【绑定+QQ号】（如：绑定123456789）再使用哦！");
        }

        [HttpPost("remove/{user_id}", Name = "RemoveUserDaily")]
        public string Remove(long user_id)
        {
            return UserDailyUtil.RemoveDaily(user_id);
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
            return NetworkUtility.JsonSerialize(img);
        }
    }
}
