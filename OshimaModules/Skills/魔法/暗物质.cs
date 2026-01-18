using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 暗物质 : Skill
    {
        public override long Id => (long)MagicID.暗物质;
        public override string Name => "暗物质";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override double MPCost => Level > 0 ? 70 + (80 * (Level - 1)) : 70;
        public override double CD => 40;
        public override double CastTime => 10;
        public override double HardnessTime { get; set; } = 4;
        public override double MagicBottleneck => 16 + 17 * (Level - 1);

        public 暗物质(Character? character = null) : base(SkillType.Magic, character)
        {
            Effects.Add(new 基于攻击力的伤害_无基础伤害(this, 1.3, 0.28));
        }
    }
}
