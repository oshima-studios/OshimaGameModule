using System.Text.Json.Serialization;

namespace Oshima.FunGame.WebAPI.Model
{
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

        [JsonPropertyName("team1_win_probability")]
        public decimal? Team1WinProbability { get; set; }
    }

    public class UpdateMatchRequest
    {
        [JsonPropertyName("uid")]
        public long Uid { get; set; }

        [JsonPropertyName("match_id")]
        public int MatchId { get; set; }

        [JsonPropertyName("team1_win_odds")]
        public decimal? Team1WinOdds { get; set; }

        [JsonPropertyName("team2_win_odds")]
        public decimal? Team2WinOdds { get; set; }

        [JsonPropertyName("start_time")]
        public DateTime? StartTime { get; set; }

        [JsonPropertyName("bet_deadline")]
        public DateTime? BetDeadline { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("team1_win_probability")]
        public decimal? Team1WinProbability { get; set; }

        [JsonPropertyName("result")]
        public string? Result { get; set; }

        [JsonPropertyName("stage")]
        public string? Stage { get; set; }
    }

    public class BettingEvent
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("status")]
        public int Status { get; set; } // 0=未开始 1=进行中 2=已结束

        [JsonPropertyName("start_time")]
        public DateTime StartTime { get; set; }

        [JsonPropertyName("end_time")]
        public DateTime EndTime { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }

    public class BettingMatch
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("event_id")]
        public int EventId { get; set; }

        [JsonPropertyName("stage")]
        public string? Stage { get; set; }

        [JsonPropertyName("team1_name")]
        public string Team1Name { get; set; } = "";

        [JsonPropertyName("team1_logo")]
        public string? Team1Logo { get; set; }

        [JsonPropertyName("team2_name")]
        public string Team2Name { get; set; } = "";

        [JsonPropertyName("team2_logo")]
        public string? Team2Logo { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; } // 0=未开始 1=进行中 2=已结束

        [JsonPropertyName("start_time")]
        public DateTime StartTime { get; set; }

        [JsonPropertyName("bet_deadline")]
        public DateTime BetDeadline { get; set; }

        [JsonPropertyName("result")]
        public string? Result { get; set; }

        [JsonPropertyName("winner")]
        public int? Winner { get; set; } // 1=team1, 2=team2, null=未定

        [JsonPropertyName("available_options")]
        public string AvailableOptions { get; set; } = "[]"; // JSON 数组字符串

        [JsonPropertyName("team1_win_odds")]
        public decimal Team1WinOdds { get; set; }

        [JsonPropertyName("team2_win_odds")]
        public decimal Team2WinOdds { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }

    public class BettingBetRecord
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("user_id")]
        public long UserId { get; set; }

        [JsonPropertyName("match_id")]
        public int MatchId { get; set; }

        [JsonPropertyName("option_type")]
        public int OptionType { get; set; } // 1=team1胜 2=team2胜 3=精确比分 4=MVP

        [JsonPropertyName("option_value")]
        public string OptionValue { get; set; } = "";

        [JsonPropertyName("amount")]
        public long Amount { get; set; }

        [JsonPropertyName("odds_at_bet")]
        public decimal OddsAtBet { get; set; }

        [JsonPropertyName("bet_time")]
        public DateTime BetTime { get; set; }

        [JsonPropertyName("is_settled")]
        public bool IsSettled { get; set; }

        [JsonPropertyName("payout")]
        public long? Payout { get; set; }

        [JsonPropertyName("is_claimed")]
        public bool IsClaimed { get; set; }

        [JsonPropertyName("result_note")]
        public string? ResultNote { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}