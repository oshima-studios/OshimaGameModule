using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 灾难冲击波 : Skill
    {
        public override long Id => (long)MagicID.灾难冲击波;
        public override string Name => "灾难冲击波";
        public override string Description => string.Join("\r\n", Effects.Select(e => e.Description));
        public override string DispelDescription => Effects.Count > 0 ? Effects.First(e => e is 灾难冲击波特效).DispelDescription : "";
        public override string ExemptionDescription => Effects.Count > 0 ? Effects.First(e => e is 灾难冲击波特效).ExemptionDescription : "";
        public override double MPCost => Level > 0 ? 95 + (75 * (Level - 1)) : 95;
        public override double CD => Level > 0 ? 35 - (1.5 * (Level - 1)) : 35;
        public override double CastTime => Level > 0 ? 6 + (0.5 * (Level - 1)) : 6;
        public override double HardnessTime { get; set; } = 5;
        public override int CanSelectTargetCount
        {
            get
            {
                return Level switch
                {
                    3 => 2,
                    4 => 2,
                    5 => 3,
                    6 => 3,
                    7 => 4,
                    8 => 4,
                    _ => 1
                };
            }
        }

        public 灾难冲击波(Character? character = null) : base(SkillType.Magic, character)
        {
            Effects.Add(new 基于攻击力的伤害_带基础伤害(this, 50, 15, 0.1, 0.04));
            Effects.Add(new 灾难冲击波特效(this, false, 0, 2, 0, 0.03, 0.02));
        }
    }

    public class 灾难冲击波特效 : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"对{Skill.TargetDescription()}造成魔法抗性降低 {ActualMDFReductionPercent * 100:0.##}%，持续 {持续时间}。";
        public override EffectType EffectType => EffectType.MagicResistBreak;
        public override DispelledType DispelledType => DispelledType.Weak;
        public override bool ExemptDuration => true;

        private string 持续时间 => _durative && _duration > 0 ? 实际持续时间 + $" {GameplayEquilibriumConstant.InGameTime}" : (!_durative && _durationTurn > 0 ? 实际持续时间 + " 回合" : $"0 {GameplayEquilibriumConstant.InGameTime}");
        private double 实际持续时间 => _durative && _duration > 0 ? _duration + _levelGrowth * (Level - 1) : (!_durative && _durationTurn > 0 ? _durationTurn + _levelGrowth * (Level - 1) : 0);
        private readonly bool _durative;
        private readonly double _duration;
        private readonly int _durationTurn;
        private readonly double _levelGrowth;
        private readonly double _MDFReductionPercent;
        private readonly double _MDFReductionPercentLevelGrowth;
        private double ActualMDFReductionPercent => Level > 0 ? _MDFReductionPercent + _MDFReductionPercentLevelGrowth * (Level - 1) : _MDFReductionPercent;

        public 灾难冲击波特效(Skill skill, bool durative = false, double duration = 0, int durationTurn = 1, double levelGrowth = 0, double MDFReductionPercent = 0, double MDFReductionPercentLevelGrowth = 0) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            _durative = durative;
            _duration = duration;
            _durationTurn = durationTurn;
            _levelGrowth = levelGrowth;
            _MDFReductionPercent = MDFReductionPercent;
            _MDFReductionPercentLevelGrowth = MDFReductionPercentLevelGrowth;
        }

        public override async Task OnSkillCasted(Character caster, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            foreach (Character target in targets)
            {
                WriteLine($"[ {target} ] 的魔法抗性降低了 {ActualMDFReductionPercent * 100:0.##}%！持续时间：{持续时间}！");
                ExMDF e = new(Skill, new(){
                    { "mdftype", 0 },
                    { "mdfvalue", -ActualMDFReductionPercent }
                }, caster);
                target.Effects.Add(e);
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
                e.EffectType = EffectType.MagicResistBreak;
                e.Source = caster;
                e.OnEffectGained(target);
                GamingQueue?.LastRound.AddApplyEffects(target, e.EffectType);
            }
        }
    }
}
