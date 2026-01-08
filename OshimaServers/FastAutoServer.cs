using System.Collections.Concurrent;
using System.Text;
using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model;
using Oshima.Core.Constant;
using Oshima.FunGame.OshimaModules.Items;
using Oshima.FunGame.OshimaModules.Skills;
using Oshima.FunGame.OshimaServers.Service;

namespace Oshima.FunGame.OshimaServers
{
    public class FastAutoServer : GameModuleServer
    {
        public override string Name => OshimaGameModuleConstant.FastAuto;
        public override string Description => OshimaGameModuleConstant.Description;
        public override string Version => OshimaGameModuleConstant.Version;
        public override string Author => OshimaGameModuleConstant.Author;
        public override string DefaultMap => OshimaGameModuleConstant.FastAutoMap;
        public override GameModuleDepend GameModuleDepend => OshimaGameModuleConstant.GameModuleDepend;
        public static List<Character> Characters { get; } = [];
        public static Dictionary<Character, CharacterStatistics> CharacterStatistics { get; } = [];
        public static PluginConfig StatsConfig { get; } = new(OshimaGameModuleConstant.FastAuto, nameof(CharacterStatistics));
        public static GameModuleLoader? GameModuleLoader { get; set; } = null;

        public override async Task<Dictionary<string, object>> GamingMessageHandler(IServerModel model, GamingType type, Dictionary<string, object> data)
        {
            Dictionary<string, object> result = [];
            // 获取model所在的房间工作类
            ModuleServerWorker worker = Workers[model.InRoom.Roomid];

            switch (type)
            {
                case GamingType.Connect:
                    string un = (DataRequest.GetDictionaryJsonObject<string>(data, "un") ?? "").Trim();
                    if (un != "" && !worker.ConnectedUser.Any(u => u.Username == un))
                    {
                        worker.ConnectedUser.Add(model.User);
                        Controller.WriteLine(un + " 已连接至房间。");
                    }
                    break;
                case GamingType.Disconnect:
                    break;
                case GamingType.Reconnect:
                    break;
                case GamingType.BanCharacter:
                    break;
                case GamingType.PickCharacter:
                    break;
                case GamingType.Random:
                    break;
                case GamingType.Round:
                    break;
                case GamingType.LevelUp:
                    break;
                case GamingType.Move:
                    break;
                case GamingType.Attack:
                    break;
                case GamingType.Skill:
                    break;
                case GamingType.Item:
                    break;
                case GamingType.Magic:
                    break;
                case GamingType.Buy:
                    break;
                case GamingType.SuperSkill:
                    break;
                case GamingType.Pause:
                    break;
                case GamingType.Unpause:
                    break;
                case GamingType.Surrender:
                    break;
                case GamingType.UpdateInfo:
                    break;
                case GamingType.Punish:
                    break;
                case GamingType.None:
                default:
                    await Task.Delay(1);
                    break;
            }

            return result;
        }

        private readonly struct ModuleServerWorker(GamingObject obj)
        {
            public GamingObject GamingObject { get; } = obj;
            public List<User> ConnectedUser { get; } = [];
            public Dictionary<string, Dictionary<string, object>> UserData { get; } = [];
            public Dictionary<User, Character> PickCharacters { get; } = [];
        }

        private ConcurrentDictionary<string, ModuleServerWorker> Workers { get; } = [];

        public override bool StartServer(GamingObject obj, params object[] args)
        {
            // 因为模组是单例的，需要为这个房间创建一个工作类接收参数，不能直接用本地变量处理
            ModuleServerWorker worker = new(obj);
            Workers[obj.Room.Roomid] = worker;
            TaskUtility.NewTask(async () => await StartGame(obj, worker)).OnError(Controller.Error);
            return true;
        }

