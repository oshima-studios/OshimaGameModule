namespace Oshima.FunGame.WebAPI.Models
{
    public class UserDaily(long user_id, long type, string daily) : BaseUserDaily(type, daily)
    {
        public long user_id { get; set; } = user_id;
    }

    public class OpenUserDaily(string user_id, long type, string daily) : BaseUserDaily(type, daily)
    {
        public string user_id { get; set; } = user_id;
    }

    public class BaseUserDaily(long type, string daily)
    {
        public long type { get; set; } = type;
        public string daily { get; set; } = daily;
    }
}
