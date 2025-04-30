using System.Text;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;
using Oshima.FunGame.OshimaModules.Skills;

namespace Oshima.FunGame.OshimaServers.Service
{
    public class FunGameSimulation
    {
        public static Dictionary<Character, CharacterStatistics> CharacterStatistics { get; } = [];
        public static Dictionary<Character, CharacterStatistics> TeamCharacterStatistics { get; } = [];
        public static PluginConfig StatsConfig { get; } = new("FunGameSimulation", nameof(CharacterStatistics));
        public static PluginConfig TeamStatsConfig { get; } = new("FunGameSimulation", nameof(TeamCharacterStatistics));
        public static PluginConfig LastRecordConfig { get; } = new("FunGameSimulation", "LastRecord");
        public static bool IsRuning { get; set; } = false;
        public static bool IsWeb { get; set; } = false;
        public static bool PrintOut { get; set; } = false;
        public static bool DeathMatchRoundDetail { get; set; } = false;
        public static string Msg { get; set; } = "";

        public static void InitFunGameSimulation()
        {
            CharacterStatistics.Clear();
            TeamCharacterStatistics.Clear();
            LastRecordConfig.Clear();

            foreach (Character c in FunGameConstant.Characters)
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

            foreach (Character c in FunGameConstant.Characters)
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
        }

        public static async Task<List<string>> StartSimulationGame(bool printout, bool isWeb = false, bool isTeam = false, bool deathMatchRoundDetail = false, int maxRespawnTimesMix = 1)
        {
            PrintOut = printout;
            IsWeb = isWeb;
            DeathMatchRoundDetail = deathMatchRoundDetail;
            try
            {
                if (IsRuning) return ["游戏正在模拟中，请勿重复请求！"];

                List<string> result = [];
                List<string> lastRecord = [];
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

                List<Character> list = [.. FunGameConstant.Characters];

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
                        FunGameService.AddCharacterSkills(c, 1, slevel, slevel);
                        Skill 疾风步 = new 疾风步(c)
                        {
                            Level = slevel
                        };
                        c.Skills.Add(疾风步);
                    }

                    // 创建顺序表并排序
                    GamingQueue actionQueue;
                    MixGamingQueue? mgq = null;
                    TeamGamingQueue? tgq = null;
                    if (isTeam)
                    {
                        tgq = new TeamGamingQueue(characters, WriteLine)
                        {
                            MaxRespawnTimes = -1,
                            MaxScoreToWin = 30
                        };
                        actionQueue = tgq;
                    }
                    else
                    {
                        mgq = new MixGamingQueue(characters, WriteLine)
                        {
                            MaxRespawnTimes = maxRespawnTimesMix
                        };
                        actionQueue = mgq;
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
                    WriteLine($"社区送温暖了，现在随机发放空投！！");
                    空投(actionQueue, 发放的卡包品质, 发放的武器品质, 发放的防具品质, 发放的鞋子品质, 发放的饰品品质);
                    WriteLine("");
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
                    actionQueue.SetCharactersToAIControl(false, characters);
                    if (PrintOut) Console.WriteLine();

                    // 团队模式
                    if (isTeam && actionQueue is TeamGamingQueue tg)
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
                        tg.AddTeam("队伍一", group1);
                        tg.AddTeam("队伍二", group2);

                        foreach (string team in tg.Teams.Keys)
                        {
                            WriteLine($"团队【{team}】的成员：\r\n{string.Join("\r\n", tg.Teams[team].Members)}\r\n");
                        }
                        result.Add(Msg);
                    }

                    // 显示初始顺序表
                    actionQueue.DisplayQueue();
                    if (PrintOut) Console.WriteLine();

                    // 总回合数
                    int maxRound = isTeam ? 9999 : 999;

                    // 随机回合奖励
                    Dictionary<long, bool> effects = [];
                    foreach (EffectID id in FunGameConstant.RoundRewards.Keys)
                    {
                        long effectID = (long)id;
                        bool isActive = false;
                        if (effectID > (long)EffectID.Active_Start)
                        {
                            isActive = true;
                        }
                        effects.Add(effectID, isActive);
                    }
                    actionQueue.InitRoundRewards(maxRound, 1, effects, id => FunGameConstant.RoundRewards[(EffectID)id]);

                    int i = 1;
                    while (i < maxRound)
                    {
                        if (i != 1)
                        {
                            lastRecord.Add(actionQueue.LastRound.ToString());
                        }

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
                                    await actionQueue.DeathCalculationAsync(winner, c);
                                }
                                result.Add(Msg);
                                mgq?.EndGameInfo(winner);
                                break;
                            }
                        }

