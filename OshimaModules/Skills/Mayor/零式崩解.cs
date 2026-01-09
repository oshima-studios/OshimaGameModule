using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 零式崩解 : Skill
    {
        public override long Id => (long)PassiveID.零式崩解;
        public override string Name => "零式崩解";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";

        public 零式崩解(Character? character = null) : base(SkillType.Passive, character)
        {
            Effects.Add(new 零式崩解特效(this));
        }

        public override IEnumerable<Effect> AddPassiveEffectToCharacter()
        {
            return Effects;
        }
    }

    public class 零式崩解特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"{Skill.SkillOwner()}的零式剑法能够轻松命中敌人弱点。{Skill.SkillOwner()}的暴击伤害提升 70%。";

        public override void OnEffectGained(Character character)
        {
            character.ExCritDMG += 0.7;
        }

        public override void OnEffectLost(Character character)
        {
            character.ExCritDMG -= 0.7;
        }
    }
}
