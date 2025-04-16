using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.PassiveEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 嗜血本能 : Skill
    {
        public override long Id => (long)SuperSkillID.嗜血本能;
        public override string Name => "嗜血本能";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override double EPCost => 100;
        public override double CD => 45;
        public override double HardnessTime { get; set; } = 5;
        public override bool CanSelectSelf => true;
        public override bool CanSelectEnemy => false;

        public 嗜血本能(Character? character = null) : base(SkillType.SuperSkill, character)
        {
            Effects.Add(new 嗜血本能特效(this));
        }
    }

    public class 嗜血本能特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"{Duration} {GameplayEquilibriumConstant.InGameTime}内，攻击拥有标记的角色将不会回收标记，增强 {最大生命值伤害 * 100:0.##}% 最大生命值伤害，并获得 {吸血 * 100:0.##}% 吸血。";
        public override bool Durative => true;
        public override double Duration => 25;

        private static double 吸血 => 0.2;
        private double 最大生命值伤害 => 0.015 * Level;

        public override void OnEffectGained(Character character)
        {
            if (character.Effects.Where(e => e is 累积之压特效).FirstOrDefault() is 累积之压特效 e)
            {
                e.系数 += 最大生命值伤害;
            }
        }

        public override void OnEffectLost(Character character)
        {
            if (character.Effects.Where(e => e is 累积之压特效).FirstOrDefault() is 累积之压特效 e)
            {
                e.系数 -= 最大生命值伤害;
            }
        }

        public override void AfterDamageCalculation(Character character, Character enemy, double damage, bool isNormalAttack, bool isMagicDamage, MagicType magicType, DamageResult damageResult)
        {
            if (character == Skill.Character && damageResult != DamageResult.Evaded && character.HP < character.MaxHP)
            {
                double 实际吸血 = 吸血 * damage;
                character.HP += 实际吸血;
                WriteLine($"[ {character} ] 回复了 {实际吸血:0.##} 点生命值！");
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
        }
    }
}
