using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Items;

namespace Oshima.FunGame.OshimaModules
{
    public class ItemModule : Milimoe.FunGame.Core.Library.Common.Addon.ItemModule
    {
        public override string Name => OshimaGameModuleConstant.Item;
        public override string Description => OshimaGameModuleConstant.Description;
        public override string Version => OshimaGameModuleConstant.Version;
        public override string Author => OshimaGameModuleConstant.Author;

        public override List<Item> Items
        {
            get
            {
                EntityModuleConfig<Item> config = new(OshimaGameModuleConstant.General, OshimaGameModuleConstant.Item);
                config.LoadConfig();
                foreach (string key in config.Keys)
                {
                    Item prev = config[key];
                    Item? next = GetItem(prev.Id, prev.Name, prev.ItemType);
                    if (next != null)
                    {
                        prev.SetPropertyToItemModuleNew(next);
                        config[key] = next;
                    }
                }
                return [.. config.Values];
            }
        }

        public override Item? GetItem(long id, string name, ItemType type)
        {
            if (type == ItemType.MagicCardPack)
            {

            }

            if (type == ItemType.Accessory)
            {
                switch ((AccessoryID)id)
                {
                    case AccessoryID.攻击之爪10:
                        return new 攻击之爪10();
                    case AccessoryID.攻击之爪30:
                        return new 攻击之爪30();
                    case AccessoryID.攻击之爪50:
                        return new 攻击之爪50();
                }
            }

            return null;
        }
    }
}
