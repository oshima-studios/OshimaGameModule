using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 咒怨洪流 : Skill
    {
        public override long Id => (long)SuperSkillID.咒怨洪流;
        public override string Name => "咒怨洪流";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";
        public override double EPCost => 100;
        public override double CD => 65;
        public override double HardnessTime { get; set; } = 3;
        public override bool CanSelectSelf => true;
        public override bool CanSelectEnemy => false;

        public 咒怨洪流(Character? character = null) : base(SkillType.SuperSkill, character)
        {
            Effects.Add(new 咒怨洪流特效(this));
        }
    }

    public class 咒怨洪流特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => "咒怨洪流";
        public override string Description => $"{Skill.SkillOwner()}将「熵核」的污染之力附着在武器之上。{Duration:0.##} {GameplayEquilibriumConstant.InGameTime}内，增加自身所有伤害的 {减伤比例 * 100:0.##}% 伤害减免；[ 蚀魂震击 ] 的冷却时间降低至 5 {GameplayEquilibriumConstant.InGameTime}，并将普通攻击转为魔法伤害；在一个回合里首次使用普通攻击后，附赠 {额外攻击次数} 次普通攻击决策点配额，但后续普通攻击必须选取不重复的目标。";
        public override bool Durative => true;
        public override double Duration => 30;
        public override DispelledType DispelledType => DispelledType.CannotBeDispelled;

        private double 减伤比例 => 0.15 + 0.02 * (Level - 1);
        private double 实际比例 = 0;
        private readonly List<Character> 本回合已攻击的目标 = [];
        private bool 本回合可附赠动作 = true;
        private readonly int 额外攻击次数 = 2;

        public override void OnEffectGained(Character character)
        {
            实际比例 = 减伤比例;
            character.NormalAttack.SetMagicType(new(this, true, MagicType.None, 999), GamingQueue);
            if (character.Effects.Where(e => e is 蚀魂震击特效).FirstOrDefault() is 蚀魂震击特效 e)
            {
                e.基础冷却时间 = 5;
                if (e.冷却时间 > e.基础冷却时间) e.冷却时间 = e.基础冷却时间;
            }
        }

        public override void OnEffectLost(Character character)
        {
            实际比例 = 0;
            character.NormalAttack.UnsetMagicType(this, GamingQueue);
            if (character.Effects.Where(e => e is 蚀魂震击特效).FirstOrDefault() is 蚀魂震击特效 e)
            {
                e.基础冷却时间 = 10;
            }
        }

        public override void OnTurnStart(Character character, List<Character> enemys, List<Character> teammates, List<Skill> skills, List<Item> items)
        {
            本回合已攻击的目标.Clear();
            本回合可附赠动作 = true;
        }

        public override void AfterCharacterNormalAttack(Character character, NormalAttack normalAttack, List<Character> targets)
        {
            本回合已攻击的目标.AddRange(targets);
            if (本回合可附赠动作 && GamingQueue != null && GamingQueue.CharacterDecisionPoints.TryGetValue(character, out DecisionPoints? dp) && dp != null)
            {
                本回合可附赠动作 = false;
                dp.AddTempActionQuota(CharacterActionType.NormalAttack, 额外攻击次数);
                WriteLine($"[ {character} ] 发动了{nameof(咒怨洪流)}！本回合可额外发动 {额外攻击次数} 次普通攻击！");
            }
        }

        public override void AlterSelectListBeforeSelection(Character character, ISkill skill, List<Character> enemys, List<Character> teammates)
        {
            if (skill is NormalAttack && !本回合可附赠动作)
            {
                enemys.RemoveAll(本回合已攻击的目标.Contains);
            }
        }

        public override double AlterActualDamageAfterCalculation(Character character, Character enemy, double damage, bool isNormalAttack, DamageType damageType, MagicType magicType, DamageResult damageResult, ref bool isEvaded, Dictionary<Effect, double> totalDamageBonus)
        {
            if (enemy == Skill.Character)
            {
                return -(damage * 实际比例);
            }
            return 0;
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            RemainDuration = Duration;
            if (!caster.Effects.Contains(this))
            {
                caster.Effects.Add(this);
                OnEffectGained(caster);
            }
            GamingQueue?.LastRound.AddApplyEffects(caster, EffectType.DefenseBoost);
        }
    }
}
