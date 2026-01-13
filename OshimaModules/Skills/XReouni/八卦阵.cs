using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 八卦阵 : Skill
    {
        public override long Id => (long)PassiveID.八卦阵;
        public override string Name => "八卦阵";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";

        public 八卦阵(Character? character = null) : base(SkillType.Passive, character)
        {
            Effects.Add(new 八卦阵特效(this));
        }

        public override IEnumerable<Effect> AddPassiveEffectToCharacter()
        {
            return Effects;
        }
    }

    public class 八卦阵特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"每次造成伤害或受到伤害时，进行投掷检定，结果为偶数时，造成的伤害提升 {伤害提升 * 100:0.##}%，受到伤害减少 {伤害减少 * 100:0.##}%；结果为奇数时，该回合提升 {奇数平衡性提升 * 100:0.##}% 暴击率和闪避率、{奇数平衡性提升 * 100:0.##}% 魔法抗性，并将下一次伤害提升 {奇数伤害提升 * 100:0.##}% 或减免 {奇数伤害减少 * 100:0.##}% 下一次受到伤害，若下一次投掷结果为偶数，将叠加该效果。";

        public bool 归元 { get; set; } = false;
        public bool 回合投掷出奇数 { get; set; } = false;
        public bool 奇数效果 { get; set; } = false;
        public double 伤害提升 { get; set; } = 0.8;
        public double 伤害减少 { get; set; } = 0.4;
        public double 奇数平衡性提升 { get; set; } = 0.1;
        public double 奇数伤害提升 { get; set; } = 0.3;
        public double 奇数伤害减少 { get; set; } = 0.15;

        public override double AlterActualDamageAfterCalculation(Character character, Character enemy, double damage, bool isNormalAttack, DamageType damageType, MagicType magicType, DamageResult damageResult, ref bool isEvaded, Dictionary<Effect, double> totalDamageBonus)
        {
            double bonus = 0;
            if (character != Skill.Character && enemy != Skill.Character)
            {
                return bonus;
            }
            if (奇数效果)
            {
                奇数效果 = false;
                if (character == Skill.Character)
                {
                    bonus = damage * 奇数伤害提升;
                    WriteLine($"[ {Skill.Character} ] 发动了八卦阵！伤害提升了 {Math.Abs(bonus):0.##} 点！");
                }
                else if (enemy == Skill.Character)
                {
                    bonus = -(damage * 奇数伤害减少);
                    WriteLine($"[ {Skill.Character} ] 发动了八卦阵！伤害减少了 {Math.Abs(bonus):0.##} 点！");
                }
            }
            bool result = 归元 || (!归元 && Random.Shared.Next(10) % 2 == 0);
            WriteLine($"[ {Skill.Character} ] 的八卦阵投掷结果为：{(result ? "偶数" : "奇数")}。");
            if (damage > 0 && result)
            {
                if (character == Skill.Character)
                {
                    bonus += damage * 伤害提升;
                    WriteLine($"[ {Skill.Character} ] 发动了八卦阵！伤害提升了 {Math.Abs(bonus):0.##} 点！");
                }
                else if (enemy == Skill.Character)
                {
                    bonus += -(damage * 伤害减少);
                    WriteLine($"[ {Skill.Character} ] 发动了八卦阵！伤害减少了 {Math.Abs(bonus):0.##} 点！");
                }
                if (归元)
                {
                    WriteLine($"[ {Skill.Character} ] 发动了归元环！冷却时间减少了 {归元环特效.冷却时间减少 * 100:0.##}%！");
                    foreach (Skill s in Skill.Character.Skills)
                    {
                        s.CurrentCD -= s.CD * 归元环特效.冷却时间减少;
                        if (s.CurrentCD < 0)
                        {
                            s.CurrentCD = 0;
                            s.Enable = true;
                        }
                    }
                }
            }
            else
            {
                if (!回合投掷出奇数)
                {
                    回合投掷出奇数 = true;
                    Effect e = new DynamicsEffect(Skill, new()
                    {
                        { "excr", 奇数平衡性提升 },
                        { "exer", 奇数平衡性提升 },
                        { "mdftype", (int)MagicType.None },
                        { "mdfvalue", 奇数平衡性提升 }
                    }, Skill.Character)
                    {
                        Name = "八卦阵·奇数效果",
                        Durative = false,
                        DurationTurn = 1,
                        RemainDurationTurn = 1
                    };
                    Skill.Character.Effects.Add(e);
                    e.OnEffectGained(Skill.Character);
                    WriteLine($"[ {Skill.Character} ] 发动了八卦阵！该回合提升 {奇数平衡性提升 * 100:0.##}% 暴击率和闪避率、{奇数平衡性提升 * 100:0.##}% 魔法抗性！");
                }
                if (!奇数效果)
                {
                    奇数效果 = true;
                }
            }
            return bonus;
        }

        public override void OnTurnEnd(Character character)
        {
            回合投掷出奇数 = false;
        }
    }
}
