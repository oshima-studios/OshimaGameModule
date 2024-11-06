using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 心灵之霞 : Skill
    {
        public override long Id => (long)MagicID.心灵之霞;
        public override string Name => "心灵之霞";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override double MPCost => Level > 0 ? 35 + (50 * (Level - 1)) : 35;
        public override double CD => 30;
        public override double CastTime => 6;
        public override double HardnessTime { get; set; } = 3;

        public 心灵之霞(Character? character = null) : base(SkillType.Magic, character)
        {
            Effects.Add(new 基于属性的伤害(this, PrimaryAttribute.INT, 80, 55, 0.5, 0.6));
        }
    }
}
