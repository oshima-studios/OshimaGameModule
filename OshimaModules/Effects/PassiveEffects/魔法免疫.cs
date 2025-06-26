using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Effects.PassiveEffects
{
    public class 魔法免疫 : Effect
    {
        public override long Id => 4111;
        public override string Name => "魔法免疫";
        public override string Description => $"此角色处于魔法免疫状态，无法选中其作为魔法技能的目标（自释放魔法技能除外），并且免疫魔法伤害。来自：[ {Source} ] 的 [ {Skill.Name} ]";
        public override EffectType EffectType => EffectType.MagicalImmune;
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

        public 魔法免疫(Skill skill, Character sourceCharacter, bool durative = false, double duration = 0, int durationTurn = 1) : base(skill)
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
            AddImmuneTypesToCharacter(character, [ImmuneType.Magical]);
        }

        public override void OnEffectLost(Character character)
        {
            RemoveImmuneTypesFromCharacter(character);
        }

        public override bool OnImmuneCheck(Character character, Character target, ISkill skill, Item? item = null)
        {
            if (character == target)
            {
                return false;
            }
            return true;
        }
    }
}