                        // 检查是否有角色可以行动
                        Character? characterToAct = await actionQueue.NextCharacterAsync();

                        // 处理回合
                        if (characterToAct != null)
                        {
                            WriteLine($"=== Round {i++} ===");
                            WriteLine("现在是 [ " + characterToAct + (tgq != null ? "（" + (tgq.GetTeam(characterToAct)?.Name ?? "") + "）" : "") + " ] 的回合！");

                            bool isGameEnd = await actionQueue.ProcessTurnAsync(characterToAct);

                            if (isGameEnd)
                            {
                                result.Add(Msg);
                                lastRecord.Add(actionQueue.LastRound.ToString());
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
                                roundMsg = actionQueue.LastRound.ToString().Trim() + $"\r\n{(tgq != null ? $"比分：{string.Join(" / ", tgq.Teams.Values.Select(t => $"{t.Name}({t.Score})"))}，击杀来自{tgq.GetTeam(tgq.LastRound.Actor)}" : "")}\r\n";
                            }
                            Msg = "";
                        }

                        // 模拟时间流逝
                        double timeLapse = await actionQueue.TimeLapse();
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
                            WriteLine($"社区送温暖了，现在随机发放空投！！");
                            空投(actionQueue, 发放的卡包品质, 发放的武器品质, 发放的防具品质, 发放的鞋子品质, 发放的饰品品质);
                            WriteLine("");
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
                    FunGameService.GetCharacterRating(actionQueue.CharacterStatistics, isTeam, tgq != null ? tgq.EliminatedTeams : []);

                    // 统计技术得分，评选 MVP
                    Character? mvp = actionQueue.CharacterStatistics.OrderByDescending(d => d.Value.Rating).Select(d => d.Key).FirstOrDefault();
                    StringBuilder mvpBuilder = new();
                    if (mvp != null)
                    {
                        CharacterStatistics stats = actionQueue.CharacterStatistics[mvp];
                        stats.MVPs++;
                        mvpBuilder.AppendLine($"{(tgq != null ? "[ " + tgq.GetTeamFromEliminated(mvp)?.Name + " ] " : "")}[ {mvp.ToStringWithLevel()} ]");
                        mvpBuilder.AppendLine($"技术得分：{stats.Rating:0.0#} / 击杀数：{stats.Kills} / 助攻数：{stats.Assists}{(actionQueue.MaxRespawnTimes != 0 ? " / 死亡数：" + stats.Deaths : "")}");
                        mvpBuilder.AppendLine($"存活时长：{stats.LiveTime:0.##} / 存活回合数：{stats.LiveRound} / 行动回合数：{stats.ActionTurn}");
                        mvpBuilder.AppendLine($"控制时长：{stats.ControlTime:0.##} / 总计治疗：{stats.TotalHeal:0.##}");
                        mvpBuilder.AppendLine($"总计伤害：{stats.TotalDamage:0.##} / 总计物理伤害：{stats.TotalPhysicalDamage:0.##} / 总计魔法伤害：{stats.TotalMagicDamage:0.##}");
                        mvpBuilder.AppendLine($"总承受伤害：{stats.TotalTakenDamage:0.##} / 总承受物理伤害：{stats.TotalTakenPhysicalDamage:0.##} / 总承受魔法伤害：{stats.TotalTakenMagicDamage:0.##}");
                        mvpBuilder.Append($"每秒伤害：{stats.DamagePerSecond:0.##} / 每回合伤害：{stats.DamagePerTurn:0.##}");
                    }

