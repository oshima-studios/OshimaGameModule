using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 地狱之门 : Skill
    {
        public override long Id => (long)MagicID.地狱之门;
        public override string Name => "地狱之门";
        public override string Description => string.Join("", Effects.Select(e => e.Description));
        public override string DispelDescription => Effects.Count > 0 ? Effects.First(e => e is 施加概率负面).DispelDescription : "";
        public override string ExemptionDescription => Effects.Count > 0 ? Effects.First(e => e is 施加概率负面).ExemptionDescription : "";
        public override double MPCost => Level > 0 ? 75 + (60 * (Level - 1)) : 75;
        public override double CD => Level > 0 ? 30 + (0.8 * (Level - 1)) : 30;
        public override double CastTime => 3;
        public override double HardnessTime { get; set; } = 8;
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

        public 地狱之门(Character? character = null) : base(SkillType.Magic, character)
        {
            Effects.Add(new 纯数值伤害(this, 75, 15, DamageType.Magical));
            Effects.Add(new 施加概率负面(this, EffectType.Bleed, true, 3, 0, 0.5, 0.45, 0.05, false, 85.0));
        }
    }
}
