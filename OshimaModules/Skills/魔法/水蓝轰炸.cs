using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 水蓝轰炸 : Skill
    {
        public override long Id => (long)MagicID.水蓝轰炸;
        public override string Name => "水蓝轰炸";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override double MPCost => Level > 0 ? 45 + (50 * (Level - 1)) : 45;
        public override double CD => 30;
        public override double CastTime => 8;
        public override double HardnessTime { get; set; } = 4;
        public override int CanSelectTargetCount => 3;
        public override bool IsNonDirectional => true;
        public override SkillRangeType SkillRangeType => SkillRangeType.Circle;
        public override int CanSelectTargetRange => 3;
        public override double MagicBottleneck => 16 + 17 * (Level - 1);

        public 水蓝轰炸(Character? character = null) : base(SkillType.Magic, character)
        {
            Effects.Add(new 基于核心属性的伤害(this, 15, 35, 0.15, 0.2));
        }
    }
}
