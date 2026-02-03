using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 全军出击 : Skill
    {
        public override long Id => (long)SuperSkillID.全军出击;
        public override string Name => "全军出击";
        public override string Description => Effects.Count > 0 ? ((全军出击特效)Effects.First()).GeneralDescription : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";
        public override double EPCost => 100;
        public override double CD => 100;
        public override double HardnessTime { get; set; } = 5;
        public override bool CanSelectSelf => true;
        public override bool CanSelectEnemy => false;

        public 全军出击(Character? character = null) : base(SkillType.SuperSkill, character)
        {
            Effects.Add(new 全军出击特效(this));
        }
    }

    public class 全军出击特效 : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description { get; set; } = "";
        public override DispelledType DispelledType => DispelledType.CannotBeDispelled;
        public string GeneralDescription => $"将雇佣兵数量立即补全至 {雇佣兵团特效.最大数量} 名，每名雇佣兵的生命值回复至满并提升 {攻击力提升 * 100:0.##}% 攻击力。在 {持续时间} {GameplayEquilibriumConstant.InGameTime}内，场上的每名雇佣兵额外为{Skill.SkillOwner()}提供 {攻击力 * 100:0.##}% 攻击力和 {行动速度:0.##} 点行动速度、{加速系数 * 100:0.##}% 加速系数、{冷却缩减 * 100:0.##}% 冷却缩减。";
        public override bool Durative => true;
        public override double Duration => 持续时间;

        public double 持续时间 => 20 + 2 * (Skill.Level - 1);
        public const double 攻击力 = 0.04;
        public const double 冷却缩减 = 0.03;
        public const double 加速系数 = 0.03;
        public const double 行动速度 = 30;
        public const double 攻击力提升 = 0.4;
        private double 实际攻击力提升 = 0;
        private double 实际行动速度提升 = 0;
        private double 实际加速系数提升 = 0;
        private double 实际冷却缩减提升 = 0;

        public 全军出击特效(Skill skill) : base(skill)
        {
            Description = GeneralDescription;
        }

        public override void OnTimeElapsed(Character character, double elapsed)
        {
            刷新技能效果(character);
        }

        public override void OnEffectLost(Character character)
        {
            Skill.IsInEffect = false;
            刷新技能效果(character);
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            Skill.IsInEffect = true;
            RemainDuration = Duration;
            if (!caster.Effects.Contains(this))
            {
                caster.Effects.Add(this);
                OnEffectGained(caster);
            }
            if (caster.Effects.FirstOrDefault(e => e is 雇佣兵团特效 && e.Skill.Character == Skill.Character) is 雇佣兵团特效 e)
            {
                e.Skill.CurrentCD = 0;
                e.Skill.Enable = true;
                int count = e.雇佣兵团.Count;
                if (count < 雇佣兵团特效.最大数量)
                {
                    do
                    {
                        count = e.新增雇佣兵(caster);
                    }
                    while (count < 雇佣兵团特效.最大数量);
                }
                foreach (雇佣兵 gyb in e.雇佣兵团)
                {
                    gyb.Recovery();
                    gyb.ExATKPercentage += 0.05;
                }
            }
            刷新技能效果(caster);
        }

        public void 刷新技能效果(Character character)
        {
            if (实际攻击力提升 != 0)
            {
                character.ExATKPercentage -= 实际攻击力提升;
                实际攻击力提升 = 0;
            }
            if (实际行动速度提升 != 0)
            {
                character.ExSPD -= 实际行动速度提升;
                实际行动速度提升 = 0;
            }
            if (实际加速系数提升 != 0)
            {
                character.ExAccelerationCoefficient -= 实际加速系数提升;
                实际加速系数提升 = 0;
            }
            if (实际冷却缩减提升 != 0)
            {
                character.ExCDR -= 实际冷却缩减提升;
                实际冷却缩减提升 = 0;
            }
            if (!Skill.IsInEffect)
            {
                return;
            }
            if (character.Effects.FirstOrDefault(e => e is 雇佣兵团特效 && e.Skill.Character == Skill.Character) is 雇佣兵团特效 e)
            {
                int count = e.雇佣兵团.Count;
                实际攻击力提升 = 攻击力 * count;
                实际行动速度提升 = 行动速度 * count;
                实际加速系数提升 = 加速系数 * count;
                实际冷却缩减提升 = 冷却缩减 * count;
                character.ExATKPercentage += 实际攻击力提升;
                character.ExSPD += 实际行动速度提升;
                character.ExAccelerationCoefficient += 实际加速系数提升;
                character.ExCDR += 实际冷却缩减提升;
                Description = $"{GeneralDescription}（当前雇佣兵数量：{count}，攻击力提升：{实际攻击力提升 * 100:0.##}% [ {character.BaseATK * 实际攻击力提升:0.##} ] 点，行动速度提升：{实际行动速度提升:0.##} 点，加速系数提升：{实际加速系数提升 * 100:0.##}%，冷却缩减提升：{实际冷却缩减提升 * 100:0.##}%）";
            }
        }
    }
}
