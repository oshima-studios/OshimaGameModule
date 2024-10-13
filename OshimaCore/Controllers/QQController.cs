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
                return NetworkUtility.JsonSerialize($"��QQ�ѱ������˰󶨣�������Ǵ�QQ�����ˣ�����ϵ�ͷ�����");
            }

            if (QQOpenID.QQAndOpenID.TryGetValue(b.Openid, out long bindqq) && bindqq != 0)
            {
                return NetworkUtility.JsonSerialize($"���Ѿ��󶨹���{bindqq}����󶨴�������ϵ�ͷ�����");
            }

            if (QQOpenID.QQAndOpenID.TryAdd(b.Openid, b.QQ))
            {
                QQOpenID.SaveConfig();
            }
            else
            {
                return NetworkUtility.JsonSerialize($"��ʧ�ܣ����Ժ����ԣ��������ʧ������ϵ�ͷ�����");
            }

            return NetworkUtility.JsonSerialize("�󶨳ɹ���");
        }
    }
}
