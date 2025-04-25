using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Effects.PassiveEffects
{
    public class 封技 : Effect
    {
        public override long Id => 4103;
        public override string Name => "封技";
        public override string Description => $"此角色被封技了，不能使用技能（魔法、战技和爆发技）。来自：[ {Source} ] 的 [ {Skill.Name} ]";
        public override EffectType EffectType => EffectType.Silence;
        public override DispelledType DispelledType => DispelledType.Weak;
        public override Character Source => _sourceCharacter;
        public override bool Durative => _durative;
        public override double Duration => _duration;
        public override int DurationTurn => _durationTurn;

        private readonly Character _sourceCharacter;
        private readonly bool _durative;
        private readonly double _duration;
        private readonly int _durationTurn;

        public 封技(Skill skill, Character sourceCharacter, bool durative = false, double duration = 0, int durationTurn = 1) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            _sourceCharacter = sourceCharacter;
            _durative = durative;
            _duration = duration;
            _durationTurn = durationTurn;
        }

        public override void OnEffectGained(Character character)
        {
            if (_durative) RemainDuration = Duration;
            else RemainDurationTurn = DurationTurn;
            AddEffectStatesToCharacter(character, [CharacterState.SkillRestricted]);
            InterruptCasting(character, Source);
        }

        public override void OnEffectLost(Character character)
        {
            RemoveEffectStatesFromCharacter(character);
        }
    }
}
