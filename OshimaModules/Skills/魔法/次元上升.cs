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
        public override string DispelDescription => Effects.Count > 0 ? Effects.First(e => e is 造成眩晕).DispelDescription : "";
        public override string ExemptionDescription => Effects.Count > 0 ? Effects.First(e => e is 造成眩晕).ExemptionDescription : "";
        public override double MPCost => Level > 0 ? 70 + (75 * (Level - 1)) : 70;
        public override double CD => Level > 0 ? 35 - (1.5 * (Level - 1)) : 35;
        public override double CastTime => 12;
        public override double HardnessTime { get; set; } = 5;
        public override double MagicBottleneck => 15 + 21 * (Level - 1);

        public 次元上升(Character? character = null) : base(SkillType.Magic, character)
        {
            Effects.Add(new 基于攻击力的伤害_带基础伤害(this, 35, 25, 0.3, 0.25));
            Effects.Add(new 造成眩晕(this, true, 10, 0));
        }
    }
}
