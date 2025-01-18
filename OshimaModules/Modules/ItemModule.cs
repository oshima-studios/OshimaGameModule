using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Oshima.Core.Constant;
using Oshima.FunGame.OshimaModules.Items;

namespace Oshima.FunGame.OshimaModules
{
    public class ItemModule : Milimoe.FunGame.Core.Library.Common.Addon.ItemModule
    {
        public override string Name => OshimaGameModuleConstant.Item;
        public override string Description => OshimaGameModuleConstant.Description;
        public override string Version => OshimaGameModuleConstant.Version;
        public override string Author => OshimaGameModuleConstant.Author;

        public override Dictionary<string, Item> Items
        {
            get
            {
                return Factory.GetGameModuleInstances<Item>(OshimaGameModuleConstant.General, OshimaGameModuleConstant.Item);
            }
        }

        protected override Factory.EntityFactoryDelegate<Item> ItemFactory()
        {
            return (id, name, args) =>
            {
                return id switch
                {
                    (long)AccessoryID.攻击之爪8 => new 攻击之爪8(),
                    (long)AccessoryID.攻击之爪20 => new 攻击之爪20(),
                    (long)AccessoryID.攻击之爪35 => new 攻击之爪35(),
                    (long)AccessoryID.攻击之爪50 => new 攻击之爪50(),
                    (long)ConsumableID.小经验书 => new 小经验书(),
                    (long)ConsumableID.中经验书 => new 中经验书(),
                    (long)ConsumableID.大经验书 => new 大经验书(),
                    (long)SpecialItemID.升华之印 => new 升华之印(),
                    (long)SpecialItemID.流光之印 => new 流光之印(),
                    (long)SpecialItemID.永恒之印 => new 永恒之印(),
                    (long)SpecialItemID.技能卷轴 => new 技能卷轴(),
                    (long)SpecialItemID.智慧之果 => new 智慧之果(),
                    (long)SpecialItemID.奥术符文 => new 奥术符文(),
                    (long)SpecialItemID.混沌之核 => new 混沌之核(),
                    (long)ConsumableID.小回复药 => new 小回复药(),
                    (long)ConsumableID.中回复药 => new 中回复药(),
                    (long)ConsumableID.大回复药 => new 大回复药(),
                    (long)ConsumableID.魔力填充剂1 => new 魔力填充剂1(),
                    (long)ConsumableID.魔力填充剂2 => new 魔力填充剂2(),
                    (long)ConsumableID.魔力填充剂3 => new 魔力填充剂3(),
                    (long)ConsumableID.能量饮料1 => new 能量饮料1(),
                    (long)ConsumableID.能量饮料2 => new 能量饮料2(),
                    (long)ConsumableID.能量饮料3 => new 能量饮料3(),
                    _ => null,
                };
            };
        }
    }
}
