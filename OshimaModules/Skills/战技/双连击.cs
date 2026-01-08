using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 双连击 : Skill
    {
        public override long Id => (long)SkillID.双连击;
        public override string Name => "双连击";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";
        public override double EPCost => 60;
        public override double CD => 30;
        public override double HardnessTime { get; set; } = 10;
        public override bool CanSelectSelf => Character?.NormalAttack.CanSelectSelf ?? false;
        public override bool CanSelectTeammate => Character?.NormalAttack.CanSelectTeammate ?? false;
        public override bool CanSelectEnemy => Character?.NormalAttack.CanSelectEnemy ?? true;
        public override int CanSelectTargetCount => Character?.NormalAttack.CanSelectTargetCount ?? 1;

        public 双连击(Character? character = null) : base(SkillType.Skill, character)
        {
            Effects.Add(new 双连击特效(this));
        }
    }

    public class 双连击特效 : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"基于普通攻击的目标选择器对目标发起 2 次普通攻击。双连击会占用 1 次普通攻击的决策点配额，当配额不足时，仅能发起 1 次普通攻击。伤害特效可叠加；不受 [ 攻击受限 ] 状态的限制。";

        public 双连击特效(Skill skill) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            if (GamingQueue != null)
            {
                bool checkQuota = true;
                if (GamingQueue.CharacterDecisionPoints.TryGetValue(caster, out DecisionPoints? dp) && dp != null)
                {
                    checkQuota = dp.CheckActionTypeQuota(CharacterActionType.NormalAttack);
                    dp.AddActionType(CharacterActionType.NormalAttack, false);
                }
                caster.NormalAttack.Attack(GamingQueue, caster, null, targets);
                if (checkQuota)
                {
                    caster.NormalAttack.Attack(GamingQueue, caster, null, targets);
                }
            }
        }
    }
}
