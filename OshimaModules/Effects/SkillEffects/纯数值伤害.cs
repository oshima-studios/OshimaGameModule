using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Effects.SkillEffects
{
    public class 纯数值伤害 : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"对目标{(TargetCount > 1 ? $"至多 {TargetCount} 个" : "")}敌人造成 {Damage:0.##} 点{(IsMagic ? CharacterSet.GetMagicDamageName(MagicType) : "物理伤害")}。";
        public override bool TargetSelf => false;
        public override int TargetCount { get; set; } = 1;
        private double Damage => Skill.Level > 0 ? 基础数值伤害 + 基础伤害等级成长 * (Skill.Level - 1) : 基础数值伤害;

        private double 基础数值伤害 { get; set; } = 200;
        private double 基础伤害等级成长 { get; set; } = 100;
        private bool IsMagic { get; set; } = true;

        public 纯数值伤害(Skill skill, double 基础数值伤害, double 基础伤害等级成长, bool isMagic = true, MagicType magicType = MagicType.None, int targetCount = 1) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            this.基础数值伤害 = 基础数值伤害;
            this.基础伤害等级成长 = 基础伤害等级成长;
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
