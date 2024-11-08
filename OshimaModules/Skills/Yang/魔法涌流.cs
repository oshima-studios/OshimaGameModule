using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 魔法涌流 : Skill
    {
        public override long Id => (long)SuperSkillID.魔法涌流;
        public override string Name => "魔法涌流";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override double EPCost => 100;
        public override double CD => 35;
        public override double HardnessTime { get; set; } = 10;
        public override bool CanSelectSelf => true;
        public override bool CanSelectEnemy => false;

        public 魔法涌流(Character? character = null) : base(SkillType.SuperSkill, character)
        {
            Effects.Add(new 魔法涌流特效(this));
        }
    }

    public class 魔法涌流特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => "魔法涌流";
        public override string Description => $"{Duration:0.##} 时间内，增加所有伤害的 {减伤比例 * 100:0.##}% 伤害减免，并将普通攻击转为魔法伤害，可叠加魔法震荡的效果。";
        public override bool Durative => true;
        public override double Duration => 25;

        private double 减伤比例 => 0.1 + 0.02 * (Level - 1);
        private double 实际比例 = 0;

        public override void OnEffectGained(Character character)
        {
            实际比例 = 减伤比例;
            character.NormalAttack.SetMagicType(true, character.MagicType);
        }

        public override void OnEffectLost(Character character)
        {
            character.NormalAttack.SetMagicType(false, character.MagicType);
        }

        public override bool AlterActualDamageAfterCalculation(Character character, Character enemy, ref double damage, bool isNormalAttack, bool isMagicDamage, MagicType magicType, DamageResult damageResult)
        {
            if (enemy == Skill.Character)
            {
                damage *= 1 - 实际比例;
            }
            return false;
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, Dictionary<string, object> others)
        {
            RemainDuration = Duration;
            if (!caster.Effects.Contains(this))
            {
                实际比例 = 0;
                caster.Effects.Add(this);
                OnEffectGained(caster);
            }
        }
    }
}
