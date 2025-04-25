using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 绝对领域 : Skill
    {
        public override long Id => (long)SuperSkillID.绝对领域;
        public override string Name => "绝对领域";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";
        public override double EPCost => Math.Max(100, Character?.EP ?? 100);
        public override double CD => 60;
        public override double HardnessTime { get; set; } = 5;
        public override bool CanSelectSelf => true;
        public override bool CanSelectEnemy => false;

        public 绝对领域(Character? character = null) : base(SkillType.SuperSkill, character)
        {
            Effects.Add(new 绝对领域特效(this));
        }
    }

    public class 绝对领域特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"{Duration:0.##} {GameplayEquilibriumConstant.InGameTime}内无法受到任何伤害，且敏捷提升 {系数 * 100:0.##}% [ {敏捷提升:0.##} ]。此技能会消耗至少 100 点能量，每额外消耗 10 能量，持续时间提升 1 {GameplayEquilibriumConstant.InGameTime}。";
        public override bool Durative => true;
        public override double Duration => 释放时的能量值 >= 100 ? 13 + (释放时的能量值 - 100) * 0.1 : 14;
        public override DispelledType DispelledType => DispelledType.CannotBeDispelled;

        private double 系数 => 0.2 + 0.015 * (Level - 1);
        private double 敏捷提升 => 系数 * Skill.Character?.BaseAGI ?? 0;
        private double 实际敏捷提升 = 0;
        private double 释放时的能量值 = 0;

        public override void OnEffectGained(Character character)
        {
            实际敏捷提升 = 敏捷提升;
            character.ExAGI += 实际敏捷提升;
            WriteLine($"[ {character} ] 的敏捷提升了 {系数 * 100:0.##}% [ {实际敏捷提升:0.##} ] ！");
        }

        public override void OnEffectLost(Character character)
        {
            character.ExAGI -= 实际敏捷提升;
        }

        public override double AlterActualDamageAfterCalculation(Character character, Character enemy, double damage, bool isNormalAttack, bool isMagicDamage, MagicType magicType, DamageResult damageResult, ref bool isEvaded, Dictionary<Effect, double> totalDamageBonus)
        {
            if (enemy == Skill.Character && damageResult != DamageResult.Evaded)
            {
                WriteLine($"[ {enemy} ] 发动了绝对领域，巧妙的化解了此伤害！");
                isEvaded = true;
                return 0;
            }
            return 0;
        }

        public override void OnSkillCasting(Character caster, List<Character> targets)
        {
            释放时的能量值 = caster.EP;
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, Dictionary<string, object> others)
        {
            RemainDuration = Duration;
            if (!caster.Effects.Contains(this))
            {
                实际敏捷提升 = 0;
                caster.Effects.Add(this);
                OnEffectGained(caster);
            }
            GamingQueue?.LastRound.Effects.TryAdd(caster, [EffectType.Invulnerable]);
        }
    }
}
