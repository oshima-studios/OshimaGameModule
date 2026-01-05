using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 回复弹 : Skill
    {
        public override long Id => (long)SkillID.回复弹;
        public override string Name => "回复弹";
        public override string Description => string.Join("", Effects.Select(e => e.Description));
        public override string DispelDescription => Effects.Count > 0 ? Effects.First(e => e is 弱驱散特效).DispelDescription : "";
        public override double EPCost => 50;
        public override double CD => 45;
        public override double HardnessTime { get; set; } = 9;
        public override bool CanSelectSelf => true;
        public override bool CanSelectTeammate => true;
        public override bool CanSelectEnemy => false;
        public override int CanSelectTargetCount => 1;

        public 回复弹(Character? character = null) : base(SkillType.Skill, character)
        {
            CastRange = 4;
            Effects.Add(new 纯数值回复生命(this, 200, 150));
            Effects.Add(new 弱驱散特效(this));
        }
    }
}
