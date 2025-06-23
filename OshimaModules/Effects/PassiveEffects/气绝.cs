using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Effects.PassiveEffects
{
    public class 气绝 : Effect
    {
        public override long Id => 4109;
        public override string Name => "气绝";
        public override string Description => $"此角色处于气绝状态，行动受限并且每{GameplayEquilibriumConstant.InGameTime}持续流失 {(_isPercentage ? $"{_durationDamagePercent * 100:0.##}% [ {Damage:0.##} ]" : Damage.ToString("0.##"))} 点当前生命值，此生命流失效果不会导致角色死亡。来自：[ {Source} ] 的 [ {Skill.Name} ]";
        public override EffectType EffectType => EffectType.Bleed;
        public override DispelledType DispelledType => DispelledType.Strong;
        public override bool IsDebuff => true;
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
        private double Damage => _isPercentage ? _targetCharacter.HP * _durationDamagePercent : _durationDamage;

        public 气绝(Skill skill, Character targetCharacter, Character sourceCharacter, bool durative = false, double duration = 0, int durationTurn = 1,
            bool isPercentage = true, double durationDamage = 100, double durationDamagePercent = 0.02) : base(skill)
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
                character.HP -= damage;
                if (character.HP <= 0)
                {
                    character.HP = 1;
                }
                GamingQueue?.CalculateCharacterDamageStatistics(Source, character, damage, DamageType.True);
                WriteLine($"[ {character} ] 因气绝而流失了 {damage:0.##} 点生命值！");
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
            AddEffectTypeToCharacter(character, [EffectType.Bleed]);
            AddEffectStatesToCharacter(character, [CharacterState.ActionRestricted]);
            InterruptCasting(character, Source);
        }

        public override void OnEffectLost(Character character)
        {
            RemoveEffectTypesFromCharacter(character);
            RemoveEffectStatesFromCharacter(character);
        }
    }
}
