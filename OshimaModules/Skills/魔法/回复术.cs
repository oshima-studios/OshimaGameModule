using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 回复术 : Skill
    {
        public override long Id => (long)MagicID.回复术;
        public override string Name => "回复术";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override double MPCost => Level > 0 ? 80 + (90 * (Level - 1)) : 80;
        public override double CD => 65;
        public override double CastTime => 3;
        public override double HardnessTime { get; set; } = 5;
        public override bool CanSelectSelf => true;
        public override bool CanSelectEnemy => false;
        public override bool CanSelectTeammate => true;
        public override int CanSelectTargetCount => 1;

        public 回复术(Character? character = null) : base(SkillType.Magic, character)
        {
            SelectTargetPredicates.Add(c => c.HP > 0 && c.HP < c.MaxHP);
            Effects.Add(new 纯数值回复生命(this, 70, 120));
        }
    }
}
