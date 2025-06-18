using Milimoe.FunGame.Core.Entity;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public static class SkillExtension
    {
        public static string TargetDescription(this Skill skill)
        {
            if (skill.SelectAllTeammates)
            {
                return "友方全体角色";
            }
            else if (skill.SelectAllEnemies)
            {
                return "敌方全体角色";
            }
            if (skill.CanSelectTeammate && !skill.CanSelectEnemy)
            {
                return $"目标{(skill.CanSelectTargetCount > 1 ? $"至多 {skill.CanSelectTargetCount} 个" : "")}友方角色";
            }
            else if (!skill.CanSelectTeammate && skill.CanSelectEnemy)
            {
                return $"目标{(skill.CanSelectTargetCount > 1 ? $"至多 {skill.CanSelectTargetCount} 个" : "")}敌方角色";
            }
            else
            {
                return $"{(skill.CanSelectTargetCount > 1 ? $"至多 {skill.CanSelectTargetCount} 个" : "")}目标";
            }
        }
    }
}
