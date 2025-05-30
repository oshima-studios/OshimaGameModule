using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.ItemEffects;
using Oshima.FunGame.OshimaModules.Skills;

namespace Oshima.FunGame.OshimaModules.Items
{
    public class 魔力填充剂
    {
        public interface MPRecovery
        {
            public double MP { get; set; }
        }

        public static void Init(Item item, double exp, int remainUseTimes = 1)
        {
            item.Skills.Active = new 魔力填充剂技能(item, exp);
            item.RemainUseTimes = remainUseTimes;
            item.IsInGameItem = false;
            item.IsReduceTimesAfterUse = true;
            item.IsRemoveAfterUse = true;
        }

        public static string UseItem(User user, Item item, Character character)
        {
            if (item.Skills.Active != null)
            {
                item.Skills.Active.OnSkillCasted(user, [character]);
                string msg = $"对角色 [ {character} ] 使用 [ {item.Name} ] 成功！";
                if (item is MPRecovery hpBook)
                {
                    msg += $"回复了 {hpBook.MP} 点魔法值！";
                }
                return msg;
            }
            return "此物品没有主动技能，无法被使用！";
        }

        public static bool OnItemUsed(User user, Item item, Dictionary<string, object> args)
        {
            string msg = "";
            bool result = false;
            Character[] targets = [];
            string key = args.Keys.FirstOrDefault(s => s.Equals("targets", StringComparison.CurrentCultureIgnoreCase)) ?? "";
            if (key != "" && args.TryGetValue(key, out object? value) && value is Character[] temp)
            {
                if (temp.Length > 0)
                {
                    targets = [temp[0]];
                    msg = UseItem(user, item, temp[0]);
                    result = true;
                }
                else
                {
                    msg = $"使用物品失败，没有作用目标！";
                }
            }
            args["msg"] = msg;
            key = args.Keys.FirstOrDefault(s => s.Equals("useCount", StringComparison.CurrentCultureIgnoreCase)) ?? "";
            if (key != "" && args.TryGetValue(key, out value) && value is int count && targets.Length > 0)
            {
                string truemsg = $"对角色 [ {targets[0]} ] 使用 {count} 个 [ {item.Name} ] 成功！";
                if (item is MPRecovery expBook)
                {
                    truemsg += $"回复了 {expBook.MP * count} 点魔法值！";
                }
                args["truemsg"] = truemsg;
            }
            return result;
        }
    }

    public class 魔力填充剂1 : Item, 魔力填充剂.MPRecovery
    {
        public override long Id => (long)ConsumableID.魔力填充剂1;
        public override string Name => "魔力填充剂Ⅰ型";
        public override string Description => Skills.Active?.Description ?? "";
        public override QualityType QualityType => QualityType.White;
        public double MP { get; set; } = 300;

        public 魔力填充剂1(User? user = null, int remainUseTimes = 1) : base(ItemType.Consumable)
        {
            User = user;
            魔力填充剂.Init(this, MP, remainUseTimes);
        }

        protected override bool OnItemUsed(User user, Dictionary<string, object> args)
        {
            return 魔力填充剂.OnItemUsed(user, this, args);
        }
    }

    public class 魔力填充剂2 : Item, 魔力填充剂.MPRecovery
    {
        public override long Id => (long)ConsumableID.魔力填充剂2;
        public override string Name => "魔力填充剂Ⅱ型";
        public override string Description => Skills.Active?.Description ?? "";
        public override QualityType QualityType => QualityType.Green;
        public double MP { get; set; } = 700;

        public 魔力填充剂2(User? user = null, int remainUseTimes = 1) : base(ItemType.Consumable)
        {
            User = user;
            魔力填充剂.Init(this, MP, remainUseTimes);
        }

        protected override bool OnItemUsed(User user, Dictionary<string, object> args)
        {
            return 魔力填充剂.OnItemUsed(user, this, args);
        }
    }

    public class 魔力填充剂3 : Item, 魔力填充剂.MPRecovery
    {
        public override long Id => (long)ConsumableID.魔力填充剂3;
        public override string Name => "魔力填充剂Ⅲ型";
        public override string Description => Skills.Active?.Description ?? "";
        public override QualityType QualityType => QualityType.Blue;
        public double MP { get; set; } = 1500;

        public 魔力填充剂3(User? user = null, int remainUseTimes = 1) : base(ItemType.Consumable)
        {
            User = user;
            魔力填充剂.Init(this, MP, remainUseTimes);
        }

        protected override bool OnItemUsed(User user, Dictionary<string, object> args)
        {
            return 魔力填充剂.OnItemUsed(user, this, args);
        }
    }

    public class 魔力填充剂技能 : Skill
    {
        public override long Id => (long)ItemActiveID.魔力填充剂;
        public override string Name => "魔力填充剂";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";

        public 魔力填充剂技能(Item? item = null, double mp = 0) : base(SkillType.Item)
        {
            Level = 1;
            Item = item;
            Effects.Add(new RecoverMP(this, new()
            {
                { "mp", mp }
            }));
        }
    }
}
