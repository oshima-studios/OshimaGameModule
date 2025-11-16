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
                    (long)AccessoryID.攻击之爪70 => new 攻击之爪70(),
                    (long)AccessoryID.攻击之爪85 => new 攻击之爪85(),
                    (long)AccessoryID.攻击之爪100 => new 攻击之爪100(),
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
                    (long)SpecialItemID.青松 => new 青松(),
                    (long)SpecialItemID.流星石 => new 流星石(),
                    (long)SpecialItemID.向日葵 => new 向日葵(),
                    (long)SpecialItemID.金铃花 => new 金铃花(),
                    (long)SpecialItemID.琉璃珠 => new 琉璃珠(),
                    (long)SpecialItemID.鸣草 => new 鸣草(),
                    (long)SpecialItemID.马尾 => new 马尾(),
                    (long)SpecialItemID.鬼兜虫 => new 鬼兜虫(),
                    (long)SpecialItemID.烈焰花花蕊 => new 烈焰花花蕊(),
                    (long)SpecialItemID.堇瓜 => new 堇瓜(),
                    (long)SpecialItemID.水晶球 => new 水晶球(),
                    (long)SpecialItemID.薰衣草 => new 薰衣草(),
                    (long)SpecialItemID.青石 => new 青石(),
                    (long)SpecialItemID.莲花 => new 莲花(),
                    (long)SpecialItemID.陶罐 => new 陶罐(),
                    (long)SpecialItemID.海灵芝 => new 海灵芝(),
                    (long)SpecialItemID.四叶草 => new 四叶草(),
                    (long)SpecialItemID.露珠 => new 露珠(),
                    (long)SpecialItemID.茉莉花 => new 茉莉花(),
                    (long)SpecialItemID.绿萝 => new 绿萝(),
                    (long)SpecialItemID.檀木扇 => new 檀木扇(),
                    (long)SpecialItemID.鸟蛋 => new 鸟蛋(),
                    (long)SpecialItemID.竹笋 => new 竹笋(),
                    (long)SpecialItemID.晶核 => new 晶核(),
                    (long)SpecialItemID.手工围巾 => new 手工围巾(),
                    (long)SpecialItemID.柳条篮 => new 柳条篮(),
                    (long)SpecialItemID.风筝 => new 风筝(),
                    (long)SpecialItemID.羽毛 => new 羽毛(),
                    (long)SpecialItemID.发光髓 => new 发光髓(),
                    (long)SpecialItemID.紫罗兰 => new 紫罗兰(),
                    (long)SpecialItemID.松果 => new 松果(),
                    (long)SpecialItemID.电气水晶 => new 电气水晶(),
                    (long)SpecialItemID.薄荷 => new 薄荷(),
                    (long)SpecialItemID.竹节 => new 竹节(),
                    (long)SpecialItemID.铁砧 => new 铁砧(),
                    (long)SpecialItemID.冰雾花 => new 冰雾花(),
                    (long)SpecialItemID.海草 => new 海草(),
                    (long)SpecialItemID.磐石 => new 磐石(),
                    (long)SpecialItemID.砂砾 => new 砂砾(),
                    (long)SpecialItemID.铁甲贝壳 => new 铁甲贝壳(),
                    (long)SpecialItemID.蜥蜴尾巴 => new 蜥蜴尾巴(),
                    (long)SpecialItemID.古老钟摆 => new 古老钟摆(),
                    (long)SpecialItemID.枯藤 => new 枯藤(),
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
