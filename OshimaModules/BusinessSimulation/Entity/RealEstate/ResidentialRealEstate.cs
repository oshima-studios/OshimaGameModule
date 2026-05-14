using Milimoe.FunGame.Core.Entity;

namespace Oshima.FunGame.OshimaModules.BusinessSimulation.Entity
{
    public class ResidentialRealEstate : RealEstate
    {
        public override RealEstateType RealEstateType => RealEstateType.Residential;

        public virtual int MaxResidents { get; set; } = 0;
        public virtual int ExperiencePerMinute { get; set; } = 0;
        public virtual double RegenerationBonus { get; set; } = 0;
        public virtual int ExtraInventorySlots { get; set; } = 0;
        public virtual int FreeReviveCount { get; set; } = 0;
        public virtual double CostPerResident { get; set; } = 0;

        public int CurrentUsedFreeReviveCount { get; set; } = 0;
        public override double MaintenanceCost => Characters.Count * CostPerResident;
        public override string MaintenanceUnit => "每人每日";
        public HashSet<Character> Characters { get; } = [];

        public ResidentialRealEstate()
        {
            UpdateSkillInfo();
        }

        public override void UpdateSkillInfo()
        {
            if (MaxResidents > 0) SkillInfo["可入住角色数量"] = $"{Characters.Count} / {MaxResidents}";
            if (ExperiencePerMinute > 0) SkillInfo["入住角色经验加成"] = $"{ExperiencePerMinute} / 人每分钟";
            if (RegenerationBonus > 0) SkillInfo["生命/魔法回复速度提升"] = $"{RegenerationBonus * 100:0.##}%";
            if (ExtraInventorySlots > 0) SkillInfo["额外玩家库存容量"] = $"{ExtraInventorySlots}";
            if (FreeReviveCount > 0) SkillInfo["免费复活次数"] = $"{CurrentUsedFreeReviveCount} / {FreeReviveCount}";
            if (Characters.Count > 0) SkillInfo["已入住角色"] = $"\r\n{string.Join("\r\n", Characters.Select(c => c.ToStringWithLevelWithOutUser()))}";
        }
    }
}
