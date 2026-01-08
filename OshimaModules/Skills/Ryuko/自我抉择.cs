using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.PassiveEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 自我抉择 : Skill
    {
        public override long Id => (long)SuperSkillID.自我抉择;
        public override string Name => "自我抉择";
        public override string Description => Effects.Count > 0 ? ((自我抉择特效)Effects.First()).通用描述 : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";
        public override string ExemptionDescription => Effects.Count > 0 ? Effects.First().ExemptionDescription : "";
        public override double EPCost => 100;
        public override double CD => 60;
        public override double HardnessTime { get; set; } = 2;
        public override bool CanSelectSelf => true;
        public override bool CanSelectEnemy => false;

        public 自我抉择(Character? character = null) : base(SkillType.SuperSkill, character)
        {
            Effects.Add(new 自我抉择特效(this));
        }
    }

    public class 自我抉择特效 : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description { get; set; } = "";
        public override bool Durative => true;
        public override DispelledType DispelledType => DispelledType.CannotBeDispelled;
        public override string DispelDescription => $"被驱散性：所有回复/禁止回复不可驱散，攻击力加成不可驱散，嘲讽需强驱散";
        public override PrimaryAttribute ExemptionType => PrimaryAttribute.AGI;
        public override bool ExemptDuration => false;

        public string 通用描述 => $"你可以选择获得哪一种力量：\r\n{熵核描述}\r\n{守护描述}";
        private string 熵核描述 => $"【熵核】加速生命回复，每{GameplayEquilibriumConstant.InGameTime}额外回复 {熵核额外回复 * 100:0.##}% 当前生命值 [ {Skill.Character?.HP * 熵核额外回复:0.##} ]，攻击力提升 {熵核攻击力提升 * 100:0.##}% [ {Skill.Character?.BaseATK * 熵核攻击力提升:0.##} ]，但是受到的伤害提升 {熵核受到伤害提升 * 100:0.##}%。" +
            $"对敌人造成伤害会使其在 {熵核影响敌人时间:0.##} {GameplayEquilibriumConstant.InGameTime}内无法获得自然的生命和魔法回复，施加此状态时，只有目标的敏捷高于你的角色才能进行豁免检定。持续 {熵核持续时间:0.##} {GameplayEquilibriumConstant.InGameTime}。";
        private static double 熵核额外回复 => 0.04;
        private double 熵核攻击力提升 => 0.2 + 0.1 * (Skill.Level - 1);
        private double 熵核受到伤害提升 => 0.15 + 0.05 * (Skill.Level - 1);
        private static double 熵核影响敌人时间 => 10;
        private double 熵核持续时间 => 15 + 2 * (Skill.Level - 1);
        private string 守护描述 => $"【守护】极致地加速生命回复，每{GameplayEquilibriumConstant.InGameTime}额外回复 {守护额外回复 * 100:0.##}% 当前生命值 [ {Skill.Character?.HP * 守护额外回复:0.##} ]，为全体友方角色提供每{GameplayEquilibriumConstant.InGameTime}额外 {守护友方回复 * 100:0.##}% 当前生命值的生命回复，并嘲讽全体敌方角色，被嘲讽的角色仅能将你作为攻击目标。" +
            $"施加嘲讽状态时，只有目标的敏捷高于你的角色才能进行豁免检定。持续 {守护持续时间:0.##} {GameplayEquilibriumConstant.InGameTime}。";
        private double 守护额外回复 => 0.08 + 0.006 * (Skill.Level - 1);
        private double 守护友方回复 => 0.01 + 0.01 * (Skill.Level - 1);
        private double 守护持续时间 => 15 + 1 * (Skill.Level - 1);
        private bool 选择熵核 { get; set; } = false;

        private double 实际攻击力提升 = 0;
        private bool 实际选择熵核 = false;

        public 自我抉择特效(Skill skill) : base(skill)
        {
            Description = 通用描述;
        }

        public override void OnEffectGained(Character character)
        {
            if (实际选择熵核)
            {
                实际攻击力提升 = 熵核攻击力提升;
                character.ExATKPercentage += 实际攻击力提升;
                WriteLine($"[ {character} ] 的攻击力提升了 {实际攻击力提升 * 100:0.##}% [ {character.BaseATK * 实际攻击力提升:0.##} ] ！");
            }
        }

        public override void OnEffectLost(Character character)
        {
            if (实际选择熵核)
            {
                character.ExATKPercentage -= 实际攻击力提升;
            }
        }

        public override double AlterActualDamageAfterCalculation(Character character, Character enemy, double damage, bool isNormalAttack, DamageType damageType, MagicType magicType, DamageResult damageResult, ref bool isEvaded, Dictionary<Effect, double> totalDamageBonus)
        {
            if (实际选择熵核 && enemy == Skill.Character && (damageResult == DamageResult.Normal || damageResult == DamageResult.Critical))
            {
                double bouns = -(damage * 熵核受到伤害提升);
                WriteLine($"[ {enemy} ] 触发了自我抉择，额外受到 {Math.Abs(bouns):0.##} 伤害！");
                return bouns;
            }
            return 0;
        }

        public override void AfterDamageCalculation(Character character, Character enemy, double damage, double actualDamage, bool isNormalAttack, DamageType damageType, MagicType magicType, DamageResult damageResult)
        {
            if (实际选择熵核 && character == Skill.Character && (damageResult == DamageResult.Normal || damageResult == DamageResult.Critical) && !CheckSkilledImmune(character, enemy, Skill))
            {
                Effect e = new 禁止治疗(Skill, character, true, 熵核影响敌人时间, 0)
                {
                    ExemptionType = PrimaryAttribute.AGI,
                    ExemptDuration = false,
                    DispelledType = DispelledType
                };
                if (enemy.AGI <= character.AGI || (enemy.AGI > character.AGI && !CheckExemption(character, enemy, e)))
                {
                    WriteLine($"[ {character} ] 对 [ {enemy} ] 施加了禁止治疗！！持续时间：{熵核影响敌人时间:0.##} {GameplayEquilibriumConstant.InGameTime}！");
                    enemy.Effects.Add(e);
                    e.OnEffectGained(enemy);
                    GamingQueue?.LastRound.AddApplyEffects(enemy, e.EffectType);
                }
            }
        }

        public override void OnCharacterInquiry(Character character, string topic, Dictionary<string, object> args, Dictionary<string, object> response)
        {
            if (topic == nameof(自我抉择))
            {
                if (response.TryGetValue("result", out object? value) && value is bool choose)
                {
                    选择熵核 = choose;
                }
                else
                {
                    选择熵核 = Random.Shared.Next() % 2 == 0;
                }
            }
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            if (caster.Effects.Contains(this))
            {
                OnEffectLost(caster);
                caster.Effects.Remove(this);
            }
            caster.Effects.Add(this);
            实际攻击力提升 = 0;
            Dictionary<string, object> response = Inquiry(caster, nameof(自我抉择), new()
            {
                { "熵核", 熵核描述 },
                { "守护", 守护描述 },
            });
            实际选择熵核 = 选择熵核;
            Duration = 实际选择熵核 ? 熵核持续时间 : 守护持续时间;
            RemainDuration = Duration;
            OnEffectGained(caster);
            if (实际选择熵核)
            {
                Description = $"作出抉择：{熵核描述}";
                Effect effect = new 持续回复(Skill, caster, caster, true, Duration, 0, true, 0, 熵核额外回复)
                {
                    DispelledType = DispelledType
                };
                WriteLine($"[ {caster} ] 获得了持续生命回复！持续时间：{Duration:0.##} {GameplayEquilibriumConstant.InGameTime}！");
                caster.Effects.Add(effect);
                effect.OnEffectGained(caster);
                GamingQueue?.LastRound.AddApplyEffects(caster, effect.EffectType);
            }
            else
            {
                Description = $"作出抉择：{守护描述}";
                Effect effect = new 持续回复(Skill, caster, caster, true, Duration, 0, true, 0, 守护额外回复)
                {
                    DispelledType = DispelledType
                };
                WriteLine($"[ {caster} ] 获得了持续生命回复！持续时间：{Duration:0.##} {GameplayEquilibriumConstant.InGameTime}！");
                caster.Effects.Add(effect);
                effect.OnEffectGained(caster);
                GamingQueue?.LastRound.AddApplyEffects(caster, effect.EffectType);
                List<Character> allEnemys = [];
                List<Character> allTeammates = [];
                if (GamingQueue != null)
                {
                    allEnemys = [.. GamingQueue.GetEnemies(caster).Where(c => c != caster && c.HP > 0)];
                    allTeammates = [.. GamingQueue.GetTeammates(caster).Where(c => c != caster && c.HP > 0)];
                }
                foreach (Character enemy in allEnemys)
                {
                    Effect e = new 愤怒(Skill, caster, caster, true, Duration, 0)
                    {
                        DispelledType = DispelledType.CannotBeDispelled
                    };
                    if (enemy.AGI <= caster.AGI || (enemy.AGI > caster.AGI && !CheckExemption(caster, enemy, e)))
                    {
                        WriteLine($"[ {caster} ] 嘲讽了 [ {enemy} ]，[ {enemy} ] 愤怒了！！持续时间：{Duration:0.##} {GameplayEquilibriumConstant.InGameTime}！");
                        enemy.Effects.Add(e);
                        e.OnEffectGained(enemy);
                        GamingQueue?.LastRound.AddApplyEffects(enemy, e.EffectType);
                    }
                }
                foreach (Character teammate in allTeammates)
                {
                    Effect e = new 持续回复(Skill, caster, caster, true, Duration, 0, true, 0, 守护友方回复)
                    {
                        DispelledType = DispelledType
                    };
                    WriteLine($"[ {caster} ] 对 [ {teammate} ] 施加了持续生命回复！持续时间：{Duration:0.##} {GameplayEquilibriumConstant.InGameTime}！");
                    teammate.Effects.Add(e);
                    e.OnEffectGained(teammate);
                    GamingQueue?.LastRound.AddApplyEffects(teammate, e.EffectType);
                }
            }
            GamingQueue?.LastRound.AddApplyEffects(caster, EffectType.Focusing, EffectType.HealOverTime);
        }
    }
}
