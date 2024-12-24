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
using Oshima.FunGame.OshimaModules.Items;

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
            return FunGameActionQueue.StartSimulationGame(false, web, team, all);
        }

        [HttpGet("stats")]
        public string GetStats([FromQuery] int? id = null)
        {
            if (id != null && id > 0 && id <= FunGameService.Characters.Count)
            {
                Character character = FunGameService.Characters[Convert.ToInt32(id) - 1];
                if (FunGameActionQueue.CharacterStatistics.TryGetValue(character, out CharacterStatistics? stats) && stats != null)
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

                    List<string> names = [.. FunGameActionQueue.CharacterStatistics.OrderByDescending(kv => kv.Value.MVPs).Select(kv => kv.Key.GetName())];
                    builder.AppendLine($"MVP������{stats.MVPs}��#{names.IndexOf(character.GetName()) + 1}��");

                    names = [.. FunGameActionQueue.CharacterStatistics.OrderByDescending(kv => kv.Value.Winrates).Select(kv => kv.Key.GetName())];
                    builder.AppendLine($"ʤ�ʣ�{stats.Winrates * 100:0.##}%��#{names.IndexOf(character.GetName()) + 1}��");
                    builder.AppendLine($"ǰ���ʣ�{stats.Top3rates * 100:0.##}%");

                    names = [.. FunGameActionQueue.CharacterStatistics.OrderByDescending(kv => kv.Value.Rating).Select(kv => kv.Key.GetName())];
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
                if (FunGameActionQueue.TeamCharacterStatistics.TryGetValue(character, out CharacterStatistics? stats) && stats != null)
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

                    List<string> names = [.. FunGameActionQueue.TeamCharacterStatistics.OrderByDescending(kv => kv.Value.MVPs).Select(kv => kv.Key.GetName())];
                    builder.AppendLine($"MVP������{stats.MVPs}��#{names.IndexOf(character.GetName()) + 1}��");
                    names = [.. FunGameActionQueue.TeamCharacterStatistics.OrderByDescending(kv => kv.Value.Winrates).Select(kv => kv.Key.GetName())];
                    builder.AppendLine($"ʤ�ʣ�{stats.Winrates * 100:0.##}%��#{names.IndexOf(character.GetName()) + 1}��");
                    names = [.. FunGameActionQueue.TeamCharacterStatistics.OrderByDescending(kv => kv.Value.Rating).Select(kv => kv.Key.GetName())];
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
                IEnumerable<Character> ratings = FunGameActionQueue.TeamCharacterStatistics.OrderByDescending(kv => kv.Value.Winrates).Select(kv => kv.Key);
                foreach (Character character in ratings)
                {
                    StringBuilder builder = new();
                    CharacterStatistics stats = FunGameActionQueue.TeamCharacterStatistics[character];
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
                IEnumerable<Character> ratings = FunGameActionQueue.CharacterStatistics.OrderByDescending(kv => kv.Value.Winrates).Select(kv => kv.Key);
                foreach (Character character in ratings)
                {
                    StringBuilder builder = new();
                    CharacterStatistics stats = FunGameActionQueue.CharacterStatistics[character];
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
                IEnumerable<Character> ratings = FunGameActionQueue.TeamCharacterStatistics.OrderByDescending(kv => kv.Value.Rating).Select(kv => kv.Key);
                foreach (Character character in ratings)
                {
                    StringBuilder builder = new();
                    CharacterStatistics stats = FunGameActionQueue.TeamCharacterStatistics[character];
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
                IEnumerable<Character> ratings = FunGameActionQueue.CharacterStatistics.OrderByDescending(kv => kv.Value.Rating).Select(kv => kv.Key);
                foreach (Character character in ratings)
                {
                    StringBuilder builder = new();
                    CharacterStatistics stats = FunGameActionQueue.CharacterStatistics[character];
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
                FunGameService.UserIdAndUsername[userid] = username;
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
                user.Inventory.Characters.Add(new CustomCharacter(userid, user.Username));
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
                FunGameService.UserIdAndUsername[user.Id] = user.Username;
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
        public string RandomCustomCharacter([FromQuery] long? qq = null, [FromQuery] bool? confirm = null)
        {
            long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
            bool isConfirm = confirm ?? false;

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();
            
            EntityModuleConfig<Character> emc = new("randomcustom", userid.ToString());
            emc.LoadConfig();

            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);
                if (user.Inventory.Characters.FirstOrDefault(c => c.Id == user.Id) is Character character)
                {
                    PrimaryAttribute oldPA = character.PrimaryAttribute;
                    double oldSTR = character.InitialSTR;
                    double oldAGI = character.InitialAGI;
                    double oldINT = character.InitialINT;
                    double oldSTRG = character.STRGrowth;
                    double oldAGIG = character.AGIGrowth;
                    double oldINTG = character.INTGrowth;
                    Character? newCustom = emc.Count > 0 ? emc.Get("newCustom") : null;

                    if (isConfirm)
                    {
                        if (newCustom != null)
                        {
                            character.PrimaryAttribute = newCustom.PrimaryAttribute;
                            character.InitialSTR = newCustom.InitialSTR;
                            character.InitialAGI = newCustom.InitialAGI;
                            character.InitialINT = newCustom.InitialINT;
                            character.STRGrowth = newCustom.STRGrowth;
                            character.AGIGrowth = newCustom.AGIGrowth;
                            character.INTGrowth = newCustom.INTGrowth;
                            user.LastTime = DateTime.Now;
                            pc.Add("user", user);
                            pc.SaveConfig();
                            emc.Clear();
                            emc.SaveConfig();
                            return NetworkUtility.JsonSerialize($"���������������ȷ�ϣ��µ��Խ���ɫ�������£�\r\n" +
                                $"�������ԣ�{CharacterSet.GetPrimaryAttributeName(oldPA)} => {CharacterSet.GetPrimaryAttributeName(character.PrimaryAttribute)}\r\n" +
                                $"��ʼ������{oldSTR}��+{oldSTRG}/Lv��=> {character.InitialSTR}��+{character.STRGrowth}/Lv��\r\n" +
                                $"��ʼ���ݣ�{oldAGI}��+{oldAGIG}/Lv��=> {character.InitialAGI}��+{character.AGIGrowth}/Lv��\r\n" +
                                $"��ʼ������{oldINT}��+{oldINTG}/Lv��=> {character.InitialINT}��+{character.INTGrowth}/Lv��");
                        }
                        else
                        {
                            return NetworkUtility.JsonSerialize($"�㻹û�л�ȡ����������Ԥ����");
                        }
                    }
                    else
                    {
                        if (newCustom is null)
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
                            newCustom = new CustomCharacter(user.Id, "");
                            user.LastTime = DateTime.Now;
                            pc.Add("user", user);
                            pc.SaveConfig();
                            emc.Add("newCustom", newCustom);
                            emc.SaveConfig();
                            return NetworkUtility.JsonSerialize($"���� {reduce} {General.GameplayEquilibriumConstant.InGameMaterial}����ȡ����������Ԥ�����£�\r\n" +
                                $"�������ԣ�{CharacterSet.GetPrimaryAttributeName(oldPA)} => {CharacterSet.GetPrimaryAttributeName(newCustom.PrimaryAttribute)}\r\n" +
                                $"��ʼ������{oldSTR}��+{oldSTRG}/Lv��=> {newCustom.InitialSTR}��+{newCustom.STRGrowth}/Lv��\r\n" +
                                $"��ʼ���ݣ�{oldAGI}��+{oldAGIG}/Lv��=> {newCustom.InitialAGI}��+{newCustom.AGIGrowth}/Lv��\r\n" +
                                $"��ʼ������{oldINT}��+{oldINTG}/Lv��=> {newCustom.InitialINT}��+{newCustom.INTGrowth}/Lv��\r\n" +
                                $"�뷢�͡�ȷ�Ͻ�ɫ���桿��ȷ�ϸ��£����߷��͡�ȡ����ɫ���桿��ȡ��������");
                        }
                        else if (newCustom.Id == user.Id)
                        {
                            return NetworkUtility.JsonSerialize($"���Ѿ���һ����ȷ�ϵ������������£�\r\n" +
                                $"�������ԣ�{CharacterSet.GetPrimaryAttributeName(oldPA)} => {CharacterSet.GetPrimaryAttributeName(newCustom.PrimaryAttribute)}\r\n" +
                                $"��ʼ������{oldSTR}��+{oldSTRG}/Lv��=> {newCustom.InitialSTR}��+{newCustom.STRGrowth}/Lv��\r\n" +
                                $"��ʼ���ݣ�{oldAGI}��+{oldAGIG}/Lv��=> {newCustom.InitialAGI}��+{newCustom.AGIGrowth}/Lv��\r\n" +
                                $"��ʼ������{oldINT}��+{oldINTG}/Lv��=> {newCustom.InitialINT}��+{newCustom.INTGrowth}/Lv��\r\n"+
                                $"�뷢�͡�ȷ�Ͻ�ɫ���桿��ȷ�ϸ��£����߷��͡�ȡ����ɫ���桿��ȡ��������");
                        }
                        else
                        {
                            return NetworkUtility.JsonSerialize($"�����Խ���ɫ����ʧ�ܣ�");
                        }
                    }
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

        [HttpPost("cancelrandomcustom")]
        public string CancelRandomCustomCharacter([FromQuery] long? qq = null)
        {
            long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            if (pc.Count > 0)
            {
                EntityModuleConfig<Character> emc = new("randomcustom", userid.ToString());
                emc.LoadConfig();
                if (emc.Count > 0)
                {
                    emc.Clear();
                    emc.SaveConfig();
                    return NetworkUtility.JsonSerialize($"��ȡ����ɫ���档");
                }
                else
                {
                    return NetworkUtility.JsonSerialize($"��Ŀǰû�д�ȷ�ϵĽ�ɫ���档");
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
                        string itemsIndex = string.Join("��", objs.Select(i => items.IndexOf(i) + 1));
                        if (objs.Count > 10)
                        {
                            itemsIndex = string.Join("��", objs.Take(10).Select(i => items.IndexOf(i) + 1)) + "��...";
                        }
                        str += $"��Ʒ��ţ�{itemsIndex}\r\n";
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
                    list.Add($"======= {ItemSet.GetItemTypeName((ItemType)itemtype)} =======");
                    foreach (string key in keys)
                    {
                        itemCount++;
                        List<Item> objs = itemCategory[key];
                        Item first = objs[0];
                        string str = $"{itemCount}. [{ItemSet.GetQualityTypeName(first.QualityType)}|{ItemSet.GetItemTypeName(first.ItemType)}] {first.Name}\r\n";
                        str += $"��Ʒ������{first.Description}\r\n";
                        string itemsIndex = string.Join("��", objs.Select(i => items.IndexOf(i) + 1));
                        if (objs.Count > 10)
                        {
                            itemsIndex = string.Join("��", objs.Take(10).Select(i => items.IndexOf(i) + 1)) + "��...";
                        }
                        str += $"��Ʒ��ţ�{itemsIndex}\r\n";
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
        public string GetCharacterInfoFromInventory([FromQuery] long? qq = null, [FromQuery] int? seq = null, [FromQuery] bool? simple = null)
        {
            try
            {
                long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
                int cIndex = seq ?? 0;
                bool isSimple = simple ?? false;

                PluginConfig pc = new("saved", userid.ToString());
                pc.LoadConfig();

                if (pc.Count > 0)
                {
                    User user = FunGameService.GetUser(pc);

                    if (cIndex > 0 && cIndex <= user.Inventory.Characters.Count)
                    {
                        Character character = user.Inventory.Characters.ToList()[cIndex - 1];
                        if (isSimple)
                        {
                            return NetworkUtility.JsonSerialize($"�������������Ϊ {cIndex} �Ľ�ɫ������Ϣ��\r\n{character.GetSimpleInfo(showEXP: true).Trim()}");
                        }
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

        [HttpPost("fightcustom")]
        public List<string> FightCustom([FromQuery] long? qq = null, [FromQuery] long? eqq = null, [FromQuery] bool? all = null)
        {
            try
            {
                long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
                long enemyid = eqq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
                bool showAllRound = all ?? false;

                PluginConfig pc = new("saved", userid.ToString());
                pc.LoadConfig();
                
                PluginConfig pc2 = new("saved", enemyid.ToString());
                pc2.LoadConfig();

                User? user1 = null, user2 = null;

                if (pc.Count > 0)
                {
                    user1 = FunGameService.GetUser(pc);
                    user1.LastTime = DateTime.Now;
                    pc.Add("user", user1);
                    pc.SaveConfig();
                }
                else
                {
                    return [noSaved];
                }

                if (pc2.Count > 0)
                {
                    user2 = FunGameService.GetUser(pc2);
                    user2.LastTime = DateTime.Now;
                    pc2.Add("user", user2);
                    pc2.SaveConfig();
                }
                else
                {
                    return [$"�Է�ò�ƻ�û�д����浵�أ�"];
                }

                if (user1 != null && user2 != null)
                {
                    return FunGameActionQueue.StartGame([user1.Inventory.MainCharacter, user2.Inventory.MainCharacter], false, false, false, false, false, showAllRound);
                }
                else
                {
                    return [$"��������ʧ�ܣ�"];
                }
            }
            catch (Exception e)
            {
                return [e.ToString()];
            }
        }
        
        [HttpPost("fightcustom2")]
        public List<string> FightCustom2([FromQuery] long? qq = null, [FromQuery] string? name = null, [FromQuery] bool? all = null)
        {
            try
            {
                if (name != null)
                {
                    long enemyid = FunGameService.UserIdAndUsername.Where(kv => kv.Value == name).Select(kv => kv.Key).FirstOrDefault();
                    if (enemyid == 0)
                    {
                        return [$"�Ҳ��������ƶ�Ӧ����ң�"];
                    }
                    return FightCustom(qq, enemyid, all);
                }
                return [$"��������ʧ�ܣ�"];
            }
            catch (Exception e)
            {
                return [e.ToString()];
            }
        }

        [HttpPost("useitem")]
        public string UseItem([FromQuery] long? qq = null, [FromQuery] int? id = null, [FromBody] int[]? characters = null)
        {
            try
            {
                long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
                int itemIndex = id ?? 0;
                List<int> charactersIndex = characters?.ToList() ?? [];

                PluginConfig pc = new("saved", userid.ToString());
                pc.LoadConfig();

                if (pc.Count > 0)
                {
                    User user = FunGameService.GetUser(pc);

                    Character? character = null;
                    Item? item = null;
                    if (itemIndex > 0 && itemIndex <= user.Inventory.Items.Count)
                    {
                        item = user.Inventory.Items.ToList()[itemIndex - 1];
                        if (itemCanUsed.Contains(item.ItemType))
                        {
                            if (item.RemainUseTimes <= 0)
                            {
                                return NetworkUtility.JsonSerialize("����Ʒʣ��ʹ�ô���Ϊ0���޷�ʹ�ã�");
                            }

                            List<Character> targets = [];
                            foreach (int characterIndex in charactersIndex)
                            {
                                if (characterIndex > 0 && characterIndex <= user.Inventory.Characters.Count)
                                {
                                    character = user.Inventory.Characters.ToList()[characterIndex - 1];
                                    targets.Add(character);
                                }
                            }
                            
                            if (FunGameService.UseItem(item, user, [.. targets], out string msg))
                            {
                                user.LastTime = DateTime.Now;
                                pc.Add("user", user);
                                pc.SaveConfig();
                            }
                            return NetworkUtility.JsonSerialize(msg);
                        }
                        else
                        {
                            return NetworkUtility.JsonSerialize($"�����Ʒ�޷�ʹ�ã�");
                        }
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
        
        [HttpPost("useitem2")]
        public string UseItem2([FromQuery] long? qq = null, [FromQuery] string? name = null, [FromQuery] int? count = null, [FromBody] int[]? characters = null)
        {
            try
            {
                long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
                string itemName = name ?? "";
                int useCount = count ?? 0;
                List<int> charactersIndex = characters?.ToList() ?? [];

                PluginConfig pc = new("saved", userid.ToString());
                pc.LoadConfig();

                if (pc.Count > 0)
                {
                    User user = FunGameService.GetUser(pc);

                    IEnumerable<Item> items = user.Inventory.Items.Where(i => i.Name == name && i.Character is null && i.ItemType != ItemType.MagicCard);
                    if (!items.Any())
                    {
                        return NetworkUtility.JsonSerialize($"����в���������Ϊ��{name}������Ʒ�������ħ���������á�ʹ��ħ������ָ�");
                    }

                    if (items.Count() >= useCount)
                    {
                        items = items.TakeLast(useCount);
                        List<string> msgs = [];
                        int successCount = 0;

                        List<Character> targets = [];
                        Character? character = null;
                        foreach (int characterIndex in charactersIndex)
                        {
                            if (characterIndex > 0 && characterIndex <= user.Inventory.Characters.Count)
                            {
                                character = user.Inventory.Characters.ToList()[characterIndex - 1];
                                targets.Add(character);
                            }
                            else
                            {
                                msgs.Add($"����в��������Ϊ {characterIndex} �Ľ�ɫ��");
                            }
                        }

                        foreach (Item item in items)
                        {
                            if (itemCanUsed.Contains(item.ItemType))
                            {
                                if (item.RemainUseTimes <= 0)
                                {
                                    msgs.Add("����Ʒʣ��ʹ�ô���Ϊ0���޷�ʹ�ã�");
                                }

                                if (FunGameService.UseItem(item, user, [.. targets], out string msg))
                                {
                                    successCount++;
                                }
                                msgs.Add(msg);
                            }
                            else
                            {
                                msgs.Add($"�����Ʒ�޷�ʹ�ã�");
                            }
                        }
                        if (successCount > 0)
                        {
                            user.LastTime = DateTime.Now;
                            pc.Add("user", user);
                            pc.SaveConfig();
                        }
                        return NetworkUtility.JsonSerialize($"ʹ����ϣ�ʹ�� {useCount} ����Ʒ���ɹ� {successCount} ����\r\n" + string.Join("\r\n", msgs.Count > 30 ? msgs.Take(30) : msgs));
                    }
                    else
                    {
                        return NetworkUtility.JsonSerialize("����Ʒ�Ŀ�ʹ������С������Ҫʹ�õ�������");
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

        [HttpPost("useitem3")]
        public string UseItem3([FromQuery] long? qq = null, [FromQuery] int? id = null, [FromQuery] int? id2 = null, [FromQuery] bool? c = null)
        {
            try
            {
                long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
                int itemIndex = id ?? 0;
                int itemToIndex = id2 ?? 0;
                bool isCharacter = c ?? false;

                PluginConfig pc = new("saved", userid.ToString());
                pc.LoadConfig();

                if (pc.Count > 0)
                {
                    User user = FunGameService.GetUser(pc);

                    Item? item = null;
                    if (itemIndex > 0 && itemIndex <= user.Inventory.Items.Count)
                    {
                        item = user.Inventory.Items.ToList()[itemIndex - 1];
                        if (item.ItemType == ItemType.MagicCard)
                        {
                            if (item.RemainUseTimes <= 0)
                            {
                                return NetworkUtility.JsonSerialize("����Ʒʣ��ʹ�ô���Ϊ0���޷�ʹ�ã�");
                            }

                            string msg = "";
                            Item? itemTo = null;
                            if (isCharacter)
                            {
                                if (itemToIndex > 0 && itemToIndex <= user.Inventory.Characters.Count)
                                {
                                    Character character = user.Inventory.Characters.ToList()[itemToIndex - 1];
                                    if (character.EquipSlot.MagicCardPack != null)
                                    {
                                        itemTo = user.Inventory.Items.FirstOrDefault(i => i.Guid == character.EquipSlot.MagicCardPack.Guid);
                                        if (itemTo != null)
                                        {
                                            msg = FunGameService.UseMagicCard(user, item, itemTo);
                                        }
                                        else
                                        {
                                            return NetworkUtility.JsonSerialize($"�����û���ҵ��˽�ɫ��Ӧ��ħ��������");
                                        }
                                    }
                                    else
                                    {
                                        return NetworkUtility.JsonSerialize($"�����ɫû��װ��ħ���������޷�����ʹ��ħ������");
                                    }
                                }
                                else
                                {
                                    return NetworkUtility.JsonSerialize($"û���ҵ������������Ӧ�Ľ�ɫ��");
                                }
                            }
                            else
                            {
                                if (itemToIndex > 0 && itemToIndex <= user.Inventory.Items.Count)
                                {
                                    itemTo = user.Inventory.Items.ToList()[itemToIndex - 1];
                                    if (itemTo != null && itemTo.ItemType == ItemType.MagicCardPack)
                                    {
                                        msg = FunGameService.UseMagicCard(user, item, itemTo);
                                    }
                                    else
                                    {
                                        return NetworkUtility.JsonSerialize($"��Ŀ��������Ӧ����Ʒ����ħ��������");
                                    }
                                }
                                else
                                {
                                    return NetworkUtility.JsonSerialize($"û���ҵ���Ŀ��������Ӧ����Ʒ��");
                                }
                            }

                            user.LastTime = DateTime.Now;
                            pc.Add("user", user);
                            pc.SaveConfig();
                            return NetworkUtility.JsonSerialize(msg);
                        }
                        else
                        {
                            return NetworkUtility.JsonSerialize($"�����Ʒ����ħ������");
                        }
                    }
                    else
                    {
                        return NetworkUtility.JsonSerialize($"û���ҵ���Ŀ��������Ӧ����Ʒ��");
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

        [HttpPost("characterlevelup")]
        public string CharacterLevelUp([FromQuery] long? qq = null, [FromQuery] int? c = null, [FromQuery] int? count = null)
        {
            try
            {
                long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
                int characterIndex = c ?? 0;
                int upCount = count ?? 0;

                PluginConfig pc = new("saved", userid.ToString());
                pc.LoadConfig();

                if (pc.Count > 0)
                {
                    User user = FunGameService.GetUser(pc);

                    Character? character = null;
                    if (characterIndex > 0 && characterIndex <= user.Inventory.Characters.Count)
                    {
                        character = user.Inventory.Characters.ToList()[characterIndex - 1];
                    }
                    else
                    {
                        return NetworkUtility.JsonSerialize($"û���ҵ������������Ӧ�Ľ�ɫ��");
                    }

                    if (character.Level == General.GameplayEquilibriumConstant.MaxLevel)
                    {
                        return NetworkUtility.JsonSerialize($"�ý�ɫ�ȼ�������������������");
                    }

                    int originalLevel = character.Level;

                    character.OnLevelUp(upCount);

                    string msg = $"������ɣ���ɫ [ {character} ] ������ {character.Level - originalLevel} ������ǰ�ȼ���{character.Level} ����";

                    if (character.Level != General.GameplayEquilibriumConstant.MaxLevel && General.GameplayEquilibriumConstant.EXPUpperLimit.TryGetValue(character.Level, out double need))
                    {
                        if (character.EXP < need)
                        {
                            msg += $"\r\n��ɫ [ {character} ] ���� {need - character.EXP} �㾭��ֵ���ܼ���������";
                        }
                        else
                        {
                            msg += $"\r\n��ɫ [ {character} ] Ŀǰͻ�ƽ��ȣ�{character.LevelBreak + 1}/{General.GameplayEquilibriumConstant.LevelBreakList.Count}����Ҫ���С���ɫͻ�ơ����ܼ���������";
                        }
                    }
                    else if (character.Level == General.GameplayEquilibriumConstant.MaxLevel)
                    {
                        msg += $"\r\n�ý�ɫ����������������ϲ��";
                    }

                    user.LastTime = DateTime.Now;
                    pc.Add("user", user);
                    pc.SaveConfig();
                    return NetworkUtility.JsonSerialize(msg);
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

        [HttpPost("getlevelbreakneedy")]
        public string GetLevelBreakNeedy([FromQuery] long? qq = null, [FromQuery] int? id = null)
        {
            long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
            int characterIndex = id ?? 0;

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);
                Character? character;
                if (characterIndex > 0 && characterIndex <= user.Inventory.Characters.Count)
                {
                    character = user.Inventory.Characters.ToList()[characterIndex - 1];
                }
                else
                {
                    return NetworkUtility.JsonSerialize($"û���ҵ������������Ӧ�Ľ�ɫ��");
                }

                if (character.LevelBreak + 1 == General.GameplayEquilibriumConstant.LevelBreakList.Count)
                {
                    return NetworkUtility.JsonSerialize($"�ý�ɫ�����ȫ����ͻ�ƽ׶Σ�������ͻ�ƣ�");
                }

                return NetworkUtility.JsonSerialize($"��ɫ [ {character} ] Ŀǰͻ�ƽ��ȣ�{character.LevelBreak + 1}/{General.GameplayEquilibriumConstant.LevelBreakList.Count}" +
                    $"\r\n�ý�ɫ��һ���ȼ�ͻ�ƽ׶��� {General.GameplayEquilibriumConstant.LevelBreakList.ToArray()[character.LevelBreak + 1]} ����������ϣ�\r\n" + FunGameService.GetLevelBreakNeedy(character.LevelBreak + 1));
            }
            else
            {
                return NetworkUtility.JsonSerialize(noSaved);
            }
        }

        [HttpPost("characterlevelbreak")]
        public string CharacterLevelBreak([FromQuery] long? qq = null, [FromQuery] int? c = null)
        {
            try
            {
                long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
                int characterIndex = c ?? 0;

                PluginConfig pc = new("saved", userid.ToString());
                pc.LoadConfig();

                if (pc.Count > 0)
                {
                    User user = FunGameService.GetUser(pc);

                    Character? character = null;
                    if (characterIndex > 0 && characterIndex <= user.Inventory.Characters.Count)
                    {
                        character = user.Inventory.Characters.ToList()[characterIndex - 1];
                    }
                    else
                    {
                        return NetworkUtility.JsonSerialize($"û���ҵ������������Ӧ�Ľ�ɫ��");
                    }

                    if (character.LevelBreak + 1 == General.GameplayEquilibriumConstant.LevelBreakList.Count)
                    {
                        return NetworkUtility.JsonSerialize($"�ý�ɫ�����ȫ����ͻ�ƽ׶Σ�������ͻ�ƣ�");
                    }

                    int originalBreak = character.LevelBreak;

                    if (FunGameService.LevelBreakNeedyList.TryGetValue(originalBreak + 1, out Dictionary<string, int>? needy) && needy != null && needy.Count > 0)
                    {
                        foreach (string key in needy.Keys)
                        {
                            int needCount = needy[key];
                            if (key == General.GameplayEquilibriumConstant.InGameMaterial)
                            {
                                if (user.Inventory.Materials >= needCount)
                                {
                                    user.Inventory.Materials -= needCount;
                                }
                                else
                                {
                                    return NetworkUtility.JsonSerialize($"���{General.GameplayEquilibriumConstant.InGameMaterial}���� {needCount} �أ�������ͻ��������");
                                }
                            }
                            else
                            {
                                if (needCount > 0)
                                {
                                    IEnumerable<Item> items = user.Inventory.Items.Where(i => i.Name == key);
                                    if (items.Count() >= needCount)
                                    {
                                        items = items.TakeLast(needCount);
                                        foreach (Item item in items)
                                        {
                                            user.Inventory.Items.Remove(item);
                                        }
                                    }
                                    else
                                    {
                                        return NetworkUtility.JsonSerialize($"�����Ʒ��{key}���������� {needCount} �أ�������ͻ��������");
                                    }
                                }
                            }
                        }
                    }

                    character.OnLevelBreak();

                    if (originalBreak == character.LevelBreak)
                    {
                        return NetworkUtility.JsonSerialize($"ͻ��ʧ�ܣ���ɫ [ {character} ] Ŀǰͻ�ƽ��ȣ�{character.LevelBreak + 1}/{General.GameplayEquilibriumConstant.LevelBreakList.Count}��" +
                            $"\r\n�ý�ɫ��һ���ȼ�ͻ�ƽ׶��� {General.GameplayEquilibriumConstant.LevelBreakList.ToArray()[character.LevelBreak + 1]} ����������ϣ�\r\n" + FunGameService.GetLevelBreakNeedy(character.LevelBreak + 1));
                    }
                    else
                    {
                        user.LastTime = DateTime.Now;
                        pc.Add("user", user);
                        pc.SaveConfig();
                        return NetworkUtility.JsonSerialize($"ͻ�Ƴɹ�����ɫ [ {character} ] Ŀǰͻ�ƽ��ȣ�{character.LevelBreak + 1}/{General.GameplayEquilibriumConstant.LevelBreakList.Count}��" +
                            $"{(character.LevelBreak + 1 == General.GameplayEquilibriumConstant.LevelBreakList.Count ?
                            "\r\n�ý�ɫ�����ȫ����ͻ�ƽ׶Σ���ϲ��" :
                            $"\r\n�ý�ɫ��һ���ȼ�ͻ�ƽ׶��� {General.GameplayEquilibriumConstant.LevelBreakList.ToArray()[character.LevelBreak + 1]} ����������ϣ�\r\n" + FunGameService.GetLevelBreakNeedy(character.LevelBreak + 1))}");
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

        [HttpPost("createitem")]
        public string CreateItem([FromQuery] long? qq = null, [FromQuery] string? name = null, [FromQuery] int? count = null, [FromQuery] long? target = null)
        {
            long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
            string itemName = name ?? "";
            int itemCount = count ?? 0;
            long targetid = target ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);

                string msg = "";
                if (user.IsAdmin)
                {
                    PluginConfig pc2 = new("saved", targetid.ToString());
                    pc2.LoadConfig();
                    if (pc2.Count > 0)
                    {
                        User user2 = FunGameService.GetUser(pc2);
                        if (itemName == General.GameplayEquilibriumConstant.InGameCurrency)
                        {
                            user2.Inventory.Credits += itemCount;
                            msg = $"��Ϊ [ {user2} ] ���� {itemCount} {General.GameplayEquilibriumConstant.InGameCurrency}";
                        }
                        else if (itemName == General.GameplayEquilibriumConstant.InGameMaterial)
                        {
                            user2.Inventory.Materials += itemCount;
                            msg = $"��Ϊ [ {user2} ] ���� {itemCount} {General.GameplayEquilibriumConstant.InGameMaterial}";
                        }
                        else if (itemName.Contains("ħ������"))
                        {
                            foreach (string type in ItemSet.QualityTypeNameArray)
                            {
                                if (itemName == $"{type}ħ������")
                                {
                                    int success = 0;
                                    for (int i = 0; i < itemCount; i++)
                                    {
                                        Item? item = FunGameService.GenerateMagicCardPack(3, ItemSet.GetQualityTypeFromName(type));
                                        if (item != null)
                                        {
                                            item.User = user2;
                                            user2.Inventory.Items.Add(item);
                                            success++;
                                        }
                                    }
                                    msg = $"��Ϊ [ {user2} ] �ɹ����� {success} ��{type}ħ������";
                                    break;
                                }
                            }
                        }
                        else if (itemName.Contains("ħ����"))
                        {
                            foreach (string type in ItemSet.QualityTypeNameArray)
                            {
                                if (itemName == $"{type}ħ����")
                                {
                                    for (int i = 0; i < itemCount; i++)
                                    {
                                        Item item = FunGameService.GenerateMagicCard(ItemSet.GetQualityTypeFromName(type));
                                        item.User = user2;
                                        user2.Inventory.Items.Add(item);
                                    }
                                    msg = $"��Ϊ [ {user2} ] ���� {itemCount} ��{type}ħ����";
                                    break;
                                }
                            }
                        }
                        else if (FunGameService.AllItems.FirstOrDefault(i => i.Name == itemName) is Item item)
                        {
                            for (int i = 0; i < itemCount; i++)
                            {
                                Item newItem = item.Copy();
                                newItem.User = user2;
                                user2.Inventory.Items.Add(newItem);
                            }
                            msg = $"��Ϊ [ {user2} ] ���� {itemCount} �� [{ItemSet.GetQualityTypeName(item.QualityType)}|{ItemSet.GetItemTypeName(item.ItemType)}] {item.Name}";
                        }
                        else
                        {
                            return NetworkUtility.JsonSerialize($"����Ʒ�����ڣ�");
                        }
                        pc2.Add("user", user2);
                        pc2.SaveConfig();
                    }
                    else
                    {
                        return NetworkUtility.JsonSerialize($"Ŀ�� UID �����ڣ�");
                    }
                }
                else
                {
                    return NetworkUtility.JsonSerialize($"��û��Ȩ��ʹ�ô�ָ�");
                }

                return NetworkUtility.JsonSerialize(msg);
            }
            else
            {
                return NetworkUtility.JsonSerialize(noSaved);
            }
        }

        [HttpPost("decomposeitem")]
        public string DecomposeItem([FromQuery] long? qq = null, [FromBody] int[]? items = null)
        {
            try
            {
                long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
                int[] ids = items ?? [];

                PluginConfig pc = new("saved", userid.ToString());
                pc.LoadConfig();

                if (pc.Count > 0)
                {
                    User user = FunGameService.GetUser(pc);

                    List<string> msgs = [];
                    int successCount = 0;
                    double totalGained = 0;
                    Dictionary<int, Item> dict = user.Inventory.Items.Select((item, index) => new { item, index })
                        .Where(x => ids.Contains(x.index) && x.item.Character is null)
                        .ToDictionary(x => x.index, x => x.item);

                    foreach (int id in dict.Keys)
                    {
                        Item item = dict[id];

                        if (user.Inventory.Items.Remove(item))
                        {
                            double gained = item.QualityType switch
                            {
                                QualityType.Gold => 80,
                                QualityType.Red => 55,
                                QualityType.Orange => 35,
                                QualityType.Purple => 20,
                                QualityType.Blue => 10,
                                QualityType.Green => 4,
                                _ => 1
                            };
                            totalGained += gained;
                            successCount++;
                        }
                    }

                    if (successCount > 0)
                    {
                        user.Inventory.Materials += totalGained;
                        user.LastTime = DateTime.Now;
                        pc.Add("user", user);
                        pc.SaveConfig();
                    }
                    return NetworkUtility.JsonSerialize($"�ֽ���ϣ��ֽ� {ids.Length} �����ɹ� {successCount} �����õ��� {totalGained} {General.GameplayEquilibriumConstant.InGameMaterial}��");
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

        [HttpPost("decomposeitem2")]
        public string DecomposeItem2([FromQuery] long? qq = null, [FromQuery] string? name = null, [FromQuery] int? count = null)
        {
            try
            {
                long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
                string itemName = name ?? "";
                int useCount = count ?? 0;

                PluginConfig pc = new("saved", userid.ToString());
                pc.LoadConfig();

                if (pc.Count > 0)
                {
                    User user = FunGameService.GetUser(pc);

                    IEnumerable<Item> items = user.Inventory.Items.Where(i => i.Name == name && i.Character is null);
                    if (!items.Any())
                    {
                        return NetworkUtility.JsonSerialize($"����в���������Ϊ��{name}������Ʒ��");
                    }

                    if (items.Count() >= useCount)
                    {
                        items = items.TakeLast(useCount);
                        List<string> msgs = [];
                        int successCount = 0;
                        double totalGained = 0;

                        foreach (Item item in items)
                        {
                            if (user.Inventory.Items.Remove(item))
                            {
                                double gained = item.QualityType switch
                                {
                                    QualityType.Gold => 80,
                                    QualityType.Red => 55,
                                    QualityType.Orange => 35,
                                    QualityType.Purple => 20,
                                    QualityType.Blue => 10,
                                    QualityType.Green => 4,
                                    _ => 1
                                };
                                totalGained += gained;
                                successCount++;
                            }
                        }
                        if (successCount > 0)
                        {
                            user.Inventory.Materials += totalGained;
                            user.LastTime = DateTime.Now;
                            pc.Add("user", user);
                            pc.SaveConfig();
                        }
                        return NetworkUtility.JsonSerialize($"�ֽ���ϣ��ֽ� {useCount} ����Ʒ���ɹ� {successCount} �����õ��� {totalGained} {General.GameplayEquilibriumConstant.InGameMaterial}��");
                    }
                    else
                    {
                        return NetworkUtility.JsonSerialize("����Ʒ�Ŀɷֽ�����С������Ҫ�ֽ��������");
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
        
        [HttpPost("decomposeitem3")]
        public string DecomposeItem3([FromQuery] long? qq = null, [FromQuery] int? q = null)
        {
            try
            {
                long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
                int qType = q ?? 0;

                if (qType < 0 || qType > (int)QualityType.Gold)
                {
                    return NetworkUtility.JsonSerialize($"Ʒ������������");
                }

                PluginConfig pc = new("saved", userid.ToString());
                pc.LoadConfig();

                if (pc.Count > 0)
                {
                    User user = FunGameService.GetUser(pc);

                    string qualityName = ItemSet.GetQualityTypeName((QualityType)qType);
                    IEnumerable<Item> items = user.Inventory.Items.Where(i => (int)i.QualityType == qType && i.Character is null);
                    if (!items.Any())
                    {
                        return NetworkUtility.JsonSerialize($"�����{qualityName}��Ʒ����Ϊ�㣡");
                    }

                    List<string> msgs = [];
                    int successCount = 0;
                    double gained = items.First().QualityType switch
                    {
                        QualityType.Gold => 80,
                        QualityType.Red => 55,
                        QualityType.Orange => 35,
                        QualityType.Purple => 20,
                        QualityType.Blue => 10,
                        QualityType.Green => 4,
                        _ => 1
                    };

                    foreach (Item item in items)
                    {
                        if (user.Inventory.Items.Remove(item))
                        {
                            successCount++;
                        }
                    }

                    double totalGained = 0;
                    if (successCount > 0)
                    {
                        totalGained = successCount * gained;
                        user.Inventory.Materials += totalGained;
                        user.LastTime = DateTime.Now;
                        pc.Add("user", user);
                        pc.SaveConfig();
                    }
                    return NetworkUtility.JsonSerialize($"�ֽ���ϣ��ɹ��ֽ� {successCount} ��{qualityName}��Ʒ���õ��� {totalGained} {General.GameplayEquilibriumConstant.InGameMaterial}��");
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

        [HttpPost("conflatemagiccardpack")]
        public string ConflateMagicCardPack([FromQuery] long? qq = null, [FromBody] int[]? items = null)
        {
            try
            {
                long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
                List<int> itemsIndex = items?.ToList() ?? [];

                PluginConfig pc = new("saved", userid.ToString());
                pc.LoadConfig();

                if (pc.Count > 0)
                {
                    User user = FunGameService.GetUser(pc);

                    Item? item = null;
                    List<Item> mfks = [];
                    foreach (int itemIndex in itemsIndex)
                    {
                        if (itemIndex > 0 && itemIndex <= user.Inventory.Items.Count)
                        {
                            item = user.Inventory.Items.ToList()[itemIndex - 1];
                            if (item.ItemType == ItemType.MagicCard && item.RemainUseTimes > 0)
                            {
                                mfks.Add(item);
                            }
                            else
                            {
                                return NetworkUtility.JsonSerialize($"����Ʒ����ħ��������ʹ�ô���Ϊ0��{itemIndex}. {item.Name}");
                            }
                        }
                        else
                        {
                            return NetworkUtility.JsonSerialize($"û���ҵ������������Ӧ����Ʒ��{itemIndex}");
                        }
                    }
                    if (mfks.Count >= 3)
                    {
                        item = FunGameService.ConflateMagicCardPack([mfks[0], mfks[1], mfks[2]]);
                        if (item != null)
                        {
                            item.User = user;
                            FunGameService.SetSellAndTradeTime(item);
                            user.Inventory.Items.Add(item);
                            user.Inventory.Items.Remove(mfks[0]);
                            user.Inventory.Items.Remove(mfks[1]);
                            user.Inventory.Items.Remove(mfks[2]);
                            user.LastTime = DateTime.Now;
                            pc.Add("user", user);
                            pc.SaveConfig();
                            return NetworkUtility.JsonSerialize($"�ϳ�ħ�������ɹ������ħ��������\r\n{item.ToStringInventory(true)}");
                        }
                        else
                        {
                            return NetworkUtility.JsonSerialize($"�ϳ�ħ������ʧ�ܣ�");
                        }
                    }
                    else
                    {
                        return NetworkUtility.JsonSerialize($"ѡ�õ�ħ�������� 3 �ţ�������ѡ��");
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

        [HttpPost("setmain")]
        public string SetMain([FromQuery] long? qq = null, [FromQuery] int? c = null)
        {
            try
            {
                long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
                int characterIndex = c ?? 0;

                PluginConfig pc = new("saved", userid.ToString());
                pc.LoadConfig();

                if (pc.Count > 0)
                {
                    User user = FunGameService.GetUser(pc);

                    Character? character = null;
                    if (characterIndex > 0 && characterIndex <= user.Inventory.Characters.Count)
                    {
                        character = user.Inventory.Characters.ToList()[characterIndex - 1];
                    }
                    else
                    {
                        return NetworkUtility.JsonSerialize($"û���ҵ������������Ӧ�Ľ�ɫ��");
                    }

                    user.Inventory.MainCharacter = character;
                    user.LastTime = DateTime.Now;
                    pc.Add("user", user);
                    pc.SaveConfig();
                    return NetworkUtility.JsonSerialize($"������ս��ɫ�ɹ���{character}");
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
        
        [HttpPost("starttraining")]
        public string StartTraining([FromQuery] long? qq = null, [FromQuery] int? c = null)
        {
            try
            {
                long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
                int characterIndex = c ?? 0;

                PluginConfig pc = new("saved", userid.ToString());
                pc.LoadConfig();

                if (pc.Count > 0)
                {
                    User user = FunGameService.GetUser(pc);

                    Character? character = null;
                    if (characterIndex > 0 && characterIndex <= user.Inventory.Characters.Count)
                    {
                        character = user.Inventory.Characters.ToList()[characterIndex - 1];
                    }
                    else
                    {
                        return NetworkUtility.JsonSerialize($"û���ҵ������������Ӧ�Ľ�ɫ��");
                    }

                    if (user.Inventory.Training.Count > 0)
                    {
                        return NetworkUtility.JsonSerialize($"���Ѿ��н�ɫ�������У���ʹ�á��������㡿ָ���������ȡ������{user.Inventory.Training.First()}��");
                    }

                    user.Inventory.Training[character.Id] = DateTime.Now;
                    user.LastTime = DateTime.Now;
                    pc.Add("user", user);
                    pc.SaveConfig();
                    return NetworkUtility.JsonSerialize($"��ɫ [{character}] ��ʼ���������һ��ʱ�����С��������㡿��ʱ��Խ������Խ��ʢ������ʱ��� 1440 ���ӣ�24Сʱ������ʱ�����κ����棬�뼰ʱ��ȡ������");
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
        
        [HttpPost("stoptraining")]
        public string StopTraining([FromQuery] long? qq = null)
        {
            try
            {
                long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));

                PluginConfig pc = new("saved", userid.ToString());
                pc.LoadConfig();

                if (pc.Count > 0)
                {
                    User user = FunGameService.GetUser(pc);

                    if (user.Inventory.Training.Count == 0)
                    {
                        return NetworkUtility.JsonSerialize($"��Ŀǰû�н�ɫ�������У���ʹ�á���������+��ɫ��š�ָ�����������");
                    }

                    long cid = user.Inventory.Training.Keys.First();
                    DateTime time = user.Inventory.Training[cid];
                    DateTime now = DateTime.Now;
                    Character? character = user.Inventory.Characters.FirstOrDefault(c => c.Id == cid);
                    if (character != null)
                    {
                        user.Inventory.Training.Remove(cid);

                        TimeSpan diff = now - time;
                        string msg = FunGameService.GetTrainingInfo(diff, false, out int totalExperience, out int smallBookCount, out int mediumBookCount, out int largeBookCount);

                        if (totalExperience > 0)
                        {
                            character.EXP += totalExperience;
                        }

                        for (int i = 0; i < smallBookCount; i++)
                        {
                            Item item = new С������(user);
                            user.Inventory.Items.Add(item);
                        }

                        for (int i = 0; i < mediumBookCount; i++)
                        {
                            Item item = new �о�����(user);
                            user.Inventory.Items.Add(item);
                        }

                        for (int i = 0; i < largeBookCount; i++)
                        {
                            Item item = new ������(user);
                            user.Inventory.Items.Add(item);
                        }

                        user.LastTime = DateTime.Now;
                        pc.Add("user", user);
                        pc.SaveConfig();
                        return NetworkUtility.JsonSerialize($"��ɫ [ {character} ] ����������{msg}");
                    }
                    else
                    {
                        return NetworkUtility.JsonSerialize($"��Ŀǰû�н�ɫ�������У�Ҳ�����ǿ����Ϣ��ȡ�쳣�����Ժ����ԡ�");
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
        
        [HttpPost("gettraininginfo")]
        public string GetTrainingInfo([FromQuery] long? qq = null)
        {
            try
            {
                long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));

                PluginConfig pc = new("saved", userid.ToString());
                pc.LoadConfig();

                if (pc.Count > 0)
                {
                    User user = FunGameService.GetUser(pc);

                    if (user.Inventory.Training.Count == 0)
                    {
                        return NetworkUtility.JsonSerialize($"��Ŀǰû�н�ɫ�������У���ʹ�á���������+��ɫ��š�ָ�����������");
                    }

                    long cid = user.Inventory.Training.Keys.First();
                    DateTime time = user.Inventory.Training[cid];
                    DateTime now = DateTime.Now;
                    Character? character = user.Inventory.Characters.FirstOrDefault(c => c.Id == cid);
                    if (character != null)
                    {
                        TimeSpan diff = now - time;
                        string msg = FunGameService.GetTrainingInfo(diff, true, out int totalExperience, out int smallBookCount, out int mediumBookCount, out int largeBookCount);

                        return NetworkUtility.JsonSerialize($"��ɫ [ {character} ] ���������У�{msg}\r\nȷ������������롾�������㡿��ȡ������");
                    }
                    else
                    {
                        return NetworkUtility.JsonSerialize($"��Ŀǰû�н�ɫ�������У�Ҳ�����ǿ����Ϣ��ȡ�쳣�����Ժ����ԡ�");
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

        [HttpPost("getskilllevelupneedy")]
        public string GetSkillLevelUpNeedy([FromQuery] long? qq = null, [FromQuery] int? c = null, [FromQuery] string? s = null)
        {
            long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
            int characterIndex = c ?? 0;
            string skillName = s ?? "";

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);
                Character? character;
                if (characterIndex > 0 && characterIndex <= user.Inventory.Characters.Count)
                {
                    character = user.Inventory.Characters.ToList()[characterIndex - 1];
                }
                else
                {
                    return NetworkUtility.JsonSerialize($"û���ҵ������������Ӧ�Ľ�ɫ��");
                }

                if (character.Skills.FirstOrDefault(s => s.Name == skillName) is Skill skill)
                {
                    if (skill.SkillType == SkillType.Skill || skill.SkillType == SkillType.SuperSkill)
                    {
                        if (skill.Level + 1 == General.GameplayEquilibriumConstant.MaxSkillLevel)
                        {
                            return NetworkUtility.JsonSerialize($"�˼��ܡ�{skill.Name}���Ѿ�����������");
                        }

                        return NetworkUtility.JsonSerialize($"��ɫ [ {character} ] �ġ�{skill.Name}�����ܵȼ���{skill.Level}/{General.GameplayEquilibriumConstant.MaxSkillLevel}" +
                            $"\r\n��һ�������������ϣ�\r\n" + FunGameService.GetSkillLevelUpNeedy(skill.Level + 1));
                    }
                    return NetworkUtility.JsonSerialize($"�˼����޷�������");
                }
                else
                {
                    return NetworkUtility.JsonSerialize($"�˽�ɫû�С�{skillName}�����ܣ�");
                }
            }
            else
            {
                return NetworkUtility.JsonSerialize(noSaved);
            }
        }

        [HttpPost("skilllevelup")]
        public string SkillLevelUp([FromQuery] long? qq = null, [FromQuery] int? c = null, [FromQuery] string? s = null)
        {
            try
            {
                long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
                int characterIndex = c ?? 0;
                string skillName = s ?? "";

                PluginConfig pc = new("saved", userid.ToString());
                pc.LoadConfig();

                if (pc.Count > 0)
                {
                    User user = FunGameService.GetUser(pc);

                    Character? character = null;
                    if (characterIndex > 0 && characterIndex <= user.Inventory.Characters.Count)
                    {
                        character = user.Inventory.Characters.ToList()[characterIndex - 1];
                    }
                    else
                    {
                        return NetworkUtility.JsonSerialize($"û���ҵ������������Ӧ�Ľ�ɫ��");
                    }

                    if (character.Skills.FirstOrDefault(s => s.Name == skillName) is Skill skill)
                    {
                        string isStudy = skill.Level == 0 ? "ѧϰ" : "����";

                        if (skill.SkillType == SkillType.Skill || skill.SkillType == SkillType.SuperSkill)
                        {
                            if (skill.Level == General.GameplayEquilibriumConstant.MaxSkillLevel)
                            {
                                return NetworkUtility.JsonSerialize($"�˼��ܡ�{skill.Name}���Ѿ�����������");
                            }

                            if (FunGameService.SkillLevelUpList.TryGetValue(skill.Level + 1, out Dictionary<string, int>? needy) && needy != null && needy.Count > 0)
                            {
                                foreach (string key in needy.Keys)
                                {
                                    int needCount = needy[key];
                                    if (key == "��ɫ�ȼ�")
                                    {
                                        if (character.Level < needCount)
                                        {
                                            return NetworkUtility.JsonSerialize($"��ɫ [ {character} ] �ȼ����� {needCount} �����޷�{isStudy}�˼��ܣ�");
                                        }
                                    }
                                    else if (key == General.GameplayEquilibriumConstant.InGameCurrency)
                                    {
                                        if (user.Inventory.Credits >= needCount)
                                        {
                                            user.Inventory.Credits -= needCount;
                                        }
                                        else
                                        {
                                            return NetworkUtility.JsonSerialize($"���{General.GameplayEquilibriumConstant.InGameCurrency}���� {needCount} �أ�������{isStudy}������");
                                        }
                                    }
                                    else if (key == General.GameplayEquilibriumConstant.InGameMaterial)
                                    {
                                        if (user.Inventory.Materials >= needCount)
                                        {
                                            user.Inventory.Materials -= needCount;
                                        }
                                        else
                                        {
                                            return NetworkUtility.JsonSerialize($"���{General.GameplayEquilibriumConstant.InGameMaterial}���� {needCount} �أ�������{isStudy}������");
                                        }
                                    }
                                    else
                                    {
                                        if (needCount > 0)
                                        {
                                            IEnumerable<Item> items = user.Inventory.Items.Where(i => i.Name == key);
                                            if (items.Count() >= needCount)
                                            {
                                                items = items.TakeLast(needCount);
                                                foreach (Item item in items)
                                                {
                                                    user.Inventory.Items.Remove(item);
                                                }
                                            }
                                            else
                                            {
                                                return NetworkUtility.JsonSerialize($"�����Ʒ��{key}���������� {needCount} �أ�������{isStudy}������");
                                            }
                                        }
                                    }
                                }

                                skill.Level += 1;

                                user.LastTime = DateTime.Now;
                                pc.Add("user", user);
                                pc.SaveConfig();
                                string msg = $"{isStudy}���ܳɹ����������ģ�{string.Join("��", needy.Select(kv => kv.Key + " * " + kv.Value))}���ɹ�����{skill.Name}������������ {skill.Level} ����";

                                if (skill.Level == General.GameplayEquilibriumConstant.MaxSkillLevel)
                                {
                                    msg += $"\r\n�˼����Ѿ�������������ϲ��";
                                }
                                else
                                {
                                    msg += $"\r\n��һ�������������ϣ�\r\n" + FunGameService.GetSkillLevelUpNeedy(skill.Level + 1);
                                }

                                return NetworkUtility.JsonSerialize(msg);
                            }

                            return NetworkUtility.JsonSerialize($"{isStudy}����ʧ�ܣ���ɫ [ {character} ] �ġ�{skill.Name}�����ܵ�ǰ�ȼ���{skill.Level}/{General.GameplayEquilibriumConstant.MaxSkillLevel}" +
                                $"\r\n��һ�������������ϣ�\r\n" + FunGameService.GetSkillLevelUpNeedy(skill.Level + 1));
                        }
                        return NetworkUtility.JsonSerialize($"�˼����޷�{isStudy}��");
                    }
                    else
                    {
                        return NetworkUtility.JsonSerialize($"�˽�ɫû�С�{skillName}�����ܣ�");
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
                FunGameActionQueue.InitFunGameActionQueue();
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
