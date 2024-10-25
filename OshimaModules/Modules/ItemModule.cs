using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
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
                    (long)AccessoryID.攻击之爪10 => new 攻击之爪10(),
                    (long)AccessoryID.攻击之爪30 => new 攻击之爪30(),
                    (long)AccessoryID.攻击之爪50 => new 攻击之爪50(),
                    _ => null,
                };
            };
        }
    }
}
