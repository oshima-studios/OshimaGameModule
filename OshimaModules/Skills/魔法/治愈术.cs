using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 治愈术 : Skill
    {
        public override long Id => (long)MagicID.治愈术;
        public override string Name => "治愈术";
        public override string Description => string.Join("", Effects.Select(e => e.Description));
        public override double MPCost => Level > 0 ? 80 + (85 * (Level - 1)) : 80;
        public override double CD => Level > 0 ? 80 - (1 * (Level - 1)) : 80;
        public override double CastTime => Level > 0 ? 5 + (0.25 * (Level - 1)) : 5;
        public override double HardnessTime { get; set; } = 6;
        public override bool CanSelectSelf => true;
        public override bool CanSelectEnemy => false;
        public override bool CanSelectTeammate => true;
        public override int CanSelectTargetCount => 1;

        public 治愈术(Character? character = null) : base(SkillType.Magic, character)
        {
            SelectTargetPredicates.Add(c => c.HP > 0 && c.HP < c.MaxHP);
            Effects.Add(new 弱驱散特效(this));
            Effects.Add(new 百分比回复生命值(this, 0.28, 0.03));
        }
    }
}
