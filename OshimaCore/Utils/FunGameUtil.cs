using System.Text;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model;
using Oshima.FunGame.OshimaModules;
using Oshima.FunGame.OshimaModules.Characters;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;
using Oshima.FunGame.OshimaModules.Items;
using Oshima.FunGame.OshimaModules.Skills;

namespace Oshima.Core.Utils
{
    public class FunGameSimulation
    {
        public static List<Character> Characters { get; } = [];
        public static List<Skill> Skills { get; } = [];
        public static List<Skill> Magics { get; } = [];
        public static List<Item> Equipment { get; } = [];
        public static List<Item> Items { get; } = [];
        public static Dictionary<Character, CharacterStatistics> CharacterStatistics { get; } = [];
        public static Dictionary<Character, CharacterStatistics> TeamCharacterStatistics { get; } = [];
        public static PluginConfig StatsConfig { get; } = new(nameof(FunGameSimulation), nameof(CharacterStatistics));
        public static PluginConfig TeamStatsConfig { get; } = new(nameof(FunGameSimulation), nameof(TeamCharacterStatistics));
        public static bool IsRuning { get; set; } = false;
        public static bool IsWeb { get; set; } = false;
        public static bool PrintOut { get; set; } = false;
        public static bool DeathMatchRoundDetail { get; set; } = false;
        public static string Msg { get; set; } = "";

        public static void InitFunGame()
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

            foreach (Character c in Characters)
            {
                TeamCharacterStatistics.Add(c, new());
            }

            TeamStatsConfig.LoadConfig();
            foreach (Character character in TeamCharacterStatistics.Keys)
            {
                if (TeamStatsConfig.ContainsKey(character.ToStringWithOutUser()))
                {
                    TeamCharacterStatistics[character] = TeamStatsConfig.Get<CharacterStatistics>(character.ToStringWithOutUser()) ?? TeamCharacterStatistics[character];
                }
            }

            Dictionary<string, Item> exItems = Factory.GetGameModuleInstances<Item>(OshimaGameModuleConstant.General, OshimaGameModuleConstant.Item);
            Equipment.AddRange(exItems.Values.Where(i => (int)i.ItemType >= 0 && (int)i.ItemType < 5));
            Equipment.AddRange([new 攻击之爪10(), new 攻击之爪30(), new 攻击之爪50()]);

            Items.AddRange(exItems.Values.Where(i => (int)i.ItemType > 4));

            Skills.AddRange([new 疾风步()]);

            Magics.AddRange([new 冰霜攻击(), new 火之矢(), new 水之矢(), new 风之轮(), new 石之锤(), new 心灵之霞(), new 次元上升(), new 暗物质(), new 回复术(), new 治愈术()]);
        }

