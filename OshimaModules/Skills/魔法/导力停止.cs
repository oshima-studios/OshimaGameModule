using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 导力停止 : Skill
    {
        public override long Id => (long)MagicID.导力停止;
        public override string Name => "导力停止";
        public override string Description => string.Join("", Effects.Select(e => e.Description));
        public override string DispelDescription => Effects.Count > 0 ? Effects.First(e => e is 施加概率负面).DispelDescription : "";
        public override string ExemptionDescription => Effects.Count > 0 ? Effects.First(e => e is 施加概率负面).ExemptionDescription : "";
        public override double MPCost => Level > 0 ? 120 + (80 * (Level - 1)) : 120;
        public override double CD => Level > 0 ? 60 - (1.5 * (Level - 1)) : 60;
        public override double CastTime => Level > 0 ? 3 + (0.5 * (Level - 1)) : 3;
        public override double HardnessTime { get; set; } = 8;
        public override bool SelectAllEnemies => true;
        public override double MagicBottleneck => 28 + 24 * (Level - 1);

        public 导力停止(Character? character = null) : base(SkillType.Magic, character)
        {
            Effects.Add(new 纯数值伤害(this, 45, 20, DamageType.Magical));
            Effects.Add(new 施加概率负面(this, EffectType.Silence, true, 8, 0, 1.2, 0.2, 0.03));
        }
    }
}
