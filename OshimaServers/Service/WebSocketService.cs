using System.Data;
using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Controller;
using Milimoe.FunGame.Core.Interface.Addons;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaServers.Service
{
    public class WebSocketService(AnonymousServer server)
    {
        public AnonymousServer ServerInstance { get; set; } = server;
        public ServerAddonController<IGameModuleServer> Controller => ServerInstance.Controller;

        public string SCAdd(Dictionary<string, object> data)
        {
            string result = "";

            using SQLHelper? sql = Controller.GetSQLHelper();
            if (sql != null)
            {
                try
                {
                    long qq = Controller.JSON.GetObject<long>(data, "qq");
                    long groupid = Controller.JSON.GetObject<long>(data, "groupid");
                    double sc = Controller.JSON.GetObject<double>(data, "sc");
                    sql.NewTransaction();
                    sql.Script = "select * from saints where qq = @qq and `group` = @group";
                    sql.Parameters.Add("qq", qq);
                    sql.Parameters.Add("group", groupid);
                    sql.ExecuteDataSet();
                    string content = Controller.JSON.GetObject<string>(data, "content") ?? "";
                    string record = "";
                    if (sql.Success)
                    {
                        record = Convert.ToString(sql.DataSet.Tables[0].Rows[0]["record"]) ?? "";
                    }
                    record = $"{DateTime.Now:MM/dd HH:mm}：{content}（{(sc < 0 ? "-" : "+") + Math.Abs(sc)}）\r\n{record}";
                    record = string.Join("\r\n", record.Split("\r\n", StringSplitOptions.RemoveEmptyEntries).Take(10));
                    if (sql.Success)
                    {
                        sql.Script = "update saints set sc = sc + @sc, record = @record where qq = @qq and `group` = @group";
                    }
                    else
                    {
                        sql.Script = "insert into saints(qq, sc, `group`, record) values(@qq, @sc, @group, @record)";
                    }
                    sql.Parameters.Add("sc", sc);
                    sql.Parameters.Add("qq", qq);
                    sql.Parameters.Add("group", groupid);
                    sql.Parameters.Add("record", record);
                    sql.Execute();
                    if (sql.Success)
                    {
                        Controller.WriteLine($"用户 {qq} 的圣人点数增加了 {sc}", LogLevel.Debug);
                        sql.Commit();
                    }
                    else
                    {
                        sql.Rollback();
                    }
                }
                catch (Exception e)
                {
                    result = e.ToString();
                    sql.Rollback();
                }
            }
            else result = "无法调用此接口：SQL 服务不可用。";

            return result;
        }

        public string SCList(Dictionary<string, object> data)
        {
            string result;

            SQLHelper? sql = Controller.SQLHelper;
            if (sql != null)
            {
                long userQQ = Controller.JSON.GetObject<long>(data, "qq");
                (bool userHas, double userSC, int userTop, string userRemark) = (false, 0, 0, "");
                long groupid = Controller.JSON.GetObject<long>(data, "groupid");
                bool reverse = Controller.JSON.GetObject<bool>(data, "reverse");
                if (!reverse)
                {
                    result = $"☆--- OSMTV 圣人排行榜 TOP10 ---☆\r\n统计时间：{DateTime.Now.ToString(General.GeneralDateTimeFormatChinese)}\r\n";
                }
                else
                {
                    result = $"☆--- OSMTV 出生排行榜 TOP10 ---☆\r\n统计时间：{DateTime.Now.ToString(General.GeneralDateTimeFormatChinese)}\r\n";
                }
                sql.Script = "select * from saints where `group` = @group order by sc" + (!reverse ? " desc" : "");
                sql.Parameters.Add("group", groupid);
                sql.ExecuteDataSet();
                if (sql.Success && sql.DataSet.Tables.Count > 0)
                {
                    int count = 0;
                    foreach (DataRow dr in sql.DataSet.Tables[0].Rows)
                    {
                        count++;
                        long qq = Convert.ToInt64(dr["qq"]);
                        double sc = Convert.ToDouble(dr["sc"]);
                        string remark = Convert.ToString(dr["remark"]) ?? "";
                        if (reverse)
                        {
                            sc = -sc;
                            remark = remark.Replace("+", "-");
                        }
                        if (qq == userQQ)
                        {
                            userHas = true;
                            userSC = sc;
                            userTop = count;
                            userRemark = remark;
                        }
                        if (count > 10) continue;
                        if (!reverse)
                        {
                            result += $"{count}. 用户：{qq}，圣人点数：{sc} 分{(remark.Trim() != "" ? $" ({remark})" : "")}\r\n";
                        }
                        else
                        {
                            result += $"{count}. 用户：{qq}，出生点数：{sc} 分{(remark.Trim() != "" ? $" ({remark})" : "")}\r\n";
                        }
                    }
                    if (!reverse && userHas)
                    {
                        result += $"你的圣人点数为：{userSC} 分{(userRemark.Trim() != "" ? $"（{userRemark}）" : "")}，排在第 {userTop} / {sql.DataSet.Tables[0].Rows.Count} 名。\r\n" +
                            $"本排行榜仅供娱乐，不代表任何官方立场或真实情况。";
                    }
                    if (reverse && userHas)
                    {
                        result += $"你的出生点数为：{userSC} 分{(userRemark.Trim() != "" ? $"（{userRemark}）" : "")}，排在出生榜第 {userTop} / {sql.DataSet.Tables[0].Rows.Count} 名。\r\n" +
                            $"本排行榜仅供娱乐，不代表任何官方立场或真实情况。";
                    }
                }
                else
                {
                    if (reverse)
                    {
                        result = "出生榜目前没有任何数据。";
                    }
                    else
                    {
                        result = "圣人榜目前没有任何数据。";
                    }
                }
            }
            else result = "无法调用此接口：SQL 服务不可用。";

            return result.Trim();
        }

        public string SCList_Backup(Dictionary<string, object> data)
        {
            string result;

            SQLHelper? sql = Controller.SQLHelper;
            if (sql != null)
            {
                long userQQ = Controller.JSON.GetObject<long>(data, "qq");
                (bool userHas, double userSC, int userTop, string userRemark) = (false, 0, 0, "");
                long groupid = Controller.JSON.GetObject<long>(data, "groupid");
                bool reverse = Controller.JSON.GetObject<bool>(data, "reverse");
                if (!reverse)
                {
                    result = $"☆--- OSMTV 圣人排行榜 TOP10 ---☆\r\n该榜单为上赛季封榜记录\r\n";
                }
                else
                {
                    result = $"☆--- OSMTV 出生排行榜 TOP10 ---☆\r\n该榜单为上赛季封榜记录\r\n";
                }
                sql.Script = "select * from saints_backup where `group` = @group order by sc" + (!reverse ? " desc" : "");
                sql.Parameters.Add("group", groupid);
                sql.ExecuteDataSet();
                if (sql.Success && sql.DataSet.Tables.Count > 0)
                {
                    int count = 0;
                    foreach (DataRow dr in sql.DataSet.Tables[0].Rows)
                    {
                        count++;
                        long qq = Convert.ToInt64(dr["qq"]);
                        double sc = Convert.ToDouble(dr["sc"]);
                        string remark = Convert.ToString(dr["remark"]) ?? "";
                        if (reverse)
                        {
                            sc = -sc;
                            remark = remark.Replace("+", "-");
                        }
                        if (qq == userQQ)
                        {
                            userHas = true;
                            userSC = sc;
                            userTop = count;
                            userRemark = remark;
                        }
                        if (count > 10) continue;
                        if (!reverse)
                        {
                            result += $"{count}. 用户：{qq}，圣人点数：{sc} 分{(remark.Trim() != "" ? $" ({remark})" : "")}\r\n";
                        }
                        else
                        {
                            result += $"{count}. 用户：{qq}，出生点数：{sc} 分{(remark.Trim() != "" ? $" ({remark})" : "")}\r\n";
                        }
                    }
                    if (!reverse && userHas)
                    {
                        result += $"你的上赛季圣人点数为：{userSC} 分{(userRemark.Trim() != "" ? $"（{userRemark}）" : "")}，排在第 {userTop} / {sql.DataSet.Tables[0].Rows.Count} 名。\r\n" +
                            $"本排行榜仅供娱乐，不代表任何官方立场或真实情况。";
                    }
                    if (reverse && userHas)
                    {
                        result += $"你的上赛季出生点数为：{userSC} 分{(userRemark.Trim() != "" ? $"（{userRemark}）" : "")}，排在出生榜第 {userTop} / {sql.DataSet.Tables[0].Rows.Count} 名。\r\n" +
                            $"本排行榜仅供娱乐，不代表任何官方立场或真实情况。";
                    }
                }
                else
                {
                    if (reverse)
                    {
                        result = "出生榜目前没有任何备份数据。";
                    }
                    else
                    {
                        result = "圣人榜目前没有任何备份数据。";
                    }
                }
            }
            else result = "无法调用此接口：SQL 服务不可用。";

            return result.Trim();
        }

        public string SCRecord(Dictionary<string, object> data)
        {
            string result = "";

            SQLHelper? sql = Controller.SQLHelper;
            if (sql != null)
            {
                long userQQ = Controller.JSON.GetObject<long>(data, "qq");
                long groupid = Controller.JSON.GetObject<long>(data, "groupid");
                result = $"☆--- 圣人点数信息 ---☆\r\n统计时间：{DateTime.Now.ToString(General.GeneralDateTimeFormatChinese)}\r\n";
                sql.Script = "select * from saints where `group` = @group order by sc desc";
                sql.Parameters.Add("group", groupid);
                sql.Parameters.Add("qq", userQQ);
                sql.ExecuteDataSet();
                if (sql.Success && sql.DataSet.Tables.Count > 0)
                {
                    Dictionary<int, DataRow> dict = sql.DataSet.Tables[0].AsEnumerable().Select((r, i) => new { Index = i + 1, Row = r }).ToDictionary(c => c.Index, c => c.Row);
                    int index = dict.Where(kv => Convert.ToInt64(kv.Value["qq"]) == userQQ).Select(r => r.Key).FirstOrDefault();
                    if (index != 0 && dict.TryGetValue(index, out DataRow? dr) && dr != null)
                    {
                        long qq = Convert.ToInt64(dr["qq"]);
                        double sc = Convert.ToDouble(dr["sc"]);
                        string remark = Convert.ToString(dr["remark"]) ?? "";
                        string record = Convert.ToString(dr["record"]) ?? "";
                        result += $"用户：{qq}，圣人点数：{sc} 分{(remark.Trim() != "" ? $" ({remark})" : "")}，排在圣人榜第 {index} / {sql.DataSet.Tables[0].Rows.Count} 名。\r\n" +
                            $"{(record != "" ? "显示近期点数变动信息：\r\n" + record + "\r\n" : "")}本系统仅供娱乐，不代表任何官方立场或真实情况。";
                    }
                    else
                    {
                        result = "你在这个群没有任何历史记录。";
                    }
                }
                else
                {
                    result = "你在这个群没有任何历史记录。";
                }
            }
            else result = "无法调用此接口：SQL 服务不可用。";

            return result.Trim();
        }
    }
}
