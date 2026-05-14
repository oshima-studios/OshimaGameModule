namespace Oshima.FunGame.OshimaModules.BusinessSimulation.Entity
{
    public class OfficeAssetMovableProperty : MovableProperty
    {
        public override MovablePropertyType MovablePropertyType => MovablePropertyType.OfficeAsset;

        public virtual string OfficeAssetType { get; set; } = "";
        public virtual int ComfortBonus { get; set; } = 0;
        public virtual double EfficiencyBonusPercent { get; set; } = 0;

        public OfficeAssetMovableProperty()
        {
            UpdateSkillInfo();
        }

        public override void UpdateSkillInfo()
        {
            if (ComfortBonus > 0) SkillInfo["舒适度"] = $"{ComfortBonus}";
            if (EfficiencyBonusPercent > 0) SkillInfo["办公效率"] = $"{EfficiencyBonusPercent * 100:0.##}%";
        }
    }
}
