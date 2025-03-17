using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Effects.SkillEffects
{
    public class 基于攻击力的伤害_带基础伤害 : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"对目标{(Skill.CanSelectTargetCount > 1 ? $"至多 {Skill.CanSelectTargetCount} 个" : "")}敌人造成 {BaseDamage:0.##} + {ATKCoefficient * 100:0.##}% 攻击力 [ {Damage:0.##} ] 点{(IsMagic ? CharacterSet.GetMagicDamageName(MagicType) : "物理伤害")}。";

        private double BaseDamage => Skill.Level > 0 ? 基础数值伤害 + 基础伤害等级成长 * (Skill.Level - 1) : 基础数值伤害;
        private double ATKCoefficient => Skill.Level > 0 ? 基础攻击力系数 + 基础系数等级成长 * (Skill.Level - 1) : 基础攻击力系数;
        private double Damage => BaseDamage + (ATKCoefficient * Skill.Character?.ATK ?? 0);
        private double 基础数值伤害 { get; set; } = 100;
        private double 基础伤害等级成长 { get; set; } = 50;
        private double 基础攻击力系数 { get; set; } = 0.2;
        private double 基础系数等级成长 { get; set; } = 0.2;
        private bool IsMagic { get; set; } = true;

        public 基于攻击力的伤害_带基础伤害(Skill skill, double 基础数值伤害, double 基础伤害等级成长, double 基础攻击力系数, double 基础系数等级成长, bool isMagic = true, MagicType magicType = MagicType.None) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            this.基础数值伤害 = 基础数值伤害;
            this.基础伤害等级成长 = 基础伤害等级成长;
            this.基础攻击力系数 = 基础攻击力系数;
            this.基础系数等级成长 = 基础系数等级成长;
            IsMagic = isMagic;
            MagicType = magicType;
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, Dictionary<string, object> others)
        {
            foreach (Character enemy in targets)
            {
                DamageToEnemy(caster, enemy, IsMagic, MagicType, Damage);
            }
        }
    }
}
