using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 时间减速复 : Skill
    {
        public override long Id => (long)MagicID.时间减速复;
        public override string Name => "时间减速·复";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string ExemptionDescription => Effects.Count > 0 ? Effects.First().ExemptionDescription : "";
        public override double MPCost => Level > 0 ? 85 + (95 * (Level - 1)) : 85;
        public override double CD => Level > 0 ? 32 - (1 * (Level - 1)) : 32;
        public override double CastTime => Level > 0 ? 4 + (1 * (Level - 1)) : 4;
        public override double HardnessTime { get; set; } = 6;
        public override int CanSelectTargetCount
        {
            get
            {
                return Level switch
                {
                    3 => 3,
                    4 => 3,
                    5 => 3,
                    6 => 4,
                    7 => 4,
                    8 => 4,
                    _ => 2
                };
            }
        }
        public override double MagicBottleneck => 14 + 14 * (Level - 1);

        public 时间减速复(Character? character = null) : base(SkillType.Magic, character)
        {
            Effects.Add(new 时间减速复特效(this, false, 0, 3, 0, 40, 30, 0.08, 0.01));
        }
    }

    public class 时间减速复特效 : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"{(!IsDebuff ? "增加" : "减少")}{Skill.TargetDescription()} {Math.Abs(ExSPD):0.##} 点行动速度，并{(!IsDebuff ? "缩短" : "延长")}目标 30% 的行动等待时间（当前硬直时间）；" +
            $"{(!IsDebuff ? "增加" : "减少")}{Skill.TargetDescription()} {Math.Abs(ExACC) * 100:0.##}% 加速系数。持续 {持续时间}。";
        public override EffectType EffectType => EffectType.Slow;
        public override DispelledType DispelledType => DispelledType.Weak;
        public override bool ExemptDuration => true;
        public override bool IsDebuff => true;

        private string 持续时间 => _durative && _duration > 0 ? 实际持续时间 + $" {GameplayEquilibriumConstant.InGameTime}" : (!_durative && _durationTurn > 0 ? 实际持续时间 + " 回合" : $"0 {GameplayEquilibriumConstant.InGameTime}");
        private double 实际持续时间 => _durative && _duration > 0 ? (_duration + _levelGrowth * (Level - 1) * MagicEfficacy) : (!_durative && _durationTurn > 0 ? ((int)Math.Round(_durationTurn + _levelGrowth * (Level - 1) * MagicEfficacy, 0, MidpointRounding.ToPositiveInfinity)) : 0);

        private readonly bool _durative;
        private readonly double _duration;
        private readonly int _durationTurn;
        private readonly double _levelGrowth;
        private readonly double _baseSpd;
        private readonly double _baseAcc;
        private readonly double _spdLevelGrowth;
        private readonly double _accLevelGrowth;

        private double ExSPD => Level > 0 ? _baseSpd + _spdLevelGrowth * (Level - 1) : _baseSpd;
        private double ExACC => Level > 0 ? _baseAcc + _accLevelGrowth * (Level - 1) : _baseAcc;

        public 时间减速复特效(Skill skill, bool durative = false, double duration = 0, int durationTurn = 1, double levelGrowth = 0, double baseSpd = 0, double spdLevelGrowth = 0, double baseAcc = 0, double accLevelGrowth = 0) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            _durative = durative;
            _duration = duration;
            _durationTurn = durationTurn;
            _levelGrowth = levelGrowth;
            _baseSpd = baseSpd;
            _spdLevelGrowth = spdLevelGrowth;
            _baseAcc = baseAcc;
            _accLevelGrowth = accLevelGrowth;
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            foreach (Character target in targets)
            {
                Effect e1 = new ExSPD(Skill, new Dictionary<string, object>()
                {
                    { "exspd", -ExSPD }
                }, caster)
                {
                    Name = Name + "·行动速度降低",
                    Durative = _durative,
                    Duration = 实际持续时间,
                    DurationTurn = (int)实际持续时间
                };
                if (!CheckExemption(caster, target, e1))
                {
                    WriteLine($"[ {target} ] 的行动速度降低了 {ExSPD:0.##} 点，行动等待时间（当前硬直时间）被延长了 30%！持续时间：{持续时间}！");
                    target.Effects.Add(e1);
                    e1.OnEffectGained(target);
                    e1.IsDebuff = true;
                    GamingQueue?.ChangeCharacterHardnessTime(target, 0.3, true, false);
                }
                Effect e2 = new AccelerationCoefficient(Skill, new()
                {
                    { "exacc", -ExACC }
                }, caster)
                {
                    Name = Name + "·加速系数降低",
                    Durative = _durative,
                    Duration = 实际持续时间,
                    DurationTurn = (int)实际持续时间
                };
                if (!CheckExemption(caster, target, e2))
                {
                    WriteLine($"[ {target} ] 的加速系数降低了 {ExACC * 100:0.##}%！持续时间：{持续时间}！");
                    target.Effects.Add(e2);
                    e2.OnEffectGained(target);
                    e2.IsDebuff = true;
                    GamingQueue?.LastRound.AddApplyEffects(target, EffectType.Slow);
                }
            }
        }
    }
}
