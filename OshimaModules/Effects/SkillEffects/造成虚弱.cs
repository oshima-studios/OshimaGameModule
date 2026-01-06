using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.PassiveEffects;
using Oshima.FunGame.OshimaModules.Skills;

namespace Oshima.FunGame.OshimaModules.Effects.SkillEffects
{
    public class 造成虚弱 : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"对{Skill.TargetDescription()}造成虚弱 {虚弱时间}，伤害降低 {ActualDamageReductionPercent * 100:0.##}%，" +
            $"物理护甲降低 {ActualDEFReductionPercent * 100:0.##}%，魔法抗性降低 {ActualMDFReductionPercent * 100:0.##}%，治疗效果降低 {ActualHealingReductionPercent * 100:0.##}%。";
        public override DispelledType DispelledType => DispelledType.Weak;
        public override EffectType EffectType => EffectType.Weaken;
        public override bool ExemptDuration => true;

        private string 虚弱时间 => _durative && _duration > 0 ? 实际虚弱时间 + $" {GameplayEquilibriumConstant.InGameTime}" : (!_durative && _durationTurn > 0 ? 实际虚弱时间 + " 回合" : $"0 {GameplayEquilibriumConstant.InGameTime}");
        private double 实际虚弱时间 => _durative && _duration > 0 ? _duration + _levelGrowth * (Level - 1) : (!_durative && _durationTurn > 0 ? _durationTurn + _levelGrowth * (Level - 1) : 0);
        private readonly bool _durative;
        private readonly double _duration;
        private readonly int _durationTurn;
        private readonly double _levelGrowth;
        private readonly double _damageReductionPercent;
        private readonly double _DEFReductionPercent;
        private readonly double _MDFReductionPercent;
        private readonly double _healingReductionPercent;
        private readonly double _damageReductionPercentLevelGrowth;
        private readonly double _DEFReductionPercentLevelGrowth;
        private readonly double _MDFReductionPercentLevelGrowth;
        private readonly double _healingReductionPercentLevelGrowth;
        private double ActualDamageReductionPercent => Level > 0 ? _damageReductionPercent + _damageReductionPercentLevelGrowth * (Level - 1) : _damageReductionPercent;
        private double ActualDEFReductionPercent => Level > 0 ? _DEFReductionPercent + _DEFReductionPercentLevelGrowth * (Level - 1) : _DEFReductionPercent;
        private double ActualMDFReductionPercent => Level > 0 ? _MDFReductionPercent + _MDFReductionPercentLevelGrowth * (Level - 1) : _MDFReductionPercent;
        private double ActualHealingReductionPercent => Level > 0 ? _healingReductionPercent + _healingReductionPercentLevelGrowth * (Level - 1) : _healingReductionPercent;

        public 造成虚弱(Skill skill, bool durative = false, double duration = 0, int durationTurn = 1, double levelGrowth = 0,
            double damageReductionPercent = 0, double DEFReductionPercent = 0, double MDFReductionPercent = 0, double healingReductionPercent = 0,
            double damageReductionPercentLevelGrowth = 0, double DEFReductionPercentLevelGrowth = 0, double MDFReductionPercentLevelGrowth = 0, double healingReductionPercentLevelGrowth = 0) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            _durative = durative;
            _duration = duration;
            _durationTurn = durationTurn;
            _levelGrowth = levelGrowth;
            _damageReductionPercent = damageReductionPercent;
            _DEFReductionPercent = DEFReductionPercent;
            _MDFReductionPercent = MDFReductionPercent;
            _healingReductionPercent = healingReductionPercent;
            _damageReductionPercentLevelGrowth = damageReductionPercentLevelGrowth;
            _DEFReductionPercentLevelGrowth = DEFReductionPercentLevelGrowth;
            _MDFReductionPercentLevelGrowth = MDFReductionPercentLevelGrowth;
            _healingReductionPercentLevelGrowth = healingReductionPercentLevelGrowth;
        }

        public override async Task OnSkillCasted(Character caster, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            foreach (Character enemy in targets)
            {
                WriteLine($"[ {caster} ] 对 [ {enemy} ] 造成了虚弱！伤害降低 {ActualDamageReductionPercent * 100:0.##}%，" +
                    $"物理护甲降低 {ActualDEFReductionPercent * 100:0.##}%，魔法抗性降低 {ActualMDFReductionPercent * 100:0.##}%，" +
                    $"治疗效果降低 {ActualHealingReductionPercent * 100:0.##}%！持续时间：{虚弱时间}！");
                虚弱 e = new(Skill, enemy, caster, _durative, _duration + _levelGrowth * (Level - 1), Convert.ToInt32(_durationTurn + _levelGrowth * (Level - 1)), ActualDamageReductionPercent, ActualDEFReductionPercent, ActualMDFReductionPercent, ActualHealingReductionPercent);
                enemy.Effects.Add(e);
                e.OnEffectGained(enemy);
                GamingQueue?.LastRound.AddApplyEffects(enemy, e.EffectType);
            }
        }
    }
}
