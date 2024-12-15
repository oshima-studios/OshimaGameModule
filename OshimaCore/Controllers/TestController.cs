using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Core.Library.SQLScript.Common;
using Oshima.Core.Constant;
using TaskScheduler = Milimoe.FunGame.Core.Api.Utility.TaskScheduler;

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
                            return NetworkUtility.JsonSerialize(msg);
                        }
                    }
                }
                catch (Exception e)
                {
                    Statics.RunningPlugin.Controller?.Error(e);
                    return NetworkUtility.JsonSerialize("无法调用此接口。原因：\r\n" + e.GetErrorInfo());
                }
            }
            return NetworkUtility.JsonSerialize("无法调用此接口。原因：与 SQL 服务器通信失败。");
        }

        [HttpGet("gettask")]
        public string GetTaskScheduler(string name)
        {
            return NetworkUtility.JsonSerialize(TaskScheduler.Shared.GetRunTimeInfo(name));
        }
    }
}