                    int top = isWeb ? actionQueue.CharacterStatistics.Count : 0; // 回执多少个角色的统计信息
                    int count = 1;
                    if (isWeb)
                    {
                        WriteLine("=== 技术得分排行榜 ==="); // 这是输出在界面上的
                        Msg = $"=== 技术得分排行榜 TOP{top} ===\r\n"; // 这个是下一条给Result回执的标题，覆盖掉上面方法里的赋值了
                    }
                    else
                    {
                        StringBuilder ratingBuilder = new();
                        if (tgq != null)
                        {
                            ratingBuilder.AppendLine($"=== 赛后数据 ===");
                            foreach (Team team in tgq.EliminatedTeams)
                            {
                                ratingBuilder.AppendLine($"☆--- [ {team} ] ---☆");
                                foreach (Character character in tgq.CharacterStatistics.Where(d => team.Members.Contains(d.Key))
                                    .OrderByDescending(d => d.Value.Rating).Select(d => d.Key))
                                {
                                    CharacterStatistics stats = tgq.CharacterStatistics[character];
                                    ratingBuilder.AppendLine($"[ {stats.Rating:0.0#} ]  {character}（{stats.Kills} / {stats.Assists} / {stats.Deaths}）");
                                }
                            }
                        }
                        else if (mgq != null)
                        {
                            ratingBuilder.AppendLine($"=== 赛后数据 ===");
                            foreach (Character statCharacter in actionQueue.CharacterStatistics
                                .OrderBy(kv => kv.Value.Deaths)
                                .ThenByDescending(kv => kv.Value.Rating)
                                .ThenByDescending(kv => kv.Value.Kills).Select(kv => kv.Key))
                            {
                                CharacterStatistics stats = actionQueue.CharacterStatistics[statCharacter];
                                ratingBuilder.AppendLine($"[ {stats.Rating:0.0#} ]  {statCharacter}（{stats.Kills} / {stats.Assists} / {stats.Deaths}）");
                            }
                        }
                        WriteLine("=== 本场比赛最佳角色 ===");
                        Msg = $"=== 本场比赛最佳角色 ===\r\n";
                        WriteLine(mvpBuilder.ToString() + "\r\n\r\n" + ratingBuilder.ToString());

                        if (PrintOut)
                        {
                            Console.WriteLine();
                            Console.WriteLine("=== 技术得分排行榜 ===");
                        }
                    }

