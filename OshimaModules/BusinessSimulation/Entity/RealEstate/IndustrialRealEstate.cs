using System.Text;

namespace Oshima.FunGame.OshimaModules.BusinessSimulation.Entity
{
    public class IndustrialRealEstate : RealEstate
    {
        public override RealEstateType RealEstateType => RealEstateType.Industrial;

        public virtual string IndustryType { get; set; } = "";
    }

    public class Factory : IndustrialRealEstate
    {
        public override string IndustryType => "工厂";

        public virtual int TruckCount { get; set; } = 0;
        public virtual int MaxProductionLines { get; } = 1;
        public virtual double CostPerLine { get; set; } = 0;

        public Dictionary<int, ProductionLine> ProductionLines { get; set; } = [];
        public int ActiveLines => ProductionLines.Values.Count(pl => pl.Active);
        public override double MaintenanceCost => ActiveLines * CostPerLine;
        public override string MaintenanceUnit => "每条有效流水线每日";

        public Factory(int maxProductionLines = 1)
        {
            Category = "工厂";
            MaxProductionLines = maxProductionLines;
            // 注意：工厂一经创建，流水线数量便无法更改
            for (int i = 0; i < MaxProductionLines; i++)
            {
                ProductionLines[i] = new();
            }
            UpdateSkillInfo();
        }

        public override void UpdateSkillInfo()
        {
            if (TruckCount > 0) SkillInfo["附赠货车"] = $"{TruckCount}";
            if (MaxProductionLines > 0)
            {
                SkillInfo["流水线数量"] = $"{ActiveLines} / {MaxProductionLines}";
                if (ProductionLines.Count > 0)
                {
                    foreach (var line in ProductionLines)
                    {
                        SkillInfo[$"流水线 {line.Key}"] = $"\r\n{line.Value}";
                    }
                }
            }
        }
    }

    public class ProductionLine
    {
        public bool Active { get; set; } = false;
        public string ItemName { get; set; } = "";
        public double CountPerMinute => 60.0 / UnitProductionTime;
        public int UnitProductionTime { get; set; } = 0;
        public override string ToString()
        {
            StringBuilder sb = new();
            sb.AppendLine($"是否启用：{(Active ? "是" : "否")}");
            sb.AppendLine($"生产货物：{ItemName}");
            sb.AppendLine($"生产时间：{UnitProductionTime} 秒 / 件");
            sb.AppendLine($"理论产量：{CountPerMinute:0.##} 件 / 分钟");
            return sb.ToString();
        }
    }
}
