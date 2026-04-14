using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model;
using Milimoe.FunGame.Core.Model.PrefabricatedEntity;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 裁决塔罗 : SoulboundSkill
    {
        public override long Id => (long)SuperSkillID.裁决塔罗;
        public override string Name => "裁决塔罗";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";
        public override double CD => 75;
        public override double HardnessTime { get; set; } = 9;
        public override int CanSelectTargetCount
        {
            get
            {
                return Level switch
                {
                    1 or 2 => 1,
                    3 or 4 => 2,
                    _ => 3
                };
            }
        }

        public 裁决塔罗(Character? character = null) : base(character)
        {
            Effects.Add(new 裁决塔罗特效(this));
        }
    }

    public class 裁决塔罗特效(SoulboundSkill skill) : SoulboundEffect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"基于{Skill.SkillOwner()}的 {ATKCoefficient * 100:0.##}% 攻击力 [ {Damage:0.##} ] 对{Skill.TargetDescription()}造成{CharacterSet.GetDamageTypeName(DamageType.Magical)}" +
            (Improvement > 0 ? $"，灵魂绑定伤害加成： {Improvement * 100:0.##}% [ {ImprovementDamage:0.##} ] 点，总伤害 {Damage + ImprovementDamage:0.##} 点；" : "；") +
            $"造成伤害后，对目标随机施加以下几种状态：\r\n1、冻结：造成 {DurationTurn} 回合完全行动不能和 30% 魔法伤害易伤；\r\n" +
            $"2、混乱：进入行动受限状态，失控并随机行动，且所有指令均会在所有角色中随机选取目标，持续 {DurationTurn} 回合；\r\n" +
            $"3、战斗不能：在 {Duration:0.##} {GameplayEquilibriumConstant.InGameTime}内无法普通攻击和使用技能（魔法、战技和爆发技）。";

        public double ATKCoefficient => 0.38 + 0.12 * (Skill.Level - 1);
        public double Damage => (Skill.Character?.ATK ?? 0) * ATKCoefficient;
        public double ImprovementDamage => Improvement > 0 ? Damage * Improvement : 0;
        public override double Duration
        {
            get
            {
                return Skill.Level switch
                {
                    4 or 5 or 6 => 14,
                    _ => 7
                };
            }
        }
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

        public override void OnSkillCasted(Character caster, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            foreach (Character target in targets)
            {
                DamageCalculationOptions options = new(caster);
                if (DamageToEnemy(caster, target, DamageType.Magical, MagicType.None, Damage + ImprovementDamage, options).ActualDamage > 0)
                {
                    Effect e = Random.Shared.Next(3) switch
                    {
                        0 => new 施加概率负面(Skill, EffectType.Freeze, false, 0, DurationTurn, 0, 1, 0),
                        1 => new 施加概率负面(Skill, EffectType.Confusion, false, 0, DurationTurn, 0, 1, 0),
                        _ => new 施加概率负面(Skill, EffectType.Cripple, true, Duration, 0, 0, 1, 0),
                    };
                    e.OnSkillCasted(caster, [target], grids, others);
                }
            }
        }
    }
}
