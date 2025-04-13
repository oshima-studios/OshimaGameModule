using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.PassiveEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 魔法震荡 : Skill
    {
        public override long Id => (long)PassiveID.魔法震荡;
        public override string Name => "魔法震荡";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";

        public 魔法震荡(Character? character = null) : base(SkillType.Passive, character)
        {
            Effects.Add(new 魔法震荡特效(this));
        }

        public override IEnumerable<Effect> AddPassiveEffectToCharacter()
        {
            return Effects;
        }
    }

    public class 魔法震荡特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"造成魔法伤害时有 35% 几率使敌人眩晕 1 回合。";

        public override void AfterDamageCalculation(Character character, Character enemy, double damage, bool isNormalAttack, bool isMagicDamage, MagicType magicType, DamageResult damageResult)
        {
            if (character == Skill.Character && isMagicDamage && damageResult != DamageResult.Evaded && new Random().NextDouble() < 0.35)
            {
                IEnumerable<Effect> effects = enemy.Effects.Where(e => e is 眩晕 && e.Skill == Skill);
                if (effects.Any())
                {
                    effects.First().RemainDurationTurn++;
                }
                else
                {
                    眩晕 e = new(Skill, character, false, 0, 1);
                    enemy.Effects.Add(e);
                    e.OnEffectGained(enemy);
                }
                WriteLine($"[ {character} ] 的魔法伤害触发了魔法震荡，[ {enemy} ] 被眩晕了！");
            }
        }
    }
}
