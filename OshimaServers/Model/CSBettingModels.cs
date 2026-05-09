using System.Text.Json.Serialization;

namespace Oshima.FunGame.WebAPI.Model
{
    /// <summary>
    /// 赛事状态
    /// </summary>
    public enum EventStatus
    {
        /// <summary>
        /// 未开始
        /// </summary>
        Upcoming,
        /// <summary>
        /// 进行中
        /// </summary>
        InProgress,
        /// <summary>
        /// 已结束
        /// </summary>
        Completed
    }

    /// <summary>
    /// 比赛状态
    /// </summary>
    public enum MatchStatus
    {
        /// <summary>
        /// 未开始
        /// </summary>
        Scheduled,
        /// <summary>
        /// 正在进行
        /// </summary>
        Live,
        /// <summary>
        /// 已结束
        /// </summary>
        Finished
    }

    /// <summary>
    /// 投注选项类型
    /// </summary>
    public enum BetOptionType
    {
        /// <summary>
        /// 队伍1胜
        /// </summary>
        Team1Win,
        /// <summary>
        /// 队伍2胜
        /// </summary>
        Team2Win,
        /// <summary>
        /// 精确比分
        /// </summary>
        Score,
        /// <summary>
        /// 赛事MVP
        /// </summary>
        MVP
    }

    /// <summary>
    /// CS 队伍
    /// </summary>
    public class CSTeam
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string LogoUrl { get; set; } = "";
        public List<string> Players { get; set; } = [];
    }

    /// <summary>
    /// 赛事（Event）
    /// </summary>
    public class CSEvent
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public EventStatus Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<CSMatch> Matches { get; set; } = [];

        /// <summary>
        /// MVP 候选人列表（玩家 OpenId 或游戏内 UID）
        /// </summary>
        public List<long> MVPCandidates { get; set; } = [];
    }

    /// <summary>
    /// 单场比赛
    /// </summary>
    public class CSMatch
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int Team1Id { get; set; }
        public int Team2Id { get; set; }
        public MatchStatus Status { get; set; }
        public DateTime StartTime { get; set; }
        public string Stage { get; set; } = "";

        /// <summary>
        /// 竞猜截止时间
        /// </summary>
        public DateTime BetDeadline { get; set; }

        /// <summary>
        /// 比赛结果（如 “16:14” 或 “2:1”）
        /// </summary>
        /// 
        public string? Result { get; set; }

        /// <summary>
        /// 获胜方（1=Team1，2=Team2，0=平局/未定）
        /// </summary>
        public int Winner { get; set; }

        /// <summary>
        /// 支持的投注选项类型（常规包含 Team1Win/Team2Win，总决赛增加 Score）
        /// </summary>
        public List<BetOptionType> AvailableOptions { get; set; } = [BetOptionType.Team1Win, BetOptionType.Team2Win];

        /// <summary>
        /// 队伍1胜赔率，默认2.5
        /// </summary>
        public decimal Team1WinOdds { get; set; } = 2.50m;

        /// <summary>
        /// 队伍2胜赔率，默认2.5
        /// </summary>
        public decimal Team2WinOdds { get; set; } = 2.50m;
    }

    /// <summary>
    /// 用户投注记录
    /// </summary>
    public class BetRecord
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public int MatchId { get; set; }
        public BetOptionType OptionType { get; set; }

        /// <summary>
        /// 投注选项内容（如 “Team1Win” 或具体比分 “16:14”）
        /// </summary>
        public string OptionValue { get; set; } = "";

        /// <summary>
        /// 投注金币
        /// </summary>
        public long Amount { get; set; }

        /// <summary>
        /// 投注时间
        /// </summary>
        public DateTime BetTime { get; set; }

        /// <summary>
        /// 是否已结算
        /// </summary>
        public bool IsSettled { get; set; }

        /// <summary>
        /// 结算金额（含本金），未结算为 null
        /// </summary>
        public long? Payout { get; set; }

        /// <summary>
        /// 结算备注
        /// </summary>
        public string? ResultNote { get; set; }
    }

    public class CreateEventRequest
    {
        [JsonPropertyName("uid")]
        public long Uid { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("start_time")]
        public DateTime StartTime { get; set; }

        [JsonPropertyName("end_time")]
        public DateTime EndTime { get; set; }
    }

    public class CreateMatchRequest
    {
        [JsonPropertyName("uid")]
        public long Uid { get; set; }

        [JsonPropertyName("event_id")]
        public int EventId { get; set; }

        [JsonPropertyName("team1_name")]
        public string Team1Name { get; set; } = "";

        [JsonPropertyName("team2_name")]
        public string Team2Name { get; set; } = "";

        [JsonPropertyName("stage")]
        public string Stage { get; set; } = "";

        [JsonPropertyName("start_time")]
        public DateTime StartTime { get; set; }

        [JsonPropertyName("bet_deadline")]
        public DateTime BetDeadline { get; set; }

        [JsonPropertyName("available_options")]
        public string AvailableOptions { get; set; } = "team1_win,team2_win";

        [JsonPropertyName("team1_win_odds")]
        public decimal? Team1WinOdds { get; set; }

        [JsonPropertyName("team2_win_odds")]
        public decimal? Team2WinOdds { get; set; }
    }
}