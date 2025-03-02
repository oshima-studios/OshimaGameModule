using System.Globalization;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Milimoe.FunGame.Core.Api.Utility;
using Oshima.FunGame.WebAPI.Models;
using Oshima.FunGame.WebAPI.Services;
using Rebex.Security.Cryptography;

namespace Oshima.FunGame.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QQBotController(IOptions<BotConfig> botConfig, ILogger<QQBotController> logger, QQBotService service, RainBOTService fungameService) : ControllerBase
    {
        private BotConfig BotConfig { get; set; } = botConfig.Value;
        private ILogger<QQBotController> Logger { get; set; } = logger;
        private QQBotService Service { get; set; } = service;
        private RainBOTService FungameService { get; set; } = fungameService;

        [HttpPost]
        public IActionResult Post([FromBody] Payload? payload)
        {
            if (payload is null)
            {
                return BadRequest("Payload 格式无效");
            }

            Logger.LogDebug("收到 Webhook 请求：{payload.Op}", payload.Op);

            try
            {
                if (payload.Op == 13)
                {
                    return HandleValidation(payload);
                }
                else if (payload.Op == 0)
                {
                    // 处理其他事件
                    return HandleEventAsync(payload);
                }
                else
                {
                    Logger.LogWarning("未处理操作码：{payload.Op}", payload.Op);
                    return Ok();
                }
            }
            catch (Exception e)
            {
                Logger.LogError("Error: {e}", e);
                return StatusCode(500, "服务器内部错误");
            }
        }

        private IActionResult HandleValidation(Payload payload)
        {
            ValidationRequest? validationPayload = JsonSerializer.Deserialize<ValidationRequest>(payload.Data.ToString() ?? "");
            if (validationPayload is null)
            {
                Logger.LogError("反序列化验证 Payload 失败");
                return BadRequest("无效的验证 Payload 格式");
            }
            string seed = BotConfig.Secret;
            while (seed.Length < 32)
            {
                seed += seed;
            }
            seed = seed[..32];

            byte[] privateKeyBytes = Encoding.UTF8.GetBytes(seed);

            Ed25519 ed25519 = new();

            ed25519.FromSeed(privateKeyBytes);

            // 将你的消息转换为 byte[]
            byte[] message = Encoding.UTF8.GetBytes(validationPayload.EventTs + validationPayload.PlainToken);

            // 使用 Sign 方法签名消息
            byte[] result = ed25519.SignMessage(message);

            string signature = Convert.ToHexString(result).ToLower(CultureInfo.InvariantCulture);


            ValidationResponse response = new()
            {
                PlainToken = validationPayload.PlainToken,
                Signature = signature
            };
            string responseJson = JsonSerializer.Serialize(response);
            Logger.LogDebug("验证相应：{responseJson}", responseJson);
            return Ok(response);
        }

        private IActionResult HandleEventAsync(Payload payload)
        {
            Logger.LogDebug("处理事件：{EventType}, 数据：{Data}", payload.EventType, payload.Data);

            try
            {
                switch (payload.EventType)
                {
                    case "C2C_MESSAGE_CREATE":
                        C2CMessage? c2cMessage = JsonSerializer.Deserialize<C2CMessage>(payload.Data.ToString() ?? "");
                        if (c2cMessage != null)
                        {
                            c2cMessage.Detail = c2cMessage.Detail.Trim();
                            if (c2cMessage.Detail.StartsWith('/'))
                            {
                                c2cMessage.Detail = c2cMessage.Detail[1..];
                            }
                            // TODO
                            Logger.LogInformation("收到来自用户 {c2cMessage.Author.UserOpenId} 的消息：{c2cMessage.Content}", c2cMessage.Author.UserOpenId, c2cMessage.Content);
                            //// 上传图片
                            //string url = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/images/zi/dj1.png";
                            //var (fileUuid, fileInfo, ttl, error) = await _service.UploadC2CMediaAsync(c2cMessage.Author.UserOpenId, 1, url);
                            //_logger.LogDebug("发送的图片地址：{url}", url);
                            //if (string.IsNullOrEmpty(error))
                            //{
                            //    // 回复消息
                            //    await _service.SendC2CMessageAsync(c2cMessage.Author.UserOpenId, $"你发送的消息是：{c2cMessage.Content}", msgId: c2cMessage.Id);
                            //    // 回复富媒体消息
                            //    await _service.SendC2CMessageAsync(c2cMessage.Author.UserOpenId, "", msgType: 7, media: new { file_info = fileInfo }, msgId: c2cMessage.Id);
                            //}
                            //else
                            //{
                            //    _logger.LogError("上传图片失败：{error}", error);
                            //}
                            TaskUtility.NewTask(async () => await FungameService.Handler(c2cMessage));
                        }
                        else
                        {
                            Logger.LogError("反序列化 C2C 消息数据失败");
                            return BadRequest("无效的 C2C 消息数据格式");
                        }
                        break;
                    case "GROUP_AT_MESSAGE_CREATE":
                        GroupAtMessage? groupAtMessage = JsonSerializer.Deserialize<GroupAtMessage>(payload.Data.ToString() ?? "");
                        if (groupAtMessage != null)
                        {
                            groupAtMessage.Detail = groupAtMessage.Detail.Trim();
                            if (groupAtMessage.Detail.StartsWith('/'))
                            {
                                groupAtMessage.Detail = groupAtMessage.Detail[1..];
                            }
                            // TODO
                            Logger.LogInformation("收到来自群组 {groupAtMessage.GroupOpenId} 的消息：{groupAtMessage.Content}", groupAtMessage.GroupOpenId, groupAtMessage.Content);
                            // 回复消息
                            //await _service.SendGroupMessageAsync(groupAtMessage.GroupOpenId, $"你发送的消息是：{groupAtMessage.Content}", msgId: groupAtMessage.Id);
                            TaskUtility.NewTask(async () => await FungameService.Handler(groupAtMessage));
                        }
                        else
                        {
                            Logger.LogError("反序列化群聊消息数据失败");
                            return BadRequest("无效的群聊消息数据格式");
                        }
                        break;
                    default:
                        Logger.LogWarning("未定义事件：{EventType}", payload.EventType);
                        break;
                }
                return Ok();
            }
            catch (JsonException e)
            {
                Logger.LogError("反序列化过程遇到错误：{e}", e);
                return BadRequest("Invalid JSON format");
            }
            catch (Exception e)
            {
                Logger.LogError("Error: {e}", e);
                return StatusCode(500, "服务器内部错误");
            }
        }

        [Authorize(AuthenticationSchemes = "CustomBearer")]
        [HttpPost("thirdparty")]
        public async Task<IActionResult> ThirdParty([FromBody] ThirdPartyMessage? msg = null)
        {
            if (msg is null) return Ok("");

            bool result = await FungameService.Handler(msg);

            if (!result || msg.IsCompleted)
            {
                return Ok(msg.Result);
            }

            return Ok("");
        }
    }
}
