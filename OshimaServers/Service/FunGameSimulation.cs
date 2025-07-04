using System.IO.Compression;
using System.Text;
using System.Text.Json;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
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

        public static async Task<List<string>> StartSimulationGame(bool printout, bool isWeb = false, bool isTeam = false, bool deathMatchRoundDetail = false, int maxRespawnTimesMix = 1, bool useStore = false)
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

                    int clevel = 10;
                    int slevel = 2;
                    int mlevel = 2;

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
                        foreach (Effect e in c.Effects)
                        {
                            e.OnEffectLost(c);
                        }
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

                    // 初始化商店 团队模式专供
                    useStore = useStore && isTeam;
                    List<Item> store = [];
                    if (useStore)
                    {
                        AddStoreItems(actionQueue, store);
                    }

                    // 开始空投
                    Msg = "";
                    int mQuality = 0;
                    int wQuality = 0;
                    int aQuality = 0;
                    int sQuality = 0;
                    int acQuality = 0;
                    double nextDropTime = 0;
                    if (!useStore)
                    {
                        WriteLine($"社区送温暖了，现在随机发放空投！！");
                        DropItems(actionQueue, mQuality, wQuality, aQuality, sQuality, acQuality, false);
                        WriteLine("");
                        if (isWeb) result.Add("=== 空投 ===\r\n" + Msg);
                        nextDropTime = isTeam ? 90 : 60;
                        if (mQuality < 5)
                        {
                            mQuality++;
                        }
                        if (wQuality < 5)
                        {
                            wQuality++;
                        }
                        if (aQuality < 5)
                        {
                            aQuality++;
                        }
                        if (sQuality < 5)
                        {
                            sQuality++;
                        }
                        if (acQuality < 5)
                        {
                            acQuality++;
                        }
                    }

                    // 显示角色信息
                    if (PrintOut) characters.ForEach(c => Console.WriteLine(c.GetInfo()));

                    // 初始化队列，准备开始游戏
                    actionQueue.InitActionQueue();
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
                            // 创建角色的用户，用于绑定金币
                            User user = Factory.GetUser();
                            user.Username = FunGameService.GenerateRandomChineseUserName();
                            user.Inventory.Credits = 20;
                            Character thisCharacter = shuffledCharacters[cid];
                            //thisCharacter.User = user;

                            if (cid % 2 == 0)
                            {
                                group1.Add(thisCharacter);
                            }
                            else
                            {
                                group2.Add(thisCharacter);
                            }
                        }

                        // 添加到团队字典，第一个为队长
                        tg.AddTeam($"{group1.First()}的小队", group1);
                        tg.AddTeam($"{group2.First()}的小队", group2);

                        foreach (string team in tg.Teams.Keys)
                        {
                            WriteLine($"团队【{team}】的成员：\r\n{string.Join("\r\n", tg.Teams[team].Members)}\r\n");
                        }
                        result.Add(Msg);
                    }

                    // 显示初始顺序表
                    actionQueue.DisplayQueue();
                    if (PrintOut) Console.WriteLine();

                    actionQueue.CharacterDeath += ActionQueue_CharacterDeath;
                    if (actionQueue is TeamGamingQueue teamQueue)
                    {
                        //teamQueue.GameEndTeam += TeamQueue_GameEndTeam;
                    }

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
                            WriteLine($"=== Round {i++} [ Time: {totalTime} ] ===");
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
                        totalTime = actionQueue.TotalTime;
                        nextDropTime -= timeLapse;

                        if (roundMsg != "")
                        {
                            if ((isTeam && deathMatchRoundDetail || !isTeam) && isWeb)
                            {
                                roundMsg += "\r\n" + Msg;
                            }
                            result.Add(roundMsg);
                        }

                        // 模拟商店购买
                        if (useStore)
                        {
                            // 发钱了
                            foreach (Character character in actionQueue.HardnessTime.Keys)
                            {
                                character.User.Inventory.Credits += 3 * timeLapse;
                            }
                            BuyItems(actionQueue, store);
                        }

                        if (!useStore && nextDropTime <= 0)
                        {
                            // 空投
                            Msg = "";
                            WriteLine($"社区送温暖了，现在随机发放空投！！");
                            DropItems(actionQueue, mQuality, wQuality, aQuality, sQuality, acQuality);
                            WriteLine("");
                            if (isWeb) result.Add("=== 空投 ===\r\n" + Msg);
                            nextDropTime = isTeam ? 100 : 40;
                            if (mQuality < 5)
                            {
                                mQuality++;
                            }
                            if (wQuality < 5)
                            {
                                wQuality++;
                            }
                            if (aQuality < 5)
                            {
                                aQuality++;
                            }
                            if (sQuality < 5)
                            {
                                sQuality++;
                            }
                            if (acQuality < 5)
                            {
                                acQuality++;
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
                        mvpBuilder.AppendLine($"控制时长：{stats.ControlTime:0.##} / 总计治疗：{stats.TotalHeal:0.##} / 护盾抵消：{stats.TotalShield:0.##}");
                        mvpBuilder.AppendLine($"总计伤害：{stats.TotalDamage:0.##} / 总计物理伤害：{stats.TotalPhysicalDamage:0.##} / 总计魔法伤害：{stats.TotalMagicDamage:0.##}");
                        mvpBuilder.AppendLine($"总承受伤害：{stats.TotalTakenDamage:0.##} / 总承受物理伤害：{stats.TotalTakenPhysicalDamage:0.##} / 总承受魔法伤害：{stats.TotalTakenMagicDamage:0.##}");
                        if (stats.TotalTrueDamage > 0 || stats.TotalTakenTrueDamage > 0) mvpBuilder.AppendLine($"总计真实伤害：{stats.TotalTrueDamage:0.##} / 总承受真实伤害：{stats.TotalTakenTrueDamage:0.##}");
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
                            builder.AppendLine($"控制时长：{stats.ControlTime:0.##} / 总计治疗：{stats.TotalHeal:0.##} / 护盾抵消：{stats.TotalShield:0.##}");
                            builder.AppendLine($"总计伤害：{stats.TotalDamage:0.##} / 总计物理伤害：{stats.TotalPhysicalDamage:0.##} / 总计魔法伤害：{stats.TotalMagicDamage:0.##}");
                            builder.AppendLine($"总承受伤害：{stats.TotalTakenDamage:0.##} / 总承受物理伤害：{stats.TotalTakenPhysicalDamage:0.##} / 总承受魔法伤害：{stats.TotalTakenMagicDamage:0.##}");
                            if (stats.TotalTrueDamage > 0 || stats.TotalTakenTrueDamage > 0) builder.AppendLine($"总计真实伤害：{stats.TotalTrueDamage:0.##} / 总承受真实伤害：{stats.TotalTakenTrueDamage:0.##}");
                            builder.Append($"每秒伤害：{stats.DamagePerSecond:0.##} / 每回合伤害：{stats.DamagePerTurn:0.##}");
                            if (count++ <= top)
                            {
                                WriteLine(builder.ToString());
                            }
                            else
                            {
                                if (PrintOut) Console.WriteLine(builder.ToString());
                            }

                            if (!useStore)
                            {
                                CharacterStatistics? totalStats = TeamCharacterStatistics.Where(kv => kv.Key.GetName() == character.GetName()).Select(kv => kv.Value).FirstOrDefault();
                                if (totalStats != null)
                                {
                                    UpdateStatistics(totalStats, stats);
                                }
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
                            builder.AppendLine($"控制时长：{stats.ControlTime:0.##} / 总计治疗：{stats.TotalHeal:0.##} / 护盾抵消：{stats.TotalShield:0.##}");
                            builder.AppendLine($"总计伤害：{stats.TotalDamage:0.##} / 总计物理伤害：{stats.TotalPhysicalDamage:0.##} / 总计魔法伤害：{stats.TotalMagicDamage:0.##}");
                            builder.AppendLine($"总承受伤害：{stats.TotalTakenDamage:0.##} / 总承受物理伤害：{stats.TotalTakenPhysicalDamage:0.##} / 总承受魔法伤害：{stats.TotalTakenMagicDamage:0.##}");
                            if (stats.TotalTrueDamage > 0 || stats.TotalTakenTrueDamage > 0) builder.AppendLine($"总计真实伤害：{stats.TotalTrueDamage:0.##} / 总承受真实伤害：{stats.TotalTakenTrueDamage:0.##}");
                            builder.Append($"每秒伤害：{stats.DamagePerSecond:0.##} / 每回合伤害：{stats.DamagePerTurn:0.##}");
                            if (count++ <= top)
                            {
                                WriteLine(builder.ToString());
                            }
                            else
                            {
                                if (PrintOut) Console.WriteLine(builder.ToString());
                            }

                            if (!useStore)
                            {
                                CharacterStatistics? totalStats = CharacterStatistics.Where(kv => kv.Key.GetName() == character.GetName()).Select(kv => kv.Value).FirstOrDefault();
                                if (totalStats != null)
                                {
                                    UpdateStatistics(totalStats, stats);
                                }
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

                    if (!useStore)
                    {
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
                    }

                    lock (LastRecordConfig)
                    {
                        LastRecordConfig.Add("last", result);
                        LastRecordConfig.Add("full", lastRecord);
                        LastRecordConfig.SaveConfig();
                    }

                    //string zipFileName = "rounds_archive.zip";
                    //WriteRoundsToZip(actionQueue.Rounds.ToDictionary(kv => kv.Round, kv => kv), zipFileName);

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

        private static async Task<bool> TeamQueue_GameEndTeam(TeamGamingQueue queue, Team winner)
        {
            foreach (Character character in winner.Members)
            {
                Item? i1 = character.UnEquip(EquipSlotType.MagicCardPack);
                Item? i2 = character.UnEquip(EquipSlotType.Weapon);
                Item? i3 = character.UnEquip(EquipSlotType.Armor);
                Item? i4 = character.UnEquip(EquipSlotType.Shoes);
                Item? i5 = character.UnEquip(EquipSlotType.Accessory1);
                Item? i6 = character.UnEquip(EquipSlotType.Accessory2);
                queue.WriteLine(character.GetInfo());
            }
            return await Task.FromResult(true);
        }

        private static async Task<bool> ActionQueue_GameEnd(GamingQueue queue, Character winner)
        {
            return await Task.FromResult(true);
        }

        private static async Task<bool> ActionQueue_CharacterDeath(GamingQueue queue, Character current, Character death)
        {
            death.Items.Clear();
            return await Task.FromResult(true);
        }

        /// <summary>
        /// 将回合记录字典序列化为 JSON，然后压缩并写入 ZIP 文件
        /// </summary>
        /// <param name="roundsData">要写入的字典数据</param>
        /// <param name="zipFilePath">输出的 ZIP 文件路径</param>
        public static void WriteRoundsToZip(Dictionary<int, RoundRecord> roundsData, string zipFilePath)
        {
            // 确保目标目录存在
            string? directory = Path.GetDirectoryName(zipFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // 创建 ZIP 文件
            using FileStream? zipFileStream = new(zipFilePath, FileMode.Create);
            using ZipArchive? zipArchive = new(zipFileStream, ZipArchiveMode.Create);
            // 在 ZIP 档案中创建一个条目来存放 JSON 数据
            // 你可以给这个条目起个名字，比如 "rounds_data.json"
            ZipArchiveEntry jsonEntry = zipArchive.CreateEntry("rounds_data.json", CompressionLevel.Optimal); // 使用 Optimal 级别压缩

            // 获取条目的流，将 JSON 数据写入这个流
            using Stream? entryStream = jsonEntry.Open();
            // 使用 System.Text.Json 直接序列化到流
            JsonSerializer.Serialize(entryStream, roundsData, JsonTool.JsonSerializerOptions);

            Console.WriteLine($"回合记录已成功序列化、压缩并写入到: {zipFilePath}");
        }

        /// <summary>
        /// 从 ZIP 文件中读取并解压 JSON 数据，然后反序列化为回合记录字典
        /// </summary>
        /// <param name="zipFilePath">输入的 ZIP 文件路径</param>
        /// <returns>反序列化后的字典数据</returns>
        public static Dictionary<int, RoundRecord>? ReadRoundsFromZip(string zipFilePath)
        {
            if (!File.Exists(zipFilePath))
            {
                Console.WriteLine($"错误: 文件不存在 {zipFilePath}");
                return null;
            }

            Dictionary<int, RoundRecord>? roundsData;

            // 打开 ZIP 文件
            using FileStream? zipFileStream = new(zipFilePath, FileMode.Open);
            using ZipArchive? zipArchive = new(zipFileStream, ZipArchiveMode.Read);
            ZipArchiveEntry? jsonEntry = zipArchive.GetEntry("rounds_data.json");

            if (jsonEntry == null)
            {
                Console.WriteLine($"错误: ZIP 档案中找不到 'rounds_data.json' 条目。");
                return null;
            }

            // 获取条目的流，从这个流中读取 JSON 数据
            using Stream? entryStream = jsonEntry.Open();
            try
            {
                roundsData = JsonSerializer.Deserialize<Dictionary<int, RoundRecord>>(entryStream, JsonTool.JsonSerializerOptions);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON 反序列化错误: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"读取或反序列化过程中发生未知错误: {ex.Message}");
                return null;
            }

            Console.WriteLine($"回合记录已成功从 {zipFilePath} 读取、解压并反序列化。");
            return roundsData;
        }

        public static void WriteLine(string str)
        {
            Msg += str + "\r\n";
            if (PrintOut) Console.WriteLine(str);
        }

        public static void AddStoreItems(GamingQueue queue, List<Item> store)
        {
            foreach (Item item in FunGameConstant.Equipment)
            {
                Item realItem = item.Copy();
                realItem.SetGamingQueue(queue);
                realItem.Price = Random.Shared.Next(1, 10) * ((int)item.QualityType + 1) * 2;
                store.Add(realItem);
            }
        }

        public static void BuyItems(GamingQueue queue, List<Item> store)
        {
            // 升级成本
            double costLevel = 20;

            // 卡包购买/升级成本
            Dictionary<QualityType, double> costMCP = new()
            {
                { QualityType.White, 5 }, // 普通
                { QualityType.Green, 10 }, // 优秀
                { QualityType.Blue, 15 }, // 稀有
                { QualityType.Purple, 20 }, // 史诗
                { QualityType.Orange, 25 }, // 传说
            };

            foreach (Character character in queue.HardnessTime.Keys)
            {
                // 购买欲望，可以加多个判断
                List<Func<bool>> funcs = [
                    () => Random.Shared.NextDouble() > 0.3
                ];

                if (funcs.All(f => f()))
                {
                    // 定义操作的概率（优先级）
                    Dictionary<string, double> pOperations = new()
                    {
                        { "升级角色", 0.8 }, // 会同时升级技能等级，优先级高
                        { "买卡包", 0.6 }, // 会获得属性加成和魔法技能，优先级高
                        { "买武器", 0.4 }, // 提升攻击力
                        { "买防具", 0.4 }, // 提升防御性能
                        { "买鞋子", 0.4 }, // 提升速度
                        { "买饰品", 0.4 } // 提升综合能力
                    };

                    // 记录操作
                    Dictionary<string, bool> operation = new()
                    {
                        { "升级角色", false },
                        { "买卡包", false },
                        { "买武器", false },
                        { "买防具", false },
                        { "买鞋子", false },
                        { "买饰品", false }
                    };

                    // 本次购买只提升？
                    bool onlyLarger = Random.Shared.NextDouble() > 0.3;

                    bool buy = true;
                    int failedBuyTimes = 0;
                    while (buy && pOperations.Values.Any(o => o != 0) && operation.Values.Any(o => o == false))
                    {
                        // 先看手上的钱能买什么
                        IEnumerable<Item> canBuys = store.Where(i => i.Price <= character.User.Inventory.Credits);

                        // 然后看能买的东西里，是否品质符合
                        Item[] weapons = [.. canBuys.Where(i => i.ItemType == ItemType.Weapon &&
                            (onlyLarger ? (int)i.QualityType > Convert.ToInt32(character.EquipSlot.Weapon?.QualityType) : (int)i.QualityType >= Convert.ToInt32(character.EquipSlot.Weapon?.QualityType)))];
                        Item[] armors = [.. canBuys.Where(i => i.ItemType == ItemType.Armor &&
                            (onlyLarger ? (int)i.QualityType > Convert.ToInt32(character.EquipSlot.Armor?.QualityType) : (int)i.QualityType >= Convert.ToInt32(character.EquipSlot.Armor?.QualityType)))];
                        Item[] shoes = [.. canBuys.Where(i => i.ItemType == ItemType.Shoes &&
                            (onlyLarger ? (int)i.QualityType > Convert.ToInt32(character.EquipSlot.Shoes?.QualityType) : (int)i.QualityType >= Convert.ToInt32(character.EquipSlot.Shoes?.QualityType)))];
                        Item[] accessories = [.. canBuys.Where(i => i.ItemType == ItemType.Accessory &&
                            (onlyLarger ? (int)i.QualityType > Math.Min(Convert.ToInt32(character.EquipSlot.Accessory1?.QualityType), Convert.ToInt32(character.EquipSlot.Accessory2?.QualityType))
                            : (int)i.QualityType >= Math.Min(Convert.ToInt32(character.EquipSlot.Accessory1?.QualityType), Convert.ToInt32(character.EquipSlot.Accessory2?.QualityType))))];

                        // 魔法卡包单独列出
                        int mQuality = Convert.ToInt32(character.EquipSlot.MagicCardPack?.QualityType);

                        if (character.User.Inventory.Credits < costLevel)
                        {
                            pOperations["升级角色"] = 0;
                        }

                        if (character.User.Inventory.Credits < costMCP[(QualityType)mQuality])
                        {
                            pOperations["买卡包"] = 0;
                        }

                        if (weapons.Length == 0)
                        {
                            pOperations["买武器"] = 0;
                        }

                        if (armors.Length == 0)
                        {
                            pOperations["买防具"] = 0;
                        }

                        if (shoes.Length == 0)
                        {
                            pOperations["买鞋子"] = 0;
                        }

                        if (accessories.Length == 0)
                        {
                            pOperations["买饰品"] = 0;
                        }

                        double p = Random.Shared.NextDouble();

                        foreach (KeyValuePair<string, double> kvp in pOperations.OrderByDescending(kv => kv.Value))
                        {
                            string op = "";
                            if (kvp.Value != 0 && p <= kvp.Value)
                            {
                                op = kvp.Key;
                            }
                            else
                            {
                                continue;
                            }
                            switch (op)
                            {
                                case "升级角色":
                                    // 钱够吗？
                                    if (character.User.Inventory.Credits >= costLevel)
                                    {
                                        character.User.Inventory.Credits -= costLevel;
                                        operation["升级角色"] = true;
                                        character.SetLevel(character.Level + 10, false);
                                        character.NormalAttack.Level += 2;
                                        foreach (Skill skill in character.Skills)
                                        {
                                            if (skill.SkillType == SkillType.Passive) continue;
                                            skill.Level += 2;
                                        }
                                        Character? original = queue.Original[character.Guid];
                                        if (original != null)
                                        {
                                            original.Level += 10;
                                            original.NormalAttack.Level += 2;
                                            foreach (Skill skill in original.Skills)
                                            {
                                                if (skill.SkillType == SkillType.Passive) continue;
                                                skill.Level += 2;
                                            }
                                        }
                                        WriteLine($"[ {character} ] 消费了 {costLevel} {General.GameplayEquilibriumConstant.InGameCurrency}，购买了【升级角色】！当前角色 {character.Level} 级，技能 {character.NormalAttack.Level} 级（战技、爆发技最高 6 级）。");
                                    }
                                    else failedBuyTimes++;
                                    break;
                                case "买卡包":
                                    // 看角色能购买的最高级卡包是什么
                                    QualityType canBuyMCP = costMCP.Where(kv => (int)kv.Key >= mQuality && character.User.Inventory.Credits >= kv.Value).OrderByDescending(kv => kv.Value).Select(kv => kv.Key).FirstOrDefault();
                                    double mcpCost = costMCP[canBuyMCP];
                                    // 买卡包
                                    if (character.User.Inventory.Credits >= mcpCost)
                                    {
                                        character.User.Inventory.Credits -= mcpCost;
                                        operation["买卡包"] = true;
                                        Item? mcp = FunGameService.GenerateMagicCardPack(3, canBuyMCP);
                                        if (mcp != null)
                                        {
                                            foreach (Skill magic in mcp.Skills.Magics)
                                            {
                                                magic.Level = character.NormalAttack.Level;
                                            }
                                            mcp.SetGamingQueue(queue);
                                            WriteLine($"[ {character} ] 消费了 {mcpCost} {General.GameplayEquilibriumConstant.InGameCurrency}，购买了【卡包-{mcp.Name}】！详细说明：\r\n{mcp}");
                                            queue.Equip(character, mcp);
                                        }
                                    }
                                    else failedBuyTimes++;
                                    break;
                                case "买武器":
                                    Item weapon = weapons[Random.Shared.Next(weapons.Length)];
                                    double wCost = weapon.Price;
                                    if (character.User.Inventory.Credits >= wCost)
                                    {
                                        character.User.Inventory.Credits -= wCost;
                                        operation["买武器"] = true;
                                        Item realItem = weapon.Copy();
                                        realItem.SetGamingQueue(queue);
                                        WriteLine($"[ {character} ] 消费了 {wCost} {General.GameplayEquilibriumConstant.InGameCurrency}，购买了武器-{realItem.Name}！详细说明：\r\n{realItem}");
                                        queue.Equip(character, realItem);
                                    }
                                    else failedBuyTimes++;
                                    break;
                                case "买防具":
                                    Item armor = armors[Random.Shared.Next(armors.Length)];
                                    double aCost = armor.Price;
                                    if (character.User.Inventory.Credits >= aCost)
                                    {
                                        character.User.Inventory.Credits -= aCost;
                                        operation["买防具"] = true;
                                        Item realItem = armor.Copy();
                                        realItem.SetGamingQueue(queue);
                                        WriteLine($"[ {character} ] 消费了 {aCost} {General.GameplayEquilibriumConstant.InGameCurrency}，购买了防具-{realItem.Name}！详细说明：\r\n{realItem}");
                                        queue.Equip(character, realItem);
                                    }
                                    else failedBuyTimes++;
                                    break;
                                case "买鞋子":
                                    Item shoe = shoes[Random.Shared.Next(shoes.Length)];
                                    double sCost = shoe.Price;
                                    if (character.User.Inventory.Credits >= sCost)
                                    {
                                        character.User.Inventory.Credits -= sCost;
                                        operation["买鞋子"] = true;
                                        Item realItem = shoe.Copy();
                                        realItem.SetGamingQueue(queue);
                                        WriteLine($"[ {character} ] 消费了 {sCost} {General.GameplayEquilibriumConstant.InGameCurrency}，购买了鞋子-{realItem.Name}！详细说明：\r\n{realItem}");
                                        queue.Equip(character, realItem);
                                    }
                                    else failedBuyTimes++;
                                    break;
                                case "买饰品":
                                    Item accessory = accessories[Random.Shared.Next(accessories.Length)];
                                    double acCost = accessory.Price;
                                    if (character.User.Inventory.Credits >= acCost)
                                    {
                                        character.User.Inventory.Credits -= acCost;
                                        operation["买饰品"] = true;
                                        Item realItem = accessory.Copy();
                                        realItem.SetGamingQueue(queue);
                                        WriteLine($"[ {character} ] 消费了 {acCost} {General.GameplayEquilibriumConstant.InGameCurrency}，购买了饰品-{realItem.Name}！详细说明：\r\n{realItem}");
                                        queue.Equip(character, realItem);
                                    }
                                    else failedBuyTimes++;
                                    break;
                                default:
                                    break;
                            }
                        }
                        // 看还要不要买，超过两次购买失败说明没钱别买了
                        buy = failedBuyTimes < 2 && funcs.All(f => f());
                    }
                    WriteLine($"[ {character} ] 结束购买，剩余{General.GameplayEquilibriumConstant.InGameCurrency}：{character.User.Inventory.Credits:0.00}。");
                }
            }
        }

        public static void DropItems(GamingQueue queue, int mQuality, int wQuality, int aQuality, int sQuality, int acQuality, bool addLevel = true)
        {
            Item[] weapons = [.. FunGameConstant.Equipment.Where(i => i.ItemType == ItemType.Weapon && (int)i.QualityType == wQuality)];
            Item[] armors = [.. FunGameConstant.Equipment.Where(i => i.ItemType == ItemType.Armor && (int)i.QualityType == aQuality)];
            Item[] shoes = [.. FunGameConstant.Equipment.Where(i => i.ItemType == ItemType.Shoes && (int)i.QualityType == sQuality)];
            Item[] accessories = [.. FunGameConstant.Equipment.Where(i => i.ItemType == ItemType.Accessory && (int)i.QualityType == acQuality)];
            Item[] consumables = [.. FunGameConstant.AllItems.Where(i => i.ItemType == ItemType.Consumable && i.IsInGameItem)];
            foreach (Character character in queue.AllCharacters)
            {
                if (addLevel)
                {
                    character.SetLevel(character.Level + 8, false);
                    character.NormalAttack.Level += 1;
                    foreach (Skill skill in character.Skills)
                    {
                        if (skill.SkillType == SkillType.Passive) continue;
                        skill.Level += 1;
                    }
                    Character? original = queue.Original[character.Guid];
                    if (original != null)
                    {
                        original.Level += 8;
                        original.NormalAttack.Level += 1;
                        foreach (Skill skill in original.Skills)
                        {
                            if (skill.SkillType == SkillType.Passive) continue;
                            skill.Level += 1;
                        }
                    }
                }
                Item? mcp = FunGameService.GenerateMagicCardPack(3, (QualityType)mQuality);
                if (mcp != null)
                {
                    foreach (Skill magic in mcp.Skills.Magics)
                    {
                        magic.Level = character.NormalAttack.Level;
                    }
                    mcp.SetGamingQueue(queue);
                    queue.Equip(character, mcp);
                }
                if (wQuality == -1 && aQuality == -1 && sQuality == -1 && acQuality == -1)
                {
                    continue;
                }
                Item? weapon = null, armor = null, shoe = null, accessory1 = null, accessory2 = null;
                if (weapons.Length > 0)
                {
                    weapon = weapons[Random.Shared.Next(weapons.Length)];
                }
                if (armors.Length > 0)
                {
                    armor = armors[Random.Shared.Next(armors.Length)];
                }
                if (shoes.Length > 0)
                {
                    shoe = shoes[Random.Shared.Next(shoes.Length)];
                }
                if (accessories.Length > 0)
                {
                    accessory1 = accessories[Random.Shared.Next(accessories.Length)];
                }
                if (accessories.Length > 0)
                {
                    accessory2 = accessories[Random.Shared.Next(accessories.Length)];
                }
                List<Item> thisDrops = [];
                if (weapon != null) thisDrops.Add(weapon);
                if (armor != null) thisDrops.Add(armor);
                if (shoe != null) thisDrops.Add(shoe);
                if (accessory1 != null) thisDrops.Add(accessory1);
                if (accessory2 != null) thisDrops.Add(accessory2);
                foreach (Item item in thisDrops)
                {
                    Item realItem = item.Copy();
                    realItem.SetGamingQueue(queue);
                    queue.Equip(character, realItem);
                }
                if (consumables.Length > 0 && character.Items.Count < 5)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Item consumable = consumables[Random.Shared.Next(consumables.Length)].Copy();
                        character.Items.Add(consumable);
                    }
                }
            }
        }

        public static void UpdateStatistics(CharacterStatistics totalStats, CharacterStatistics stats)
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
            totalStats.TotalHeal = Calculation.Round2Digits(totalStats.TotalHeal + stats.TotalHeal);
            totalStats.LiveRound += stats.LiveRound;
            totalStats.ActionTurn += stats.ActionTurn;
            totalStats.LiveTime = Calculation.Round2Digits(totalStats.LiveTime + stats.LiveTime);
            totalStats.ControlTime = Calculation.Round2Digits(totalStats.ControlTime + stats.ControlTime);
            totalStats.TotalShield = Calculation.Round2Digits(totalStats.TotalShield + stats.TotalShield);
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
                totalStats.AvgTrueDamage = Calculation.Round2Digits(totalStats.TotalTrueDamage / totalStats.Plays);
                totalStats.AvgTakenDamage = Calculation.Round2Digits(totalStats.TotalTakenDamage / totalStats.Plays);
                totalStats.AvgTakenPhysicalDamage = Calculation.Round2Digits(totalStats.TotalTakenPhysicalDamage / totalStats.Plays);
                totalStats.AvgTakenMagicDamage = Calculation.Round2Digits(totalStats.TotalTakenMagicDamage / totalStats.Plays);
                totalStats.AvgTakenTrueDamage = Calculation.Round2Digits(totalStats.TotalTakenTrueDamage / totalStats.Plays);
                totalStats.AvgHeal = Calculation.Round2Digits(totalStats.TotalHeal / totalStats.Plays);
                totalStats.AvgLiveRound = totalStats.LiveRound / totalStats.Plays;
                totalStats.AvgActionTurn = totalStats.ActionTurn / totalStats.Plays;
                totalStats.AvgLiveTime = Calculation.Round2Digits(totalStats.LiveTime / totalStats.Plays);
                totalStats.AvgControlTime = Calculation.Round2Digits(totalStats.ControlTime / totalStats.Plays);
                totalStats.AvgShield = Calculation.Round2Digits(totalStats.TotalShield / totalStats.Plays);
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
