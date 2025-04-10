using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.SQLScript.Entity;
using Oshima.Core.Configs;
using Oshima.FunGame.OshimaModules.Characters;
using Oshima.FunGame.OshimaModules.Items;
using Oshima.FunGame.OshimaModules.Regions;
using Oshima.FunGame.OshimaServers.Service;

namespace Oshima.FunGame.WebAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "CustomBearer")]
    [ApiController]
    [Route("[controller]")]
    public class FunGameController(ILogger<FunGameController> logger) : ControllerBase
    {
        private ILogger<FunGameController> Logger { get; set; } = logger;

        private const int drawCardReduce = 2000;
        private const int drawCardReduce_Material = 10;
        private const string noSaved = "你还没有创建存档！请发送【创建存档】创建。";
        private const string refused = "暂时无法使用此指令。";

        [AllowAnonymous]
        [HttpGet("test")]
        public async Task<List<string>> GetTest([FromQuery] bool? isweb = null, [FromQuery] bool? isteam = null, [FromQuery] bool? showall = null)
        {
            bool web = isweb ?? true;
            bool team = isteam ?? false;
            bool all = showall ?? false;
            return await FunGameSimulation.StartSimulationGame(false, web, team, all);
        }

        [AllowAnonymous]
        [HttpGet("stats")]
        public string GetStats([FromQuery] int? id = null)
        {
            if (id != null && id > 0 && id <= FunGameConstant.Characters.Count)
            {
                Character character = FunGameConstant.Characters[Convert.ToInt32(id) - 1];
                if (FunGameSimulation.CharacterStatistics.TryGetValue(character, out CharacterStatistics? stats) && stats != null)
                {
                    StringBuilder builder = new();

                    builder.AppendLine(character.ToStringWithLevelWithOutUser());
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

        [AllowAnonymous]
        [HttpGet("teamstats")]
        public string GetTeamStats([FromQuery] int? id = null)
        {
            if (id != null && id > 0 && id <= FunGameConstant.Characters.Count)
            {
                Character character = FunGameConstant.Characters[Convert.ToInt32(id) - 1];
                if (FunGameSimulation.TeamCharacterStatistics.TryGetValue(character, out CharacterStatistics? stats) && stats != null)
                {
                    StringBuilder builder = new();

                    builder.AppendLine(character.ToStringWithLevelWithOutUser());
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

        [AllowAnonymous]
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
                    builder.AppendLine(character.ToStringWithLevelWithOutUser());
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
                    builder.AppendLine(character.ToStringWithLevelWithOutUser());
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

        [AllowAnonymous]
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
                    builder.AppendLine(character.ToStringWithLevelWithOutUser());
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
                    builder.AppendLine(character.ToStringWithLevelWithOutUser());
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

        [AllowAnonymous]
        [HttpGet("characterinfo")]
        public string GetCharacterInfo([FromQuery] int? id = null)
        {
            if (id != null && id > 0 && id <= FunGameConstant.Characters.Count)
            {
                Character c = FunGameConstant.Characters[Convert.ToInt32(id) - 1].Copy();
                c.Level = General.GameplayEquilibriumConstant.MaxLevel;
                c.NormalAttack.Level = General.GameplayEquilibriumConstant.MaxNormalAttackLevel;
                FunGameService.AddCharacterSkills(c, 1, General.GameplayEquilibriumConstant.MaxSkillLevel, General.GameplayEquilibriumConstant.MaxSuperSkillLevel);

                return NetworkUtility.JsonSerialize(c.GetInfo().Trim());
            }
            return NetworkUtility.JsonSerialize("");
        }

        [AllowAnonymous]
        [HttpGet("skillinfo")]
        public string GetSkillInfo([FromQuery] long? uid = null, [FromQuery] long? id = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
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
                character = FunGameConstant.Characters[1].Copy();
                msg.Add($"技能展示的属性基于演示角色：[ {character} ]");
            }
            IEnumerable<Skill> skills = FunGameConstant.AllSkills;
            if (id != null && FunGameConstant.Characters.Count > 1)
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

        [AllowAnonymous]
        [HttpGet("skillinfoname")]
        public string GetSkillInfo_Name([FromQuery] long? uid = null, [FromQuery] string? name = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
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
                character = FunGameConstant.Characters[1].Copy();
                msg.Add($"技能展示的属性基于演示角色：[ {character} ]");
            }
            IEnumerable<Skill> skills = FunGameConstant.AllSkills;
            if (name != null && FunGameConstant.Characters.Count > 1)
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

        [AllowAnonymous]
        [HttpGet("iteminfo")]
        public string GetItemInfo([FromQuery] long? uid = null, [FromQuery] long? id = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
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
                character = FunGameConstant.Characters[1].Copy();
                msg.Add($"技能展示的属性基于演示角色：[ {character} ]");
            }
            IEnumerable<Item> items = FunGameConstant.AllItems;
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

        [AllowAnonymous]
        [HttpGet("iteminfoname")]
        public string GetItemInfo_Name([FromQuery] long? uid = null, [FromQuery] string? name = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
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
                character = FunGameConstant.Characters[1].Copy();
                msg.Add($"技能展示的属性基于演示角色：[ {character} ]");
            }
            IEnumerable<Item> items = FunGameConstant.AllItems;
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

        [AllowAnonymous]
        [HttpGet("newmagiccard")]
        public string GenerateMagicCard()
        {
            Item i = FunGameService.GenerateMagicCard();
            return NetworkUtility.JsonSerialize(i.ToString(false, true));
        }

        [AllowAnonymous]
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
        public string CreateSaved([FromQuery] long? uid = null, [FromQuery] string? name = null)
        {
            string username = FunGameService.GenerateRandomChineseUserName();
            using SQLHelper? sqlHelper = Factory.OpenFactory.GetSQLHelper();

            if (name != null && sqlHelper != null)
            {
                try
                {
                    if (name.Trim() == "")
                    {
                        return NetworkUtility.JsonSerialize("未提供接入ID！");
                    }
                    Logger.LogInformation("[Reg] 接入ID：{name}", name);
                    sqlHelper.ExecuteDataSet(FunGameService.Select_CheckAutoKey(sqlHelper, name));
                    if (sqlHelper.Success)
                    {
                        return NetworkUtility.JsonSerialize("你已经创建过存档！");
                    }
                    string password = name.Encrypt(name);
                    sqlHelper.NewTransaction();
                    sqlHelper.Execute(UserQuery.Insert_Register(sqlHelper, username, password, "", "", name));
                    if (sqlHelper.Result == SQLResult.Success)
                    {
                        sqlHelper.ExecuteDataSet(FunGameService.Select_CheckAutoKey(sqlHelper, name));
                        if (sqlHelper.Success)
                        {
                            User user = Factory.GetUser(sqlHelper.DataSet);
                            sqlHelper.Commit();
                            user.Inventory.Credits = 5000;
                            user.Inventory.Characters.Add(new CustomCharacter(FunGameConstant.CustomCharacterId, username, nickname: username));
                            FunGameConstant.UserIdAndUsername[user.Id] = user;
                            PluginConfig pc = new("saved", user.Id.ToString());
                            pc.LoadConfig();
                            pc.Add("user", user);
                            pc.SaveConfig();
                            return NetworkUtility.JsonSerialize($"创建存档成功！你的昵称是【{username}】。");
                        }
                        else
                        {
                            return NetworkUtility.JsonSerialize("无法处理注册，创建存档失败！");
                        }
                    }
                    else
                    {
                        sqlHelper.Rollback();
                    }
                }
                catch (Exception e)
                {
                    sqlHelper.Rollback();
                    Logger.LogError(e, "Error: ");
                }
                return NetworkUtility.JsonSerialize("无法处理注册，创建存档失败！");
            }
            else if (uid != null && uid != 0)
            {
                PluginConfig pc = new("saved", uid.Value.ToString());
                pc.LoadConfig();

                if (pc.Count == 0)
                {
                    User user = Factory.GetUser(uid.Value, username, DateTime.Now, DateTime.Now, uid + "@qq.com", username);
                    user.Inventory.Credits = 5000;
                    user.Inventory.Characters.Add(new CustomCharacter(uid.Value, username));
                    FunGameConstant.UserIdAndUsername[uid.Value] = user;
                    pc.Add("user", user);
                    pc.SaveConfig();
                    return NetworkUtility.JsonSerialize($"创建存档成功！你的昵称是【{username}】。");
                }
                else
                {
                    return NetworkUtility.JsonSerialize("你已经创建过存档！");
                }
            }
            return NetworkUtility.JsonSerialize("创建存档失败！");
        }

        [HttpPost("restoresaved")]
        public string RestoreSaved([FromQuery] long? uid = null)
        {
            //long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));

            //PluginConfig pc = new("saved", userid.ToString());
            //pc.LoadConfig();

            //if (pc.Count > 0)
            //{
            //    User user = FunGameService.GetUser(pc);
            //    user.Inventory.Credits = 5000;
            //    user.Inventory.Materials = 0;
            //    user.Inventory.Characters.Clear();
            //    user.Inventory.Items.Clear();
            //    user.Inventory.Characters.Add(new CustomCharacter(FunGameConstant.CustomCharacterId, user.Username));
            //    user.LastTime = DateTime.Now;
            //    pc.Add("user", user);
            //    pc.SaveConfig();
            //    return NetworkUtility.JsonSerialize($"你的存档已还原成功。");
            //}
            //else
            //{
            //    return NetworkUtility.JsonSerialize(noSaved);
            //}
            return NetworkUtility.JsonSerialize($"无法还原用户 {uid} 的存档，此功能维护中。");
        }

        [HttpPost("showsaved")]
        public string ShowSaved([FromQuery] long? uid = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);

                StringBuilder builder = new();
                builder.AppendLine($"☆★☆ {user.Username}的存档信息 ☆★☆");
                builder.AppendLine($"UID：{user.Id}");
                builder.AppendLine($"{General.GameplayEquilibriumConstant.InGameCurrency}：{user.Inventory.Credits:0.00}");
                builder.AppendLine($"{General.GameplayEquilibriumConstant.InGameMaterial}：{user.Inventory.Materials:0.00}");
                builder.AppendLine($"角色数量：{user.Inventory.Characters.Count}");
                builder.AppendLine($"主战角色：{user.Inventory.MainCharacter.ToStringWithLevelWithOutUser()}");
                Character[] squad = [.. user.Inventory.Characters.Where(c => user.Inventory.Squad.Contains(c.Id))];
                Dictionary<Character, int> characters = user.Inventory.Characters
                    .Select((character, index) => new { character, index })
                    .ToDictionary(x => x.character, x => x.index + 1);
                builder.AppendLine($"小队成员：{(squad.Length > 0 ? string.Join(" / ", squad.Select(c => $"[#{characters[c]}]{c.NickName}({c.Level})")) : "空")}");
                if (user.Inventory.Training.Count > 0)
                {
                    builder.AppendLine($"正在练级：{string.Join(" / ", user.Inventory.Characters.Where(c => user.Inventory.Training.ContainsKey(c.Id)).Select(c => c.ToStringWithLevelWithOutUser()))}");
                }
                builder.AppendLine($"物品数量：{user.Inventory.Items.Count}");
                long clubid = 0;
                if (pc.TryGetValue("club", out object? value) && long.TryParse(value.ToString(), out long temp))
                {
                    clubid = temp;
                }
                EntityModuleConfig<Club> emc = new("clubs", clubid.ToString());
                emc.LoadConfig();
                Club? club = emc.Get("club");
                if (club != null)
                {
                    builder.AppendLine($"所属社团：{club.Name} [{club.Prefix}]");
                }
                else
                {
                    builder.AppendLine($"所属社团：无");
                }
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
        public string ReName([FromQuery] long? uid = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));

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
                user.NickName = user.Username;
                if (user.Inventory.Characters.FirstOrDefault(c => c.Id == FunGameConstant.CustomCharacterId) is Character character)
                {
                    character.Name = user.Username;
                    character.NickName = user.NickName;
                }
                if (user.Inventory.Name.EndsWith("的库存"))
                {
                    user.Inventory.Name = user.Username + "的库存";
                }
                FunGameConstant.UserIdAndUsername[user.Id] = user;
                user.LastTime = DateTime.Now;
                pc.Add("user", user);
                pc.SaveConfig();
                return NetworkUtility.JsonSerialize($"消耗 {reduce} {General.GameplayEquilibriumConstant.InGameCurrency}，你的新昵称是【{user.Username}】");
            }
            else
            {
                return NetworkUtility.JsonSerialize(noSaved);
            }
        }

        [HttpPost("randomcustom")]
        public string RandomCustomCharacter([FromQuery] long? uid = null, [FromQuery] bool? confirm = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
            bool isConfirm = confirm ?? false;

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            EntityModuleConfig<Character> emc = new("randomcustom", userid.ToString());
            emc.LoadConfig();

            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);
                if (user.Inventory.Characters.FirstOrDefault(c => c.Id == FunGameConstant.CustomCharacterId) is Character character)
                {
                    PrimaryAttribute oldPA = character.PrimaryAttribute;
                    double oldHP = character.InitialHP;
                    double oldMP = character.InitialMP;
                    double oldATK = character.InitialATK;
                    double oldSTR = character.InitialSTR;
                    double oldAGI = character.InitialAGI;
                    double oldINT = character.InitialINT;
                    double oldSTRG = character.STRGrowth;
                    double oldAGIG = character.AGIGrowth;
                    double oldINTG = character.INTGrowth;
                    double oldSPD = character.InitialSPD;
                    double oldHR = character.InitialHR;
                    double oldMR = character.InitialMR;
                    Character? newCustom = emc.Count > 0 ? emc.Get("newCustom") : null;

                    if (isConfirm)
                    {
                        if (newCustom != null)
                        {
                            character.PrimaryAttribute = newCustom.PrimaryAttribute;
                            character.InitialHP = newCustom.InitialHP;
                            character.InitialMP = newCustom.InitialMP;
                            character.InitialATK = newCustom.InitialATK;
                            character.InitialSTR = newCustom.InitialSTR;
                            character.InitialAGI = newCustom.InitialAGI;
                            character.InitialINT = newCustom.InitialINT;
                            character.STRGrowth = newCustom.STRGrowth;
                            character.AGIGrowth = newCustom.AGIGrowth;
                            character.INTGrowth = newCustom.INTGrowth;
                            character.InitialSPD = newCustom.InitialSPD;
                            character.InitialHR = newCustom.InitialHR;
                            character.InitialMR = newCustom.InitialMR;
                            user.LastTime = DateTime.Now;
                            pc.Add("user", user);
                            pc.SaveConfig();
                            emc.Clear();
                            emc.SaveConfig();
                            return NetworkUtility.JsonSerialize($"你已完成重随属性确认，新的自建角色属性如下：\r\n" +
                                $"核心属性：{CharacterSet.GetPrimaryAttributeName(oldPA)} => {CharacterSet.GetPrimaryAttributeName(character.PrimaryAttribute)}\r\n" +
                                $"初始生命：{oldHP} => {character.InitialHP}\r\n" +
                                $"初始魔法：{oldMP} => {character.InitialMP}\r\n" +
                                $"初始攻击：{oldATK} => {character.InitialATK}\r\n" +
                                $"初始力量：{oldSTR}（+{oldSTRG}/Lv）=> {character.InitialSTR}（+{character.STRGrowth}/Lv）\r\n" +
                                $"初始敏捷：{oldAGI}（+{oldAGIG}/Lv）=> {character.InitialAGI}（+{character.AGIGrowth}/Lv）\r\n" +
                                $"初始智力：{oldINT}（+{oldINTG}/Lv）=> {character.InitialINT}（+{character.INTGrowth}/Lv）\r\n" +
                                $"初始速度：{oldSPD} => {character.InitialSPD}\r\n" +
                                $"生命回复：{oldHR} => {character.InitialHR}\r\n" +
                                $"魔法回复：{oldMR} => {character.InitialMR}\r\n");
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
                            newCustom = new CustomCharacter(FunGameConstant.CustomCharacterId, "");
                            FunGameService.SetCharacterPrimaryAttribute(newCustom);
                            user.LastTime = DateTime.Now;
                            pc.Add("user", user);
                            pc.SaveConfig();
                            emc.Add("newCustom", newCustom);
                            emc.SaveConfig();
                            return NetworkUtility.JsonSerialize($"消耗 {reduce} {General.GameplayEquilibriumConstant.InGameMaterial}，获取到重随属性预览如下：\r\n" +
                                $"核心属性：{CharacterSet.GetPrimaryAttributeName(oldPA)} => {CharacterSet.GetPrimaryAttributeName(newCustom.PrimaryAttribute)}\r\n" +
                                $"初始生命：{oldHP} => {newCustom.InitialHP}\r\n" +
                                $"初始魔法：{oldMP} => {newCustom.InitialMP}\r\n" +
                                $"初始攻击：{oldATK} => {newCustom.InitialATK}\r\n" +
                                $"初始力量：{oldSTR}（+{oldSTRG}/Lv）=> {newCustom.InitialSTR}（+{newCustom.STRGrowth}/Lv）\r\n" +
                                $"初始敏捷：{oldAGI}（+{oldAGIG}/Lv）=> {newCustom.InitialAGI}（+{newCustom.AGIGrowth}/Lv）\r\n" +
                                $"初始智力：{oldINT}（+{oldINTG}/Lv）=> {newCustom.InitialINT}（+{newCustom.INTGrowth}/Lv）\r\n" +
                                $"初始速度：{oldSPD} => {newCustom.InitialSPD}\r\n" +
                                $"生命回复：{oldHR} => {newCustom.InitialHR}\r\n" +
                                $"魔法回复：{oldMR} => {newCustom.InitialMR}\r\n" +
                                $"请发送【确认角色重随】来确认更新，或者发送【取消角色重随】来取消操作。");
                        }
                        else if (newCustom.Id == FunGameConstant.CustomCharacterId)
                        {
                            return NetworkUtility.JsonSerialize($"你已经有一个待确认的重随属性如下：\r\n" +
                                $"核心属性：{CharacterSet.GetPrimaryAttributeName(oldPA)} => {CharacterSet.GetPrimaryAttributeName(newCustom.PrimaryAttribute)}\r\n" +
                                $"初始生命：{oldHP} => {newCustom.InitialHP}\r\n" +
                                $"初始魔法：{oldMP} => {newCustom.InitialMP}\r\n" +
                                $"初始攻击：{oldATK} => {newCustom.InitialATK}\r\n" +
                                $"初始力量：{oldSTR}（+{oldSTRG}/Lv）=> {newCustom.InitialSTR}（+{newCustom.STRGrowth}/Lv）\r\n" +
                                $"初始敏捷：{oldAGI}（+{oldAGIG}/Lv）=> {newCustom.InitialAGI}（+{newCustom.AGIGrowth}/Lv）\r\n" +
                                $"初始智力：{oldINT}（+{oldINTG}/Lv）=> {newCustom.InitialINT}（+{newCustom.INTGrowth}/Lv）\r\n" +
                                $"初始速度：{oldSPD} => {newCustom.InitialSPD}\r\n" +
                                $"生命回复：{oldHR} => {newCustom.InitialHR}\r\n" +
                                $"魔法回复：{oldMR} => {newCustom.InitialMR}\r\n" +
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
        public string CancelRandomCustomCharacter([FromQuery] long? uid = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));

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
        public string GetInventoryInfo([FromQuery] long? uid = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));

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
        public List<string> GetInventoryInfo2([FromQuery] long? uid = null, [FromQuery] int? page = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
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
                int maxPage = (int)Math.Ceiling((double)total / FunGameConstant.ItemsPerPage2);
                if (maxPage < 1) maxPage = 1;
                if (showPage <= maxPage)
                {
                    List<object> inventory = [.. characters, .. items];
                    Dictionary<int, object> dict = inventory.Select((obj, index) => new { Index = index + 1, Value = obj }).ToDictionary(k => k.Index, v => v.Value);
                    List<int> seq = [.. FunGameService.GetPage(dict.Keys, showPage, FunGameConstant.ItemsPerPage2)];
                    bool showCharacter = true;
                    bool showItem = true;
                    int characterCount = 0;
                    int itemCount = 0;

                    int prevSequence = dict.Take((showPage - 1) * FunGameConstant.ItemsPerPage2).Count();

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
        public List<string> GetInventoryInfo3([FromQuery] long? uid = null, [FromQuery] int? page = null, [FromQuery] int? order = null, [FromQuery] int? orderqty = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
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

                int maxPage = (int)Math.Ceiling((double)itemCategory.Count / FunGameConstant.ItemsPerPage1);
                if (maxPage < 1) maxPage = 1;
                if (showPage <= maxPage)
                {
                    List<string> keys = [.. FunGameService.GetPage(itemCategory.Keys, showPage, FunGameConstant.ItemsPerPage1)];
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
                        IEnumerable<Item> itemsEquipable = objs.Where(i => i.IsEquipment && i.Character is null);
                        string itemsEquipableIndex = string.Join("，", itemsEquipable.Select(i => items.IndexOf(i) + 1));
                        if (itemsEquipable.Count() > 10)
                        {
                            itemsEquipableIndex = string.Join("，", itemsEquipable.Take(10).Select(i => items.IndexOf(i) + 1)) + "，...";
                        }
                        IEnumerable<Item> itemsSellable = objs.Where(i => i.IsSellable);
                        string itemsSellableIndex = string.Join("，", itemsSellable.Select(i => items.IndexOf(i) + 1));
                        if (itemsSellable.Count() > 10)
                        {
                            itemsSellableIndex = string.Join("，", itemsSellable.Take(10).Select(i => items.IndexOf(i) + 1)) + "，...";
                        }
                        IEnumerable<Item> itemsTradable = objs.Where(i => i.IsTradable);
                        string itemsTradableIndex = string.Join("，", itemsTradable.Select(i => items.IndexOf(i) + 1));
                        if (itemsTradable.Count() > 10)
                        {
                            itemsTradableIndex = string.Join("，", itemsTradable.Take(10).Select(i => items.IndexOf(i) + 1)) + "，...";
                        }
                        str += $"物品序号：{itemsIndex}\r\n";
                        if (itemsEquipableIndex != "") str += $"可装备序号：{itemsEquipableIndex}\r\n";
                        if (itemsSellableIndex != "") str += $"可出售序号：{itemsSellableIndex}\r\n";
                        if (itemsTradableIndex != "") str += $"可交易序号：{itemsTradableIndex}\r\n";
                        str += $"拥有数量：{objs.Count}（" + (first.IsEquipment ? $"可装备数量：{itemsEquipable.Count()}，" : "") +
                            (FunGameConstant.ItemCanUsed.Contains(first.ItemType) ? $"可使用数量：{objs.Count(i => i.RemainUseTimes > 0)}，" : "") +
                            $"可出售数量：{itemsSellable.Count()}，可交易数量：{itemsTradable.Count()}）";
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
        public List<string> GetInventoryInfo4([FromQuery] long? uid = null, [FromQuery] int? page = null, [FromQuery] int? type = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
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

                int maxPage = (int)Math.Ceiling((double)itemCategory.Count / FunGameConstant.ItemsPerPage1);
                if (maxPage < 1) maxPage = 1;
                if (showPage <= maxPage)
                {
                    List<string> keys = [.. FunGameService.GetPage(itemCategory.Keys, showPage, FunGameConstant.ItemsPerPage1)];
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
                        IEnumerable<Item> itemsEquipable = objs.Where(i => i.IsEquipment && i.Character is null);
                        string itemsEquipableIndex = string.Join("，", itemsEquipable.Select(i => items.IndexOf(i) + 1));
                        if (itemsEquipable.Count() > 10)
                        {
                            itemsEquipableIndex = string.Join("，", itemsEquipable.Take(10).Select(i => items.IndexOf(i) + 1)) + "，...";
                        }
                        IEnumerable<Item> itemsSellable = objs.Where(i => i.IsSellable);
                        string itemsSellableIndex = string.Join("，", itemsSellable.Select(i => items.IndexOf(i) + 1));
                        if (itemsSellable.Count() > 10)
                        {
                            itemsSellableIndex = string.Join("，", itemsSellable.Take(10).Select(i => items.IndexOf(i) + 1)) + "，...";
                        }
                        IEnumerable<Item> itemsTradable = objs.Where(i => i.IsTradable);
                        string itemsTradableIndex = string.Join("，", itemsTradable.Select(i => items.IndexOf(i) + 1));
                        if (itemsTradable.Count() > 10)
                        {
                            itemsTradableIndex = string.Join("，", itemsTradable.Take(10).Select(i => items.IndexOf(i) + 1)) + "，...";
                        }
                        str += $"物品序号：{itemsIndex}\r\n";
                        if (itemsEquipableIndex != "") str += $"可装备序号：{itemsEquipableIndex}\r\n";
                        if (itemsSellableIndex != "") str += $"可出售序号：{itemsSellableIndex}\r\n";
                        if (itemsTradableIndex != "") str += $"可交易序号：{itemsTradableIndex}\r\n";
                        str += $"拥有数量：{objs.Count}（" + (first.IsEquipment ? $"可装备数量：{itemsEquipable.Count()}，" : "") +
                            (FunGameConstant.ItemCanUsed.Contains(first.ItemType) ? $"可使用数量：{objs.Count(i => i.RemainUseTimes > 0)}，" : "") +
                            $"可出售数量：{itemsSellable.Count()}，可交易数量：{itemsTradable.Count()}）";
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
        public List<string> GetInventoryInfo5([FromQuery] long? uid = null, [FromQuery] int? page = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
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
                int maxPage = (int)Math.Ceiling((double)total / 15);
                if (maxPage < 1) maxPage = 1;
                if (showPage <= maxPage)
                {
                    List<object> inventory = [.. characters];
                    Dictionary<int, object> dict = inventory.Select((obj, index) => new { Index = index + 1, Value = obj }).ToDictionary(k => k.Index, v => v.Value);
                    List<int> seq = [.. FunGameService.GetPage(dict.Keys, showPage, 15)];
                    bool showCharacter = true;
                    int characterCount = 0;

                    int prevSequence = dict.Take((showPage - 1) * 15).Count();

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
        public string NewCustomCharacter([FromQuery] long? uid = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);
                if (user.Inventory.Characters.Any(c => c.Id == FunGameConstant.CustomCharacterId))
                {
                    return NetworkUtility.JsonSerialize($"你已经拥有一个自建角色【{user.Username}】，无法再创建！");
                }
                else
                {
                    user.Inventory.Characters.Add(new CustomCharacter(FunGameConstant.CustomCharacterId, user.Username));
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
        public string DrawCard([FromQuery] long? uid = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));

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
        public List<string> DrawCards([FromQuery] long? uid = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));

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
        public string DrawCard_Material([FromQuery] long? uid = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));

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
        public List<string> DrawCards_Material([FromQuery] long? uid = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));

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
        public string ExchangeCredits([FromQuery] long? uid = null, [FromQuery] double? materials = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
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
        public string GetCharacterInfoFromInventory([FromQuery] long? uid = null, [FromQuery] int? seq = null, [FromQuery] bool? simple = null)
        {
            try
            {
                long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
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
        public string GetCharacterSkills([FromQuery] long? uid = null, [FromQuery] int? seq = null)
        {
            try
            {
                long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
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
        public string GetCharacterItems([FromQuery] long? uid = null, [FromQuery] int? seq = null)
        {
            try
            {
                long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
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
        public string GetItemInfoFromInventory([FromQuery] long? uid = null, [FromQuery] int? seq = null)
        {
            try
            {
                long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
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

        [HttpPost("showiteminfoname")]
        public string GetItemInfoFromInventory_Name([FromQuery] long? uid = null, [FromQuery] string? name = null, [FromQuery] int? page = null)
        {
            try
            {
                long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
                string itemName = name ?? "";
                int showPage = page ?? 1;
                if (showPage <= 0) showPage = 1;

                PluginConfig pc = new("saved", userid.ToString());
                pc.LoadConfig();

                if (pc.Count > 0)
                {
                    User user = FunGameService.GetUser(pc);

                    var objs = user.Inventory.Items.Select((item, index) => new { item, index })
                        .Where(obj => obj.item.Name == itemName);

                    Dictionary<int, Item> items = [];
                    if (objs.Any())
                    {
                        items = objs.ToDictionary(d => d.index + 1, d => d.item);
                    }
                    else
                    {
                        return NetworkUtility.JsonSerialize($"你库存中没有任何名为【{itemName}】的物品！");
                    }

                    int total = items.Count;
                    int maxPage = (int)Math.Ceiling((double)total / 100);
                    if (maxPage < 1) maxPage = 1;
                    if (showPage <= maxPage)
                    {
                        IEnumerable<int> showItems = FunGameService.GetPage(items.Keys, showPage, 100);
                        if (showItems.Any())
                        {
                            int itemIndex = showItems.First();
                            Item item = items[itemIndex];
                            string str = $"☆--- [ {item.Name} ] ---☆\r\n";
                            str += ItemSet.GetQualityTypeName(item.QualityType) + " " +
                                (item.WeaponType == WeaponType.None ? ItemSet.GetItemTypeName(item.ItemType) : ItemSet.GetItemTypeName(item.ItemType) + "-" + ItemSet.GetWeaponTypeName(item.WeaponType));
                            str += $"\r\n物品描述：{item.Description}\r\n";

                            string itemsIndex = string.Join("，", items.Keys);

                            var itemsEquipabled = items.Where(kv => kv.Value.Character != null);
                            var itemsEquipable = items.Where(kv => kv.Value.Character is null);
                            string itemsEquipabledIndex = "";
                            string itemsEquipableIndex = "";
                            if (item.IsEquipment)
                            {
                                itemsEquipabledIndex = string.Join("，", itemsEquipabled.Select(kv => kv.Key));
                                itemsEquipableIndex = string.Join("，", itemsEquipable.Select(kv => kv.Key));
                            }

                            var itemsCanUsed = items.Where(kv => kv.Value.RemainUseTimes > 0);
                            string itemsCanUsedIndex = "";
                            if (FunGameConstant.ItemCanUsed.Contains(item.ItemType))
                            {
                                itemsCanUsedIndex = string.Join("，", itemsCanUsed.Select(kv => kv.Key));
                            }

                            var itemsSellable = items.Where(kv => kv.Value.IsSellable);
                            string itemsSellableIndex = string.Join("，", itemsSellable.Select(kv => kv.Key));

                            var itemsTradable = items.Where(kv => kv.Value.IsTradable);
                            string itemsTradableIndex = string.Join("，", itemsTradable.Select(kv => kv.Key));

                            str += $"物品序号：{itemsIndex}\r\n";
                            if (itemsEquipabledIndex != "") str += $"已装备序号：{itemsEquipabledIndex}\r\n";
                            if (itemsEquipableIndex != "") str += $"可装备序号：{itemsEquipableIndex}\r\n";
                            if (itemsCanUsedIndex != "") str += $"可使用序号：{itemsCanUsedIndex}\r\n";
                            if (itemsSellableIndex != "") str += $"可出售序号：{itemsSellableIndex}\r\n";
                            if (itemsTradableIndex != "") str += $"可交易序号：{itemsTradableIndex}\r\n";

                            str += $"拥有数量：{items.Count}（";
                            if (item.IsEquipment)
                            {
                                str += $"已装备数量：{itemsEquipabled.Count()}，可装备数量：{itemsEquipable.Count()}，";
                            }
                            if (FunGameConstant.ItemCanUsed.Contains(item.ItemType))
                            {
                                str += $"可使用数量：{itemsCanUsed.Count()}，";
                            }
                            str += $"可出售数量：{itemsSellable.Count()}，可交易数量：{itemsTradable.Count()}）\r\n";
                            str += $"页数：{showPage} / {maxPage}";
                            return NetworkUtility.JsonSerialize(str.Trim());
                        }
                        else
                        {
                            return NetworkUtility.JsonSerialize($"你库存中没有任何名为【{itemName}】的物品！");
                        }
                    }
                    else
                    {
                        return NetworkUtility.JsonSerialize($"没有这么多页！当前总页数为 {maxPage}，但你请求的是第 {showPage} 页。");
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
        public string EquipItem([FromQuery] long? uid = null, [FromQuery] int? c = null, [FromQuery] int? i = null)
        {
            try
            {
                long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
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
        public string UnEquipItem([FromQuery] long? uid = null, [FromQuery] int? c = null, [FromQuery] int? i = null)
        {
            try
            {
                long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
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
        public async Task<List<string>> FightCustom([FromQuery] long? uid = null, [FromQuery] long? eqq = null, [FromQuery] bool? all = null)
        {
            try
            {
                long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
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
                    return await FunGameActionQueue.NewAndStartGame([user1.Inventory.MainCharacter, user2.Inventory.MainCharacter], false, false, false, false, showAllRound);
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
        public async Task<List<string>> FightCustom2([FromQuery] long? uid = null, [FromQuery] string? name = null, [FromQuery] bool? all = null)
        {
            try
            {
                if (name != null)
                {
                    long enemyid = FunGameConstant.UserIdAndUsername.Where(kv => kv.Value.Username == name).Select(kv => kv.Key).FirstOrDefault();
                    if (enemyid == 0)
                    {
                        return [$"找不到此昵称对应的玩家！"];
                    }
                    return await FightCustom(uid, enemyid, all);
                }
                return [$"决斗发起失败！"];
            }
            catch (Exception e)
            {
                return [e.ToString()];
            }
        }

        [HttpPost("fightcustomteam")]
        public async Task<List<string>> FightCustomTeam([FromQuery] long? uid = null, [FromQuery] long? eqq = null, [FromQuery] bool? all = null)
        {
            try
            {
                long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
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
                    Team team2 = new($"{user2.Username}的小队" + (userid == enemyid ? "2" : ""), squad2);
                    return await FunGameActionQueue.NewAndStartTeamGame([team1, team2], 0, 0, false, false, false, false, showAllRound);
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
        public async Task<List<string>> FightCustomTeam2([FromQuery] long? uid = null, [FromQuery] string? name = null, [FromQuery] bool? all = null)
        {
            try
            {
                if (name != null)
                {
                    long enemyid = FunGameConstant.UserIdAndUsername.Where(kv => kv.Value.Username == name).Select(kv => kv.Key).FirstOrDefault();
                    if (enemyid == 0)
                    {
                        return [$"找不到此昵称对应的玩家！"];
                    }
                    return await FightCustomTeam(uid, enemyid, all);
                }
                return [$"决斗发起失败！"];
            }
            catch (Exception e)
            {
                return [e.ToString()];
            }
        }

        [HttpPost("useitem")]
        public string UseItem([FromQuery] long? uid = null, [FromQuery] int? id = null, [FromBody] int[]? characters = null)
        {
            try
            {
                long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
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
                        if (FunGameConstant.ItemCanUsed.Contains(item.ItemType))
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
        public string UseItem2([FromQuery] long? uid = null, [FromQuery] string? name = null, [FromQuery] int? count = null, [FromBody] int[]? characters = null)
        {
            try
            {
                long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
                string itemName = name ?? "";
                int useCount = count ?? 0;
                if (useCount <= 0)
                {
                    return NetworkUtility.JsonSerialize("数量必须大于0！");
                }
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
        public string UseItem3([FromQuery] long? uid = null, [FromQuery] int? id = null, [FromQuery] int? id2 = null, [FromQuery] bool? c = null)
        {
            try
            {
                long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
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
        public string CharacterLevelUp([FromQuery] long? uid = null, [FromQuery] int? c = null, [FromQuery] int? count = null)
        {
            try
            {
                long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
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
        public string GetLevelBreakNeedy([FromQuery] long? uid = null, [FromQuery] int? id = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
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
        public string CharacterLevelBreak([FromQuery] long? uid = null, [FromQuery] int? c = null)
        {
            try
            {
                long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
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

                    if (FunGameConstant.LevelBreakNeedyList.TryGetValue(originalBreak + 1, out Dictionary<string, int>? needy) && needy != null && needy.Count > 0)
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
        public string CreateItem([FromQuery] long? uid = null, [FromQuery] string? name = null, [FromQuery] int? count = null, [FromQuery] long? target = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
            string itemName = name ?? "";
            int itemCount = count ?? 0;
            if (itemCount <= 0)
            {
                return NetworkUtility.JsonSerialize("数量必须大于0！");
            }
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
                        else if (FunGameConstant.AllItems.FirstOrDefault(i => i.Name == itemName) is Item item)
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
        public string DecomposeItem([FromQuery] long? uid = null, [FromBody] int[]? items = null)
        {
            try
            {
                long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
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
                                QualityType.Gold => 28,
                                QualityType.Red => 21,
                                QualityType.Orange => 15,
                                QualityType.Purple => 10,
                                QualityType.Blue => 6,
                                QualityType.Green => 3,
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
        public string DecomposeItem2([FromQuery] long? uid = null, [FromQuery] string? name = null, [FromQuery] int? count = null)
        {
            try
            {
                long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
                string itemName = name ?? "";
                int useCount = count ?? 0;
                if (useCount <= 0)
                {
                    return NetworkUtility.JsonSerialize("数量必须大于0！");
                }

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
                                    QualityType.Gold => 28,
                                    QualityType.Red => 21,
                                    QualityType.Orange => 15,
                                    QualityType.Purple => 10,
                                    QualityType.Blue => 6,
                                    QualityType.Green => 3,
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
        public string DecomposeItem3([FromQuery] long? uid = null, [FromQuery] int? q = null)
        {
            try
            {
                long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
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
                        QualityType.Gold => 28,
                        QualityType.Red => 21,
                        QualityType.Orange => 15,
                        QualityType.Purple => 10,
                        QualityType.Blue => 6,
                        QualityType.Green => 3,
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
        public string ConflateMagicCardPack([FromQuery] long? uid = null, [FromBody] int[]? items = null)
        {
            try
            {
                long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
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
        public string SetMain([FromQuery] long? uid = null, [FromQuery] int? c = null)
        {
            try
            {
                long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
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
        public string StartTraining([FromQuery] long? uid = null, [FromQuery] int? c = null)
        {
            try
            {
                long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
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
        public string StopTraining([FromQuery] long? uid = null)
        {
            try
            {
                long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));

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
        public string GetTrainingInfo([FromQuery] long? uid = null)
        {
            try
            {
                long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));

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
        public string GetSkillLevelUpNeedy([FromQuery] long? uid = null, [FromQuery] int? c = null, [FromQuery] string? s = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
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
        public string SkillLevelUp([FromQuery] long? uid = null, [FromQuery] int? c = null, [FromQuery] string? s = null)
        {
            try
            {
                long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
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

                            if (FunGameConstant.SkillLevelUpList.TryGetValue(skill.Level + 1, out Dictionary<string, int>? needy) && needy != null && needy.Count > 0)
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
                                        if (character.LevelBreak + 1 < needCount)
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
        public string GetNormalAttackLevelUpNeedy([FromQuery] long? uid = null, [FromQuery] int? c = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
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
        public string NormalAttackLevelUp([FromQuery] long? uid = null, [FromQuery] int? c = null)
        {
            try
            {
                long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
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

                    if (FunGameConstant.NormalAttackLevelUpList.TryGetValue(na.Level + 1, out Dictionary<string, int>? needy) && needy != null && needy.Count > 0)
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
                                if (character.LevelBreak + 1 < needCount)
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
        public async Task<List<string>> FightBoss([FromQuery] long? uid = null, [FromQuery] int? index = null, [FromQuery] bool? all = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
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

                    Character boss2 = CharacterBuilder.Build(boss, false, true, null, FunGameConstant.AllItems, FunGameConstant.AllSkills, false);
                    List<string> msgs = await FunGameActionQueue.NewAndStartGame([user.Inventory.MainCharacter, boss2], false, false, false, false, showAllRound);

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
        public string AddSquad([FromQuery] long? uid = null, [FromQuery] int? c = null)
        {
            try
            {
                long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
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
        public string RemoveSquad([FromQuery] long? uid = null, [FromQuery] int? c = null)
        {
            try
            {
                long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
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
        public string SetSquad([FromQuery] long? uid = null, [FromBody] int[]? c = null)
        {
            try
            {
                long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
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
        public string ClearSquad([FromQuery] long? uid = null, [FromBody] int[]? c = null)
        {
            try
            {
                long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
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
        public string ShowSquad([FromQuery] long? uid = null)
        {
            try
            {
                long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));

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
        public async Task<List<string>> FightBossTeam([FromQuery] long? uid = null, [FromQuery] int? index = null, [FromQuery] bool? all = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
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

                    Character boss2 = CharacterBuilder.Build(boss, false, true, null, FunGameConstant.AllItems, FunGameConstant.AllSkills, false);
                    Team team1 = new($"{user.Username}的小队", squad);
                    Team team2 = new($"Boss", [boss2]);
                    List<string> msgs = await FunGameActionQueue.NewAndStartTeamGame([team1, team2], showAllRound: showAllRound);

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
        public string CheckQuestList([FromQuery] long? uid = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));

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
        public string CheckWorkingQuest([FromQuery] long? uid = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));

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
        public List<string> AcceptQuest([FromQuery] long? uid = null, [FromQuery] int? id = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
            int questid = id ?? 0;

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            List<string> msgs = [];
            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);

                EntityModuleConfig<Quest> quests = new("quests", userid.ToString());
                quests.LoadConfig();
                if (quests.Count > 0 && quests.Values.FirstOrDefault(q => q.Id == questid) is Quest quest)
                {
                    IEnumerable<Quest> workingQuests = quests.Values.Where(q => q.QuestType != QuestType.Progressive && q.Status == QuestState.InProgress);
                    if (workingQuests.Any())
                    {
                        msgs.Add($"你正在进行任务【{string.Join("，【", workingQuests.Select(q => q.Name))}】，无法开始新任务！\r\n{quest}");
                    }
                    else if (quest.Status == QuestState.Completed)
                    {
                        msgs.Add($"任务【{quest.Name}】已经完成了！\r\n{quest}");
                    }
                    else if (quest.Status == QuestState.Settled)
                    {
                        msgs.Add($"任务【{quest.Name}】已经结算并发放奖励了哦！\r\n{quest}");
                    }
                    else
                    {
                        quest.StartTime = DateTime.Now;
                        quest.Status = QuestState.InProgress;
                        if (quest.QuestType == QuestType.Continuous)
                        {
                            // 持续性任务会在持续时间结束后自动完成并结算
                            msgs.Add($"开始任务【{quest.Name}】成功！任务信息如下：\r\n{quest}\r\n预计完成时间：{DateTime.Now.AddMinutes(quest.EstimatedMinutes).ToString(General.GeneralDateTimeFormatChinese)}");
                        }
                        else if (quest.QuestType == QuestType.Immediate)
                        {
                            msgs.Add($"开始任务【{quest.Name}】成功！任务信息如下：\r\n{quest}");
                            // TODO：实现任务逻辑
                            quest.Status = QuestState.Completed;
                            msgs.Add("在任务过程中，你碰巧遇到了米莉，任务直接完成了！");
                        }
                        else
                        {
                            msgs.Add($"开始任务【{quest.Name}】成功！任务信息如下：\r\n{quest}");
                            // TODO：进度条任务需要完成任务的指标，实现任务逻辑
                            quest.Progress = quest.MaxProgress;
                            quest.Status = QuestState.Completed;
                            msgs.Add("在任务过程中，你碰巧遇到了米莉，任务直接完成了！");
                        }
                        quests.SaveConfig();
                    }
                }
                else
                {
                    msgs.Add($"没有找到序号为 {questid} 的任务！请使用【任务列表】指令来检查你的任务列表！");
                }

                user.LastTime = DateTime.Now;
                pc.Add("user", user);
                pc.SaveConfig();

                return msgs;
            }
            else
            {
                return [noSaved];
            }
        }

        [HttpPost("settlequest")]
        public string SettleQuest([FromQuery] long? uid = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));

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
        public string ShowMainCharacterOrSquadStatus([FromQuery] long? uid = null, [FromQuery] bool? squad = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
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
        public string SignIn([FromQuery] long? uid = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);
                bool sign = false;
                int days = 0;
                DateTime lastTime = DateTime.MinValue;
                DateTime newLastTime = DateTime.Now;
                if (pc.TryGetValue("signed", out object? value) && value is bool temp && temp)
                {
                    sign = true;
                }
                if (pc.TryGetValue("days", out value) && int.TryParse(value.ToString(), out int temp2))
                {
                    days = temp2;
                }

                if (pc.TryGetValue("lastTime", out value) && DateTime.TryParse(value.ToString(), out lastTime) && (newLastTime.Date - lastTime.Date).TotalDays > 1)
                {
                    days = 0;
                }

                if (sign)
                {
                    return NetworkUtility.JsonSerialize($"你今天已经签过到了哦！" +
                        (lastTime != DateTime.MinValue ? $"\r\n你上一次签到时间：{lastTime.ToString(General.GeneralDateTimeFormatChinese)}，连续签到：{days} 天。" : ""));
                }

                string msg = FunGameService.GetSignInResult(user, days);
                user.LastTime = newLastTime;
                pc.Add("user", user);
                pc.Add("signed", true);
                pc.Add("days", days + 1);
                pc.Add("lastTime", newLastTime);
                pc.SaveConfig();
                return NetworkUtility.JsonSerialize(msg);
            }
            else
            {
                return NetworkUtility.JsonSerialize(noSaved);
            }
        }

        [HttpPost("joinclub")]
        public string JoinClub([FromQuery] long? uid = null, [FromQuery] long? id = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
            long clubid = id ?? 0;

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);
                if (pc.TryGetValue("club", out object? value) && long.TryParse(value.ToString(), out long userClub) && userClub != 0)
                {
                    return NetworkUtility.JsonSerialize($"你需要先退出当前社团才可以加入新社团。");
                }

                EntityModuleConfig<Club> emc = new("clubs", clubid.ToString());
                emc.LoadConfig();
                Club? club = emc.Get("club");
                if (club is null)
                {
                    return NetworkUtility.JsonSerialize($"不存在编号为 {clubid} 的社团！");
                }

                if (!club.IsPublic)
                {
                    return NetworkUtility.JsonSerialize($"社团 [ {club.Name} ] 未公开，只能通过邀请加入。");
                }

                string msg = "";

                if (club.IsNeedApproval || club.Applicants.ContainsKey(userid))
                {
                    club.ApplicationTime[userid] = DateTime.Now;
                    club.Applicants[userid] = user;
                    msg += $"已向社团 [ {club.Name} ] 提交加入申请！";
                }
                else
                {
                    club.MemberJoinTime[userid] = DateTime.Now;
                    club.Members[userid] = user;
                    msg += $"加入社团 [ {club.Name} ] 成功！";
                    pc.Add("club", clubid);
                }
                emc.Add("club", club);
                emc.SaveConfig();

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

        [HttpPost("quitclub")]
        public string QuitClub([FromQuery] long? uid = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);
                long clubid = 0;
                if (pc.TryGetValue("club", out object? value) && long.TryParse(value.ToString(), out long temp))
                {
                    clubid = temp;
                }

                if (clubid == 0)
                {
                    return NetworkUtility.JsonSerialize($"你当前没有加入任何社团！");
                }

                EntityModuleConfig<Club> emc = new("clubs", clubid.ToString());
                emc.LoadConfig();
                Club? club = emc.Get("club");
                if (club is null)
                {
                    return NetworkUtility.JsonSerialize($"不存在编号为 {clubid} 的社团！");
                }

                if (club.Master?.Id == userid)
                {
                    return NetworkUtility.JsonSerialize($"你是社团的社长，不能退出社团，请转让社长或【解散社团】！");
                }

                if (!club.Members.Remove(userid))
                {
                    return NetworkUtility.JsonSerialize($"你不是此社团的成员，请联系管理员处理。");
                }

                club.MemberJoinTime.Remove(userid);
                emc.Add("club", club);
                emc.SaveConfig();

                string msg = $"退出社团 [ {club.Name} ] 成功！";
                user.LastTime = DateTime.Now;
                pc.Add("user", user);
                pc.Add("club", 0);
                pc.SaveConfig();
                return NetworkUtility.JsonSerialize(msg);
            }
            else
            {
                return NetworkUtility.JsonSerialize(noSaved);
            }
        }

        [HttpPost("createclub")]
        public string CreateClub([FromQuery] long? uid = null, [FromQuery] bool? @public = null, [FromQuery] string? prefix = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
            bool isPublic = @public ?? false;
            string clubPrefix = prefix ?? "";

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);
                if (pc.TryGetValue("club", out object? value) && long.TryParse(value.ToString(), out long userClub) && userClub != 0)
                {
                    return NetworkUtility.JsonSerialize($"你需要先退出当前社团才可以创建新社团。");
                }

                string pattern = @"^[a-zA-Z-_=+*%#^~.?!;:'"",]{3,4}$";
                if (!Regex.IsMatch(clubPrefix, pattern))
                {
                    return NetworkUtility.JsonSerialize($"社团的前缀只能包含总共3-4个英文字母和数字、允许的特殊字符，此前缀不满足条件。");
                }

                HashSet<long> clubids = [];
                string directoryPath = $@"{AppDomain.CurrentDomain.BaseDirectory}configs/clubs";
                if (Directory.Exists(directoryPath))
                {
                    string[] filePaths = Directory.GetFiles(directoryPath);
                    foreach (string filePath in filePaths)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(filePath);
                        if (long.TryParse(fileName, out long id))
                        {
                            clubids.Add(id);
                        }
                    }
                }

                long clubid = clubids.Count > 0 ? clubids.Max() + 1 : 1;
                Club club = new()
                {
                    Id = clubid,
                    Guid = Guid.NewGuid(),
                    Name = FunGameService.GenerateRandomChineseName(),
                    Master = user,
                    Prefix = clubPrefix,
                    Members = new()
                    {
                        { user.Id, user }
                    },
                    MemberJoinTime = new()
                    {
                        { user.Id, DateTime.Now }
                    },
                    IsPublic = isPublic,
                    IsNeedApproval = false
                };

                EntityModuleConfig<Club> emc = new("clubs", clubid.ToString());
                emc.LoadConfig();
                emc.Add("club", club);
                emc.SaveConfig();

                string msg = $"创建社团 [ {club.Name} ] （编号 {clubid}）成功！";
                user.LastTime = DateTime.Now;
                pc.Add("user", user);
                pc.Add("club", clubid);
                pc.SaveConfig();
                return NetworkUtility.JsonSerialize(msg);
            }
            else
            {
                return NetworkUtility.JsonSerialize(noSaved);
            }
        }

        [HttpPost("showclubinfo")]
        public string ShowClubInfo([FromQuery] long? uid = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);

                string msg;
                long clubid = 0;
                if (pc.TryGetValue("club", out object? value) && long.TryParse(value.ToString(), out long temp))
                {
                    clubid = temp;
                }
                EntityModuleConfig<Club> emc = new("clubs", clubid.ToString());
                emc.LoadConfig();
                Club? club = emc.Get("club");
                if (club != null)
                {
                    string master = "无";
                    if (FunGameConstant.UserIdAndUsername.TryGetValue(club.Master?.Id ?? 0, out User? user2) && user2 != null)
                    {
                        master = user2.Username;
                    }

                    StringBuilder builer = new();
                    builer.AppendLine($"☆--- {user.Username}的社团信息 ---☆");
                    builer.AppendLine($"所属社团：{club.Name} [{club.Prefix}]");
                    builer.AppendLine($"社团编号：{club.Id}");
                    builer.AppendLine($"社团社长：{master}");
                    builer.AppendLine($"是否公开：{(club.IsPublic ? "公开" : "私密")}");
                    if (club.IsPublic) builer.AppendLine($"加入规则：{(club.IsNeedApproval ? "需要批准" : "直接加入")}");
                    builer.AppendLine($"成员数量：{club.Members.Count}");
                    if (club.Master?.Id == userid || club.Admins.ContainsKey(userid))
                    {
                        builer.AppendLine($"管理员数量：{club.Admins.Count}");
                        builer.AppendLine($"申请人数量：{club.Applicants.Count}");
                        builer.AppendLine($"社团基金：{club.ClubPoins}");
                        if (club.Master?.Id == userid)
                        {
                            builer.AppendLine("你是此社团的社长");
                        }
                        if (club.Admins.ContainsKey(userid))
                        {
                            builer.AppendLine("你是此社团的管理员");
                        }
                    }
                    builer.AppendLine($"社团描述：{club.Description}");
                    msg = builer.ToString().Trim();
                }
                else
                {
                    msg = $"你目前还没有加入任何社团。";
                }

                return NetworkUtility.JsonSerialize(msg);
            }
            else
            {
                return NetworkUtility.JsonSerialize(noSaved);
            }
        }

        [HttpPost("showclubmemberlist")]
        public string ShowClubMemberList([FromQuery] long? uid = null, [FromQuery] int? type = null, [FromQuery] int? page = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
            int showType = type ?? 0;
            int showPage = page ?? 1;
            if (showPage <= 0) showPage = 1;

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);

                string msg;
                long clubid = 0;
                if (pc.TryGetValue("club", out object? value) && long.TryParse(value.ToString(), out long temp))
                {
                    clubid = temp;
                }
                EntityModuleConfig<Club> emc = new("clubs", clubid.ToString());
                emc.LoadConfig();
                Club? club = emc.Get("club");
                if (club != null)
                {
                    StringBuilder builer = new();
                    int count;
                    switch (showType)
                    {
                        case 1:
                            builer.AppendLine($"☆--- 社团 [ {club.Name} ] 管理员列表 ---☆");
                            count = 1;
                            List<long> admins = [];
                            if (club.Master != null && club.Master.Id != 0)
                            {
                                admins.Add(club.Master.Id);
                            }
                            admins.AddRange(club.Admins.Keys);

                            int maxPage = (int)Math.Ceiling((double)admins.Count / FunGameConstant.ItemsPerPage2);
                            if (maxPage < 1) maxPage = 1;
                            if (showPage <= maxPage)
                            {
                                admins = [.. FunGameService.GetPage(admins, showPage, FunGameConstant.ItemsPerPage2)];
                                foreach (long uid2 in admins)
                                {
                                    if (FunGameConstant.UserIdAndUsername.TryGetValue(uid2, out User? user2) && user2 != null)
                                    {
                                        builer.AppendLine($"{count}.");
                                        builer.AppendLine($"UID：{user2.Id}");
                                        builer.AppendLine($"玩家昵称：{user2.Username}");
                                        builer.AppendLine($"加入时间：{club.MemberJoinTime[user2.Id].ToString(General.GeneralDateTimeFormatChinese)}");
                                    }
                                    count++;
                                }
                                builer.AppendLine($"页数：{showPage} / {maxPage}");
                            }
                            else
                            {
                                NetworkUtility.JsonSerialize($"没有这么多页！当前总页数为 {maxPage}，但你请求的是第 {showPage} 页。");
                            }
                            break;
                        case 2:
                            if (club.Master?.Id == user.Id || club.Admins.ContainsKey(user.Id))
                            {
                                builer.AppendLine($"☆--- 社团 [ {club.Name} ] 申请人列表 ---☆");
                                count = 1;
                                maxPage = (int)Math.Ceiling((double)club.Applicants.Count / FunGameConstant.ItemsPerPage2);
                                if (maxPage < 1) maxPage = 1;
                                if (showPage <= maxPage)
                                {
                                    IEnumerable<long> applicants = FunGameService.GetPage(club.Applicants.Keys, showPage, FunGameConstant.ItemsPerPage2);
                                    foreach (long uid2 in applicants)
                                    {
                                        if (FunGameConstant.UserIdAndUsername.TryGetValue(uid2, out User? user2) && user2 != null)
                                        {
                                            builer.AppendLine($"{count}.");
                                            builer.AppendLine($"UID：{user2.Id}");
                                            builer.AppendLine($"玩家昵称：{user2.Username}");
                                            builer.AppendLine($"申请时间：{club.ApplicationTime[user2.Id].ToString(General.GeneralDateTimeFormatChinese)}");
                                        }
                                        count++;
                                    }
                                    builer.AppendLine($"页数：{showPage} / {maxPage}");
                                }
                                else
                                {
                                    NetworkUtility.JsonSerialize($"没有这么多页！当前总页数为 {maxPage}，但你请求的是第 {showPage} 页。");
                                }
                            }
                            else
                            {
                                builer.Append("你没有权限查看这个列表！");
                            }
                            break;
                        case 0:
                        default:
                            builer.AppendLine($"☆--- 社团 [ {club.Name} ] 成员列表 ---☆");
                            count = 1;
                            maxPage = (int)Math.Ceiling((double)club.Members.Count / FunGameConstant.ItemsPerPage2);
                            if (maxPage < 1) maxPage = 1;
                            if (showPage <= maxPage)
                            {
                                IEnumerable<long> members = FunGameService.GetPage(club.Members.Keys, showPage, FunGameConstant.ItemsPerPage2);
                                foreach (long uid2 in members)
                                {
                                    if (FunGameConstant.UserIdAndUsername.TryGetValue(uid2, out User? user2) && user2 != null)
                                    {
                                        builer.AppendLine($"{count}.");
                                        builer.AppendLine($"UID：{user2.Id}");
                                        builer.AppendLine($"玩家昵称：{user2.Username}");
                                        string userType = "社员";
                                        if (club.Master?.Id == user2.Id)
                                        {
                                            userType = "社长";
                                        }
                                        else if (club.Admins.ContainsKey(user2.Id))
                                        {
                                            userType = "管理员";
                                        }
                                        builer.AppendLine($"社团身份：{userType}");
                                        builer.AppendLine($"加入时间：{club.MemberJoinTime[user2.Id].ToString(General.GeneralDateTimeFormatChinese)}");
                                    }
                                    count++;
                                }
                                builer.AppendLine($"页数：{showPage} / {maxPage}");
                            }
                            else
                            {
                                NetworkUtility.JsonSerialize($"没有这么多页！当前总页数为 {maxPage}，但你请求的是第 {showPage} 页。");
                            }
                            break;
                    }

                    msg = builer.ToString().Trim();
                }
                else
                {
                    msg = $"你目前还没有加入任何社团。";
                }

                return NetworkUtility.JsonSerialize(msg);
            }
            else
            {
                return NetworkUtility.JsonSerialize(noSaved);
            }
        }

        [HttpPost("disbandclub")]
        public string DisbandClub([FromQuery] long? uid = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);
                long clubid = 0;
                if (pc.TryGetValue("club", out object? value) && long.TryParse(value.ToString(), out long temp))
                {
                    clubid = temp;
                }

                if (clubid == 0)
                {
                    return NetworkUtility.JsonSerialize($"你当前没有加入任何社团！");
                }

                EntityModuleConfig<Club> emc = new("clubs", clubid.ToString());
                emc.LoadConfig();
                Club? club = emc.Get("club");
                if (club is null)
                {
                    return NetworkUtility.JsonSerialize($"不存在编号为 {clubid} 的社团！");
                }

                if (club.Master?.Id != userid)
                {
                    return NetworkUtility.JsonSerialize($"你不是社团的社长，没有权限使用此指令！");
                }

                string msg;
                string path = $@"{AppDomain.CurrentDomain.BaseDirectory}configs/clubs/{clubid}.json";
                try
                {
                    System.IO.File.Delete(path);
                    msg = $"解散社团 [ {club.Name} ] 成功！";
                    user.LastTime = DateTime.Now;
                    pc.Add("user", user);
                    pc.Add("club", 0);
                    pc.SaveConfig();
                    string directoryPath = $@"{AppDomain.CurrentDomain.BaseDirectory}configs/saved";
                    if (Directory.Exists(directoryPath))
                    {
                        string[] filePaths = Directory.GetFiles(directoryPath);
                        foreach (string filePath in filePaths)
                        {
                            string fileName = Path.GetFileNameWithoutExtension(filePath);
                            PluginConfig pc2 = new("saved", fileName);
                            pc2.LoadConfig();
                            if (pc2.TryGetValue("club", out value) && long.TryParse(value.ToString(), out long userClub) && userClub == clubid)
                            {
                                User user2 = FunGameService.GetUser(pc2);
                                user2.LastTime = DateTime.Now;
                                pc2.Add("user", user2);
                                pc2.Add("club", 0);
                                pc2.SaveConfig();
                            }
                        }
                    }
                }
                catch
                {
                    msg = $"解散社团 [ {club.Name} ] 失败，请联系服务器管理员处理！";
                }
                return NetworkUtility.JsonSerialize(msg);
            }
            else
            {
                return NetworkUtility.JsonSerialize(noSaved);
            }
        }

        [HttpPost("approveclub")]
        public string ApproveClub([FromQuery] long? uid = null, [FromQuery] long? id = null, [FromQuery] bool? approval = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
            long applicant = id ?? 0;
            bool isApproval = approval ?? false;

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);

                string msg = "";
                if (pc.TryGetValue("club", out object? value) && long.TryParse(value.ToString(), out long userClub) && userClub != 0)
                {
                    EntityModuleConfig<Club> emc = new("clubs", userClub.ToString());
                    emc.LoadConfig();
                    Club? club = emc.Get("club");
                    if (club is null)
                    {
                        return NetworkUtility.JsonSerialize($"你当前没有加入任何社团！");
                    }

                    if (club.Master?.Id == userid || club.Admins.ContainsKey(userid))
                    {
                        PluginConfig pc2 = new("saved", applicant.ToString());
                        pc2.LoadConfig();
                        if (pc2.ContainsKey("user"))
                        {
                            User user2 = FunGameService.GetUser(pc2);

                            if (club.Applicants.ContainsKey(user2.Id))
                            {
                                club.ApplicationTime.Remove(applicant);
                                club.Applicants.Remove(applicant);
                                if (isApproval)
                                {
                                    club.MemberJoinTime[applicant] = DateTime.Now;
                                    club.Members[applicant] = user2;
                                    msg += $"已批准 [ {user2.Username} ] 加入社团 [ {club.Name} ] ！";
                                    if (!pc2.ContainsKey("club") || (pc2.TryGetValue("club", out value) && long.TryParse(value.ToString(), out long user2Club) && user2Club == 0))
                                    {
                                        user2.LastTime = DateTime.Now;
                                        pc2.Add("user", user2);
                                        pc2.Add("club", userClub);
                                        pc2.SaveConfig();
                                    }
                                    else
                                    {
                                        msg += $"\r\n但是对方已经加入其它社团，与你的社团无缘了！";
                                    }
                                }
                                else
                                {
                                    msg += $"已拒绝 [ {user2.Username} ] 加入社团 [ {club.Name} ] ！";
                                }

                                emc.Add("club", club);
                                emc.SaveConfig();
                            }
                            else
                            {
                                return NetworkUtility.JsonSerialize($"对方并没有申请此社团！");
                            }
                        }
                        else
                        {
                            return NetworkUtility.JsonSerialize($"对方似乎还没创建存档呢！");
                        }
                    }
                    else
                    {
                        return NetworkUtility.JsonSerialize($"你没有权限审批申请人！");
                    }
                }
                else
                {
                    return NetworkUtility.JsonSerialize($"你当前没有加入任何社团！");
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

        [HttpPost("kickclub")]
        public string KickClub([FromQuery] long? uid = null, [FromQuery] long? id = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
            long kickid = id ?? 0;

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);

                string msg = "";
                if (pc.TryGetValue("club", out object? value) && long.TryParse(value.ToString(), out long userClub) && userClub != 0)
                {
                    EntityModuleConfig<Club> emc = new("clubs", userClub.ToString());
                    emc.LoadConfig();
                    Club? club = emc.Get("club");
                    if (club is null)
                    {
                        return NetworkUtility.JsonSerialize($"你当前没有加入任何社团！");
                    }

                    if (club.Master?.Id == userid || club.Admins.ContainsKey(userid))
                    {
                        if (!club.Admins.ContainsKey(kickid) || (club.Master?.Id == userid && club.Admins.ContainsKey(kickid)))
                        {
                            PluginConfig pc2 = new("saved", kickid.ToString());
                            pc2.LoadConfig();
                            if (pc2.ContainsKey("user"))
                            {
                                User user2 = FunGameService.GetUser(pc2);

                                if (club.Members.ContainsKey(user2.Id))
                                {
                                    club.MemberJoinTime.Remove(user2.Id);
                                    club.Members.Remove(user2.Id);
                                    club.Admins.Remove(user2.Id);
                                    msg += $"操作成功，已将 [ {user2.Username} ] 踢出社团 [ {club.Name} ] ！";
                                    user2.LastTime = DateTime.Now;
                                    pc2.Add("user", user2);
                                    pc2.Add("club", 0);
                                    pc2.SaveConfig();

                                    emc.Add("club", club);
                                    emc.SaveConfig();
                                }
                                else
                                {
                                    return NetworkUtility.JsonSerialize($"对方并不在此社团中，无法踢出！");
                                }
                            }
                            else
                            {
                                return NetworkUtility.JsonSerialize($"对方似乎还没创建存档呢！");
                            }
                        }
                        else
                        {
                            return NetworkUtility.JsonSerialize($"你没有权限踢出管理员！");
                        }
                    }
                    else
                    {
                        return NetworkUtility.JsonSerialize($"你没有权限踢出成员！");
                    }
                }
                else
                {
                    return NetworkUtility.JsonSerialize($"你当前没有加入任何社团！");
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

        [HttpPost("changeclub")]
        public string ChangeClub([FromQuery] long? uid = null, [FromQuery] string? part = null, [FromBody] string[]? args = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
            string name = part?.Trim().ToLower() ?? "";
            string[] values = args ?? [];

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);
                string msg;

                if (pc.TryGetValue("club", out object? value) && long.TryParse(value.ToString(), out long userClub) && userClub != 0)
                {
                    EntityModuleConfig<Club> emc = new("clubs", userClub.ToString());
                    emc.LoadConfig();
                    Club? club = emc.Get("club");
                    if (club is null)
                    {
                        return NetworkUtility.JsonSerialize($"你当前没有加入任何社团！");
                    }

                    bool isMaster = club.Master?.Id == userid;
                    bool isAdmin = club.Admins.ContainsKey(userid);

                    if (isMaster || isAdmin)
                    {
                        switch (name)
                        {
                            case "name":
                                if (!isMaster)
                                {
                                    return NetworkUtility.JsonSerialize("只有社长可以修改社团名称！");
                                }
                                if (values.Length > 0)
                                {
                                    if (values[0].Length >= 2 && values[0].Length <= 10)
                                    {
                                        club.Name = values[0];
                                        msg = "修改成功，新的社团名称是：" + club.Name;
                                    }
                                    else return NetworkUtility.JsonSerialize("社团名称只能包含2至15个字符！");
                                }
                                else
                                {
                                    return NetworkUtility.JsonSerialize("请提供新的社团名称！");
                                }
                                break;
                            case "prefix":
                                if (!isMaster)
                                {
                                    return NetworkUtility.JsonSerialize("只有社长可以修改社团前缀！");
                                }
                                string pattern = @"^[a-zA-Z0-9-_=+*%#^~.?!;:'"",]{3,4}$";
                                if (values.Length > 0)
                                {
                                    string clubPrefix = values[0];
                                    if (!Regex.IsMatch(clubPrefix, pattern))
                                    {
                                        return NetworkUtility.JsonSerialize($"社团的前缀只能包含总共3-4个英文字母和数字、允许的特殊字符，此前缀不满足条件。");
                                    }
                                    club.Prefix = clubPrefix;
                                    msg = "修改成功，新的社团前缀是：" + club.Prefix;
                                }
                                else
                                {
                                    return NetworkUtility.JsonSerialize("请提供新的社团前缀！");
                                }
                                break;
                            case "description":
                                if (values.Length > 0)
                                {
                                    msg = "修改成功，原先的社团描述：\r\n" + club.Description;
                                    club.Description = string.Join(" ", values);
                                    msg += "\r\n新的社团描述：" + club.Description;
                                }
                                else
                                {
                                    return NetworkUtility.JsonSerialize("请提供新的社团描述！");
                                }
                                break;
                            case "isneedapproval":
                                if (values.Length > 0 && bool.TryParse(values[0], out bool isNeedApproval))
                                {
                                    club.IsNeedApproval = isNeedApproval;
                                    msg = "修改成功，社团现在" + (club.IsNeedApproval ? "需要批准才能加入。" : "可以直接加入。");
                                }
                                else
                                {
                                    return NetworkUtility.JsonSerialize("请提供正确的布尔值（true 或 false）来设置加入是否需要批准！");
                                }
                                break;
                            case "ispublic":
                                if (values.Length > 0 && bool.TryParse(values[0], out bool isPublic))
                                {
                                    club.IsPublic = isPublic;
                                    msg = "修改成功，社团现在" + (club.IsPublic ? "是公开的，可以任何人加入。" : "是私密的，只能通过邀请加入。");
                                }
                                else
                                {
                                    return NetworkUtility.JsonSerialize("请提供正确的布尔值（true 或 false）来设置社团是否公开/私密！");
                                }
                                break;
                            case "setadmin":
                                if (!isMaster)
                                {
                                    return NetworkUtility.JsonSerialize("只有社长可以设置社团管理员！");
                                }
                                if (values.Length > 0 && long.TryParse(values[0], out long id) && club.Members.ContainsKey(id) && FunGameConstant.UserIdAndUsername.TryGetValue(id, out User? user2) && user2 != null)
                                {
                                    club.Admins[id] = user2;
                                    msg = $"将 [ {user2.Username} ] 设置为社团管理员成功！";
                                }
                                else
                                {
                                    return NetworkUtility.JsonSerialize("指定的用户不是此社团的成员！");
                                }
                                break;
                            case "setnotadmin":
                                if (!isMaster)
                                {
                                    return NetworkUtility.JsonSerialize("只有社长可以取消社团管理员！");
                                }
                                if (values.Length > 0 && long.TryParse(values[0], out id) && club.Members.ContainsKey(id) && FunGameConstant.UserIdAndUsername.TryGetValue(id, out user2) && user2 != null)
                                {
                                    if (club.Admins.Remove(id))
                                    {
                                        msg = $"取消 [ {user2.Username} ] 的社团管理员身份成功！";
                                    }
                                    else
                                    {
                                        msg = $"设置失败，[ {user2.Username} ] 并不是社团管理员！";
                                    }
                                }
                                else
                                {
                                    return NetworkUtility.JsonSerialize("指定的用户不是此社团的成员！");
                                }
                                break;
                            case "setmaster":
                                if (!isMaster)
                                {
                                    return NetworkUtility.JsonSerialize("只有社长可以转让社团！");
                                }
                                if (values.Length > 0 && long.TryParse(values[0], out id) && club.Members.ContainsKey(id) && FunGameConstant.UserIdAndUsername.TryGetValue(id, out user2) && user2 != null)
                                {
                                    club.Master = user2;
                                    club.Admins.Remove(user2.Id);
                                    club.Admins[user.Id] = user;
                                    msg = $"设置成功！即日起，[ {user2.Username} ] 是社团 [ {club.Name} ] 的新社长！";
                                }
                                else
                                {
                                    return NetworkUtility.JsonSerialize("指定的用户不是此社团的成员！");
                                }
                                break;
                            default:
                                return NetworkUtility.JsonSerialize("未知的社团设置项，设置失败。");
                        }

                        emc.Add("club", club);
                        emc.SaveConfig();
                    }
                    else
                    {
                        return NetworkUtility.JsonSerialize($"你没有权限修改社团设置！");
                    }
                }
                else
                {
                    return NetworkUtility.JsonSerialize($"你当前没有加入任何社团！");
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

        [HttpPost("showdailystore")]
        public string ShowDailyStore([FromQuery] long? uid = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);

                GeneralEventArgs e = new()
                {
                    EventMsg = $"{user.Username}正在访问每日商店"
                };
                FunGameService.ServerPluginLoader?.OnBeforeOpenStoreEvent(user, e);
                if (e.Cancel)
                {
                    return NetworkUtility.JsonSerialize(refused + (e.EventMsg != "" ? $"原因：{e.EventMsg}" : ""));
                }

                EntityModuleConfig<Store> store = new("stores", userid.ToString());
                store.LoadConfig();
                string msg = FunGameService.CheckDailyStore(store, user);
                store.SaveConfig();

                user.LastTime = DateTime.Now;
                pc.Add("user", user);
                pc.SaveConfig();

                e.EventMsg = $"{user.Username}访问每日商店成功";
                FunGameService.ServerPluginLoader?.OnAfterOpenStoreEvent(user, e);

                return NetworkUtility.JsonSerialize(msg);
            }
            else
            {
                return NetworkUtility.JsonSerialize(noSaved);
            }
        }

        [HttpPost("dailystorebuy")]
        public string DailyStoreBuy([FromQuery] long? uid = null, [FromQuery] long? id = null, [FromQuery] int? count = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
            long goodid = id ?? 0;
            int buycount = count ?? 1;
            if (buycount <= 0)
            {
                return NetworkUtility.JsonSerialize("数量必须大于0！");
            }

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);

                EntityModuleConfig<Store> store = new("stores", userid.ToString());
                store.LoadConfig();

                string msg = "";
                Store? daily = store.Get("daily");
                if (daily != null)
                {
                    if (daily.Goods.Values.FirstOrDefault(g => g.Id == goodid) is Goods good)
                    {
                        msg = FunGameService.StoreBuyItem(daily, good, user, buycount);
                    }
                    else
                    {
                        return NetworkUtility.JsonSerialize($"没有对应编号的商品！");
                    }

                }
                else
                {
                    return NetworkUtility.JsonSerialize($"商品列表为空，请使用【每日商店】指令来获取商品列表！");
                }

                store.Add("daily", daily);
                store.SaveConfig();
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

        [HttpPost("dailystoreshowinfo")]
        public string DailyStoreShowInfo([FromQuery] long? uid = null, [FromQuery] long? id = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
            long goodid = id ?? 0;

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);

                EntityModuleConfig<Store> store = new("stores", userid.ToString());
                store.LoadConfig();

                string msg = "";
                Store? daily = store.Get("daily");
                if (daily != null)
                {
                    if (daily.Goods.Values.FirstOrDefault(g => g.Id == goodid) is Goods good)
                    {
                        int count = 0;
                        string itemMsg = "";
                        foreach (Item item in good.Items)
                        {
                            count++;
                            Item newItem = item.Copy();
                            newItem.Character = user.Inventory.MainCharacter;
                            newItem.SetLevel(1);
                            itemMsg += $"[ {count} ] {newItem.ToString(false, true)}".Trim();
                        }
                        msg = good.ToString().Split("包含物品：")[0].Trim();
                        msg += $"\r\n包含物品：\r\n" + itemMsg +
                            $"\r\n剩余库存：{good.Stock}";
                    }
                    else
                    {
                        return NetworkUtility.JsonSerialize($"没有对应编号的物品！");
                    }

                }
                else
                {
                    return NetworkUtility.JsonSerialize($"商品列表不存在，请刷新！");
                }

                return NetworkUtility.JsonSerialize(msg);
            }
            else
            {
                return NetworkUtility.JsonSerialize(noSaved);
            }
        }

        [HttpPost("creategiftbox")]
        public string CreateGiftBox([FromQuery] long? uid = null, [FromQuery] string? name = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
            string itemName = name ?? "";

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);

                if (FunGameConstant.AllItems.FirstOrDefault(i => i.Name == itemName) is Item item)
                {
                    PluginConfig pc2 = new("giftbox", "giftbox");
                    pc2.LoadConfig();

                    List<long> list = [];
                    if (pc2.TryGetValue(itemName, out object? value) && value is List<long> tempList)
                    {
                        list = [.. tempList];
                    }

                    if (list.Contains(user.Id))
                    {
                        return NetworkUtility.JsonSerialize($"你已经领取过这个礼包【{itemName}】啦，不能重复领取哦！");
                    }

                    Item newItem = item.Copy();
                    newItem.IsSellable = false;
                    newItem.IsTradable = false;
                    newItem.User = user;
                    user.Inventory.Items.Add(newItem);
                    string msg = $"恭喜你获得礼包【{itemName}】一份！";

                    list.Add(user.Id);
                    pc2.Add(itemName, list);
                    pc2.SaveConfig();

                    user.LastTime = DateTime.Now;
                    pc.Add("user", user);
                    pc.SaveConfig();

                    return NetworkUtility.JsonSerialize(msg);
                }
                else
                {
                    return NetworkUtility.JsonSerialize("没有找到这个礼包，可能已经过期。");
                }
            }
            else
            {
                return NetworkUtility.JsonSerialize(noSaved);
            }
        }

        [HttpGet("getregion")]
        public List<string> GetRegion([FromQuery] int? index = null)
        {
            List<string> regions = [];
            if (index != null)
            {
                if (FunGameConstant.Regions.FirstOrDefault(kv => kv.Id == index) is OshimaRegion region)
                {
                    regions.Add(region.ToString());
                }
                else
                {
                    regions.Add($"找不到指定编号的地区！");
                }
            }
            else if (FunGameConstant.Regions.Count > 0)
            {
                regions.Add($"世界地图：");
                for (int i = 0; i < FunGameConstant.Regions.Count; i++)
                {
                    OshimaRegion region = FunGameConstant.Regions[i];
                    List<Item> crops = [];
                    if (FunGameConstant.ExploreItems.TryGetValue(region, out List<Item>? list) && list != null)
                    {
                        crops = list;
                    }
                    regions.Add($"{region.Id}. {region.Name}" + (crops.Count > 0 ? "（作物：" + string.Join("，", crops.Select(i => i.Name)) + "）" : ""));
                }
                regions.Add($"提示：使用【查地区+序号】指令来查看指定地区的信息。");
            }
            else
            {
                regions.Add($"世界地图遇到了问题，暂时无法显示……");
            }
            return regions;
        }

        [HttpPost("exploreregion")]
        public string ExploreRegion([FromQuery] long? uid = null, [FromQuery] long? id = null)
        {
            long userid = uid ?? Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 11));
            long regionid = id ?? 0;

            PluginConfig pc = new("saved", userid.ToString());
            pc.LoadConfig();

            string msg = "";
            if (pc.Count > 0)
            {
                User user = FunGameService.GetUser(pc);

                if (regionid > 0 && regionid <= FunGameConstant.Regions.Count && FunGameConstant.Regions.FirstOrDefault(r => r.Id == regionid) is OshimaRegion region)
                {
                    msg = $"开始探索【{region.Name}】，探索时间：{FunGameConstant.ExploreTime} 分钟。（骗你的，其实还没做）";
                }
                else
                {
                    return $"没有找到与这个序号相对应的地区！";
                }

                user.LastTime = DateTime.Now;
                pc.Add("user", user);
                pc.SaveConfig();

                return msg;
            }
            else
            {
                return noSaved;
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
    }
}
