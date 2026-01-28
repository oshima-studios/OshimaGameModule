using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;

namespace Oshima.FunGame.OshimaModules.Effects.PassiveEffects
{
    public class 复制技能 : Effect
    {
        public override long Id => (long)PassiveEffectID.复制技能;
        public override string Name => "复制技能";
        public override string Description => $"此角色持有技能 [ {_skill.Name} ] 的复制品。来自：[ {Source} ]";
        public override EffectType EffectType => EffectType.Mark;
        public override Character Source => _sourceCharacter;
        public override DispelledType DispelledType => DispelledType.CannotBeDispelled;
        public Skill CopiedSkill => _skill;

        private readonly Character _sourceCharacter;
        private readonly Skill _skill;

        public 复制技能(Skill skill, Character sourceCharacter, Skill copiedSkill) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            _sourceCharacter = sourceCharacter;
            _skill = copiedSkill.Copy();
            _skill.Level = copiedSkill.Level;
        }

        public CharacterActionType LastType { get; set; } = CharacterActionType.None;
        public Skill? LastSkill { get; set; } = null;

        public override void OnEffectGained(Character character)
        {
            _skill.Character = character;
            _skill.OnLevelUp();
            character.Skills.Add(_skill);
        }

        public override void OnEffectLost(Character character)
        {
            _skill.RemoveSkillFromCharacter(character);
        }
    }
}
