using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 开宫 : Skill
    {
        public override long Id => (long)PassiveID.开宫;
        public override string Name => "开宫";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";

        public 开宫(Character? character = null) : base(SkillType.Passive, character)
        {
            Effects.Add(new 开宫特效(this));
        }

        public override IEnumerable<Effect> AddPassiveEffectToCharacter()
        {
            return Effects;
        }
    }

    public class 开宫特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"当场上有角色死亡时，如果该角色死于技能，则{Skill.SkillOwner()}获得该技能的使用权持续 3 回合；如果该角色死于普通攻击，则{Skill.SkillOwner()}的普通攻击将转为魔法伤害并且无视闪避，持续 3 回合。" +
            $"然后给予击杀者时雨标记，如果击杀者为队友，{Skill.SkillOwner()}对其的治疗加成提升 100%，并且使其能够攻击{Skill.SkillOwner()}，攻击{Skill.SkillOwner()}时，视为对{Skill.SkillOwner()}治疗，治疗值基于伤害值的 50%；如果为敌人，{Skill.SkillOwner()}对其的伤害加成提升 200%，并且使其能够攻击其队友。时雨标记持续 3 回合。";

        public override void OnEffectGained(Character character)
        {

        }

        public override void OnEffectLost(Character character)
        {

        }
    }
}
