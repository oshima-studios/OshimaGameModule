using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Oshima.Core.Configs;
using Oshima.FunGame.WebAPI.Models;

namespace Oshima.FunGame.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QQController(ILogger<QQController> logger) : ControllerBase
    {
        private readonly ILogger<QQController> _logger = logger;

        [HttpPost("bind")]
        public string Bind([FromBody] BindQQ b)
        {
            if (b.Openid.Trim() == "" || b.QQ <= 0)
            {
                return "请输入正确的OpenID和QQ！";
            }

            if (QQOpenID.QQAndOpenID.TryGetValue(b.Openid, out long bindqq) && bindqq != 0)
            {
                return $"你已经绑定过：{bindqq}，如绑定错误请联系客服处理。";
            }

            if (QQOpenID.QQAndOpenID.Values.Any(qq => qq == b.QQ && b.Openid != b.Openid))
            {
                return $"此QQ {b.QQ} 已被其他人绑定，如果你是此QQ的主人，请联系客服处理。";
            }

            if (QQOpenID.QQAndOpenID.TryAdd(b.Openid, b.QQ))
            {
                QQOpenID.SaveConfig();
            }
            else
            {
                return $"绑定失败，请稍后再试！如持续绑定失败请联系客服处理。";
            }

            return "绑定成功！如果需要解除绑定，请发送【解绑+QQ号】（如：解绑123456789）！";
        }

        [HttpPost("unbind")]
        public string Unbind([FromBody] BindQQ b)
        {
            if (QQOpenID.QQAndOpenID.TryGetValue(b.Openid, out long bindqq) && bindqq == b.QQ && QQOpenID.QQAndOpenID.Remove(b.Openid))
            {
                return $"解绑成功！";
            }

            return "解绑失败！没有查到绑定的信息或者此账号已被其他人绑定，如果你是此QQ的主人，请联系客服处理。";
        }
    }
}
