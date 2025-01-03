using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.ItemEffects;
using Oshima.FunGame.OshimaModules.Skills;

namespace Oshima.FunGame.OshimaModules.Items
{
    public class 能量饮料
    {
        public interface EPBook
        {
            public double EP { get; set; }
        }

        public static void Init(Item item, double exp, int remainUseTimes = 1)
        {
            item.Skills.Active = new 能量饮料技能(item, exp);
            item.RemainUseTimes = remainUseTimes;
            item.IsInGameItem = false;
            item.IsReduceTimesAfterUse = true;
            item.IsRemoveAfterUse = true;
        }

        public static string UseItem(Item item, Character character)
        {
            if (item.Skills.Active != null)
            {
                item.Skills.Active.OnSkillCasted([character]);
                string msg = $"对角色 [ {character} ] 使用 [ {item.Name} ] 成功！";
                if (item is EPBook hpBook)
                {
                    msg += $"回复了 {hpBook.EP} 点能量值！";
                }
                return msg;
            }
            return "此物品没有主动技能，无法被使用！";
        }

        public static bool OnItemUsed(Item item, Dictionary<string, object> args)
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
                    msg = UseItem(item, temp[0]);
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
                if (item is EPBook expBook)
                {
                    truemsg += $"回复了 {expBook.EP * count} 点能量值！";
                }
                args["truemsg"] = truemsg;
            }
            return result;
        }
    }

    public class 能量饮料1 : Item, 能量饮料.EPBook
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

        protected override bool OnItemUsed(Dictionary<string, object> args)
        {
            return 能量饮料.OnItemUsed(this, args);
        }
    }

    public class 能量饮料2 : Item, 能量饮料.EPBook
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

        protected override bool OnItemUsed(Dictionary<string, object> args)
        {
            return 能量饮料.OnItemUsed(this, args);
        }
    }
    
    public class 能量饮料3 : Item, 能量饮料.EPBook
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

        protected override bool OnItemUsed(Dictionary<string, object> args)
        {
            return 能量饮料.OnItemUsed(this, args);
        }
    }

    public class 能量饮料技能 : Skill
    {
        public override long Id => (long)ItemActiveID.能量饮料;
        public override string Name => "能量饮料";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";

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
