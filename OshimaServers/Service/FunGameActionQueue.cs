using System.Text;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Model;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;
using Oshima.FunGame.OshimaModules.Models;

namespace Oshima.FunGame.OshimaServers.Service
{
    public class FunGameActionQueue
    {
        public GamingQueue GamingQueue { get; set; } = new();
        public bool IsWeb { get; set; } = false;
        public bool PrintOut { get; set; } = false;
        public bool DeathMatchRoundDetail { get; set; } = false;
        public List<string> Result { get; } = [];

        private string _msg = "";

        public async Task<List<string>> StartGame(List<Character> characters, int maxRespawnTimes = 0, int maxScoreToWin = 0, bool printout = false, bool isWeb = false, bool deathMatchRoundDetail = false, bool showRoundEndDetail = false, bool showAllRound = false)
        {
            Result.Clear();
            PrintOut = printout;
            IsWeb = isWeb;
            DeathMatchRoundDetail = deathMatchRoundDetail;
            try
            {
                _msg = "";

                if (PrintOut) Console.WriteLine();
                if (PrintOut) Console.WriteLine("Start!!!");
                if (PrintOut) Console.WriteLine();

                // 创建顺序表并排序
                MixGamingQueue actionQueue = new(characters, WriteLine)
                {
                    MaxRespawnTimes = maxRespawnTimes,
                    MaxScoreToWin = maxScoreToWin
                };
                actionQueue.InitActionQueue();
                actionQueue.SetCharactersToAIControl(false, characters);
                foreach (Character dead in characters)
                {
                    if (dead.HP <= 0)
                    {
                        actionQueue.Queue.Remove(dead);
                        actionQueue.Eliminated.Add(dead);
                    }
                }
                GamingQueue = actionQueue;
                if (PrintOut) Console.WriteLine();

                // 总游戏时长
                double totalTime = 0;

                // 显示角色信息
                if (PrintOut) characters.ForEach(c => Console.WriteLine(c.GetInfo()));

                // 显示初始顺序表
                actionQueue.DisplayQueue();
                if (PrintOut) Console.WriteLine();

                // 总回合数
                int maxRound = 999;

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
                    _msg = "";
                    if (i == maxRound - 1)
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
                        await actionQueue.EndGameInfo(winner);
                        Result.Add(_msg);
                        break;
                    }

                    // 检查是否有角色可以行动
                    Character? characterToAct = await actionQueue.NextCharacterAsync();

                    // 处理回合
                    if (characterToAct != null)
                    {
                        WriteLine($"=== Round {i++} ===");
                        WriteLine($"现在是 [ {characterToAct} ] 的回合！");

                        bool isGameEnd = await actionQueue.ProcessTurnAsync(characterToAct);

                        if (isGameEnd)
                        {
                            Result.Add(_msg);
                            break;
                        }

                        if (showRoundEndDetail) actionQueue.DisplayQueue();
                        WriteLine("");
                    }

                    string roundMsg = "";
                    if (actionQueue.LastRound.HasKill || showAllRound)
                    {
                        roundMsg = _msg;
                        if (!deathMatchRoundDetail)
                        {
                            roundMsg = actionQueue.LastRound.ToString().Trim() + "\r\n";
                            if (showAllRound)
                            {
                                foreach (Character character in actionQueue.Queue)
                                {
                                    roundMsg += $"[ {character} ] 生命值：{character.HP:0.##}/{character.MaxHP:0.##} / 魔法值：{character.MP:0.##}/{character.MaxMP:0.##}\r\n";
                                }
                            }
                        }
                        _msg = "";
                    }

                    // 模拟时间流逝
                    double timeLapse = await actionQueue.TimeLapse();
                    totalTime += timeLapse;

                    if (roundMsg != "")
                    {
                        if (isWeb)
                        {
                            roundMsg += "\r\n" + _msg;
                        }
                        Result.Add(roundMsg);
                    }
                }

                if (PrintOut)
                {
                    Console.WriteLine("--- End ---");
                    Console.WriteLine($"总游戏时长：{totalTime:0.##}");
                    Console.WriteLine("");
                }

                // 赛后统计
                FunGameService.GetCharacterRating(actionQueue.CharacterStatistics, false, []);

