using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.PassiveEffects;

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
        public override string Description => $"{Skill.SkillOwner()}进入 [ {nameof(长期监视)} ] 状态，时刻监视着场上的一举一动。当场上有角色死亡时，如果该角色死于技能，则{Skill.SkillOwner()}复制该技能获得使用权，持续 3 回合，该复制品没有冷却时间；如果该角色死于普通攻击，则{Skill.SkillOwner()}的普通攻击将转为魔法伤害并且无视闪避，持续 3 回合。" +
            $"接着，{Skill.SkillOwner()}给予击杀者 [ {nameof(时雨标记)} ]。{Skill.SkillOwner()}在造成魔法伤害时，会基于伤害值的 50% 治疗持有标记的友方角色；{Skill.SkillOwner()}与所有持有标记的友方角色对持有标记的敌方角色的伤害加成提升 100%，并且使持有标记的敌方角色在持续时间内的回合开始阶段，有 60% 概率陷入混乱。" +
            $"混乱：进入行动受限状态，失控并随机行动，且在进行攻击指令时，可能会选取友方角色为目标。时雨标记持续 3 回合。";

        public override void OnEffectGained(Character character)
        {

        }

        public override void OnEffectLost(Character character)
        {

        }
    }
}
