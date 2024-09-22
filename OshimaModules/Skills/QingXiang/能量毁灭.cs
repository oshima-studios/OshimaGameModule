using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 能量毁灭 : Skill
    {
        public override long Id => (long)SuperSkillID.能量毁灭;
        public override string Name => "能量毁灭";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override double EPCost => 100;
        public override double CD => 55 - 3 * (Level - 1);
        public override double HardnessTime { get; set; } = 25;

        public 能量毁灭(Character? character = null) : base(SkillType.SuperSkill, character)
        {
            Effects.Add(new 能量毁灭特效(this));
        }
    }

    public class 能量毁灭特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"对所有角色造成 " +
            $"{能量系数 * 100:0.##}% 其现有能量值 + {智力系数 * 100:0.##}% 智力 [ {智力伤害} ] 的魔法伤害。";
        public override bool TargetSelf => false;
        public override double TargetRange => 999;

        private double 智力系数 => Calculation.Round4Digits(0.55 * Level);
        private double 智力伤害 => Calculation.Round2Digits(智力系数 * Skill.Character?.INT ?? 0);
        private double 能量系数 => Calculation.Round4Digits(0.75 * Level);

        public override void OnSkillCasted(Character caster, List<Character> enemys, List<Character> teammates, Dictionary<string, object> others)
        {
            foreach (Character c in enemys)
            {
                WriteLine($"[ {caster} ] 正在毁灭 [ {c} ] 的能量！！");
                double ep = c.EP;
                DamageToEnemy(caster, c, true, MagicType, Calculation.Round2Digits(ep * 能量系数 + 智力伤害));
            }
        }
    }
}
