using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;

namespace Oshima.FunGame.OshimaModules.Effects.PassiveEffects
{
    public class 持续回复 : Effect
    {
        public override long Id => (long)PassiveEffectID.持续回复;
        public override string Name => "持续回复";
        public override string Description => $"此角色处于持续回复状态，每{GameplayEquilibriumConstant.InGameTime}回复 {(_isPercentage ? $"{_durationHealPercent * 100:0.##}% [ {Heal:0.##} ]" : Heal.ToString("0.##"))} 点当前生命值。来自：[ {Source} ] 的 [ {Skill.Name} ]";
        public override EffectType EffectType => EffectType.HealOverTime;
        public override bool IsDebuff => false;
        public override Character Source => _sourceCharacter;
        public override bool Durative => _durative;
        public override double Duration => _duration;
        public override int DurationTurn => _durationTurn;

        private readonly Character _targetCharacter;
        private readonly Character _sourceCharacter;
        private readonly bool _durative;
        private readonly double _duration;
        private readonly int _durationTurn;
        private readonly bool _isPercentage;
        private readonly double _durationHeal;
        private readonly double _durationHealPercent;
        private double Heal => _isPercentage ? _targetCharacter.HP * _durationHealPercent : _durationHeal;

        public 持续回复(Skill skill, Character targetCharacter, Character sourceCharacter, bool durative = false, double duration = 0, int durationTurn = 1,
            bool isPercentage = true, double durationHeal = 100, double durationHealPercent = 0.02) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            _targetCharacter = targetCharacter;
            _sourceCharacter = sourceCharacter;
            _durative = durative;
            _duration = duration;
            _durationTurn = durationTurn;
            _isPercentage = isPercentage;
            _durationHeal = durationHeal;
            _durationHealPercent = durationHealPercent;
        }

        private double GetHeal(double hp, double elapsed)
        {
            if (hp <= 0)
            {
                return 0;
            }
            double heal = _isPercentage ? hp * _durationHealPercent : _durationHeal;
            return heal * elapsed;
        }

        public override void OnTimeElapsed(Character character, double elapsed)
        {
            if (character == _targetCharacter && character.HP > 0)
            {
                double hp = character.HP;
                double heal = GetHeal(hp, elapsed);
                if (elapsed > 1)
                {
                    heal = 0;
                    int loop = 0;
                    int elapsedSecond = (int)elapsed;
                    for (; loop < elapsedSecond; loop++)
                    {
                        double current = GetHeal(hp, 1);
                        heal += current;
                        hp -= current;
                        elapsed--;
                    }
                    if (elapsed > 0)
                    {
                        heal += GetHeal(hp, elapsed);
                    }
                }
                HealToTarget(Source, character, heal, triggerEffects: false);
            }
        }

        public override void OnEffectGained(Character character)
        {
            if (_durative && RemainDuration == 0)
            {
                RemainDuration = Duration;
            }
            else if (RemainDurationTurn == 0)
            {
                RemainDurationTurn = DurationTurn;
            }
            AddEffectTypeToCharacter(character, [EffectType.HealOverTime]);
        }

        public override void OnEffectLost(Character character)
        {
            RemoveEffectTypesFromCharacter(character);
        }
    }
}
