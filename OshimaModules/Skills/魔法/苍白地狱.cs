using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 苍白地狱 : Skill
    {
        public override long Id => (long)MagicID.苍白地狱;
        public override string Name => "苍白地狱";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override double MPCost => Level > 0 ? 45 + (55 * (Level - 1)) : 45;
        public override double CD => 35;
        public override double CastTime => 9;
        public override double HardnessTime { get; set; } = 3;
        public override int CanSelectTargetCount => 3;
        public override double MagicBottleneck => 15 + 22 * (Level - 1);

        public 苍白地狱(Character? character = null) : base(SkillType.Magic, character)
        {
            Effects.Add(new 基于属性的伤害(this, PrimaryAttribute.INT, 30, 25, 0.25, 0.2));
        }
    }
}
