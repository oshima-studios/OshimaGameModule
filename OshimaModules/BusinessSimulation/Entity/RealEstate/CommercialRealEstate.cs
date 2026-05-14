namespace Oshima.FunGame.OshimaModules.BusinessSimulation.Entity
{
    public class CommercialRealEstate : RealEstate
    {
        public override RealEstateType RealEstateType => RealEstateType.Commercial;

        public virtual string CommercialType { get; set; } = "";
        public virtual double Attractiveness { get; set; } = 0;

        public CommercialRealEstate()
        {
            UpdateSkillInfo();
        }

        public override void UpdateSkillInfo()
        {
            if (Attractiveness > 0) SkillInfo["吸引力"] = $"{Attractiveness * 100:0.##}%";
        }
    }

    public class Shop : CommercialRealEstate
    {
        public override string CommercialType => "商铺";

        public virtual int ShelfCount { get; set; } = 0; 
        public virtual int InventoryCapacity { get; set; } = 0;
        public virtual int TruckCount { get; set; } = 0;

        public Shop()
        {
            UpdateSkillInfo();
        }

        public override void UpdateSkillInfo()
        {
            base.UpdateSkillInfo();
            if (ShelfCount > 0) SkillInfo["货架数量"] = $"{ShelfCount}";
            if (InventoryCapacity > 0) SkillInfo["库存容量"] = $"{InventoryCapacity}";
            if (TruckCount > 0) SkillInfo["附赠货车"] = $"{TruckCount}";
        }
    }

    public class ParkingLot : CommercialRealEstate
    {
        public override string CommercialType => "停车场";

        public virtual int BonusTruckCount { get; set; } = 0;
        public virtual int MaxTruckCapacity { get; set; } = 0;
        public virtual int PublicSpots { get; set; } = 0;
        public virtual double HourlyParkingFee { get; set; } = 0;

        public int CurrentTruckCount { get; set; } = 0;
        public int CurrentPublicVehicles { get; set; } = 0;

        public ParkingLot()
        {
            UpdateSkillInfo();
        }

        public override void UpdateSkillInfo()
        {
            base.UpdateSkillInfo();
            if (BonusTruckCount > 0) SkillInfo["附赠货车"] = $"{BonusTruckCount}";
            if (MaxTruckCapacity > 0) SkillInfo["货车停放"] = $"{CurrentTruckCount} / {MaxTruckCapacity}";
            if (PublicSpots > 0) SkillInfo["公共车位"] = $"{CurrentPublicVehicles} / {PublicSpots}";
            if (HourlyParkingFee > 0) SkillInfo["公共停车费收入"] = $"{HourlyParkingFee:0.##} {GameplayEquilibriumConstant.InGameCurrency} / 辆每小时";
        }
    }
}
