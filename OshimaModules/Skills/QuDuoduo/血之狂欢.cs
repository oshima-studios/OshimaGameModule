using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 血之狂欢 : Skill
    {
        public override long Id => (long)SuperSkillID.血之狂欢;
        public override string Name => "血之狂欢";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";
        public override double EPCost => 100;
        public override double CD => 45;
        public override double HardnessTime { get; set; } = 7;
        public override bool CanSelectSelf => true;
        public override bool CanSelectEnemy => false;

        public 血之狂欢(Character? character = null) : base(SkillType.SuperSkill, character)
        {
            Effects.Add(new 血之狂欢特效(this));
        }
    }

    public class 血之狂欢特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"获得 40% 吸血，持续 {Duration:0.##} {GameplayEquilibriumConstant.InGameTime}。";
        public override bool Durative => true;
        public override double Duration => 30;
        public override DispelledType DispelledType => DispelledType.CannotBeDispelled;

        public override void AfterDamageCalculation(Character character, Character enemy, double damage, double actualDamage, bool isNormalAttack, bool isMagicDamage, MagicType magicType, DamageResult damageResult)
        {
            if (character == Skill.Character && (damageResult == DamageResult.Normal || damageResult == DamageResult.Critical) && character.HP < character.MaxHP)
            {
                double 实际吸血 = 0.4 * damage;
                HealToTarget(character, character, 实际吸血);
            }
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, Dictionary<string, object> others)
        {
            RemainDuration = Duration;
            if (!caster.Effects.Contains(this))
            {
                caster.Effects.Add(this);
                OnEffectGained(caster);
            }
            GamingQueue?.LastRound.ApplyEffects.TryAdd(caster, [EffectType.Lifesteal]);
        }
    }
}
