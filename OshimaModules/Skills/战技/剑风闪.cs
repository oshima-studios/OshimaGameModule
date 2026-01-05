using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 剑风闪 : Skill
    {
        public override long Id => (long)SkillID.剑风闪;
        public override string Name => "剑风闪";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";
        public override double EPCost => 70;
        public override double CD => 28;
        public override double HardnessTime { get; set; } = 10;
        public override int CanSelectTargetRange => 2;

        public 剑风闪(Character? character = null) : base(SkillType.Skill, character)
        {
            Effects.Add(new 基于攻击力的伤害_带基础伤害(this, 55, 50, 0.1, 0.035, DamageType.Physical));
        }
    }
}
