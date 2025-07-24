using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaServers.Model
{
    public class ForgeModel
    {
        public Guid Guid { get; set; } = Guid.NewGuid();
        public bool MasterForge { get; set; } = false;
        public Dictionary<string, int> ForgeMaterials { get; set; } = [];
        public long TargetRegionId { get; set; } = 0;
        public QualityType TargetQuality { get; set; } = QualityType.White;
        public Dictionary<long, double> RegionProbabilities { get; set; } = [];
        public bool Result { get; set; } = false;
        public QualityType ResultQuality { get; set; } = QualityType.White;
        public string ResultItem { get; set; } = "";
        public long ResultRegion { get; set; } = 0;
        public string ResultString { get; set; } = "";
        public double ResultPoints => ResultPointsGeneral + ResultPointsSuccess + ResultPointsFail;
        public double ResultPointsGeneral { get; set; } = 0;
        public double ResultPointsSuccess { get; set; } = 0;
        public double ResultPointsFail { get; set; } = 0;
    }
}
