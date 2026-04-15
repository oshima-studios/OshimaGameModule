using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model.PrefabricatedEntity;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 光明之环 : SoulboundSkill
    {
        public override long Id => (long)SuperSkillID.光明之环;
        public override string Name => "光明之环";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";
        public override double CD => 100;
        public override double HardnessTime { get; set; } = 0;
        public override bool CanSelectEnemy => false;
        public override bool CanSelectTeammate => true;
        public override bool CanSelectSelf => true;
        public override bool SelectAllTeammates => true;
        public override bool AllowSelectDead => true;

        public 光明之环(Character? character = null) : base(character)
        {
            Effects.Add(new 光明之环特效(this));
        }
    }

    public class 光明之环特效(SoulboundSkill skill) : SoulboundEffect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"对{Skill.TargetDescription()}施加强驱散，并基于{(Skill.Character != null ? CharacterSet.GetPrimaryAttributeName(Skill.Character.PrimaryAttribute) : "核心属性")}的 {PACoefficient * 100:0.##}% 治疗或复苏{Skill.TargetDescription()}，回复 {Heal:0.##} 点生命值。" +
            (Improvement > 0 ? $"灵魂绑定额外效果：增加目标 {Improvement / 2 * 100:0.##}% 物理伤害减免和魔法抗性，持续 2 回合。" : "") + $"复苏：如果目标已死亡，则复活目标。复苏只能至多回复至其最大生命值的 20%。";

        public double PACoefficient => 1.35 + 0.65 * (Skill.Level - 1);
        public double Heal => (Skill.Character?.PrimaryAttributeValue ?? 0) * PACoefficient;

        public override void OnSkillCasted(Character caster, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            List<Character> deads = [.. targets.Where(c => c.HP == 0)];
            List<Character> alives = [.. targets.Where(c => c.HP > 0)];
            Effect e = new 强驱散特效(Skill);
            e.OnSkillCasted(caster, alives, grids, others);
            foreach (Character target in alives)
            {
                HealToTarget(caster, target, Heal);
            }
            foreach (Character target in deads)
            {
                HealToTarget(caster, target, Math.Min(Heal, target.MaxHP * 0.2), true, false);
            }
            if (Improvement > 0)
            {
                double dr = Improvement / 2;
                foreach (Character target in targets)
                {
                    e = new DynamicsEffect(Skill, new()
                    {
                        ["expdr"] = dr,
                        ["mdftype"] = 0,
                        ["mdfvalue"] = dr
                    }, caster)
                    {
                        Durative = false,
                        Duration = 0,
                        DurationTurn = 2,
                        RemainDurationTurn = 2,
                        IsDebuff = false
                    };
                    target.Effects.Add(e);
                    e.OnEffectGained(target);
                    RecordCharacterApplyEffects(target, EffectType.DefenseBoost);
                    WriteLine($"[ {target} ] 提升了 {dr * 100:0.##}% 物理伤害减免和魔法抗性！");
                }
            }
        }
    }
}