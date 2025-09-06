using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 迅捷之势 : Skill
    {
        public override long Id => (long)SuperSkillID.迅捷之势;
        public override string Name => "迅捷之势";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";
        public override double EPCost => 100;
        public override double CD => 80 - 4 * (Level - 1);
        public override double HardnessTime { get; set; } = 10;
        public override bool CanSelectSelf => true;
        public override bool CanSelectEnemy => false;

        public 迅捷之势(Character? character = null) : base(SkillType.SuperSkill, character)
        {
            Effects.Add(new 迅捷之势特效(this));
        }
    }

    public class 迅捷之势特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"{Duration:0.##} {GameplayEquilibriumConstant.InGameTime}内，提升自身 25% 物理伤害减免和魔法抗性，普通攻击转为魔法伤害，且硬直时间减少 30%，并基于 {智力系数 * 100:0.##}% 智力 [ {智力加成:0.##} ] 强化普通攻击伤害。";
        public override bool Durative => true;
        public override double Duration => 40;
        public override DispelledType DispelledType => DispelledType.CannotBeDispelled;

        private double 智力系数 => 1.4 + 0.4 * (Level - 1);
        private double 智力加成 => 智力系数 * Skill.Character?.INT ?? 0;
        private double 物理伤害减免 => 0.25;
        private double 魔法抗性 => 0.25;
        private double 实际物理伤害减免 = 0;
        private double 实际魔法抗性 = 0;

        public override void OnEffectGained(Character character)
        {
            character.NormalAttack.SetMagicType(new(this, true, MagicType.None, 999), GamingQueue);
            实际物理伤害减免 = 物理伤害减免;
            实际魔法抗性 = 魔法抗性;
            character.ExPDR += 实际物理伤害减免;
            character.MDF[character.MagicType] += 实际魔法抗性;
            WriteLine($"[ {character} ] 提升了 {实际物理伤害减免 * 100:0.##}% 物理伤害减免，{实际魔法抗性 * 100:0.##}% 魔法抗性！！");
        }

        public override void OnEffectLost(Character character)
        {
            character.NormalAttack.UnsetMagicType(this, GamingQueue);
            character.ExPDR -= 实际物理伤害减免;
            character.MDF[character.MagicType] -= 实际魔法抗性;
            实际物理伤害减免 = 0;
            实际魔法抗性 = 0;
        }

        public override double AlterExpectedDamageBeforeCalculation(Character character, Character enemy, double damage, bool isNormalAttack, DamageType damageType, MagicType magicType, Dictionary<Effect, double> totalDamageBonus)
        {
            if (character == Skill.Character && isNormalAttack)
            {
                WriteLine($"[ {character} ] 发动了迅捷之势！伤害提升了 {智力加成:0.##} 点！");
                return 智力加成;
            }
            return 0;
        }

        public override void AlterHardnessTimeAfterNormalAttack(Character character, ref double baseHardnessTime, ref bool isCheckProtected)
        {
            baseHardnessTime *= 0.3;
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, Dictionary<string, object> others)
        {
            RemainDuration = Duration;
            if (!caster.Effects.Contains(this))
            {
                caster.Effects.Add(this);
                OnEffectGained(caster);
            }
            GamingQueue?.LastRound.ApplyEffects.TryAdd(caster, [EffectType.DamageBoost, EffectType.Haste, EffectType.DefenseBoost]);
        }
    }
}
