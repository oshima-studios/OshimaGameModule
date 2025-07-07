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
                return "��������ȷ��OpenID��QQ��";
            }

            if (QQOpenID.QQAndOpenID.TryGetValue(b.Openid, out long bindqq) && bindqq != 0)
            {
                return $"���Ѿ��󶨹���{bindqq}����󶨴�������ϵ�ͷ�����";
            }

            if (QQOpenID.QQAndOpenID.Values.Any(qq => qq == b.QQ && b.Openid != b.Openid))
            {
                return $"��QQ {b.QQ} �ѱ������˰󶨣�������Ǵ�QQ�����ˣ�����ϵ�ͷ�����";
            }

            if (QQOpenID.QQAndOpenID.TryAdd(b.Openid, b.QQ))
            {
                QQOpenID.SaveConfig();
            }
            else
            {
                return $"��ʧ�ܣ����Ժ����ԣ��������ʧ������ϵ�ͷ�����";
            }

            return "�󶨳ɹ��������Ҫ����󶨣��뷢�͡����+QQ�š����磺���123456789����";
        }

        [HttpPost("unbind")]
        public string Unbind([FromBody] BindQQ b)
        {
            if (QQOpenID.QQAndOpenID.TryGetValue(b.Openid, out long bindqq) && bindqq == b.QQ && QQOpenID.QQAndOpenID.Remove(b.Openid))
            {
                return $"���ɹ���";
            }

            return "���ʧ�ܣ�û�в鵽�󶨵���Ϣ���ߴ��˺��ѱ������˰󶨣�������Ǵ�QQ�����ˣ�����ϵ�ͷ�����";
        }
    }
}
