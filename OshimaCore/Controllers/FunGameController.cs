using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Oshima.Core.Configs;
using Oshima.Core.Models;
using Oshima.Core.Utils;

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
                    builder.AppendLine($"�ܼƲ�������{stats.Plays}");
                    builder.AppendLine($"�ܼƹھ�����{stats.Wins}");
                    builder.AppendLine($"�ܼ�ǰ������{stats.Top3s}");
                    builder.AppendLine($"�ܼưܳ�����{stats.Loses}");
                    builder.AppendLine($"ʤ�ʣ�{stats.Winrates * 100:0.##}%");
                    builder.AppendLine($"ǰ���ʣ�{stats.Top3rates * 100:0.##}%");
                    builder.AppendLine($"�ϴ�������{stats.LastRank} / �������Σ�{stats.AvgRank}");

                    return NetworkUtility.JsonSerialize(builder.ToString());
                }
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
            return NetworkUtility.JsonSerialize("��ʧ�ܣ����Ժ����ԡ�");
        }
    }
}
