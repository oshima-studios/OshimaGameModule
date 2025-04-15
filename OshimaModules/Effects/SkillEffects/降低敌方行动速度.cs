using Milimoe.FunGame.Core.Entity;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;

namespace Oshima.FunGame.OshimaModules.Effects.SkillEffects
{
    public class 降低敌方行动速度 : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"降低目标{(Skill.CanSelectTargetCount > 1 ? $"至多 {Skill.CanSelectTargetCount} 个" : "")}敌人 {Math.Abs(SPD):0.##} 点行动速度 {持续时间}。";

        private double SPD => Level > 0 ? -Math.Abs(基础数值速度 + 基础速度等级成长 * (Level - 1)) : -Math.Abs(基础数值速度);
        private double 基础数值速度 { get; set; } = 30;
        private double 基础速度等级成长 { get; set; } = 20;
        private string 持续时间 => _durative && _duration > 0 ? _duration + $" {GameplayEquilibriumConstant.InGameTime}" : (!_durative && _durationTurn > 0 ? _durationTurn + " 回合" : $"0 {GameplayEquilibriumConstant.InGameTime}");
        private readonly bool _durative;
        private readonly double _duration;
        private readonly int _durationTurn;

        public 降低敌方行动速度(Skill skill, double 基础数值速度, double 基础速度等级成长, bool durative = true, double duration = 40, int durationTurn = 0) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            this.基础数值速度 = 基础数值速度;
            this.基础速度等级成长 = 基础速度等级成长;
            _durative = durative;
            _duration = duration;
            _durationTurn = durationTurn;
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, Dictionary<string, object> others)
        {
            foreach (Character target in targets)
            {
                WriteLine($"[ {target} ] 的行动速度降低了 {SPD:0.##} ！持续时间：{持续时间} {GameplayEquilibriumConstant.InGameTime}！");
                ExSPD e = new(Skill, new Dictionary<string, object>()
                {
                    { "exspd", SPD }
                }, caster)
                {
                    Durative = _durative,
                    Duration = _duration,
                    DurationTurn = _durationTurn
                };
                target.Effects.Add(e);
                e.OnEffectGained(target);
                GamingQueue?.LastRound.Effects.TryAdd(target, e.EffectType);
            }
        }
    }
}