                // 统计技术得分，评选 MVP
                Character? mvp = actionQueue.CharacterStatistics.OrderByDescending(d => d.Value.Rating).Select(d => d.Key).FirstOrDefault();
                StringBuilder mvpBuilder = new();
                if (mvp != null)
                {
                    CharacterStatistics stats = actionQueue.CharacterStatistics[mvp];
                    stats.MVPs++;
                    mvpBuilder.AppendLine($"[ {mvp.ToStringWithLevel()} ]");
                    mvpBuilder.AppendLine($"技术得分：{stats.Rating:0.0#} / 击杀数：{stats.Kills} / 助攻数：{stats.Assists}{(actionQueue.MaxRespawnTimes != 0 ? " / 死亡数：" + stats.Deaths : "")}");
                    mvpBuilder.AppendLine($"存活时长：{stats.LiveTime:0.##} / 存活回合数：{stats.LiveRound} / 行动回合数：{stats.ActionTurn}");
                    mvpBuilder.AppendLine($"控制时长：{stats.ControlTime:0.##} / 总计治疗：{stats.TotalHeal:0.##} / 护盾抵消：{stats.TotalShield:0.##}");
                    mvpBuilder.AppendLine($"总计伤害：{stats.TotalDamage:0.##} / 总计物理伤害：{stats.TotalPhysicalDamage:0.##} / 总计魔法伤害：{stats.TotalMagicDamage:0.##}");
                    mvpBuilder.AppendLine($"总承受伤害：{stats.TotalTakenDamage:0.##} / 总承受物理伤害：{stats.TotalTakenPhysicalDamage:0.##} / 总承受魔法伤害：{stats.TotalTakenMagicDamage:0.##}");
                    if (stats.TotalTrueDamage > 0 || stats.TotalTakenTrueDamage > 0) mvpBuilder.AppendLine($"总计真实伤害：{stats.TotalTrueDamage:0.##} / 总承受真实伤害：{stats.TotalTakenTrueDamage:0.##}");
                    mvpBuilder.Append($"每秒伤害：{stats.DamagePerSecond:0.##} / 每回合伤害：{stats.DamagePerTurn:0.##}");
                }

                int top = isWeb ? actionQueue.CharacterStatistics.Count : 2; // 回执多少个角色的统计信息
                int count = 1;
                _msg = $"=== 技术得分排行榜 TOP{top} ===\r\n";

                foreach (Character character in actionQueue.CharacterStatistics.OrderByDescending(d => d.Value.Rating).Select(d => d.Key))
                {
                    StringBuilder builder = new();
                    CharacterStatistics stats = actionQueue.CharacterStatistics[character];
                    builder.AppendLine($"{count + ". "}[ {character.ToStringWithLevel()} ]");
                    builder.AppendLine($"技术得分：{stats.Rating:0.0#} / 击杀数：{stats.Kills} / 助攻数：{stats.Assists}{(actionQueue.MaxRespawnTimes != 0 ? " / 死亡数：" + stats.Deaths : "")}");
                    builder.AppendLine($"存活时长：{stats.LiveTime:0.##} / 存活回合数：{stats.LiveRound} / 行动回合数：{stats.ActionTurn}");
                    builder.AppendLine($"控制时长：{stats.ControlTime:0.##} / 总计治疗：{stats.TotalHeal:0.##} / 护盾抵消：{stats.TotalShield:0.##}");
                    builder.AppendLine($"总计伤害：{stats.TotalDamage:0.##} / 总计物理伤害：{stats.TotalPhysicalDamage:0.##} / 总计魔法伤害：{stats.TotalMagicDamage:0.##}");
                    builder.AppendLine($"总承受伤害：{stats.TotalTakenDamage:0.##} / 总承受物理伤害：{stats.TotalTakenPhysicalDamage:0.##} / 总承受魔法伤害：{stats.TotalTakenMagicDamage:0.##}");
                    if (stats.TotalTrueDamage > 0 || stats.TotalTakenTrueDamage > 0) builder.AppendLine($"总计真实伤害：{stats.TotalTrueDamage:0.##} / 总承受真实伤害：{stats.TotalTakenTrueDamage:0.##}");
                    builder.AppendLine($"每秒伤害：{stats.DamagePerSecond:0.##} / 每回合伤害：{stats.DamagePerTurn:0.##}");
                    builder.Append($"生命值：{character.HP:0.##}/{character.MaxHP:0.##} / 魔法值：{character.MP:0.##}/{character.MaxMP:0.##}");
                    if (count++ <= top)
                    {
                        WriteLine(builder.ToString());
                    }
                    else
                    {
                        if (PrintOut) Console.WriteLine(builder.ToString());
                    }
                }

                Result.Add(_msg);

                // 显示每个角色的信息
                if (isWeb)
                {
                    for (i = actionQueue.Eliminated.Count - 1; i >= 0; i--)
                    {
                        Character character = actionQueue.Eliminated[i];
                        Result.Add($"=== 角色 [ {character} ] ===\r\n{character.GetInfo()}");
                    }
                }

