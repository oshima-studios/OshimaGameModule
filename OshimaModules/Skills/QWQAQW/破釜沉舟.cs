using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 破釜沉舟 : Skill
    {
        public override long Id => (long)PassiveID.破釜沉舟;
        public override string Name => "破釜沉舟";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";

        public 破釜沉舟(Character? character = null) : base(SkillType.Passive, character)
        {
            Effects.Add(new 破釜沉舟特效(this));
            Effects.Add(new ExMaxHP2(this, new()
            {
                { "exhp", -0.2 }
            })
            {
                ParentEffect = Effects.First(),
                ForceHideInStatusBar = true
            });
        }

        public override IEnumerable<Effect> AddPassiveEffectToCharacter()
        {
            return Effects;
        }
    }

    public class 破釜沉舟特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"{Skill.SkillOwner()}已经看透了生命的真谛。{Skill.SkillOwner()}的最大生命值减少 20%，并获得破釜沉舟特效。破釜沉舟：生命值高于 40% 时，受到额外的 [ {高于40额外伤害下限}～{高于40额外伤害上限}% ] 伤害，获得 [ 累计所受伤害的 {高于40的加成下限}～{高于40的加成上限}%  ] 伤害加成；生命值低于等于 40% 时，不会受到额外的伤害，并且获得 [ 累计受到的伤害 {低于40的加成下限}～{低于40的加成上限}% ] 的伤害加成。" +
            $"在没有累计任何受到伤害的时候，将获得 {常规伤害加成 * 100:0.##}% 伤害加成和 {常规生命偷取 * 100:0.##}% 生命偷取。通过累计受到伤害发动破釜沉舟时，目标的闪避检定获得 40% 的减值，并立即清除累计受到的伤害。" + (累计受到的伤害 > 0 ? $"（当前累计受到伤害：{累计受到的伤害:0.##}）" : "");

        private bool 已通过累计受到伤害发动破釜沉舟 = false;
        private double 累计受到的伤害 = 0;
        private double 这次的伤害加成 = 0;
        private double 受到伤害之前的HP = 0;
        private double 这次受到的额外伤害 = 0;
        private readonly double 常规伤害加成 = 0.4;
        private readonly double 常规生命偷取 = 0.2;
        private readonly int 高于40额外伤害上限 = 40;
        private readonly int 高于40额外伤害下限 = 20;
        private readonly int 高于40的加成上限 = 90;
        private readonly int 高于40的加成下限 = 60;
        private readonly int 低于40的加成上限 = 160;
        private readonly int 低于40的加成下限 = 120;

        private double 伤害加成(double damage)
        {
            double 系数 = 常规伤害加成;
            Character? character = Skill.Character;
            if (character != null && 累计受到的伤害 != 0)
            {
                if (character.HP > character.MaxHP * 0.4)
                {
                    系数 = (Random.Shared.Next(高于40的加成下限, 高于40的加成上限) + 0.0) / 100;
                }
                else
                {
                    系数 = (Random.Shared.Next(低于40的加成下限, 低于40的加成上限) + 0.0) / 100;
                }
                return 系数 * 累计受到的伤害;
            }
            return 系数 * damage;
        }

        public override double AlterExpectedDamageBeforeCalculation(Character character, Character enemy, double damage, bool isNormalAttack, DamageType damageType, MagicType magicType, Dictionary<Effect, double> totalDamageBonus)
        {
            if (character == Skill.Character)
            {
                已通过累计受到伤害发动破釜沉舟 = 累计受到的伤害 != 0;
                这次的伤害加成 = 伤害加成(damage);
                WriteLine($"[ {character} ] 发动了破釜沉舟，获得了 {这次的伤害加成:0.##} 点伤害加成！");
                return 这次的伤害加成;
            }

            if (enemy == Skill.Character)
            {
                受到伤害之前的HP = enemy.HP;
                if (enemy.HP > enemy.MaxHP * 0.4)
                {
                    // 额外受到伤害
                    double 系数 = (Random.Shared.Next(高于40额外伤害下限, 高于40额外伤害上限) + 0.0) / 100;
                    这次受到的额外伤害 = damage * 系数;
                    WriteLine($"[ {enemy} ] 的破釜沉舟触发，将额外受到 {这次受到的额外伤害:0.##} 点伤害！");
                }
                else 这次受到的额外伤害 = 0;
                return 这次受到的额外伤害;
            }

            return 0;
        }

        public override void AfterDamageCalculation(Character character, Character enemy, double damage, double actualDamage, bool isNormalAttack, DamageType damageType, MagicType magicType, DamageResult damageResult)
        {
            if (enemy == Skill.Character && (damageResult == DamageResult.Normal || damageResult == DamageResult.Critical))
            {
                累计受到的伤害 += actualDamage;
                if (enemy.HP < 0 && 受到伤害之前的HP - actualDamage + 这次受到的额外伤害 > 0)
                {
                    enemy.HP = 10;
                    WriteLine($"[ {enemy} ] 的破釜沉舟触发，保护了自己不进入死亡！！");
                }
            }

            if (character == Skill.Character && 已通过累计受到伤害发动破釜沉舟)
            {
                累计受到的伤害 = 0;
                double ls = actualDamage * 常规生命偷取;
                HealToTarget(character, character, ls, triggerEffects: false);
                WriteLine($"[ {character} ] 发动了破釜沉舟，回复了 {ls:0.##} 点生命值！");
            }
        }

        public override bool BeforeEvadeCheck(Character actor, Character enemy, ref double throwingBonus)
        {
            if (已通过累计受到伤害发动破釜沉舟 && actor == Skill.Character)
            {
                已通过累计受到伤害发动破釜沉舟 = false;
                throwingBonus -= 0.4;
            }
            return true;
        }

        public override void OnTurnEnd(Character character)
        {
            已通过累计受到伤害发动破釜沉舟 = false;
        }
    }
}
