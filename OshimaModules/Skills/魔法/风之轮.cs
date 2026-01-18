using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 风之轮 : Skill
    {
        public override long Id => (long)MagicID.风之轮;
        public override string Name => "风之轮";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override double MPCost => Level > 0 ? 50 + (60 * (Level - 1)) : 50;
        public override double CD => 30;
        public override double CastTime => 10;
        public override double HardnessTime { get; set; } = 4;
        public override double MagicBottleneck => 12 + 13 * (Level - 1);

        public 风之轮(Character? character = null) : base(SkillType.Magic, character)
        {
            Effects.Add(new 基于属性的伤害(this, PrimaryAttribute.AGI, 120, 80, 0.4, 0.4));
        }
    }
}
