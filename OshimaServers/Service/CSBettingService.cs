using System.Data;
using System.Text;
using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaServers.Model;

namespace Oshima.FunGame.WebAPI.Services
{
    public class CSBettingService
    {
        public static (string, int) GetEventsOverview(int page, int pageSize)
        {
            using SQLHelper? sql = Factory.OpenFactory.GetSQLHelper();
            if (sql == null) return ("数据库连接失败。", 0);

            UpdateStatuses(sql);

            // 获取总数
            sql.ExecuteDataSet("SELECT COUNT(*) FROM csbetting_events");
            int total = sql.Success ? Convert.ToInt32(sql.DataSet.Tables[0].Rows[0][0]) : 0;
            int totalPages = (int)Math.Ceiling(total / (double)pageSize);
            if (page > totalPages) page = totalPages;
            if (page < 1) page = 1;
            if (total == 0)
                return ("暂无赛事。", 1);

            int offset = (page - 1) * pageSize;
            sql.Parameters["@page_size"] = pageSize;
            sql.Parameters["@offset"] = offset;
            sql.ExecuteDataSet($@"
                SELECT id, name, status, start_time, end_time
                FROM csbetting_events
                ORDER BY 
                    CASE status WHEN 1 THEN 0 WHEN 0 THEN 1 ELSE 2 END,
                    CASE WHEN status = 2 THEN end_time END DESC,
                    CASE WHEN status != 2 THEN start_time END ASC
                LIMIT {pageSize} OFFSET {offset}");

            if (!sql.Success || sql.DataSet.Tables.Count == 0)
                return ("暂无赛事。", 1);

            StringBuilder sb = new();
            sb.AppendLine($"🏆 赛事列表{(totalPages > 1 ? $"（第 {page}/{totalPages} 页）" : "")}");
            foreach (DataRow row in sql.DataSet.Tables[0].Rows)
            {
                int id = Convert.ToInt32(row["id"]);
                string name = row["name"].ToString() ?? "";
                int status = Convert.ToInt32(row["status"]);
                string statusStr = status switch { 0 => "未开始", 1 => "进行中", 2 => "已结束", _ => "未知" };
                DateTime startTime = Convert.ToDateTime(row["start_time"]);
                DateTime endTime = Convert.ToDateTime(row["end_time"]);
                sb.AppendLine($"🏆 [{id}] {name.CreateCmdInput($"赛事详情 {id}")} ({statusStr}：{startTime:yyyy/MM/dd} ~ {endTime:yyyy/MM/dd})");
            }
            return (sb.ToString().TrimEnd(), totalPages);
        }

        public static (string, int) GetEventDetail(int eventId, int page, int pageSize)
        {
            using SQLHelper? sql = Factory.OpenFactory.GetSQLHelper();
            if (sql == null) return ("数据库连接失败。", 0);

            UpdateStatuses(sql);

            // 获取赛事信息
            sql.Parameters["@id"] = eventId;
            sql.ExecuteDataSet("SELECT * FROM csbetting_events WHERE id = @id");
            if (!sql.Success || sql.DataSet.Tables[0].Rows.Count == 0)
                return ("赛事不存在。", 0);
            DataRow evt = sql.DataSet.Tables[0].Rows[0];
            string name = evt["name"].ToString() ?? "";
            int status = Convert.ToInt32(evt["status"]);
            DateTime start = Convert.ToDateTime(evt["start_time"]);
            DateTime end = Convert.ToDateTime(evt["end_time"]);
            string statusStr = status switch { 0 => "未开始", 1 => "进行中", 2 => "已结束", _ => "未知" };

            StringBuilder header = new();
            header.AppendLine($"赛事：{name}");
            header.AppendLine($"状态：{statusStr}");
            header.AppendLine($"时间：{start:yyyy/MM/dd} ~ {end:yyyy/MM/dd}");

            // 比赛总数
            sql.Parameters["@eid"] = eventId;
            sql.ExecuteDataSet("SELECT COUNT(*) FROM csbetting_matches WHERE event_id = @eid");
            int total = sql.Success ? Convert.ToInt32(sql.DataSet.Tables[0].Rows[0][0]) : 0;
            int totalPages = (int)Math.Ceiling(total / (double)pageSize);
            if (page > totalPages) page = totalPages;
            if (page < 1) page = 1;

            header.AppendLine($"比赛列表{(totalPages > 1 ? $"（第 {page}/{totalPages} 页）" : "")}：");

            // 分页查询比赛
            int offset = (page - 1) * pageSize;
            sql.Parameters["@eid"] = eventId;
            sql.ExecuteDataSet($@"
                SELECT id, team1_name, team2_name, status, bet_deadline, stage, start_time
                FROM csbetting_matches
                WHERE event_id = @eid
                ORDER BY 
                    CASE status WHEN 0 THEN 0 WHEN 1 THEN 1 ELSE 2 END,
                    CASE WHEN status = 2 THEN start_time END DESC,
                    CASE WHEN status != 2 THEN start_time END ASC
                LIMIT {pageSize} OFFSET {offset}");

            StringBuilder matches = new();
            if (sql.Success && sql.DataSet?.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in sql.DataSet.Tables[0].Rows)
                {
                    int mid = Convert.ToInt32(row["id"]);
                    string t1 = row["team1_name"].ToString() ?? "";
                    string t2 = row["team2_name"].ToString() ?? "";
                    int mstatus = Convert.ToInt32(row["status"]);
                    DateTime deadline = Convert.ToDateTime(row["bet_deadline"]);
                    string stage = row["stage"].ToString() ?? "";
                    string mStatusStr = mstatus switch { 0 => "未开始", 1 => "进行中", 2 => "已结束", _ => "未知" };
                    string matchLabel = $"{t1} vs {t2}";
                    string clickableMatch = matchLabel.CreateCmdInput($"比赛详情 {mid}");
                    matches.AppendLine($"  [{mid}] {(stage != "" ? $"{stage} " : "")} {clickableMatch} (状态：{mStatusStr}, 截止：{deadline:MM-dd HH:mm})");
                }
            }
            else
            {
                matches.AppendLine("暂无比赛。");
            }

            return (header.ToString() + matches.ToString(), totalPages);
        }

        /// <summary>
        /// 获取所有比赛列表（赛程），按开始时间排序，支持分页
        /// </summary>
        public static (string, int) GetAllMatches(int page, int pageSize)
        {
            using SQLHelper? sql = Factory.OpenFactory.GetSQLHelper();
            if (sql == null) return ("数据库连接失败。", 0);

            UpdateStatuses(sql);

            // 总数
            sql.ExecuteDataSet("SELECT COUNT(*) FROM csbetting_matches");
            int total = sql.Success ? Convert.ToInt32(sql.DataSet.Tables[0].Rows[0][0]) : 0;
            int totalPages = (int)Math.Ceiling(total / (double)pageSize);
            if (page > totalPages) page = totalPages;
            if (page < 1) page = 1;
            if (total == 0)
                return ("暂无比赛赛程。", 1);

            int offset = (page - 1) * pageSize;
            sql.Parameters["@offset"] = offset;
            sql.Parameters["@limit"] = pageSize;

            sql.ExecuteDataSet($@"
                SELECT m.id, m.team1_name, m.team2_name, m.status, m.start_time, m.stage,
                       e.name AS event_name, e.id AS event_id
                FROM csbetting_matches m
                LEFT JOIN csbetting_events e ON m.event_id = e.id
                ORDER BY 
                    CASE m.status WHEN 1 THEN 0 WHEN 0 THEN 1 ELSE 2 END,
                    m.start_time ASC
                LIMIT @limit OFFSET @offset");

            if (!sql.Success || sql.DataSet.Tables.Count == 0 || sql.DataSet.Tables[0].Rows.Count == 0)
                return ("暂无比赛赛程。", 1);

            StringBuilder sb = new();
            sb.AppendLine($"📅 比赛赛程{(totalPages > 1 ? $"（第 {page}/{totalPages} 页）" : "")}");

            foreach (DataRow row in sql.DataSet.Tables[0].Rows)
            {
                int id = Convert.ToInt32(row["id"]);
                string t1 = row["team1_name"].ToString() ?? "";
                string t2 = row["team2_name"].ToString() ?? "";
                int status = Convert.ToInt32(row["status"]);
                DateTime start = Convert.ToDateTime(row["start_time"]);
                string stage = row["stage"]?.ToString() ?? "";
                string eventName = row["event_name"]?.ToString() ?? "";
                long eventId = Convert.ToInt64(row["event_id"]);

                string statusStr = status switch { 0 => "未开始", 1 => "进行中", 2 => "已结束", _ => "未知" };
                string matchLabel = $"{t1} vs {t2}".CreateCmdInput($"比赛详情 {id}");

                sb.Append($"[{id}] {matchLabel}");
                if (!string.IsNullOrWhiteSpace(eventName)) sb.Append($" ({eventName.CreateCmdInput($"赛事详情 {eventId}")}{(!string.IsNullOrWhiteSpace(stage) ? $" - {stage}" : "")})");
                sb.AppendLine($" | {statusStr} | {start:MM-dd HH:mm}");
            }

            return (sb.ToString().TrimEnd(), totalPages);
        }

        public static string GetMatchDetail(int matchId, out int status)
        {
            status = 0;
            using SQLHelper? sql = Factory.OpenFactory.GetSQLHelper();
            if (sql != null)
            {
                UpdateStatuses(sql);

                sql.Parameters["@mid"] = matchId;
                sql.ExecuteDataSet("SELECT * FROM csbetting_matches WHERE id = @mid");
                if (!sql.Success || sql.DataSet.Tables[0].Rows.Count == 0) return "比赛不存在。";
                DataRow row = sql.DataSet.Tables[0].Rows[0];
                long eventId = Convert.ToInt64(row["event_id"]);
                string t1 = row["team1_name"].ToString() ?? "";
                string t2 = row["team2_name"].ToString() ?? "";
                status = Convert.ToInt32(row["status"]);
                DateTime start = Convert.ToDateTime(row["start_time"]);
                DateTime deadline = Convert.ToDateTime(row["bet_deadline"]);
                string stage = row["stage"].ToString() ?? "";
                string available = row["available_options"]?.ToString() ?? "[]";
                string description = row["description"].ToString() ?? "";
                string result = row["result"] != DBNull.Value ? row["result"].ToString() ?? "" : "";
                long winner = row["winner"] != DBNull.Value ? Convert.ToInt64(row["winner"]) : 0;
                decimal team1Odds = Convert.ToDecimal(row["team1_win_odds"]);
                decimal team2Odds = Convert.ToDecimal(row["team2_win_odds"]);

                string eventName = "";
                sql.Parameters["@eid"] = eventId;
                DataRow? rowEvent = sql.ExecuteDataRow("SELECT name FROM csbetting_events WHERE id = @eid");
                if (rowEvent != null)
                {
                    eventName = rowEvent["name"].ToString() ?? "";
                }

                string statusStr = status switch { 0 => "未开始", 1 => "进行中", 2 => "已结束", _ => "未知" };
                StringBuilder sb = new();
                sb.AppendLine($"比赛 #{matchId}");
                if (eventName.Trim() != "")
                {
                    sb.AppendLine($"> {eventName.CreateCmdInput($"赛事详情 {eventId}")}");
                    if (stage.Trim() != "") sb.AppendLine($"{stage}");
                    sb.AppendLine();
                }
                sb.AppendLine($"{t1} vs {t2}".CreateCmdInput($"比赛详情 {matchId}"));
                sb.AppendLine($"开赛：{start:yyyy/MM/dd HH:mm}");
                sb.AppendLine($"竞猜截止：{deadline:yyyy/MM/dd HH:mm}");
                sb.AppendLine($"状态：{statusStr}");
                if (status == 0)
                {
                    sb.AppendLine($"可用选项：");
                    if (available.Contains("team1_win")) sb.AppendLine($"  - 队伍1 {t1} 胜 (x {team1Odds})");
                    if (available.Contains("team2_win")) sb.AppendLine($"  - 队伍2 {t2} 胜 (x {team2Odds})");
                    if (available.Contains("score")) sb.AppendLine($"  - 精确比分 (x 4)");
                    if (available.Contains("mvp")) sb.AppendLine($"  - 赛事MVP (x 3.5)");
                }
                else if (status == 2)
                {
                    string winnerName = winner switch { 1 => t1, 2 => t2, 3 => result, _ => "待定" };
                    sb.AppendLine($"胜者：{winnerName}");
                    if (winner != 3) sb.AppendLine($"结果：{result}");
                }

                if (!string.IsNullOrWhiteSpace(description))
                {
                    sb.AppendLine($"> 📝 {description}\r\n");
                }

                return sb.ToString();
            }
            return "数据库连接失败。";
        }

        public static bool PlaceBet(long uid, int matchId, string option, long amount, out string error)
        {
            error = "";
            using SQLHelper? sql = Factory.OpenFactory.GetSQLHelper();
            if (sql != null)
            {
                // 检查比赛
                sql.Parameters["@mid"] = matchId;
                sql.ExecuteDataSet("SELECT * FROM csbetting_matches WHERE id = @mid");
                if (!sql.Success || sql.DataSet.Tables[0].Rows.Count == 0)
                {
                    error = "比赛不存在。";
                    return false;
                }
                DataRow row = sql.DataSet.Tables[0].Rows[0];
                int status = Convert.ToInt32(row["status"]);
                DateTime deadline = Convert.ToDateTime(row["bet_deadline"]);
                if (status != 0 || DateTime.Now > deadline)
                {
                    error = "当前比赛已结束或非投注期。";
                    return false;
                }

                // --- 单场比赛投注上限检查 ---
                long alreadyBet = 0;
                long totalBet = amount;
                sql.Parameters["@uid"] = uid;
                sql.Parameters["@mid"] = matchId;
                sql.ExecuteDataSet("SELECT COALESCE(SUM(amount), 0) AS total FROM csbetting_bet_records WHERE user_id = @uid AND match_id = @mid");
                if (sql.Success && sql.DataSet.Tables[0].Rows.Count > 0)
                {
                    alreadyBet = Convert.ToInt64(sql.DataSet.Tables[0].Rows[0]["total"] ?? 0L);
                    totalBet += alreadyBet;
                }
                if (totalBet > 5000)
                {
                    error = $"本场比赛你的投注总额不能超过 5000 {General.GameplayEquilibriumConstant.InGameCurrency}（已投 {alreadyBet}）。";
                    return false;
                }

                string available = row["available_options"]?.ToString() ?? "[]";
                int optionType;
                string optionValue = option;
                if (option == "team1" && available.Contains("team1_win"))
                    optionType = 1;
                else if (option == "team2" && available.Contains("team2_win"))
                    optionType = 2;
                else if (option.StartsWith("score:") && available.Contains("score"))
                {
                    optionType = 3;
                    optionValue = option.Replace("score:", "").Trim();
                }
                else if (option.StartsWith("mvp:") && available.Contains("mvp"))
                {
                    optionType = 4;
                    optionValue = option.Replace("mvp:", "").Trim();
                }
                else
                {
                    error = "无效的投注选项。";
                    return false;
                }

                // 写入投注记录
                sql.Parameters["@uid"] = uid;
                sql.Parameters["@mid"] = matchId;
                sql.Parameters["@otype"] = optionType;
                sql.Parameters["@oval"] = optionValue;
                sql.Parameters["@amt"] = amount;
                sql.Parameters["@time"] = DateTime.Now;
                sql.Execute("INSERT INTO csbetting_bet_records (user_id, match_id, option_type, option_value, amount, bet_time) VALUES (@uid, @mid, @otype, @oval, @amt, @time)");
                if (!sql.Success)
                {
                    error = "投注记录写入失败。";
                    return false;
                }
                return true;
            }
            error = "数据库连接失败。";
            return false;
        }

        public static string SettleMatch(int matchId, string winner, string result)
        {
            using SQLHelper? sql = Factory.OpenFactory.GetSQLHelper();
            if (sql != null)
            {
                UpdateStatuses(sql);

                sql.Parameters["@mid"] = matchId;
                sql.ExecuteDataSet("SELECT * FROM csbetting_matches WHERE id = @mid");
                if (!sql.Success || sql.DataSet.Tables[0].Rows.Count == 0)
                    return "比赛不存在。";
                DataRow row = sql.DataSet.Tables[0].Rows[0];
                string available = row["available_options"]?.ToString() ?? "[]";
                bool isMvp = available.Contains("mvp", StringComparison.CurrentCultureIgnoreCase);
                int status = Convert.ToInt32(row["status"]);
                if (status == 2)
                    return "比赛已结算。";

                int winTeam = 3;
                if (!isMvp)
                {
                    if (winner == "team1") winTeam = 1;
                    else if (winner == "team2") winTeam = 2;
                    else return "请指定获胜方为 team1 或 team2。MVP 赛事获胜方请直接指定选手 ID。";
                }

                // 更新比赛结果
                sql.Parameters["@res"] = result;
                sql.Parameters["@win"] = winTeam;
                sql.Parameters["@mid"] = matchId;
                sql.Execute("UPDATE csbetting_matches SET status=2, result=@res, winner=@win WHERE id=@mid");

                // 获取所有未结算投注
                sql.Parameters["@mid"] = matchId;
                sql.ExecuteDataSet("SELECT * FROM csbetting_bet_records WHERE match_id=@mid AND is_settled=0");
                if (sql.Success && sql.DataSet.Tables[0].Rows.Count > 0)
                {
                    decimal team1Odds = Convert.ToDecimal(row["team1_win_odds"]);
                    decimal team2Odds = Convert.ToDecimal(row["team2_win_odds"]);
                    foreach (DataRow bet in sql.DataSet.Tables[0].Rows)
                    {
                        long betId = Convert.ToInt64(bet["id"]);
                        int otype = Convert.ToInt32(bet["option_type"]);
                        string ovalue = bet["option_value"].ToString() ?? "";
                        long amount = Convert.ToInt64(bet["amount"]);

                        bool win = false;
                        if (otype == 1 && winTeam == 1) win = true;
                        else if (otype == 2 && winTeam == 2) win = true;
                        else if (otype == 3 && ovalue.Replace("：", ":").Equals(result, StringComparison.CurrentCultureIgnoreCase)) win = true;
                        else if (otype == 4 && ovalue.Equals(result, StringComparison.CurrentCultureIgnoreCase)) win = true;

                        long payout = 0;
                        string note = "未中奖";
                        if (win)
                        {
                            double odds = otype switch
                            {
                                1 => (double)team1Odds,
                                2 => (double)team2Odds,
                                3 => 4,
                                4 => 3.5,
                                _ => 2.5
                            };
                            payout = (long)(amount * odds);
                            note = "中奖";
                        }
                        sql.Parameters["@payout"] = payout;
                        sql.Parameters["@note"] = note;
                        sql.Parameters["@bid"] = betId;
                        sql.Execute("UPDATE csbetting_bet_records SET is_settled=1, payout=@payout, result_note=@note WHERE id=@bid");
                    }
                }
                return $"比赛 {matchId} 结算完成。";
            }
            return "数据库连接失败。";
        }

        public static (string, int) GetMyBets(long uid, long mid = -1, int page = 1, int pageSize = 10)
        {
            using SQLHelper? sql = Factory.OpenFactory.GetSQLHelper();
            if (sql == null) return ("数据库连接失败。", 0);

            UpdateStatuses(sql);

            bool paged = true;
            sql.Parameters["@uid"] = uid;
            string matchFilter = "";
            if (mid > 0)
            {
                sql.Parameters["@mid"] = mid;
                matchFilter = " AND br.match_id = @mid";
                paged = false;
            }

            int totalPages = 0;
            if (paged)
            {
                // 总数查询：不同比赛的数量
                sql.ExecuteDataSet($@"
                SELECT COUNT(DISTINCT br.match_id) 
                FROM csbetting_bet_records br
                JOIN csbetting_matches m ON br.match_id = m.id
                WHERE br.user_id = @uid {matchFilter}");
                int total = sql.Success ? Convert.ToInt32(sql.DataSet.Tables[0].Rows[0][0]) : 0;
                totalPages = (int)Math.Ceiling(total / (double)pageSize);
                if (page > totalPages) page = totalPages;
                if (page < 1) page = 1;
                if (total == 0)
                    return ("你还没有任何竞猜记录。", 1);
            }

            // 构建分页SQL片段
            string limitClause = "";
            if (paged)
            {
                int offset = (page - 1) * pageSize;
                limitClause = $" LIMIT {pageSize} OFFSET {offset}";
            }

            // 分页聚合查询
            sql.Parameters["@uid"] = uid;
            sql.ExecuteDataSet($@"
                SELECT br.match_id, m.team1_name, m.team2_name, m.status AS match_status, m.start_time,
                       GROUP_CONCAT(CONCAT(br.option_type, ':', br.option_value, ':', br.amount) ORDER BY br.id SEPARATOR '|') AS details,
                       SUM(br.amount) AS total_amount,
                       MIN(br.is_settled) AS all_settled,
                       SUM(CASE WHEN br.is_settled = 1 AND br.payout > 0 AND br.is_claimed = 1 THEN 1 ELSE 0 END) AS claimed_count,
                       SUM(CASE WHEN br.is_settled = 1 AND br.payout > 0 AND br.is_claimed = 0 THEN 1 ELSE 0 END) AS unclaimed_count,
                       SUM(CASE WHEN br.is_settled = 1 AND br.payout = 0 THEN 1 ELSE 0 END) AS lost_count,
                       SUM(CASE WHEN br.is_settled = 1 THEN br.payout ELSE 0 END) AS total_payout
                FROM csbetting_bet_records br
                JOIN csbetting_matches m ON br.match_id = m.id
                WHERE br.user_id = @uid {matchFilter}
                GROUP BY br.match_id, m.team1_name, m.team2_name, m.status, m.start_time
                ORDER BY 
                    MIN(br.is_settled) ASC,
                    CASE WHEN MIN(br.is_settled) = 0 THEN MIN(m.start_time) END ASC,
                    CASE WHEN MIN(br.is_settled) = 1 THEN MIN(m.start_time) END DESC
                {limitClause}");

            if (!sql.Success || sql.DataSet.Tables[0].Rows.Count == 0)
                return ("你还没有任何竞猜记录。", 1);

            StringBuilder sb = new();
            if (mid > 0) sb.Append("你的本场竞猜记录：\r\n> ");
            else sb.Append($"我的竞猜{(paged && totalPages > 1 ? $"（第 {page}/{totalPages} 页）" : "")}\r\n> ");
            foreach (DataRow row in sql.DataSet.Tables[0].Rows)
            {
                int matchId = Convert.ToInt32(row["match_id"]);
                string t1 = row["team1_name"].ToString() ?? "";
                string t2 = row["team2_name"].ToString() ?? "";
                int matchStatus = Convert.ToInt32(row["match_status"]);
                long totalAmount = Convert.ToInt64(row["total_amount"]);
                long totalPayout = Convert.ToInt64(row["total_payout"]);
                int allSettled = Convert.ToInt32(row["all_settled"]);
                int claimedCount = Convert.ToInt32(row["claimed_count"]);
                int unclaimedCount = Convert.ToInt32(row["unclaimed_count"]);
                int lostCount = Convert.ToInt32(row["lost_count"]);

                // 解析投注详情
                string detailsStr = row["details"].ToString() ?? "";
                string[]? parts = detailsStr.Split('|');
                List<string> summary = [];
                foreach (string part in parts)
                {
                    string[] items = part.Split(':');
                    if (items.Length >= 3)
                    {
                        int otype = int.Parse(items[0]);
                        string ovalue = items[1];
                        long oamount = long.Parse(items[2]);
                        string optStr = otype switch
                        {
                            1 => $"{t1}胜",
                            2 => $"{t2}胜",
                            3 => $"比分 {ovalue}",
                            4 => $"MVP {ovalue}",
                            _ => ovalue
                        };
                        summary.Add($"{optStr} {oamount}G");
                    }
                }

                string detailLine = string.Join(", ", summary);
                string statusLine;
                if (allSettled == 0)
                {
                    statusLine = "待开奖";
                }
                else
                {
                    if (claimedCount > 0 && unclaimedCount == 0 && lostCount == 0)
                        statusLine = "已领取";
                    else if (unclaimedCount > 0)
                        statusLine = "待领奖";
                    else if (lostCount > 0 && claimedCount == 0 && unclaimedCount == 0)
                        statusLine = "未中奖";
                    else
                        statusLine = "部分已领";
                }

                string matchLabel = $"{t1} vs {t2}".CreateCmdInput($"比赛详情 {matchId}");
                sb.Append($"[比赛{matchId}] {matchLabel} | ");
                sb.Append($"投注：{totalAmount}G ({detailLine}) | ");
                sb.Append($"状态：{statusLine}");
                if (totalPayout > 0)
                    sb.Append($" (+{totalPayout}G)");
                sb.AppendLine();
            }
            return (sb.ToString().TrimEnd(), totalPages);
        }

        /// <summary>
        /// 根据当前时间更新赛事和比赛的状态（仅更新未结束的记录）
        /// </summary>
        private static void UpdateStatuses(SQLHelper sql)
        {
            DateTime now = DateTime.Now;

            // 更新赛事状态：0→1 (进行中)，1→2 (已结束)
            sql.Parameters["@now"] = now;
            sql.Execute("UPDATE csbetting_events SET status = 1 WHERE status = 0 AND start_time <= @now AND end_time > @now");
            sql.Parameters["@now"] = now;
            sql.Execute("UPDATE csbetting_events SET status = 2 WHERE status <= 1 AND end_time <= @now");

            // 更新比赛状态：0→1 (进行中)，1→2 (已结束) - 注意不要覆盖已结算的比赛（winner 为 null 时视为未结束）
            sql.Parameters["@now"] = now;
            sql.Execute("UPDATE csbetting_matches SET status = 1 WHERE status = 0 AND start_time <= @now AND bet_deadline < @now AND winner IS NULL");
            // 对于已经过了开始时间但还没有 winner 且状态为 1 的，可保留为进行中；实际上只要 winner 为 null，状态应为 1（进行中）
            // 如果有结果但 winner 不为 null，管理员应该已经手动结算，状态会设为 2，这里不做额外修改。
            // 安全起见，只更新未开始的，以及当比赛时间已过且无 winner 时自动变成进行中。
            // 如果需要自动结束（比如时间过长），可再添加规则，但竞猜系统通常由管理员手动结算结束。
            // 这里只做基础更新。
        }

        public static long ClaimRewards(long uid)
        {
            // 返回领取的总金币，由上层加到用户身上
            using SQLHelper? sql = Factory.OpenFactory.GetSQLHelper();
            if (sql != null)
            {
                sql.Parameters["@uid"] = uid;
                sql.ExecuteDataSet("SELECT id, payout FROM csbetting_bet_records WHERE user_id=@uid AND is_settled=1 AND payout>0 AND is_claimed=0");
                if (!sql.Success || sql.DataSet.Tables[0].Rows.Count == 0) return 0;

                long total = 0;
                foreach (DataRow row in sql.DataSet.Tables[0].Rows)
                {
                    long bid = Convert.ToInt64(row["id"]);
                    long payout = Convert.ToInt64(row["payout"]);
                    total += payout;
                    sql.Parameters["@bid"] = bid;
                    sql.Execute("UPDATE csbetting_bet_records SET is_claimed=1 WHERE id=@bid");
                }
                return total;
            }
            return 0;
        }

        // 创建赛事
        public static bool CreateEvent(string name, DateTime startTime, DateTime endTime, out string error, out long? newEventId)
        {
            error = "";
            newEventId = null;
            using SQLHelper? sql = Factory.OpenFactory.GetSQLHelper();
            if (sql != null)
            {
                // 简单校验
                if (string.IsNullOrWhiteSpace(name))
                {
                    error = "赛事名称不能为空。";
                    return false;
                }
                if (endTime <= startTime)
                {
                    error = "结束时间必须晚于开始时间。";
                    return false;
                }

                sql.Parameters["@name"] = name;
                sql.Parameters["@start"] = startTime;
                sql.Parameters["@end"] = endTime;
                sql.Execute("INSERT INTO csbetting_events (name, status, start_time, end_time) VALUES (@name, 0, @start, @end)");

                if (sql.Success)
                {
                    newEventId = sql.LastInsertId;
                    return true;
                }
                error = "赛事创建失败，数据库错误。";
                return false;
            }
            error = "数据库连接失败。";
            return false;
        }

        // 创建比赛
        public static bool CreateMatch(int eventId, string team1Name, string team2Name, string stage, DateTime startTime, DateTime betDeadline, string availableOptions, decimal? team1WinOdds, decimal? team2WinOdds, out string error, out long? newMatchId)
        {
            error = "";
            newMatchId = null;
            using var sql = Factory.OpenFactory.GetSQLHelper();
            if (sql != null)
            {
                // 检查赛事存在
                sql.Parameters["@eid"] = eventId;
                sql.ExecuteDataSet("SELECT id FROM csbetting_events WHERE id = @eid");
                if (!sql.Success || sql.DataSet?.Tables[0].Rows.Count == 0)
                {
                    error = "赛事不存在。";
                    return false;
                }

                // 处理可用选项 JSON
                List<string> options = [.. availableOptions.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                              .Select(s => s.Trim())
                                              .Where(s => s.Length > 0)];
                string optionsJson = System.Text.Json.JsonSerializer.Serialize(options);

                // 如果比赛包含比分或 MVP 选项，不允许自定义猜胜者赔率
                bool hasSpecialOption = options.Contains("score") || options.Contains("mvp");
                if ((team1WinOdds.HasValue || team2WinOdds.HasValue) && hasSpecialOption)
                {
                    error = "比赛包含比分或MVP选项时，不能自定义猜胜者赔率。";
                    return false;
                }

                decimal t1Odds = team1WinOdds ?? 2.50m;
                decimal t2Odds = team2WinOdds ?? 2.50m;
                if (t1Odds <= 0 || t2Odds <= 0)
                {
                    error = "赔率必须大于0。";
                    return false;
                }

                sql.Parameters["@eid"] = eventId;
                sql.Parameters["@t1"] = team1Name;
                sql.Parameters["@t2"] = team2Name;
                sql.Parameters["@stage"] = stage;
                sql.Parameters["@start"] = startTime;
                sql.Parameters["@deadline"] = betDeadline;
                sql.Parameters["@opts"] = optionsJson;
                sql.Parameters["@t1_odds"] = t1Odds;
                sql.Parameters["@t2_odds"] = t2Odds;
                sql.Execute(@"INSERT INTO csbetting_matches 
          (event_id, team1_name, team2_name, stage, start_time, bet_deadline, available_options, team1_win_odds, team2_win_odds, status) 
          VALUES (@eid, @t1, @t2, @stage, @start, @deadline, @opts, @t1_odds, @t2_odds, 0)");

                if (sql.Success)
                {
                    newMatchId = sql.LastInsertId;
                    return true;
                }
                error = "比赛创建失败，数据库错误。";
                return false;
            }
            error = "数据库连接失败。";
            return false;
        }

        /// <summary>
        /// 管理员提前结束竞猜（将未开始比赛标记为进行中）
        /// </summary>
        public static bool CloseBetting(int matchId, out string message)
        {
            using SQLHelper? sql = Factory.OpenFactory.GetSQLHelper();
            if (sql == null)
            {
                message = "数据库连接失败。";
                return false;
            }

            UpdateStatuses(sql);

            sql.Parameters["@mid"] = matchId;
            sql.ExecuteDataSet("SELECT status, team1_name, team2_name FROM csbetting_matches WHERE id = @mid");
            if (!sql.Success || sql.DataSet.Tables[0].Rows.Count == 0)
            {
                message = "比赛不存在。";
                return false;
            }

            int status = Convert.ToInt32(sql.DataSet.Tables[0].Rows[0]["status"]);
            if (status != 0)
            {
                message = "该比赛已开始或已结束，无需关闭投注。";
                return false;
            }

            sql.Execute("UPDATE csbetting_matches SET status = 1 WHERE id = @mid");
            if (sql.Success)
            {
                string t1 = sql.DataSet.Tables[0].Rows[0]["team1_name"].ToString() ?? "";
                string t2 = sql.DataSet.Tables[0].Rows[0]["team2_name"].ToString() ?? "";
                string matchlabel = $"{t1} vs {t2}".CreateCmdInput($"比赛详情 {matchId}");
                message = $"[比赛{matchId}] {matchlabel} 的竞猜已提前关闭。";
                return true;
            }

            message = "更新比赛状态失败。";
            return false;
        }
    }
}
