using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Effects.PassiveEffects
{
    public class 魔法护盾 : Effect
    {
        public override long Id => 4107;
        public override string Name => "魔法护盾";
        public override string Description => $"此角色拥有魔法护盾{CurrentShield}。来自：[ {Source} ] 的 [ {Skill.Name} ]";
        public override EffectType EffectType => EffectType.Shield;
        public override DispelledType DispelledType => DispelledType.CannotBeDispelled;
        public override bool DurativeWithoutDuration => true;
        public override bool IsDebuff => false;
        public override Character Source => _sourceCharacter;
        public override bool Durative => _durative;
        public override double Duration => _duration;
        public override int DurationTurn => _durationTurn;

        private string CurrentShield
        {
            get
            {
                if (_targetCharacter != null && _targetCharacter.Shield.ShieldOfEffects.TryGetValue(this, out ShieldOfEffect? value) && value != null)
                {
                    return $"，护盾值：{value.Shield:0.##} 点";
                }
                else return "";
            }
        }
        private readonly Character _targetCharacter;
        private readonly Character _sourceCharacter;
        private readonly double _shield;
        private readonly bool _durative;
        private readonly double _duration;
        private readonly int _durationTurn;

        public 魔法护盾(Skill skill, Character targetCharacter, Character sourceCharacter, double shield, bool durative = false, double duration = 0, int durationTurn = 0) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            _targetCharacter = targetCharacter;
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
            character.Shield.AddShieldOfEffect(new(this, _shield, ShieldType.Magical, MagicType.None));
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
