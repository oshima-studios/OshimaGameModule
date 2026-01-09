using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 全军出击 : Skill
    {
        public override long Id => (long)SuperSkillID.全军出击;
        public override string Name => "全军出击";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";
        public override double EPCost => 100;
        public override double CD => 80;
        public override double HardnessTime { get; set; } = 5;
        public override bool CanSelectSelf => true;
        public override bool CanSelectEnemy => false;

        public 全军出击(Character? character = null) : base(SkillType.SuperSkill, character)
        {
            Effects.Add(new 全军出击特效(this));
        }
    }

    public class 全军出击特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"将雇佣兵数量立即补全至 {雇佣兵团特效.最大数量} 名，持续 {持续时间} 秒。每名雇佣兵额外为{Skill.SkillOwner()}提供 {攻击力 * 100:0.##}% 攻击力和 {行动速度:0.##} 点行动速度、{加速系数 * 100:0.##}% 加速系数、{冷却缩减 * 100:0.##}% 冷却缩减，每名雇佣兵提升 {攻击力提升 * 100:0.##}% 攻击力。";
        public override DispelledType DispelledType => DispelledType.CannotBeDispelled;

        public double 持续时间 => 30 + 3 * (Skill.Level - 1);
        public const double 攻击力 = 0.05;
        public const double 冷却缩减 = 0.05;
        public const double 加速系数 = 0.05;
        public const double 行动速度 = 25;
        public const double 攻击力提升 = 0.4;

        public override void OnSkillCasted(Character caster, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            RemainDuration = Duration;
            if (!caster.Effects.Contains(this))
            {
                caster.Effects.Add(this);
                OnEffectGained(caster);
            }
            GamingQueue?.LastRound.AddApplyEffects(caster, EffectType.CritBoost, EffectType.PenetrationBoost);
        }
    }
}
