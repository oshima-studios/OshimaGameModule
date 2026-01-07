using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 金刚击 : Skill
    {
        public override long Id => (long)SkillID.金刚击;
        public override string Name => "金刚击";
        public override string Description => string.Join("", Effects.Select(e => e.Description));
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";
        public override string ExemptionDescription => Effects.Count > 0 ? Effects.First(e => e is 打断施法).ExemptionDescription : "";
        public override double EPCost => 60;
        public override double CD => 20;
        public override double HardnessTime { get; set; } = 8;

        public 金刚击(Character? character = null) : base(SkillType.Skill, character)
        {
            Effects.Add(new 基于攻击力的伤害_带基础伤害(this, 65, 65, 0.09,0.04, DamageType.Physical));
            Effects.Add(new 打断施法(this));
        }
    }
}
