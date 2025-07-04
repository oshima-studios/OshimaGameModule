using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 时间减速 : Skill
    {
        public override long Id => (long)MagicID.时间减速;
        public override string Name => "时间减速";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override double MPCost => Level > 0 ? 75 + (85 * (Level - 1)) : 75;
        public override double CD => Level > 0 ? 60 - (1 * (Level - 1)) : 60;
        public override double CastTime => Level > 0 ? 7 + (1.5 * (Level - 1)) : 7;
        public override double HardnessTime { get; set; } = 8;

        public 时间减速(Character? character = null) : base(SkillType.Magic, character)
        {
            Effects.Add(new 降低敌方行动速度(this, 60, 30));
        }
    }
}
