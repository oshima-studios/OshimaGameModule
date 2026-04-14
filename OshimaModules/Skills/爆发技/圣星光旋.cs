using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model;
using Milimoe.FunGame.Core.Model.PrefabricatedEntity;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 圣星光旋 : SoulboundSkill
    {
        public override long Id => (long)SuperSkillID.圣星光旋;
        public override string Name => "圣星光旋";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";
        public override double CD => 100;
        public override double HardnessTime { get; set; } = 16;
        public override bool SelectAllEnemies => true;

        public 圣星光旋(Character? character = null) : base(character)
        {
            Effects.Add(new 圣星光旋特效(this));
        }
    }

    public class 圣星光旋特效(SoulboundSkill skill) : SoulboundEffect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"对{Skill.TargetDescription()}造成 {PACoefficient * 100:0.##}% " +
            $"{(Skill.Character != null ? CharacterSet.GetPrimaryAttributeName(Skill.Character.PrimaryAttribute) : "核心属性")} [ {PADamage:0.##} ] + {GeneralDamage:0.##} 点{CharacterSet.GetDamageTypeName(DamageType.True)}。" +
            (Improvement > 0 ? $"灵魂绑定伤害加成： {Improvement * 100:0.##}% [ {ImprovementDamage:0.##} ] 点，" : "") + $"总伤害 {Damage + ImprovementDamage:0.##} 点。" +
            $"随后，有 25% 概率对受到伤害的目标造成眩晕 1 回合。眩晕：进入完全行动不能状态。";

        public double PACoefficient => 0.35 + 0.05 * (Skill.Level - 1);
        public double PADamage => (Skill.Character?.PrimaryAttributeValue ?? 0) * PACoefficient;
        public double GeneralDamage => 70 * Skill.Level;
        public double Damage => GeneralDamage + PADamage;
        public double ImprovementDamage => Improvement > 0 ? Damage * Improvement : 0;

        public override void OnSkillCasted(Character caster, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            List<Character> valid = [];
            foreach (Character target in targets)
            {
                DamageCalculationOptions options = new(caster);
                if (DamageToEnemy(caster, target, DamageType.True, MagicType.None, Damage + ImprovementDamage, options).ActualDamage > 0)
                {
                    if (Random.Shared.NextDouble() < 0.25)
                    {
                        valid.Add(target);
                    }
                }
            }
            Effect e = new 造成眩晕(Skill);
            e.OnSkillCasted(caster, valid, grids, others);
        }
    }
}
