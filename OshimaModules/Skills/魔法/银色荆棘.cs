using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.PassiveEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 银色荆棘 : Skill
    {
        public override long Id => (long)MagicID.银色荆棘;
        public override string Name => "银色荆棘";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";
        public override double MPCost => Level > 0 ? 100 + (80 * (Level - 1)) : 100;
        public override double CD => Level > 0 ? 100 - (1.5 * (Level - 1)) : 100;
        public override double CastTime => 12;
        public override double HardnessTime { get; set; } = 5;
        public override int CanSelectTargetCount
        {
            get
            {
                return Level switch
                {
                    4 => 2,
                    5 => 2,
                    6 => 2,
                    7 => 3,
                    8 => 3,
                    _ => 1
                };
            }
        }

        public 银色荆棘(Character? character = null) : base(SkillType.Magic, character)
        {
            Effects.Add(new 银色荆棘特效(this, false, 0, 2, 0, 0.6, 0.04));
        }
    }

    public class 银色荆棘特效 : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"对{Skill.TargetDescription()}造成 {Damage:0.##} 点{CharacterSet.GetDamageTypeName(DamageType.Magical, MagicType)}。" +
            $"随后 {ActualConfusionProbability * 100:0.##}% 概率使目标进入混乱状态，持续 {持续时间}。混乱：进入行动受限状态，失控并随机行动，且在进行攻击指令时，可能会选取友方角色为目标。";
        public override DispelledType DispelledType => DispelledType.Strong;
        public override EffectType EffectType => EffectType.Confusion;

        private string 持续时间 => _durative && _duration > 0 ? 实际持续时间 + $" {GameplayEquilibriumConstant.InGameTime}" : (!_durative && _durationTurn > 0 ? 实际持续时间 + " 回合" : $"0 {GameplayEquilibriumConstant.InGameTime}");
        private double 实际持续时间 => _durative && _duration > 0 ? _duration + _levelGrowth * (Level - 1) : (!_durative && _durationTurn > 0 ? _durationTurn + _levelGrowth * (Level - 1) : 0);
        private double Damage => Skill.Level > 0 ? 基础数值伤害 + 基础伤害等级成长 * (Skill.Level - 1) : 基础数值伤害;
        private double 基础数值伤害 { get; set; } = 50;
        private double 基础伤害等级成长 { get; set; } = 50;
        private double ActualConfusionProbability => Level > 0 ? _confusionProbability + _confusionProbabilityLevelGrowth * (Level - 1) : _confusionProbability;
        private readonly bool _durative;
        private readonly double _duration;
        private readonly int _durationTurn;
        private readonly double _levelGrowth;
        private readonly double _confusionProbability;
        private readonly double _confusionProbabilityLevelGrowth;

        public 银色荆棘特效(Skill skill, bool durative = false, double duration = 0, int durationTurn = 1, double levelGrowth = 0, double confusionProbability = 0, double confusionProbabilityLevelGrowth = 0) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            _durative = durative;
            _duration = duration;
            _durationTurn = durationTurn;
            _levelGrowth = levelGrowth;
            _confusionProbability = confusionProbability;
            _confusionProbabilityLevelGrowth = confusionProbabilityLevelGrowth;
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, Dictionary<string, object> others)
        {
            foreach (Character target in targets)
            {
                DamageToEnemy(caster, target, DamageType.Magical, MagicType, Damage);
                if (target.HP > 0 && Random.Shared.NextDouble() < ActualConfusionProbability)
                {
                    WriteLine($"[ {target} ] 陷入了混乱！！持续时间：{持续时间}！");
                    混乱 e = new(Skill, caster);
                    if (_durative && _duration > 0)
                    {
                        e.Durative = true;
                        e.Duration = 实际持续时间;
                        e.RemainDuration = 实际持续时间;
                    }
                    else if (!_durative && _durationTurn > 0)
                    {
                        e.Durative = false;
                        e.DurationTurn = (int)实际持续时间;
                        e.RemainDurationTurn = (int)实际持续时间;
                    }
                    target.Effects.Add(e);
                    e.OnEffectGained(target);
                    GamingQueue?.LastRound.ApplyEffects.TryAdd(target, [e.EffectType]);
                }
            }
        }
    }
}
