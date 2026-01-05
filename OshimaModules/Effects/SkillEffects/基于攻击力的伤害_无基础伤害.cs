using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Skills;

namespace Oshima.FunGame.OshimaModules.Effects.SkillEffects
{
    public class 基于攻击力的伤害_无基础伤害 : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"对{Skill.TargetDescription()}造成 {ATKCoefficient * 100:0.##}% 攻击力 [ {Damage:0.##} ] 点{CharacterSet.GetDamageTypeName(DamageType, MagicType)}。";

        private double ATKCoefficient => Skill.Level > 0 ? 基础攻击力系数 + 基础系数等级成长 * (Skill.Level - 1) : 基础攻击力系数;
        private double Damage => ATKCoefficient * Skill.Character?.ATK ?? 0;
        private double 基础攻击力系数 { get; set; } = 1.5;
        private double 基础系数等级成长 { get; set; } = 0.25;
        private DamageType DamageType { get; set; } = DamageType.Magical;

        public 基于攻击力的伤害_无基础伤害(Skill skill, double 基础攻击力系数, double 基础系数等级成长, DamageType damageType = DamageType.Magical, MagicType magicType = MagicType.None) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            this.基础攻击力系数 = 基础攻击力系数;
            this.基础系数等级成长 = 基础系数等级成长;
            DamageType = damageType;
            MagicType = magicType;
        }

        public override async Task OnSkillCasted(Character caster, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            foreach (Character enemy in targets)
            {
                DamageToEnemy(caster, enemy, DamageType, MagicType, Damage);
            }
        }
    }
}
