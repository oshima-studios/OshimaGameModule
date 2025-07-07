using System.Globalization;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Core.Library.SQLScript.Common;
using Oshima.Core.Configs;
using Oshima.FunGame.WebAPI.Constant;
using TaskScheduler = Milimoe.FunGame.Core.Api.Utility.TaskScheduler;

namespace Oshima.FunGame.WebAPI.Controllers
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

        [HttpGet("gethmacsha256")]
        public string UseHMACSHA256(string msg, string key)
        {
            return Encryption.HmacSha256(msg, key);
        }

        [HttpGet("getrsa")]
        public string GetRSA(string msg, string key)
        {
            return Encryption.RSADecrypt(msg, key);
        }

        [HttpGet("setrsa")]
        public string SetRSA(string msg, string key)
        {
            return Encryption.RSAEncrypt(msg, key);
        }

        /// <summary>
        /// 1: public, 2: private
        /// </summary>
        /// <returns></returns>
        [HttpGet("getxml")]
        public string[] GetRSAXMLString()
        {
            using RSACryptoServiceProvider rsa = new();
            return [rsa.ToXmlString(false), rsa.ToXmlString(true)];
        }

        [HttpGet("getlastlogintime")]
        public string GetLastLoginTime()
        {
            if (Statics.RunningPlugin != null)
            {
                try
                {
                    SQLHelper? sql = Statics.RunningPlugin.Controller.SQLHelper;
                    if (sql != null)
                    {
                        sql.ExecuteDataSet(ServerLoginLogs.Select_GetLastLoginTime());
                        if (sql.Success && DateTime.TryParse(sql.DataSet.Tables[0].Rows[0][ServerLoginLogs.Column_LastTime].ToString(), out DateTime date))
                        {
                            string month = date.ToString("MMM", CultureInfo.InvariantCulture);
                            int day = date.Day;
                            string time = date.ToString("HH:mm:ss", CultureInfo.InvariantCulture);

                            string msg = "服务器最后启动时间：" + $"{month}. {day}, {date.Year} {time}";
                            return msg;
                        }
                    }
                }
                catch (Exception e)
                {
                    Statics.RunningPlugin.Controller.Error(e);
                    return "无法调用此接口。原因：\r\n" + e.GetErrorInfo();
                }
            }
            return "无法调用此接口。原因：与 SQL 服务器通信失败。";
        }

        [HttpGet("gettask")]
        public string GetTaskScheduler(string name)
        {
            return TaskScheduler.Shared.GetRunTimeInfo(name);
        }

        [HttpGet("sendtest")]
        public string SendTest(long key, string to)
        {
            if (Statics.RunningPlugin != null && key == GeneralSettings.Master)
            {
                try
                {
                    MailSender? sender = Statics.RunningPlugin.Controller.MailSender;
                    if (sender != null && sender.Send(new(sender, "Test Mail", "Hello!", to)) == Milimoe.FunGame.Core.Library.Constant.MailSendResult.Success)
                    {
                        return "发送成功。";
                    }
                    return "发送失败。";
                }
                catch (Exception e)
                {
                    Statics.RunningPlugin.Controller.Error(e);
                    return "无法调用此接口。原因：\r\n" + e.GetErrorInfo();
                }
            }
            return "无法调用此接口。";
        }
    }
}
