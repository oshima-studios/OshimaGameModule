using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.PassiveEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 电刑 : Skill
    {
        public override long Id => (long)PassiveID.电刑;
        public override string Name => "电刑";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";

        public 电刑(Character? character = null) : base(SkillType.Passive, character)
        {
            Effects.Add(new 电刑特效(this));
        }

        public override IEnumerable<Effect> AddPassiveEffectToCharacter()
        {
            return Effects;
        }
    }

    public class 电刑特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"造成伤害时，标记目标 25 {GameplayEquilibriumConstant.InGameTime}并叠加 1 层数，当目标身上的电刑标记达到 3 层时，此次伤害提升 {伤害百分比 * 100:0.##}%。";
        private double 伤害百分比 => Skill.Character != null ? 0.3 + Skill.Character.Level * 0.005 : 0;

        public override double AlterActualDamageAfterCalculation(Character character, Character enemy, double damage, bool isNormalAttack, DamageType damageType, MagicType magicType, DamageResult damageResult, ref bool isEvaded, Dictionary<Effect, double> totalDamageBonus)
        {
            if (Skill.Character != null && Skill.Character == character && (damageResult == DamageResult.Normal || damageResult == DamageResult.Critical))
            {
                if (enemy.Effects.Where(e => e is 电刑标记 && e.Source == character).FirstOrDefault() is 电刑标记 e)
                {
                    e.层数++;
                    if (e.层数 >= 3)
                    {
                        double 额外伤害 = damage * 伤害百分比;
                        WriteLine($"[ {character} ] 发动了电刑的 3 层效果，伤害提升了 {伤害百分比 * 100:0.##}%！额外造成 {额外伤害:0.##} 点{CharacterSet.GetDamageTypeName(damageType)}！");
                        e.RemainDuration = 0;
                        enemy.Effects.Remove(e);
                        return 额外伤害;
                    }
                }
                else
                {
                    e = new 电刑标记(Skill, character)
                    {
                        Durative = true,
                        Duration = 25,
                        RemainDuration = 25
                    };
                    enemy.Effects.Add(e);
                }
            }
            return 0;
        }
    }
}
