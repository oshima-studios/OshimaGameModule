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
        public override string ExemptionDescription => Effects.Count > 0 ? Effects.First().ExemptionDescription : "";
        public override double MPCost => Level > 0 ? 65 + (75 * (Level - 1)) : 65;
        public override double CD => Level > 0 ? 28 - (1 * (Level - 1)) : 28;
        public override double CastTime => Level > 0 ? 2 + (2 * (Level - 1)) : 2;
        public override double HardnessTime { get; set; } = 5;
        public override double MagicBottleneck => 12 + 13 * (Level - 1);

        public 时间减速(Character? character = null) : base(SkillType.Magic, character)
        {
            Effects.Add(new 降低敌方行动速度(this, 65, 35));
        }
    }
}
