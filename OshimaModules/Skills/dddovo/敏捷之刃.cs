using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 敏捷之刃 : Skill
    {
        public override long Id => (long)PassiveID.敏捷之刃;
        public override string Name => "敏捷之刃";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";

        public 敏捷之刃(Character? character = null) : base(SkillType.Passive, character)
        {
            Effects.Add(new 敏捷之刃特效(this));
        }

        public override IEnumerable<Effect> AddPassiveEffectToCharacter()
        {
            return Effects;
        }
    }

    public class 敏捷之刃特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"每次普通攻击都将附带基于 {敏捷系数 * 100:0.##}% 敏捷 [ {敏捷伤害} ] 的魔法伤害。";

        private double 敏捷伤害 => 敏捷系数 * Skill.Character?.AGI ?? 0;
        private readonly double 敏捷系数 = 2.5;
        private bool 是否是嵌套伤害 = false;

        public override void AfterDamageCalculation(Character character, Character enemy, double damage, double actualDamage, bool isNormalAttack, bool isMagicDamage, MagicType magicType, DamageResult damageResult)
        {
            if (character == Skill.Character && isNormalAttack && (damageResult == DamageResult.Normal || damageResult == DamageResult.Critical) && !是否是嵌套伤害 && enemy.HP > 0)
            {
                WriteLine($"[ {character} ] 发动了敏捷之刃！将造成额外伤害！");
                是否是嵌套伤害 = true;
                DamageToEnemy(character, enemy, true, magicType, 敏捷伤害);
            }

            if (character == Skill.Character && 是否是嵌套伤害)
            {
                是否是嵌套伤害 = false;
            }
        }
    }
}
