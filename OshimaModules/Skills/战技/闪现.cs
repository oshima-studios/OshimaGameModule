using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    [Obsolete("非指向性技能测试，请勿使用")]
    public class 闪现 : Skill
    {
        public override long Id => (long)SkillID.闪现;
        public override string Name => "闪现";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";
        public override double EPCost => 25;
        public override double CD => 25;
        public override double HardnessTime { get; set; } = 3;
        public override bool IsNonDirectional => true;

        public 闪现(Character? character = null) : base(SkillType.Skill, character)
        {
            Effects.Add(new 闪现特效(this));
        }
    }

    public class 闪现特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"立即将角色传送到范围内的任意一个没有被角色占据的指定地点。";
        public override string DispelDescription => "";

        public override void OnSkillCasted(Character caster, List<Character> targets, Dictionary<string, object> others)
        {

        }
    }
}
