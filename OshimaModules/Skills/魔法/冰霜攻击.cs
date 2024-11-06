using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 冰霜攻击 : Skill
    {
        public override long Id => (long)MagicID.冰霜攻击;
        public override string Name => "冰霜攻击";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override double MPCost => Level > 0 ? 50 + (50 * (Level - 1)) : 50;
        public override double CD => 25;
        public override double CastTime => 8;
        public override double HardnessTime { get; set; } = 3;

        public 冰霜攻击(Character? character = null) : base(SkillType.Magic, character)
        {
            Effects.Add(new 基于属性的伤害(this, PrimaryAttribute.INT, 90, 60, 0.35, 0.4));
        }
    }
}
