using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model;
using Milimoe.FunGame.Core.Model.PrefabricatedEntity;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 女王之怒 : SoulboundSkill
    {
        public override long Id => (long)SuperSkillID.女王之怒;
        public override string Name => "女王之怒";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";
        public override double CD => 70;
        public override double HardnessTime { get; set; } = 11;

        public 女王之怒(Character? character = null) : base(character)
        {
            Effects.Add(new 女王之怒特效(this));
        }
    }

    public class 女王之怒特效(SoulboundSkill skill) : SoulboundEffect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"基于{Skill.SkillOwner()}的 {ATKCoefficient * 100:0.##}% 攻击力 [ {Damage:0.##} ] 对{Skill.TargetDescription()}造成{CharacterSet.GetDamageTypeName(DamageType.Physical)}" +
            (Improvement > 0 ? $"，灵魂绑定伤害加成： {Improvement * 100:0.##}% [ {ImprovementDamage:0.##} ] 点，总伤害 {Damage + ImprovementDamage:0.##} 点；" : "；") +
            $"造成伤害后，对目标施加 {虚弱时间}虚弱。虚弱：伤害降低 {DamageReductionPercent * 100:0.##}%，物理护甲降低 {DEFReductionPercent * 100:0.##}%，" +
            $"魔法抗性降低 {MDFReductionPercent * 100:0.##}%，治疗效果降低 {HealingReductionPercent * 100:0.##}%";

        public double ATKCoefficient => 0.4 + 0.22 * (Skill.Level - 1);
        public double Damage => (Skill.Character?.ATK ?? 0) * ATKCoefficient;
        public double ImprovementDamage => Improvement > 0 ? Damage * Improvement : 0;
        public override bool Durative => false;
        public override int DurationTurn
        {
            get
            {
                return Skill.Level switch
                {
                    4 or 5 or 6 => 3,
                    _ => 2
                };
            }
        }
        public double DurationLevelGrowth
        {
            get
            {
                return Skill.Level switch
                {
                    _ => 0
                };
            }
        }
        public double DamageReductionPercent
        {
            get
            {
                return Skill.Level switch
                {
                    1 => 0.10,
                    2 => 0.14,
                    3 => 0.18,
                    4 => 0.22,
                    5 => 0.26,
                    6 => 0.3,
                    _ => 0
                };
            }
        }
        public double DEFReductionPercent
        {
            get
            {
                return Skill.Level switch
                {
                    1 => 0.3,
                    2 => 0.35,
                    3 => 0.4,
                    4 => 0.45,
                    5 => 0.50,
                    6 => 0.55,
                    _ => 0
                };
            }
        }
        public double MDFReductionPercent
        {
            get
            {
                return Skill.Level switch
                {
                    1 => 0.08,
                    2 => 0.11,
                    3 => 0.14,
                    4 => 0.17,
                    5 => 0.20,
                    6 => 0.23,
                    _ => 0
                };
            }
        }
        public double HealingReductionPercent
        {
            get
            {
                return Skill.Level switch
                {
                    1 => 0.12,
                    2 => 0.16,
                    3 => 0.20,
                    4 => 0.24,
                    5 => 0.28,
                    6 => 0.32,
                    _ => 0
                };
            }
        }

        private string 虚弱时间 => Durative && Duration > 0 ? $"{实际虚弱时间:0.##} {GameplayEquilibriumConstant.InGameTime}" : (!Durative && DurationTurn > 0 ? 实际虚弱时间 + " 回合" : $"0 {GameplayEquilibriumConstant.InGameTime}");
        private double 实际虚弱时间 => Durative && Duration > 0 ? Duration + DurationLevelGrowth * (Level - 1) : (!Durative && DurationTurn > 0 ? DurationTurn + DurationLevelGrowth * (Level - 1) : 0);

        public override void OnSkillCasted(Character caster, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            List<Character> valid = [];
            foreach (Character target in targets)
            {
                DamageCalculationOptions options = new(caster);
                if (DamageToEnemy(caster, target, DamageType.Physical, MagicType.None, Damage + ImprovementDamage, options).ActualDamage > 0)
                {
                    valid.Add(target);
                }
            }
            造成虚弱 e = new(Skill, Durative, Duration, DurationTurn, DurationLevelGrowth, DamageReductionPercent, DEFReductionPercent, MDFReductionPercent, HealingReductionPercent);
            e.OnSkillCasted(caster, valid, grids, others);
        }
    }
}
