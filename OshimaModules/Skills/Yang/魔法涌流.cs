using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 魔法涌流 : Skill
    {
        public override long Id => (long)SuperSkillID.魔法涌流;
        public override string Name => "魔法涌流";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";
        public override double EPCost => 100;
        public override double CD => 35;
        public override double HardnessTime { get; set; } = 3;
        public override bool CanSelectSelf => true;
        public override bool CanSelectEnemy => false;

        public 魔法涌流(Character? character = null) : base(SkillType.SuperSkill, character)
        {
            Effects.Add(new 魔法涌流特效(this));
        }
    }

    public class 魔法涌流特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => "魔法涌流";
        public override string Description => $"{Duration:0.##} {GameplayEquilibriumConstant.InGameTime}内，增加自身所有伤害的 {减伤比例 * 100:0.##}% 伤害减免；【魔法震荡】的冷却时间降低至 5 {GameplayEquilibriumConstant.InGameTime}，并将普通攻击转为魔法伤害，允许普通攻击时选择至多 3 个目标。";
        public override bool Durative => true;
        public override double Duration => 30;
        public override DispelledType DispelledType => DispelledType.CannotBeDispelled;

        private double 减伤比例 => 0.15 + 0.04 * (Level - 1);
        private double 实际比例 = 0;

        public override void OnEffectGained(Character character)
        {
            character.NormalAttack.CanSelectTargetCount += 2;
            实际比例 = 减伤比例;
            character.NormalAttack.SetMagicType(new(this, true, MagicType.None, 999), GamingQueue);
            if (character.Effects.Where(e => e is 魔法震荡特效).FirstOrDefault() is 魔法震荡特效 e)
            {
                e.基础冷却时间 = 5;
                if (e.冷却时间 > e.基础冷却时间) e.冷却时间 = e.基础冷却时间;
            }
        }

        public override void OnEffectLost(Character character)
        {
            实际比例 = 0;
            character.NormalAttack.CanSelectTargetCount -= 2;
            character.NormalAttack.UnsetMagicType(this, GamingQueue);
            if (character.Effects.Where(e => e is 魔法震荡特效).FirstOrDefault() is 魔法震荡特效 e)
            {
                e.基础冷却时间 = 10;
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

        public override void OnSkillCasted(Character caster, List<Character> targets, Dictionary<string, object> others)
        {
            RemainDuration = Duration;
            if (!caster.Effects.Contains(this))
            {
                caster.Effects.Add(this);
                OnEffectGained(caster);
            }
            GamingQueue?.LastRound.ApplyEffects.TryAdd(caster, [EffectType.DefenseBoost]);
        }
    }
}
