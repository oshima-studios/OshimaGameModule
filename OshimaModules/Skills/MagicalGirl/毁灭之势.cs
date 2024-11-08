using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 毁灭之势 : Skill
    {
        public override long Id => (long)PassiveID.毁灭之势;
        public override string Name => "毁灭之势";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";

        public 毁灭之势(Character? character = null) : base(SkillType.Passive, character)
        {
            Effects.Add(new 毁灭之势特效(this));
        }

        public override IEnumerable<Effect> AddInactiveEffectToCharacter()
        {
            return Effects;
        }
    }

    public class 毁灭之势特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"每时间提升 {伤害提升 * 100:0.##}% 所有伤害，无上限，但受到伤害时效果清零。" + (累计伤害 > 0 ? $"（当前总提升：{累计伤害 * 100:0.##}%）" : "");
        
        private readonly double 伤害提升 = 0.04;
        private double 累计伤害 = 0;

        public override bool AlterActualDamageAfterCalculation(Character character, Character enemy, ref double damage, bool isNormalAttack, bool isMagicDamage, MagicType magicType, DamageResult damageResult)
        {
            if (damageResult != DamageResult.Evaded)
            {
                if (enemy == Skill.Character && damage > 0 && !enemy.Effects.Where(e => e is 绝对领域特效).Any())
                {
                    累计伤害 = 0;
                }

                if (character == Skill.Character)
                {
                    double 实际伤害提升 = damage * 累计伤害;
                    damage += 实际伤害提升;
                    if (实际伤害提升 > 0) WriteLine($"[ {character} ] 的伤害提升了 {实际伤害提升:0.##} 点！");
                }
            }
            return false;
        }

        public override void OnTimeElapsed(Character character, double eapsed)
        {
            累计伤害 += 伤害提升 * eapsed;
            WriteLine($"[ {character} ] 的 [ {Name} ] 效果增加了，当前总提升：{累计伤害 * 100:0.##}%。");
        }
    }
}
