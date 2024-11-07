using Milimoe.FunGame.Core.Entity;
using Oshima.FunGame.OshimaModules.Effects.PassiveEffects;

namespace Oshima.FunGame.OshimaModules.Effects.SkillEffects
{
    public class 造成眩晕 : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"对目标{(TargetCount > 1 ? $"至多 {TargetCount} 个" : "")}敌人造成眩晕 {眩晕时间}。";
        public override bool TargetSelf => false;

        private string 眩晕时间 => _durative && _duration > 0 ? _duration + " 时间" : (!_durative && _durationTurn > 0 ? _durationTurn + " 回合" : "0 时间");
        private readonly bool _durative;
        private readonly double _duration;
        private readonly int _durationTurn;

        public 造成眩晕(Skill skill, bool durative = false, double duration = 0, int durationTurn = 1) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            _durative = durative;
            _duration = duration;
            _durationTurn = durationTurn;
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, Dictionary<string, object> others)
        {
            foreach (Character enemy in targets)
            {
                WriteLine($"[ {caster} ] 眩晕了 [ {enemy} ] ！持续时间：{眩晕时间}！");
                眩晕 e = new(Skill, caster, false, 0, 1);
                enemy.Effects.Add(e);
                e.OnEffectGained(enemy);
                GamingQueue?.LastRound.Effects.TryAdd(enemy, e.EffectType);
            }
        }
    }
}
