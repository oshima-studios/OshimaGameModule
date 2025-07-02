using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 回复术改 : Skill
    {
        public override long Id => (long)MagicID.回复术改;
        public override string Name => "回复术·改";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override double MPCost => Level > 0 ? 80 + (80 * (Level - 1)) : 80;
        public override double CD => 80;
        public override double CastTime => 3;
        public override double HardnessTime { get; set; } = 7;
        public override bool CanSelectSelf => true;
        public override bool CanSelectEnemy => false;
        public override bool CanSelectTeammate => true;
        public override int CanSelectTargetCount => 1;

        public 回复术改(Character? character = null) : base(SkillType.Magic, character)
        {
            SelectTargetPredicates.Add(c => c.HP > 0 && c.HP < c.MaxHP);
            Effects.Add(new 百分比回复生命值(this, 0.36, 0.04));
        }
    }
}
