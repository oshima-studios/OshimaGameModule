using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Skills;

namespace Oshima.FunGame.OshimaModules.Effects.SkillEffects
{
    public class 纯数值伤害 : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"对{Skill.TargetDescription()}造成 {Damage:0.##} 点{CharacterSet.GetDamageTypeName(DamageType, MagicType)}。";

        private double Damage => Skill.Level > 0 ? 基础数值伤害 + 基础伤害等级成长 * (Skill.Level - 1) : 基础数值伤害;
        private double 基础数值伤害 { get; set; } = 200;
        private double 基础伤害等级成长 { get; set; } = 100;
        private DamageType DamageType { get; set; } = DamageType.Magical;

        public 纯数值伤害(Skill skill, double 基础数值伤害, double 基础伤害等级成长, DamageType damageType, MagicType magicType = MagicType.None) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            this.基础数值伤害 = 基础数值伤害;
            this.基础伤害等级成长 = 基础伤害等级成长;
            DamageType = damageType;
            MagicType = magicType;
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            foreach (Character enemy in targets)
            {
                DamageToEnemy(caster, enemy, DamageType, MagicType, Damage);
            }
        }
    }
}
