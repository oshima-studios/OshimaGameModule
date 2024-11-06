using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Effects.SkillEffects
{
    public class 基于攻击力的伤害_无基础伤害 : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"对目标{(TargetCount > 1 ? $"至多 {TargetCount} 个" : "")}敌人造成 {ATKCoefficient * 100:0.##}% 攻击力 [ {Damage:0.##} ] 点{(IsMagic ? CharacterSet.GetMagicDamageName(MagicType) : "物理伤害")}。";
        public override bool TargetSelf => false;
        public override int TargetCount { get; set; } = 1;
        private double ATKCoefficient => Skill.Level > 0 ? 基础攻击力系数 + 基础系数等级成长 * (Skill.Level - 1) : 基础攻击力系数;
        private double Damage => ATKCoefficient * Skill.Character?.ATK ?? 0;

        private double 基础攻击力系数 { get; set; } = 1.5;
        private double 基础系数等级成长 { get; set; } = 0.25;
        private bool IsMagic { get; set; } = true;

        public 基于攻击力的伤害_无基础伤害(Skill skill, double 基础攻击力系数, double 基础系数等级成长, bool isMagic = true, MagicType magicType = MagicType.None, int targetCount = 1) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            this.基础攻击力系数 = 基础攻击力系数;
            this.基础系数等级成长 = 基础系数等级成长;
            IsMagic = isMagic;
            MagicType = magicType;
            TargetCount = targetCount;
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
