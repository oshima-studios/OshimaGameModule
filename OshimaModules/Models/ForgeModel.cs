using System.Text;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Models
{
    public class ForgeModel
    {
        public Guid Guid { get; set; } = Guid.NewGuid();
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public bool MasterForge { get; set; } = false;
        public bool MasterForgingSuccess { get; set; } = false;
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

        public string GetMaterials()
        {
            return $"☆--- 配方 ---☆\r\n{string.Join("\r\n", ForgeMaterials.Select(kv => $"{kv.Key}：{kv.Value} 个"))}";
        }

        public string GetForgingInfo()
        {
            StringBuilder builder = new();

            builder.AppendLine($"☆★☆ 锻造信息 ☆★☆");
            builder.AppendLine($"创建时间：{CreateTime.ToString(General.GeneralDateTimeFormatChinese)}");
            if (MasterForge)
            {
                builder.AppendLine($"大师锻造：是");
                builder.AppendLine($"目标地区：{FunGameConstant.RegionsName[TargetRegionId]}");
                builder.AppendLine($"目标品质：{ItemSet.GetQualityTypeName(TargetQuality)}");
            }
            else
            {
                builder.AppendLine($"大师锻造：否");
            }
            builder.AppendLine(GetMaterials());

            return builder.ToString().Trim();
        }
    }
}
