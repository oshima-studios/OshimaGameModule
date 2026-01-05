using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 天堂之吻 : Skill
    {
        public override long Id => (long)SkillID.天堂之吻;
        public override string Name => "天堂之吻";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";
        public override double EPCost => 60;
        public override double CD => 40;
        public override double HardnessTime { get; set; } = 10;
        public override bool CanSelectSelf => true;
        public override bool CanSelectTeammate => true;
        public override bool CanSelectEnemy => false;
        public override int CanSelectTargetCount => 2;

        public 天堂之吻(Character? character = null) : base(SkillType.Skill, character)
        {
            CastRange = 5;
            Effects.Add(new 提升友方行动速度(this, 120, 50, duration: 20));
        }
    }
}
