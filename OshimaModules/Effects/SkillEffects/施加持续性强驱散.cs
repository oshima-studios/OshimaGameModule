using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.PassiveEffects;
using Oshima.FunGame.OshimaModules.Skills;

namespace Oshima.FunGame.OshimaModules.Effects.SkillEffects
{
    public class 施加持续性强驱散 : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"持续强驱散{Skill.TargetDescription()}{(_durativeWithoutDuration ? _durationString : $"，持续 {持续时间}")}。\r\n持续性驱散是持续性临时驱散，它会在持续时间结束之后恢复目标尚未结束的特效。";
        public override DispelType DispelType => DispelType.DurativeStrong;

        private string 持续时间 => _durative && _duration > 0 ? 实际持续时间 + $" {GameplayEquilibriumConstant.InGameTime}" : (!_durative && _durationTurn > 0 ? 实际持续时间 + " 回合" : $"0 {GameplayEquilibriumConstant.InGameTime}");
        private double 实际持续时间 => _durative && _duration > 0 ? _duration + _levelGrowth * (Level - 1) : (!_durative && _durationTurn > 0 ? _durationTurn + _levelGrowth * (Level - 1) : 0);
        private readonly bool _durativeWithoutDuration;
        private readonly string _durationString;
        private readonly bool _durative;
        private readonly double _duration;
        private readonly int _durationTurn;
        private readonly double _levelGrowth;

        public 施加持续性强驱散(Skill skill, bool durativeWithoutDuration = false, string durationString = "", bool durative = false, double duration = 0, int durationTurn = 1, double levelGrowth = 0) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            _durativeWithoutDuration = durativeWithoutDuration;
            _durationString = durationString;
            if (!_durativeWithoutDuration)
            {
                _durative = durative;
                _duration = duration;
                _durationTurn = durationTurn;
                _levelGrowth = levelGrowth;
            }
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, Dictionary<string, object> others)
        {
            Dictionary<Character, bool> isTeammateDictionary = GamingQueue?.GetIsTeammateDictionary(caster, targets) ?? [];
            foreach (Character target in targets)
            {
                WriteLine($"[ {caster} ] 对 [ {target} ] 施加了持续性强驱散！持续时间：{持续时间}！");
                bool isDebuff = true;
                if (isTeammateDictionary.TryGetValue(target, out bool value))
                {
                    isDebuff = !value;
                }
                if (target == caster)
                {
                    isDebuff = false;
                }
                if (target.Effects.FirstOrDefault(e => e is 持续性强驱散) is 持续性强驱散 e && e.DurativeWithoutDuration == _durativeWithoutDuration && e.Durative == _durative && e.IsDebuff == isDebuff)
                {
                    if (_duration > e.Duration) e.Duration = _duration;
                    if (_durationTurn > e.DurationTurn) e.DurationTurn = _durationTurn;
                    e.DispelledType = DispelledType;
                    e.ParentEffect = ParentEffect;
                }
                else
                {
                    e = new(Skill, caster, _durativeWithoutDuration, _durative, _duration, _durationTurn);
                    target.Effects.Add(e);
                    e.OnEffectGained(target);
                    e.IsDebuff = isDebuff;
                    e.DispelledType = DispelledType;
                    e.ParentEffect = ParentEffect;
                }
                GamingQueue?.LastRound.ApplyEffects.TryAdd(target, [e.EffectType]);
            }
        }
    }
}
