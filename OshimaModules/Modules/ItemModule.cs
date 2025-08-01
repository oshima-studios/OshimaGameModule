﻿using Milimoe.FunGame.Core.Api.Utility;
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
        public Dictionary<string, Item> KnownItems { get; } = [];

        public override Dictionary<string, Item> Items
        {
            get
            {
                Dictionary<string, Item> items = Factory.GetGameModuleInstances<Item>(OshimaGameModuleConstant.General, OshimaGameModuleConstant.Item);
                if (KnownItems.Count == 0 && items.Count > 0)
                {
                    foreach (string key in items.Keys)
                    {
                        KnownItems[key] = items[key];
                    }
                }
                return items;
            }
        }

        protected override Factory.EntityFactoryDelegate<Item> ItemFactory()
        {
            return (id, name, args) =>
            {
                return id switch
                {
                    (long)AccessoryID.攻击之爪10 => new 攻击之爪10(),
                    (long)AccessoryID.攻击之爪25 => new 攻击之爪25(),
                    (long)AccessoryID.攻击之爪40 => new 攻击之爪40(),
                    (long)AccessoryID.攻击之爪55 => new 攻击之爪55(),
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
                    (long)SpecialItemID.奖券 => new 奖券(),
                    (long)SpecialItemID.十连奖券 => new 十连奖券(),
                    (long)SpecialItemID.改名卡 => new 改名卡(),
                    (long)SpecialItemID.原初之印 => new 原初之印(),
                    (long)SpecialItemID.创生之印 => new 创生之印(),
                    (long)SpecialItemID.法则精粹 => new 法则精粹(),
                    (long)SpecialItemID.大师锻造券 => new 大师锻造券(),
                    (long)SpecialItemID.钻石 => new 钻石(),
                    (long)SpecialItemID.探索许可 => new 探索许可(),
                    (long)ConsumableID.小回复药 => new 小回复药(),
                    (long)ConsumableID.中回复药 => new 中回复药(),
                    (long)ConsumableID.大回复药 => new 大回复药(),
                    (long)ConsumableID.魔力填充剂1 => new 魔力填充剂1(),
                    (long)ConsumableID.魔力填充剂2 => new 魔力填充剂2(),
                    (long)ConsumableID.魔力填充剂3 => new 魔力填充剂3(),
                    (long)ConsumableID.能量饮料1 => new 能量饮料1(),
                    (long)ConsumableID.能量饮料2 => new 能量饮料2(),
                    (long)ConsumableID.能量饮料3 => new 能量饮料3(),
                    (long)ConsumableID.复苏药1 => new 复苏药1(),
                    (long)ConsumableID.复苏药2 => new 复苏药2(),
                    (long)ConsumableID.复苏药3 => new 复苏药3(),
                    (long)ConsumableID.全回复药 => new 全回复药(),
                    (long)GiftBoxID.年夜饭 => new 年夜饭(),
                    (long)GiftBoxID.蛇年大吉 => new 蛇年大吉(),
                    (long)GiftBoxID.新春快乐 => new 新春快乐(),
                    (long)GiftBoxID.毕业礼包 => new 毕业礼包(),
                    (long)GiftBoxID.魔法卡礼包 => new 魔法卡礼包(),
                    (long)GiftBoxID.探索助力礼包 => new 探索助力礼包(),
                    _ => null,
                };
            };
        }
    }
}
