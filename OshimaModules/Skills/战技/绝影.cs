using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 绝影 : Skill
    {
        public override long Id => (long)SkillID.绝影;
        public override string Name => "绝影";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";
        public override double EPCost => 45;
        public override double CD => 12;
        public override double HardnessTime { get; set; } = 7;

        public 绝影(Character? character = null) : base(SkillType.Skill, character)
        {
            Effects.Add(new 基于攻击力的伤害_带基础伤害(this, 75, 70, 0.075, 0.055, DamageType.Physical));
        }
    }
}
