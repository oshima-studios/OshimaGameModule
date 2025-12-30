using Milimoe.FunGame.Core.Entity;
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
        public override string Description => $"基于普通攻击的目标选择器对目标发起 2 次普通攻击。若该回合已使用过普通攻击，则只会发起 1 次普通攻击；使用该技能后，该回合不再允许普通攻击。伤害特效可叠加；不受 [ 攻击受限 ] 状态的限制。";

        public 双连击特效(Skill skill) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, Dictionary<string, object> others)
        {
            if (GamingQueue != null)
            {
                bool hasAttacked = false;
                if (GamingQueue.CharacterDecisionPoints.TryGetValue(caster, out DecisionPoints? dp) && dp != null)
                {
                    hasAttacked = dp.ActionTypes.Contains(CharacterActionType.NormalAttack);
                    dp.ActionTypes.Add(CharacterActionType.NormalAttack);
                }
                caster.NormalAttack.Attack(GamingQueue, caster, targets);
                if (!hasAttacked)
                {
                    caster.NormalAttack.Attack(GamingQueue, caster, targets);
                }
            }
        }
    }
}
