using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.ItemEffects;
using Oshima.FunGame.OshimaModules.Skills;

namespace Oshima.FunGame.OshimaModules.Items
{
    public class 回复药
    {
        public interface HPRecovery
        {
            public double HP { get; set; }
        }

        public static void Init(Item item, double exp, int remainUseTimes = 1)
        {
            item.Skills.Active = new 回复药技能(item, exp);
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
                if (item is HPRecovery hpBook)
                {
                    msg += $"回复了 {hpBook.HP} 点生命值！";
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
                if (item is HPRecovery expBook)
                {
                    truemsg += $"回复了 {expBook.HP * count} 点生命值！";
                }
                args["truemsg"] = truemsg;
            }
            return result;
        }
    }

    public class 小回复药 : Item, 回复药.HPRecovery
    {
        public override long Id => (long)ConsumableID.小回复药;
        public override string Name => "小回复药";
        public override string Description => Skills.Active?.Description ?? "";
        public override QualityType QualityType => QualityType.White;
        public double HP { get; set; } = 600;

        public 小回复药(User? user = null, int remainUseTimes = 1) : base(ItemType.Consumable)
        {
            User = user;
            回复药.Init(this, HP, remainUseTimes);
        }

        protected override bool OnItemUsed(User user, Dictionary<string, object> args)
        {
            return 回复药.OnItemUsed(user, this, args);
        }
    }

    public class 中回复药 : Item, 回复药.HPRecovery
    {
        public override long Id => (long)ConsumableID.中回复药;
        public override string Name => "中回复药";
        public override string Description => Skills.Active?.Description ?? "";
        public override QualityType QualityType => QualityType.Green;
        public double HP { get; set; } = 1800;

        public 中回复药(User? user = null, int remainUseTimes = 1) : base(ItemType.Consumable)
        {
            User = user;
            回复药.Init(this, HP, remainUseTimes);
        }

        protected override bool OnItemUsed(User user, Dictionary<string, object> args)
        {
            return 回复药.OnItemUsed(user, this, args);
        }
    }

    public class 大回复药 : Item, 回复药.HPRecovery
    {
        public override long Id => (long)ConsumableID.大回复药;
        public override string Name => "大回复药";
        public override string Description => Skills.Active?.Description ?? "";
        public override QualityType QualityType => QualityType.Blue;
        public double HP { get; set; } = 3000;

        public 大回复药(User? user = null, int remainUseTimes = 1) : base(ItemType.Consumable)
        {
            User = user;
            回复药.Init(this, HP, remainUseTimes);
        }

        protected override bool OnItemUsed(User user, Dictionary<string, object> args)
        {
            return 回复药.OnItemUsed(user, this, args);
        }
    }

    public class 回复药技能 : Skill
    {
        public override long Id => (long)ItemActiveID.回复药;
        public override string Name => "回复药";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";

        public 回复药技能(Item? item = null, double hp = 0) : base(SkillType.Item)
        {
            Level = 1;
            Item = item;
            Effects.Add(new RecoverHP(this, new()
            {
                { "hp", hp }
            }));
        }
    }
}
