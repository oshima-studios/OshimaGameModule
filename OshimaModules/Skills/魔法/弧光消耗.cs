using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 弧光消耗 : Skill
    {
        public override long Id => (long)MagicID.弧光消耗;
        public override string Name => "弧光消耗";
        public override string Description => string.Join("", Effects.Select(e => e.Description));
        public override double MPCost => Level > 0 ? 35 + (30 * (Level - 1)) : 35;
        public override double CD => 35;
        public override double CastTime => 4;
        public override double HardnessTime { get; set; } = 5;
        public override int CanSelectTargetCount => 3;
        public override bool IsNonDirectional => true;
        public override SkillRangeType SkillRangeType => SkillRangeType.Circle;
        public override int CanSelectTargetRange => 3;

        public 弧光消耗(Character? character = null) : base(SkillType.Magic, character)
        {
            Effects.Add(new 吸取魔法值(this, 35, 30, false, 0.7));
            Effects.Add(new 吸取能量值(this, 4, 3, false, 0.3));
        }
    }
}
