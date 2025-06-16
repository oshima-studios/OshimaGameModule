using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Effects.PassiveEffects
{
    public class 物理护盾 : Effect
    {
        public override long Id => 4106;
        public override string Name => "物理护盾";
        public override string Description => $"此角色拥有物理护盾。来自：[ {Source} ] 的 [ {Skill.Name} ]";
        public override EffectType EffectType => EffectType.Shield;
        public override DispelledType DispelledType => DispelledType.CannotBeDispelled;
        public override bool DurativeWithoutDuration => true;
        public override bool IsDebuff => false;
        public override Character Source => _sourceCharacter;
        public override bool Durative => _durative;
        public override double Duration => _duration;
        public override int DurationTurn => _durationTurn;

        private readonly Character _sourceCharacter;
        private readonly double _shield;
        private readonly bool _durative;
        private readonly double _duration;
        private readonly int _durationTurn;

        public 物理护盾(Skill skill, Character sourceCharacter, double shield, bool durative = false, double duration = 0, int durationTurn = 0) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            _sourceCharacter = sourceCharacter;
            _durative = durative;
            _duration = duration;
            _durationTurn = durationTurn;
            _shield = shield;
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
            character.Shield.AddShieldOfEffect(new(this, _shield, false, MagicType.None));
        }

        public override void OnEffectLost(Character character)
        {
            character.Shield.RemoveShieldOfEffect(this);
        }

        public override bool OnShieldBroken(Character character, Character attacker, Effect effet, double overFlowing)
        {
            if (effet == this)
            {
                character.Shield.RemoveShieldOfEffect(this);
                character.Effects.Remove(this);
            }
            return true;
        }
    }
}
