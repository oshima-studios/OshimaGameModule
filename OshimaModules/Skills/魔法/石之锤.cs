using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 石之锤 : Skill
    {
        public override long Id => (long)MagicID.石之锤;
        public override string Name => "石之锤";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override double MPCost => Level > 0 ? 35 + (30 * (Level - 1)) : 35;
        public override double CD => 22;
        public override double CastTime => 6;
        public override double HardnessTime { get; set; } = 4;
        public override double MagicBottleneck => 12 + 13 * (Level - 1);

        public 石之锤(Character? character = null) : base(SkillType.Magic, character)
        {
            Effects.Add(new 基于属性的伤害(this, PrimaryAttribute.STR, 100, 50, 0.55, 0.4));
        }
    }
}
