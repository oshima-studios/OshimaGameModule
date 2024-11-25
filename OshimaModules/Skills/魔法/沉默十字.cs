using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 沉默十字 : Skill
    {
        public override long Id => (long)MagicID.沉默十字;
        public override string Name => "沉默十字";
        public override string Description => Effects.Count > 0 ? string.Join("\r\n", Effects.Select(e => e.Description)) : "";
        public override double MPCost => Level > 0 ? 65 + (65 * (Level - 1)) : 65;
        public override double CD => Level > 0 ? 85 - (1.5 * (Level - 1)) : 85;
        public override double CastTime => Level > 0 ? 10 - (0.5 * (Level - 1)) : 10;
        public override double HardnessTime { get; set; } = 10;

        public 沉默十字(Character? character = null) : base(SkillType.Magic, character)
        {
            Effects.Add(new 造成封技(this, false, 0, 2));
        }
    }
}
