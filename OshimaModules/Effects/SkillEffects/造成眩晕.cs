using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.PassiveEffects;

namespace Oshima.FunGame.OshimaModules.Effects.SkillEffects
{
    public class 造成眩晕 : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"对{Skill.TargetDescription()}造成眩晕 {眩晕时间}。";
        public override DispelledType DispelledType => DispelledType.Strong;
        public override EffectType EffectType => EffectType.Stun;
        public override bool ExemptDuration => true;

        private string 眩晕时间 => _durative && _duration > 0 ? 实际眩晕时间 + $" {GameplayEquilibriumConstant.InGameTime}" : (!_durative && _durationTurn > 0 ? 实际眩晕时间 + " 回合" : $"0 {GameplayEquilibriumConstant.InGameTime}");
        private double 实际眩晕时间 => _durative && _duration > 0 ? _duration + _levelGrowth * (Level - 1) : (!_durative && _durationTurn > 0 ? _durationTurn + _levelGrowth * (Level - 1) : 0);
        private readonly bool _durative;
        private readonly double _duration;
        private readonly int _durationTurn;
        private readonly double _levelGrowth;

        public 造成眩晕(Skill skill, bool durative = false, double duration = 0, int durationTurn = 1, double levelGrowth = 0) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            _durative = durative;
            _duration = duration;
            _durationTurn = durationTurn;
            _levelGrowth = levelGrowth;
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            foreach (Character enemy in targets)
            {
                if (enemy.HP <= 0) continue;
                眩晕 e = new(Skill, caster, _durative, _duration + _levelGrowth * (Level - 1), Convert.ToInt32(_durationTurn + _levelGrowth * (Level - 1)));
                if (!CheckExemption(caster, enemy, e))
                {
                    WriteLine($"[ {caster} ] 眩晕了 [ {enemy} ] ！持续时间：{眩晕时间}！");
                    enemy.Effects.Add(e);
                    e.OnEffectGained(enemy);
                    GamingQueue?.LastRound.AddApplyEffects(enemy, e.EffectType);
                }
            }
        }
    }
}
