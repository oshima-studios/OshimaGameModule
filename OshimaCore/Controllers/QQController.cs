using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Milimoe.FunGame.Core.Api.Utility;
using Oshima.Core.Configs;
using Oshima.Core.Models;

namespace Oshima.Core.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QQController(ILogger<QQController> logger) : ControllerBase
    {
        private readonly ILogger<QQController> _logger = logger;

        [HttpPost("bind")]
        public string Post([FromBody] BindQQ b)
        {
            if (QQOpenID.QQAndOpenID.Values.Any(qq => qq == b.QQ))
            {
                return NetworkUtility.JsonSerialize($"此QQ已被其他人绑定，如果你是此QQ的主人，请联系客服处理。");
            }

            if (QQOpenID.QQAndOpenID.TryGetValue(b.Openid, out long bindqq) && bindqq != 0)
            {
                return NetworkUtility.JsonSerialize($"你已经绑定过：{bindqq}，如绑定错误请联系客服处理。");
            }

            if (QQOpenID.QQAndOpenID.TryAdd(b.Openid, b.QQ))
            {
                QQOpenID.SaveConfig();
            }
            else
            {
                return NetworkUtility.JsonSerialize($"绑定失败，请稍后再试！如持续绑定失败请联系客服处理。");
            }

            return NetworkUtility.JsonSerialize("绑定成功！");
        }
    }
}
