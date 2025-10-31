using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.PassiveEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 强攻 : Skill
    {
        public override long Id => (long)PassiveID.强攻;
        public override string Name => "强攻";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";

        public 强攻(Character? character = null) : base(SkillType.Passive, character)
        {
            Effects.Add(new 强攻特效(this));
        }

        public override IEnumerable<Effect> AddPassiveEffectToCharacter()
        {
            return Effects;
        }
    }

    public class 强攻特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"连续 3 次普通攻击同一目标时，造成额外的 {额外伤害系数 * 100:0.##}% 伤害并使其进入易损状态，后续承受伤害提升 {易损伤害提升百分比 * 100:0.##}%，持续 {易损持续时间:0.##} {GameplayEquilibriumConstant.InGameTime}。";

        private double 易损伤害提升百分比 => Skill.Character != null ? 0.15 + Skill.Character.Level * 0.004 : 0.15;
        private double 额外伤害系数 => Skill.Character != null ? 0.06 + Skill.Character.Level * 0.005 : 0.06;
        private double 易损持续时间 => Skill.Character != null ? 15 + Skill.Character.Level * 0.1 : 15;

        private int 目标连续攻击次数 = 0;
        private Character? 当前目标 = null;

        public override double AlterActualDamageAfterCalculation(Character character, Character enemy, double damage, bool isNormalAttack, DamageType damageType, MagicType magicType, DamageResult damageResult, ref bool isEvaded, Dictionary<Effect, double> totalDamageBonus)
        {
            if (Skill.Character != null && Skill.Character == character && isNormalAttack && (damageResult == DamageResult.Normal || damageResult == DamageResult.Critical))
            {
                if (当前目标 != enemy)
                {
                    目标连续攻击次数 = 0;
                    当前目标 = enemy;
                }

                目标连续攻击次数++;

                if (目标连续攻击次数 == 3)
                {
                    double 额外伤害 = damage * 额外伤害系数;
                    WriteLine($"[ {character} ] 发动了强攻！将额外造成 {额外伤害:0.##} 点伤害！");
                    目标连续攻击次数 = 0;
                    WriteLine($"[ {character} ] 对 [ {enemy} ] 施加了易损状态！[ {enemy} ] 的承受伤害提升 {易损伤害提升百分比 * 100:0.##}%！");
                    施加易损状态(character, enemy);
                    return 额外伤害;
                }
            }
            return 0;
        }

        private void 施加易损状态(Character character, Character target)
        {
            Effect e = new 易损(Skill, target, character, true, 易损持续时间, 0, 易损伤害提升百分比);
            target.Effects.Add(e);
            e.OnEffectGained(target);
            RecordCharacterApplyEffects(target, EffectType.Vulnerable);
        }
    }
}
