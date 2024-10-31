using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 精准打击 : Skill
    {
        public override long Id => (long)SuperSkillID.精准打击;
        public override string Name => "精准打击";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override double EPCost => 100;
        public override double CD => 40 - 1 * (Level - 1);
        public override double HardnessTime { get; set; } = 8;

        public 精准打击(Character? character = null) : base(SkillType.SuperSkill, character)
        {
            Effects.Add(new 精准打击特效(this));
        }
    }

    public class 精准打击特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"30 时间内暴击率提升 {暴击率提升 * 100:0.##}%，暴击伤害提升 {暴击伤害提升 * 100:0.##}%，物理穿透提升 {物理穿透提升 * 100:0.##}%。";
        public override bool TargetSelf => true;
        public override bool Durative => true;
        public override double Duration => 30;

        private double 暴击率提升 => 0.2 + 0.03 * (Level - 1);
        private double 暴击伤害提升 => 0.8 + 0.04 * (Level - 1);
        private double 物理穿透提升 => 0.3;
        private double 实际暴击率提升 = 0;
        private double 实际暴击伤害提升 = 0;
        private double 实际物理穿透提升 = 0;

        public override void OnEffectGained(Character character)
        {
            实际暴击率提升 = 暴击率提升;
            实际暴击伤害提升 = 暴击伤害提升;
            实际物理穿透提升 = 物理穿透提升;
            character.ExCritRate += 实际暴击率提升;
            character.ExCritDMG += 实际暴击伤害提升;
            character.PhysicalPenetration += 实际物理穿透提升;
            WriteLine($"[ {character} ] 的暴击率提升了 [ {实际暴击率提升 * 100:0.##}% ]，暴击伤害提升了 [ {实际暴击伤害提升 * 100:0.##}% ]，物理穿透提升了 [ {实际物理穿透提升 * 100:0.##}% ] ！！");
        }

        public override void OnEffectLost(Character character)
        {
            character.ExCritRate -= 实际暴击率提升;
            character.ExCritDMG -= 实际暴击伤害提升;
            character.PhysicalPenetration -= 实际物理穿透提升;
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, Dictionary<string, object> others)
        {
            RemainDuration = Duration;
            if (!caster.Effects.Contains(this))
            {
                实际暴击率提升 = 0;
                实际暴击伤害提升 = 0;
                实际物理穿透提升 = 0;
                caster.Effects.Add(this);
                OnEffectGained(caster);
            }
        }
    }
}
