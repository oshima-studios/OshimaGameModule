using System.Text;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public static class SkillExtension
    {
        public static string TargetDescription(this Skill skill)
        {
            if (skill.IsNonDirectional)
            {
                return skill.RangeTargetDescription();
            }

            string str;

            if (skill.SelectAllTeammates)
            {
                str = "友方全体角色";
            }
            else if (skill.SelectAllEnemies)
            {
                str = "敌方全体角色";
            }
            else if (skill.CanSelectTeammate && !skill.CanSelectEnemy)
            {
                str = $"目标{(skill.CanSelectTargetCount > 1 ? $"至多 {skill.CanSelectTargetCount} 个" : "")}友方角色{(!skill.CanSelectSelf ? "（不可选择自身）" : "")}";
            }
            else if (!skill.CanSelectTeammate && skill.CanSelectEnemy)
            {
                str = $"目标{(skill.CanSelectTargetCount > 1 ? $"至多 {skill.CanSelectTargetCount} 个" : "")}敌方角色";
            }
            else if (!skill.CanSelectTeammate && !skill.CanSelectEnemy && skill.CanSelectSelf)
            {
                str = $"自身";
            }
            else
            {
                str = $"{(skill.CanSelectTargetCount > 1 ? $"至多 {skill.CanSelectTargetCount} 个" : "")}目标";
            }

            if (skill.CanSelectTargetRange > 0)
            {
                str += $"以及以{(skill.CanSelectTargetCount > 1 ? "这些" : "该")}目标为中心，半径为 {skill.CanSelectTargetRange} 格的菱形区域中的等同阵营角色";
            }

            return str;
        }

        public static string RangeTargetDescription(this Skill skill)
        {
            string str = "";

            int range = skill.CanSelectTargetRange;
            if (range <= 0)
            {
                str = "目标地点";
            }
            else
            {
                switch (skill.SkillRangeType)
                {
                    case SkillRangeType.Diamond:
                        str = "目标菱形区域";
                        break;
                    case SkillRangeType.Circle:
                        str = "目标圆形区域";
                        break;
                    case SkillRangeType.Square:
                        str = "目标正方形区域";
                        break;
                    case SkillRangeType.Line:
                        str = "自身与目标地点之间的直线区域";
                        break;
                    case SkillRangeType.LinePass:
                        str = "自身与目标地点之间的直线区域以及贯穿该目标地点直至地图边缘的直线区域";
                        break;
                    case SkillRangeType.Sector:
                        str = "目标扇形区域";
                        break;
                    default:
                        break;
                }
            }

            if (skill.SelectIncludeCharacterGrid)
            {
                if (skill.CanSelectTeammate && !skill.CanSelectEnemy)
                {
                    str = $"{str}中的所有友方角色{(!skill.CanSelectSelf ? "（包括自身）" : "")}";
                }
                else if (!skill.CanSelectTeammate && skill.CanSelectEnemy)
                {
                    str = $"{str}中的所有敌方角色";
                }
                else
                {
                    str = $"{str}中的所有角色";
                }
            }
            else
            {
                str = "一个不包含被角色占据的";
            }

            return str;
        }
    }
}
