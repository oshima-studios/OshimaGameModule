using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 等离子之波 : Skill
    {
        public override long Id => (long)MagicID.等离子之波;
        public override string Name => "等离子之波";
        public override string Description => string.Join("", Effects.Select(e => e.Description));
        public override string DispelDescription => Effects.Count > 0 ? Effects.First(e => e is 施加概率负面).DispelDescription : "";
        public override string ExemptionDescription => Effects.Count > 0 ? Effects.First(e => e is 施加概率负面).ExemptionDescription : "";
        public override double MPCost => Level > 0 ? 70 + (75 * (Level - 1)) : 70;
        public override double CD => Level > 0 ? 20 + (1.5 * (Level - 1)) : 20;
        public override double CastTime => 6;
        public override double HardnessTime { get; set; } = 6;
        public override int CanSelectTargetCount
        {
            get
            {
                return Level switch
                {
                    4 => 2,
                    5 => 2,
                    6 => 3,
                    7 => 3,
                    8 => 4,
                    _ => 1
                };
            }
        }

        public 等离子之波(Character? character = null) : base(SkillType.Magic, character)
        {
            Effects.Add(new 纯数值伤害(this, 60, 20, DamageType.Magical));
            Effects.Add(new 施加概率负面(this, EffectType.Silence, false, 0, 2, 0, 0.45, 0.05));
        }
    }
}
