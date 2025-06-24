using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 变幻之心 : Skill
    {
        public override long Id => (long)SuperSkillID.变幻之心;
        public override string Name => "变幻之心";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";
        public override double EPCost => 100;
        public override double CD => 20;
        public override double HardnessTime { get; set; } = 3;
        public override bool CanSelectSelf => true;
        public override bool CanSelectEnemy => false;

        public 变幻之心(Character? character = null) : base(SkillType.SuperSkill, character)
        {
            Effects.Add(new 变幻之心特效(this));
        }
    }

    public class 变幻之心特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => "变幻之心";
        public override string Description => $"检查 [ 智慧与力量 ] 的模式。在力量模式下，立即回复 {生命值回复 * 100:0.##}% 生命值，同时下 {吸血次数} 次对敌人造成伤害时获得 30% 生命偷取；" +
            $"智力模式下，下 {魔法加成次数} 次魔法伤害提升 {伤害提升 * 100:0.##}%。此技能效果不叠加，重复释放时将重置次数；若是模式切换后重复释放，将回收先前的效果。{当前次数描述}";
        public override DispelledType DispelledType => DispelledType.CannotBeDispelled;

        private string 当前次数描述
        {
            get
            {
                string str = "";
                if (当前吸血次数 > 0 || 当前魔法加成次数 > 0)
                {
                    str = 当前吸血次数 > 0 ? "吸血次数" : (当前魔法加成次数 > 0 ? "魔法加成次数" : "");
                    if (str != "")
                    {
                        str = $"剩余{str}：";
                    }
                    if (当前吸血次数 > 0)
                    {
                        str += $"{当前吸血次数} 次。";
                    }
                    else if (当前魔法加成次数 > 0)
                    {
                        str += $"{当前魔法加成次数} 次。";
                    }
                }
                return str;
            }
        }
        private int 吸血次数
        {
            get
            {
                return Level switch
                {
                    1 => 3,
                    2 => 3,
                    3 => 4,
                    4 => 4,
                    5 => 5,
                    6 => 5,
                    _ => 3
                };
            }
        }
        private int 魔法加成次数
        {
            get
            {
                return Level switch
                {
                    1 => 1,
                    2 => 2,
                    3 => 2,
                    4 => 3,
                    5 => 3,
                    6 => 4,
                    _ => 1
                };
            }
        }
        private double 生命值回复 => 0.25 + 0.03 * (Level - 1);
        private double 伤害提升 => 0.6 + 0.4 * (Level - 1);
        private int 当前吸血次数 = 0;
        private int 当前魔法加成次数 = 0;

        public override void OnEffectGained(Character character)
        {
            if (character.PrimaryAttribute == PrimaryAttribute.STR)
            {
                当前吸血次数 = 吸血次数;
            }
            else if (character.PrimaryAttribute == PrimaryAttribute.INT)
            {
                当前魔法加成次数 = 魔法加成次数;
            }
        }

        public override void OnEffectLost(Character character)
        {
            当前吸血次数 = 0;
            当前魔法加成次数 = 0;
        }

        public override double AlterExpectedDamageBeforeCalculation(Character character, Character enemy, double damage, bool isNormalAttack, DamageType damageType, MagicType magicType, Dictionary<Effect, double> totalDamageBonus)
        {
            if (character == Skill.Character && damageType == DamageType.Magical && 当前魔法加成次数 > 0)
            {
                当前魔法加成次数--;
                double 实际伤害提升百分比 = 伤害提升;
                double 实际伤害提升 = damage * 实际伤害提升百分比;
                WriteLine($"[ {character} ] 发动了变幻之心！伤害提升了 {实际伤害提升:0.##} 点！变幻之心加成剩余：{当前魔法加成次数} 次。");
                if (当前魔法加成次数 == 0)
                {
                    character.Effects.Remove(this);
                    OnEffectLost(character);
                }
                return 实际伤害提升;
            }
            return 0;
        }

        public override void AfterDamageCalculation(Character character, Character enemy, double damage, double actualDamage, bool isNormalAttack, DamageType damageType, MagicType magicType, DamageResult damageResult)
        {
            if (character == Skill.Character && 当前吸血次数 > 0 && (damageResult == DamageResult.Normal || damageResult == DamageResult.Critical) && damage > 0)
            {
                当前吸血次数--;
                double 实际吸血 = damage * 0.3;
                character.HP += 实际吸血;
                WriteLine($"[ {character} ] 发动了变幻之心！回复了 {实际吸血:0.##} 点生命值！变幻之心加成剩余：{当前吸血次数} 次。");
                if (当前吸血次数 == 0)
                {
                    character.Effects.Remove(this);
                    OnEffectLost(character);
                }
            }
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, Dictionary<string, object> others)
        {
            IEnumerable<Effect> effects = caster.Effects.Where(e => e is 智慧与力量特效);
            if (effects.Any())
            {
                if (caster.Effects.Contains(this))
                {
                    caster.Effects.Remove(this);
                    OnEffectLost(caster);
                }
                caster.Effects.Add(this);
                OnEffectGained(caster);
                if (caster.PrimaryAttribute == PrimaryAttribute.STR)
                {
                    double 回复的生命 = 生命值回复 * caster.MaxHP;
                    HealToTarget(caster, caster, 回复的生命);
                    GamingQueue?.LastRound.ApplyEffects.TryAdd(caster, [EffectType.Lifesteal]);
                }
                else if (caster.PrimaryAttribute == PrimaryAttribute.INT)
                {
                    GamingQueue?.LastRound.ApplyEffects.TryAdd(caster, [EffectType.DamageBoost]);
                }
            }
        }
    }
}