        public static List<string> StartGame(bool printout, bool isWeb = false, bool isTeam = false, bool deathMatchRoundDetail = false)
        {
            PrintOut = printout;
            IsWeb = isWeb;
            DeathMatchRoundDetail = deathMatchRoundDetail;
            try
            {
                if (IsRuning) return ["游戏正在模拟中，请勿重复请求！"];

                List<string> result = [];
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

                        //IEnumerable<Skill> magics = Magics.OrderBy(x => Random.Shared.Next()).Take(3);
                        //foreach (Skill magic in magics)
                        //{
                        //    Skill m = magic.Copy();
                        //    m.Character = c;
                        //    m.Level = mlevel;
                        //    c.Skills.Add(m);
                        //}

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

                    // 创建顺序表并排序
                    ActionQueue actionQueue = new(characters, isTeam, WriteLine);
                    if (isTeam)
                    {
                        actionQueue.MaxRespawnTimes = -1;
                        actionQueue.MaxScoreToWin = 30;
                    }
                    if (PrintOut) Console.WriteLine();

                    // 总游戏时长
                    double totalTime = 0;

                    // 开始空投
                    Msg = "";
                    int 发放的卡包品质 = 0;
                    int 发放的武器品质 = 0;
                    int 发放的防具品质 = 0;
                    int 发放的鞋子品质 = 0;
                    int 发放的饰品品质 = 0;
                    空投(actionQueue, 发放的卡包品质, 发放的武器品质, 发放的防具品质, 发放的鞋子品质, 发放的饰品品质);
                    if (isWeb) result.Add("=== 空投 ===\r\n" + Msg);
                    double 下一次空投 = isTeam ? 80 : 40;
                    if (发放的卡包品质 < 4)
                    {
                        发放的卡包品质++;
                    }
                    if (发放的武器品质 < 4)
                    {
                        发放的武器品质++;
                    }
                    if (发放的防具品质 < 1)
                    {
                        发放的防具品质++;
                    }
                    if (发放的鞋子品质 < 1)
                    {
                        发放的鞋子品质++;
                    }
                    if (发放的饰品品质 < 3)
                    {
                        发放的饰品品质++;
                    }

                    // 显示角色信息
                    if (PrintOut) characters.ForEach(c => Console.WriteLine(c.GetInfo()));

                    // 因赋予了装备，所以清除排序重新排
                    actionQueue.ClearQueue();
                    actionQueue.InitCharacterQueue(characters);
                    if (PrintOut) Console.WriteLine();

                    // 团队模式
                    if (isTeam)
                    {
                        Msg = "=== 团队模式随机分组 ===\r\n\r\n";
                        // 打乱角色列表
                        List<Character> shuffledCharacters = [.. characters.OrderBy(c => Random.Shared.Next())];

                        // 创建两个团队
                        List<Character> group1 = [];
                        List<Character> group2 = [];

                        // 将角色交替分配到两个团队中
                        for (int cid = 0; cid < shuffledCharacters.Count; cid++)
                        {
                            if (cid % 2 == 0)
                            {
                                group1.Add(shuffledCharacters[cid]);
                            }
                            else
                            {
                                group2.Add(shuffledCharacters[cid]);
                            }
                        }

                        // 添加到团队字典
                        actionQueue.AddTeam("队伍一", group1);
                        actionQueue.AddTeam("队伍二", group2);

                        foreach (string team in actionQueue.Teams.Keys)
                        {
                            WriteLine($"团队【{team}】的成员：\r\n{string.Join("\r\n", actionQueue.Teams[team].Members)}\r\n");
                        }
                        result.Add(Msg);
                    }

                    // 显示初始顺序表
                    actionQueue.DisplayQueue();
                    if (PrintOut) Console.WriteLine();

                    // 总回合数
                    int maxRound = isTeam ? 9999 : 999;

                    // 随机回合奖励
                    Dictionary<int, List<Skill>> roundRewards = GenerateRoundRewards(maxRound);

                    int i = 1;
                    while (i < maxRound)
                    {
                        Msg = "";
                        if (i == maxRound - 1)
                        {
                            if (isTeam)
                            {
                                WriteLine("两队打到天昏地暗，流局了！！");
                                break;
                            }
                            else
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
                        }

                        // 检查是否有角色可以行动
                        Character? characterToAct = actionQueue.NextCharacter();

                        // 处理回合
                        if (characterToAct != null)
                        {
                            // 获取回合奖励
                            List<Skill> skillRewards = [];
                            if (roundRewards.TryGetValue(i, out List<Skill>? effectList) && effectList != null)
                            {
                                skillRewards = new(effectList);
                            }

                            WriteLine($"=== Round {i++} ===");
                            WriteLine("现在是 [ " + characterToAct + (isTeam ? "（" + (actionQueue.GetTeam(characterToAct)?.Name ?? "") + "）" : "") + " ] 的回合！");

                            // 实际的回合奖励
                            List<Skill> realSkillRewards = [];
                            if (skillRewards.Count > 0)
                            {
                                foreach (Skill skill in skillRewards)
                                {
                                    Dictionary<string, object> effectArgs = [];
                                    if (RoundRewards.TryGetValue((EffectID)skill.Id, out Dictionary<string, object>? dict) && dict != null)
                                    {
                                        effectArgs = new(dict);
                                    }
                                    Dictionary<string, object> args = new()
                                    {
                                        { "skill", skill },
                                        { "values", effectArgs }
                                    };
                                    skill.GamingQueue = actionQueue;
                                    skill.Effects.Add(Factory.OpenFactory.GetInstance<Effect>(skill.Id, "", args));
                                    skill.Character = characterToAct;
                                    skill.Level = 1;
                                    actionQueue.LastRound.RoundRewards.Add(skill);
                                    WriteLine($"[ {characterToAct} ] 获得了回合奖励！{skill.Description}".Trim());
                                    if (skill.IsActive)
                                    {
                                        actionQueue.LastRound.Targets.Add(characterToAct);
                                        skill.OnSkillCasted(actionQueue, characterToAct, [characterToAct]);
                                    }
                                    else
                                    {
                                        characterToAct.Skills.Add(skill);
                                        realSkillRewards.Add(skill);
                                    }
                                }
                            }

                            bool isGameEnd = actionQueue.ProcessTurn(characterToAct);

                            if (realSkillRewards.Count > 0)
                            {
                                foreach (Skill skill in realSkillRewards)
                                {
                                    foreach (Effect e in skill.Effects)
                                    {
                                        e.OnEffectLost(characterToAct);
                                        characterToAct.Effects.Remove(e);
                                    }
                                    characterToAct.Skills.Remove(skill);
                                    skill.Character = null;
                                }
                            }

                            if (isGameEnd)
                            {
                                result.Add(Msg);
                                break;
                            }

                            actionQueue.DisplayQueue();
                            WriteLine("");
                        }

                        string roundMsg = "";
                        if (actionQueue.LastRound.HasKill)
                        {
                            roundMsg = Msg;
                            if (!deathMatchRoundDetail)
                            {
                                roundMsg = actionQueue.LastRound.ToString();
                            }
                            Msg = "";
                        }

                        // 模拟时间流逝
                        double timeLapse = actionQueue.TimeLapse();
                        totalTime += timeLapse;
                        下一次空投 -= timeLapse;

                        if (roundMsg != "")
                        {
                            if ((isTeam && deathMatchRoundDetail || !isTeam) && isWeb)
                            {
                                roundMsg += "\r\n" + Msg;
                            }
                            result.Add(roundMsg);
                        }

                        if (下一次空投 <= 0)
                        {
                            // 空投
                            Msg = "";
                            空投(actionQueue, 发放的卡包品质, 发放的武器品质, 发放的防具品质, 发放的鞋子品质, 发放的饰品品质);
                            if (isWeb) result.Add("=== 空投 ===\r\n" + Msg);
                            下一次空投 = isTeam ? 100 : 40;
                            if (发放的卡包品质 < 4)
                            {
                                发放的卡包品质++;
                            }
                            if (发放的武器品质 < 4)
                            {
                                发放的武器品质++;
                            }
                            if (发放的防具品质 < 1)
                            {
                                发放的防具品质++;
                            }
                            if (发放的鞋子品质 < 1)
                            {
                                发放的鞋子品质++;
                            }
                            if (发放的饰品品质 < 3)
                            {
                                发放的饰品品质++;
                            }
                        }
                    }

                    if (PrintOut)
                    {
                        Console.WriteLine("--- End ---");
                        Console.WriteLine($"总游戏时长：{totalTime:0.##}");
                        Console.WriteLine("");
                    }

                    // 赛后统计
                    GetCharacterRating(actionQueue.CharacterStatistics, isTeam, actionQueue.EliminatedTeams);
                    int top = isWeb ? actionQueue.CharacterStatistics.Count : 0; // 回执多少个角色的统计信息
                    int count = 1;
                    if (isWeb)
                    {
                        WriteLine("=== 技术得分排行榜 ==="); // 这是输出在界面上的
                        Msg = $"=== 技术得分排行榜 TOP{top} ===\r\n"; // 这个是下一条给Result回执的标题，覆盖掉上面方法里的赋值了
                    }
                    else
                    {
                        WriteLine("=== 本场比赛最佳角色 ===");
                        Msg = $"=== 本场比赛最佳角色 ===\r\n";
                        // 统计技术得分
                        Character? character = actionQueue.CharacterStatistics.OrderByDescending(d => d.Value.Rating).Select(d => d.Key).FirstOrDefault();
                        if (character != null)
                        {
                            CharacterStatistics stats = actionQueue.CharacterStatistics[character];
                            stats.MVPs++;
                            StringBuilder builder = new();
                            builder.AppendLine($"{(isWeb ? count + "." : (isTeam ? "[ " + actionQueue.GetTeamFromEliminated(character)?.Name + " ]" ?? "" : ""))} [ {character.ToStringWithLevel()} ]");
                            builder.AppendLine($"技术得分：{stats.Rating:0.##} / 击杀数：{stats.Kills} / 助攻数：{stats.Assists}{(actionQueue.MaxRespawnTimes != 0 ? " / 死亡数：" + stats.Deaths : "")}");
                            builder.AppendLine($"存活时长：{stats.LiveTime} / 存活回合数：{stats.LiveRound} / 行动回合数：{stats.ActionTurn}");
                            builder.AppendLine($"总计伤害：{stats.TotalDamage} / 总计物理伤害：{stats.TotalPhysicalDamage} / 总计魔法伤害：{stats.TotalMagicDamage}");
                            builder.AppendLine($"总承受伤害：{stats.TotalTakenDamage} / 总承受物理伤害：{stats.TotalTakenPhysicalDamage} / 总承受魔法伤害：{stats.TotalTakenMagicDamage}");
                            builder.Append($"每秒伤害：{stats.DamagePerSecond} / 每回合伤害：{stats.DamagePerTurn}");
                            WriteLine(builder.ToString());
                        }
                    }

                    if (PrintOut)
                    {
                        Console.WriteLine();
                        Console.WriteLine("=== 技术得分排行榜 ===");
                    }

                    if (isTeam)
                    {
                        foreach (Character character in actionQueue.CharacterStatistics.OrderByDescending(d => d.Value.Rating).Select(d => d.Key))
                        {
                            StringBuilder builder = new();
                            CharacterStatistics stats = actionQueue.CharacterStatistics[character];
                            builder.AppendLine($"{(isWeb ? count + "." : ("[ " + actionQueue.GetTeamFromEliminated(character)?.Name + " ]" ?? ""))} [ {character.ToStringWithLevel()} ]");
                            builder.AppendLine($"技术得分：{stats.Rating:0.##} / 击杀数：{stats.Kills} / 助攻数：{stats.Assists}{(actionQueue.MaxRespawnTimes != 0 ? " / 死亡数：" + stats.Deaths : "")}");
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

                            CharacterStatistics? totalStats = TeamCharacterStatistics.Where(kv => kv.Key.GetName() == character.GetName()).Select(kv => kv.Value).FirstOrDefault();
                            if (totalStats != null)
                            {
                                UpdateStatistics(totalStats, stats);
                            }
                        }
                    }
                    else
                    {
                        foreach (Character character in actionQueue.CharacterStatistics.OrderByDescending(d => d.Value.Rating).Select(d => d.Key))
                        {
                            StringBuilder builder = new();
                            CharacterStatistics stats = actionQueue.CharacterStatistics[character];
                            builder.AppendLine($"{(isWeb ? count + ". " : "")}[ {character.ToStringWithLevel()} ]");
                            builder.AppendLine($"技术得分：{stats.Rating:0.##} / 击杀数：{stats.Kills} / 助攻数：{stats.Assists}{(actionQueue.MaxRespawnTimes != 0 ? " / 死亡数：" + stats.Deaths : "")}");
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
                                UpdateStatistics(totalStats, stats);
                            }
                        }
                    }

                    result.Add(Msg);

                    // 显示每个角色的信息
                    if (isWeb)
                    {
                        if (isTeam)
                        {
                            top = 1;
                            for (i = actionQueue.EliminatedTeams.Count - 1; i >= 0; i--)
                            {
                                Team team = actionQueue.EliminatedTeams[i];
                                string topTeam = "";
                                if (top == 1)
                                {
                                    topTeam = "冠军";
                                }
                                if (top == 2)
                                {
                                    topTeam = "亚军";
                                }
                                if (top == 3)
                                {
                                    topTeam = "季军";
                                }
                                if (top > 3)
                                {
                                    topTeam = $"第 {top} 名";
                                }
                                foreach (Character character in team.Members)
                                {
                                    result.Add($"== {topTeam}团队 [ {team.Name} ] ==\r\n== 角色：[ {character} ] ==\r\n{character.GetInfo()}");
                                }
                                top++;
                            }
                        }
                        else
                        {
                            for (i = actionQueue.Eliminated.Count - 1; i >= 0; i--)
                            {
                                Character character = actionQueue.Eliminated[i];
                                result.Add($"=== 角色 [ {character} ] ===\r\n{character.GetInfo()}");
                            }
                        }
                    }

                    if (isTeam)
                    {
                        lock (TeamStatsConfig)
                        {
                            foreach (Character c in TeamCharacterStatistics.Keys)
                            {
                                TeamStatsConfig.Add(c.ToStringWithOutUser(), TeamCharacterStatistics[c]);
                            }
                            TeamStatsConfig.SaveConfig();
                        }
                    }
                    else
                    {
                        lock (StatsConfig)
                        {
                            foreach (Character c in CharacterStatistics.Keys)
                            {
                                StatsConfig.Add(c.ToStringWithOutUser(), CharacterStatistics[c]);
                            }
                            StatsConfig.SaveConfig();
                        }
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

        public static void 空投(ActionQueue queue, int mQuality, int wQuality, int aQuality, int sQuality, int acQuality)
        {
            WriteLine($"社区送温暖了，现在随机发放空投！！");
            foreach (Character character in queue.Queue)
            {
                Item[] 武器 = Equipment.Where(i => i.Id.ToString().StartsWith("11") && (int)i.QualityType == wQuality).ToArray();
                Item[] 防具 = Equipment.Where(i => i.Id.ToString().StartsWith("12") && (int)i.QualityType == aQuality).ToArray();
                Item[] 鞋子 = Equipment.Where(i => i.Id.ToString().StartsWith("13") && (int)i.QualityType == sQuality).ToArray();
                Item[] 饰品 = Equipment.Where(i => i.Id.ToString().StartsWith("14") && (int)i.QualityType == acQuality).ToArray();
                Item? a = null, b = null, c = null, d = null;
                if (武器.Length > 0)
                {
                    a = 武器[Random.Shared.Next(武器.Length)];
                }
                if (防具.Length > 0)
                {
                    b = 防具[Random.Shared.Next(防具.Length)];
                }
                if (鞋子.Length > 0)
                {
                    c = 鞋子[Random.Shared.Next(鞋子.Length)];
                }
                if (饰品.Length > 0)
                {
                    d = 饰品[Random.Shared.Next(饰品.Length)];
                }
                List<Item> 这次发放的空投 = [];
                if (a != null) 这次发放的空投.Add(a);
                if (b != null) 这次发放的空投.Add(b);
                if (c != null) 这次发放的空投.Add(c);
                if (d != null) 这次发放的空投.Add(d);
                Item? 魔法卡包 = GenerateMagicCardPack(3, (QualityType)mQuality);
                if (魔法卡包 != null)
                {
                    foreach (Skill magic in 魔法卡包.Skills.Magics)
                    {
                        magic.Level = 8;
                    }
                    魔法卡包.SetGamingQueue(queue);
                    queue.Equip(character, 魔法卡包);
                }
                foreach (Item item in 这次发放的空投)
                {
                    Item realItem = item.Copy();
                    realItem.SetGamingQueue(queue);
                    queue.Equip(character, realItem);
                }
            }
            WriteLine("");
        }

        public static List<Item> GenerateMagicCards(int count, QualityType? qualityType = null)
        {
            List<Item> items = [];

            for (int i = 0; i < count; i++)
            {
                items.Add(GenerateMagicCard(qualityType));
            }

            return items;
        }
        
        public static Item GenerateMagicCard(QualityType? qualityType = null)
        {
            Item item = Factory.GetItem();
            item.Id = Convert.ToInt64("16" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 8));
            item.Name = GenerateRandomChineseName();
            item.ItemType = ItemType.MagicCard;

            int total;
            if (qualityType != null)
            {
                total = qualityType switch
                {
                    QualityType.Green => Random.Shared.Next(7, 13),
                    QualityType.Blue => Random.Shared.Next(13, 19),
                    QualityType.Purple => Random.Shared.Next(19, 25),
                    QualityType.Orange => Random.Shared.Next(25, 31),
                    _ => Random.Shared.Next(1, 7)
                };
                item.QualityType = (QualityType)qualityType;
            }
            else
            {
                total = Random.Shared.Next(1, 31);
                if (total > 6 && total <= 12)
                {
                    item.QualityType = QualityType.Green;
                }
                else if (total > 12 && total <= 18)
                {
                    item.QualityType = QualityType.Blue;
                }
                else if (total > 18 && total <= 24)
                {
                    item.QualityType = QualityType.Purple;
                }
                else if (total > 24 && total <= 30)
                {
                    item.QualityType = QualityType.Orange;
                }
            }

            GenerateAndAddSkillToMagicCard(item, total);

            return item;
        }

        public static void GenerateAndAddSkillToMagicCard(Item item, int total)
        {
            Skill magic = Magics[Random.Shared.Next(Magics.Count)].Copy();
            magic.Guid = item.Guid;
            magic.Level = (int)item.QualityType switch
            {
                2 => 2,
                4 => 3,
                6 => 4,
                _ => 1
            };
            item.Skills.Active = magic;

            // 初始化属性值
            int str = 0, agi = 0, intelligence = 0;

            // 随机决定将多少个属性赋给其中一个属性，确保至少一个不为零
            int nonZeroAttributes = Random.Shared.Next(1, Math.Min(4, total + 1)); // 随机决定非零属性的数量，确保在 total = 1 时最多只有1个非零属性

            // 根据非零属性数量分配属性点
            if (nonZeroAttributes == 1)
            {
                // 只有一个属性不为零
                int attribute = Random.Shared.Next(0, 3);
                if (attribute == 0) str = total;
                else if (attribute == 1) agi = total;
                else intelligence = total;
            }
            else if (nonZeroAttributes == 2 && total >= 2)
            {
                // 两个属性不为零
                int first = Random.Shared.Next(1, total); // 第一个属性的值
                int second = total - first; // 第二个属性的值

                int attribute = Random.Shared.Next(0, 3);
                if (attribute == 0)
                {
                    str = first;
                }
                else if (attribute == 1)
                {
                    agi = first;
                }
                else
                {
                    intelligence = first;
                }

                attribute = Random.Shared.Next(0, 3);
                while ((attribute == 0 && str > 0) || (attribute == 1 && agi > 0) || (attribute == 2 && intelligence > 0))
                {
                    attribute = Random.Shared.Next(0, 3);
                }

                if (attribute == 0)
                {
                    str = second;
                }
                else if (attribute == 1)
                {
                    agi = second;
                }
                else
                {
                    intelligence = second;
                }
            }
            else if (total >= 3)
            {
                // 三个属性都不为零
                str = Random.Shared.Next(1, total - 1); // 第一个属性的值
                agi = Random.Shared.Next(1, total - str); // 第二个属性的值
                intelligence = total - str - agi; // 剩下的值给第三个属性
            }

            Skill skill = Factory.OpenFactory.GetInstance<Skill>(item.Id, item.Name, []);
            GenerateAndAddEffectsToMagicCard(skill, str, agi, intelligence);

            if (magic.Level > 1) item.Name += $" +{magic.Level - 1}";
            skill.Level = 1;
            List<string> strings = [];
            if (str > 0) strings.Add($"{str:0.##} 点力量");
            if (agi > 0) strings.Add($"{agi:0.##} 点敏捷");
            if (intelligence > 0) strings.Add($"{intelligence:0.##} 点智力");
            item.Description = $"包含魔法：{item.Skills.Active.Name}\r\n" +
                $"增加角色属性：{string.Join("，", strings)}";
            item.Skills.Passives.Add(skill);
        }

        public static void GenerateAndAddEffectsToMagicCard(Skill skill, int str, int agi, int intelligence)
        {
            if (str > 0)
            {
                skill.Effects.Add(Factory.OpenFactory.GetInstance<Effect>((long)EffectID.ExSTR, "", new()
                    {
                        { "skill", skill },
                        {
                            "values", new Dictionary<string, object>()
                            {
                                { "exstr", str }
                            }
                        }
                    }));
            }

            if (agi > 0)
            {
                skill.Effects.Add(Factory.OpenFactory.GetInstance<Effect>((long)EffectID.ExAGI, "", new()
                    {
                        { "skill", skill },
                        {
                            "values", new Dictionary<string, object>()
                            {
                                { "exagi", agi }
                            }
                        }
                    }));
            }

            if (intelligence > 0)
            {
                skill.Effects.Add(Factory.OpenFactory.GetInstance<Effect>((long)EffectID.ExINT, "", new()
                    {
                        { "skill", skill },
                        {
                            "values", new Dictionary<string, object>()
                            {
                                { "exint", intelligence }
                            }
                        }
                    }));
            }
        }

        public static Item? ConflateMagicCardPack(IEnumerable<Item> magicCards)
        {
            if (magicCards.Any())
            {
                List<Skill> magics = [.. magicCards.Where(i => i.Skills.Active != null).Select(i => i.Skills.Active)];
                List<Skill> passives = [.. magicCards.SelectMany(i => i.Skills.Passives)];
                Item item = Factory.GetItem();
                item.Id = Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 8));
                item.Name = GenerateRandomChineseName();
                item.ItemType = ItemType.MagicCardPack;
                double str = 0, agi = 0, intelligence = 0;
                foreach (Skill skill in passives)
                {
                    Skill newSkill = skill.Copy();
                    foreach (Effect effect in newSkill.Effects)
                    {
                        switch ((EffectID)effect.Id)
                        {
                            case EffectID.ExSTR:
                                if (effect is ExSTR exstr)
                                {
                                    str += exstr.Value;
                                }
                                break;
                            case EffectID.ExAGI:
                                if (effect is ExAGI exagi)
                                {
                                    agi += exagi.Value;
                                }
                                break;
                            case EffectID.ExINT:
                                if (effect is ExINT exint)
                                {
                                    intelligence += exint.Value;
                                }
                                break;
                        }
                    }
                    newSkill.Level = skill.Level;
                    item.Skills.Passives.Add(newSkill);
                }
                List<string> strings = [];
                if (str > 0) strings.Add($"{str:0.##} 点力量");
                if (agi > 0) strings.Add($"{agi:0.##} 点敏捷");
                if (intelligence > 0) strings.Add($"{intelligence:0.##} 点智力");
                foreach (Skill skill in magics)
                {
                    IEnumerable<Skill> has = item.Skills.Magics.Where(m => m.Id == skill.Id);
                    if (has.Any() && has.First() is Skill s)
                    {
                        s.Level += skill.Level;
                        if (s.Level > 1) s.Name = s.Name.Split(' ')[0] + $" +{s.Level - 1}";
                    }
                    else
                    {
                        Skill magic = skill.Copy();
                        magic.Guid = item.Guid;
                        magic.Level = skill.Level;
                        item.Skills.Magics.Add(magic);
                    }
                }
                item.Description = $"包含魔法：{string.Join("，", item.Skills.Magics.Select(m => m.Name + (m.Level > 1 ? $" +{m.Level - 1}" : "")))}\r\n" +
                    $"增加角色属性：{string.Join("，", strings)}";
                double total = str + agi + intelligence;
                if (total > 18 && total <= 36)
                {
                    item.QualityType = QualityType.Green;
                }
                else if (total > 36 && total <= 54)
                {
                    item.QualityType = QualityType.Blue;
                }
                else if (total > 54 && total <= 72)
                {
                    item.QualityType = QualityType.Purple;
                }
                else if (total > 72 && total <= 90)
                {
                    item.QualityType = QualityType.Orange;
                }
                return item;
            }
            return null;
        }

        public static Item? GenerateMagicCardPack(int magicCardCount, QualityType? qualityType = null)
        {
            List<Item> magicCards = GenerateMagicCards(magicCardCount, qualityType);
            Item? magicCardPack = ConflateMagicCardPack(magicCards);
            return magicCardPack;
        }

        public static void Reload()
        {
            Characters.Clear();
            CharacterStatistics.Clear();
            TeamCharacterStatistics.Clear();
            Equipment.Clear();
            Skills.Clear();
            Magics.Clear();

            InitFunGame();
        }

        public static Dictionary<EffectID, Dictionary<string, object>> RoundRewards
        {
            get
            {
                return new()
                {
                    {
                        EffectID.ExATK,
                        new()
                        {
                            { "exatk", Random.Shared.Next(40, 80) }
                        }
                    },
                    {
                        EffectID.ExCritRate,
                        new()
                        {
                            { "excr", Math.Clamp(Random.Shared.NextDouble(), 0.25, 0.5) }
                        }
                    },
                    {
                        EffectID.ExCritDMG,
                        new()
                        {
                            { "excrd", Math.Clamp(Random.Shared.NextDouble(), 0.5, 1) }
                        }
                    },
                    {
                        EffectID.ExATK2,
                        new()
                        {
                            { "exatk", Math.Clamp(Random.Shared.NextDouble(), 0.15, 0.3) }
                        }
                    },
                    {
                        EffectID.RecoverHP,
                        new()
                        {
                            { "hp", Random.Shared.Next(160, 640) }
                        }
                    },
                    {
                        EffectID.RecoverMP,
                        new()
                        {
                            { "mp", Random.Shared.Next(140, 490) }
                        }
                    },
                    {
                        EffectID.RecoverHP2,
                        new()
                        {
                            { "hp", Math.Clamp(Random.Shared.NextDouble(), 0.04, 0.08) }
                        }
                    },
                    {
                        EffectID.RecoverMP2,
                        new()
                        {
                            { "mp", Math.Clamp(Random.Shared.NextDouble(), 0.09, 0.18) }
                        }
                    }
                };
            }
        }

        public static Dictionary<int, List<Skill>> GenerateRoundRewards(int maxRound)
        {
            Dictionary<int, List<Skill>> roundRewards = [];

            int currentRound = 1;
            while (currentRound <= maxRound)
            {
                currentRound += Random.Shared.Next(1, 9);

                if (currentRound <= maxRound)
                {
                    List<Skill> skills = [];

                    // 添加回合奖励特效
                    long effectID = (long)RoundRewards.Keys.ToArray()[Random.Shared.Next(RoundRewards.Count)];
                    Dictionary<string, object> args = [];
                    if (effectID > (long)EffectID.Active_Start)
                    {
                        args.Add("active", true);
                        args.Add("self", true);
                        args.Add("enemy", false);
                    }

                    skills.Add(Factory.OpenFactory.GetInstance<Skill>(effectID, "回合奖励", args));

                    roundRewards[currentRound] = skills;
                }
            }

            return roundRewards;
        }

        public static void UpdateStatistics(CharacterStatistics totalStats, CharacterStatistics stats)
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
            totalStats.FirstKills += stats.FirstKills;
            totalStats.FirstDeaths += stats.FirstDeaths;
            totalStats.LastRank = stats.LastRank;
            double totalRank = totalStats.AvgRank * totalStats.Plays + totalStats.LastRank;
            double totalRating = totalStats.Rating * totalStats.Plays + stats.Rating;
            totalStats.Plays += stats.Plays;
            if (totalStats.Plays != 0) totalStats.AvgRank = Calculation.Round2Digits(totalRank / totalStats.Plays);
            else totalStats.AvgRank = stats.LastRank;
            if (totalStats.Plays != 0) totalStats.Rating = Calculation.Round4Digits(totalRating / totalStats.Plays);
            else totalStats.Rating = stats.Rating;
            totalStats.Wins += stats.Wins;
            totalStats.Top3s += stats.Top3s;
            totalStats.Loses += stats.Loses;
            totalStats.MVPs += stats.MVPs;
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

        public static void GetCharacterRating(Dictionary<Character, CharacterStatistics> statistics, bool isTeam, List<Team> teams)
        {
            foreach (Character character in statistics.Keys)
            {
                Team? team = null;
                if (isTeam)
                {
                    team = teams.Where(t => t.IsOnThisTeam(character)).FirstOrDefault();
                }
                statistics[character].Rating = CalculateRating(statistics[character], team);
            }
        }

        public static double CalculateRating(CharacterStatistics stats, Team? team = null)
        {
            // 基础得分
            double baseScore = (stats.Kills + stats.Assists) / (stats.Kills + stats.Assists + stats.Deaths + 0.01);
            if (team is null)
            {
                baseScore += stats.Kills * 0.1;
                if (stats.Deaths == 0)
                {
                    baseScore += 0.5;
                }
            }
            else
            {
                baseScore = baseScore * 0.6 + 0.4 * (stats.Kills / (stats.Kills + stats.Deaths + 0.01));
            }

            // 伤害贡献
            double logDamageContribution = Math.Log(1 + (stats.TotalDamage / (stats.TotalTakenDamage + 1e-6)));

            // 存活时间贡献
            double liveTimeContribution = Math.Log(1 + (stats.LiveTime / (stats.TotalTakenDamage + 0.01) * 100));

            // 团队模式参团率加成
            double teamContribution = 0;
            if (team != null)
            {
                teamContribution = (stats.Kills + stats.Assists) / (team.Score + 0.01);
                if (team.IsWinner)
                {
                    teamContribution += 0.15;
                }
            }

            // 权重设置
            double k = stats.Deaths > 0 ? 0.2 : 0.075; // 伤害贡献权重
            double l = stats.Deaths > 0 ? 0.2 : 0.05; // 存活时间权重
            double t = stats.Deaths > 0 ? 0.2 : 0.075; // 参团率权重

            // 计算最终评分
            double rating = baseScore + k * logDamageContribution + l * liveTimeContribution + t * teamContribution;

            // 确保评分在合理范围内
            return Math.Max(0.01, rating);
        }

        public static string GenerateRandomChineseName()
        {
            // 定义一个包含常用汉字的字符串
            string commonChineseCharacters = "云星宝灵梦龙花雨风叶山川月石羽水竹金" +
                "玉海火雷光天地云凤虎虹珠华霞鹏雪银沙松桃兰竹青霜鸿康龙骏波泉河湖江泽" +
                "洋林枫梅桂樱桐竹晴韵凌兰若悠碧涛渊风雷壁石剑影霖玄承珍雅耀星瑞龙鹤烟" +
                "影凤燕霏翼羽翔璃绮纱绫绣锦瑜琼瑾璇璧琳琪琳瑶瑛芝杏茜荷莉莹菡莲诗羽珍" +
                "瑰翠椒槐榆槿柱梧桐曜曙晶暖智煌熙灵霓珠熠燕熹熠碧瑶琳嘉琪瑶琴瑶琴碧曼" +
                "菁蓉菲瑾淑妙惠嘉华秋涵智映巧慧茹瑜瑶荣菱霏曦容芬玲瑛琪瑜澜碧影凌清涛" +
                "湘泽澄泓泓翠澜润璇珺湘晨曦晶翠瑾澜涟润淑洁悠雅翠霏涵淑珍绮翠润";

            // 随机生成名字长度，2到5个字
            int nameLength = Random.Shared.Next(2, 6);
            StringBuilder name = new();

            for (int i = 0; i < nameLength; i++)
            {
                // 从常用汉字集中随机选择一个汉字
                char chineseCharacter = commonChineseCharacters[Random.Shared.Next(commonChineseCharacters.Length)];
                name.Append(chineseCharacter);
            }

            return name.ToString();
        }

        public static User GetUser(PluginConfig pc)
        {
            User user = pc.Get<User>("user") ?? Factory.GetUser();

            List<Character> characters = new(user.Inventory.Characters);
            List<Item> items = new(user.Inventory.Items);
            user.Inventory.Characters.Clear();
            user.Inventory.Items.Clear();

            foreach (Character inventoryCharacter in characters)
            {
                Character realCharacter = CharacterBuilder.Build(inventoryCharacter, false);
                realCharacter.User = user;
                user.Inventory.Characters.Add(realCharacter);
            }

            foreach (Item inventoryItem in items)
            {
                Item realItem = inventoryItem.Copy(true, true);
                if (realItem.IsEquipment)
                {
                    IEnumerable<Character> has = user.Inventory.Characters.Where(character =>
                    {
                        if (realItem.ItemType == ItemType.MagicCardPack && character.EquipSlot.MagicCardPack != null && realItem.Guid == character.EquipSlot.MagicCardPack.Guid)
                        {
                            return true;
                        }
                        if (realItem.ItemType == ItemType.Weapon && character.EquipSlot.Weapon != null && realItem.Guid == character.EquipSlot.Weapon.Guid)
                        {
                            return true;
                        }
                        if (realItem.ItemType == ItemType.Armor && character.EquipSlot.Armor != null && realItem.Guid == character.EquipSlot.Armor.Guid)
                        {
                            return true;
                        }
                        if (realItem.ItemType == ItemType.Shoes && character.EquipSlot.Shoes != null && realItem.Guid == character.EquipSlot.Shoes.Guid)
                        {
                            return true;
                        }
                        if (realItem.ItemType == ItemType.Accessory)
                        {
                            if (character.EquipSlot.Accessory1 != null && realItem.Guid == character.EquipSlot.Accessory1.Guid)
                            {
                                return true;
                            }
                            else if (character.EquipSlot.Accessory2 != null && realItem.Guid == character.EquipSlot.Accessory2.Guid)
                            {
                                return true;
                            }
                        }
                        return false;
                    });
                    if (has.Any() && has.First() is Character character)
                    {
                        realItem.Character = character;
                    }
                }
                user.Inventory.Items.Add(realItem);
            }

            return user;
        }
    }
}
