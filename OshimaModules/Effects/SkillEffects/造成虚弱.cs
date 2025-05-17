using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.PassiveEffects;

namespace Oshima.FunGame.OshimaModules.Effects.SkillEffects
{
    public class 造成虚弱 : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"对{TargetDescription}造成虚弱 {虚弱时间}，伤害降低 {_damageReductionPercent * 100:0.##}%，" +
            $"物理护甲降低 {_DEFReductionPercent * 100:0.##}%，魔法抗性降低 {_MDFReductionPercent * 100:0.##}%，治疗效果降低 {_healingReductionPercent * 100:0.##}%。";
        public override DispelledType DispelledType => DispelledType.Weak;
        public string TargetDescription => Skill.SelectAllEnemies ? "敌方全体角色" : $"目标{(Skill.CanSelectTargetCount > 1 ? $"至多 {Skill.CanSelectTargetCount} 个" : "")}敌人";

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

        public 造成虚弱(Skill skill, bool durative = false, double duration = 0, int durationTurn = 1, double levelGrowth = 0,
            double damageReductionPercent = 0, double DEFReductionPercent = 0, double MDFReductionPercent = 0, double healingReductionPercent = 0) : base(skill)
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
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, Dictionary<string, object> others)
        {
            foreach (Character enemy in targets)
            {
                WriteLine($"[ {caster} ] 对 [ {enemy} ] 造成了虚弱！伤害降低 {_damageReductionPercent * 100:0.##}%，" +
                    $"物理护甲降低 {_DEFReductionPercent * 100:0.##}%，魔法抗性降低 {_MDFReductionPercent * 100:0.##}%，" +
                    $"治疗效果降低 {_healingReductionPercent * 100:0.##}%！持续时间：{虚弱时间}！");
                虚弱 e = new(Skill, enemy, caster, _durative, _duration, _durationTurn, _damageReductionPercent, _DEFReductionPercent, _MDFReductionPercent, _healingReductionPercent);
                enemy.Effects.Add(e);
                e.OnEffectGained(enemy);
                GamingQueue?.LastRound.ApplyEffects.TryAdd(enemy, [e.EffectType]);
            }
        }
    }
}
