using System.Data;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Exception;
using Oshima.FunGame.OshimaServers.Service;
using Oshima.FunGame.WebAPI.Constant;

namespace Oshima.FunGame.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OshimaController(SQLHelper sql) : ControllerBase
    {
        [HttpGet("saints")]
        public Dictionary<string, object> GetSaints(long group, bool reverse)
        {
            Dictionary<string, object> dict = [];
            dict["msg"] = "";
            if (Statics.RunningPlugin != null)
            {
                try
                {
                    sql.Script = "select qq UID, '-' as Times, SC, Remark, Record from saints where `group` = @group order by sc" + (!reverse ? " desc" : "");
                    sql.Parameters.Add("group", group);
                    sql.ExecuteDataSet();
                    if (sql.Success)
                    {
                        List<Dictionary<string, object>> data = Utility.DataSetConverter.ConvertFirstTableToDictionary(sql.DataSet);
                        dict["data"] = data;
                        return dict;
                    }
                }
                catch (Exception e)
                {
                    Statics.RunningPlugin.Controller.Error(e);
                    dict["msg"] = "无法调用此接口。原因：\r\n" + e.GetErrorInfo();
                    return dict;
                }
            }
            dict["msg"] = "无法调用此接口。原因：与 SQL 服务器通信失败。";
            return dict;
        }

        [HttpGet("ratings")]
        public Dictionary<string, object> GetRatings()
        {
            Dictionary<string, object> dict = [];
            List<Dictionary<string, object>> data = [];

            IEnumerable<Character> ratings = FunGameSimulation.TeamCharacterStatistics.OrderByDescending(kv => kv.Value.Rating).Select(kv => kv.Key);
            foreach (Character character in ratings)
            {
                Dictionary<string, object> table = [];
                StringBuilder builder = new();
                CharacterStatistics stats = FunGameSimulation.TeamCharacterStatistics[character];
                table["Character"] = character.ToStringWithOutUser();
                table["Maps"] = stats.Plays;
                table["Wins"] = stats.Wins;
                table["Winrate"] = $"{stats.Winrate * 100:0.##}%";
                table["Rating"] = $"{stats.Rating:0.0#}";
                table["MVPs"] = stats.MVPs;
                data.Add(table);
            }

            dict["data"] = data;
            return dict;
        }
    }
}
