namespace Oshima.FunGame.OshimaModules.Models
{
    public class LastStoreModel
    {
        public DateTime LastTime { get; set; } = DateTime.MinValue;
        public bool IsDaily { get; set; } = false;
        public string StoreRegion { get; set; } = "";
        public string StoreName { get; set; } = "";
    }
}
