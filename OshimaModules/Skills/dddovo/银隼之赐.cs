using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 银隼之赐 : Skill
    {
        public override long Id => (long)PassiveID.银隼之赐;
        public override string Name => "银隼之赐";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override double CD => 5;

        public 银隼之赐(Character? character = null) : base(SkillType.Passive, character)
        {
            Effects.Add(new 银隼之赐特效(this));
        }

        public override IEnumerable<Effect> AddPassiveEffectToCharacter()
        {
            return Effects;
        }
    }

    public class 银隼之赐特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"{Skill.SkillOwner()}的「银隼」正如其名，优雅而致命。普通攻击命中后，附带基于 {敏捷系数 * 100:0.##}% 敏捷 [ {敏捷伤害:0.##} ] 点魔法伤害，可叠加普攻特效。每 {Skill.CD:0.##} {GameplayEquilibriumConstant.InGameTime}仅能触发一次。";

        private double 敏捷伤害 => 敏捷系数 * Skill.Character?.AGI ?? 0;

        private readonly double 敏捷系数 = 2;
        private bool 是否是嵌套伤害 = false;

        public override void AfterDamageCalculation(Character character, Character enemy, double damage, double actualDamage, bool isNormalAttack, DamageType damageType, MagicType magicType, DamageResult damageResult)
        {
            if (Skill.CurrentCD == 0 && character == Skill.Character && isNormalAttack && (damageResult == DamageResult.Normal || damageResult == DamageResult.Critical) && !是否是嵌套伤害 && enemy.HP > 0)
            {
                Skill.CurrentCD = Skill.CD;
                WriteLine($"[ {character} ] 发动了银隼之赐！将造成额外伤害！");
                是否是嵌套伤害 = true;
                DamageToEnemy(character, enemy, DamageType.Magical, magicType, 敏捷伤害, new(character)
                {
                    TriggerEffects = false
                });
            }

            if (character == Skill.Character && 是否是嵌套伤害)
            {
                是否是嵌套伤害 = false;
            }
        }
    }
}
