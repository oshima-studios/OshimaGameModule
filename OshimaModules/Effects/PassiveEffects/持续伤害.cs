using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;

namespace Oshima.FunGame.OshimaModules.Effects.PassiveEffects
{
    public class 持续伤害 : Effect
    {
        public override long Id => (long)PassiveEffectID.持续伤害;
        public override string Name { get; set; } = "持续伤害";
        public override string Description => $"此角色正受到持续伤害，每{GameplayEquilibriumConstant.InGameTime}受到 {(_isPercentage ? $"{_durationDamagePercent * 100:0.##}% 当前生命值 [ {Damage:0.##} ]" : Damage.ToString("0.##"))} 点{CharacterSet.GetDamageTypeName(_damageType)}。来自：[ {Source} ] 的 [ {Skill.Name} ]";
        public override bool IsDebuff { get; set; } = true;
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
        private readonly double _durationDamage;
        private readonly double _durationDamagePercent;
        private readonly DamageType _damageType = DamageType.Physical;
        private readonly DamageCalculationOptions? _options = null;
        private double Damage => _isPercentage ? _targetCharacter.HP * _durationDamagePercent : _durationDamage;

        public 持续伤害(Skill skill, Character targetCharacter, Character sourceCharacter, bool durative = false, double duration = 0, int durationTurn = 1,
            bool isPercentage = true, double durationDamage = 100, double durationDamagePercent = 0.02, DamageType damageType = DamageType.Physical, DamageCalculationOptions? options = null) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            _targetCharacter = targetCharacter;
            _sourceCharacter = sourceCharacter;
            _durative = durative;
            _duration = duration;
            _durationTurn = durationTurn;
            _isPercentage = isPercentage;
            _durationDamage = durationDamage;
            _durationDamagePercent = durationDamagePercent;
            _damageType = damageType;
            _options = options;
        }

        private double GetDamage(double hp, double elapsed)
        {
            if (hp <= 0)
            {
                return 0;
            }
            double damage = _isPercentage ? hp * _durationDamagePercent : _durationDamage;
            return damage * elapsed;
        }

        public override void OnTimeElapsed(Character character, double elapsed)
        {
            if (character == _targetCharacter && character.HP > 0)
            {
                double hp = character.HP;
                double damage = GetDamage(hp, elapsed);
                if (elapsed > 1)
                {
                    damage = 0;
                    int loop = 0;
                    int elapsedSecond = (int)elapsed;
                    for (; loop < elapsedSecond; loop++)
                    {
                        double current = GetDamage(hp, 1);
                        damage += current;
                        hp -= current;
                        elapsed--;
                    }
                    if (elapsed > 0)
                    {
                        damage += GetDamage(hp, elapsed);
                    }
                }
                DamageToEnemy(Source, character, _damageType, MagicType, damage, _options);
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
            AddEffectTypeToCharacter(character, [EffectType]);
        }

        public override void OnEffectLost(Character character)
        {
            RemoveEffectTypesFromCharacter(character);
        }
    }
}
