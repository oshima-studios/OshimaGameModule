using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 养命功 : Skill
    {
        public override long Id => (long)SkillID.养命功;
        public override string Name => "养命功";
        public override string Description => string.Join("", Effects.Select(e => e.Description));
        public override string DispelDescription => Effects.Count > 0 ? Effects.First(e => e is 施加持续性弱驱散).DispelDescription : "";
        public override double EPCost => 55;
        public override double CD => 45;
        public override double HardnessTime { get; set; } = 10;
        public override bool CanSelectSelf => true;
        public override bool CanSelectEnemy => false;
        public override bool CanSelectTeammate => false;

        public 养命功(Character? character = null) : base(SkillType.Skill, character)
        {
            Effects.Add(new 施加持续性弱驱散(this, durationTurn: 3));
        }
    }
}
