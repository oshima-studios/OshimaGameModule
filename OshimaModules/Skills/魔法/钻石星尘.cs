using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 钻石星尘 : Skill
    {
        public override long Id => (long)MagicID.钻石星尘;
        public override string Name => "钻石星尘";
        public override string Description => string.Join("", Effects.Select(e => e.Description));
        public override string DispelDescription => "被驱散性：冻结需强驱散，易伤可弱驱散";
        public override double MPCost => Level > 0 ? 80 + (75 * (Level - 1)) : 80;
        public override double CD => Level > 0 ? 35 + (2 * (Level - 1)) : 35;
        public override double CastTime => 9;
        public override double HardnessTime { get; set; } = 6;
        public override int CanSelectTargetCount
        {
            get
            {
                return Level switch
                {
                    4 or 5 or 6 => 2,
                    7 or 8 => 3,
                    _ => 1
                };
            }
        }
        public override bool IsNonDirectional => true;
        public override SkillRangeType SkillRangeType => SkillRangeType.Circle;
        public override int CanSelectTargetRange => 2;
        public override double MagicBottleneck => 35 + 24 * (Level - 1);

        public 钻石星尘(Character? character = null) : base(SkillType.Magic, character)
        {
            ExemptionDescription = $"冻结{SkillSet.GetExemptionDescription(EffectType.Freeze)}\r\n易伤{SkillSet.GetExemptionDescription(EffectType.Vulnerable)}";
            Effects.Add(new 纯数值伤害(this, 60, 30, DamageType.Magical));
            Effects.Add(new 施加概率负面(this, EffectType.Freeze, false, 0, 1, 0, 0.45, 0.05));
            Effects.Add(new 施加概率负面(this, EffectType.Vulnerable, false, 0, 3, 0, 0.45, 0.05, DamageType.Magical, 0.3));
        }
    }
}
