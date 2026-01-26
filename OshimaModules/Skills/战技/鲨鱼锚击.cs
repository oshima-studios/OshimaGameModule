using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 鲨鱼锚击 : Skill
    {
        public override long Id => (long)SkillID.鲨鱼锚击;
        public override string Name => "鲨鱼锚击";
        public override string Description => string.Join("", Effects.Select(e => e.Description));
        public override string DispelDescription => Effects.Count > 0 ? Effects.First(e => e is 施加概率负面).DispelDescription : "";
        public override string ExemptionDescription => Effects.Count > 0 ? Effects.First(e => e is 施加概率负面).ExemptionDescription : "";
        public override double EPCost => 55;
        public override double CD => 20;
        public override double HardnessTime { get; set; } = 8;

        public 鲨鱼锚击(Character? character = null) : base(SkillType.Skill, character)
        {
            CastRange = 4;
            Effects.Add(new 基于攻击力的伤害_带基础伤害(this, 50, 40, 0.055, 0.04, DamageType.Physical));
            Effects.Add(new 施加概率负面(this, EffectType.Disarm, false, 0, 1, 0, 1, 0));
        }
    }
}
