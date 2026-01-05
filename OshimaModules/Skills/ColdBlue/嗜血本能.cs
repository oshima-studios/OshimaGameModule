using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 嗜血本能 : Skill
    {
        public override long Id => (long)SuperSkillID.嗜血本能;
        public override string Name => "嗜血本能";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";
        public override double EPCost => 100;
        public override double CD => 55;
        public override double HardnessTime { get; set; } = 7;
        public override bool CanSelectSelf => true;
        public override bool CanSelectEnemy => false;

        public 嗜血本能(Character? character = null) : base(SkillType.SuperSkill, character)
        {
            Effects.Add(new 嗜血本能特效(this));
        }
    }

    public class 嗜血本能特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"{Duration} {GameplayEquilibriumConstant.InGameTime}内，增强 [ 累积之压 ] 的最大生命值伤害 {最大生命值伤害 * 100:0.##}%，并获得 {吸血 * 100:0.##}% 吸血。";
        public override bool Durative => true;
        public override double Duration => 25;
        public override DispelledType DispelledType => DispelledType.CannotBeDispelled;

        private static double 吸血 => 0.2;
        private double 最大生命值伤害 => 0.015 * Level;

        public override void OnEffectGained(Character character)
        {
            character.Lifesteal += 吸血;
            if (character.Effects.Where(e => e is 累积之压特效).FirstOrDefault() is 累积之压特效 e)
            {
                e.系数 += 最大生命值伤害;
            }
        }

        public override void OnEffectLost(Character character)
        {
            character.Lifesteal -= 吸血;
            if (character.Effects.Where(e => e is 累积之压特效).FirstOrDefault() is 累积之压特效 e)
            {
                e.系数 -= 最大生命值伤害;
            }
        }

        public override async Task OnSkillCasted(Character caster, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            RemainDuration = Duration;
            if (!caster.Effects.Contains(this))
            {
                caster.Effects.Add(this);
                OnEffectGained(caster);
            }
            GamingQueue?.LastRound.AddApplyEffects(caster, EffectType.DamageBoost, EffectType.Lifesteal);
        }
    }
}
