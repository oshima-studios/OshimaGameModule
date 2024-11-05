using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 火之矢 : Skill
    {
        public override long Id => (long)MagicID.火之矢;
        public override string Name => "火之矢";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override double MPCost => 30 + (40 * (Level - 1));
        public override double CD => 20;
        public override double CastTime => 6;
        public override double HardnessTime { get; set; } = 3;

        public 火之矢(Character? character = null) : base(SkillType.Magic, character)
        {
            Effects.Add(new 火之矢特效(this));
        }
    }

    public class 火之矢特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"对目标敌人造成 {基础伤害:0.##} + {系数 * 100:0.##}% {CharacterSet.GetPrimaryAttributeName(Skill.Character?.PrimaryAttribute ?? PrimaryAttribute.INT)} {(Skill.Level == 0 ? $"(+{等级系数 * 100:0.##}%/Lv)" : "")} [ {Damage:0.##} ] 点{CharacterSet.GetMagicDamageName(MagicType)}。";
        public override bool TargetSelf => false;
        public override int TargetCount => 1;

        private static double 等级系数 => 0.35;
        private double 基础伤害 => Skill.Level != 0 ? 80 + 65 * (Skill.Level - 1) : 80;
        private double 系数 => Skill.Level != 0 ? 0.45 + 等级系数 * (Skill.Level - 1) : 0.45;
        private double Damage => 基础伤害 + 系数 * Skill.Character?.PrimaryAttributeValue ?? 0;

        public override void OnSkillCasted(Character caster, List<Character> targets, Dictionary<string, object> others)
        {
            if (targets.Count > 0)
            {
                Character enemy = targets[0];
                DamageToEnemy(caster, enemy, true, MagicType, Damage);
            }
        }
    }
}
