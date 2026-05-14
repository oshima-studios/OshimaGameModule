using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Milimoe.FunGame.Core.Api.Utility;
using Oshima.FunGame.WebAPI.Model;

namespace Oshima.FunGame.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BettingController : ControllerBase
    {
        /// <summary>
        /// 获取赛事列表（分页 + 可选状态过滤）
        /// </summary>
        [AllowAnonymous]
        [HttpGet("events")]
        public ActionResult<IEnumerable<BettingEvent>> GetEvents(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] int? status = null)
        {
            using var sql = Factory.OpenFactory.GetSQLHelper();
            if (sql == null) return StatusCode(500, "数据库连接失败");
            sql.ClearParametersAfterExecute = false;

            // 动态条件
            string where = status.HasValue ? "WHERE status = @status" : "";
            if (status.HasValue) sql.Parameters["@status"] = status.Value;

            // 总数
            sql.ExecuteDataSet($"SELECT COUNT(*) FROM csbetting_events {where}");
            int total = sql.Success ? Convert.ToInt32(sql.DataSet.Tables[0].Rows[0][0]) : 0;
            int totalPages = (int)Math.Ceiling(total / (double)pageSize);
            if (page > totalPages) page = totalPages;
            if (page < 1) page = 1;

            int offset = (page - 1) * pageSize;
            sql.ExecuteDataSet($@"
                SELECT * FROM csbetting_events {where}
                ORDER BY start_time DESC
                LIMIT {pageSize} OFFSET {offset}");

            if (!sql.Success || sql.DataSet.Tables.Count == 0)
                return Ok(new { data = Array.Empty<BettingEvent>(), page, totalPages, total });

            var list = new List<BettingEvent>();
            foreach (DataRow row in sql.DataSet.Tables[0].Rows)
            {
                list.Add(MapEvent(row));
            }

            return Ok(new { data = list, page, totalPages, total });
        }

        /// <summary>
        /// 获取单个赛事详情
        /// </summary>
        [AllowAnonymous]
        [HttpGet("events/{id:int}")]
        public ActionResult<BettingEvent> GetEvent(int id)
        {
            using var sql = Factory.OpenFactory.GetSQLHelper();
            if (sql == null) return StatusCode(500, "数据库连接失败");
            sql.ClearParametersAfterExecute = false;

            sql.Parameters["@id"] = id;
            DataRow? row = sql.ExecuteDataRow("SELECT * FROM csbetting_events WHERE id = @id");
            if (row == null) return NotFound();

            return Ok(MapEvent(row));
        }

        /// <summary>
        /// 获取比赛列表（分页 + 可选过滤）
        /// </summary>
        [AllowAnonymous]
        [HttpGet("matches")]
        public ActionResult<IEnumerable<BettingMatch>> GetMatches(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] int? eventId = null,
            [FromQuery] int? status = null)
        {
            using var sql = Factory.OpenFactory.GetSQLHelper();
            if (sql == null) return StatusCode(500, "数据库连接失败");
            sql.ClearParametersAfterExecute = false;

            var conditions = new List<string>();
            if (eventId.HasValue)
            {
                conditions.Add("event_id = @eventId");
                sql.Parameters["@eventId"] = eventId.Value;
            }
            if (status.HasValue)
            {
                conditions.Add("status = @status");
                sql.Parameters["@status"] = status.Value;
            }
            string where = conditions.Count > 0 ? "WHERE " + string.Join(" AND ", conditions) : "";

            sql.ExecuteDataSet($"SELECT COUNT(*) FROM csbetting_matches {where}");
            int total = sql.Success ? Convert.ToInt32(sql.DataSet.Tables[0].Rows[0][0]) : 0;
            int totalPages = (int)Math.Ceiling(total / (double)pageSize);
            if (page > totalPages) page = totalPages;
            if (page < 1) page = 1;

            int offset = (page - 1) * pageSize;
            sql.ExecuteDataSet($@"
                SELECT * FROM csbetting_matches {where}
                ORDER BY start_time DESC
                LIMIT {pageSize} OFFSET {offset}");

            if (!sql.Success || sql.DataSet.Tables.Count == 0)
                return Ok(new { data = Array.Empty<BettingMatch>(), page, totalPages, total });

            var list = new List<BettingMatch>();
            foreach (DataRow row in sql.DataSet.Tables[0].Rows)
            {
                list.Add(MapMatch(row));
            }

            return Ok(new { data = list, page, totalPages, total });
        }

        /// <summary>
        /// 获取单个比赛详情
        /// </summary>
        [AllowAnonymous]
        [HttpGet("matches/{id:int}")]
        public ActionResult<BettingMatch> GetMatch(int id)
        {
            using var sql = Factory.OpenFactory.GetSQLHelper();
            if (sql == null) return StatusCode(500, "数据库连接失败");
            sql.ClearParametersAfterExecute = false;

            sql.Parameters["@id"] = id;
            DataRow? row = sql.ExecuteDataRow("SELECT * FROM csbetting_matches WHERE id = @id");
            if (row == null) return NotFound();

            return Ok(MapMatch(row));
        }

        /// <summary>
        /// 获取投注记录列表（管理员专用）
        /// </summary>
        [Authorize(AuthenticationSchemes = "CustomBearer")]
        [HttpGet("bets")]
        public ActionResult<IEnumerable<BettingBetRecord>> GetBets(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] long? userId = null,
            [FromQuery] int? matchId = null,
            [FromQuery] bool? isSettled = null)
        {
            // 简易管理员检查（可替换为实际鉴权）
            if (!User.IsInRole("Admin") && !User.IsInRole("Operator"))
                return Forbid();

            using var sql = Factory.OpenFactory.GetSQLHelper();
            if (sql == null) return StatusCode(500, "数据库连接失败");
            sql.ClearParametersAfterExecute = false;

            var conditions = new List<string>();
            if (userId.HasValue)
            {
                conditions.Add("user_id = @uid");
                sql.Parameters["@uid"] = userId.Value;
            }
            if (matchId.HasValue)
            {
                conditions.Add("match_id = @mid");
                sql.Parameters["@mid"] = matchId.Value;
            }
            if (isSettled.HasValue)
            {
                conditions.Add("is_settled = @settled");
                sql.Parameters["@settled"] = isSettled.Value ? 1 : 0;
            }
            string where = conditions.Count > 0 ? "WHERE " + string.Join(" AND ", conditions) : "";

            sql.ExecuteDataSet($"SELECT COUNT(*) FROM csbetting_bet_records {where}");
            int total = sql.Success ? Convert.ToInt32(sql.DataSet.Tables[0].Rows[0][0]) : 0;
            int totalPages = (int)Math.Ceiling(total / (double)pageSize);
            if (page > totalPages) page = totalPages;
            if (page < 1) page = 1;

            int offset = (page - 1) * pageSize;
            sql.ExecuteDataSet($@"
                SELECT * FROM csbetting_bet_records {where}
                ORDER BY bet_time DESC
                LIMIT {pageSize} OFFSET {offset}");

            if (!sql.Success || sql.DataSet.Tables.Count == 0)
                return Ok(new { data = Array.Empty<BettingBetRecord>(), page, totalPages, total });

            var list = new List<BettingBetRecord>();
            foreach (DataRow row in sql.DataSet.Tables[0].Rows)
            {
                list.Add(MapBetRecord(row));
            }

            return Ok(new { data = list, page, totalPages, total });
        }

        // ---------- 映射辅助方法 ----------
        private static BettingEvent MapEvent(DataRow row)
        {
            return new BettingEvent
            {
                Id = Convert.ToInt32(row["id"]),
                Name = row["name"].ToString() ?? "",
                Status = Convert.ToInt32(row["status"]),
                StartTime = Convert.ToDateTime(row["start_time"]),
                EndTime = Convert.ToDateTime(row["end_time"]),
                CreatedAt = Convert.ToDateTime(row["created_at"]),
                UpdatedAt = Convert.ToDateTime(row["updated_at"])
            };
        }

        private static BettingMatch MapMatch(DataRow row)
        {
            return new BettingMatch
            {
                Id = Convert.ToInt32(row["id"]),
                EventId = Convert.ToInt32(row["event_id"]),
                Stage = row["stage"]?.ToString(),
                Team1Name = row["team1_name"].ToString() ?? "",
                Team1Logo = row["team1_logo"]?.ToString(),
                Team2Name = row["team2_name"].ToString() ?? "",
                Team2Logo = row["team2_logo"]?.ToString(),
                Status = Convert.ToInt32(row["status"]),
                StartTime = Convert.ToDateTime(row["start_time"]),
                BetDeadline = Convert.ToDateTime(row["bet_deadline"]),
                Result = row["result"]?.ToString(),
                Winner = row["winner"] != DBNull.Value ? Convert.ToInt32(row["winner"]) : null,
                AvailableOptions = row["available_options"].ToString() ?? "[]",
                Team1WinOdds = Convert.ToDecimal(row["team1_win_odds"]),
                Team2WinOdds = Convert.ToDecimal(row["team2_win_odds"]),
                Description = row["description"]?.ToString(),
                CreatedAt = Convert.ToDateTime(row["created_at"]),
                UpdatedAt = Convert.ToDateTime(row["updated_at"]),
                BettingEnabled = Convert.ToInt32(row["betting_enabled"]) == 1
            };
        }

        private static BettingBetRecord MapBetRecord(DataRow row)
        {
            return new BettingBetRecord
            {
                Id = Convert.ToInt64(row["id"]),
                UserId = Convert.ToInt64(row["user_id"]),
                MatchId = Convert.ToInt32(row["match_id"]),
                OptionType = Convert.ToInt32(row["option_type"]),
                OptionValue = row["option_value"].ToString() ?? "",
                Amount = Convert.ToInt64(row["amount"]),
                OddsAtBet = Convert.ToDecimal(row["odds_at_bet"]),
                BetTime = Convert.ToDateTime(row["bet_time"]),
                IsSettled = Convert.ToInt32(row["is_settled"]) == 1,
                Payout = row["payout"] != DBNull.Value ? Convert.ToInt64(row["payout"]) : null,
                IsClaimed = Convert.ToInt32(row["is_claimed"]) == 1,
                ResultNote = row["result_note"]?.ToString(),
                CreatedAt = Convert.ToDateTime(row["created_at"]),
                UpdatedAt = Convert.ToDateTime(row["updated_at"])
            };
        }
    }
}
