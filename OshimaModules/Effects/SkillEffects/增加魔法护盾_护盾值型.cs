using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Skills;

namespace Oshima.FunGame.OshimaModules.Effects.SkillEffects
{
    public class 增加魔法护盾_护盾值型 : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"为{Skill.TargetDescription()}提供 {护盾值:0.##} 点魔法护盾值。";

        private double 护盾值 => Level > 0 ? Math.Abs(基础数值护盾 + 基础护盾等级成长 * (Level - 1)) : Math.Abs(基础数值护盾);
        private double 基础数值护盾 { get; set; } = 200;
        private double 基础护盾等级成长 { get; set; } = 100;

        public 增加魔法护盾_护盾值型(Skill skill, double 基础数值护盾, double 基础护盾等级成长) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            this.基础数值护盾 = 基础数值护盾;
            this.基础护盾等级成长 = 基础护盾等级成长;
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            foreach (Character target in targets)
            {
                target.Shield[true, MagicType.None] += 护盾值;
                WriteLine($"[ {target} ] 获得了 {护盾值:0.##} 点魔法护盾值！");
                GamingQueue?.LastRound.AddApplyEffects(target, EffectType.Shield);
            }
        }
    }
}