        private async Task StartGame(GamingObject obj, ModuleServerWorker worker)
        {
            try
            {
                while (true)
                {
                    if (worker.ConnectedUser.Count == obj.All.Count)
                    {
                        break;
                    }
                    await Task.Delay(500);
                }

                Dictionary<User, Character> characters = [];
                List<Character> characterPickeds = [];
                Dictionary<string, object> data = [];

                // 抽取角色
                foreach (User user in obj.Users)
                {
                    List<Character> list = [.. Characters.Where(c => !characterPickeds.Contains(c))];
                    Character cr = list[Random.Shared.Next(list.Count - 1)];
                    string msg = $"{user.Username} 抽到了 [ {cr} ]";
                    characterPickeds.Add(cr);
                    characters.Add(user, cr.Copy());
                    Controller.WriteLine(msg);
                    SendAllGamingMessage(obj, data, msg);
                }

                int clevel = 60;
                int slevel = 6;
                int mlevel = 8;

                List<Character> inGameCharacters = [.. characters.Values];

                // 升级和赋能
                foreach (Character c in inGameCharacters)
                {
                    c.Level = clevel;
                    c.NormalAttack.Level = mlevel;

                    Skill 冰霜攻击 = new 冰霜攻击(c)
                    {
                        Level = mlevel
                    };
                    c.Skills.Add(冰霜攻击);

                    Skill 疾风步 = new 疾风步(c)
                    {
                        Level = slevel
                    };
                    c.Skills.Add(疾风步);

                    FunGameService.AddCharacterSkills(c, 1, slevel, slevel);

                    SendAllGamingMessage(obj, data, c.GetInfo());
                }

                MixGamingQueue actionQueue = new(inGameCharacters, (str) =>
                {
                    SendAllGamingMessage(obj, data, str);
                });
                actionQueue.InitActionQueue();
                actionQueue.SetCharactersToAIControl(false, inGameCharacters);

                // 总游戏时长
                double totalTime = 0;
                // 总死亡数
                int deaths = 0;

                // 开始空投
                空投(actionQueue, totalTime);

                // 总回合数
                int i = 1;
                while (i < 999)
                {
                    if (i == 998)
                    {
                        SendAllGamingMessage(obj, data, $"=== 终局审判 ===");
                        Dictionary<Character, double> 他们的血量百分比 = [];
                        foreach (Character c in inGameCharacters)
                        {
                            他们的血量百分比.TryAdd(c, Calculation.Round4Digits(c.HP / c.MaxHP));
                        }
                        double max = 他们的血量百分比.Values.Max();
                        Character winner = 他们的血量百分比.Keys.Where(c => 他们的血量百分比[c] == max).First();
                        SendAllGamingMessage(obj, data, "[ " + winner + " ] 成为了天选之人！！");
                        foreach (Character c in inGameCharacters.Where(c => c != winner && c.HP > 0))
                        {
                            SendAllGamingMessage(obj, data, "[ " + winner + " ] 对 [ " + c + " ] 造成了 99999999999 点真实伤害。");
                            actionQueue.DeathCalculation(winner, c);
                        }
                        actionQueue.EndGameInfo(winner);
                        break;
                    }

                    // 检查是否有角色可以行动
                    Character? characterToAct = actionQueue.NextCharacter();

                    // 处理回合
                    if (characterToAct != null)
                    {
                        SendAllGamingMessage(obj, data, $"=== Round {i++} ===");
                        SendAllGamingMessage(obj, data, "现在是 [ " + characterToAct + " ] 的回合！");

                        if (actionQueue.Queue.Count == 0)
                        {
                            break;
                        }

                        bool isGameEnd = actionQueue.ProcessTurn(characterToAct);
                        if (isGameEnd)
                        {
                            break;
                        }

                        actionQueue.DisplayQueue();
                    }

                    // 模拟时间流逝
                    totalTime += actionQueue.TimeLapse();

                    if (actionQueue.Eliminated.Count > deaths)
                    {
                        deaths = actionQueue.Eliminated.Count;
                    }
                }

                SendAllGamingMessage(obj, data, "--- End ---");
                SendAllGamingMessage(obj, data, "总游戏时长：" + Calculation.Round2Digits(totalTime));

                // 赛后统计
                SendAllGamingMessage(obj, data, "=== 伤害排行榜 ===");
                int top = inGameCharacters.Count;
                int count = 1;
                foreach (Character character in actionQueue.CharacterStatistics.OrderByDescending(d => d.Value.TotalDamage).Select(d => d.Key))
                {
                    StringBuilder builder = new();
                    CharacterStatistics stats = actionQueue.CharacterStatistics[character];
                    builder.AppendLine($"{count++}. [ {character.ToStringWithLevel()} ] （{stats.Kills} / {stats.Assists}）");
                    builder.AppendLine($"存活时长：{stats.LiveTime} / 存活回合数：{stats.LiveRound} / 行动回合数：{stats.ActionTurn} / 总计决策数：{stats.TurnDecisions} / 总计决策点：{stats.UseDecisionPoints}");
                    builder.AppendLine($"总计伤害：{stats.TotalDamage} / 总计物理伤害：{stats.TotalPhysicalDamage} / 总计魔法伤害：{stats.TotalMagicDamage}");
                    builder.AppendLine($"总承受伤害：{stats.TotalTakenDamage} / 总承受物理伤害：{stats.TotalTakenPhysicalDamage} / 总承受魔法伤害：{stats.TotalTakenMagicDamage}");
                    builder.Append($"每秒伤害：{stats.DamagePerSecond} / 每回合伤害：{stats.DamagePerTurn}");

                    SendAllGamingMessage(obj, data, builder.ToString());

                    CharacterStatistics? totalStats = CharacterStatistics.Where(kv => kv.Key.GetName() == character.GetName()).Select(kv => kv.Value).FirstOrDefault();
                    if (totalStats != null)
                    {
                        // 统计此角色的所有数据
                        totalStats.TotalDamage = Calculation.Round2Digits(totalStats.TotalDamage + stats.TotalDamage);
                        totalStats.TotalPhysicalDamage = Calculation.Round2Digits(totalStats.TotalPhysicalDamage + stats.TotalPhysicalDamage);
                        totalStats.TotalMagicDamage = Calculation.Round2Digits(totalStats.TotalMagicDamage + stats.TotalMagicDamage);
                        totalStats.TotalTrueDamage = Calculation.Round2Digits(totalStats.TotalTrueDamage + stats.TotalTrueDamage);
                        totalStats.TotalTakenDamage = Calculation.Round2Digits(totalStats.TotalTakenDamage + stats.TotalTakenDamage);
                        totalStats.TotalTakenPhysicalDamage = Calculation.Round2Digits(totalStats.TotalTakenPhysicalDamage + stats.TotalTakenPhysicalDamage);
                        totalStats.TotalTakenMagicDamage = Calculation.Round2Digits(totalStats.TotalTakenMagicDamage + stats.TotalTakenMagicDamage);
                        totalStats.TotalTakenTrueDamage = Calculation.Round2Digits(totalStats.TotalTakenTrueDamage + stats.TotalTakenTrueDamage);
                        totalStats.LiveRound += stats.LiveRound;
                        totalStats.ActionTurn += stats.ActionTurn;
                        totalStats.LiveTime = Calculation.Round2Digits(totalStats.LiveTime + stats.LiveTime);
                        totalStats.TotalEarnedMoney += stats.TotalEarnedMoney;
                        totalStats.Kills += stats.Kills;
                        totalStats.Deaths += stats.Deaths;
                        totalStats.Assists += stats.Assists;
                        totalStats.LastRank = stats.LastRank;
                        double totalRank = totalStats.AvgRank * totalStats.Plays + totalStats.LastRank;
                        totalStats.Plays += stats.Plays;
                        if (totalStats.Plays != 0) totalStats.AvgRank = Calculation.Round2Digits(totalRank / totalStats.Plays);
                        totalStats.Wins += stats.Wins;
                        totalStats.Top3s += stats.Top3s;
                        totalStats.Loses += stats.Loses;
                        if (totalStats.Plays != 0)
                        {
                            totalStats.AvgDamage = Calculation.Round2Digits(totalStats.TotalDamage / totalStats.Plays);
                            totalStats.AvgPhysicalDamage = Calculation.Round2Digits(totalStats.TotalPhysicalDamage / totalStats.Plays);
                            totalStats.AvgMagicDamage = Calculation.Round2Digits(totalStats.TotalMagicDamage / totalStats.Plays);
                            totalStats.AvgTrueDamage = Calculation.Round2Digits(totalStats.TotalTrueDamage / totalStats.Plays);
                            totalStats.AvgTakenDamage = Calculation.Round2Digits(totalStats.TotalTakenDamage / totalStats.Plays);
                            totalStats.AvgTakenPhysicalDamage = Calculation.Round2Digits(totalStats.TotalTakenPhysicalDamage / totalStats.Plays);
                            totalStats.AvgTakenMagicDamage = Calculation.Round2Digits(totalStats.TotalTakenMagicDamage / totalStats.Plays);
                            totalStats.AvgTakenTrueDamage = Calculation.Round2Digits(totalStats.TotalTakenTrueDamage / totalStats.Plays);
                            totalStats.AvgLiveRound = totalStats.LiveRound / totalStats.Plays;
                            totalStats.AvgActionTurn = totalStats.ActionTurn / totalStats.Plays;
                            totalStats.AvgLiveTime = Calculation.Round2Digits(totalStats.LiveTime / totalStats.Plays);
                            totalStats.AvgEarnedMoney = totalStats.TotalEarnedMoney / totalStats.Plays;
                            totalStats.Winrate = Calculation.Round4Digits(Convert.ToDouble(totalStats.Wins) / Convert.ToDouble(totalStats.Plays));
                            totalStats.Top3rate = Calculation.Round4Digits(Convert.ToDouble(totalStats.Top3s) / Convert.ToDouble(totalStats.Plays));
                        }
                        if (totalStats.LiveRound != 0) totalStats.DamagePerRound = Calculation.Round2Digits(totalStats.TotalDamage / totalStats.LiveRound);
                        if (totalStats.ActionTurn != 0) totalStats.DamagePerTurn = Calculation.Round2Digits(totalStats.TotalDamage / totalStats.ActionTurn);
                        if (totalStats.LiveTime != 0) totalStats.DamagePerSecond = Calculation.Round2Digits(totalStats.TotalDamage / totalStats.LiveTime);
                    }
                }

                // 显示每个角色的信息
                for (i = actionQueue.Eliminated.Count - 1; i >= 0; i--)
                {
                    Character character = actionQueue.Eliminated[i];
                    SendAllGamingMessage(obj, data, $"=== 角色 [ {character} ] ===\r\n{character.GetInfo()}");
                }

                lock (StatsConfig)
                {
                    foreach (Character c in CharacterStatistics.Keys)
                    {
                        StatsConfig.Add(c.ToStringWithOutUser(), CharacterStatistics[c]);
                    }
                    StatsConfig.SaveConfig();
                }

                // 结束
                SendEndGame(obj);
                worker.ConnectedUser.Clear();
                Workers.Remove(obj.Room.Roomid, out _);
            }
            catch (Exception e)
            {
                TXTHelper.AppendErrorLog(e.ToString());
            }
        }

