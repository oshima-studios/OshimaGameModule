using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 熵灭极诣 : Skill
    {
        public override long Id => (long)SuperSkillID.熵灭极诣;
        public override string Name => "熵灭·极诣";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";
        public override double EPCost => 100;
        public override double CD => 55;
        public override double HardnessTime { get; set; } = 0;
        public override bool CanSelectSelf => true;
        public override bool CanSelectEnemy => false;

        public 熵灭极诣(Character? character = null) : base(SkillType.SuperSkill, character)
        {
            Effects.Add(new 熵灭极诣特效(this));
        }
    }

    public class 熵灭极诣特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"{Skill.SkillOwner()}释放「熵灭」的力量，获得 135% 力量 [ {攻击力加成:0.##} ] 的攻击力加成，但每次普通攻击命中时都会损失自身 6% 当前生命值 [ {当前生命值:0.##} ]，持续 {Duration:0.##} {GameplayEquilibriumConstant.InGameTime}。";
        public override bool Durative => true;
        public override double Duration => 10 + 2 * (Level - 1);
        public override DispelledType DispelledType => DispelledType.CannotBeDispelled;

        private double 攻击力加成 => Skill.Character?.STR * 1.35 ?? 0;
        private double 当前生命值 => Skill.Character?.HP * 0.06 ?? 0;
        private double 实际攻击力加成 = 0;

        public override void OnEffectGained(Character character)
        {
            实际攻击力加成 = 攻击力加成;
            character.ExATK2 += 实际攻击力加成;
            WriteLine($"[ {character} ] 的攻击力增加了 [ {实际攻击力加成:0.##} ] ！");
        }

        public override void OnEffectLost(Character character)
        {
            // 恢复到原始攻击力
            character.ExATK2 -= 实际攻击力加成;
        }

        public override void AfterDamageCalculation(Character character, Character enemy, double damage, double actualDamage, bool isNormalAttack, DamageType damageType, MagicType magicType, DamageResult damageResult)
        {
            if (character == Skill.Character && isNormalAttack && (damageResult == DamageResult.Normal || damageResult == DamageResult.Critical))
            {
                double 生命值减少 = 当前生命值;
                character.HP -= 生命值减少;
                WriteLine($"[ {character} ] 由于自身力量过于强大而被反噬，损失了 [ {生命值减少:0.##} ] 点生命值！");
            }
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            RemainDuration = Duration;
            if (!caster.Effects.Contains(this))
            {
                实际攻击力加成 = 0;
                caster.Effects.Add(this);
                OnEffectGained(caster);
            }
            GamingQueue?.LastRound.AddApplyEffects(caster, EffectType.DamageBoost);
        }
    }
}
