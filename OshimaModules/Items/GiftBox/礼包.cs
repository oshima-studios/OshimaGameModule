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

        public static bool OnItemUsed(Item item, Dictionary<string, object> args)
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

        protected override bool OnItemUsed(Dictionary<string, object> args)
        {
            return 礼包.OnItemUsed(this, args);
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

        protected override bool OnItemUsed(Dictionary<string, object> args)
        {
            return 礼包.OnItemUsed(this, args);
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

        protected override bool OnItemUsed(Dictionary<string, object> args)
        {
            return 礼包.OnItemUsed(this, args);
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
