using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.Core.Configs;
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
        public List<string> GetTest([FromQuery] bool? isweb = null, [FromQuery] bool? isteam = null, [FromQuery] bool? showall = null)
        {
            bool web = isweb ?? true;
            bool team = isteam ?? false;
            bool all = showall ?? false;
            return FunGameSimulation.StartGame(false, web, team, all);
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
                    builder.AppendLine($"�ܼ�����˺���{stats.TotalDamage:0.##} / ������{stats.AvgDamage:0.##}");
                    builder.AppendLine($"�ܼ���������˺���{stats.TotalPhysicalDamage:0.##} / ������{stats.AvgPhysicalDamage:0.##}");
                    builder.AppendLine($"�ܼ����ħ���˺���{stats.TotalMagicDamage:0.##} / ������{stats.AvgMagicDamage:0.##}");
                    builder.AppendLine($"�ܼ������ʵ�˺���{stats.TotalRealDamage:0.##} / ������{stats.AvgRealDamage:0.##}");
                    builder.AppendLine($"�ܼƳ����˺���{stats.TotalTakenDamage:0.##} / ������{stats.AvgTakenDamage:0.##}");
                    builder.AppendLine($"�ܼƳ��������˺���{stats.TotalTakenPhysicalDamage:0.##} / ������{stats.AvgTakenPhysicalDamage:0.##}");
                    builder.AppendLine($"�ܼƳ���ħ���˺���{stats.TotalTakenMagicDamage:0.##} / ������{stats.AvgTakenMagicDamage:0.##}");
                    builder.AppendLine($"�ܼƳ�����ʵ�˺���{stats.TotalTakenRealDamage:0.##} / ������{stats.AvgTakenRealDamage:0.##}");
                    builder.AppendLine($"�ܼƴ��غ�����{stats.LiveRound} / ������{stats.AvgLiveRound}");
                    builder.AppendLine($"�ܼ��ж��غ�����{stats.ActionTurn} / ������{stats.AvgActionTurn}");
                    builder.AppendLine($"�ܼƴ��ʱ����{stats.LiveTime:0.##} / ������{stats.AvgLiveTime:0.##}");
                    builder.AppendLine($"�ܼ�׬ȡ��Ǯ��{stats.TotalEarnedMoney} / ������{stats.AvgEarnedMoney}");
                    builder.AppendLine($"ÿ�غ��˺���{stats.DamagePerRound:0.##}");
                    builder.AppendLine($"ÿ�ж��غ��˺���{stats.DamagePerTurn:0.##}");
                    builder.AppendLine($"ÿ���˺���{stats.DamagePerSecond:0.##}");
                    builder.AppendLine($"�ܼƻ�ɱ����{stats.Kills}" + (stats.Plays != 0 ? $" / ������{(double)stats.Kills / stats.Plays:0.##}" : ""));
                    builder.AppendLine($"�ܼ���������{stats.Deaths}" + (stats.Plays != 0 ? $" / ������{(double)stats.Deaths / stats.Plays:0.##}" : ""));
                    builder.AppendLine($"�ܼ���������{stats.Assists}" + (stats.Plays != 0 ? $" / ������{(double)stats.Assists / stats.Plays:0.##}" : ""));
                    builder.AppendLine($"�ܼ���ɱ����{stats.FirstKills}" + (stats.Plays != 0 ? $" / ��ɱ�ʣ�{(double)stats.FirstKills / stats.Plays * 100:0.##}%" : ""));
                    builder.AppendLine($"�ܼ���������{stats.FirstDeaths}" + (stats.Plays != 0 ? $" / �����ʣ�{(double)stats.FirstDeaths / stats.Plays * 100:0.##}%" : ""));
                    builder.AppendLine($"�ܼƲ�������{stats.Plays}");
                    builder.AppendLine($"�ܼƹھ�����{stats.Wins}");
                    builder.AppendLine($"�ܼ�ǰ������{stats.Top3s}");
                    builder.AppendLine($"�ܼưܳ�����{stats.Loses}");
                    builder.AppendLine($"MVP������{stats.MVPs}");

                    List<string> names = [.. FunGameSimulation.CharacterStatistics.OrderByDescending(kv => kv.Value.Winrates).Select(kv => kv.Key.GetName())];
                    builder.AppendLine($"ʤ�ʣ�{stats.Winrates * 100:0.##}%��#{names.IndexOf(character.GetName()) + 1}��");
                    builder.AppendLine($"ǰ���ʣ�{stats.Top3rates * 100:0.##}%");

                    names = [.. FunGameSimulation.CharacterStatistics.OrderByDescending(kv => kv.Value.Rating).Select(kv => kv.Key.GetName())];
                    builder.AppendLine($"�����÷֣�{stats.Rating:0.0#}��#{names.IndexOf(character.GetName()) + 1}��");

                    builder.AppendLine($"�ϴ�������{stats.LastRank} / �������Σ�{stats.AvgRank:0.##}");

                    return NetworkUtility.JsonSerialize(builder.ToString());
                }
            }
            return NetworkUtility.JsonSerialize("");
        }

        [HttpGet("teamstats")]
        public string GetTeamStats([FromQuery] int? id = null)
        {
            if (id != null && id > 0 && id <= FunGameSimulation.Characters.Count)
            {
                Character character = FunGameSimulation.Characters[Convert.ToInt32(id) - 1];
                if (FunGameSimulation.TeamCharacterStatistics.TryGetValue(character, out CharacterStatistics? stats) && stats != null)
                {
                    StringBuilder builder = new();

                    builder.AppendLine(character.ToString());
                    builder.AppendLine($"�ܼ�����˺���{stats.TotalDamage:0.##} / ������{stats.AvgDamage:0.##}");
                    builder.AppendLine($"�ܼ���������˺���{stats.TotalPhysicalDamage:0.##} / ������{stats.AvgPhysicalDamage:0.##}");
                    builder.AppendLine($"�ܼ����ħ���˺���{stats.TotalMagicDamage:0.##} / ������{stats.AvgMagicDamage:0.##}");
                    builder.AppendLine($"�ܼ������ʵ�˺���{stats.TotalRealDamage:0.##} / ������{stats.AvgRealDamage:0.##}");
                    builder.AppendLine($"�ܼƳ����˺���{stats.TotalTakenDamage:0.##} / ������{stats.AvgTakenDamage:0.##}");
                    builder.AppendLine($"�ܼƳ��������˺���{stats.TotalTakenPhysicalDamage:0.##} / ������{stats.AvgTakenPhysicalDamage:0.##}");
                    builder.AppendLine($"�ܼƳ���ħ���˺���{stats.TotalTakenMagicDamage:0.##} / ������{stats.AvgTakenMagicDamage:0.##}");
                    builder.AppendLine($"�ܼƳ�����ʵ�˺���{stats.TotalTakenRealDamage:0.##} / ������{stats.AvgTakenRealDamage:0.##}");
                    builder.AppendLine($"�ܼƴ��غ�����{stats.LiveRound} / ������{stats.AvgLiveRound}");
                    builder.AppendLine($"�ܼ��ж��غ�����{stats.ActionTurn} / ������{stats.AvgActionTurn}");
                    builder.AppendLine($"�ܼƴ��ʱ����{stats.LiveTime:0.##} / ������{stats.AvgLiveTime:0.##}");
                    builder.AppendLine($"�ܼ�׬ȡ��Ǯ��{stats.TotalEarnedMoney} / ������{stats.AvgEarnedMoney}");
                    builder.AppendLine($"ÿ�غ��˺���{stats.DamagePerRound:0.##}");
                    builder.AppendLine($"ÿ�ж��غ��˺���{stats.DamagePerTurn:0.##}");
                    builder.AppendLine($"ÿ���˺���{stats.DamagePerSecond:0.##}");
                    builder.AppendLine($"�ܼƻ�ɱ����{stats.Kills}" + (stats.Plays != 0 ? $" / ������{(double)stats.Kills / stats.Plays:0.##}" : ""));
                    builder.AppendLine($"�ܼ���������{stats.Deaths}" + (stats.Plays != 0 ? $" / ������{(double)stats.Deaths / stats.Plays:0.##}" : ""));
                    builder.AppendLine($"��ɱ�����ȣ�{(stats.Deaths == 0 ? stats.Kills : ((double)stats.Kills / stats.Deaths)):0.##}");
                    builder.AppendLine($"�ܼ���������{stats.Assists}" + (stats.Plays != 0 ? $" / ������{(double)stats.Assists / stats.Plays:0.##}" : ""));
                    builder.AppendLine($"�ܼ���ɱ����{stats.FirstKills}" + (stats.Plays != 0 ? $" / ��ɱ�ʣ�{(double)stats.FirstKills / stats.Plays * 100:0.##}%" : ""));
                    builder.AppendLine($"�ܼ���������{stats.FirstDeaths}" + (stats.Plays != 0 ? $" / �����ʣ�{(double)stats.FirstDeaths / stats.Plays * 100:0.##}%" : ""));
                    builder.AppendLine($"�ܼƲ�������{stats.Plays}");
                    builder.AppendLine($"�ܼƹھ�����{stats.Wins}");
                    builder.AppendLine($"�ܼưܳ�����{stats.Loses}");
                    builder.AppendLine($"MVP������{stats.MVPs}");

                    List<string> names = [.. FunGameSimulation.TeamCharacterStatistics.OrderByDescending(kv => kv.Value.Winrates).Select(kv => kv.Key.GetName())];
                    builder.AppendLine($"ʤ�ʣ�{stats.Winrates * 100:0.##}%��#{names.IndexOf(character.GetName()) + 1}��");
                    names = [.. FunGameSimulation.TeamCharacterStatistics.OrderByDescending(kv => kv.Value.Rating).Select(kv => kv.Key.GetName())];
                    builder.AppendLine($"�����÷֣�{stats.Rating:0.0#}��#{names.IndexOf(character.GetName()) + 1}��");

                    return NetworkUtility.JsonSerialize(builder.ToString());
                }
            }
            return NetworkUtility.JsonSerialize("");
        }

        [HttpGet("winraterank")]
        public string GetWinrateRank([FromQuery] bool? isteam = null)
        {
            bool team = isteam ?? false;
            if (team)
            {
                List<string> strings = [];
                IEnumerable<Character> ratings = FunGameSimulation.TeamCharacterStatistics.OrderByDescending(kv => kv.Value.Winrates).Select(kv => kv.Key);
                foreach (Character character in ratings)
                {
                    StringBuilder builder = new();
                    CharacterStatistics stats = FunGameSimulation.TeamCharacterStatistics[character];
                    builder.AppendLine(character.ToString());
                    builder.AppendLine($"�ܼƲ�������{stats.Plays}");
                    builder.AppendLine($"�ܼƹھ�����{stats.Wins}");
                    builder.AppendLine($"ʤ�ʣ�{stats.Winrates * 100:0.##}%");
                    builder.AppendLine($"�����÷֣�{stats.Rating:0.0#}");
                    builder.AppendLine($"MVP������{stats.MVPs}");
                    strings.Add(builder.ToString());
                }
                return NetworkUtility.JsonSerialize(strings);
            }
            else
            {
                List<string> strings = [];
                IEnumerable<Character> ratings = FunGameSimulation.CharacterStatistics.OrderByDescending(kv => kv.Value.Winrates).Select(kv => kv.Key);
                foreach (Character character in ratings)
                {
                    StringBuilder builder = new();
                    CharacterStatistics stats = FunGameSimulation.CharacterStatistics[character];
                    builder.AppendLine(character.ToString());
                    builder.AppendLine($"�ܼƲ�������{stats.Plays}");
                    builder.AppendLine($"�ܼƹھ�����{stats.Wins}");
                    builder.AppendLine($"ʤ�ʣ�{stats.Winrates * 100:0.##}%");
                    builder.AppendLine($"ǰ���ʣ�{stats.Top3rates * 100:0.##}%");
                    builder.AppendLine($"�����÷֣�{stats.Rating:0.0#}");
                    builder.AppendLine($"�ϴ�������{stats.LastRank} / �������Σ�{stats.AvgRank:0.##}");
                    builder.AppendLine($"MVP������{stats.MVPs}");
                    strings.Add(builder.ToString());
                }
                return NetworkUtility.JsonSerialize(strings);
            }
        }

        [HttpGet("ratingrank")]
        public string GetRatingRank([FromQuery] bool? isteam = null)
        {
            bool team = isteam ?? false;
            if (team)
            {
                List<string> strings = [];
                IEnumerable<Character> ratings = FunGameSimulation.TeamCharacterStatistics.OrderByDescending(kv => kv.Value.Rating).Select(kv => kv.Key);
                foreach (Character character in ratings)
                {
                    StringBuilder builder = new();
                    CharacterStatistics stats = FunGameSimulation.TeamCharacterStatistics[character];
                    builder.AppendLine(character.ToString());
                    builder.AppendLine($"�ܼƲ�������{stats.Plays}");
                    builder.AppendLine($"�ܼƹھ�����{stats.Wins}");
                    builder.AppendLine($"ʤ�ʣ�{stats.Winrates * 100:0.##}%");
                    builder.AppendLine($"�����÷֣�{stats.Rating:0.0#}");
                    builder.AppendLine($"MVP������{stats.MVPs}");
                    strings.Add(builder.ToString());
                }
                return NetworkUtility.JsonSerialize(strings);
            }
            else
            {
                List<string> strings = [];
                IEnumerable<Character> ratings = FunGameSimulation.CharacterStatistics.OrderByDescending(kv => kv.Value.Rating).Select(kv => kv.Key);
                foreach (Character character in ratings)
                {
                    StringBuilder builder = new();
                    CharacterStatistics stats = FunGameSimulation.CharacterStatistics[character];
                    builder.AppendLine(character.ToString());
                    builder.AppendLine($"�ܼƲ�������{stats.Plays}");
                    builder.AppendLine($"�ܼƹھ�����{stats.Wins}");
                    builder.AppendLine($"ʤ�ʣ�{stats.Winrates * 100:0.##}%");
                    builder.AppendLine($"ǰ���ʣ�{stats.Top3rates * 100:0.##}%");
                    builder.AppendLine($"�����÷֣�{stats.Rating:0.0#}");
                    builder.AppendLine($"�ϴ�������{stats.LastRank} / �������Σ�{stats.AvgRank:0.##}");
                    builder.AppendLine($"MVP������{stats.MVPs}");
                    strings.Add(builder.ToString());
                }
                return NetworkUtility.JsonSerialize(strings);
            }
        }

        [HttpGet("cjs")]
        public string GetCharacterIntroduce([FromQuery] int? id = null)
        {
            if (id != null && id > 0 && id <= FunGameSimulation.Characters.Count)
            {
                Character c = FunGameSimulation.Characters[Convert.ToInt32(id) - 1].Copy();
                c.Level = General.GameplayEquilibriumConstant.MaxLevel;
                c.NormalAttack.Level = General.GameplayEquilibriumConstant.MaxNormalAttackLevel;

                if (id == 1)
                {
                    Skill META�� = new META��(c)
                    {
                        Level = 1
                    };
                    c.Skills.Add(META��);

                    Skill �������� = new ��������(c)
                    {
                        Level = General.GameplayEquilibriumConstant.MaxMagicLevel
                    };
                    c.Skills.Add(��������);
                }

                if (id == 2)
                {
                    Skill ����֮�� = new ����֮��(c)
                    {
                        Level = 1
                    };
                    c.Skills.Add(����֮��);

                    Skill ���֮�� = new ���֮��(c)
                    {
                        Level = General.GameplayEquilibriumConstant.MaxSkillLevel
                    };
                    c.Skills.Add(���֮��);
                }

                if (id == 3)
                {
                    Skill ħ���� = new ħ����(c)
                    {
                        Level = 1
                    };
                    c.Skills.Add(ħ����);

                    Skill ħ��ӿ�� = new ħ��ӿ��(c)
                    {
                        Level = General.GameplayEquilibriumConstant.MaxSkillLevel
                    };
                    c.Skills.Add(ħ��ӿ��);
                }

                if (id == 4)
                {
                    Skill ���ܷ��� = new ���ܷ���(c)
                    {
                        Level = 1
                    };
                    c.Skills.Add(���ܷ���);

                    Skill ���ص��� = new ���ص���(c)
                    {
                        Level = General.GameplayEquilibriumConstant.MaxSkillLevel
                    };
                    c.Skills.Add(���ص���);
                }

                if (id == 5)
                {
                    Skill �ǻ������� = new �ǻ�������(c)
                    {
                        Level = 1
                    };
                    c.Skills.Add(�ǻ�������);

                    Skill ���֮�� = new ���֮��(c)
                    {
                        Level = General.GameplayEquilibriumConstant.MaxSkillLevel
                    };
                    c.Skills.Add(���֮��);
                }

                if (id == 6)
                {
                    Skill ������� = new �������(c)
                    {
                        Level = 1
                    };
                    c.Skills.Add(�������);

                    Skill ��׼��� = new ��׼���(c)
                    {
                        Level = General.GameplayEquilibriumConstant.MaxSkillLevel
                    };
                    c.Skills.Add(��׼���);
                }

                if (id == 7)
                {
                    Skill ����֮�� = new ����֮��(c)
                    {
                        Level = 1
                    };
                    c.Skills.Add(����֮��);

                    Skill �������� = new ��������(c)
                    {
                        Level = General.GameplayEquilibriumConstant.MaxSkillLevel
                    };
                    c.Skills.Add(��������);
                }

                if (id == 8)
                {
                    Skill �ݽߴ�� = new �ݽߴ��(c)
                    {
                        Level = 1
                    };
                    c.Skills.Add(�ݽߴ��);

                    Skill �������� = new ��������(c)
                    {
                        Level = General.GameplayEquilibriumConstant.MaxSkillLevel
                    };
                    c.Skills.Add(��������);
                }

                if (id == 9)
                {
                    Skill �������� = new ��������(c)
                    {
                        Level = 1
                    };
                    c.Skills.Add(��������);

                    Skill Ѹ��֮�� = new Ѹ��֮��(c)
                    {
                        Level = General.GameplayEquilibriumConstant.MaxSkillLevel
                    };
                    c.Skills.Add(Ѹ��֮��);
                }

                if (id == 10)
                {
                    Skill �ۻ�֮ѹ = new �ۻ�֮ѹ(c)
                    {
                        Level = 1
                    };
                    c.Skills.Add(�ۻ�֮ѹ);

                    Skill ��Ѫ���� = new ��Ѫ����(c)
                    {
                        Level = General.GameplayEquilibriumConstant.MaxSkillLevel
                    };
                    c.Skills.Add(��Ѫ����);
                }

                if (id == 11)
                {
                    Skill ����֮�� = new ����֮��(c)
                    {
                        Level = 1
                    };
                    c.Skills.Add(����֮��);

                    Skill ƽ��ǿ�� = new ƽ��ǿ��(c)
                    {
                        Level = General.GameplayEquilibriumConstant.MaxSkillLevel
                    };
                    c.Skills.Add(ƽ��ǿ��);
                }

                if (id == 12)
                {
                    Skill �������� = new ��������(c)
                    {
                        Level = 1
                    };
                    c.Skills.Add(��������);

                    Skill Ѫ֮�� = new Ѫ֮��(c)
                    {
                        Level = General.GameplayEquilibriumConstant.MaxSkillLevel
                    };
                    c.Skills.Add(Ѫ֮��);
                }

                return NetworkUtility.JsonSerialize(c.GetInfo().Trim());
            }
            return NetworkUtility.JsonSerialize("");
        }

        [HttpGet("cjn")]
        public string GetSkillInfo([FromQuery] long? id = null)
        {
            IEnumerable<Skill> skills = FunGameSimulation.Skills.Union(FunGameSimulation.Magics);
            if (id != null && FunGameSimulation.Characters.Count > 1)
            {
                List<string> msg = [];
                Character c = FunGameSimulation.Characters[1].Copy();
                Skill? s = skills.Where(s => s.Id == id).FirstOrDefault()?.Copy();
                if (s != null)
                {
                    s.Character = c;
                    msg.Add($"����չʾ�����Ի�����ʾ��ɫ��[ {c} ]");
                    msg.Add(c.ToStringWithLevel() + "\r\n" + s.ToString());
                    s.Level++; ;
                    msg.Add(c.ToStringWithLevel() + "\r\n" + s.ToString());
                    c.Level = General.GameplayEquilibriumConstant.MaxLevel;
                    s.Level = s.IsMagic ? General.GameplayEquilibriumConstant.MaxMagicLevel : General.GameplayEquilibriumConstant.MaxSkillLevel;
                    msg.Add(c.ToStringWithLevel() + "\r\n" + s.ToString());
                }

                return NetworkUtility.JsonSerialize(string.Join("\r\n\r\n", msg));
            }
            return NetworkUtility.JsonSerialize("");
        }
        
        [HttpGet("cwp")]
        public string GetItemInfo([FromQuery] long? id = null)
        {
            IEnumerable<Item> items = FunGameSimulation.Items;
            if (id != null)
            {
                List<string> msg = [];
                Item? i = items.Where(i => i.Id == id).FirstOrDefault()?.Copy();
                if (i != null)
                {
                    i.SetLevel(1);
                    msg.Add(i.ToString());
                }

                return NetworkUtility.JsonSerialize(string.Join("\r\n\r\n", msg));
            }
            return NetworkUtility.JsonSerialize("");
        }
        
        [HttpGet("reload")]
        public string Relaod([FromQuery] long? master  = null)
        {
            if (master != null && master == GeneralSettings.Master)
            {
                FunGameSimulation.Reload();
                return NetworkUtility.JsonSerialize("FunGame�����¼��ء�");
            }
            return NetworkUtility.JsonSerialize("�ṩ�Ĳ�������ȷ��");
        }

        [HttpPost("post")]
        public string PostName([FromBody] string name)
        {
            return NetworkUtility.JsonSerialize($"Your Name received successfully: {name}.");
        }

        [HttpPost("bind")]
        public string Post([FromBody] BindQQ b)
        {
            return NetworkUtility.JsonSerialize("��ʧ�ܣ����Ժ����ԡ�");
        }
    }
}
