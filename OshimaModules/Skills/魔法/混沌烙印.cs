using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 混沌烙印 : Skill
    {
        public override long Id => (long)MagicID.混沌烙印;
        public override string Name => "混沌烙印";
        public override string Description => Effects.Count > 0 ? string.Join("\r\n", Effects.Select(e => e.Description)) : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First(e => e is 造成气绝).DispelDescription : "";
        public override double MPCost => Level > 0 ? 65 + (70 * (Level - 1)) : 65;
        public override double CD => Level > 0 ? 65 - (1.5 * (Level - 1)) : 65;
        public override double CastTime => 4;
        public override double HardnessTime { get; set; } = 5;
        public override int CanSelectTargetCount => 1;

        public 混沌烙印(Character? character = null) : base(SkillType.Magic, character)
        {
            Effects.Add(new 造成气绝(this, true, 4, 0, 0.5, true, 0, 0.05));
        }
    }
}
