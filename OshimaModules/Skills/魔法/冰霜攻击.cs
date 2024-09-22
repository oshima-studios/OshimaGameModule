using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 冰霜攻击 : Skill
    {
        public override long Id => (long)MagicID.冰霜攻击;
        public override string Name => "冰霜攻击";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override double MPCost => 30 + (50 * (Level - 1));
        public override double CD => 20;
        public override double CastTime => 6;
        public override double HardnessTime { get; set; } = 3;

        public 冰霜攻击(Character? character = null) : base(SkillType.Magic, character)
        {
            Effects.Add(new 冰霜攻击特效(this));
        }
    }

    public class 冰霜攻击特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"对目标敌人造成 {Calculation.Round2Digits(90 + 60 * (Skill.Level - 1))} + {Calculation.Round2Digits((1.2 + 1.8 * (Skill.Level - 1)) * 100)}% 智力 [ {Damage} ] 点{CharacterSet.GetMagicDamageName(MagicType)}。";
        public override bool TargetSelf => false;
        public override int TargetCount => 1;

        private double Damage
        {
            get
            {
                double d = 0;
                if (Skill.Character != null)
                {
                    d = Calculation.Round2Digits(90 + 60 * (Skill.Level - 1) + (1.2 + 1.8 * (Skill.Level - 1)) * Skill.Character.INT);
                }
                return d;
            }
        }

        public override void OnSkillCasted(Character caster, List<Character> enemys, List<Character> teammates, Dictionary<string, object> others)
        {
            if (enemys.Count > 0)
            {
                Character enemy = enemys[new Random().Next(enemys.Count)];
                DamageToEnemy(caster, enemy, true, MagicType, Damage);
            }
        }
    }
}
