using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

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
        public override string Description => $"每次造成伤害或受到伤害时，进行投掷检定，结果为偶数时，造成的伤害提升 {伤害提升 * 100:0.##}%，受到伤害减少 {伤害减少 * 100:0.##}%；反之不产生任何效果。";

        public bool 归元 { get; set; } = false;
        public double 伤害提升 { get; set; } = 1;
        public double 伤害减少 { get; set; } = 0.5;

        public override double AlterActualDamageAfterCalculation(Character character, Character enemy, double damage, bool isNormalAttack, DamageType damageType, MagicType magicType, DamageResult damageResult, ref bool isEvaded, Dictionary<Effect, double> totalDamageBonus)
        {
            double bouns = 0;
            if (character == Skill.Character)
            {
                bool result = 归元 || (!归元 && Random.Shared.Next(10) % 2 == 0);
                WriteLine($"[ {character} ] 的八卦阵投掷结果为：{(result ? "偶数" : "奇数")}。");
                if (damage > 0 && result)
                {
                    Character c = character;
                    if (character == Skill.Character)
                    {
                        bouns = damage * 伤害提升;
                        WriteLine($"[ {character} ] 发动了八卦阵！伤害提升了 {Math.Abs(bouns):0.##} 点！");
                    }
                    else if (enemy == Skill.Character)
                    {
                        c = enemy;
                        bouns = -(damage * 伤害减少);
                        WriteLine($"[ {character} ] 发动了八卦阵！伤害减少了 {Math.Abs(bouns):0.##} 点！");
                    }
                    if (归元)
                    {
                        WriteLine($"[ {character} ] 发动了归元环！冷却时间减少了 {归元环特效.冷却时间减少:0.##} {GameplayEquilibriumConstant.InGameTime}！");
                        foreach (Skill s in c.Skills)
                        {
                            if (s.CurrentCD >= 归元环特效.冷却时间阈值)
                            {
                                s.CurrentCD -= 归元环特效.冷却时间减少;
                                if (s.CurrentCD < 0)
                                {
                                    s.CurrentCD = 0;
                                    s.Enable = true;
                                }
                            }
                        }
                    }
                }
            }
            return bouns;
        }
    }
}
