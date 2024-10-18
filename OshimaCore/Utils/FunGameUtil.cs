using System.Text;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model;
using Oshima.FunGame.OshimaModules.Characters;
using Oshima.FunGame.OshimaModules.Items;
using Oshima.FunGame.OshimaModules.Skills;

namespace Oshima.Core.Utils
{
    public class FunGameSimulation
    {
        public FunGameSimulation()
        {
            InitCharacter();

            bool printout = true;
            List<string> strs = StartGame(printout);
            if (printout == false)
            {
                foreach (string str in strs)
                {
                    Console.WriteLine(str);
                }
            }

            Console.ReadKey();
        }

        public static List<Character> Characters { get; } = [];
        public static Dictionary<Character, CharacterStatistics> CharacterStatistics { get; } = [];
        public static PluginConfig StatsConfig { get; } = new(nameof(FunGameSimulation), nameof(CharacterStatistics));
        public static bool IsRuning { get; set; } = false;
        public static bool IsWeb { get; set; } = false;
        public static bool PrintOut { get; set; } = false;
        public static string Msg { get; set; } = "";

        public static List<string> StartGame(bool printout, bool isWeb = false)
        {
            PrintOut = printout;
            IsWeb = isWeb;
            try
            {
                if (IsRuning) return ["游戏正在模拟中，请勿重复请求！"];

                List<string> result = [];
                int deaths = 0;
                Msg = "";

                IsRuning = true;

                // M = 0, W = 7, P1 = 1, P3 = 1
                // M = 1, W = 6, P1 = 2, P3 = 0
                // M = 2, W = 4, P1 = 0, P3 = 2
                // M = 2, W = 5, P1 = 0, P3 = 0
                // M = 3, W = 3, P1 = 1, P3 = 1
                // M = 4, W = 2, P1 = 2, P3 = 0
                // M = 5, W = 0, P1 = 0, P3 = 2
                // M = 5, W = 1, P1 = 0, P3 = 0

                List<Character> list = new(Characters);

                if (list.Count > 11)
                {
                    if (PrintOut) Console.WriteLine();
                    if (PrintOut) Console.WriteLine("Start!!!");
                    if (PrintOut) Console.WriteLine();

                    Character character1 = list[0].Copy();
                    Character character2 = list[1].Copy();
                    Character character3 = list[2].Copy();
                    Character character4 = list[3].Copy();
                    Character character5 = list[4].Copy();
                    Character character6 = list[5].Copy();
                    Character character7 = list[6].Copy();
                    Character character8 = list[7].Copy();
                    Character character9 = list[8].Copy();
                    Character character10 = list[9].Copy();
                    Character character11 = list[10].Copy();
                    Character character12 = list[11].Copy();

                    List<Character> characters = [
                        character1, character2, character3, character4,
                        character5, character6, character7, character8,
                        character9, character10, character11, character12
                    ];

                    int clevel = 60;
                    int slevel = 6;
                    int mlevel = 8;

                    // 升级和赋能
                    for (int index = 0; index < characters.Count; index++)
                    {
                        Character c = characters[index];
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

                        if (c == character1)
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

                        if (c == character2)
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

                        if (c == character3)
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

                        if (c == character4)
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

                        if (c == character5)
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

                        if (c == character6)
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

                        if (c == character7)
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

                        if (c == character8)
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

                        if (c == character9)
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

                        if (c == character10)
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

                        if (c == character11)
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

                        if (c == character12)
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
                    }

                    // 显示角色信息
                    if (PrintOut) characters.ForEach(c => Console.WriteLine(c.GetInfo()));

                    // 创建顺序表并排序
                    ActionQueue actionQueue = new(characters, WriteLine);
                    if (PrintOut) Console.WriteLine();

                    // 显示初始顺序表
                    actionQueue.DisplayQueue();
                    if (PrintOut) Console.WriteLine();

                    // 总游戏时长
                    double totalTime = 0;

                    // 开始空投
                    Msg = "";
                    空投(actionQueue, totalTime);
                    if (isWeb) result.Add("=== 空投 ===\r\n" + Msg);

                    // 总回合数
                    int i = 1;
                    while (i < 999)
                    {
                        Msg = "";
                        if (i == 998)
                        {
                            WriteLine($"=== 终局审判 ===");
                            Dictionary<Character, double> 他们的血量百分比 = [];
                            foreach (Character c in characters)
                            {
                                他们的血量百分比.TryAdd(c, c.HP / c.MaxHP);
                            }
                            double max = 他们的血量百分比.Values.Max();
                            Character winner = 他们的血量百分比.Keys.Where(c => 他们的血量百分比[c] == max).First();
                            WriteLine("[ " + winner + " ] 成为了天选之人！！");
                            foreach (Character c in characters.Where(c => c != winner && c.HP > 0))
                            {
                                WriteLine("[ " + winner + " ] 对 [ " + c + " ] 造成了 99999999999 点真实伤害。");
                                actionQueue.DeathCalculation(winner, c);
                            }
                            actionQueue.EndGameInfo(winner);
                            result.Add(Msg);
                            break;
                        }

                        // 检查是否有角色可以行动
                        Character? characterToAct = actionQueue.NextCharacter();

                        // 处理回合
                        if (characterToAct != null)
                        {
                            WriteLine($"=== Round {i++} ===");
                            WriteLine("现在是 [ " + characterToAct + " ] 的回合！");

                            bool isGameEnd = actionQueue.ProcessTurn(characterToAct);
                            if (isGameEnd)
                            {
                                result.Add(Msg);
                                break;
                            }

                            actionQueue.DisplayQueue();
                            WriteLine("");
                        }

                        // 模拟时间流逝
                        totalTime += actionQueue.TimeLapse();

                        if (actionQueue.Eliminated.Count > deaths)
                        {
                            deaths = actionQueue.Eliminated.Count;
                            if (!isWeb)
                            {
                                string roundMsg = Msg;
                                string[] strs = roundMsg.Split("==== 角色状态 ====");
                                if (strs.Length > 0)
                                {
                                    roundMsg = strs[0];
                                }
                                result.Add(roundMsg);
                            }
                            else result.Add(Msg);
                        }
                    }

                    if (PrintOut)
                    {
                        Console.WriteLine("--- End ---");
                        Console.WriteLine($"总游戏时长：{totalTime:0.##}");
                        Console.WriteLine("");
                    }

                    // 赛后统计
                    WriteLine("=== 伤害排行榜 ===");
                    int top = isWeb ? 12 : 6;
                    Msg = $"=== 伤害排行榜 TOP{top} ===\r\n";
                    int count = 1;
                    foreach (Character character in actionQueue.CharacterStatistics.OrderByDescending(d => d.Value.TotalDamage).Select(d => d.Key))
                    {
                        StringBuilder builder = new();
                        CharacterStatistics stats = actionQueue.CharacterStatistics[character];
                        builder.AppendLine($"{count}. [ {character.ToStringWithLevel()} ] （{stats.Kills} / {stats.Assists}）");
                        builder.AppendLine($"存活时长：{stats.LiveTime} / 存活回合数：{stats.LiveRound} / 行动回合数：{stats.ActionTurn}");
                        builder.AppendLine($"总计伤害：{stats.TotalDamage} / 总计物理伤害：{stats.TotalPhysicalDamage} / 总计魔法伤害：{stats.TotalMagicDamage}");
                        builder.AppendLine($"总承受伤害：{stats.TotalTakenDamage} / 总承受物理伤害：{stats.TotalTakenPhysicalDamage} / 总承受魔法伤害：{stats.TotalTakenMagicDamage}");
                        builder.Append($"每秒伤害：{stats.DamagePerSecond} / 每回合伤害：{stats.DamagePerTurn}");
                        if (count++ <= top)
                        {
                            WriteLine(builder.ToString());
                        }
                        else
                        {
                            if (PrintOut) Console.WriteLine(builder.ToString());
                        }

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
                    result.Add(Msg);

                    // 显示每个角色的信息
                    if (isWeb)
                    {
                        for (i = actionQueue.Eliminated.Count - 1; i >= 0; i--)
                        {
                            Character character = actionQueue.Eliminated[i];
                            result.Add($"=== 角色 [ {character} ] ===\r\n{character.GetInfo()}");
                        }
                    }

                    lock (StatsConfig)
                    {
                        foreach (Character c in CharacterStatistics.Keys)
                        {
                            StatsConfig.Add(c.ToStringWithOutUser(), CharacterStatistics[c]);
                        }
                        StatsConfig.SaveConfig();
                    }

                    IsRuning = false;
                }

                return result;
            }
            catch (Exception ex)
            {
                IsRuning = false;
                Console.WriteLine(ex);
                return [ex.ToString()];
            }
        }

        public static void WriteLine(string str)
        {
            Msg += str + "\r\n";
            if (PrintOut) Console.WriteLine(str);
        }

        public static void 空投(ActionQueue queue, double totalTime)
        {
            Item[] 这次发放的空投;
            if (totalTime == 0)
            {
                WriteLine("社区送温暖了，现在向所有人发放 [ 攻击之爪 +50 ]！！");
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

        public static void InitCharacter()
        {
            Characters.Add(new OshimaShiya());
            Characters.Add(new XinYin());
            Characters.Add(new Yang());
            Characters.Add(new NanGanYu());
            Characters.Add(new NiuNan());
            Characters.Add(new DokyoMayor());
            Characters.Add(new MagicalGirl());
            Characters.Add(new QingXiang());
            Characters.Add(new QWQAQW());
            Characters.Add(new ColdBlue());
            Characters.Add(new dddovo());
            Characters.Add(new Quduoduo());

            foreach (Character c in Characters)
            {
                CharacterStatistics.Add(c, new());
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
