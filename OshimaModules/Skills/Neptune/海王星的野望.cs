using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.PassiveEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 海王星的野望 : Skill
    {
        public override long Id => (long)SuperSkillID.海王星的野望;
        public override string Name => "海王星的野望";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";
        public override double EPCost => 100;
        public override double CD => 75;
        public override double HardnessTime { get; set; } = 10;
        public override bool CanSelectSelf => false;
        public override bool CanSelectEnemy => true;
        public override bool CanSelectTeammate => false;
        public override int CanSelectTargetCount => 3;
        public override bool IsNonDirectional => true;
        public override int CanSelectTargetRange => 3;

        public 海王星的野望(Character? character = null) : base(SkillType.SuperSkill, character)
        {
            Effects.Add(new 海王星的野望特效(this));
        }
    }

    public class 海王星的野望特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"标记{Skill.TargetDescription()}，持续 {持续时间:0.##} {GameplayEquilibriumConstant.InGameTime}。立即对标记目标造成 {直接伤害:0.##} 点{CharacterSet.GetDamageTypeName(DamageType.Magical, MagicType)}。" +
            $"在持续时间内{爆炸伤害描述}在此期间，{Skill.SkillOwner()}的力量提升 60% [ {力量提升:0.##} ]，并且 [ {nameof(深海之戟)} ] 改变为相同机制，可以叠加触发。无视免疫。";
        public override bool Durative => true;
        public override double Duration => 持续时间;
        public override DispelledType DispelledType => DispelledType.CannotBeDispelled;
        public override MagicType MagicType => Skill.Character?.MagicType ?? MagicType.None;
        public override ImmuneType IgnoreImmune => ImmuneType.All;

        public string 爆炸伤害描述 => $"对受到标记的目标造成伤害时将产生爆炸，爆炸将产生 {分裂伤害系数 * 100:0.##}% 分裂伤害。分裂伤害为全图索敌，会优先分裂至三个在持续时间内对{Skill.SkillOwner()}造成伤害最多的敌人，若没有符合条件的敌人或敌人数量不足，则将分裂至被标记的敌人，或至多三个随机的敌人。";
        public double 直接伤害 => 180 + 75 * (Skill.Level - 1);
        public double 持续时间 => 25 + 2 * (Skill.Level - 1);
        public double 分裂伤害系数 => 0.25 + 0.02 * (Skill.Level - 1);
        public double 力量提升 => 0.6 * (Skill.Character?.BaseSTR ?? 0);
        public Dictionary<Character, double> 敌人伤害统计 { get; set; } = [];

        private double 实际力量提升 = 0;

        public override void OnEffectGained(Character character)
        {
            实际力量提升 = 力量提升;
            character.ExSTR += 实际力量提升;
            if (character.Effects.Where(e => e is 深海之戟特效).FirstOrDefault() is 深海之戟特效 e)
            {
                e.野望 = this;
            }
        }

        public override void OnEffectLost(Character character)
        {
            character.ExSTR -= 实际力量提升;
            if (character.Effects.Where(e => e is 深海之戟特效).FirstOrDefault() is 深海之戟特效 e)
            {
                e.野望 = null;
            }
        }

        public override void AfterDamageCalculation(Character character, Character enemy, double damage, double actualDamage, bool isNormalAttack, DamageType damageType, MagicType magicType, DamageResult damageResult)
        {
            if (enemy == Skill.Character)
            {
                if (!敌人伤害统计.TryAdd(character, actualDamage))
                {
                    敌人伤害统计[character] += actualDamage;
                }
            }

            if (character == Skill.Character && (damageResult == DamageResult.Normal || damageResult == DamageResult.Critical) && enemy.Effects.FirstOrDefault(e => e is 海王星的野望标记 && e.Source == Skill.Character) is 海王星的野望标记 e)
            {
                分裂伤害(character, enemy, actualDamage, damageType, magicType);
            }
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            实际力量提升 = 0;
            RemainDuration = Duration;
            if (!caster.Effects.Contains(this))
            {
                caster.Effects.Add(this);
                OnEffectGained(caster);
            }
            foreach (Character target in targets)
            {
                DamageToEnemy(caster, target, DamageType.Magical, MagicType, 直接伤害);
            }
            // 造成伤害之后再一起上标记，否则会立即触发标记特效
            foreach (Character target in targets)
            {
                Effect e = new 海王星的野望标记(Skill, caster)
                {
                    Durative = true,
                    Duration = 持续时间,
                    RemainDuration = Duration
                };
                target.Effects.Add(e);
                e.OnEffectGained(target);
                AddEffectTypeToCharacter(target, [e.EffectType]);
            }
            RecordCharacterApplyEffects(caster, EffectType.DamageBoost);
        }

        public void 分裂伤害(Character character, Character enemy, double damage, DamageType damageType, MagicType magicType)
        {
            List<Character> targets = [];
            targets.AddRange(敌人伤害统计.Where(w => w.Key != character && w.Key != enemy && w.Key.HP > 0).OrderByDescending(o => o.Value).Select(s => s.Key).Take(3));
            if (targets.Count < 3)
            {
                int count = 3 - targets.Count;
                // 获取所有敌人
                List<Character> allEnemys = [];
                if (GamingQueue != null)
                {
                    allEnemys = [.. GamingQueue.GetEnemies(character).Where(c => c != character && c != enemy && !targets.Contains(c) && c.HP > 0)];
                    targets.AddRange(allEnemys.Where(c => c.Effects.Any(e => e is 海王星的野望标记 && e.Source == character)).Take(count));
                    if (targets.Count < 3)
                    {
                        count = 3 - targets.Count;
                        targets.AddRange(allEnemys.OrderBy(o => Random.Shared.Next()).Take(count));
                    }
                }
            }
            damage *= 分裂伤害系数;
            foreach (Character target in targets)
            {
                DamageToEnemy(character, target, damageType, magicType, damage, new(character)
                {
                    CalculateCritical = false,
                    CalculateReduction = true,
                    TriggerEffects = false,
                    IgnoreImmune = true
                });
            }
        }
    }
}
