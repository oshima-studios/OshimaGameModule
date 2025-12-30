using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 旋风轮 : Skill
    {
        public override long Id => (long)SkillID.旋风轮;
        public override string Name => "旋风轮";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";
        public override double EPCost => 80;
        public override double CD => 25;
        public override double HardnessTime { get; set; } = 8;
        public override int CanSelectTargetCount => 5;

        public 旋风轮(Character? character = null) : base(SkillType.Skill, character)
        {
            CastRange = 6;
            Effects.Add(new 基于攻击力的伤害_带基础伤害(this, 40, 55, 0.1, 0.03, DamageType.Physical));
        }
    }
}
