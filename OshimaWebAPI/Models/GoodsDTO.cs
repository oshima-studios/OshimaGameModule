namespace Oshima.FunGame.WebAPI.Models
{
    public class GoodsDTO
    {
        public long Id { get; set; } = 0;
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime? ExpireTime { get; set; } = null;
        public int Stock { get; set; } = 0;
        public int Quota { get; set; } = 0;
        public double CurrencyPrice { get; set; } = 0;
        public double MaterialPrice { get; set; } = 0;
        public Dictionary<string, object> Values { get; set; } = [];
    }
}
