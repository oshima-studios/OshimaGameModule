using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.Core.Models;
using Oshima.Core.Utils;
using Oshima.FunGame.OshimaModules.Skills;

namespace Oshima.Core.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FunGameController(ILogger<UserDailyController> logger) : ControllerBase
    {
        private readonly ILogger<UserDailyController> _logger = logger;

        [HttpGet("test")]
        public List<string> GetTest([FromQuery] bool? isweb = null)
        {
            if (isweb ?? true)
            {
                return FunGameSimulation.StartGame(false, true);
            }
            else
            {
                return FunGameSimulation.StartGame(false, false);
            }
        }

        [HttpGet("stats")]
        public string GetStats([FromQuery] int? id = null)
        {
            if (id != null && id > 0 && id <= FunGameSimulation.Characters.Count)
            {
                Character character = FunGameSimulation.Characters[Convert.ToInt32(id) - 1];
                if (FunGameSimulation.CharacterStatistics.TryGetValue(character, out CharacterStatistics? stats) && stats != null)
                {
                    StringBuilder builder = new();

                    builder.AppendLine(character.ToString());
                    builder.AppendLine($"总计造成伤害：{stats.TotalDamage:0.##} / 场均：{stats.AvgDamage:0.##}");
                    builder.AppendLine($"总计造成物理伤害：{stats.TotalPhysicalDamage:0.##} / 场均：{stats.AvgPhysicalDamage:0.##}");
                    builder.AppendLine($"总计造成魔法伤害：{stats.TotalMagicDamage:0.##} / 场均：{stats.AvgMagicDamage:0.##}");
                    builder.AppendLine($"总计造成真实伤害：{stats.TotalRealDamage:0.##} / 场均：{stats.AvgRealDamage:0.##}");
                    builder.AppendLine($"总计承受伤害：{stats.TotalTakenDamage:0.##} / 场均：{stats.AvgTakenDamage:0.##}");
                    builder.AppendLine($"总计承受物理伤害：{stats.TotalTakenPhysicalDamage:0.##} / 场均：{stats.AvgTakenPhysicalDamage:0.##}");
                    builder.AppendLine($"总计承受魔法伤害：{stats.TotalTakenMagicDamage:0.##} / 场均：{stats.AvgTakenMagicDamage:0.##}");
                    builder.AppendLine($"总计承受真实伤害：{stats.TotalTakenRealDamage:0.##} / 场均：{stats.AvgTakenRealDamage:0.##}");
                    builder.AppendLine($"总计存活回合数：{stats.LiveRound} / 场均：{stats.AvgLiveRound}");
                    builder.AppendLine($"总计行动回合数：{stats.ActionTurn} / 场均：{stats.AvgActionTurn}");
                    builder.AppendLine($"总计存活时长：{stats.LiveTime:0.##} / 场均：{stats.AvgLiveTime:0.##}");
                    builder.AppendLine($"总计赚取金钱：{stats.TotalEarnedMoney} / 场均：{stats.AvgEarnedMoney}");
                    builder.AppendLine($"每回合伤害：{stats.DamagePerRound:0.##}");
                    builder.AppendLine($"每行动回合伤害：{stats.DamagePerTurn:0.##}");
                    builder.AppendLine($"每秒伤害：{stats.DamagePerSecond:0.##}");
                    builder.AppendLine($"总计击杀数：{stats.Kills}" + (stats.Plays != 0 ? $" / 场均：{(double)stats.Kills / stats.Plays:0.##}" : ""));
                    builder.AppendLine($"总计死亡数：{stats.Deaths}" + (stats.Plays != 0 ? $" / 场均：{(double)stats.Deaths / stats.Plays:0.##}" : ""));
                    builder.AppendLine($"总计助攻数：{stats.Assists}" + (stats.Plays != 0 ? $" / 场均：{(double)stats.Assists / stats.Plays:0.##}" : ""));
                    builder.AppendLine($"总计参赛数：{stats.Plays}");
                    builder.AppendLine($"总计冠军数：{stats.Wins}");
                    builder.AppendLine($"总计前三数：{stats.Top3s}");
                    builder.AppendLine($"总计败场数：{stats.Loses}");
                    builder.AppendLine($"胜率：{stats.Winrates * 100:0.##}%");
                    builder.AppendLine($"前三率：{stats.Top3rates * 100:0.##}%");
                    builder.AppendLine($"上次排名：{stats.LastRank} / 场均名次：{stats.AvgRank}");

                    return NetworkUtility.JsonSerialize(builder.ToString());
                }
            }
            return NetworkUtility.JsonSerialize("");
        }

        [HttpGet("cjs")]
        public string GetCharacterIntroduce([FromQuery] int? id = null)
        {
            if (id != null && id > 0 && id <= FunGameSimulation.Characters.Count)
            {
                Character c = FunGameSimulation.Characters[Convert.ToInt32(id) - 1].Copy();
                c.Level = General.GameplayEquilibriumConstant.MaxLevel;
                c.NormalAttack.Level = General.GameplayEquilibriumConstant.MaxNormalAttackLevel;

                Skill 冰霜攻击 = new 冰霜攻击(c)
                {
                    Level = General.GameplayEquilibriumConstant.MaxMagicLevel
                };
                c.Skills.Add(冰霜攻击);

                Skill 疾风步 = new 疾风步(c)
                {
                    Level = General.GameplayEquilibriumConstant.MaxSkillLevel
                };
                c.Skills.Add(疾风步);

                if (id == 1)
                {
                    Skill META马 = new META马(c)
                    {
                        Level = 1
                    };
                    c.Skills.Add(META马);

                    Skill 力量爆发 = new 力量爆发(c)
                    {
                        Level = General.GameplayEquilibriumConstant.MaxMagicLevel
                    };
                    c.Skills.Add(力量爆发);
                }

                if (id == 2)
                {
                    Skill 心灵之火 = new 心灵之火(c)
                    {
                        Level = 1
                    };
                    c.Skills.Add(心灵之火);

                    Skill 天赐之力 = new 天赐之力(c)
                    {
                        Level = General.GameplayEquilibriumConstant.MaxSkillLevel
                    };
                    c.Skills.Add(天赐之力);
                }

                if (id == 3)
                {
                    Skill 魔法震荡 = new 魔法震荡(c)
                    {
                        Level = 1
                    };
                    c.Skills.Add(魔法震荡);

                    Skill 魔法涌流 = new 魔法涌流(c)
                    {
                        Level = General.GameplayEquilibriumConstant.MaxSkillLevel
                    };
                    c.Skills.Add(魔法涌流);
                }

                if (id == 4)
                {
                    Skill 灵能反射 = new 灵能反射(c)
                    {
                        Level = 1
                    };
                    c.Skills.Add(灵能反射);

                    Skill 三重叠加 = new 三重叠加(c)
                    {
                        Level = General.GameplayEquilibriumConstant.MaxSkillLevel
                    };
                    c.Skills.Add(三重叠加);
                }

                if (id == 5)
                {
                    Skill 智慧与力量 = new 智慧与力量(c)
                    {
                        Level = 1
                    };
                    c.Skills.Add(智慧与力量);

                    Skill 变幻之心 = new 变幻之心(c)
                    {
                        Level = General.GameplayEquilibriumConstant.MaxSkillLevel
                    };
                    c.Skills.Add(变幻之心);
                }

                if (id == 6)
                {
                    Skill 致命打击 = new 致命打击(c)
                    {
                        Level = 1
                    };
                    c.Skills.Add(致命打击);

                    Skill 精准打击 = new 精准打击(c)
                    {
                        Level = General.GameplayEquilibriumConstant.MaxSkillLevel
                    };
                    c.Skills.Add(精准打击);
                }

                if (id == 7)
                {
                    Skill 毁灭之势 = new 毁灭之势(c)
                    {
                        Level = 1
                    };
                    c.Skills.Add(毁灭之势);

                    Skill 绝对领域 = new 绝对领域(c)
                    {
                        Level = General.GameplayEquilibriumConstant.MaxSkillLevel
                    };
                    c.Skills.Add(绝对领域);
                }

                if (id == 8)
                {
                    Skill 枯竭打击 = new 枯竭打击(c)
                    {
                        Level = 1
                    };
                    c.Skills.Add(枯竭打击);

                    Skill 能量毁灭 = new 能量毁灭(c)
                    {
                        Level = General.GameplayEquilibriumConstant.MaxSkillLevel
                    };
                    c.Skills.Add(能量毁灭);
                }

                if (id == 9)
                {
                    Skill 玻璃大炮 = new 玻璃大炮(c)
                    {
                        Level = 1
                    };
                    c.Skills.Add(玻璃大炮);

                    Skill 迅捷之势 = new 迅捷之势(c)
                    {
                        Level = General.GameplayEquilibriumConstant.MaxSkillLevel
                    };
                    c.Skills.Add(迅捷之势);
                }

                if (id == 10)
                {
                    Skill 累积之压 = new 累积之压(c)
                    {
                        Level = 1
                    };
                    c.Skills.Add(累积之压);

                    Skill 嗜血本能 = new 嗜血本能(c)
                    {
                        Level = General.GameplayEquilibriumConstant.MaxSkillLevel
                    };
                    c.Skills.Add(嗜血本能);
                }

                if (id == 11)
                {
                    Skill 敏捷之刃 = new 敏捷之刃(c)
                    {
                        Level = 1
                    };
                    c.Skills.Add(敏捷之刃);

                    Skill 平衡强化 = new 平衡强化(c)
                    {
                        Level = General.GameplayEquilibriumConstant.MaxSkillLevel
                    };
                    c.Skills.Add(平衡强化);
                }

                if (id == 12)
                {
                    Skill 弱者猎手 = new 弱者猎手(c)
                    {
                        Level = 1
                    };
                    c.Skills.Add(弱者猎手);

                    Skill 血之狂欢 = new 血之狂欢(c)
                    {
                        Level = General.GameplayEquilibriumConstant.MaxSkillLevel
                    };
                    c.Skills.Add(血之狂欢);
                }

                return NetworkUtility.JsonSerialize(c.GetInfo().Trim());
            }
            return NetworkUtility.JsonSerialize("");
        }

        [HttpPost("post")]
        public string PostName([FromBody] string name)
        {
            return NetworkUtility.JsonSerialize($"Your Name received successfully: {name}.");
        }

        [HttpPost("bind")]
        public string Post([FromBody] BindQQ b)
        {
            return NetworkUtility.JsonSerialize("绑定失败，请稍后再试。");
        }
    }
}