                return Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return [ex.ToString()];
            }
        }

        public async Task<List<string>> StartTeamGame(List<Team> teams, int maxRespawnTimes = 0, int maxScoreToWin = 0, bool printout = false, bool isWeb = false, bool deathMatchRoundDetail = false, bool showRoundEndDetail = false, bool showAllRound = false)
        {
            Result.Clear();
            PrintOut = printout;
            IsWeb = isWeb;
            DeathMatchRoundDetail = deathMatchRoundDetail;
            try
            {
                _msg = "";

                if (teams.Count > 1)
                {
                    if (PrintOut) Console.WriteLine();
                    if (PrintOut) Console.WriteLine("Start!!!");
                    if (PrintOut) Console.WriteLine();

                    List<Character> characters = [.. teams.SelectMany(t => t.Members)];

                    // 创建顺序表并排序
                    TeamGamingQueue actionQueue = new(characters, WriteLine)
                    {
                        MaxRespawnTimes = maxRespawnTimes,
                        MaxScoreToWin = maxScoreToWin
                    };
                    actionQueue.InitActionQueue();
                    actionQueue.SetCharactersToAIControl(false, characters);
                    foreach (Character dead in characters)
                    {
                        if (dead.HP <= 0)
                        {
                            actionQueue.Queue.Remove(dead);
                            actionQueue.Eliminated.Add(dead);
                        }
                    }
                    GamingQueue = actionQueue;
                    if (PrintOut) Console.WriteLine();

                    // 总游戏时长
                    double totalTime = 0;

                    // 显示角色信息
                    if (PrintOut) characters.ForEach(c => Console.WriteLine(c.GetInfo()));

                    // 添加到团队字典
                    foreach (Team team in teams)
                    {
                        actionQueue.AddTeam(team.Name, team.Members);
                        Result.Add($"团队【{team}】的成员：\r\n{string.Join("\r\n", team.Members)}\r\n");
                    }

                    // 显示初始顺序表
                    actionQueue.DisplayQueue();
                    if (PrintOut) Console.WriteLine();

                    // 总回合数
                    int maxRound = 9999;

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
                        _msg = "";
                        if (i == maxRound - 1)
                        {
                            WriteLine("两队打到天昏地暗，流局了！！");
                            break;
                        }

                        // 检查是否有角色可以行动
                        Character? characterToAct = await actionQueue.NextCharacterAsync();

                        // 处理回合
                        if (characterToAct != null)
                        {
                            WriteLine($"=== Round {i++} ===");
                            WriteLine("现在是 [ " + characterToAct + "（" + (actionQueue.GetTeam(characterToAct)?.Name ?? "") + "）" + " ] 的回合！");

                            bool isGameEnd = await actionQueue.ProcessTurnAsync(characterToAct);

                            if (isGameEnd)
                            {
                                Result.Add(_msg);
                                break;
                            }

                            if (showRoundEndDetail) actionQueue.DisplayQueue();
                            WriteLine("");
                        }

                        string roundMsg = "";
                        if (actionQueue.LastRound.HasKill || showAllRound)
                        {
                            roundMsg = _msg;
                            if (!deathMatchRoundDetail)
                            {
                                roundMsg = actionQueue.LastRound.ToString().Trim() + $"\r\n" +
                                    (actionQueue.LastRound.HasKill ? $"比分：{string.Join(" / ", actionQueue.Teams.Values.Select(t => $"{t.Name}({t.Score})"))}" +
                                        $"，击杀来自{actionQueue.GetTeam(actionQueue.LastRound.Actor)}\r\n" : "");
                                if (showAllRound)
                                {
                                    Character[] showHPMP = [actionQueue.LastRound.Actor, .. actionQueue.LastRound.Targets];
                                    foreach (Character character in showHPMP)
                                    {
                                        roundMsg += $"[ {character} ] 生命值：{character.HP:0.##}/{character.MaxHP:0.##} / 魔法值：{character.MP:0.##}/{character.MaxMP:0.##}\r\n";
                                    }
                                }
                            }
                            _msg = "";
                        }

                        // 模拟时间流逝
                        double timeLapse = await actionQueue.TimeLapse();
                        totalTime += timeLapse;

                        if (roundMsg != "")
                        {
                            if (deathMatchRoundDetail && isWeb)
                            {
                                roundMsg += "\r\n" + _msg;
                            }
                            Result.Add(roundMsg);
                        }
                    }

                    if (PrintOut)
                    {
                        Console.WriteLine("--- End ---");
                        Console.WriteLine($"总游戏时长：{totalTime:0.##}");
                        Console.WriteLine("");
                    }

                    // 赛后统计
                    FunGameService.GetCharacterRating(actionQueue.CharacterStatistics, true, actionQueue.EliminatedTeams);

                    // 统计技术得分，评选 MVP
                    StringBuilder mvpBuilder = new();
                    Character? mvp = actionQueue.CharacterStatistics.OrderByDescending(d => d.Value.Rating).Select(d => d.Key).FirstOrDefault();
                    if (mvp != null)
                    {
                        CharacterStatistics stats = actionQueue.CharacterStatistics[mvp];
                        stats.MVPs++;
                        Team? team = actionQueue.GetTeamFromEliminated(mvp);
                        mvpBuilder.AppendLine($"=== 本场比赛最佳角色 ===");
                        mvpBuilder.AppendLine($"{(team != null ? "[ " + team.Name + " ] " : "")}[ {mvp.ToStringWithLevel()} ]");
                        mvpBuilder.AppendLine($"技术得分：{stats.Rating:0.0#} / 击杀数：{stats.Kills} / 助攻数：{stats.Assists}{(actionQueue.MaxRespawnTimes != 0 ? " / 死亡数：" + stats.Deaths : "")}");
                        mvpBuilder.AppendLine($"存活时长：{stats.LiveTime:0.##} / 存活回合数：{stats.LiveRound} / 行动回合数：{stats.ActionTurn}");
                        mvpBuilder.AppendLine($"控制时长：{stats.ControlTime:0.##} / 总计治疗：{stats.TotalHeal:0.##} / 护盾抵消：{stats.TotalShield:0.##}");
                        mvpBuilder.AppendLine($"总计伤害：{stats.TotalDamage:0.##} / 总计物理伤害：{stats.TotalPhysicalDamage:0.##} / 总计魔法伤害：{stats.TotalMagicDamage:0.##}");
                        mvpBuilder.AppendLine($"总承受伤害：{stats.TotalTakenDamage:0.##} / 总承受物理伤害：{stats.TotalTakenPhysicalDamage:0.##} / 总承受魔法伤害：{stats.TotalTakenMagicDamage:0.##}");
                        if (stats.TotalTrueDamage > 0 || stats.TotalTakenTrueDamage > 0) mvpBuilder.AppendLine($"总计真实伤害：{stats.TotalTrueDamage:0.##} / 总承受真实伤害：{stats.TotalTakenTrueDamage:0.##}");
                        mvpBuilder.Append($"每秒伤害：{stats.DamagePerSecond:0.##} / 每回合伤害：{stats.DamagePerTurn:0.##}");
                    }

                    // 赛后数据
                    StringBuilder ratingBuilder = new();
                    ratingBuilder.AppendLine($"=== 赛后数据 ===");
                    foreach (Team team in teams)
                    {
                        ratingBuilder.AppendLine($"☆--- [ {team} ] ---☆");
                        foreach (Character character in actionQueue.CharacterStatistics.Where(d => team.Members.Contains(d.Key))
                            .OrderByDescending(d => d.Value.Rating).Select(d => d.Key))
                        {
                            CharacterStatistics stats = actionQueue.CharacterStatistics[character];
                            ratingBuilder.AppendLine($"[ {stats.Rating:0.0#} ]  {character}（{stats.Kills} / {stats.Assists} / {stats.Deaths}）");
                        }
                    }
                    Result.Add(mvpBuilder.ToString() + "\r\n\r\n" + ratingBuilder.ToString());
                }

                return Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return [ex.ToString()];
            }
        }

        public void WriteLine(string str)
        {
            _msg += str + "\r\n";
            if (PrintOut) Console.WriteLine(str);
        }

        public static async Task<List<string>> NewAndStartGame(List<Character> characters, int maxRespawnTimes = 0, int maxScoreToWin = 0, bool printout = false, bool isWeb = false, bool deathMatchRoundDetail = false, bool showRoundEndDetail = false, bool showAllRound = false)
        {
            return await new FunGameActionQueue().StartGame(characters, maxRespawnTimes, maxScoreToWin, printout, isWeb, deathMatchRoundDetail, showRoundEndDetail, showAllRound);
        }

        public static async Task<List<string>> NewAndStartTeamGame(List<Team> teams, int maxRespawnTimes = 0, int maxScoreToWin = 0, bool printout = false, bool isWeb = false, bool deathMatchRoundDetail = false, bool showRoundEndDetail = false, bool showAllRound = false)
        {
            return await new FunGameActionQueue().StartTeamGame(teams, maxRespawnTimes, maxScoreToWin, printout, isWeb, deathMatchRoundDetail, showRoundEndDetail, showAllRound);
        }
    }
}
