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
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";

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
        public override string Description => $"对处于完全行动不能、行动受限、战斗不能、技能受限、攻击受限状态的敌人额外造成 {系数 * 100:0.##}% 力量 [ {伤害加成:0.##} ] 点伤害；造成魔法伤害时使敌人眩晕 1 回合，冷却 {基础冷却时间:0.##} {GameplayEquilibriumConstant.InGameTime}。" +
            (冷却时间 > 0 ? $"（正在冷却：剩余 {冷却时间:0.##} {GameplayEquilibriumConstant.InGameTime}）" : "");
        public override string DispelDescription => "被驱散性：眩晕需强驱散";
        public double 冷却时间 { get; set; } = 0;
        public double 基础冷却时间 { get; set; } = 10;
        private static double 系数 => 4;
        private double 伤害加成 => 系数 * Skill.Character?.STR ?? 0;

        public override double AlterExpectedDamageBeforeCalculation(Character character, Character enemy, double damage, bool isNormalAttack, DamageType damageType, MagicType magicType, Dictionary<Effect, double> totalDamageBonus)
        {
            if (character == Skill.Character && (
                enemy.CharacterState == CharacterState.NotActionable ||
                enemy.CharacterState == CharacterState.ActionRestricted ||
                enemy.CharacterState == CharacterState.BattleRestricted ||
                enemy.CharacterState == CharacterState.SkillRestricted ||
                enemy.CharacterState == CharacterState.AttackRestricted))
            {
                return 伤害加成;
            }
            return 0;
        }

        public override void AfterDamageCalculation(Character character, Character enemy, double damage, double actualDamage, bool isNormalAttack, DamageType damageType, MagicType magicType, DamageResult damageResult)
        {
            if (character == Skill.Character && damageType == DamageType.Magical && actualDamage > 0 && 冷却时间 == 0 && (damageResult == DamageResult.Normal || damageResult == DamageResult.Critical))
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
                冷却时间 = 基础冷却时间;
            }
        }

        public override void OnTimeElapsed(Character character, double elapsed)
        {
            if (冷却时间 > 0)
            {
                冷却时间 -= elapsed;
                if (冷却时间 <= 0)
                {
                    冷却时间 = 0;
                }
            }
        }
    }
}
