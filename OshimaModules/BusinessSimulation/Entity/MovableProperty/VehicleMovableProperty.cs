using System.Text;
using Milimoe.FunGame.Core.Entity;

namespace Oshima.FunGame.OshimaModules.BusinessSimulation.Entity
{
    public class VehicleMovableProperty : MovableProperty
    {
        public override string Category => "载具";
        public override MovablePropertyType MovablePropertyType => MovablePropertyType.Vehicle;

        public virtual string VehicleType { get; set; } = "";
        public virtual int PassengerCapacity { get; set; } = 0;
        public virtual int CargoCapacity { get; set; } = 0;
        public virtual double TransportTime { get; set; } = 5;
        public virtual double TransportCost { get; set; } = 0;
        public virtual double LoadingTime { get; set; } = 0;
        public virtual List<RealEstate> ParkingAvailable { get; set; } = [];

        public bool Enabled
        {
            get => !IsParked && field;
            set;
        }
        public bool IsParked { get; set; } = false;
        public PackingRequirement ParkingIn { get; set; } = new(Guid.Empty, "");
        public List<TransportSchedule> TransportScheduleList { get; } = [];
        public List<Character> VehicleOperations { get; } = [];

        public VehicleMovableProperty()
        {
            UpdateSkillInfo();
        }

        public override void UpdateSkillInfo()
        {
            base.UpdateSkillInfo();
            if (PassengerCapacity > 0) SkillInfo["乘客定员"] = $"{PassengerCapacity}";
            if (CargoCapacity > 0) SkillInfo["载货容量"] = $"{CargoCapacity}";
            if (TransportTime > 0) SkillInfo["运输时间"] = $"{TransportTime:0.##} 分钟 / 次";
            if (TransportCost > 0) SkillInfo["运输成本"] = $"{TransportCost:0.##} {GameplayEquilibriumConstant.InGameCurrency} / 次";
            if (LoadingTime > 0) SkillInfo["装载时间"] = $"{LoadingTime:0.##} 分钟 / 百件货物";
            if (VehicleOperations.Count > 0) SkillInfo["车务担当"] = $"\r\n{string.Join("\r\n", VehicleOperations.Select(c => c.ToStringWithLevelWithOutUser()))}";
            if (TransportScheduleList.Count > 0)
            {
                int count = 0;
                SkillInfo["时间表"] = $"";
                foreach (TransportSchedule ts in TransportScheduleList)
                {
                    SkillInfo[$"第 {++count} 站"] = $"\r\n{ts}";
                }
            }
            SkillInfo["停放于"] = $"\r\n{ParkingIn}";
        }
    }

    public enum LoadingStrategy
    {
        Full = 1,
        Half = 2,
        AllowForTime = 3,
        SpecifiedCapacity = 4
    }

    public class TransportSchedule(Guid esId, string esName)
    {
        public Guid TargetRealEstate { get; set; } = esId;
        public string TargetRealEstateName { get; set; } = esName;

        public LoadingStrategy LoadingStrategy { get; set; } = LoadingStrategy.Full;
        public int WaitingTime { get; set; } = 0;
        public double SpecifiedCapacity { get; set; } = 0;

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.AppendLine($"目标地点：{TargetRealEstateName}");
            sb.AppendLine($"装载策略：{GetLoadingStrategyName(LoadingStrategy)}");
            return sb.ToString();
        }

        public string GetLoadingStrategyName(LoadingStrategy ls)
        {
            return ls switch
            {
                LoadingStrategy.Full => "满载",
                LoadingStrategy.Half => "半载",
                LoadingStrategy.AllowForTime => $"等待 {WaitingTime} 分钟",
                LoadingStrategy.SpecifiedCapacity => $"装载容量达到 {SpecifiedCapacity} 件",
                _ => "未知装载策略"
            };
        }
    }

    public class PackingRequirement(Guid esId, string esName)
    {
        public Guid TargetRealEstate { get; set; } = esId;
        public string TargetRealEstateName { get; set; } = esName;

        public override string ToString() => TargetRealEstateName;
    }
}
