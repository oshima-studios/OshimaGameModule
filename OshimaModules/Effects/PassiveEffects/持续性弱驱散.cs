using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;

namespace Oshima.FunGame.OshimaModules.Effects.PassiveEffects
{
    public class 持续性弱驱散 : Effect
    {
        public override long Id => (long)PassiveEffectID.持续性弱驱散;
        public override string Name => "持续性弱驱散";
        public override string Description => $"此角色正在被持续性弱驱散。无法保护吟唱动作。来自：[ {Source} ] 的 [ {Skill.Name} ]";
        public override EffectType EffectType => EffectType.WeakDispelling;
        public override DispelType DispelType => DispelType.DurativeWeak;
        public override Character Source => _sourceCharacter;
        public override bool DurativeWithoutDuration => _durativeWithoutDuration;
        public override bool Durative => _durative;
        public override double Duration => _duration;
        public override int DurationTurn => _durationTurn;

        private readonly Character _sourceCharacter;
        private readonly bool _durativeWithoutDuration;
        private readonly bool _durative;
        private readonly double _duration;
        private readonly int _durationTurn;

        public 持续性弱驱散(Skill skill, Character sourceCharacter, bool durativeWithoutDuration = false, bool durative = false, double duration = 0, int durationTurn = 1) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            DispelledType = DispelledType.Weak;
            _sourceCharacter = sourceCharacter;
            _durativeWithoutDuration = durativeWithoutDuration;
            if (!_durativeWithoutDuration)
            {
                _durative = durative;
                _duration = duration;
                _durationTurn = durationTurn;
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
        }
    }
}
