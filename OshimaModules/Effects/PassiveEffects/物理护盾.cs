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

        public 物理护盾(Skill skill, Character sourceCharacter, double shield, bool durative = false, double duration = 0, int durationTurn = 1) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            _sourceCharacter = sourceCharacter;
            _shield = shield;
            _durative = durative;
            _duration = duration;
            _durationTurn = durationTurn;
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
            character.Shield.Physical += _shield;
        }

        public override bool OnShieldBroken(Character character, Character attacker, bool isMagic, MagicType magicType, double damage, double shield, double overFlowing)
        {
            Effect[] effects = [.. character.Effects.Where(e => e is 物理护盾)];
            foreach (Effect effect in effects)
            {
                character.Effects.Remove(effect);
            }
            return true;
        }
    }
}
