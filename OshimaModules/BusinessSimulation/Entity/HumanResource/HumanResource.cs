using System.Text;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.BusinessSimulation.Interface;

namespace Oshima.FunGame.OshimaModules.BusinessSimulation.Entity
{
    public class HumanResource : BaseEntity, IDescription, IBusinessSimulationEntity
    {
        public virtual QualityType QualityType { get; set; } = QualityType.White;
        public virtual HumanResourceType HumanResourceType { get; set; } = HumanResourceType.General;
        public virtual string Description { get; set; } = "";
        public virtual string GeneralDescription { get; set; } = "";
        public virtual string BackgroundStory { get; set; } = "";
        public virtual string Category { get; set; } = "";

        public bool Enable { get; set; } = true;
        public Dictionary<string, string> SkillInfo { get; set; } = [];

        public string ToString(bool isShowGeneralDescription, bool isShowInStore = false)
        {
            StringBuilder builder = new();

            builder.AppendLine($"【{Name}】");

            string itemquality = ItemSet.GetQualityTypeName(QualityType);
            string itemtype = GetHumanResourceTypeName(HumanResourceType);
            if (itemtype != "") itemtype = $" {itemtype}";

            builder.AppendLine($"{itemquality + itemtype}");
            if (!string.IsNullOrWhiteSpace(Category)) builder.AppendLine(Category);

            if (isShowGeneralDescription && GeneralDescription != "")
            {
                builder.AppendLine("描述：" + GeneralDescription);
            }
            else if (Description != "")
            {
                builder.AppendLine("描述：" + Description);
            }

            if (SkillInfo.Count > 0)
            {
                builder.AppendLine("=== 技能 ===");
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

        public override bool Equals(IBaseEntity? other) => other is HumanResource hr && GetIdName() == hr.GetIdName();

        public static string GetHumanResourceTypeName(HumanResourceType type)
        {
            return type switch
            {
                HumanResourceType.General => "通用人力资源",
                HumanResourceType.Employee => "员工",
                HumanResourceType.Manager => "经理",
                HumanResourceType.Team => "团队",
                HumanResourceType.CooperationPartner => "合作伙伴",
                _ => "未知人力资源"
            };
        }

        public virtual void UpdateSkillInfo()
        {

        }
    }

    public enum HumanResourceType
    {
        General,
        Employee,
        Manager,
        Team,
        CooperationPartner
    }
}
