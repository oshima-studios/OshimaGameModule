using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 天赐之力 : Skill
    {
        public override long Id => (long)SuperSkillID.天赐之力;
        public override string Name => "天赐之力";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";
        public override double EPCost => 100;
        public override double CD => 60;
        public override double HardnessTime { get; set; } = 15;
        public override bool CanSelectSelf => true;
        public override bool CanSelectEnemy => false;

        public 天赐之力(Character? character = null) : base(SkillType.SuperSkill, character)
        {
            Effects.Add(new 天赐之力特效(this));
        }
    }

    public class 天赐之力特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"{Duration:0.##} {GameplayEquilibriumConstant.InGameTime}内，增加 40% 攻击力 [ {攻击力提升:0.##} ]、30% 物理穿透和 25% 闪避率（不可叠加），普通攻击硬直时间额外减少 20%，基于 {系数 * 100:0.##}% 敏捷 [ {伤害加成:0.##} ] 强化普通攻击的伤害。在持续时间内，【心灵之火】的冷却时间降低至 3 {GameplayEquilibriumConstant.InGameTime}。";
        public override bool Durative => true;
        public override double Duration => 40;
        public override DispelledType DispelledType => DispelledType.CannotBeDispelled;

        private double 系数 => 1.2 * (1 + 0.6 * (Skill.Level - 1));
        private double 伤害加成 => 系数 * Skill.Character?.AGI ?? 0;
        private double 攻击力提升 => 0.4 * Skill.Character?.BaseATK ?? 0;
        private double 实际的攻击力提升 = 0;

        public override void OnEffectGained(Character character)
        {
            实际的攻击力提升 = 攻击力提升;
            character.ExATK2 += 实际的攻击力提升;
            character.PhysicalPenetration += 0.3;
            character.ExEvadeRate += 0.25;
            if (character.Effects.Where(e => e is 心灵之火特效).FirstOrDefault() is 心灵之火特效 e)
            {
                e.基础冷却时间 = 3;
                if (e.冷却时间 > e.基础冷却时间) e.冷却时间 = e.基础冷却时间;
            }
        }

        public override void OnEffectLost(Character character)
        {
            character.ExATK2 -= 实际的攻击力提升;
            character.PhysicalPenetration -= 0.3;
            character.ExEvadeRate -= 0.25;
            if (character.Effects.Where(e => e is 心灵之火特效).FirstOrDefault() is 心灵之火特效 e)
            {
                e.基础冷却时间 = 8;
            }
        }

        public override CharacterActionType AlterActionTypeBeforeAction(Character character, CharacterState state, ref bool canUseItem, ref bool canCastSkill, ref double pUseItem, ref double pCastSkill, ref double pNormalAttack)
        {
            pNormalAttack += 0.1;
            return CharacterActionType.None;
        }

        public override double AlterExpectedDamageBeforeCalculation(Character character, Character enemy, double damage, bool isNormalAttack, bool isMagicDamage, MagicType magicType, Dictionary<Effect, double> totalDamageBonus)
        {
            if (character == Skill.Character && isNormalAttack)
            {
                return 伤害加成;
            }
            return 0;
        }

        public override void AlterHardnessTimeAfterNormalAttack(Character character, ref double baseHardnessTime, ref bool isCheckProtected)
        {
            baseHardnessTime *= 0.8;
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, Dictionary<string, object> others)
        {
            RemainDuration = Duration;
            if (!caster.Effects.Contains(this))
            {
                caster.Effects.Add(this);
                OnEffectGained(caster);
            }
            GamingQueue?.LastRound.ApplyEffects.TryAdd(caster, [EffectType.DamageBoost, EffectType.PenetrationBoost]);
        }
    }
}
