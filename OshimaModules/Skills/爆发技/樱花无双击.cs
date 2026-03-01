using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model;
using Milimoe.FunGame.Core.Model.PrefabricatedEntity;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 樱花无双击 : SoulboundSkill
    {
        public override long Id => (long)SuperSkillID.樱花无双击;
        public override string Name => "樱花无双击";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";
        public override double CD => 60;
        public override double HardnessTime { get; set; } = 8;

        public 樱花无双击(Character? character = null) : base(character)
        {
            Effects.Add(new 樱花无双击特效(this));
        }
    }

    public class 樱花无双击特效(SoulboundSkill skill) : SoulboundEffect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"基于{Skill.SkillOwner()}的 {ATKCoefficient * 100:0.##}% 攻击力 [ {Damage:0.##} ] 对{Skill.TargetDescription()}造成{CharacterSet.GetDamageTypeName(DamageType.Physical)}" +
            (Improvement > 0 ? $"，灵魂绑定伤害加成： {Improvement * 100:0.##}% [ {ImprovementDamage:0.##} ] 点，总伤害 {Damage + ImprovementDamage:0.##} 点；" : "；") +
            $"造成伤害后，基于伤害值的 {HealCoefficient * 100:0.##}% 回复自身 [ {Heal(Damage):0.##} ] 点生命值。";

        public double ATKCoefficient => 0.6 + 0.15 * (Skill.Level - 1);
        public double Damage => (Skill.Character?.ATK ?? 0) * ATKCoefficient;
        public double ImprovementDamage => Improvement > 0 ? Damage * Improvement : 0;
        public double HealCoefficient => 0.1 + 0.05 * (Skill.Level - 1);
        public double Heal(double damage) => damage * HealCoefficient;

        public override void OnSkillCasted(Character caster, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            foreach (Character target in targets)
            {
                DamageCalculationOptions options = new(caster);
                DamageRecord record = DamageToEnemy(caster, target, DamageType.Physical, MagicType.None, Damage + ImprovementDamage, options);
                HealToTarget(caster, caster, Heal(record.ActualDamage));
            }
        }
    }
}
