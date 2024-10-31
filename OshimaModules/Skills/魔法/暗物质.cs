using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 暗物质 : Skill
    {
        public override long Id => (long)MagicID.暗物质;
        public override string Name => "暗物质";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override double MPCost => 30 + (50 * (Level - 1));
        public override double CD => 25;
        public override double CastTime => 6;
        public override double HardnessTime { get; set; } = 3;

        public 暗物质(Character? character = null) : base(SkillType.Magic, character)
        {
            Effects.Add(new 暗物质特效(this));
        }
    }

    public class 暗物质特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"对目标敌人造成 {基础伤害:0.##} + {智力系数 * 100:0.##}% 智力 [ {Damage:0.##} ] 点{CharacterSet.GetMagicDamageName(MagicType)}。";
        public override bool TargetSelf => false;
        public override int TargetCount => 1;

        private double 基础伤害 => 90 + 60 * (Skill.Level - 1);
        private double 智力系数 => (0.5 + 0.6 * (Skill.Level - 1));
        private double Damage => 基础伤害 + 智力系数 * Skill.Character?.INT ?? 0;

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
