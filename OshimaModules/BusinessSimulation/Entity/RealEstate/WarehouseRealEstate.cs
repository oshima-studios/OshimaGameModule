namespace Oshima.FunGame.OshimaModules.BusinessSimulation.Entity
{
    public class WarehouseRealEstate : RealEstate
    {
        public override RealEstateType RealEstateType => RealEstateType.Warehouse;

        public virtual int InventoryCapacity { get; set; } = 0;

        public WarehouseRealEstate()
        {
            UpdateSkillInfo();
        }

        public override void UpdateSkillInfo()
        {
            if (InventoryCapacity > 0) SkillInfo["库存容量"] = $"{InventoryCapacity}";
        }
    }
}
