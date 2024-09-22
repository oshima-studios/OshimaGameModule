using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Effects
{
    public class 眩晕 : Effect
    {
        public override long Id => 4101;
        public override string Name => "眩晕";
        public override string Description => $"此角色被眩晕了，不能行动。来自：[ {Source} ] 的 [ {Skill.Name} ]";
        public override EffectType EffectType => EffectType.Stun;
        public override bool TargetSelf => true;
        public override Character Source => _sourceCharacter;
        public override bool Durative => _durative;
        public override double Duration => _duration;
        public override int DurationTurn => _durationTurn;

        private readonly Character _sourceCharacter;
        private readonly bool _durative;
        private readonly double _duration;
        private readonly int _durationTurn;

        public 眩晕(Skill skill, Character sourceCharacter, bool durative = false, double duration = 0, int durationTurn = 1) : base(skill)
        {
            ActionQueue = skill.ActionQueue;
            _sourceCharacter = sourceCharacter;
            _durative = durative;
            _duration = duration;
            _durationTurn = durationTurn;
        }

        public override void OnEffectGained(Character character)
        {
            if (_durative) RemainDuration = Duration;
            else RemainDurationTurn = DurationTurn;
            character.CharacterEffectStates.Add(this, [CharacterState.NotActionable]);
            character.UpdateCharacterState();
            InterruptCasting(character, Source);
        }

        public override void OnEffectLost(Character character)
        {
            character.CharacterEffectStates.Remove(this);
            character.UpdateCharacterState();
        }
    }
}
