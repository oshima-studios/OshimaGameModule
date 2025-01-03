using System.Text;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(AuthenticationSchemes = "CustomBearer")]
    [ApiController]
    [Route("[controller]")]
    public class FunGameController(ILogger<FunGameController> logger) : ControllerBase
    {
        public static ItemType[] ItemCanUsed => [ItemType.Consumable, ItemType.MagicCard, ItemType.SpecialItem, ItemType.GiftBox, ItemType.Others];

        private readonly ILogger<FunGameController> _logger = logger;
        private const int drawCardReduce = 2000;
        private const int drawCardReduce_Material = 10;
        private const string noSaved = "你还没有创建存档！请发送【创建存档】创建。";

        [HttpGet("test")]
        public List<string> GetTest([FromQuery] bool? isweb = null, [FromQuery] bool? isteam = null, [FromQuery] bool? showall = null)
        {
            bool web = isweb ?? true;
            bool team = isteam ?? false;
            bool all = showall ?? false;
            return FunGameSimulation.StartSimulationGame(false, web, team, all);
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
                    builder.AppendLine($"总计首杀数：{stats.FirstKills}" + (stats.Plays != 0 ? $" / 首杀率：{(double)stats.FirstKills / stats.Plays * 100:0.##}%" : ""));
                    builder.AppendLine($"总计首死数：{stats.FirstDeaths}" + (stats.Plays != 0 ? $" / 首死率：{(double)stats.FirstDeaths / stats.Plays * 100:0.##}%" : ""));
                    builder.AppendLine($"总计参赛数：{stats.Plays}");
                    builder.AppendLine($"总计冠军数：{stats.Wins}");
                    builder.AppendLine($"总计前三数：{stats.Top3s}");
                    builder.AppendLine($"总计败场数：{stats.Loses}");

                    List<string> names = [.. FunGameSimulation.CharacterStatistics.OrderByDescending(kv => kv.Value.MVPs).Select(kv => kv.Key.GetName())];
                    builder.AppendLine($"MVP次数：{stats.MVPs}（#{names.IndexOf(character.GetName()) + 1}）");

                    names = [.. FunGameSimulation.CharacterStatistics.OrderByDescending(kv => kv.Value.Winrates).Select(kv => kv.Key.GetName())];
                    builder.AppendLine($"胜率：{stats.Winrates * 100:0.##}%（#{names.IndexOf(character.GetName()) + 1}）");
                    builder.AppendLine($"前三率：{stats.Top3rates * 100:0.##}%");

                    names = [.. FunGameSimulation.CharacterStatistics.OrderByDescending(kv => kv.Value.Rating).Select(kv => kv.Key.GetName())];
                    builder.AppendLine($"技术得分：{stats.Rating:0.0#}（#{names.IndexOf(character.GetName()) + 1}）");

                    builder.AppendLine($"上次排名：{stats.LastRank} / 场均名次：{stats.AvgRank:0.##}");

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
                    builder.AppendLine($"击杀死亡比：{(stats.Deaths == 0 ? stats.Kills : ((double)stats.Kills / stats.Deaths)):0.##}");
                    builder.AppendLine($"总计助攻数：{stats.Assists}" + (stats.Plays != 0 ? $" / 场均：{(double)stats.Assists / stats.Plays:0.##}" : ""));
                    builder.AppendLine($"总计首杀数：{stats.FirstKills}" + (stats.Plays != 0 ? $" / 首杀率：{(double)stats.FirstKills / stats.Plays * 100:0.##}%" : ""));
                    builder.AppendLine($"总计首死数：{stats.FirstDeaths}" + (stats.Plays != 0 ? $" / 首死率：{(double)stats.FirstDeaths / stats.Plays * 100:0.##}%" : ""));
                    builder.AppendLine($"总计参赛数：{stats.Plays}");
                    builder.AppendLine($"总计胜场数：{stats.Wins}");
                    builder.AppendLine($"总计败场数：{stats.Loses}");

                    List<string> names = [.. FunGameSimulation.TeamCharacterStatistics.OrderByDescending(kv => kv.Value.MVPs).Select(kv => kv.Key.GetName())];
                    builder.AppendLine($"MVP次数：{stats.MVPs}（#{names.IndexOf(character.GetName()) + 1}）");
                    names = [.. FunGameSimulation.TeamCharacterStatistics.OrderByDescending(kv => kv.Value.Winrates).Select(kv => kv.Key.GetName())];
                    builder.AppendLine($"胜率：{stats.Winrates * 100:0.##}%（#{names.IndexOf(character.GetName()) + 1}）");
                    names = [.. FunGameSimulation.TeamCharacterStatistics.OrderByDescending(kv => kv.Value.Rating).Select(kv => kv.Key.GetName())];
                    builder.AppendLine($"技术得分：{stats.Rating:0.0#}（#{names.IndexOf(character.GetName()) + 1}）");

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
                    builder.AppendLine($"总计参赛数：{stats.Plays}");
                    builder.AppendLine($"总计冠军数：{stats.Wins}");
                    builder.AppendLine($"胜率：{stats.Winrates * 100:0.##}%");
                    builder.AppendLine($"技术得分：{stats.Rating:0.0#}");
                    builder.AppendLine($"MVP次数：{stats.MVPs}");
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
                    builder.AppendLine($"总计参赛数：{stats.Plays}");
                    builder.AppendLine($"总计冠军数：{stats.Wins}");
                    builder.AppendLine($"胜率：{stats.Winrates * 100:0.##}%");
                    builder.AppendLine($"前三率：{stats.Top3rates * 100:0.##}%");
                    builder.AppendLine($"技术得分：{stats.Rating:0.0#}");
                    builder.AppendLine($"上次排名：{stats.LastRank} / 场均名次：{stats.AvgRank:0.##}");
                    builder.AppendLine($"MVP次数：{stats.MVPs}");
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
                    builder.AppendLine($"总计参赛数：{stats.Plays}");
                    builder.AppendLine($"总计冠军数：{stats.Wins}");
                    builder.AppendLine($"胜率：{stats.Winrates * 100:0.##}%");
                    builder.AppendLine($"技术得分：{stats.Rating:0.0#}");
                    builder.AppendLine($"MVP次数：{stats.MVPs}");
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
                    builder.AppendLine($"总计参赛数：{stats.Plays}");
                    builder.AppendLine($"总计冠军数：{stats.Wins}");
                    builder.AppendLine($"胜率：{stats.Winrates * 100:0.##}%");
                    builder.AppendLine($"前三率：{stats.Top3rates * 100:0.##}%");
                    builder.AppendLine($"技术得分：{stats.Rating:0.0#}");
                    builder.AppendLine($"上次排名：{stats.LastRank} / 场均名次：{stats.AvgRank:0.##}");
                    builder.AppendLine($"MVP次数：{stats.MVPs}");
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
        public string GetSkillInfo([FromQuery] long? qq = null, [FromQuery] long? id = null)
        {
            long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();
            List<string> msg = [];
            Character? character = null;
            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);
                character = user.Inventory.MainCharacter;
                msg.Add($"技能展示的属性基于你的主战角色：[ {character} ]");
            }
            else
            {
                character = FunGameService.Characters[1].Copy();
                msg.Add($"技能展示的属性基于演示角色：[ {character} ]");
            }
            IEnumerable<Skill> skills = FunGameService.AllSkills;
            if (id != null && FunGameService.Characters.Count > 1)
            {
                Skill? skill = skills.Where(s => s.Id == id).FirstOrDefault()?.Copy();
                if (skill != null)
                {
                    msg.Add(character.ToStringWithLevel() + "\r\n" + skill.ToString());
                    skill.Character = character;
                    skill.Level++; ;
                    msg.Add(character.ToStringWithLevel() + "\r\n" + skill.ToString());
                    character.Level = General.GameplayEquilibriumConstant.MaxLevel;
                    skill.Level = skill.IsMagic ? General.GameplayEquilibriumConstant.MaxMagicLevel : General.GameplayEquilibriumConstant.MaxSkillLevel;
                    msg.Add(character.ToStringWithLevel() + "\r\n" + skill.ToString());

                    return NetworkUtility.JsonSerialize(string.Join("\r\n", msg));
                }
            }

            return NetworkUtility.JsonSerialize("");
        }
        
        [HttpGet("skillinfoname")]
        public string GetSkillInfo_Name([FromQuery] long? qq = null, [FromQuery] string? name = null)
        {
            long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();
            List<string> msg = [];
            Character? character = null;
            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);
                character = user.Inventory.MainCharacter;
                msg.Add($"技能展示的属性基于你的主战角色：[ {character} ]");
            }
            else
            {
                character = FunGameService.Characters[1].Copy();
                msg.Add($"技能展示的属性基于演示角色：[ {character} ]");
            }
            IEnumerable<Skill> skills = FunGameService.AllSkills;
            if (name != null && FunGameService.Characters.Count > 1)
            {
                Skill? skill = skills.Where(s => s.Name == name).FirstOrDefault()?.Copy();
                if (skill != null)
                {
                    msg.Add(character.ToStringWithLevel() + "\r\n" + skill.ToString());
                    skill.Character = character;
                    skill.Level++; ;
                    msg.Add(character.ToStringWithLevel() + "\r\n" + skill.ToString());
                    character.Level = General.GameplayEquilibriumConstant.MaxLevel;
                    skill.Level = skill.IsMagic ? General.GameplayEquilibriumConstant.MaxMagicLevel : General.GameplayEquilibriumConstant.MaxSkillLevel;
                    msg.Add(character.ToStringWithLevel() + "\r\n" + skill.ToString());

                    return NetworkUtility.JsonSerialize(string.Join("\r\n", msg));
                }
            }

            return NetworkUtility.JsonSerialize("");
        }

        [HttpGet("iteminfo")]
        public string GetItemInfo([FromQuery] long? qq = null, [FromQuery] long? id = null)
        {
            long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();
            List<string> msg = [];
            Character? character = null;
            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);
                character = user.Inventory.MainCharacter;
                msg.Add($"技能展示的属性基于你的主战角色：[ {character} ]");
            }
            else
            {
                character = FunGameService.Characters[1].Copy();
                msg.Add($"技能展示的属性基于演示角色：[ {character} ]");
            }
            IEnumerable<Item> items = FunGameService.AllItems;
            if (id != null)
            {
                Item? item = items.Where(i => i.Id == id).FirstOrDefault()?.Copy();
                if (item != null)
                {
                    item.Character = character;
                    item.SetLevel(1);
                    msg.Add(item.ToString(false, true));

                    return NetworkUtility.JsonSerialize(string.Join("\r\n", msg));
                }
            }
            return NetworkUtility.JsonSerialize("");
        }
        
        [HttpGet("iteminfoname")]
        public string GetItemInfo_Name([FromQuery] long? qq = null, [FromQuery] string? name = null)
        {
            long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();
            List<string> msg = [];
            Character? character = null;
            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);
                character = user.Inventory.MainCharacter;
                msg.Add($"技能展示的属性基于你的主战角色：[ {character} ]");
            }
            else
            {
                character = FunGameService.Characters[1].Copy();
                msg.Add($"技能展示的属性基于演示角色：[ {character} ]");
            }
            IEnumerable<Item> items = FunGameService.AllItems;
            if (name != null)
            {
                Item? item = items.Where(i => i.Name == name).FirstOrDefault()?.Copy();
                if (item != null)
                {
                    item.Character = character;
                    item.SetLevel(1);
                    msg.Add(item.ToString(false, true));

                    return NetworkUtility.JsonSerialize(string.Join("\r\n", msg));
                }
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
                return NetworkUtility.JsonSerialize($"创建存档成功！你的用户名是【{username}】。");
            }
            else
            {
                return NetworkUtility.JsonSerialize("你已经创建过存档！");
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
                return NetworkUtility.JsonSerialize($"你的存档已还原成功。");
            }
            else
            {
                return NetworkUtility.JsonSerialize(noSaved);
            }
        }

        [HttpPost("showsaved")]
        public string ShowSaved([FromQuery] long? qq = null)
        {
            long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);

                StringBuilder builder = new();
                builder.AppendLine($"☆★☆ {user.Username}的存档信息 ☆★☆");
                builder.AppendLine($"{General.GameplayEquilibriumConstant.InGameCurrency}：{user.Inventory.Credits:0.00}");
                builder.AppendLine($"{General.GameplayEquilibriumConstant.InGameMaterial}：{user.Inventory.Materials:0.00}");
                builder.AppendLine($"角色数量：{user.Inventory.Characters.Count}");
                builder.AppendLine($"主战角色：{user.Inventory.MainCharacter.ToStringWithLevelWithOutUser()}");
                Character[] squad = [.. user.Inventory.Characters.Where(c => user.Inventory.Squad.Contains(c.Id))];
                Dictionary<Character, int> characters = user.Inventory.Characters
                    .Select((character, index) => new { character, index })
                    .ToDictionary(x => x.character, x => x.index + 1);
                builder.AppendLine($"小队成员：{(squad.Length > 0 ? string.Join(" / ", squad.Select(c => $"[#{characters[c]}]{c.Name}({c.Level})")) : "空")}");
                if (user.Inventory.Training.Count > 0)
                {
                    builder.AppendLine($"正在练级：{string.Join(" / ", user.Inventory.Characters.Where(c => user.Inventory.Training.ContainsKey(c.Id)).Select(c => c.ToStringWithLevelWithOutUser()))}");
                }
                builder.AppendLine($"物品数量：{user.Inventory.Items.Count}");
                builder.AppendLine($"所属社团：无");
                builder.AppendLine($"注册时间：{user.RegTime.ToString(General.GeneralDateTimeFormatChinese)}");
                builder.AppendLine($"最后访问：{user.LastTime.ToString(General.GeneralDateTimeFormatChinese)}");

                user.LastTime = DateTime.Now;
                pc.Add("user", user);
                pc.SaveConfig();
                return NetworkUtility.JsonSerialize(builder.ToString().Trim());
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
                    return NetworkUtility.JsonSerialize($"你的{General.GameplayEquilibriumConstant.InGameCurrency}不足 {reduce} 呢，无法改名！");
                }

                user.Username = FunGameService.GenerateRandomChineseUserName();
                if (user.Inventory.Characters.FirstOrDefault(c => c.Id == user.Id) is Character character)
                {
                    character.Name = user.Username;
                }
                if (user.Inventory.Name.EndsWith("的库存"))
                {
                    user.Inventory.Name = user.Username + "的库存";
                }
                FunGameService.UserIdAndUsername[user.Id] = user.Username;
                user.LastTime = DateTime.Now;
                pc.Add("user", user);
                pc.SaveConfig();
                return NetworkUtility.JsonSerialize($"消耗 {reduce} {General.GameplayEquilibriumConstant.InGameCurrency}，你的新名字是【{user.Username}】");
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
                            return NetworkUtility.JsonSerialize($"你已完成重随属性确认，新的自建角色属性如下：\r\n" +
                                $"核心属性：{CharacterSet.GetPrimaryAttributeName(oldPA)} => {CharacterSet.GetPrimaryAttributeName(character.PrimaryAttribute)}\r\n" +
                                $"初始力量：{oldSTR}（+{oldSTRG}/Lv）=> {character.InitialSTR}（+{character.STRGrowth}/Lv）\r\n" +
                                $"初始敏捷：{oldAGI}（+{oldAGIG}/Lv）=> {character.InitialAGI}（+{character.AGIGrowth}/Lv）\r\n" +
                                $"初始智力：{oldINT}（+{oldINTG}/Lv）=> {character.InitialINT}（+{character.INTGrowth}/Lv）");
                        }
                        else
                        {
                            return NetworkUtility.JsonSerialize($"你还没有获取过重随属性预览！");
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
                                return NetworkUtility.JsonSerialize($"你的{General.GameplayEquilibriumConstant.InGameMaterial}不足 {reduce} 呢，无法重随自建角色属性！");
                            }
                            newCustom = new CustomCharacter(user.Id, "");
                            user.LastTime = DateTime.Now;
                            pc.Add("user", user);
                            pc.SaveConfig();
                            emc.Add("newCustom", newCustom);
                            emc.SaveConfig();
                            return NetworkUtility.JsonSerialize($"消耗 {reduce} {General.GameplayEquilibriumConstant.InGameMaterial}，获取到重随属性预览如下：\r\n" +
                                $"核心属性：{CharacterSet.GetPrimaryAttributeName(oldPA)} => {CharacterSet.GetPrimaryAttributeName(newCustom.PrimaryAttribute)}\r\n" +
                                $"初始力量：{oldSTR}（+{oldSTRG}/Lv）=> {newCustom.InitialSTR}（+{newCustom.STRGrowth}/Lv）\r\n" +
                                $"初始敏捷：{oldAGI}（+{oldAGIG}/Lv）=> {newCustom.InitialAGI}（+{newCustom.AGIGrowth}/Lv）\r\n" +
                                $"初始智力：{oldINT}（+{oldINTG}/Lv）=> {newCustom.InitialINT}（+{newCustom.INTGrowth}/Lv）\r\n" +
                                $"请发送【确认角色重随】来确认更新，或者发送【取消角色重随】来取消操作。");
                        }
                        else if (newCustom.Id == user.Id)
                        {
                            return NetworkUtility.JsonSerialize($"你已经有一个待确认的重随属性如下：\r\n" +
                                $"核心属性：{CharacterSet.GetPrimaryAttributeName(oldPA)} => {CharacterSet.GetPrimaryAttributeName(newCustom.PrimaryAttribute)}\r\n" +
                                $"初始力量：{oldSTR}（+{oldSTRG}/Lv）=> {newCustom.InitialSTR}（+{newCustom.STRGrowth}/Lv）\r\n" +
                                $"初始敏捷：{oldAGI}（+{oldAGIG}/Lv）=> {newCustom.InitialAGI}（+{newCustom.AGIGrowth}/Lv）\r\n" +
                                $"初始智力：{oldINT}（+{oldINTG}/Lv）=> {newCustom.InitialINT}（+{newCustom.INTGrowth}/Lv）\r\n"+
                                $"请发送【确认角色重随】来确认更新，或者发送【取消角色重随】来取消操作。");
                        }
                        else
                        {
                            return NetworkUtility.JsonSerialize($"重随自建角色属性失败！");
                        }
                    }
                }
                else
                {
                    return NetworkUtility.JsonSerialize($"你似乎没有自建角色，请发送【生成自建角色】创建！");
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
                    return NetworkUtility.JsonSerialize($"已取消角色重随。");
                }
                else
                {
                    return NetworkUtility.JsonSerialize($"你目前没有待确认的角色重随。");
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
                list.Add($"☆★☆ {user.Inventory.Name} ☆★☆");
                list.Add($"{General.GameplayEquilibriumConstant.InGameCurrency}：{user.Inventory.Credits:0.00}");
                list.Add($"{General.GameplayEquilibriumConstant.InGameMaterial}：{user.Inventory.Materials:0.00}");
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
                                list.Add("======= 角色 =======");
                            }
                            str = $"{prevSequence + characterCount}. {character.ToStringWithLevelWithOutUser()}";
                        }
                        if (obj is Item item)
                        {
                            itemCount++;
                            if (showItem)
                            {
                                showItem = false;
                                list.Add("======= 物品 =======");
                            }
                            str = $"{index - (characterCount > 0 ? prevSequence + characterCount : characters.Count)}. [{ItemSet.GetQualityTypeName(item.QualityType)}|{ItemSet.GetItemTypeName(item.ItemType)}] {item.Name}\r\n";
                            str += $"{item.ToStringInventory(false).Trim()}";
                        }
                        list.Add(str);
                    }

                    list.Add($"页数：{showPage} / {maxPage}");
                }
                else
                {
                    list.Add($"没有这么多页！当前总页数为 {maxPage}，但你请求的是第 {showPage} 页。");
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
                list.Add($"☆★☆ {user.Inventory.Name} ☆★☆");
                list.Add($"{General.GameplayEquilibriumConstant.InGameCurrency}：{user.Inventory.Credits:0.00}");
                list.Add($"{General.GameplayEquilibriumConstant.InGameMaterial}：{user.Inventory.Materials:0.00}");
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
                    list.Add("======= 物品 =======");
                    foreach (string key in keys)
                    {
                        itemCount++;
                        List<Item> objs = itemCategory[key];
                        Item first = objs[0];
                        string str = $"{itemCount}. [{ItemSet.GetQualityTypeName(first.QualityType)}|{ItemSet.GetItemTypeName(first.ItemType)}] {first.Name}\r\n";
                        str += $"物品描述：{first.Description}\r\n";
                        string itemsIndex = string.Join("，", objs.Select(i => items.IndexOf(i) + 1));
                        if (objs.Count > 10)
                        {
                            itemsIndex = string.Join("，", objs.Take(10).Select(i => items.IndexOf(i) + 1)) + "，...";
                        }
                        str += $"物品序号：{itemsIndex}\r\n";
                        str += $"拥有数量：{objs.Count}（" + (first.IsEquipment ? $"可装备数量：{objs.Count(i => i.Character is null)}，" : "") +
                            (ItemCanUsed.Contains(first.ItemType) ? $"可使用数量：{objs.Count(i => i.RemainUseTimes > 0)}，" : "") +
                            $"可出售数量：{objs.Count(i => i.IsSellable)}，可交易数量：{objs.Count(i => i.IsTradable)}）";
                        list.Add(str);
                    }
                    list.Add($"页数：{showPage} / {maxPage}");
                }
                else
                {
                    list.Add($"没有这么多页！当前总页数为 {maxPage}，但你请求的是第 {showPage} 页。");
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
                    return ["没有指定物品的类型，请使用通用查询方法！"];
                }

                User user = FunGameService.GetUser(pc);
                list.Add($"☆★☆ {user.Inventory.Name} ☆★☆");
                list.Add($"{General.GameplayEquilibriumConstant.InGameCurrency}：{user.Inventory.Credits:0.00}");
                list.Add($"{General.GameplayEquilibriumConstant.InGameMaterial}：{user.Inventory.Materials:0.00}");
                List<Item> items = [.. user.Inventory.Items];

                Dictionary<string, List<Item>> itemCategory = [];
                foreach (Item item in items)
                {
                    if (!itemCategory.TryAdd(item.GetIdName(), [item]))
                    {
                        itemCategory[item.GetIdName()].Add(item);
                    }
                }

                // 按品质倒序、数量倒序排序
                itemCategory = itemCategory.OrderByDescending(kv => kv.Value.FirstOrDefault()?.QualityType ?? 0).ThenByDescending(kv => kv.Value.Count).ToDictionary();

                // 移除所有非指定类型的物品
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
                        str += $"物品描述：{first.Description}\r\n";
                        string itemsIndex = string.Join("，", objs.Select(i => items.IndexOf(i) + 1));
                        if (objs.Count > 10)
                        {
                            itemsIndex = string.Join("，", objs.Take(10).Select(i => items.IndexOf(i) + 1)) + "，...";
                        }
                        str += $"物品序号：{itemsIndex}\r\n";
                        str += $"拥有数量：{objs.Count}（" + (first.IsEquipment ? $"可装备数量：{objs.Count(i => i.Character is null)}，" : "") +
                            (ItemCanUsed.Contains(first.ItemType) ? $"可使用数量：{objs.Count(i => i.RemainUseTimes > 0)}，" : "") +
                            $"可出售数量：{objs.Count(i => i.IsSellable)}，可交易数量：{objs.Count(i => i.IsTradable)}）";
                        list.Add(str);
                    }
                    list.Add($"页数：{showPage} / {maxPage}");
                }
                else
                {
                    list.Add($"没有这么多页！当前总页数为 {maxPage}，但你请求的是第 {showPage} 页。");
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
                list.Add($"☆★☆ {user.Inventory.Name} ☆★☆");
                list.Add($"{General.GameplayEquilibriumConstant.InGameCurrency}：{user.Inventory.Credits:0.00}");
                list.Add($"{General.GameplayEquilibriumConstant.InGameMaterial}：{user.Inventory.Materials:0.00}");
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
                                list.Add("======= 角色 =======");
                            }
                            str = $"{prevSequence + characterCount}. {character.ToStringWithLevelWithOutUser()}";
                        }
                        list.Add(str);
                    }

                    list.Add($"页数：{showPage} / {maxPage}");
                }
                else
                {
                    list.Add($"没有这么多页！当前总页数为 {maxPage}，但你请求的是第 {showPage} 页。");
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
                    return NetworkUtility.JsonSerialize($"你已经拥有一个自建角色【{user.Username}】，无法再创建！");
                }
                else
                {
                    user.Inventory.Characters.Add(new CustomCharacter(userid, user.Username));
                    user.LastTime = DateTime.Now;
                    pc.Add("user", user);
                    pc.SaveConfig();
                    return NetworkUtility.JsonSerialize($"恭喜你成功创建了一个自建角色【{user.Username}】，请查看你的角色库存！");
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
                    return NetworkUtility.JsonSerialize($"你的{General.GameplayEquilibriumConstant.InGameCurrency}不足 {reduce} 呢，无法抽卡！");
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
                    return NetworkUtility.JsonSerialize($"消耗 {reduce} {General.GameplayEquilibriumConstant.InGameCurrency}，你什么也没抽中……");
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
                    return [$"你的{General.GameplayEquilibriumConstant.InGameCurrency}不足 {reduce} 呢，无法十连抽卡！"];
                }

                List<string> result = [$"消耗 {reduce} {General.GameplayEquilibriumConstant.InGameCurrency}，恭喜你抽到了："];
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
                    result[0] = $"消耗 {reduce} {General.GameplayEquilibriumConstant.InGameCurrency}，你什么也没抽中……";
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
                    return NetworkUtility.JsonSerialize($"你的{General.GameplayEquilibriumConstant.InGameMaterial}不足 {reduce} 呢，无法抽卡！");
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
                    return NetworkUtility.JsonSerialize($"消耗 {reduce} {General.GameplayEquilibriumConstant.InGameMaterial}，你什么也没抽中……");
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
                    return [$"你的{General.GameplayEquilibriumConstant.InGameMaterial}不足 {reduce} 呢，无法十连抽卡！"];
                }

                List<string> result = [$"消耗 {reduce} {General.GameplayEquilibriumConstant.InGameMaterial}，恭喜你抽到了："];
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
                    result[0] = $"消耗 {reduce} {General.GameplayEquilibriumConstant.InGameMaterial}，你什么也没抽中……";
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
                    return NetworkUtility.JsonSerialize($"兑换成功！你消耗了 {reduce} {General.GameplayEquilibriumConstant.InGameMaterial}，增加了 {reward} {General.GameplayEquilibriumConstant.InGameCurrency}！");
                }
                else
                {
                    return NetworkUtility.JsonSerialize($"你的{General.GameplayEquilibriumConstant.InGameMaterial}不足 {reduce}，最低消耗 10 {General.GameplayEquilibriumConstant.InGameMaterial}兑换 2000 {General.GameplayEquilibriumConstant.InGameCurrency}！");
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

                    if (cIndex == 0)
                    {
                        if (isSimple)
                        {
                            return NetworkUtility.JsonSerialize($"这是你的主战角色简略信息：\r\n{user.Inventory.MainCharacter.GetSimpleInfo(showEXP: true).Trim()}");
                        }
                        return NetworkUtility.JsonSerialize($"这是你的主战角色详细信息：\r\n{user.Inventory.MainCharacter.GetInfo().Trim()}");
                    }
                    else
                    {
                        if (cIndex > 0 && cIndex <= user.Inventory.Characters.Count)
                        {
                            Character character = user.Inventory.Characters.ToList()[cIndex - 1];
                            if (isSimple)
                            {
                                return NetworkUtility.JsonSerialize($"这是你库存中序号为 {cIndex} 的角色简略信息：\r\n{character.GetSimpleInfo(showEXP: true).Trim()}");
                            }
                            return NetworkUtility.JsonSerialize($"这是你库存中序号为 {cIndex} 的角色详细信息：\r\n{character.GetInfo().Trim()}");
                        }
                        else
                        {
                            return NetworkUtility.JsonSerialize($"没有找到与这个序号相对应的角色！");
                        }
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
        
        [HttpPost("showcharacterskills")]
        public string GetCharacterSkills([FromQuery] long? qq = null, [FromQuery] int? seq = null)
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

                    if (cIndex == 0)
                    {
                        return NetworkUtility.JsonSerialize($"这是你的主战角色技能信息：\r\n{user.Inventory.MainCharacter.GetSkillInfo().Trim()}");
                    }
                    else
                    {
                        if (cIndex > 0 && cIndex <= user.Inventory.Characters.Count)
                        {
                            Character character = user.Inventory.Characters.ToList()[cIndex - 1];
                            return NetworkUtility.JsonSerialize($"这是你库存中序号为 {cIndex} 的角色技能信息：\r\n{character.GetSkillInfo().Trim()}");
                        }
                        else
                        {
                            return NetworkUtility.JsonSerialize($"没有找到与这个序号相对应的角色！");
                        }
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
        
        [HttpPost("showcharacteritems")]
        public string GetCharacterItems([FromQuery] long? qq = null, [FromQuery] int? seq = null)
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

                    if (cIndex == 0)
                    {
                        return NetworkUtility.JsonSerialize($"这是你的主战角色装备物品信息：\r\n{user.Inventory.MainCharacter.GetItemInfo(showEXP: true).Trim()}");
                    }
                    else
                    {
                        if (cIndex > 0 && cIndex <= user.Inventory.Characters.Count)
                        {
                            Character character = user.Inventory.Characters.ToList()[cIndex - 1];
                            return NetworkUtility.JsonSerialize($"这是你库存中序号为 {cIndex} 的角色装备物品信息：\r\n{character.GetItemInfo(showEXP: true).Trim()}");
                        }
                        else
                        {
                            return NetworkUtility.JsonSerialize($"没有找到与这个序号相对应的角色！");
                        }
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
                        return NetworkUtility.JsonSerialize($"这是你库存中序号为 {itemIndex} 的物品详细信息：\r\n{item.ToStringInventory(true).Trim()}");
                    }
                    else
                    {
                        return NetworkUtility.JsonSerialize($"没有找到与这个序号相对应的物品！");
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
                        return NetworkUtility.JsonSerialize($"没有找到与这个序号相对应的角色！");
                    }
                    if (itemIndex > 0 && itemIndex <= user.Inventory.Items.Count)
                    {
                        item = user.Inventory.Items.ToList()[itemIndex - 1];
                        if ((int)item.ItemType < (int)ItemType.MagicCardPack || (int)item.ItemType > (int)ItemType.Accessory)
                        {
                            return NetworkUtility.JsonSerialize($"这个物品无法被装备！");
                        }
                        else if (item.Character != null)
                        {
                            return NetworkUtility.JsonSerialize($"这个物品无法被装备！[ {item.Character.ToStringWithLevelWithOutUser()} ] 已装备此物品。");
                        }
                    }
                    else
                    {
                        return NetworkUtility.JsonSerialize($"没有找到与这个序号相对应的物品！");
                    }
                    if (character != null && item != null && character.Equip(item))
                    {
                        user.LastTime = DateTime.Now;
                        pc.Add("user", user);
                        pc.SaveConfig();
                        return NetworkUtility.JsonSerialize($"装备{ItemSet.GetQualityTypeName(item.QualityType)}{ItemSet.GetItemTypeName(item.ItemType)}【{item.Name}】成功！" +
                            $"（{ItemSet.GetEquipSlotTypeName(item.EquipSlotType)}栏位）\r\n物品描述：{item.Description}");
                    }
                    else
                    {
                        return NetworkUtility.JsonSerialize($"装备失败！可能是角色、物品不存在或者其他原因。");
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
                            return NetworkUtility.JsonSerialize($"取消装备{ItemSet.GetQualityTypeName(item.QualityType)}{ItemSet.GetItemTypeName(item.ItemType)}【{item.Name}】成功！（{ItemSet.GetEquipSlotTypeName(type)}栏位）");
                        }
                        else return NetworkUtility.JsonSerialize($"取消装备失败！角色并没有装备{ItemSet.GetEquipSlotTypeName(type)}，或者库存中不存在此物品！");
                    }
                    else
                    {
                        return NetworkUtility.JsonSerialize($"没有找到与这个序号相对应的角色！");
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
                    return [$"对方貌似还没有创建存档呢！"];
                }

                if (user1 != null && user2 != null)
                {
                    user1.Inventory.MainCharacter.Recovery(EP: 200);
                    user2.Inventory.MainCharacter.Recovery(EP: 200);
                    return FunGameActionQueue.NewAndStartGame([user1.Inventory.MainCharacter, user2.Inventory.MainCharacter], false, false, false, false, showAllRound);
                }
                else
                {
                    return [$"决斗发起失败！"];
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
                        return [$"找不到此名称对应的玩家！"];
                    }
                    return FightCustom(qq, enemyid, all);
                }
                return [$"决斗发起失败！"];
            }
            catch (Exception e)
            {
                return [e.ToString()];
            }
        }

        [HttpPost("fightcustomteam")]
        public List<string> FightCustomTeam([FromQuery] long? qq = null, [FromQuery] long? eqq = null, [FromQuery] bool? all = null)
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

                    if (user1.Inventory.Squad.Count == 0)
                    {
                        return [$"你尚未设置小队，请先设置1-4名角色！"];
                    }

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

                    if (user2.Inventory.Squad.Count == 0)
                    {
                        return [$"对方尚未设置小队，无法决斗。"];
                    }

                    user2.LastTime = DateTime.Now;
                    pc2.Add("user", user2);
                    pc2.SaveConfig();
                }
                else
                {
                    return [$"对方貌似还没有创建存档呢！"];
                }

                if (user1 != null && user2 != null)
                {
                    Character[] squad1 = [.. user1.Inventory.Characters.Where(c => user1.Inventory.Squad.Contains(c.Id)).Select(c => c)];
                    Character[] squad2 = [.. user2.Inventory.Characters.Where(c => user2.Inventory.Squad.Contains(c.Id)).Select(c => c)];
                    foreach (Character character in squad1.Union(squad2))
                    {
                        character.Recovery(EP: 200);
                    }
                    Team team1 = new($"{user1.Username}的小队", squad1);
                    Team team2 = new($"{user2.Username}的小队", squad2);
                    return FunGameActionQueue.NewAndStartTeamGame([team1, team2], 0, 0, false, false, false, false, showAllRound);
                }
                else
                {
                    return [$"决斗发起失败！"];
                }
            }
            catch (Exception e)
            {
                return [e.ToString()];
            }
        }

        [HttpPost("fightcustomteam2")]
        public List<string> FightCustomTeam2([FromQuery] long? qq = null, [FromQuery] string? name = null, [FromQuery] bool? all = null)
        {
            try
            {
                if (name != null)
                {
                    long enemyid = FunGameService.UserIdAndUsername.Where(kv => kv.Value == name).Select(kv => kv.Key).FirstOrDefault();
                    if (enemyid == 0)
                    {
                        return [$"找不到此名称对应的玩家！"];
                    }
                    return FightCustomTeam(qq, enemyid, all);
                }
                return [$"决斗发起失败！"];
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
                        if (ItemCanUsed.Contains(item.ItemType))
                        {
                            if (item.RemainUseTimes <= 0)
                            {
                                return NetworkUtility.JsonSerialize("此物品剩余使用次数为0，无法使用！");
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
                            
                            if (FunGameService.UseItem(item, user, targets, out string msg))
                            {
                                user.LastTime = DateTime.Now;
                                pc.Add("user", user);
                                pc.SaveConfig();
                            }
                            return NetworkUtility.JsonSerialize(msg);
                        }
                        else
                        {
                            return NetworkUtility.JsonSerialize($"这个物品无法使用！");
                        }
                    }
                    else
                    {
                        return NetworkUtility.JsonSerialize($"没有找到与这个序号相对应的物品！");
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
                        return NetworkUtility.JsonSerialize($"库存中不存在名称为【{name}】的物品！如果是魔法卡，请用【使用魔法卡】指令。");
                    }

                    if (items.Count() >= useCount)
                    {
                        items = items.TakeLast(useCount);
                        List<string> msgs = [];

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
                                msgs.Add($"库存中不存在序号为 {characterIndex} 的角色！");
                            }
                        }

                        // 一个失败全部失败
                        if (FunGameService.UseItems(items, user, targets, msgs))
                        {
                            user.LastTime = DateTime.Now;
                            pc.Add("user", user);
                            pc.SaveConfig();
                        }
                        return NetworkUtility.JsonSerialize($"成功使用 {useCount} 件物品！\r\n" + string.Join("\r\n", msgs.Count > 30 ? msgs.Take(30) : msgs));
                    }
                    else
                    {
                        return NetworkUtility.JsonSerialize("此物品的可使用数量小于你想要使用的数量！");
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
                                return NetworkUtility.JsonSerialize("此物品剩余使用次数为0，无法使用！");
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
                                            return NetworkUtility.JsonSerialize($"库存中没有找到此角色对应的魔法卡包！");
                                        }
                                    }
                                    else
                                    {
                                        return NetworkUtility.JsonSerialize($"这个角色没有装备魔法卡包，无法对其使用魔法卡！");
                                    }
                                }
                                else
                                {
                                    return NetworkUtility.JsonSerialize($"没有找到与这个序号相对应的角色！");
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
                                        return NetworkUtility.JsonSerialize($"与目标序号相对应的物品不是魔法卡包！");
                                    }
                                }
                                else
                                {
                                    return NetworkUtility.JsonSerialize($"没有找到与目标序号相对应的物品！");
                                }
                            }

                            user.LastTime = DateTime.Now;
                            pc.Add("user", user);
                            pc.SaveConfig();
                            return NetworkUtility.JsonSerialize(msg);
                        }
                        else
                        {
                            return NetworkUtility.JsonSerialize($"这个物品不是魔法卡！");
                        }
                    }
                    else
                    {
                        return NetworkUtility.JsonSerialize($"没有找到与目标序号相对应的物品！");
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
                        return NetworkUtility.JsonSerialize($"没有找到与这个序号相对应的角色！");
                    }

                    if (character.Level == General.GameplayEquilibriumConstant.MaxLevel)
                    {
                        return NetworkUtility.JsonSerialize($"该角色等级已满，无需再升级！");
                    }

                    int originalLevel = character.Level;

                    character.OnLevelUp(upCount);

                    string msg = $"升级完成！角色 [ {character} ] 共提升 {character.Level - originalLevel} 级，当前等级：{character.Level} 级。";

                    if (character.Level != General.GameplayEquilibriumConstant.MaxLevel && General.GameplayEquilibriumConstant.EXPUpperLimit.TryGetValue(character.Level, out double need))
                    {
                        if (character.EXP < need)
                        {
                            msg += $"\r\n角色 [ {character} ] 仍需 {need - character.EXP} 点经验值才能继续升级。";
                        }
                        else
                        {
                            msg += $"\r\n角色 [ {character} ] 目前突破进度：{character.LevelBreak + 1}/{General.GameplayEquilibriumConstant.LevelBreakList.Count}，需要进行【角色突破】才能继续升级。";
                        }
                    }
                    else if (character.Level == General.GameplayEquilibriumConstant.MaxLevel)
                    {
                        msg += $"\r\n该角色已升级至满级，恭喜！";
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
                    return NetworkUtility.JsonSerialize($"没有找到与这个序号相对应的角色！");
                }

                if (character.LevelBreak + 1 == General.GameplayEquilibriumConstant.LevelBreakList.Count)
                {
                    return NetworkUtility.JsonSerialize($"该角色已完成全部的突破阶段，无需再突破！");
                }

                return NetworkUtility.JsonSerialize($"角色 [ {character} ] 目前突破进度：{character.LevelBreak + 1}/{General.GameplayEquilibriumConstant.LevelBreakList.Count}" +
                    $"\r\n该角色下一个等级突破阶段在 {General.GameplayEquilibriumConstant.LevelBreakList.ToArray()[character.LevelBreak + 1]} 级，所需材料：\r\n" + FunGameService.GetLevelBreakNeedy(character.LevelBreak + 1));
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
                        return NetworkUtility.JsonSerialize($"没有找到与这个序号相对应的角色！");
                    }

                    if (character.LevelBreak + 1 == General.GameplayEquilibriumConstant.LevelBreakList.Count)
                    {
                        return NetworkUtility.JsonSerialize($"该角色已完成全部的突破阶段，无需再突破！");
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
                                    return NetworkUtility.JsonSerialize($"你的{General.GameplayEquilibriumConstant.InGameMaterial}不足 {needCount} 呢，不满足突破条件！");
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
                                        return NetworkUtility.JsonSerialize($"你的物品【{key}】数量不足 {needCount} 呢，不满足突破条件！");
                                    }
                                }
                            }
                        }
                    }

                    character.OnLevelBreak();

                    if (originalBreak == character.LevelBreak)
                    {
                        return NetworkUtility.JsonSerialize($"突破失败！角色 [ {character} ] 目前突破进度：{character.LevelBreak + 1}/{General.GameplayEquilibriumConstant.LevelBreakList.Count}。" +
                            $"\r\n该角色下一个等级突破阶段在 {General.GameplayEquilibriumConstant.LevelBreakList.ToArray()[character.LevelBreak + 1]} 级，所需材料：\r\n" + FunGameService.GetLevelBreakNeedy(character.LevelBreak + 1));
                    }
                    else
                    {
                        user.LastTime = DateTime.Now;
                        pc.Add("user", user);
                        pc.SaveConfig();
                        return NetworkUtility.JsonSerialize($"突破成功！角色 [ {character} ] 目前突破进度：{character.LevelBreak + 1}/{General.GameplayEquilibriumConstant.LevelBreakList.Count}。" +
                            $"{(character.LevelBreak + 1 == General.GameplayEquilibriumConstant.LevelBreakList.Count ?
                            "\r\n该角色已完成全部的突破阶段，恭喜！" :
                            $"\r\n该角色下一个等级突破阶段在 {General.GameplayEquilibriumConstant.LevelBreakList.ToArray()[character.LevelBreak + 1]} 级，所需材料：\r\n" + FunGameService.GetLevelBreakNeedy(character.LevelBreak + 1))}");
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
                            msg = $"已为 [ {user2} ] 生成 {itemCount} {General.GameplayEquilibriumConstant.InGameCurrency}";
                        }
                        else if (itemName == General.GameplayEquilibriumConstant.InGameMaterial)
                        {
                            user2.Inventory.Materials += itemCount;
                            msg = $"已为 [ {user2} ] 生成 {itemCount} {General.GameplayEquilibriumConstant.InGameMaterial}";
                        }
                        else if (itemName.Contains("魔法卡包"))
                        {
                            foreach (string type in ItemSet.QualityTypeNameArray)
                            {
                                if (itemName == $"{type}魔法卡包")
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
                                    msg = $"已为 [ {user2} ] 成功生成 {success} 个{type}魔法卡包";
                                    break;
                                }
                            }
                        }
                        else if (itemName.Contains("魔法卡"))
                        {
                            foreach (string type in ItemSet.QualityTypeNameArray)
                            {
                                if (itemName == $"{type}魔法卡")
                                {
                                    for (int i = 0; i < itemCount; i++)
                                    {
                                        Item item = FunGameService.GenerateMagicCard(ItemSet.GetQualityTypeFromName(type));
                                        item.User = user2;
                                        user2.Inventory.Items.Add(item);
                                    }
                                    msg = $"已为 [ {user2} ] 生成 {itemCount} 张{type}魔法卡";
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
                            msg = $"已为 [ {user2} ] 生成 {itemCount} 个 [{ItemSet.GetQualityTypeName(item.QualityType)}|{ItemSet.GetItemTypeName(item.ItemType)}] {item.Name}";
                        }
                        else
                        {
                            return NetworkUtility.JsonSerialize($"此物品不存在！");
                        }
                        pc2.Add("user", user2);
                        pc2.SaveConfig();
                    }
                    else
                    {
                        return NetworkUtility.JsonSerialize($"目标 UID 不存在！");
                    }
                }
                else
                {
                    return NetworkUtility.JsonSerialize($"你没有权限使用此指令！");
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
                    return NetworkUtility.JsonSerialize($"分解完毕！分解 {ids.Length} 件，成功 {successCount} 件，得到了 {totalGained} {General.GameplayEquilibriumConstant.InGameMaterial}！");
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
                        return NetworkUtility.JsonSerialize($"库存中不存在名称为【{name}】的物品！");
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
                        return NetworkUtility.JsonSerialize($"分解完毕！分解 {useCount} 件物品，成功 {successCount} 件，得到了 {totalGained} {General.GameplayEquilibriumConstant.InGameMaterial}！");
                    }
                    else
                    {
                        return NetworkUtility.JsonSerialize("此物品的可分解数量小于你想要分解的数量！");
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
                    return NetworkUtility.JsonSerialize($"品质序号输入错误！");
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
                        return NetworkUtility.JsonSerialize($"库存中{qualityName}物品数量为零！");
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
                    return NetworkUtility.JsonSerialize($"分解完毕！成功分解 {successCount} 件{qualityName}物品，得到了 {totalGained} {General.GameplayEquilibriumConstant.InGameMaterial}！");
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
                                return NetworkUtility.JsonSerialize($"此物品不是魔法卡或者使用次数为0：{itemIndex}. {item.Name}");
                            }
                        }
                        else
                        {
                            return NetworkUtility.JsonSerialize($"没有找到与这个序号相对应的物品：{itemIndex}");
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
                            return NetworkUtility.JsonSerialize($"合成魔法卡包成功！获得魔法卡包：\r\n{item.ToStringInventory(true)}");
                        }
                        else
                        {
                            return NetworkUtility.JsonSerialize($"合成魔法卡包失败！");
                        }
                    }
                    else
                    {
                        return NetworkUtility.JsonSerialize($"选用的魔法卡不足 3 张，请重新选择！");
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
                        return NetworkUtility.JsonSerialize($"没有找到与这个序号相对应的角色！");
                    }

                    user.Inventory.MainCharacter = character;
                    user.LastTime = DateTime.Now;
                    pc.Add("user", user);
                    pc.SaveConfig();
                    return NetworkUtility.JsonSerialize($"设置主战角色成功：{character}");
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
                        return NetworkUtility.JsonSerialize($"没有找到与这个序号相对应的角色！");
                    }

                    if (user.Inventory.Training.Count > 0)
                    {
                        return NetworkUtility.JsonSerialize($"你已经有角色在练级中，请使用【练级结算】指令结束并获取奖励：{user.Inventory.Training.First()}！");
                    }

                    user.Inventory.Training[character.Id] = DateTime.Now;
                    user.LastTime = DateTime.Now;
                    pc.Add("user", user);
                    pc.SaveConfig();
                    return NetworkUtility.JsonSerialize($"角色 [{character}] 开始练级，请过一段时间后进行【练级结算】，时间越长奖励越丰盛！练级时间上限 1440 分钟（24小时），超时将不会再产生收益，请按时领取奖励！");
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
                        return NetworkUtility.JsonSerialize($"你目前没有角色在练级中，请使用【开启练级+角色序号】指令进行练级。");
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
                            Item item = new 小经验书(user);
                            user.Inventory.Items.Add(item);
                        }

                        for (int i = 0; i < mediumBookCount; i++)
                        {
                            Item item = new 中经验书(user);
                            user.Inventory.Items.Add(item);
                        }

                        for (int i = 0; i < largeBookCount; i++)
                        {
                            Item item = new 大经验书(user);
                            user.Inventory.Items.Add(item);
                        }

                        user.LastTime = DateTime.Now;
                        pc.Add("user", user);
                        pc.SaveConfig();
                        return NetworkUtility.JsonSerialize($"角色 [ {character} ] 练级结束，{msg}");
                    }
                    else
                    {
                        return NetworkUtility.JsonSerialize($"你目前没有角色在练级中，也可能是库存信息获取异常，请稍后再试。");
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
                        return NetworkUtility.JsonSerialize($"你目前没有角色在练级中，请使用【开启练级+角色序号】指令进行练级。");
                    }

                    long cid = user.Inventory.Training.Keys.First();
                    DateTime time = user.Inventory.Training[cid];
                    DateTime now = DateTime.Now;
                    Character? character = user.Inventory.Characters.FirstOrDefault(c => c.Id == cid);
                    if (character != null)
                    {
                        TimeSpan diff = now - time;
                        string msg = FunGameService.GetTrainingInfo(diff, true, out int totalExperience, out int smallBookCount, out int mediumBookCount, out int largeBookCount);

                        return NetworkUtility.JsonSerialize($"角色 [ {character} ] 正在练级中，{msg}\r\n确认无误后请输入【练级结算】领取奖励！");
                    }
                    else
                    {
                        return NetworkUtility.JsonSerialize($"你目前没有角色在练级中，也可能是库存信息获取异常，请稍后再试。");
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
                    return NetworkUtility.JsonSerialize($"没有找到与这个序号相对应的角色！");
                }

                if (character.Skills.FirstOrDefault(s => s.Name == skillName) is Skill skill)
                {
                    if (skill.SkillType == SkillType.Skill || skill.SkillType == SkillType.SuperSkill)
                    {
                        if (skill.Level + 1 == General.GameplayEquilibriumConstant.MaxSkillLevel)
                        {
                            return NetworkUtility.JsonSerialize($"此技能【{skill.Name}】已经升至满级！");
                        }

                        return NetworkUtility.JsonSerialize($"角色 [ {character} ] 的【{skill.Name}】技能等级：{skill.Level} / {General.GameplayEquilibriumConstant.MaxSkillLevel}" +
                            $"\r\n下一级所需升级材料：\r\n" + FunGameService.GetSkillLevelUpNeedy(skill.Level + 1));
                    }
                    return NetworkUtility.JsonSerialize($"此技能无法升级！");
                }
                else
                {
                    return NetworkUtility.JsonSerialize($"此角色没有【{skillName}】技能！");
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
                        return NetworkUtility.JsonSerialize($"没有找到与这个序号相对应的角色！");
                    }

                    if (character.Skills.FirstOrDefault(s => s.Name == skillName) is Skill skill)
                    {
                        string isStudy = skill.Level == 0 ? "学习" : "升级";

                        if (skill.SkillType == SkillType.Skill || skill.SkillType == SkillType.SuperSkill)
                        {
                            if (skill.Level == General.GameplayEquilibriumConstant.MaxSkillLevel)
                            {
                                return NetworkUtility.JsonSerialize($"此技能【{skill.Name}】已经升至满级！");
                            }

                            if (FunGameService.SkillLevelUpList.TryGetValue(skill.Level + 1, out Dictionary<string, int>? needy) && needy != null && needy.Count > 0)
                            {
                                foreach (string key in needy.Keys)
                                {
                                    int needCount = needy[key];
                                    if (key == "角色等级")
                                    {
                                        if (character.Level < needCount)
                                        {
                                            return NetworkUtility.JsonSerialize($"角色 [ {character} ] 等级不足 {needCount} 级，无法{isStudy}此技能！");
                                        }
                                    }
                                    else if (key == "角色突破进度")
                                    {
                                        if (character.LevelBreak < needCount)
                                        {
                                            return NetworkUtility.JsonSerialize($"角色 [ {character} ] 等级突破进度不足 {needCount} 等阶，无法{isStudy}此技能！");
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
                                            return NetworkUtility.JsonSerialize($"你的{General.GameplayEquilibriumConstant.InGameCurrency}不足 {needCount} 呢，不满足{isStudy}条件！");
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
                                            return NetworkUtility.JsonSerialize($"你的{General.GameplayEquilibriumConstant.InGameMaterial}不足 {needCount} 呢，不满足{isStudy}条件！");
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
                                                return NetworkUtility.JsonSerialize($"你的物品【{key}】数量不足 {needCount} 呢，不满足{isStudy}条件！");
                                            }
                                        }
                                    }
                                }

                                skill.Level += 1;

                                user.LastTime = DateTime.Now;
                                pc.Add("user", user);
                                pc.SaveConfig();
                                needy.Remove("角色等级");
                                needy.Remove("角色突破进度");
                                string msg = $"{isStudy}技能成功！本次消耗：{string.Join("，", needy.Select(kv => kv.Key + " * " + kv.Value))}，成功将【{skill.Name}】技能提升至 {skill.Level} 级！";

                                if (skill.Level == General.GameplayEquilibriumConstant.MaxSkillLevel)
                                {
                                    msg += $"\r\n此技能已经升至满级，恭喜！";
                                }
                                else
                                {
                                    msg += $"\r\n下一级所需升级材料：\r\n" + FunGameService.GetSkillLevelUpNeedy(skill.Level + 1);
                                }

                                return NetworkUtility.JsonSerialize(msg);
                            }

                            return NetworkUtility.JsonSerialize($"{isStudy}技能失败！角色 [ {character} ] 的【{skill.Name}】技能当前等级：{skill.Level}/{General.GameplayEquilibriumConstant.MaxSkillLevel}" +
                                $"\r\n下一级所需升级材料：\r\n" + FunGameService.GetSkillLevelUpNeedy(skill.Level + 1));
                        }
                        return NetworkUtility.JsonSerialize($"此技能无法{isStudy}！");
                    }
                    else
                    {
                        return NetworkUtility.JsonSerialize($"此角色没有【{skillName}】技能！");
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

        [HttpPost("getnormalattacklevelupneedy")]
        public string GetNormalAttackLevelUpNeedy([FromQuery] long? qq = null, [FromQuery] int? c = null)
        {
            long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
            int characterIndex = c ?? 0;

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
                    return NetworkUtility.JsonSerialize($"没有找到与这个序号相对应的角色！");
                }

                NormalAttack na = character.NormalAttack;

                if (na.Level + 1 == General.GameplayEquilibriumConstant.MaxNormalAttackLevel)
                {
                    return NetworkUtility.JsonSerialize($"角色 [ {character} ] 的【{na.Name}】已经升至满级！");
                }
                return NetworkUtility.JsonSerialize($"角色 [ {character} ] 的【{na.Name}】等级：{na.Level} / {General.GameplayEquilibriumConstant.MaxNormalAttackLevel}" +
                    $"\r\n下一级所需升级材料：\r\n" + FunGameService.GetNormalAttackLevelUpNeedy(na.Level + 1));
            }
            else
            {
                return NetworkUtility.JsonSerialize(noSaved);
            }
        }

        [HttpPost("normalattacklevelup")]
        public string NormalAttackLevelUp([FromQuery] long? qq = null, [FromQuery] int? c = null)
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
                        return NetworkUtility.JsonSerialize($"没有找到与这个序号相对应的角色！");
                    }

                    NormalAttack na = character.NormalAttack;
                    if (na.Level == General.GameplayEquilibriumConstant.MaxNormalAttackLevel)
                    {
                        return NetworkUtility.JsonSerialize($"角色 [ {character} ] 的【{na.Name}】已经升至满级！");
                    }

                    if (FunGameService.NormalAttackLevelUpList.TryGetValue(na.Level + 1, out Dictionary<string, int>? needy) && needy != null && needy.Count > 0)
                    {
                        foreach (string key in needy.Keys)
                        {
                            int needCount = needy[key];
                            if (key == "角色等级")
                            {
                                if (character.Level < needCount)
                                {
                                    return NetworkUtility.JsonSerialize($"角色 [ {character} ] 等级不足 {needCount} 级，无法升级此技能！");
                                }
                            }
                            else if (key == "角色突破进度")
                            {
                                if (character.LevelBreak < needCount)
                                {
                                    return NetworkUtility.JsonSerialize($"角色 [ {character} ] 等级突破进度不足 {needCount} 等阶，无法升级此技能！");
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
                                    return NetworkUtility.JsonSerialize($"你的{General.GameplayEquilibriumConstant.InGameCurrency}不足 {needCount} 呢，不满足升级条件！");
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
                                    return NetworkUtility.JsonSerialize($"你的{General.GameplayEquilibriumConstant.InGameMaterial}不足 {needCount} 呢，不满足升级条件！");
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
                                        return NetworkUtility.JsonSerialize($"你的物品【{key}】数量不足 {needCount} 呢，不满足升级条件！");
                                    }
                                }
                            }
                        }

                        na.Level += 1;

                        user.LastTime = DateTime.Now;
                        pc.Add("user", user);
                        pc.SaveConfig();
                        needy.Remove("角色等级");
                        needy.Remove("角色突破进度");
                        string msg = $"角色 [ {character} ] 升级【{na.Name}】成功！本次消耗：{string.Join("，", needy.Select(kv => kv.Key + " * " + kv.Value))}，成功将【{na.Name}】提升至 {na.Level} 级！";

                        if (na.Level == General.GameplayEquilibriumConstant.MaxNormalAttackLevel)
                        {
                            msg += $"\r\n{na.Name}已经升至满级，恭喜！";
                        }
                        else
                        {
                            msg += $"\r\n下一级所需升级材料：\r\n" + FunGameService.GetNormalAttackLevelUpNeedy(na.Level + 1);
                        }

                        return NetworkUtility.JsonSerialize(msg);
                    }

                    return NetworkUtility.JsonSerialize($"升级{na.Name}失败！角色 [ {character} ] 的【{na.Name}】当前等级：{na.Level}/{General.GameplayEquilibriumConstant.MaxNormalAttackLevel}" +
                        $"\r\n下一级所需升级材料：\r\n" + FunGameService.GetSkillLevelUpNeedy(na.Level + 1));
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

        [HttpGet("getboss")]
        public List<string> GetBoss([FromQuery] int? index = null)
        {
            List<string> bosses = [];
            if (index != null)
            {
                if (FunGameService.Bosses.Values.FirstOrDefault(kv => kv.Id == index) is Character boss)
                {
                    bosses.Add(boss.GetInfo(false));
                }
                else
                {
                    bosses.Add($"找不到指定编号的 Boss！");
                }
            }
            else if (FunGameService.Bosses.Count > 0)
            {
                bosses.Add($"Boss 列表：");
                foreach (int i in FunGameService.Bosses.Keys)
                {
                    Character boss = FunGameService.Bosses[i];
                    bosses.Add($"{i}. {boss.ToStringWithLevelWithOutUser()}");
                }
            }
            else
            {
                bosses.Add($"现在没有任何 Boss，请等待刷新~");
            }
            return bosses;
        }
        
        [HttpPost("fightboss")]
        public List<string> FightBoss([FromQuery] long? qq = null, [FromQuery] int? index = null, [FromQuery] bool? all = null)
        {
            long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
            int bossIndex = index ?? 0;
            bool showAllRound = all ?? false;

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);

                if (FunGameService.Bosses.Values.FirstOrDefault(kv => kv.Id == index) is Character boss)
                {
                    if (user.Inventory.MainCharacter.HP < user.Inventory.MainCharacter.MaxHP * 0.1)
                    {
                        return [$"主战角色重伤未愈，当前生命值低于 10%，请先等待生命值自动回复或设置其他主战角色！"];
                    }

                    Character boss2 = CharacterBuilder.Build(boss, false, true, null, FunGameService.AllItems, FunGameService.AllSkills, false);
                    List<string> msgs = FunGameActionQueue.NewAndStartGame([user.Inventory.MainCharacter, boss2], false, false, false, false, showAllRound);

                    if (boss2.HP <= 0)
                    {
                        FunGameService.Bosses.Remove(bossIndex);
                        double gained = boss.Level;
                        user.Inventory.Materials += gained;
                        msgs.Add($"恭喜你击败了 Boss，获得 {gained} 材料奖励！");
                    }
                    else
                    {
                        boss.HP = boss2.HP;
                        boss.MP = boss2.MP;
                        boss.EP = boss2.EP;
                        msgs.Add($"挑战 Boss 失败，请稍后再来！");
                    }
                    user.LastTime = DateTime.Now;
                    pc.Add("user", user);
                    pc.SaveConfig();

                    return msgs;
                }
                else
                {
                    return [$"找不到指定编号的 Boss！"];
                }
            }
            else
            {
                return [noSaved];
            }
        }

        [HttpPost("addsquad")]
        public string AddSquad([FromQuery] long? qq = null, [FromQuery] int? c = null)
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
                        return NetworkUtility.JsonSerialize($"没有找到与这个序号相对应的角色！");
                    }

                    if (user.Inventory.Squad.Count >= 4)
                    {
                        return NetworkUtility.JsonSerialize($"小队人数已满 4 人，无法继续添加角色！当前小队角色如下：\r\n" +
                            string.Join("\r\n", user.Inventory.Characters.Where(c => user.Inventory.Squad.Contains(c.Id))));
                    }
                    
                    if (user.Inventory.Squad.Contains(character.Id))
                    {
                        return NetworkUtility.JsonSerialize($"此角色已经在小队中了！");
                    }

                    user.Inventory.Squad.Add(character.Id);
                    user.LastTime = DateTime.Now;
                    pc.Add("user", user);
                    pc.SaveConfig();
                    return NetworkUtility.JsonSerialize($"添加小队角色成功：{character}\r\n当前小队角色如下：\r\n" +
                            string.Join("\r\n", user.Inventory.Characters.Where(c => user.Inventory.Squad.Contains(c.Id))));
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
        
        [HttpPost("removesquad")]
        public string RemoveSquad([FromQuery] long? qq = null, [FromQuery] int? c = null)
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
                        return NetworkUtility.JsonSerialize($"没有找到与这个序号相对应的角色！");
                    }

                    if (!user.Inventory.Squad.Contains(character.Id))
                    {
                        return NetworkUtility.JsonSerialize($"此角色不在小队中！");
                    }

                    user.Inventory.Squad.Remove(character.Id);
                    user.LastTime = DateTime.Now;
                    pc.Add("user", user);
                    pc.SaveConfig();
                    return NetworkUtility.JsonSerialize($"移除小队角色成功：{character}\r\n当前小队角色如下：\r\n" +
                            string.Join("\r\n", user.Inventory.Characters.Where(c => user.Inventory.Squad.Contains(c.Id))));
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

        [HttpPost("setsquad")]
        public string SetSquad([FromQuery] long? qq = null, [FromBody] int[]? c = null)
        {
            try
            {
                long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
                int[] characterIndexs = c ?? [];

                PluginConfig pc = new("saved", userid.ToString());
                pc.LoadConfig();

                if (pc.Count > 0)
                {
                    User user = FunGameService.GetUser(pc);

                    user.Inventory.Squad.Clear();
                    foreach (int characterIndex in characterIndexs)
                    {
                        Character? character = null;
                        if (characterIndex > 0 && characterIndex <= user.Inventory.Characters.Count)
                        {
                            character = user.Inventory.Characters.ToList()[characterIndex - 1];
                        }
                        else
                        {
                            return NetworkUtility.JsonSerialize($"设置失败：没有找到与序号 {characterIndex} 相对应的角色！");
                        }
                        user.Inventory.Squad.Add(character.Id);
                    }

                    user.LastTime = DateTime.Now;
                    pc.Add("user", user);
                    pc.SaveConfig();
                    return NetworkUtility.JsonSerialize($"设置小队成员成功！当前小队角色如下：\r\n" +
                            string.Join("\r\n", user.Inventory.Characters.Where(c => user.Inventory.Squad.Contains(c.Id))));
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
        
        [HttpPost("clearsquad")]
        public string ClearSquad([FromQuery] long? qq = null, [FromBody] int[]? c = null)
        {
            try
            {
                long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
                int[] characterIndexs = c ?? [];

                PluginConfig pc = new("saved", userid.ToString());
                pc.LoadConfig();

                if (pc.Count > 0)
                {
                    User user = FunGameService.GetUser(pc);

                    user.Inventory.Squad.Clear();
                    user.LastTime = DateTime.Now;
                    pc.Add("user", user);
                    pc.SaveConfig();
                    return NetworkUtility.JsonSerialize($"清空小队成员成功！");
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
        
        [HttpPost("showsquad")]
        public string ShowSquad([FromQuery] long? qq = null)
        {
            try
            {
                long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));

                PluginConfig pc = new("saved", userid.ToString());
                pc.LoadConfig();

                if (pc.Count > 0)
                {
                    User user = FunGameService.GetUser(pc);
                    return NetworkUtility.JsonSerialize($"你的当前小队角色如下：\r\n" +
                            string.Join("\r\n", user.Inventory.Characters.Where(c => user.Inventory.Squad.Contains(c.Id))));
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

        [HttpPost("fightbossteam")]
        public List<string> FightBossTeam([FromQuery] long? qq = null, [FromQuery] int? index = null, [FromQuery] bool? all = null)
        {
            long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
            int bossIndex = index ?? 0;
            bool showAllRound = all ?? false;

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);

                if (FunGameService.Bosses.Values.FirstOrDefault(kv => kv.Id == index) is Character boss)
                {
                    Character[] squad = [.. user.Inventory.Characters.Where(c => user.Inventory.Squad.Contains(c.Id))];

                    if (squad.All(c => c.HP < c.MaxHP * 0.1))
                    {
                        return [$"小队角色均重伤未愈，当前生命值低于 10%，请先等待生命值自动回复或重组小队！\r\n" +
                            "当前小队角色如下：\r\n" +
                            string.Join("\r\n", user.Inventory.Characters.Where(c => user.Inventory.Squad.Contains(c.Id)))];
                    }

                    Character boss2 = CharacterBuilder.Build(boss, false, true, null, FunGameService.AllItems, FunGameService.AllSkills, false);
                    Team team1 = new($"{user.Username}的小队", squad);
                    Team team2 = new($"Boss", [boss2]);
                    List<string> msgs = FunGameActionQueue.NewAndStartTeamGame([team1, team2], showAllRound: showAllRound);

                    if (boss2.HP <= 0)
                    {
                        FunGameService.Bosses.Remove(bossIndex);
                        double gained = boss.Level;
                        user.Inventory.Materials += gained;
                        msgs.Add($"恭喜你击败了 Boss，获得 {gained} 材料奖励！");
                    }
                    else
                    {
                        boss.HP = boss2.HP;
                        boss.MP = boss2.MP;
                        boss.EP = boss2.EP;
                        msgs.Add($"挑战 Boss 失败，请稍后再来！");
                    }
                    user.LastTime = DateTime.Now;
                    pc.Add("user", user);
                    pc.SaveConfig();

                    return msgs;
                }
                else
                {
                    return [$"找不到指定编号的 Boss！"];
                }
            }
            else
            {
                return [noSaved];
            }
        }

        [HttpPost("checkquestlist")]
        public string CheckQuestList([FromQuery] long? qq = null)
        {
            long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);

                EntityModuleConfig<Quest> quests = new("quests", userid.ToString());
                quests.LoadConfig();
                string msg = FunGameService.CheckQuestList(quests);
                quests.SaveConfig();

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
        
        [HttpPost("checkworkingquest")]
        public string CheckWorkingQuest([FromQuery] long? qq = null)
        {
            long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);

                EntityModuleConfig<Quest> quests = new("quests", userid.ToString());
                quests.LoadConfig();
                string msg = "";
                IEnumerable<Quest> working = quests.Values.Where(q => q.Status == QuestState.InProgress);
                if (working.Any())
                {
                    msg = "你正在进行中的任务详情如下：\r\n" + string.Join("\r\n", working);
                }
                else
                {
                    msg = "你当前没有正在进行中的任务！";
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
        
        [HttpPost("acceptquest")]
        public string AcceptQuest([FromQuery] long? qq = null, [FromQuery] int? id = null)
        {
            long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
            int questid = id ?? 0;

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();
            
            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);

                string msg = "";
                EntityModuleConfig<Quest> quests = new("quests", userid.ToString());
                quests.LoadConfig();
                if (quests.Count > 0)
                {
                    if (quests.Values.FirstOrDefault(q => q.Status == QuestState.InProgress) is Quest quest)
                    {
                        msg = $"你正在进行任务【{quest.Name}】，无法开始新任务！\r\n{quest}";
                    }
                    else if (quests.Values.FirstOrDefault(q => q.Id == questid) is Quest quest2)
                    {
                        if (quest2.Status != 0)
                        {
                            msg = $"这个任务正在进行中，或已经完成，不能重复做任务！";
                        }
                        else
                        {
                            quest2.StartTime = DateTime.Now;
                            quest2.Status = QuestState.InProgress;
                            quests.SaveConfig();
                            msg = $"开始任务【{quest2.Name}】成功！任务信息如下：\r\n{quest2}\r\n预计完成时间：{DateTime.Now.AddMinutes(quest2.EstimatedMinutes).ToString(General.GeneralDateTimeFormatChinese)}";
                        }
                    }
                    else
                    {
                        msg = $"没有找到序号为 {questid} 的任务！";
                    }
                }
                else
                {
                    msg = "任务列表为空，请等待刷新！";
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

        [HttpPost("settlequest")]
        public string SettleQuest([FromQuery] long? qq = null)
        {
            long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);

                EntityModuleConfig<Quest> quests = new("quests", userid.ToString());
                quests.LoadConfig();
                if (quests.Count > 0 && FunGameService.SettleQuest(user, quests))
                {
                    quests.SaveConfig();
                    user.LastTime = DateTime.Now;
                    pc.Add("user", user);
                    pc.SaveConfig();
                }

                return NetworkUtility.JsonSerialize("任务结算已完成，请查看你的任务列表！");
            }
            else
            {
                return NetworkUtility.JsonSerialize(noSaved);
            }
        }
        
        [HttpPost("showmaincharacterorsquadstatus")]
        public string ShowMainCharacterOrSquadStatus([FromQuery] long? qq = null, [FromQuery] bool? squad = null)
        {
            long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
            bool showSquad = squad ?? false; 

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);
                string msg = "";

                if (showSquad)
                {
                    Character[] characters = [.. user.Inventory.Characters.Where(c => user.Inventory.Squad.Contains(c.Id))];
                    foreach (Character character in characters)
                    {
                        if (msg != "") msg += "\r\n";
                        msg += character.GetSimpleInfo(true, false, false, true);
                    }
                }
                else
                {
                    msg = user.Inventory.MainCharacter.GetSimpleInfo(true, false, false, true);
                }

                return NetworkUtility.JsonSerialize(msg);
            }
            else
            {
                return NetworkUtility.JsonSerialize(noSaved);
            }
        }

        [HttpPost("signin")]
        public string SignIn([FromQuery] long? qq = null)
        {
            long userid = qq ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);
                bool sign = false;
                if (pc.TryGetValue("signed", out object? value) && value is bool temp && temp)
                {
                    sign = true;
                }

                if (sign)
                {
                    return NetworkUtility.JsonSerialize("你今天已经签过到了哦！");
                }

                string msg = FunGameService.GetSignInResult(user);
                user.LastTime = DateTime.Now;
                pc.Add("user", user);
                pc.Add("signed", true);
                pc.SaveConfig();
                return NetworkUtility.JsonSerialize(msg);
            }
            else
            {
                return NetworkUtility.JsonSerialize(noSaved);
            }
        }

        [HttpGet("reload")]
        public string Relaod([FromQuery] long? master = null)
        {
            if (master != null && master == GeneralSettings.Master)
            {
                FunGameService.Reload();
                FunGameSimulation.InitFunGameSimulation();
                return NetworkUtility.JsonSerialize("FunGame已重新加载。");
            }
            return NetworkUtility.JsonSerialize("提供的参数不正确。");
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
