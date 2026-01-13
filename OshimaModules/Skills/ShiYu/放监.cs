using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 放监 : Skill
    {
        public override long Id => (long)SuperSkillID.放监;
        public override string Name => "放监";
        public override string Description => Effects.Count > 0 ? ((放监特效)Effects.First()).通用描述 : "";
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

    public class 放监特效 : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description { get; set; } = "";
        public override DispelledType DispelledType => DispelledType.CannotBeDispelled;

        public string 通用描述 => $"使场上现有的时雨标记变得不可驱散，并且刷新为持续 3 回合。并给予持有时雨标记的敌方角色 [ 宫监手标记 ]，宫监手标记不可驱散，持续 3 回合。持有宫监手标记的角色，必须完成以下两个任务以消除标记，否则将在标记消失时受到基于{Skill.SkillOwner()} {核心属性系数 * 100:0.##}% 核心属性 + {攻击力系数 * 100:0.##}% 攻击力 [ {Skill.Character?.PrimaryAttributeValue * 核心属性系数 + Skill.Character?.ATK * 攻击力系数:0.##} ] 的真实伤害：\r\n" +
            $"1. 使用 [ 普通攻击 ] 攻击一次队友，此伤害必定暴击且无视闪避；\r\n2. 对{Skill.SkillOwner()}释放一个指向性技能，{Skill.SkillOwner()}将此技能效果无效化并且复制该技能获得使用权持续 4 回合，该复制品没有冷却时间。";
        public double 核心属性系数 => 1.1 * Skill.Level;
        public double 攻击力系数 => 0.4 + 0.2 * (Skill.Level - 1);

        public 放监特效(Skill skill) : base(skill)
        {
            Description = 通用描述;
        }

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
