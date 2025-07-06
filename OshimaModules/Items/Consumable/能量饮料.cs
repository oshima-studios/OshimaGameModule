using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.ItemEffects;
using Oshima.FunGame.OshimaModules.Skills;

namespace Oshima.FunGame.OshimaModules.Items
{
    public class 能量饮料
    {
        public interface EPAdd
        {
            public double EP { get; set; }
        }

        public static void Init(Item item, double ep, int remainUseTimes = 1)
        {
            item.Skills.Active = new 能量饮料技能(item, ep);
            item.RemainUseTimes = remainUseTimes;
            item.IsReduceTimesAfterUse = true;
            item.IsRemoveAfterUse = true;
        }

        public static string UseItem(User user, Item item, Character character)
        {
            if (item.Skills.Active != null)
            {
                item.Skills.Active.OnSkillCasted(user, [character]);
                string msg = $"对角色 [ {character} ] 使用 [ {item.Name} ] 成功！";
                if (item is EPAdd hpBook)
                {
                    msg += $"获得了 {hpBook.EP} 点能量值！";
                }
                return msg;
            }
            return "此物品没有主动技能，无法被使用！";
        }

        public static bool OnItemUsed(User user, Item item, int times, Dictionary<string, object> args)
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
            string truemsg = $"对角色 [ {targets[0]} ] 使用 {times} 个 [ {item.Name} ] 成功！";
            if (item is EPAdd expBook)
            {
                truemsg += $"获得了 {expBook.EP * times} 点能量值！";
            }
            args["truemsg"] = truemsg;
            return result;
        }
    }

    public class 能量饮料1 : Item, 能量饮料.EPAdd
    {
        public override long Id => (long)ConsumableID.能量饮料1;
        public override string Name => "能量饮料";
        public override string Description => Skills.Active?.Description ?? "";
        public override QualityType QualityType => QualityType.Green;
        public double EP { get; set; } = 50;

        public 能量饮料1(User? user = null, int remainUseTimes = 1) : base(ItemType.Consumable)
        {
            User = user;
            能量饮料.Init(this, EP, remainUseTimes);
        }

        protected override bool OnItemUsed(User user, int times, Dictionary<string, object> args)
        {
            return 能量饮料.OnItemUsed(user, this, times, args);
        }
    }

    public class 能量饮料2 : Item, 能量饮料.EPAdd
    {
        public override long Id => (long)ConsumableID.能量饮料2;
        public override string Name => "能量饮料 Pro";
        public override string Description => Skills.Active?.Description ?? "";
        public override QualityType QualityType => QualityType.Blue;
        public double EP { get; set; } = 100;

        public 能量饮料2(User? user = null, int remainUseTimes = 1) : base(ItemType.Consumable)
        {
            User = user;
            能量饮料.Init(this, EP, remainUseTimes);
        }

        protected override bool OnItemUsed(User user, int times, Dictionary<string, object> args)
        {
            return 能量饮料.OnItemUsed(user, this, times, args);
        }
    }

    public class 能量饮料3 : Item, 能量饮料.EPAdd
    {
        public override long Id => (long)ConsumableID.能量饮料3;
        public override string Name => "能量饮料 Pro Max";
        public override string Description => Skills.Active?.Description ?? "";
        public override QualityType QualityType => QualityType.Purple;
        public double EP { get; set; } = 200;

        public 能量饮料3(User? user = null, int remainUseTimes = 1) : base(ItemType.Consumable)
        {
            User = user;
            能量饮料.Init(this, EP, remainUseTimes);
        }

        protected override bool OnItemUsed(User user, int times, Dictionary<string, object> args)
        {
            return 能量饮料.OnItemUsed(user, this, times, args);
        }
    }

    public class 能量饮料技能 : Skill
    {
        public override long Id => (long)ItemActiveID.能量饮料;
        public override string Name => "能量饮料";
        public override string Description => string.Join("", Effects.Select(e => e.Description));
        public override bool CanSelectSelf => true;
        public override bool CanSelectTeammate => true;
        public override bool CanSelectEnemy => false;
        public override int CanSelectTargetCount => 1;

        public 能量饮料技能(Item? item = null, double ep = 0) : base(SkillType.Item)
        {
            Level = 1;
            Item = item;
            Effects.Add(new GetEP(this, new()
            {
                { "ep", ep }
            }));
        }
    }
}