                    if (tgq != null)
                    {
                        foreach (Character character in tgq.CharacterStatistics.OrderByDescending(d => d.Value.Rating).Select(d => d.Key))
                        {
                            StringBuilder builder = new();
                            CharacterStatistics stats = tgq.CharacterStatistics[character];
                            builder.AppendLine($"{(isWeb ? count + "." : ("[ " + tgq.GetTeamFromEliminated(character)?.Name + " ]" ?? ""))} [ {character.ToStringWithLevel()} ]");
                            builder.AppendLine($"技术得分：{stats.Rating:0.0#} / 击杀数：{stats.Kills} / 助攻数：{stats.Assists}{(tgq.MaxRespawnTimes != 0 ? " / 死亡数：" + stats.Deaths : "")}");
                            builder.AppendLine($"存活时长：{stats.LiveTime:0.##} / 存活回合数：{stats.LiveRound} / 行动回合数：{stats.ActionTurn}");
                            builder.AppendLine($"控制时长：{stats.ControlTime:0.##} / 总计治疗：{stats.TotalHeal:0.##}");
                            builder.AppendLine($"总计伤害：{stats.TotalDamage:0.##} / 总计物理伤害：{stats.TotalPhysicalDamage:0.##} / 总计魔法伤害：{stats.TotalMagicDamage:0.##}");
                            builder.AppendLine($"总承受伤害：{stats.TotalTakenDamage:0.##} / 总承受物理伤害：{stats.TotalTakenPhysicalDamage:0.##} / 总承受魔法伤害：{stats.TotalTakenMagicDamage:0.##}");
                            builder.Append($"每秒伤害：{stats.DamagePerSecond:0.##} / 每回合伤害：{stats.DamagePerTurn:0.##}");
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
                            builder.AppendLine($"技术得分：{stats.Rating:0.0#} / 击杀数：{stats.Kills} / 助攻数：{stats.Assists}{(actionQueue.MaxRespawnTimes != 0 ? " / 死亡数：" + stats.Deaths : "")}");
                            builder.AppendLine($"存活时长：{stats.LiveTime:0.##} / 存活回合数：{stats.LiveRound} / 行动回合数：{stats.ActionTurn}");
                            builder.AppendLine($"控制时长：{stats.ControlTime:0.##} / 总计治疗：{stats.TotalHeal:0.##}");
                            builder.AppendLine($"总计伤害：{stats.TotalDamage:0.##} / 总计物理伤害：{stats.TotalPhysicalDamage:0.##} / 总计魔法伤害：{stats.TotalMagicDamage:0.##}");
                            builder.AppendLine($"总承受伤害：{stats.TotalTakenDamage:0.##} / 总承受物理伤害：{stats.TotalTakenPhysicalDamage:0.##} / 总承受魔法伤害：{stats.TotalTakenMagicDamage:0.##}");
                            builder.Append($"每秒伤害：{stats.DamagePerSecond:0.##} / 每回合伤害：{stats.DamagePerTurn:0.##}");
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
                    lastRecord.Add(Msg);

                    // 显示每个角色的信息
                    if (isWeb)
                    {
                        if (tgq != null)
                        {
                            top = 1;
                            for (i = tgq.EliminatedTeams.Count - 1; i >= 0; i--)
                            {
                                Team team = tgq.EliminatedTeams[i];
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
                                    string msg = $"== {topTeam}团队 [ {team.Name} ] ==\r\n== 角色：[ {character} ] ==\r\n{character.GetInfo()}";
                                    result.Add(msg);
                                    lastRecord.Add(msg);
                                }
                                top++;
                            }
                        }
                        else
                        {
                            for (i = actionQueue.Eliminated.Count - 1; i >= 0; i--)
                            {
                                Character character = actionQueue.Eliminated[i];
                                string msg = $"=== 角色 [ {character} ] ===\r\n{character.GetInfo()}";
                                result.Add(msg);
                                lastRecord.Add(msg);
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

                    lock (LastRecordConfig)
                    {
                        LastRecordConfig.Add("last", result);
                        LastRecordConfig.Add("full", lastRecord);
                        LastRecordConfig.SaveConfig();
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

        public static void 空投(GamingQueue queue, int mQuality, int wQuality, int aQuality, int sQuality, int acQuality)
        {
            foreach (Character character in queue.Queue)
            {
                Item[] 武器 = [.. FunGameConstant.Equipment.Where(i => i.Id.ToString().StartsWith("11") && (int)i.QualityType == wQuality)];
                Item[] 防具 = [.. FunGameConstant.Equipment.Where(i => i.Id.ToString().StartsWith("12") && (int)i.QualityType == aQuality)];
                Item[] 鞋子 = [.. FunGameConstant.Equipment.Where(i => i.Id.ToString().StartsWith("13") && (int)i.QualityType == sQuality)];
                Item[] 饰品 = [.. FunGameConstant.Equipment.Where(i => i.Id.ToString().StartsWith("14") && (int)i.QualityType == acQuality)];
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
                Item? 魔法卡包 = FunGameService.GenerateMagicCardPack(3, (QualityType)mQuality);
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
            totalStats.TotalHeal = Calculation.Round2Digits(totalStats.TotalHeal + stats.TotalHeal);
            totalStats.LiveRound += stats.LiveRound;
            totalStats.ActionTurn += stats.ActionTurn;
            totalStats.LiveTime = Calculation.Round2Digits(totalStats.LiveTime + stats.LiveTime);
            totalStats.ControlTime = Calculation.Round2Digits(totalStats.ControlTime + stats.ControlTime);
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
                totalStats.AvgHeal = Calculation.Round2Digits(totalStats.TotalHeal / totalStats.Plays);
                totalStats.AvgLiveRound = totalStats.LiveRound / totalStats.Plays;
                totalStats.AvgActionTurn = totalStats.ActionTurn / totalStats.Plays;
                totalStats.AvgLiveTime = Calculation.Round2Digits(totalStats.LiveTime / totalStats.Plays);
                totalStats.AvgControlTime = Calculation.Round2Digits(totalStats.ControlTime / totalStats.Plays);
                totalStats.AvgEarnedMoney = totalStats.TotalEarnedMoney / totalStats.Plays;
                totalStats.Winrates = Calculation.Round4Digits(Convert.ToDouble(totalStats.Wins) / Convert.ToDouble(totalStats.Plays));
                totalStats.Top3rates = Calculation.Round4Digits(Convert.ToDouble(totalStats.Top3s) / Convert.ToDouble(totalStats.Plays));
            }
            if (totalStats.LiveRound != 0) totalStats.DamagePerRound = Calculation.Round2Digits(totalStats.TotalDamage / totalStats.LiveRound);
            if (totalStats.ActionTurn != 0) totalStats.DamagePerTurn = Calculation.Round2Digits(totalStats.TotalDamage / totalStats.ActionTurn);
            if (totalStats.LiveTime != 0) totalStats.DamagePerSecond = Calculation.Round2Digits(totalStats.TotalDamage / totalStats.LiveTime);
        }
    }
}
