using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 死亡咆哮 : Skill
    {
        public override long Id => (long)MagicID.死亡咆哮;
        public override string Name => "死亡咆哮";
        public override string Description => string.Join("", Effects.Select(e => e.Description));
        public override string DispelDescription => Effects.Count > 0 ? Effects.First(e => e is 施加概率负面).DispelDescription : "";
        public override double MPCost => Level > 0 ? 70 + (75 * (Level - 1)) : 70;
        public override double CD => Level > 0 ? 55 + (1 * (Level - 1)) : 55;
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

        public 死亡咆哮(Character? character = null) : base(SkillType.Magic, character)
        {
            Effects.Add(new 纯数值伤害(this, 55, 25, DamageType.Magical));
            Effects.Add(new 施加概率负面(this, EffectType.Cripple, true, 3, 0, 1, 0.24, 0.08));
        }
    }
}
