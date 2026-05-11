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
    }
}