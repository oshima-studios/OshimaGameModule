using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model;
using Milimoe.FunGame.Core.Model.PrefabricatedEntity;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 漆黑之牙 : SoulboundSkill
    {
        public override long Id => (long)SuperSkillID.漆黑之牙;
        public override string Name => "漆黑之牙";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";
        public override double CD => 80;
        public override double HardnessTime { get; set; } = 13;
        public override bool SelectAllEnemies => true;
        public int SkillKills { get; set; } = 0;

        public 漆黑之牙(Character? character = null) : base(character)
        {
            Effects.Add(new 漆黑之牙特效(this));
        }

        public override void OnCharacterRespawn(Skill newSkill)
        {
            if (newSkill is 漆黑之牙 s)
            {
                s.SkillKills = SkillKills;
            }
        }
    }

    public class 漆黑之牙特效(SoulboundSkill skill) : SoulboundEffect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"基于{Skill.SkillOwner()}的 {ATKCoefficient * 100:0.##}% 攻击力 [ {Damage:0.##} ] 对{Skill.TargetDescription()}造成{CharacterSet.GetDamageTypeName(DamageType.Physical)}。" +
            $"每当此技能造成一名敌人死亡，永久提升 4% 伤害，当前提升：{SkillKillsCoefficient * 100:0.##}% [ {SkillKillsDamage:0.##} ] 点；" +
            (Improvement > 0 ? $"灵魂绑定伤害加成： {Improvement * 100:0.##}% [ {ImprovementDamage:0.##} ] 点；" : "") +
            $"总伤害 {Damage + SkillKillsDamage + ImprovementDamage:0.##} 点。";

        public 漆黑之牙? CSkill => Skill is 漆黑之牙 s ? s : null;
        public double ATKCoefficient => 0.4 + 0.1 * (Skill.Level - 1);
        public double Damage => (Skill.Character?.ATK ?? 0) * ATKCoefficient;
        public double ImprovementDamage => Improvement > 0 ? Damage * Improvement : 0;
        public double SkillKillsCoefficient => CSkill is null ? 0 : CSkill.SkillKills * 0.04;
        public double SkillKillsDamage => SkillKillsCoefficient > 0 ? Damage * SkillKillsCoefficient : 0;

        public override void OnSkillCasted(Character caster, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            foreach (Character target in targets)
            {
                DamageCalculationOptions options = new(caster);
                DamageToEnemy(caster, target, DamageType.Physical, MagicType.None, Damage + SkillKillsDamage + ImprovementDamage, options);
                if (target.HP <= 0 && CSkill != null)
                {
                    CSkill.SkillKills++;
                }
            }
        }
    }
}
