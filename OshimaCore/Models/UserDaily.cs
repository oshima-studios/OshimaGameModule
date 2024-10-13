namespace Oshima.Core.Models
{
    public class UserDaily(long user_id, long type, string daily)
    {
        public long user_id { get; set; } = user_id;
        public long type { get; set; } = type;
        public string daily { get; set; } = daily;
    }
}
