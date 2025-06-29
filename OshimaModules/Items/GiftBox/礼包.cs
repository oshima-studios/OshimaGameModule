using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Skills;

namespace Oshima.FunGame.OshimaModules.Items
{
    public class 礼包
    {
        public interface GiftBox
        {
            public Dictionary<string, int> Gifts { get; set; }
        }

        public static void Init(Item item, Dictionary<string, int> gifts, int remainUseTimes = 1)
        {
            if (item is GiftBox box)
            {
                box.Gifts = gifts;
            }
            item.Skills.Active = new 礼包技能(item);
            item.RemainUseTimes = remainUseTimes;
            item.IsInGameItem = false;
            item.IsReduceTimesAfterUse = true;
            item.IsRemoveAfterUse = true;
        }

        public static bool OnItemUsed(User user, Item item, Dictionary<string, object> args)
        {
            string msg = "";
            if (item is GiftBox box)
            {

            }
            args["msg"] = msg;
            return msg.Trim() != "";
        }
    }

    public class 年夜饭 : Item, 礼包.GiftBox
    {
        public override long Id => (long)GiftBoxID.年夜饭;
        public override string Name => "年夜饭";
        public override string Description => Skills.Active?.Description ?? "";
        public override QualityType QualityType => QualityType.White;
        public Dictionary<string, int> Gifts { get; set; } = [];

        public 年夜饭(User? user = null, int remainUseTimes = 1) : base(ItemType.GiftBox)
        {
            User = user;
            礼包.Init(this, new()
            {
                { General.GameplayEquilibriumConstant.InGameCurrency, 100000 },
                { General.GameplayEquilibriumConstant.InGameMaterial, 2000 },
                { new 技能卷轴().Name, 20 },
                { new 升华之印().Name, 20 },
                { new 流光之印().Name, 10 },
                { new 大经验书().Name, 20 },
                { new 大回复药().Name, 5 },
                { new 魔力填充剂3().Name, 5 },
                { new 能量饮料3().Name, 5 }
            }, remainUseTimes);
        }

        protected override bool OnItemUsed(User user, Dictionary<string, object> args)
        {
            return 礼包.OnItemUsed(user, this, args);
        }
    }

    public class 蛇年大吉 : Item, 礼包.GiftBox
    {
        public override long Id => (long)GiftBoxID.蛇年大吉;
        public override string Name => "蛇年大吉";
        public override string Description => Skills.Active?.Description ?? "";
        public override QualityType QualityType => QualityType.White;
        public Dictionary<string, int> Gifts { get; set; } = [];

        public 蛇年大吉(User? user = null, int remainUseTimes = 1) : base(ItemType.GiftBox)
        {
            User = user;
            礼包.Init(this, new()
            {
                { General.GameplayEquilibriumConstant.InGameCurrency, 88888 },
                { General.GameplayEquilibriumConstant.InGameMaterial, 888 },
                { new 技能卷轴().Name, 20 },
                { new 智慧之果().Name, 10 },
                { new 奥术符文().Name, 5 },
                { new 混沌之核().Name, 3 },
                { new 升华之印().Name, 20 },
                { new 流光之印().Name, 10 },
                { new 永恒之印().Name, 3 },
                { new 大经验书().Name, 20 }
            }, remainUseTimes);
        }

        protected override bool OnItemUsed(User user, Dictionary<string, object> args)
        {
            return 礼包.OnItemUsed(user, this, args);
        }
    }

    public class 新春快乐 : Item, 礼包.GiftBox
    {
        public override long Id => (long)GiftBoxID.新春快乐;
        public override string Name => "新春快乐";
        public override string Description => Skills.Active?.Description ?? "";
        public override QualityType QualityType => QualityType.White;
        public Dictionary<string, int> Gifts { get; set; } = [];

        public 新春快乐(User? user = null, int remainUseTimes = 1) : base(ItemType.GiftBox)
        {
            User = user;
            礼包.Init(this, new()
            {
                { General.GameplayEquilibriumConstant.InGameCurrency, 100000 },
                { General.GameplayEquilibriumConstant.InGameMaterial, 2000 },
                { new 技能卷轴().Name, 20 },
                { new 升华之印().Name, 20 },
                { new 流光之印().Name, 10 },
                { new 大经验书().Name, 20 },
                { new 大回复药().Name, 5 },
                { new 魔力填充剂3().Name, 5 },
                { new 能量饮料3().Name, 5 }
            }, remainUseTimes);
        }

        protected override bool OnItemUsed(User user, Dictionary<string, object> args)
        {
            return 礼包.OnItemUsed(user, this, args);
        }
    }

    public class 毕业礼包 : Item, 礼包.GiftBox
    {
        public override long Id => (long)GiftBoxID.毕业礼包;
        public override string Name => "毕业礼包";
        public override string Description => Skills.Active?.Description ?? "";
        public override QualityType QualityType => QualityType.Gold;
        public Dictionary<string, int> Gifts { get; set; } = [];

        public 毕业礼包(User? user = null, int remainUseTimes = 1) : base(ItemType.GiftBox)
        {
            User = user;
            礼包.Init(this, new()
            {
                { General.GameplayEquilibriumConstant.InGameCurrency, 294000 },
                { General.GameplayEquilibriumConstant.InGameMaterial, 2660 },
                { new 升华之印().Name, 49 },
                { new 流光之印().Name, 21 },
                { new 永恒之印().Name, 6 },
                { new 技能卷轴().Name, 78 },
                { new 智慧之果().Name, 35 },
                { new 奥术符文().Name, 10 },
                { new 混沌之核().Name, 2 },
                { new 大经验书().Name, 164 }
            }, remainUseTimes);
        }
    }

    public class 魔法卡礼包 : Item, 礼包.GiftBox
    {
        public override long Id => (long)GiftBoxID.魔法卡礼包;
        public override string Name => "魔法卡礼包";
        public override string Description => Skills.Active?.Description ?? "";
        public int Count { get; set; } = 1;
        public Dictionary<string, int> Gifts { get; set; } = [];
        private const string GiftName = "与礼包同品质、随机属性、随机魔法技能的魔法卡";

        public 魔法卡礼包(QualityType type = QualityType.White, int count = 1, User? user = null, int remainUseTimes = 1) : base(ItemType.GiftBox)
        {
            QualityType = type;
            Others.Add("QualityType", (int)type);
            Count = count;
            Others.Add("Count", count);
            User = user;
            礼包.Init(this, new()
            {
                { GiftName, count }
            }, remainUseTimes);
        }

        protected override void AfterCopy()
        {
            if (Others.TryGetValue("QualityType", out object? value) && int.TryParse(value.ToString(), out int qualityType))
            {
                QualityType = (QualityType)qualityType;
            }
            if (Others.TryGetValue("Count", out value) && int.TryParse(value.ToString(), out int count))
            {
                Count = count;
                Gifts[GiftName] = count;
            }
        }
    }

    public class 礼包技能 : Skill
    {
        public override long Id => (long)ItemActiveID.礼包;
        public override string Name => "礼包";
        public override string Description
        {
            get
            {
                if (Item is 礼包.GiftBox box && box.Gifts.Count > 0)
                {
                    return "打开后可立即获得：" + string.Join("，", box.Gifts.Select(kv => $"{kv.Key} * {kv.Value}"));
                }
                return "";
            }
        }

        public 礼包技能(Item? item = null) : base(SkillType.Item)
        {
            Level = 1;
            Item = item;
        }
    }
}
