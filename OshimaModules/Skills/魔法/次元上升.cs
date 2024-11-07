using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 次元上升 : Skill
    {
        public override long Id => (long)MagicID.次元上升;
        public override string Name => "次元上升";
        public override string Description => Effects.Count > 0 ? string.Join("\r\n", Effects.Select(e => e.Description)) : "";
        public override double MPCost => Level > 0 ? 70 + (75 * (Level - 1)) : 70;
        public override double CD => 100;
        public override double CastTime => 12;
        public override double HardnessTime { get; set; } = 5;

        public 次元上升(Character? character = null) : base(SkillType.Magic, character)
        {
            Effects.Add(new 造成眩晕(this, true, 15, 0));
            Effects.Add(new 基于攻击力的伤害_带基础伤害(this, 90, 110, 0.5, 0.3));
        }
    }
}
