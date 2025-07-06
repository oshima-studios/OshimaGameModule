using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.ItemEffects;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;
using Oshima.FunGame.OshimaModules.Skills;

namespace Oshima.FunGame.OshimaModules.Items
{
    public class 复苏药
    {
        public interface HPRecovery
        {
            public double HP { get; set; }
        }

        public static void Init(Item item, double hp, int remainUseTimes = 1, bool isPercentage = false)
        {
            item.Skills.Active = new 复苏药技能(item, hp, isPercentage);
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
                if (item is HPRecovery hpBook)
                {
                    msg += $"回复了 {hpBook.HP} 点生命值！";
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
            if (item is HPRecovery expBook)
            {
                truemsg += $"回复了 {expBook.HP * times} 点生命值！";
            }
            args["truemsg"] = truemsg;
            return result;
        }
    }

    public class 复苏药1 : Item, 复苏药.HPRecovery
    {
        public override long Id => (long)ConsumableID.复苏药1;
        public override string Name => "复苏药";
        public override string Description => Skills.Active?.Description ?? "";
        public override QualityType QualityType => QualityType.Green;
        public double HP { get; set; } = 500;

        public 复苏药1(User? user = null, int remainUseTimes = 1) : base(ItemType.Consumable)
        {
            User = user;
            复苏药.Init(this, HP, remainUseTimes);
        }

        protected override bool OnItemUsed(User user, int times, Dictionary<string, object> args)
        {
            return 复苏药.OnItemUsed(user, this, times, args);
        }
    }

    public class 复苏药2 : Item, 复苏药.HPRecovery
    {
        public override long Id => (long)ConsumableID.复苏药2;
        public override string Name => "复苏药·改";
        public override string Description => Skills.Active?.Description ?? "";
        public override QualityType QualityType => QualityType.Blue;
        public double HP { get; set; } = 2500;

        public 复苏药2(User? user = null, int remainUseTimes = 1) : base(ItemType.Consumable)
        {
            User = user;
            复苏药.Init(this, HP, remainUseTimes);
        }

        protected override bool OnItemUsed(User user, int times, Dictionary<string, object> args)
        {
            return 复苏药.OnItemUsed(user, this, times, args);
        }
    }

    public class 复苏药3 : Item, 复苏药.HPRecovery
    {
        public override long Id => (long)ConsumableID.复苏药3;
        public override string Name => "复苏药·全";
        public override string Description => Skills.Active?.Description ?? "";
        public override QualityType QualityType => QualityType.Purple;
        public double HP { get; set; } = 1;

        public 复苏药3(User? user = null, int remainUseTimes = 1) : base(ItemType.Consumable)
        {
            User = user;
            复苏药.Init(this, HP, remainUseTimes, true);
        }

        protected override bool OnItemUsed(User user, int times, Dictionary<string, object> args)
        {
            return 复苏药.OnItemUsed(user, this, times, args);
        }
    }

    public class 复苏药技能 : Skill
    {
        public override long Id => (long)ItemActiveID.复苏药;
        public override string Name => "复苏药";
        public override string Description => string.Join("", Effects.Select(e => e.Description));
        public override bool SelectAllTeammates => _canSelectAllTeammates;
        public override bool SelectAllEnemies => _canSelectAllEnemies;
        public override bool CanSelectSelf => _canSelectSelf;
        public override bool CanSelectTeammate => _canSelectTeammate;
        public override bool CanSelectEnemy => _canSelectEnemy;
        public override int CanSelectTargetCount => _canSelectCount;

        private readonly bool _canSelectAllTeammates;
        private readonly bool _canSelectAllEnemies;
        private readonly bool _canSelectSelf;
        private readonly bool _canSelectTeammate;
        private readonly bool _canSelectEnemy;
        private readonly int _canSelectCount;

        public 复苏药技能(Item? item = null, double hp = 0, bool isPercentage = false, bool canSelectAllTeammates = false, bool canSelectAllEnemies = false, bool canSelectSelf = true, bool canSelectTeammate = true, bool canSelectEnemy = false, int canSelectCount = 1) : base(SkillType.Item)
        {
            Level = 1;
            Item = item;
            _canSelectAllTeammates = canSelectAllTeammates;
            _canSelectAllEnemies = canSelectAllEnemies;
            _canSelectSelf = canSelectSelf;
            _canSelectTeammate = canSelectTeammate;
            _canSelectEnemy = canSelectEnemy;
            _canSelectCount = canSelectCount;
            Effects.Add(new 强驱散特效(this));
            if (!isPercentage)
            {
                Effects.Add(new RecoverHP(this, new()
                {
                    { "hp", hp },
                    { "respawn", true }
                }));
            }
            else
            {
                Effects.Add(new RecoverHP2(this, new()
                {
                    { "hp", hp },
                    { "respawn", true }
                }));
            }
        }
    }
}
