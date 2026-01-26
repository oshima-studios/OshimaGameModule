using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 贪欲猎手 : Skill
    {
        public override long Id => (long)PassiveID.贪欲猎手;
        public override string Name => "贪欲猎手";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";

        public 贪欲猎手(Character? character = null) : base(SkillType.Passive, character)
        {
            Effects.Add(new 贪欲猎手特效(this));
        }

        public override IEnumerable<Effect> AddPassiveEffectToCharacter()
        {
            return Effects;
        }
    }

    public class 贪欲猎手特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"造成伤害后，基于造成伤害值的 {生命回复系数 * 100:0.##}% 回复生命值。";

        private double 生命回复系数 => Skill.Character != null ? 0.04 + Skill.Character.Level / 10 * 0.01 : 0.04;

        public override void AfterDamageCalculation(Character character, Character enemy, double damage, double actualDamage, bool isNormalAttack, DamageType damageType, MagicType magicType, DamageResult damageResult)
        {
            if (Skill.Character != null && Skill.Character == character && isNormalAttack && (damageResult == DamageResult.Normal || damageResult == DamageResult.Critical))
            {
                HealToTarget(character, character, 生命回复系数 * actualDamage);
            }
        }
    }
}