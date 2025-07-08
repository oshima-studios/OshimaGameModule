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
        public override double CD => Level > 0 ? 70 + (2 * (Level - 1)) : 70;
        public override double CastTime => 9;
        public override double HardnessTime { get; set; } = 6;
        public override int CanSelectTargetCount
        {
            get
            {
                return Level switch
                {
                    4 => 2,
                    5 => 2,
                    6 => 2,
                    7 => 3,
                    8 => 3,
                    _ => 1
                };
            }
        }

        public 钻石星尘(Character? character = null) : base(SkillType.Magic, character)
        {
            Effects.Add(new 纯数值伤害(this, 60, 30, DamageType.Magical));
            Effects.Add(new 施加概率负面(this, EffectType.Freeze, false, 0, 2, 0, 0.45, 0.05));
            Effects.Add(new 施加概率负面(this, EffectType.Vulnerable, false, 0, 3, 0, 0.45, 0.05, DamageType.Magical, 0.3));
        }
    }
}
