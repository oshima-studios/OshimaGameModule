using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 迅捷之势 : Skill
    {
        public override long Id => (long)SuperSkillID.迅捷之势;
        public override string Name => "迅捷之势";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override double EPCost => 100;
        public override double CD => 60 - 2 * (Level - 1);
        public override double HardnessTime { get; set; } = 15;

        public 迅捷之势(Character? character = null) : base(SkillType.SuperSkill, character)
        {
            Effects.Add(new 迅捷之势特效(this));
        }
    }

    public class 迅捷之势特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"{Duration} 时间内，普通攻击转为魔法伤害，且硬直时间减少50%，并基于 {智力系数 * 100:0.##}% 智力 [{智力加成}] 强化普通攻击伤害。";
        public override bool TargetSelf => true;
        public override bool Durative => true;
        public override double Duration => 40;

        private double 智力系数 => Calculation.Round4Digits(1.4 + 0.4 * (Level - 1));
        private double 智力加成 => Calculation.Round2Digits(智力系数 * Skill.Character?.INT ?? 0);

        public override void OnEffectGained(Character character)
        {
            character.NormalAttack.SetMagicType(true, character.MagicType);
        }

        public override void OnEffectLost(Character character)
        {
            character.NormalAttack.SetMagicType(false, character.MagicType);
        }

        public override void AlterExpectedDamageBeforeCalculation(Character character, Character enemy, ref double damage, bool isNormalAttack, bool isMagicDamage, MagicType magicType)
        {
            if (character == Skill.Character && isNormalAttack)
            {
                damage = Calculation.Round2Digits(damage + 智力加成);
            }
        }

        public override void AlterHardnessTimeAfterNormalAttack(Character character, ref double baseHardnessTime, ref bool isCheckProtected)
        {
            baseHardnessTime = Calculation.Round2Digits(baseHardnessTime * 0.5);
        }

        public override void OnSkillCasted(Character caster, List<Character> enemys, List<Character> teammates, Dictionary<string, object> others)
        {
            RemainDuration = Duration;
            if (!caster.Effects.Contains(this))
            {
                caster.Effects.Add(this);
                OnEffectGained(caster);
            }
        }
    }
}
