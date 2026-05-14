namespace Oshima.FunGame.OshimaModules.BusinessSimulation.Entity
{
    public class DeviceMovableProperty : MovableProperty
    {
        public override MovablePropertyType MovablePropertyType => MovablePropertyType.Device;

        public virtual string DeviceType { get; set; } = "";
        public virtual double ProductionSpeedBonusPercent { get; set; } = 0;
        public virtual double EnergyConsumption { get; set; } = 0;
        public virtual int DeviceLevel { get; set; } = 1;

        public List<string> CompatibleItem { get; set; } = [];

        public DeviceMovableProperty()
        {
            UpdateSkillInfo();
        }

        public override void UpdateSkillInfo()
        {
            base.UpdateSkillInfo();
            if (ProductionSpeedBonusPercent > 0) SkillInfo["生产速度"] = $"{ProductionSpeedBonusPercent:0.##}%";
            if (EnergyConsumption > 0) SkillInfo["能耗"] = $"{EnergyConsumption:0.##} 每日";
            if (DeviceLevel > 0) SkillInfo["设备等级"] = $"{DeviceLevel}";
            if (CompatibleItem.Count > 0) SkillInfo["适用货物"] = $"\r\n{string.Join("\r\n", CompatibleItem)}";
        }
    }
}
