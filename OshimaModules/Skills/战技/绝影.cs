using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 绝影 : Skill
    {
        public override long Id => (long)SkillID.绝影;
        public override string Name => "绝影";
        public override string Description => string.Join("", Effects.Select(e => e.Description));
        public override string DispelDescription => Effects.Count > 0 ? Effects.First(e => e is 施加概率负面).DispelDescription : "";
        public override double EPCost => 60;
        public override double CD => 18;
        public override double HardnessTime { get; set; } = 7;

        public 绝影(Character? character = null) : base(SkillType.Skill, character)
        {
            CastRange = 6;
            Effects.Add(new 基于攻击力的伤害_带基础伤害(this, 70, 55, 0.095, 0.045, DamageType.Physical));
            Effects.Add(new 施加概率负面(this, EffectType.Delay, true, 15, 0, 0, 1, 0, 0.3));
        }
    }
}
