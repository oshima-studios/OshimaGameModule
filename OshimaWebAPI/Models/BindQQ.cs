namespace Oshima.FunGame.WebAPI.Models
{
    public class BindQQ(string openid, long qq)
    {
        public string Openid { get; set; } = openid;
        public long QQ { get; set; } = qq;
    }
}
