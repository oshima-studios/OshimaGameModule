using System.Text;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.BusinessSimulation.Interface;

namespace Oshima.FunGame.OshimaModules.BusinessSimulation.Entity
{
    public class RealEstate : BaseEntity, IDescription, IBusinessSimulationEntity
    {
        public virtual QualityType QualityType { get; set; } = QualityType.White;
        public virtual RealEstateType RealEstateType { get; set; } = RealEstateType.General;
        public virtual string Description { get; set; } = "";
        public virtual string GeneralDescription { get; set; } = "";
        public virtual string BackgroundStory { get; set; } = "";
        public virtual double Price { get; set; } = 0;
        public virtual string Category { get; set; } = "";
        public virtual double MaintenanceCost { get; set; } = 0;
        public virtual string MaintenanceUnit { get; set; } = "每日";

        public bool Enable { get; set; } = true;
        public bool IsPurchasable { get; set; } = true;
        public double OriginalPrice { get; set; } = 0;
        public bool IsSellable { get; set; } = true;
        public DateTime NextSellableTime { get; set; } = DateTime.MinValue;
        public Dictionary<string, string> SkillInfo { get; set; } = [];

        public string ToString(bool isShowGeneralDescription, bool isShowInStore = false)
        {
            StringBuilder builder = new();

            builder.AppendLine($"【{Name}】");

            string itemquality = ItemSet.GetQualityTypeName(QualityType);
            string itemtype = GetRealEstateTypeName(RealEstateType);
            if (itemtype != "") itemtype = $" {itemtype}";

            builder.AppendLine($"{itemquality + itemtype}");
            if (!string.IsNullOrWhiteSpace(Category)) builder.AppendLine(Category);

            if (isShowInStore && Price > 0)
            {
                builder.AppendLine($"售价：{Price:0.##} {GameplayEquilibriumConstant.InGameCurrency}");
            }
            else if (Price > 0)
            {
                builder.AppendLine($"回收价：{Price:0.##} {GameplayEquilibriumConstant.InGameCurrency}");
            }
            if (OriginalPrice > 0) builder.AppendLine($"在商店或市场中出售时，售价不得超过原价：{OriginalPrice:0.##} {GameplayEquilibriumConstant.InGameCurrency}");

            if (isShowInStore)
            {
                if (IsSellable)
                {
                    builder.AppendLine($"购买此资产后可立即出售");
                }
                else if (NextSellableTime != DateTime.MinValue)
                {
                    builder.AppendLine($"购买此资产后，将在 {NextSellableTime.ToString(General.GeneralDateTimeFormatChinese)} 后可出售");
                }
            }
            else
            {
                if (IsSellable)
                {
                    builder.AppendLine("可出售");
                }
                else if (!IsSellable && NextSellableTime != DateTime.MinValue)
                {
                    builder.AppendLine($"此资产将在 {NextSellableTime.ToString(General.GeneralDateTimeFormatChinese)} 后可出售");
                }
                else if (!IsSellable)
                {
                    builder.AppendLine("不可出售");
                }
            }

            if (MaintenanceCost > 0)
            {
                builder.AppendLine($"维护费：{(MaintenanceUnit != "" ? MaintenanceUnit : "每日")} {MaintenanceCost:0.##} {GameplayEquilibriumConstant.InGameCurrency}");
            }

            if (isShowGeneralDescription && GeneralDescription != "")
            {
                builder.AppendLine("资产描述：" + GeneralDescription);
            }
            else if (Description != "")
            {
                builder.AppendLine("资产描述：" + Description);
            }

            if (SkillInfo.Count > 0)
            {
                builder.AppendLine("=== 资产技能 ===");
                foreach (var skill in SkillInfo)
                {
                    builder.AppendLine($"{skill.Key}{(!string.IsNullOrWhiteSpace(skill.Value) ? $"：{skill.Value}" : "")}");
                }
            }

            if (BackgroundStory != "")
            {
                builder.AppendLine($"\"{BackgroundStory}\"");
            }

            return builder.ToString();
        }

        public override bool Equals(IBaseEntity? other) => other is RealEstate re && GetIdName() == re.GetIdName();

        public static string GetRealEstateTypeName(RealEstateType type)
        {
            return type switch
            {
                RealEstateType.General => "通用资产",
                RealEstateType.Residential => "住宅资产",
                RealEstateType.Commercial => "商业资产",
                RealEstateType.Industrial => "工业资产",
                RealEstateType.Warehouse => "仓储资产",
                _ => "未知资产"
            };
        }

        public virtual void UpdateSkillInfo()
        {
            
        }
    }

    public enum RealEstateType
    {
        General,
        Residential,
        Commercial,
        Industrial,
        Warehouse
    }
}
