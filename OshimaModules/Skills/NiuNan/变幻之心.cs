using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 变幻之心 : Skill
    {
        public override long Id => (long)SuperSkillID.变幻之心;
        public override string Name => "变幻之心";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override double EPCost => 100;
        public override double CD => 30;
        public override double HardnessTime { get; set; } = 10;
        public override bool CanSelectSelf => true;
        public override bool CanSelectEnemy => false;

        public 变幻之心(Character? character = null) : base(SkillType.SuperSkill, character)
        {
            Effects.Add(new 变幻之心特效(this));
        }
    }

    public class 变幻之心特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => "变幻之心";
        public override string Description => $"检查 [ 智慧与力量 ] 的模式。在力量模式下，立即回复 {生命值回复 * 100:0.##}% 生命值；智力模式下，下一次魔法伤害提升 {伤害提升 * 100:0.##}%。";
        
        private double 生命值回复 => 0.25 + 0.03 * (Level - 1);
        private double 伤害提升 => 0.55 + 0.25 * (Level - 1);

        public override void OnEffectGained(Character character)
        {
            Skill.IsInEffect = true;
        }

        public override void OnEffectLost(Character character)
        {
            Skill.IsInEffect = false;
        }

        public override void AlterExpectedDamageBeforeCalculation(Character character, Character enemy, ref double damage, bool isNormalAttack, bool isMagicDamage, MagicType magicType)
        {
            if (character == Skill.Character && isMagicDamage)
            {
                double 实际伤害提升百分比 = 伤害提升;
                double 实际伤害提升 = damage * 实际伤害提升百分比;
                damage += 实际伤害提升;
                WriteLine($"[ {character} ] 发动了变幻之心！伤害提升了 {实际伤害提升:0.##} 点！");
                character.Effects.Remove(this);
                OnEffectLost(character);
            }
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, Dictionary<string, object> others)
        {
            IEnumerable<Effect> effects = caster.Effects.Where(e => e is 智慧与力量特效);
            if (effects.Any())
            {
                if (caster.PrimaryAttribute == PrimaryAttribute.STR)
                {
                    double 回复的生命 = 生命值回复 * caster.MaxHP;
                    caster.HP += 回复的生命;
                    WriteLine($"[ {caster} ] 回复了 {回复的生命:0.##} 点生命值！");
                }
                else if (caster.PrimaryAttribute == PrimaryAttribute.INT)
                {
                    if (!caster.Effects.Contains(this))
                    {
                        caster.Effects.Add(this);
                        OnEffectGained(caster);
                    }
                }
            }
        }
    }
}
