using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 零式灭杀 : Skill
    {
        public override long Id => (long)SuperSkillID.零式灭杀;
        public override string Name => "零式灭杀";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";
        public override double EPCost => 100;
        public override double CD => 60 - 1.5 * (Level - 1);
        public override double HardnessTime { get; set; } = 8;
        public override bool CanSelectSelf => true;
        public override bool CanSelectEnemy => false;

        public 零式灭杀(Character? character = null) : base(SkillType.SuperSkill, character)
        {
            Effects.Add(new 零式灭杀特效(this));
        }
    }

    public class 零式灭杀特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"{Skill.SkillOwner()}展现零式剑法精准而凶残的奥义。30 {GameplayEquilibriumConstant.InGameTime}内暴击率提升 {暴击率提升 * 100:0.##}%，暴击伤害提升 {暴击伤害提升 * 100:0.##}%，物理穿透提升 {物理穿透提升 * 100:0.##}%。";
        public override bool Durative => true;
        public override double Duration => 30;
        public override DispelledType DispelledType => DispelledType.CannotBeDispelled;

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

        public override void OnSkillCasted(Character caster, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
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
            GamingQueue?.LastRound.AddApplyEffects(caster, EffectType.CritBoost, EffectType.PenetrationBoost);
        }
    }
}
