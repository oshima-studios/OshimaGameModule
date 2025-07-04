using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 破碎虚空 : Skill
    {
        public override long Id => (long)MagicID.破碎虚空;
        public override string Name => "破碎虚空";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override double MPCost => Level > 0 ? 80 + (80 * (Level - 1)) : 80;
        public override double CD => 60;
        public override double CastTime => 10;
        public override double HardnessTime { get; set; } = 6;
        public override int CanSelectTargetCount => 3;

        public 破碎虚空(Character? character = null) : base(SkillType.Magic, character)
        {
            Effects.Add(new 基于攻击力的伤害_无基础伤害(this, 0.60, 0.1));
        }
    }
}
