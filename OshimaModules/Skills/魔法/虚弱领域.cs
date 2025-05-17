using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 虚弱领域 : Skill
    {
        public override long Id => (long)MagicID.虚弱领域;
        public override string Name => "虚弱领域";
        public override string Description => Effects.Count > 0 ? string.Join("\r\n", Effects.Select(e => e.Description)) : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First(e => e is 造成虚弱).DispelDescription : "";
        public override double MPCost => Level > 0 ? 95 + (95 * (Level - 1)) : 90;
        public override double CD => Level > 0 ? 85 - (1 * (Level - 1)) : 85;
        public override double CastTime => 8;
        public override double HardnessTime { get; set; } = 3;
        public override bool SelectAllEnemies => true;

        public 虚弱领域(Character? character = null) : base(SkillType.Magic, character)
        {
            Effects.Add(new 造成虚弱(this, false, 0, 3, 0, 0.1, 1, 0.1, 0.25));
        }
    }
}
