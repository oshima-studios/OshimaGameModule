using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.Core.Configs;
using Oshima.Core.Models;
using Oshima.Core.Utils;
using Oshima.FunGame.OshimaModules.Characters;

namespace Oshima.Core.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FunGameController(ILogger<FunGameController> logger) : ControllerBase
    {
        private readonly ILogger<FunGameController> _logger = logger;
        private const int drawCardReduce = 2000;
        private const int drawCardReduce_Material = 10;
        private const string noSaved = "�㻹û�д����浵���뷢�͡������浵��������";
        private readonly ItemType[] itemCanUsed = [ItemType.Consumable, ItemType.MagicCard, ItemType.SpecialItem, ItemType.GiftBox, ItemType.Others];

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
            if (id != null && id > 0 && id <= FunGameService.Characters.Count)
            {
                Character character = FunGameService.Characters[Convert.ToInt32(id) - 1];
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

                    List<string> names = [.. FunGameSimulation.CharacterStatistics.OrderByDescending(kv => kv.Value.MVPs).Select(kv => kv.Key.GetName())];
                    builder.AppendLine($"MVP������{stats.MVPs}��#{names.IndexOf(character.GetName()) + 1}��");

                    names = [.. FunGameSimulation.CharacterStatistics.OrderByDescending(kv => kv.Value.Winrates).Select(kv => kv.Key.GetName())];
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
            if (id != null && id > 0 && id <= FunGameService.Characters.Count)
            {
                Character character = FunGameService.Characters[Convert.ToInt32(id) - 1];
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
                    builder.AppendLine($"�ܼ�ʤ������{stats.Wins}");
                    builder.AppendLine($"�ܼưܳ�����{stats.Loses}");

                    List<string> names = [.. FunGameSimulation.TeamCharacterStatistics.OrderByDescending(kv => kv.Value.MVPs).Select(kv => kv.Key.GetName())];
                    builder.AppendLine($"MVP������{stats.MVPs}��#{names.IndexOf(character.GetName()) + 1}��");
                    names = [.. FunGameSimulation.TeamCharacterStatistics.OrderByDescending(kv => kv.Value.Winrates).Select(kv => kv.Key.GetName())];
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

        [HttpGet("characterinfo")]
        public string GetCharacterInfo([FromQuery] int? id = null)
        {
            if (id != null && id > 0 && id <= FunGameService.Characters.Count)
            {
                Character c = FunGameService.Characters[Convert.ToInt32(id) - 1].Copy();
                c.Level = General.GameplayEquilibriumConstant.MaxLevel;
                c.NormalAttack.Level = General.GameplayEquilibriumConstant.MaxNormalAttackLevel;
                FunGameService.AddCharacterSkills(c, 1, General.GameplayEquilibriumConstant.MaxSkillLevel, General.GameplayEquilibriumConstant.MaxSuperSkillLevel);

                return NetworkUtility.JsonSerialize(c.GetInfo().Trim());
            }
            return NetworkUtility.JsonSerialize("");
        }

        [HttpGet("skillinfo")]
        public string GetSkillInfo([FromQuery] long? id = null)
        {
            IEnumerable<Skill> skills = FunGameService.Skills.Union(FunGameService.Magics);
            if (id != null && FunGameService.Characters.Count > 1)
            {
                List<string> msg = [];
                Character c = FunGameService.Characters[1].Copy();
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

        [HttpGet("iteminfo")]
        public string GetItemInfo([FromQuery] long? id = null)
        {
            IEnumerable<Item> items = FunGameService.Equipment;
            if (id != null)
            {
                List<string> msg = [];
                Item? i = items.Where(i => i.Id == id).FirstOrDefault()?.Copy();
                if (i != null)
                {
                    i.SetLevel(1);
                    msg.Add(i.ToString(false, true));
                }

                return NetworkUtility.JsonSerialize(string.Join("\r\n\r\n", msg));
            }
            return NetworkUtility.JsonSerialize("");
        }

        [HttpGet("newmagiccard")]
        public string GenerateMagicCard()
        {
            Item i = FunGameService.GenerateMagicCard();
            return NetworkUtility.JsonSerialize(i.ToString(false, true));
        }

        [HttpGet("newmagiccardpack")]
        public string GenerateMagicCardPack()
        {
            Item? i = FunGameService.GenerateMagicCardPack(3);
            if (i != null)
            {
                return NetworkUtility.JsonSerialize(i.ToString(false, true));
            }
            return NetworkUtility.JsonSerialize("");
        }

        [HttpPost("createsaved")]
        public string CreateSaved([FromQuery] long? qq = null, [FromQuery] string? name = null)
        {
            long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
            string username = name ?? FunGameService.GenerateRandomChineseUserName();

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            if (pc.Count == 0)
            {
                User user = Factory.GetUser(userid, username, DateTime.Now, DateTime.Now, userid + "@qq.com", username);
                user.Inventory.Credits = 5000000;
                user.Inventory.Characters.Add(new CustomCharacter(userid, username));
                pc.Add("user", user);
                pc.SaveConfig();
                return NetworkUtility.JsonSerialize($"�����浵�ɹ�������û����ǡ�{username}����");
            }
            else
            {
                return NetworkUtility.JsonSerialize("���Ѿ��������浵��");
            }
        }
        
        [HttpPost("restoresaved")]
        public string RestoreSaved([FromQuery] long? qq = null)
        {
            long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);
                user.Inventory.Credits = 5000000;
                user.Inventory.Materials = 0;
                user.Inventory.Characters.Clear();
                user.Inventory.Items.Clear();
                user.LastTime = DateTime.Now;
                pc.Add("user", user);
                pc.SaveConfig();
                return NetworkUtility.JsonSerialize($"��Ĵ浵�ѻ�ԭ�ɹ���");
            }
            else
            {
                return NetworkUtility.JsonSerialize(noSaved);
            }
        }

        [HttpPost("rename")]
        public string ReName([FromQuery] long? qq = null)
        {
            long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);

                int reduce = 1500;
                if (user.Inventory.Credits >= reduce)
                {
                    user.Inventory.Credits -= reduce;
                }
                else
                {
                    return NetworkUtility.JsonSerialize($"���{General.GameplayEquilibriumConstant.InGameCurrency}���� {reduce} �أ��޷�������");
                }

                user.Username = FunGameService.GenerateRandomChineseUserName();
                if (user.Inventory.Characters.FirstOrDefault(c => c.Id == user.Id) is Character character)
                {
                    character.Name = user.Username;
                }
                if (user.Inventory.Name.EndsWith("�Ŀ��"))
                {
                    user.Inventory.Name = user.Username + "�Ŀ��";
                }
                user.LastTime = DateTime.Now;
                pc.Add("user", user);
                pc.SaveConfig();
                return NetworkUtility.JsonSerialize($"���� {reduce} {General.GameplayEquilibriumConstant.InGameCurrency}������������ǡ�{user.Username}��");
            }
            else
            {
                return NetworkUtility.JsonSerialize(noSaved);
            }
        }
        
        [HttpPost("randomcustom")]
        public string RandomCustomCharacter([FromQuery] long? qq = null)
        {
            long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);

                if (user.Inventory.Characters.FirstOrDefault(c => c.Id == user.Id) is Character character)
                {
                    int reduce = 20;
                    if (user.Inventory.Materials >= reduce)
                    {
                        user.Inventory.Materials -= reduce;
                    }
                    else
                    {
                        return NetworkUtility.JsonSerialize($"���{General.GameplayEquilibriumConstant.InGameMaterial}���� {reduce} �أ��޷������Խ���ɫ���ԣ�");
                    }

                    PrimaryAttribute oldPA = character.PrimaryAttribute;
                    double oldSTR = character.InitialSTR;
                    double oldAGI = character.InitialAGI;
                    double oldINT = character.InitialINT;
                    double oldSTRG = character.STRGrowth;
                    double oldAGIG = character.AGIGrowth;
                    double oldINTG = character.INTGrowth;
                    Character temp = new CustomCharacter(0, "");
                    character.PrimaryAttribute = temp.PrimaryAttribute;
                    character.InitialSTR = temp.InitialSTR;
                    character.InitialAGI = temp.InitialAGI;
                    character.InitialINT = temp.InitialINT;
                    character.STRGrowth = temp.STRGrowth;
                    character.AGIGrowth = temp.AGIGrowth;
                    character.INTGrowth = temp.INTGrowth;
                    user.LastTime = DateTime.Now;
                    pc.Add("user", user);
                    pc.SaveConfig();
                    return NetworkUtility.JsonSerialize($"���� {reduce} {General.GameplayEquilibriumConstant.InGameMaterial}������Խ���ɫ�Ѹ��ĳ�ʼ���ԣ�\r\n" +
                        $"�������ԣ�{CharacterSet.GetPrimaryAttributeName(oldPA)} => {CharacterSet.GetPrimaryAttributeName(character.PrimaryAttribute)}\r\n" +
                        $"��ʼ������{oldSTR}��+{oldSTRG}/Lv��=> {character.InitialSTR}��+{character.STRGrowth}/Lv��\r\n" +
                        $"��ʼ���ݣ�{oldAGI}��+{oldAGIG}/Lv��=> {character.InitialAGI}��+{character.AGIGrowth}/Lv��\r\n" +
                        $"��ʼ������{oldINT}��+{oldINTG}/Lv��=> {character.InitialINT}��+{character.INTGrowth}/Lv��");
                }
                else
                {
                    return NetworkUtility.JsonSerialize($"���ƺ�û���Խ���ɫ���뷢�͡������Խ���ɫ��������");
                }
            }
            else
            {
                return NetworkUtility.JsonSerialize(noSaved);
            }
        }

        [HttpPost("inventoryinfo")]
        public string GetInventoryInfo([FromQuery] long? qq = null)
        {
            long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);

                return NetworkUtility.JsonSerialize(user.Inventory.ToString(false));
            }
            else
            {
                return NetworkUtility.JsonSerialize(noSaved);
            }
        }

        [HttpPost("inventoryinfo2")]
        public List<string> GetInventoryInfo2([FromQuery] long? qq = null, [FromQuery] int? page = null)
        {
            long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
            int showPage = page ?? 1;
            if (showPage <= 0) showPage = 1;

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            List<string> list = [];
            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);
                list.Add($"���� {user.Inventory.Name} ����");
                list.Add($"{General.GameplayEquilibriumConstant.InGameCurrency}��{user.Inventory.Credits:0.00}");
                list.Add($"{General.GameplayEquilibriumConstant.InGameMaterial}��{user.Inventory.Materials:0.00}");
                List<Character> characters = [.. user.Inventory.Characters];
                List<Item> items = [.. user.Inventory.Items];
                int total = characters.Count + items.Count;
                int maxPage = (int)Math.Ceiling((double)total / 10);
                if (maxPage < 1) maxPage = 1;
                if (showPage <= maxPage)
                {
                    List<object> inventory = [.. characters, .. items];
                    Dictionary<int, object> dict = inventory.Select((obj, index) => new { Index = index + 1, Value = obj }).ToDictionary(k => k.Index, v => v.Value);
                    List<int> seq = [.. FunGameService.GetPage(dict.Keys, showPage, 10)];
                    bool showCharacter = true;
                    bool showItem = true;
                    int characterCount = 0;
                    int itemCount = 0;

                    int prevSequence = dict.Take((showPage - 1) * 10).Count();

                    foreach (int index in seq)
                    {
                        object obj = dict[index];
                        string str = "";
                        if (obj is Character character)
                        {
                            characterCount++;
                            if (showCharacter)
                            {
                                showCharacter = false;
                                list.Add("======= ��ɫ =======");
                            }
                            str = $"{prevSequence + characterCount}. {character.ToStringWithLevelWithOutUser()}";
                        }
                        if (obj is Item item)
                        {
                            itemCount++;
                            if (showItem)
                            {
                                showItem = false;
                                list.Add("======= ��Ʒ =======");
                            }
                            str = $"{index - (characterCount > 0 ? prevSequence + characterCount : characters.Count)}. [{ItemSet.GetQualityTypeName(item.QualityType)}|{ItemSet.GetItemTypeName(item.ItemType)}] {item.Name}\r\n";
                            str += $"{item.ToStringInventory(false).Trim()}";
                        }
                        list.Add(str);
                    }

                    list.Add($"ҳ����{showPage} / {maxPage}");
                }
                else
                {
                    list.Add($"û����ô��ҳ����ǰ��ҳ��Ϊ {maxPage}������������ǵ� {showPage} ҳ��");
                }
            }
            else
            {
                list.Add(noSaved);
            }
            return list;
        }

        [HttpPost("inventoryinfo3")]
        public List<string> GetInventoryInfo3([FromQuery] long? qq = null, [FromQuery] int? page = null, [FromQuery] int? order = null, [FromQuery] int? orderqty = null)
        {
            long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
            int showPage = page ?? 1;
            if (showPage <= 0) showPage = 1;

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            List<string> list = [];
            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);
                list.Add($"���� {user.Inventory.Name} ����");
                list.Add($"{General.GameplayEquilibriumConstant.InGameCurrency}��{user.Inventory.Credits:0.00}");
                list.Add($"{General.GameplayEquilibriumConstant.InGameMaterial}��{user.Inventory.Materials:0.00}");
                List<Item> items = [.. user.Inventory.Items];

                Dictionary<string, List<Item>> itemCategory = [];
                foreach (Item item in items)
                {
                    if (!itemCategory.TryAdd(item.GetIdName(), [item]))
                    {
                        itemCategory[item.GetIdName()].Add(item);
                    }
                }
                if (orderqty != 0)
                {
                    IOrderedEnumerable<KeyValuePair<string, List<Item>>>? orderEnum = null;
                    if (order != 0)
                    {
                        orderEnum = order switch
                        {
                            1 => itemCategory.OrderBy(kv => kv.Value.FirstOrDefault()?.QualityType ?? 0),
                            2 => itemCategory.OrderByDescending(kv => kv.Value.FirstOrDefault()?.QualityType ?? 0),
                            3 => itemCategory.OrderBy(kv => kv.Value.FirstOrDefault()?.ItemType ?? 0),
                            4 => itemCategory.OrderByDescending(kv => kv.Value.FirstOrDefault()?.ItemType ?? 0),
                            _ => itemCategory.OrderBy(kv => 0)
                        };
                    }
                    if (orderEnum != null)
                    {
                        if (orderqty == 1)
                        {
                            orderEnum = orderEnum.ThenBy(kv => kv.Value.Count);
                        }
                        else
                        {
                            orderEnum = orderEnum.ThenByDescending(kv => kv.Value.Count);
                        }
                        itemCategory = orderEnum.ToDictionary();
                    }
                }

                int maxPage = (int)Math.Ceiling((double)itemCategory.Count / 10);
                if (maxPage < 1) maxPage = 1;
                if (showPage <= maxPage)
                {
                    List<string> keys = [.. FunGameService.GetPage(itemCategory.Keys, showPage, 10)];
                    int itemCount = 0;
                    list.Add("======= ��Ʒ =======");
                    foreach (string key in keys)
                    {
                        itemCount++;
                        List<Item> objs = itemCategory[key];
                        Item first = objs[0];
                        string str = $"{itemCount}. [{ItemSet.GetQualityTypeName(first.QualityType)}|{ItemSet.GetItemTypeName(first.ItemType)}] {first.Name}\r\n";
                        str += $"��Ʒ������{first.Description}\r\n";
                        str += $"��Ʒ��ţ�{string.Join("��", objs.Select(i => items.IndexOf(i) + 1))}\r\n";
                        str += $"ӵ��������{objs.Count}��" + (first.IsEquipment ? $"��װ��������{objs.Count(i => i.Character is null)}��" : "") +
                            (itemCanUsed.Contains(first.ItemType) ? $"��ʹ��������{objs.Count(i => i.RemainUseTimes > 0)}��" : "") +
                            $"�ɳ���������{objs.Count(i => i.IsSellable)}���ɽ���������{objs.Count(i => i.IsTradable)}��";
                        list.Add(str);
                    }
                    list.Add($"ҳ����{showPage} / {maxPage}");
                }
                else
                {
                    list.Add($"û����ô��ҳ����ǰ��ҳ��Ϊ {maxPage}������������ǵ� {showPage} ҳ��");
                }
            }
            else
            {
                list.Add(noSaved);
            }
            return list;
        }
        
        [HttpPost("inventoryinfo4")]
        public List<string> GetInventoryInfo4([FromQuery] long? qq = null, [FromQuery] int? page = null, [FromQuery] int? type = null)
        {
            long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
            int showPage = page ?? 1;
            int itemtype = type ?? -1;
            if (showPage <= 0) showPage = 1;

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            List<string> list = [];
            if (pc.Count > 0)
            {
                if (type == -1)
                {
                    return ["û��ָ����Ʒ�����ͣ���ʹ��ͨ�ò�ѯ������"];
                }

                User user = FunGameService.GetUser(pc);
                list.Add($"���� {user.Inventory.Name} ����");
                list.Add($"{General.GameplayEquilibriumConstant.InGameCurrency}��{user.Inventory.Credits:0.00}");
                list.Add($"{General.GameplayEquilibriumConstant.InGameMaterial}��{user.Inventory.Materials:0.00}");
                List<Item> items = [.. user.Inventory.Items];

                Dictionary<string, List<Item>> itemCategory = [];
                foreach (Item item in items)
                {
                    if (!itemCategory.TryAdd(item.GetIdName(), [item]))
                    {
                        itemCategory[item.GetIdName()].Add(item);
                    }
                }

                // ��Ʒ�ʵ���������������
                itemCategory = itemCategory.OrderByDescending(kv => kv.Value.FirstOrDefault()?.QualityType ?? 0).ThenByDescending(kv => kv.Value.Count).ToDictionary();

                // �Ƴ����з�ָ�����͵���Ʒ
                foreach (List<Item> listTemp in itemCategory.Values)
                {
                    if (listTemp.First() is Item item && (int)item.ItemType != itemtype)
                    {
                        itemCategory.Remove(item.GetIdName());
                    }
                }

                int maxPage = (int)Math.Ceiling((double)itemCategory.Count / 10);
                if (maxPage < 1) maxPage = 1;
                if (showPage <= maxPage)
                {
                    List<string> keys = [.. FunGameService.GetPage(itemCategory.Keys, showPage, 10)];
                    int itemCount = 0;
                    list.Add($"======= {ItemSet.GetItemTypeName((ItemType)itemtype).Replace("��Ʒ", "")}��Ʒ =======");
                    foreach (string key in keys)
                    {
                        itemCount++;
                        List<Item> objs = itemCategory[key];
                        Item first = objs[0];
                        string str = $"{itemCount}. [{ItemSet.GetQualityTypeName(first.QualityType)}|{ItemSet.GetItemTypeName(first.ItemType)}] {first.Name}\r\n";
                        str += $"��Ʒ������{first.Description}\r\n";
                        str += $"��Ʒ��ţ�{string.Join("��", objs.Select(i => items.IndexOf(i) + 1))}\r\n";
                        str += $"ӵ��������{objs.Count}��" + (first.IsEquipment ? $"��װ��������{objs.Count(i => i.Character is null)}��" : "") +
                            (itemCanUsed.Contains(first.ItemType) ? $"��ʹ��������{objs.Count(i => i.RemainUseTimes > 0)}��" : "") +
                            $"�ɳ���������{objs.Count(i => i.IsSellable)}���ɽ���������{objs.Count(i => i.IsTradable)}��";
                        list.Add(str);
                    }
                    list.Add($"ҳ����{showPage} / {maxPage}");
                }
                else
                {
                    list.Add($"û����ô��ҳ����ǰ��ҳ��Ϊ {maxPage}������������ǵ� {showPage} ҳ��");
                }
            }
            else
            {
                list.Add(noSaved);
            }
            return list;
        }

        [HttpPost("inventoryinfo5")]
        public List<string> GetInventoryInfo5([FromQuery] long? qq = null, [FromQuery] int? page = null)
        {
            long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
            int showPage = page ?? 1;
            if (showPage <= 0) showPage = 1;

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            List<string> list = [];
            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);
                list.Add($"���� {user.Inventory.Name} ����");
                list.Add($"{General.GameplayEquilibriumConstant.InGameCurrency}��{user.Inventory.Credits:0.00}");
                list.Add($"{General.GameplayEquilibriumConstant.InGameMaterial}��{user.Inventory.Materials:0.00}");
                List<Character> characters = [.. user.Inventory.Characters];
                int total = characters.Count;
                int maxPage = (int)Math.Ceiling((double)total / 6);
                if (maxPage < 1) maxPage = 1;
                if (showPage <= maxPage)
                {
                    List<object> inventory = [.. characters];
                    Dictionary<int, object> dict = inventory.Select((obj, index) => new { Index = index + 1, Value = obj }).ToDictionary(k => k.Index, v => v.Value);
                    List<int> seq = [.. FunGameService.GetPage(dict.Keys, showPage, 6)];
                    bool showCharacter = true;
                    int characterCount = 0;

                    int prevSequence = dict.Take((showPage - 1) * 6).Count();

                    foreach (int index in seq)
                    {
                        object obj = dict[index];
                        string str = "";
                        if (obj is Character character)
                        {
                            characterCount++;
                            if (showCharacter)
                            {
                                showCharacter = false;
                                list.Add("======= ��ɫ =======");
                            }
                            str = $"{prevSequence + characterCount}. {character.ToStringWithLevelWithOutUser()}";
                        }
                        list.Add(str);
                    }

                    list.Add($"ҳ����{showPage} / {maxPage}");
                }
                else
                {
                    list.Add($"û����ô��ҳ����ǰ��ҳ��Ϊ {maxPage}������������ǵ� {showPage} ҳ��");
                }
            }
            else
            {
                list.Add(noSaved);
            }
            return list;
        }

        [HttpPost("newcustomcharacter")]
        public string NewCustomCharacter([FromQuery] long? qq = null)
        {
            long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);
                if (user.Inventory.Characters.Any(c => c.Id == user.Id))
                {
                    return NetworkUtility.JsonSerialize($"���Ѿ�ӵ��һ���Խ���ɫ��{user.Username}�����޷��ٴ�����");
                }
                else
                {
                    user.Inventory.Characters.Add(new CustomCharacter(userid, user.Username));
                    user.LastTime = DateTime.Now;
                    pc.Add("user", user);
                    pc.SaveConfig();
                    return NetworkUtility.JsonSerialize($"��ϲ��ɹ�������һ���Խ���ɫ��{user.Username}������鿴��Ľ�ɫ��棡");
                }
            }
            else
            {
                return NetworkUtility.JsonSerialize(noSaved);
            }
        }
        
        [HttpPost("drawcard")]
        public string DrawCard([FromQuery] long? qq = null)
        {
            long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);

                int reduce = drawCardReduce;
                if (user.Inventory.Credits >= reduce)
                {
                    user.Inventory.Credits -= reduce;
                }
                else
                {
                    return NetworkUtility.JsonSerialize($"���{General.GameplayEquilibriumConstant.InGameCurrency}���� {reduce} �أ��޷��鿨��");
                }

                double dice = Random.Shared.NextDouble();
                if (dice > 0.8)
                {
                    string msg = FunGameService.GetDrawCardResult(reduce, user);
                    user.LastTime = DateTime.Now;
                    pc.Add("user", user);
                    pc.SaveConfig();
                    return NetworkUtility.JsonSerialize(msg);
                }
                else
                {
                    pc.SaveConfig();
                    return NetworkUtility.JsonSerialize($"���� {reduce} {General.GameplayEquilibriumConstant.InGameCurrency}����ʲôҲû���С���");
                }
            }
            else
            {
                return NetworkUtility.JsonSerialize(noSaved);
            }
        }

        [HttpPost("drawcards")]
        public List<string> DrawCards([FromQuery] long? qq = null)
        {
            long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);

                int reduce = drawCardReduce * 10;
                if (user.Inventory.Credits >= reduce)
                {
                    user.Inventory.Credits -= reduce;
                }
                else
                {
                    return [$"���{General.GameplayEquilibriumConstant.InGameCurrency}���� {reduce} �أ��޷�ʮ���鿨��"];
                }

                List<string> result = [$"���� {reduce} {General.GameplayEquilibriumConstant.InGameCurrency}����ϲ��鵽�ˣ�"];
                int count = 0;
                for (int i = 0; i < 10; i++)
                {
                    double dice = Random.Shared.NextDouble();
                    if (dice > 0.8)
                    {
                        count++;
                        result.Add(FunGameService.GetDrawCardResult(reduce, user, true, count));
                    }
                }
                if (result.Count == 1)
                {
                    result[0] = $"���� {reduce} {General.GameplayEquilibriumConstant.InGameCurrency}����ʲôҲû���С���";
                }
                user.LastTime = DateTime.Now;
                pc.Add("user", user);
                pc.SaveConfig();
                return result;
            }
            else
            {
                return [noSaved];
            }
        }

        [HttpPost("drawcardm")]
        public string DrawCard_Material([FromQuery] long? qq = null)
        {
            long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);

                int reduce = drawCardReduce_Material;
                if (user.Inventory.Materials >= reduce)
                {
                    user.Inventory.Materials -= reduce;
                }
                else
                {
                    return NetworkUtility.JsonSerialize($"���{General.GameplayEquilibriumConstant.InGameMaterial}���� {reduce} �أ��޷��鿨��");
                }

                double dice = Random.Shared.NextDouble();
                if (dice > 0.8)
                {
                    string msg = FunGameService.GetDrawCardResult(reduce, user);
                    user.LastTime = DateTime.Now;
                    pc.Add("user", user);
                    pc.SaveConfig();
                    return NetworkUtility.JsonSerialize(msg);
                }
                else
                {
                    pc.SaveConfig();
                    return NetworkUtility.JsonSerialize($"���� {reduce} {General.GameplayEquilibriumConstant.InGameMaterial}����ʲôҲû���С���");
                }
            }
            else
            {
                return NetworkUtility.JsonSerialize(noSaved);
            }
        }

        [HttpPost("drawcardsm")]
        public List<string> DrawCards_Material([FromQuery] long? qq = null)
        {
            long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);

                int reduce = drawCardReduce_Material * 10;
                if (user.Inventory.Materials >= reduce)
                {
                    user.Inventory.Materials -= reduce;
                }
                else
                {
                    return [$"���{General.GameplayEquilibriumConstant.InGameMaterial}���� {reduce} �أ��޷�ʮ���鿨��"];
                }

                List<string> result = [$"���� {reduce} {General.GameplayEquilibriumConstant.InGameMaterial}����ϲ��鵽�ˣ�"];
                int count = 0;
                for (int i = 0; i < 10; i++)
                {
                    double dice = Random.Shared.NextDouble();
                    if (dice > 0.8)
                    {
                        count++;
                        result.Add(FunGameService.GetDrawCardResult(reduce, user, true, count));
                    }
                }
                if (result.Count == 1)
                {
                    result[0] = $"���� {reduce} {General.GameplayEquilibriumConstant.InGameMaterial}����ʲôҲû���С���";
                }
                user.LastTime = DateTime.Now;
                pc.Add("user", user);
                pc.SaveConfig();
                return result;
            }
            else
            {
                return [noSaved];
            }
        }

        [HttpPost("exchangecredits")]
        public string ExchangeCredits([FromQuery] long? qq = null, [FromQuery] double? materials = null)
        {
            long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
            double useMaterials = materials ?? 0;

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);

                int reduce = (int)useMaterials >= 10 ? (int)useMaterials : 10;
                reduce -= reduce % 10;
                if (reduce >= 10 && user.Inventory.Materials >= reduce)
                {
                    int reward = reduce / 10 * 2000;
                    user.Inventory.Credits += reward;
                    user.Inventory.Materials -= reduce;
                    user.LastTime = DateTime.Now;
                    pc.Add("user", user);
                    pc.SaveConfig();
                    return NetworkUtility.JsonSerialize($"�һ��ɹ����������� {reduce} {General.GameplayEquilibriumConstant.InGameMaterial}�������� {reward} {General.GameplayEquilibriumConstant.InGameCurrency}��");
                }
                else
                {
                    return NetworkUtility.JsonSerialize($"���{General.GameplayEquilibriumConstant.InGameMaterial}���� {reduce}��������� 10 {General.GameplayEquilibriumConstant.InGameMaterial}�һ� 2000 {General.GameplayEquilibriumConstant.InGameCurrency}��");
                }
            }
            else
            {
                return NetworkUtility.JsonSerialize(noSaved);
            }
        }

        [HttpPost("showcharacterinfo")]
        public string GetCharacterInfoFromInventory([FromQuery] long? qq = null, [FromQuery] int? seq = null)
        {
            try
            {
                long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
                int cIndex = seq ?? 0;

                PluginConfig pc = new("saved", userid.ToString());
                pc.LoadConfig();

                if (pc.Count > 0)
                {
                    User user = FunGameService.GetUser(pc);

                    if (cIndex > 0 && cIndex <= user.Inventory.Characters.Count)
                    {
                        Character character = user.Inventory.Characters.ToList()[cIndex - 1];
                        return NetworkUtility.JsonSerialize($"�������������Ϊ {cIndex} �Ľ�ɫ��ϸ��Ϣ��\r\n{character.GetInfo().Trim()}");
                    }
                    else
                    {
                        return NetworkUtility.JsonSerialize($"û���ҵ������������Ӧ�Ľ�ɫ��");
                    }
                }
                else
                {
                    return NetworkUtility.JsonSerialize(noSaved);
                }
            }
            catch (Exception e)
            {
                return NetworkUtility.JsonSerialize(e.ToString());
            }
        }

        [HttpPost("showiteminfo")]
        public string GetItemInfoFromInventory([FromQuery] long? qq = null, [FromQuery] int? seq = null)
        {
            try
            {
                long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
                int itemIndex = seq ?? 0;

                PluginConfig pc = new("saved", userid.ToString());
                pc.LoadConfig();

                if (pc.Count > 0)
                {
                    User user = FunGameService.GetUser(pc);

                    if (itemIndex > 0 && itemIndex <= user.Inventory.Items.Count)
                    {
                        Item item = user.Inventory.Items.ToList()[itemIndex - 1];
                        return NetworkUtility.JsonSerialize($"�������������Ϊ {itemIndex} ����Ʒ��ϸ��Ϣ��\r\n{item.ToStringInventory(true).Trim()}");
                    }
                    else
                    {
                        return NetworkUtility.JsonSerialize($"û���ҵ������������Ӧ����Ʒ��");
                    }
                }
                else
                {
                    return NetworkUtility.JsonSerialize(noSaved);
                }
            }
            catch (Exception e)
            {
                return NetworkUtility.JsonSerialize(e.ToString());
            }
        }
        
        [HttpPost("equipitem")]
        public string EquipItem([FromQuery] long? qq = null, [FromQuery] int? c = null, [FromQuery] int? i = null)
        {
            try
            {
                long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
                int characterIndex = c ?? 0;
                int itemIndex = i ?? 0;

                PluginConfig pc = new("saved", userid.ToString());
                pc.LoadConfig();

                if (pc.Count > 0)
                {
                    User user = FunGameService.GetUser(pc);

                    Character? character = null;
                    Item? item = null;
                    if (characterIndex > 0 && characterIndex <= user.Inventory.Characters.Count)
                    {
                        character = user.Inventory.Characters.ToList()[characterIndex - 1];
                    }
                    else
                    {
                        return NetworkUtility.JsonSerialize($"û���ҵ������������Ӧ�Ľ�ɫ��");
                    }
                    if (itemIndex > 0 && itemIndex <= user.Inventory.Items.Count)
                    {
                        item = user.Inventory.Items.ToList()[itemIndex - 1];
                        if ((int)item.ItemType < (int)ItemType.MagicCardPack || (int)item.ItemType > (int)ItemType.Accessory)
                        {
                            return NetworkUtility.JsonSerialize($"�����Ʒ�޷���װ����");
                        }
                        else if (item.Character != null)
                        {
                            return NetworkUtility.JsonSerialize($"�����Ʒ�޷���װ����[ {item.Character.ToStringWithLevelWithOutUser()} ] ��װ������Ʒ��");
                        }
                    }
                    else
                    {
                        return NetworkUtility.JsonSerialize($"û���ҵ������������Ӧ����Ʒ��");
                    }
                    if (character != null && item != null && character.Equip(item))
                    {
                        user.LastTime = DateTime.Now;
                        pc.Add("user", user);
                        pc.SaveConfig();
                        return NetworkUtility.JsonSerialize($"װ��{ItemSet.GetQualityTypeName(item.QualityType)}{ItemSet.GetItemTypeName(item.ItemType)}��{item.Name}���ɹ���" +
                            $"��{ItemSet.GetEquipSlotTypeName(item.EquipSlotType)}��λ��\r\n��Ʒ������{item.Description}");
                    }
                    else
                    {
                        return NetworkUtility.JsonSerialize($"װ��ʧ�ܣ������ǽ�ɫ����Ʒ�����ڻ�������ԭ��");
                    }
                }
                else
                {
                    return NetworkUtility.JsonSerialize(noSaved);
                }
            }
            catch (Exception e)
            {
                return NetworkUtility.JsonSerialize(e.ToString());
            }
        }
        
        [HttpPost("unequipitem")]
        public string UnEquipItem([FromQuery] long? qq = null, [FromQuery] int? c = null, [FromQuery] int? i = null)
        {
            try
            {
                long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
                int characterIndex = c ?? 0;
                EquipSlotType type = (EquipSlotType)(i ?? 0);

                PluginConfig pc = new("saved", userid.ToString());
                pc.LoadConfig();

                if (pc.Count > 0)
                {
                    User user = FunGameService.GetUser(pc);

                    Character? character = null;
                    if (characterIndex > 0 && characterIndex <= user.Inventory.Characters.Count)
                    {
                        character = user.Inventory.Characters.ToList()[characterIndex - 1];
                        Item? item = character.UnEquip(type);
                        if (item != null && user.Inventory.Items.Where(i => i.Guid == item.Guid).FirstOrDefault() is Item itemInventory)
                        {
                            itemInventory.EquipSlotType = EquipSlotType.None;
                            user.LastTime = DateTime.Now;
                            pc.Add("user", user);
                            pc.SaveConfig();
                            return NetworkUtility.JsonSerialize($"ȡ��װ��{ItemSet.GetQualityTypeName(item.QualityType)}{ItemSet.GetItemTypeName(item.ItemType)}��{item.Name}���ɹ�����{ItemSet.GetEquipSlotTypeName(type)}��λ��");
                        }
                        else return NetworkUtility.JsonSerialize($"ȡ��װ��ʧ�ܣ���ɫ��û��װ��{ItemSet.GetEquipSlotTypeName(type)}�����߿���в����ڴ���Ʒ��");
                    }
                    else
                    {
                        return NetworkUtility.JsonSerialize($"û���ҵ������������Ӧ�Ľ�ɫ��");
                    }
                }
                else
                {
                    return NetworkUtility.JsonSerialize(noSaved);
                }
            }
            catch (Exception e)
            {
                return NetworkUtility.JsonSerialize(e.ToString());
            }
        }

        [HttpGet("reload")]
        public string Relaod([FromQuery] long? master = null)
        {
            if (master != null && master == GeneralSettings.Master)
            {
                FunGameService.Reload();
                FunGameSimulation.InitFunGame();
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
