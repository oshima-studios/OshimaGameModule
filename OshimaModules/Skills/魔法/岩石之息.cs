using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 岩石之息 : Skill
    {
        public override long Id => (long)MagicID.岩石之息;
        public override string Name => "岩石之息";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override double MPCost => Level > 0 ? 40 + (35 * (Level - 1)) : 40;
        public override double CD => 32;
        public override double CastTime => 9;
        public override double HardnessTime { get; set; } = 5;
        public override int CanSelectTargetCount => 3;
        public override bool IsNonDirectional => true;
        public override SkillRangeType SkillRangeType => SkillRangeType.LinePass;
        public override int CanSelectTargetRange => 2;

        public 岩石之息(Character? character = null) : base(SkillType.Magic, character)
        {
            Effects.Add(new 基于属性的伤害(this, PrimaryAttribute.STR, 30, 10, 0.30, 0.2));
        }
    }
}
