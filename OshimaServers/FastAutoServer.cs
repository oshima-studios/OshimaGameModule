using System.Text;
using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules;
using Oshima.FunGame.OshimaModules.Items;
using Oshima.FunGame.OshimaModules.Skills;

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
        public static List<User> ConnectedUsers { get; } = [];

        public override async Task<Dictionary<string, object>> GamingMessageHandler(string username, GamingType type, Dictionary<string, object> data)
        {
            Dictionary<string, object> result = [];

            switch (type)
            {
                case GamingType.Connect:
                    string un = (DataRequest.GetDictionaryJsonObject<string>(data, "un") ?? "").Trim();
                    if (un != "" && !ConnectedUsers.Where(u => u.Username == un).Any())
                    {
                        ConnectedUsers.Add(Users.Where(u => u.Username == un).First());
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
                    break;
            }

            return result;
        }

        protected Room Room = General.HallInstance;
        protected List<User> Users = [];
        protected IServerModel? RoomMaster;
        protected Dictionary<string, IServerModel> All = [];

        public override bool StartServer(string GameModule, Room Room, List<User> Users, IServerModel RoomMasterServerModel, Dictionary<string, IServerModel> ServerModels, params object[] Args)
        {
            // 将参数转为本地属性
            this.Room = Room;
            this.Users = Users;
            RoomMaster = RoomMasterServerModel;
            All = ServerModels;
            TaskUtility.NewTask(StartGame).OnError(Controller.Error);
            return true;
        }

        public async Task StartGame()
        {
            try
            {
                while (true)
                {
                    if (ConnectedUsers.Count == Users.Count)
                    {
                        break;
                    }
                    await Task.Delay(500);
                }

                Dictionary<User, Character> characters = [];
                List<Character> characterPickeds = [];
                Dictionary<string, object> data = [];

                // 抽取角色
                foreach (User user in Users)
                {
                    List<Character> list = [.. Characters.Where(c => !characterPickeds.Contains(c))];
                    Character cr = list[Random.Shared.Next(list.Count - 1)];
                    string msg = $"{user.Username} 抽到了 [ {cr} ]";
                    characterPickeds.Add(cr);
                    characters.Add(user, cr.Copy());
                    Controller.WriteLine(msg);
                    SendAllGamingMessage(data, msg);
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

                    if (c.Id == 1)
                    {
                        Skill META马 = new META马(c)
                        {
                            Level = 1
                        };
                        c.Skills.Add(META马);

                        Skill 力量爆发 = new 力量爆发(c)
                        {
                            Level = mlevel
                        };
                        c.Skills.Add(力量爆发);
                    }

                    if (c.Id == 2)
                    {
                        Skill 心灵之火 = new 心灵之火(c)
                        {
                            Level = 1
                        };
                        c.Skills.Add(心灵之火);

                        Skill 天赐之力 = new 天赐之力(c)
                        {
                            Level = slevel
                        };
                        c.Skills.Add(天赐之力);
                    }

                    if (c.Id == 3)
                    {
                        Skill 魔法震荡 = new 魔法震荡(c)
                        {
                            Level = 1
                        };
                        c.Skills.Add(魔法震荡);

                        Skill 魔法涌流 = new 魔法涌流(c)
                        {
                            Level = slevel
                        };
                        c.Skills.Add(魔法涌流);
                    }

                    if (c.Id == 4)
                    {
                        Skill 灵能反射 = new 灵能反射(c)
                        {
                            Level = 1
                        };
                        c.Skills.Add(灵能反射);

                        Skill 三重叠加 = new 三重叠加(c)
                        {
                            Level = slevel
                        };
                        c.Skills.Add(三重叠加);
                    }

                    if (c.Id == 5)
                    {
                        Skill 智慧与力量 = new 智慧与力量(c)
                        {
                            Level = 1
                        };
                        c.Skills.Add(智慧与力量);

                        Skill 变幻之心 = new 变幻之心(c)
                        {
                            Level = slevel
                        };
                        c.Skills.Add(变幻之心);
                    }

                    if (c.Id == 6)
                    {
                        Skill 致命打击 = new 致命打击(c)
                        {
                            Level = 1
                        };
                        c.Skills.Add(致命打击);

                        Skill 精准打击 = new 精准打击(c)
                        {
                            Level = slevel
                        };
                        c.Skills.Add(精准打击);
                    }

                    if (c.Id == 7)
                    {
                        Skill 毁灭之势 = new 毁灭之势(c)
                        {
                            Level = 1
                        };
                        c.Skills.Add(毁灭之势);

                        Skill 绝对领域 = new 绝对领域(c)
                        {
                            Level = slevel
                        };
                        c.Skills.Add(绝对领域);
                    }

                    if (c.Id == 8)
                    {
                        Skill 枯竭打击 = new 枯竭打击(c)
                        {
                            Level = 1
                        };
                        c.Skills.Add(枯竭打击);

                        Skill 能量毁灭 = new 能量毁灭(c)
                        {
                            Level = slevel
                        };
                        c.Skills.Add(能量毁灭);
                    }

                    if (c.Id == 9)
                    {
                        Skill 玻璃大炮 = new 玻璃大炮(c)
                        {
                            Level = 1
                        };
                        c.Skills.Add(玻璃大炮);

                        Skill 迅捷之势 = new 迅捷之势(c)
                        {
                            Level = slevel
                        };
                        c.Skills.Add(迅捷之势);
                    }

                    if (c.Id == 10)
                    {
                        Skill 累积之压 = new 累积之压(c)
                        {
                            Level = 1
                        };
                        c.Skills.Add(累积之压);

                        Skill 嗜血本能 = new 嗜血本能(c)
                        {
                            Level = slevel
                        };
                        c.Skills.Add(嗜血本能);
                    }

                    if (c.Id == 11)
                    {
                        Skill 敏捷之刃 = new 敏捷之刃(c)
                        {
                            Level = 1
                        };
                        c.Skills.Add(敏捷之刃);

                        Skill 平衡强化 = new 平衡强化(c)
                        {
                            Level = slevel
                        };
                        c.Skills.Add(平衡强化);
                    }

                    if (c.Id == 12)
                    {
                        Skill 弱者猎手 = new 弱者猎手(c)
                        {
                            Level = 1
                        };
                        c.Skills.Add(弱者猎手);

                        Skill 血之狂欢 = new 血之狂欢(c)
                        {
                            Level = slevel
                        };
                        c.Skills.Add(血之狂欢);
                    }

                    SendAllGamingMessage(data, c.GetInfo());
                }

                ActionQueue actionQueue = new(inGameCharacters, (str) =>
                {
                    SendAllGamingMessage(data, str);
                });

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
                        SendAllGamingMessage(data, $"=== 终局审判 ===");
                        Dictionary<Character, double> 他们的血量百分比 = [];
                        foreach (Character c in inGameCharacters)
                        {
                            他们的血量百分比.TryAdd(c, Calculation.Round4Digits(c.HP / c.MaxHP));
                        }
                        double max = 他们的血量百分比.Values.Max();
                        Character winner = 他们的血量百分比.Keys.Where(c => 他们的血量百分比[c] == max).First();
                        SendAllGamingMessage(data, "[ " + winner + " ] 成为了天选之人！！");
                        foreach (Character c in inGameCharacters.Where(c => c != winner && c.HP > 0))
                        {
                            SendAllGamingMessage(data, "[ " + winner + " ] 对 [ " + c + " ] 造成了 99999999999 点真实伤害。");
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
                        SendAllGamingMessage(data, $"=== Round {i++} ===");
                        SendAllGamingMessage(data, "现在是 [ " + characterToAct + " ] 的回合！");

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

                SendAllGamingMessage(data, "--- End ---");
                SendAllGamingMessage(data, "总游戏时长：" + Calculation.Round2Digits(totalTime));

                // 赛后统计
                SendAllGamingMessage(data, "=== 伤害排行榜 ===");
                int top = inGameCharacters.Count;
                int count = 1;
                foreach (Character character in actionQueue.CharacterStatistics.OrderByDescending(d => d.Value.TotalDamage).Select(d => d.Key))
                {
                    StringBuilder builder = new();
                    CharacterStatistics stats = actionQueue.CharacterStatistics[character];
                    builder.AppendLine($"{count++}. [ {character.ToStringWithLevel()} ] （{stats.Kills} / {stats.Assists}）");
                    builder.AppendLine($"存活时长：{stats.LiveTime} / 存活回合数：{stats.LiveRound} / 行动回合数：{stats.ActionTurn}");
                    builder.AppendLine($"总计伤害：{stats.TotalDamage} / 总计物理伤害：{stats.TotalPhysicalDamage} / 总计魔法伤害：{stats.TotalMagicDamage}");
                    builder.AppendLine($"总承受伤害：{stats.TotalTakenDamage} / 总承受物理伤害：{stats.TotalTakenPhysicalDamage} / 总承受魔法伤害：{stats.TotalTakenMagicDamage}");
                    builder.Append($"每秒伤害：{stats.DamagePerSecond} / 每回合伤害：{stats.DamagePerTurn}");

                    SendAllGamingMessage(data, builder.ToString());

                    CharacterStatistics? totalStats = CharacterStatistics.Where(kv => kv.Key.GetName() == character.GetName()).Select(kv => kv.Value).FirstOrDefault();
                    if (totalStats != null)
                    {
                        // 统计此角色的所有数据
                        totalStats.TotalDamage = Calculation.Round2Digits(totalStats.TotalDamage + stats.TotalDamage);
                        totalStats.TotalPhysicalDamage = Calculation.Round2Digits(totalStats.TotalPhysicalDamage + stats.TotalPhysicalDamage);
                        totalStats.TotalMagicDamage = Calculation.Round2Digits(totalStats.TotalMagicDamage + stats.TotalMagicDamage);
                        totalStats.TotalRealDamage = Calculation.Round2Digits(totalStats.TotalRealDamage + stats.TotalRealDamage);
                        totalStats.TotalTakenDamage = Calculation.Round2Digits(totalStats.TotalTakenDamage + stats.TotalTakenDamage);
                        totalStats.TotalTakenPhysicalDamage = Calculation.Round2Digits(totalStats.TotalTakenPhysicalDamage + stats.TotalTakenPhysicalDamage);
                        totalStats.TotalTakenMagicDamage = Calculation.Round2Digits(totalStats.TotalTakenMagicDamage + stats.TotalTakenMagicDamage);
                        totalStats.TotalTakenRealDamage = Calculation.Round2Digits(totalStats.TotalTakenRealDamage + stats.TotalTakenRealDamage);
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
                            totalStats.AvgRealDamage = Calculation.Round2Digits(totalStats.TotalRealDamage / totalStats.Plays);
                            totalStats.AvgTakenDamage = Calculation.Round2Digits(totalStats.TotalTakenDamage / totalStats.Plays);
                            totalStats.AvgTakenPhysicalDamage = Calculation.Round2Digits(totalStats.TotalTakenPhysicalDamage / totalStats.Plays);
                            totalStats.AvgTakenMagicDamage = Calculation.Round2Digits(totalStats.TotalTakenMagicDamage / totalStats.Plays);
                            totalStats.AvgTakenRealDamage = Calculation.Round2Digits(totalStats.TotalTakenRealDamage / totalStats.Plays);
                            totalStats.AvgLiveRound = totalStats.LiveRound / totalStats.Plays;
                            totalStats.AvgActionTurn = totalStats.ActionTurn / totalStats.Plays;
                            totalStats.AvgLiveTime = Calculation.Round2Digits(totalStats.LiveTime / totalStats.Plays);
                            totalStats.AvgEarnedMoney = totalStats.TotalEarnedMoney / totalStats.Plays;
                            totalStats.Winrates = Calculation.Round4Digits(Convert.ToDouble(totalStats.Wins) / Convert.ToDouble(totalStats.Plays));
                            totalStats.Top3rates = Calculation.Round4Digits(Convert.ToDouble(totalStats.Top3s) / Convert.ToDouble(totalStats.Plays));
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
                    SendAllGamingMessage(data, $"=== 角色 [ {character} ] ===\r\n{character.GetInfo()}");
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
                await Send(All.Values, SocketMessageType.EndGame, Room, Users);
                foreach (IServerModel model in All.Values)
                {
                    model.NowGamingServer = null;
                }
                ConnectedUsers.Clear();
            }
            catch (Exception e)
            {
                TXTHelper.AppendErrorLog(e.ToString());
            }
        }

        public static void 空投(ActionQueue queue, double totalTime)
        {
            Item[] 这次发放的空投;
            if (totalTime == 0)
            {
                foreach (Character character in queue.Queue)
                {
                    这次发放的空投 = [new 攻击之爪50()];
                    foreach (Item item in 这次发放的空投)
                    {
                        queue.Equip(character, EquipItemToSlot.Accessory1, item);
                    }
                }
            }
        }

        public void SendAllGamingMessage(Dictionary<string, object> data, string str, bool showmessage = false)
        {
            data.Clear();
            data.Add("msg", str);
            data.Add("showmessage", showmessage);
            SendGamingMessage(All.Values, GamingType.UpdateInfo, data);
        }

        public override void AfterLoad(params object[] args)
        {
            foreach (Character c in GameModuleDepend.Characters)
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
