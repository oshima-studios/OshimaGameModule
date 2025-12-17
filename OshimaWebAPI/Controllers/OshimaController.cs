using System.Data;
using Microsoft.AspNetCore.Mvc;
using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Library.Exception;
using Oshima.FunGame.WebAPI.Constant;

namespace Oshima.FunGame.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OshimaController : ControllerBase
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
                    SQLHelper? sql = Statics.RunningPlugin.Controller.SQLHelper;
                    if (sql != null)
                    {
                        sql.Script = "select * from saints where `group` = @group order by sc" + (!reverse ? " desc" : "");
                        sql.Parameters.Add("group", group);
                        sql.ExecuteDataSet();
                        if (sql.Success)
                        {
                            List<Dictionary<string, object>> data = DataSetConverter.ConvertFirstTableToDictionary(sql.DataSet);
                            dict["data"] = data;
                            return dict;
                        }
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

        public static class DataSetConverter
        {
            /// <summary>
            /// 将DataSet转换为Dictionary列表
            /// </summary>
            /// <param name="dataSet">输入的DataSet</param>
            /// <returns>Dictionary列表，每个Dictionary代表一行数据</returns>
            public static List<Dictionary<string, object>> ConvertDataSetToDictionary(DataSet dataSet)
            {
                List<Dictionary<string, object>> result = [];

                if (dataSet == null || dataSet.Tables.Count == 0)
                    return result;

                foreach (DataTable table in dataSet.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        Dictionary<string, object> rowDict = [];

                        foreach (DataColumn column in table.Columns)
                        {
                            // 处理DBNull值
                            if (row[column] != DBNull.Value)
                            {
                                rowDict[column.ColumnName] = row[column];
                            }
                        }

                        result.Add(rowDict);
                    }
                }

                return result;
            }

            /// <summary>
            /// 将DataSet的第一张表转换为Dictionary列表
            /// </summary>
            /// <param name="dataSet">输入的DataSet</param>
            /// <returns>Dictionary列表，每个Dictionary代表一行数据</returns>
            public static List<Dictionary<string, object>> ConvertFirstTableToDictionary(DataSet dataSet)
            {
                List<Dictionary<string, object>> result = [];

                if (dataSet == null || dataSet.Tables.Count == 0)
                    return result;

                DataTable table = dataSet.Tables[0];

                foreach (DataRow row in table.Rows)
                {
                    Dictionary<string, object> rowDict = [];

                    foreach (DataColumn column in table.Columns)
                    {
                        // 处理DBNull值
                        if (row[column] != DBNull.Value)
                        {
                            rowDict[column.ColumnName] = row[column];
                        }
                    }

                    result.Add(rowDict);
                }

                return result;
            }
        }
    }
}
