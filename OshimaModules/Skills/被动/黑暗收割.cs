using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 黑暗收割 : Skill
    {
        public override long Id => (long)PassiveID.黑暗收割;
        public override string Name => "黑暗收割";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";

        public 黑暗收割(Character? character = null) : base(SkillType.Passive, character)
        {
            Effects.Add(new 黑暗收割特效(this));
        }

        public override IEnumerable<Effect> AddPassiveEffectToCharacter()
        {
            return Effects;
        }
    }

    public class 黑暗收割特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"对一半以下生命值的目标造成伤害时，将收集其灵魂以永久提升自身伤害，每个灵魂提供 {额外伤害提升 * 100:0.##}% 伤害加成。最多收集 {最多灵魂数量} 个灵魂。" +
            $"若自身死亡，灵魂将损失一半。（当前灵魂数量：{当前灵魂数量} 个；伤害提升：{当前灵魂数量 * 额外伤害提升 * 100:0.##}%）";

        private static double 额外伤害提升 => 0.02;
        private int 当前灵魂数量 { get; set; } = 0;
        private int 最多灵魂数量 => Skill.Character != null ? 10 + Skill.Character.Level / 10 * 5 : 10;
        private bool 触发标记 { get; set; } = false;

        public override double AlterExpectedDamageBeforeCalculation(Character character, Character enemy, double damage, bool isNormalAttack, DamageType damageType, MagicType magicType, Dictionary<Effect, double> totalDamageBonus)
        {
            if (character == Skill.Character && (enemy.HP / enemy.MaxHP) < 0.5)
            {
                触发标记 = true;
                return damage * 当前灵魂数量 * 额外伤害提升;
            }
            return 0;
        }

        public override void AfterDamageCalculation(Character character, Character enemy, double damage, double actualDamage, bool isNormalAttack, DamageType damageType, MagicType magicType, DamageResult damageResult)
        {
            if (触发标记 && character == Skill.Character && (damageResult == DamageResult.Normal || damageResult == DamageResult.Critical) && 当前灵魂数量 < 最多灵魂数量)
            {
                触发标记 = false;
                当前灵魂数量++;
                WriteLine($"[ {character} ] 通过黑暗收割收集了一个灵魂！当前灵魂数：{当前灵魂数量}");
            }
        }

        public override void AfterDeathCalculation(Character death, Character? killer, Dictionary<Character, int> continuousKilling, Dictionary<Character, int> earnedMoney)
        {
            if (death == Skill.Character && 当前灵魂数量 > 0)
            {
                int lost = 当前灵魂数量 / 2;
                当前灵魂数量 -= lost;
                WriteLine($"[ {death} ] 因死亡损失了 [ {lost} ] 个灵魂！");
            }
        }

        public override void OnTurnEnd(Character character)
        {
            触发标记 = false;
        }
    }
}
