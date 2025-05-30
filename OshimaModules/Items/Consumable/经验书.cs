using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.ItemEffects;
using Oshima.FunGame.OshimaModules.Skills;

namespace Oshima.FunGame.OshimaModules.Items
{
    public class 经验书
    {
        public interface EXPBook
        {
            public double EXP { get; set; }
        }

        public static void Init(Item item, double exp, int remainUseTimes = 1)
        {
            item.Skills.Active = new 经验书技能(item, exp);
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
                if (item is EXPBook expBook)
                {
                    msg += $"获得了 {expBook.EXP} 点经验值！";
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
                if (item is EXPBook expBook)
                {
                    truemsg += $"获得了 {expBook.EXP * count} 点经验值！";
                }
                args["truemsg"] = truemsg;
            }
            return result;
        }
    }

    public class 小经验书 : Item, 经验书.EXPBook
    {
        public override long Id => (long)ConsumableID.小经验书;
        public override string Name => "小经验书";
        public override string Description => Skills.Active?.Description ?? "";
        public override QualityType QualityType => QualityType.White;
        public double EXP { get; set; } = 200;

        public 小经验书(User? user = null, int remainUseTimes = 1) : base(ItemType.Consumable)
        {
            User = user;
            经验书.Init(this, EXP, remainUseTimes);
        }

        protected override bool OnItemUsed(User user, Dictionary<string, object> args)
        {
            return 经验书.OnItemUsed(user, this, args);
        }
    }

    public class 中经验书 : Item, 经验书.EXPBook
    {
        public override long Id => (long)ConsumableID.中经验书;
        public override string Name => "中经验书";
        public override string Description => Skills.Active?.Description ?? "";
        public override QualityType QualityType => QualityType.Green;
        public double EXP { get; set; } = 500;

        public 中经验书(User? user = null, int remainUseTimes = 1) : base(ItemType.Consumable)
        {
            User = user;
            经验书.Init(this, EXP, remainUseTimes);
        }

        protected override bool OnItemUsed(User user, Dictionary<string, object> args)
        {
            return 经验书.OnItemUsed(user, this, args);
        }
    }

    public class 大经验书 : Item, 经验书.EXPBook
    {
        public override long Id => (long)ConsumableID.大经验书;
        public override string Name => "大经验书";
        public override string Description => Skills.Active?.Description ?? "";
        public override QualityType QualityType => QualityType.Blue;
        public double EXP { get; set; } = 1000;

        public 大经验书(User? user = null, int remainUseTimes = 1) : base(ItemType.Consumable)
        {
            User = user;
            经验书.Init(this, EXP, remainUseTimes);
        }

        protected override bool OnItemUsed(User user, Dictionary<string, object> args)
        {
            return 经验书.OnItemUsed(user, this, args);
        }
    }

    public class 经验书技能 : Skill
    {
        public override long Id => (long)ItemActiveID.经验书;
        public override string Name => "经验书";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";

        public 经验书技能(Item? item = null, double exp = 0) : base(SkillType.Item)
        {
            Level = 1;
            Item = item;
            Effects.Add(new GetEXP(this, new()
            {
                { "exp", exp }
            }));
        }
    }
}
