using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;

namespace Oshima.FunGame.OshimaModules.Effects.PassiveEffects
{
    public class 迟滞 : Effect
    {
        public override long Id => (long)PassiveEffectID.迟滞;
        public override string Name { get; set; } = "迟滞";
        public override string Description => $"此角色处于迟滞状态，普通攻击和技能的硬直时间、当前行动等待时间延长 {_hardnessReductionPercent * 100:0.##}%。来自：[ {Source} ] 的 [ {Skill.Name} ]";
        public override EffectType EffectType => EffectType.Slow;
        public override DispelledType DispelledType => DispelledType.Weak;
        public override bool IsDebuff => true;
        public override Character Source => _sourceCharacter;
        public override bool Durative => _durative;
        public override double Duration => _duration;
        public override int DurationTurn => _durationTurn;
        public override bool ExemptDuration => true;

        private readonly Character _sourceCharacter;
        private readonly bool _durative;
        private readonly double _duration;
        private readonly int _durationTurn;
        private readonly double _hardnessReductionPercent;

        public 迟滞(Skill skill, Character sourceCharacter, bool durative = false, double duration = 0, int durationTurn = 1, double healingReductionPercent = 0) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            _sourceCharacter = sourceCharacter;
            _durative = durative;
            _duration = duration;
            _durationTurn = durationTurn;
            _hardnessReductionPercent = healingReductionPercent;
        }

        public override void AlterHardnessTimeAfterCastSkill(Character character, Skill skill, ref double baseHardnessTime, ref bool isCheckProtected)
        {
            baseHardnessTime *= 1 + _hardnessReductionPercent;
        }

        public override void AlterHardnessTimeAfterNormalAttack(Character character, ref double baseHardnessTime, ref bool isCheckProtected)
        {
            baseHardnessTime *= 1 + _hardnessReductionPercent;
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
            AddEffectTypeToCharacter(character, [EffectType.Slow]);
        }

        public override void OnEffectLost(Character character)
        {
            RemoveEffectTypesFromCharacter(character);
        }

        public void ApplyChange(Character character)
        {
            GamingQueue?.ChangeCharacterHardnessTime(character, _hardnessReductionPercent, true, false);
        }
    }
}
