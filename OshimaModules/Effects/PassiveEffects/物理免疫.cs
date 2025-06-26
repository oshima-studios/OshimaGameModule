using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Effects.PassiveEffects
{
    public class 物理免疫 : Effect
    {
        public override long Id => 4112;
        public override string Name => "物理免疫";
        public override string Description => $"此角色处于物理免疫状态，免疫物理伤害。来自：[ {Source} ] 的 [ {Skill.Name} ]";
        public override EffectType EffectType => EffectType.PhysicalImmune;
        public override DispelledType DispelledType => DispelledType.Weak;
        public override bool IsDebuff => false;
        public override Character Source => _sourceCharacter;
        public override bool Durative => _durative;
        public override double Duration => _duration;
        public override int DurationTurn => _durationTurn;

        private readonly Character _sourceCharacter;
        private readonly bool _durative;
        private readonly double _duration;
        private readonly int _durationTurn;

        public 物理免疫(Skill skill, Character sourceCharacter, bool durative = false, double duration = 0, int durationTurn = 1) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            _sourceCharacter = sourceCharacter;
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
            AddImmuneTypesToCharacter(character, [ImmuneType.Physical]);
        }

        public override void OnEffectLost(Character character)
        {
            RemoveImmuneTypesFromCharacter(character);
        }
    }
}
