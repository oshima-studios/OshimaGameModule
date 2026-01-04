using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 胧 : Skill
    {
        public override long Id => (long)SkillID.胧;
        public override string Name => "胧";
        public override string Description => string.Join("", Effects.Select(e => e.Description));
        public override string DispelDescription => Effects.Count > 0 ? Effects.First(e => e is 施加概率负面).DispelDescription : "";
        public override double EPCost => 60;
        public override double CD => 20;
        public override double HardnessTime { get; set; } = 9;

        public 胧(Character? character = null) : base(SkillType.Skill, character)
        {
            CastRange = 4;
            Effects.Add(new 基于攻击力的伤害_带基础伤害(this, 60, 45, 0.065, 0.035, DamageType.Physical));
            Effects.Add(new 施加概率负面(this, EffectType.Cripple, false, 0, 1, 0, 0.45, 0.05));
        }
    }
}
