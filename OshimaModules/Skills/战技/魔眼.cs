using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 魔眼 : Skill
    {
        public override long Id => (long)SkillID.魔眼;
        public override string Name => "魔眼";
        public override string Description => string.Join("", Effects.Select(e => e.Description));
        public override string DispelDescription => "被驱散性：迟滞可弱驱散，混乱需强驱散";
        public override double EPCost => 65;
        public override double CD => 24;
        public override double HardnessTime { get; set; } = 8;

        public 魔眼(Character? character = null) : base(SkillType.Skill, character)
        {
            CastRange = 2;
            Effects.Add(new 施加概率负面(this, EffectType.Delay, false, 0, 3, 0, 1, 0, 0.5));
            Effects.Add(new 施加概率负面(this, EffectType.Confusion, false, 0, 2, 0, 0.45, 0.05));
        }
    }
}
