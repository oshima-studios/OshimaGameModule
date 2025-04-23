using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 疾风步 : Skill
    {
        public override long Id => (long)SkillID.疾风步;
        public override string Name => "疾风步";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";
        public override double EPCost => 60;
        public override double CD => 35;
        public override double HardnessTime { get; set; } = 5;
        public override bool CanSelectSelf => true;
        public override bool CanSelectEnemy => false;

        public 疾风步(Character? character = null) : base(SkillType.Skill, character)
        {
            Effects.Add(new 疾风步特效(this));
        }
    }

    public class 疾风步特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"进入不可选中状态，获得 100 行动速度，提高 8% 暴击率，持续 {Duration:0.##} {GameplayEquilibriumConstant.InGameTime}。破隐一击：在持续时间内，首次造成伤害会附加 {系数 * 100:0.##}% 敏捷 [ {伤害加成:0.##} ] 的强化伤害，并解除不可选中状态。";
        public override string DispelDescription => "不可选中状态生效期间，此技能不可驱散，否则可弱驱散";
        public override bool Durative => true;
        public override double Duration => 12 + (1 * (Level - 1));

        private double 系数 => 0.5 + 0.5 * (Skill.Level - 1);
        private double 伤害加成 => 系数 * Skill.Character?.AGI ?? 0;
        private bool 首次伤害 { get; set; } = true;
        private bool 破隐一击 { get; set; } = false;

        public override void OnEffectGained(Character character)
        {
            Skill.IsInEffect = true;
            AddEffectTypeToCharacter(character, [EffectType.Unselectable]);
            character.ExSPD += 100;
            character.ExCritRate += 0.08;
            GamingQueue?.InterruptCastingAsync(character);
        }

        public override void OnEffectLost(Character character)
        {
            Skill.IsInEffect = false;
            if (!破隐一击)
            {
                // 在没有打出破隐一击的情况下，恢复角色状态
                RemoveEffectTypesFromCharacter(character);
            }
            character.ExSPD -= 100;
            character.ExCritRate -= 0.08;
        }

        public override double AlterActualDamageAfterCalculation(Character character, Character enemy, double damage, bool isNormalAttack, bool isMagicDamage, MagicType magicType, DamageResult damageResult, ref bool isEvaded, Dictionary<Effect, double> totalDamageBonus)
        {
            if (character == Skill.Character && damageResult != DamageResult.Evaded && 首次伤害)
            {
                首次伤害 = false;
                破隐一击 = true;
                RemoveEffectTypesFromCharacter(character);
                double d = 伤害加成;
                WriteLine($"[ {character} ] 触发了疾风步破隐一击，获得了 [ {d:0.##} ] 点伤害加成！");
                return d;
            }
            return 0;
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, Dictionary<string, object> others)
        {
            if (!caster.Effects.Contains(this))
            {
                GamingQueue?.LastRound.Effects.Add(caster, EffectType.Unselectable);
                首次伤害 = true;
                破隐一击 = false;
                RemainDuration = Duration;
                caster.Effects.Add(this);
                OnEffectGained(caster);
            }
        }
    }
}
