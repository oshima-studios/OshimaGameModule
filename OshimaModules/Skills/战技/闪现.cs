using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 闪现 : Skill
    {
        public override long Id => (long)SkillID.闪现;
        public override string Name => "闪现";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";
        public override double EPCost => 25;
        public override double CD => 35 - 1.5 * Level;
        public override double HardnessTime { get; set; } = 3;
        public override bool IsNonDirectional => true;
        public override bool CanSelectSelf => true;
        public override bool CanSelectEnemy => false;
        public override bool CanSelectTeammate => false;
        public override int CanSelectTargetRange => 0;
        public override bool SelectIncludeCharacterGrid => false;
        public override bool AllowSelectNoCharacterGrid => true;

        public 闪现(Character? character = null) : base(SkillType.Skill, character)
        {
            CastRange = 9;
            Effects.Add(new 闪现特效(this));
        }
    }

    public class 闪现特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"立即将角色传送到范围内的任意一个没有被角色占据的指定地点，并附赠一次战技的决策点配额。";
        public override string DispelDescription => "";

        public override void OnSkillCasted(Character caster, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            if (GamingQueue?.Map is GameMap map && grids.Count > 0)
            {
                map.CharacterMove(caster, map.GetCharacterCurrentGrid(caster), grids[0]);
            }
            if (GamingQueue != null && GamingQueue.CharacterDecisionPoints.TryGetValue(caster, out DecisionPoints? dp) && dp != null)
            {
                dp.AddTempActionQuota(this, CharacterActionType.CastSkill);
            }
        }
    }
}