        public static void 空投(GamingQueue queue, double totalTime)
        {
            Item[] 这次发放的空投;
            if (totalTime == 0)
            {
                foreach (Character character in queue.Queue)
                {
                    这次发放的空投 = [new 攻击之爪25()];
                    foreach (Item item in 这次发放的空投)
                    {
                        queue.Equip(character, EquipSlotType.Accessory1, item, out _);
                    }
                }
            }
        }

        private void SendAllGamingMessage(GamingObject obj, Dictionary<string, object> data, string str, bool showmessage = false)
        {
            data.Clear();
            data.Add("msg", str);
            data.Add("showmessage", showmessage);
            _ = SendGamingMessage(obj.All.Values, GamingType.UpdateInfo, data);
        }

        public override void AfterLoad(GameModuleLoader loader, params object[] args)
        {
            foreach (Character c in GameModuleDepend.Characters.Values)
            {
                Character character = c.Copy();
                Characters.Add(character);
                CharacterStatistics.Add(character, new());
            }

            StatsConfig.LoadConfig();
            foreach (Character character in CharacterStatistics.Keys)
            {
                if (StatsConfig.ContainsKey(character.ToStringWithOutUser()))
                {
                    CharacterStatistics[character] = StatsConfig.Get<CharacterStatistics>(character.ToStringWithOutUser()) ?? CharacterStatistics[character];
                }
            }
        }
    }
}
