using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 神之因果 : Skill
    {
        public override long Id => (long)SuperSkillID.神之因果;
        public override string Name => "神之因果";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";
        public override double EPCost => 100;
        public override bool CostAllEP => true;
        public override double CD => 90;
        public override double HardnessTime { get; set; } = 1;
        public override bool CanSelectSelf => true;
        public override bool CanSelectEnemy => false;

        public 神之因果(Character? character = null) : base(SkillType.SuperSkill, character)
        {
            Effects.Add(new 神之因果特效(this));
        }
    }

    public class 神之因果特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"{Skill.SkillOwner()}展开一个持续 3 回合的宏大领域，覆盖整个战场。在此领域内，一切必然都将被概率暂时覆盖。\r\n" +
            $"领域规则一：命运共享。领域持续期间，所有角色（包括自身）的暴击率、闪避率将被暂时移除，替换为一个统一的命运值（初始为 50%）。每次进行相关检定时，都基于此命运值进行计算。\r\n" +
            $"领域规则二：骰子指挥。{Skill.SkillOwner()}的每次行动前，可任意指定一名角色（不论敌友）进行一次强制骰子检定。\r\n" +
            $"投掷结果将强制该角色执行以下行动之一：\r\n" +
            $"1.攻击：攻击力提升 {攻击力提升 * 100:0.##}% 并随机对一名敌方角色进行普通攻击。\r\n" +
            $"2.防御：{Skill.SkillOwner()}结束本回合并进入防御状态，下一次行动前，获得 {减伤提升 * 100:0.##}% 物理伤害减免和魔法抗性。\r\n" +
            $"3.奉献：治疗生命值百分比最低的一名角色，治疗量为：2d20 * {Skill.SkillOwner()} {智力系数 * 100:0.##}% 智力 [ {Skill.Character?.INT * 智力系数:0.##} ]。\r\n" +
            $"4.混乱：对该角色周围造成一次 [ 6d{Skill.Level} * 60 ] 点真实伤害（不论敌友）。\r\n" +
            $"完成强制骰子检定后，{Skill.SkillOwner()}可指定任意一名角色提升 10% 命运值。\r\n" +
            $"领域规则三：观测奖励。领域持续期间，每当有角色因命运值检定成功（如闪避、暴击）或执行了强制骰子行动，{Skill.SkillOwner()}都会获得一层因果点。" +
            $"领域结束时，每层因果点将转化为{Skill.SkillOwner()}对随机一名角色造成 [ 10d100 * 因果点 ] 的真实伤害（敌方）或治疗（友方）。";
        public override DispelledType DispelledType => DispelledType.CannotBeDispelled;

        public double 攻击力提升 => 0.3 + 0.1 * (Skill.Level - 1);
        public double 减伤提升 => 0.2 + 0.08 * (Skill.Level - 1);
        public double 智力系数 => 1.5 + 0.02 * (Skill.Level - 1);

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
