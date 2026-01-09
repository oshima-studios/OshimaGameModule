using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 放监 : Skill
    {
        public override long Id => (long)SuperSkillID.放监;
        public override string Name => "放监";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";
        public override double EPCost => 100;
        public override double CD => 65;
        public override double HardnessTime { get; set; } = 14;
        public override bool CanSelectSelf => true;
        public override bool CanSelectEnemy => false;

        public 放监(Character? character = null) : base(SkillType.SuperSkill, character)
        {
            Effects.Add(new 放监特效(this));
        }
    }

    public class 放监特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"使时雨标记变得不可驱散，并且延长至 5 回合。持有标记的角色，必须完成以下任务来消除标记，否则将在标记消失时受到基于{Skill.SkillOwner()} 660% 核心属性 + 100% 攻击力 [ {Skill.Character?.PrimaryAttributeValue * 6.6 + Skill.Character?.ATK:0.##} ] 的魔法伤害（该伤害必定暴击）：\r\n" +
            $"1. 如果是敌人，则必须攻击一次队友，此伤害必定暴击且无视闪避；如果是队友，必须攻击一次{Skill.SkillOwner()}，治疗加成再度提升 100%。\r\n2. 对{Skill.SkillOwner()}释放一个指向性技能，{Skill.SkillOwner()}将此技能效果无效化并且获得该技能的使用权持续 4 回合。\r\n此技能对队友的伤害将不会导致队友死亡。";
        public override DispelledType DispelledType => DispelledType.CannotBeDispelled;

        public override void OnSkillCasted(Character caster, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            RemainDuration = Duration;
            if (!caster.Effects.Contains(this))
            {
                caster.Effects.Add(this);
                OnEffectGained(caster);
            }
            GamingQueue?.LastRound.AddApplyEffects(caster, EffectType.CritBoost, EffectType.PenetrationBoost);
        }
    }
}
