using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;

namespace Oshima.FunGame.OshimaModules.Effects.PassiveEffects
{
    public class 混乱 : Effect
    {
        public override long Id => (long)PassiveEffectID.混乱;
        public override string Name => "混乱";
        public override string Description => $"此角色处于混乱状态，行动受限且失控，行动回合中无法自主行动而是随机行动，在进行攻击指令时，可能会选取友方角色为目标。来自：[ {Source} ] 的 [ {Skill.Name} ]";
        public override EffectType EffectType => EffectType.Confusion;
        public override DispelledType DispelledType => DispelledType.Strong;
        public override bool IsDebuff => true;
        public override Character Source => _sourceCharacter;
        public override bool Durative => _durative;
        public override double Duration => _duration;
        public override int DurationTurn => _durationTurn;

        private readonly Character _sourceCharacter;
        private readonly bool _durative;
        private readonly double _duration;
        private readonly int _durationTurn;

        public 混乱(Skill skill, Character sourceCharacter, bool durative = false, double duration = 0, int durationTurn = 1) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            _sourceCharacter = sourceCharacter;
            _durative = durative;
            _duration = duration;
            _durationTurn = durationTurn;
        }

        public override void AlterSelectListBeforeAction(Character character, List<Character> enemys, List<Character> teammates, List<Skill> skills, Dictionary<Character, int> continuousKilling, Dictionary<Character, int> earnedMoney)
        {
            // 为了确保角色能够混乱行动，这里需要将角色设置为可行动
            if (character.CharacterState == CharacterState.ActionRestricted)
            {
                GamingQueue?.SetCharactersToAIControl(true, false, character);
                character.CharacterState = CharacterState.Actionable;
            }
            enemys.AddRange(teammates);
            teammates.AddRange(enemys);
        }

        public override CharacterActionType AlterActionTypeBeforeAction(Character character, DecisionPoints dp, CharacterState state, ref bool canUseItem, ref bool canCastSkill, ref double pUseItem, ref double pCastSkill, ref double pNormalAttack, ref bool forceAction)
        {
            forceAction = true;
            return Milimoe.FunGame.Core.Model.GamingQueue.GetActionType(dp, pUseItem, pCastSkill, pNormalAttack);
        }

        public override void OnTurnEnd(Character character)
        {
            character.UpdateCharacterState();
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
            GamingQueue?.SetCharactersToAIControl(true, false, character);
            AddEffectStatesToCharacter(character, [CharacterState.ActionRestricted]);
            AddEffectTypeToCharacter(character, [EffectType.Confusion]);
            InterruptCasting(character, Source);
        }

        public override void OnEffectLost(Character character)
        {
            GamingQueue?.SetCharactersToAIControl(true, true, character);
            RemoveEffectStatesFromCharacter(character);
            RemoveEffectTypesFromCharacter(character);
        }
    }
}
