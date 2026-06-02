using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Models;
using Oshima.FunGame.OshimaServers.Model;
using Oshima.FunGame.WebAPI.Model;

namespace Oshima.FunGame.WebAPI.Services
{
    public partial class RainBOTService
    {
        public async Task<bool> HandleCSBettingInput(IBotMessage e, long uid)
        {
            if (e.Detail == "预测帮助" || e.Detail == "赛事帮助")
            {
                e.UseNotice = false;
                BotReply reply = new()
                {
                    Markdown = new MarkdownMessage
                    {
                        Content = "🎮 赛事预测系统帮助：\r\n"
                                + $"✨ {"赛事列表".CreateCmdInput()}   - 查看所有赛事\r\n"
                                + $"✨ {"比赛列表".CreateCmdInput()}   - 查看所有比赛\r\n"
                                + $"✨ {"创建存档".CreateCmdInput()}   - 创建存档后可预测\r\n"
                                + $"✨ {"我的预测".CreateCmdInput()}   - 查看我的预测记录\r\n"
                                + $"✨ {"预测领奖".CreateCmdInput()}   - 领取预测奖励\r\n"
                                + $"✨ {"比赛详情".CreateCmdInput()}   - 查看单场比赛并预测"
                    },
                    Keyboard = new KeyboardMessage()
                        .AppendButtons(2,
                            Button.CreateCmdButton("📋 赛事列表", "赛事列表"),
                            Button.CreateCmdButton("📅 比赛列表", "比赛列表"),
                            Button.CreateCmdButton("⚙️ 创建存档", "创建存档"),
                            Button.CreateCmdButton("📜 我的预测", "我的预测"),
                            Button.CreateCmdButton("💰 预测领奖", "预测领奖"),
                            Button.CreateCmdButton("❓ 预测帮助", "预测帮助"))
                };
                await SendAsync(e, "赛事预测", reply);
                return true;
            }

            // 赛程：显示所有比赛
            if (e.Detail.StartsWith("赛程") || e.Detail.StartsWith("比赛列表"))
            {
                int page = 1;
                string detail = e.Detail.Replace("赛程", "").Replace("比赛列表", "").Trim();
                System.Text.RegularExpressions.Match match = GetFirstNumber().Match(detail);
                if (match.Success && int.TryParse(match.Value, out int p)) page = p;

                BotReply reply = BettingController.GetAllMatches(page);
                reply.Keyboard ??= new KeyboardMessage();
                reply.Keyboard.AppendButtonsWithNewRow(2,
                    Button.CreateCmdButton("📋 赛事列表", "赛事列表"),
                    Button.CreateCmdButton("📅 比赛列表", "比赛列表"),
                    Button.CreateCmdButton("📜 我的预测", "我的预测"),
                    Button.CreateCmdButton("❓ 预测帮助", "预测帮助"));
                await SendAsync(e, "比赛赛程", reply);
                return true;
            }

            if (e.Detail.StartsWith("比赛详情"))
            {
                string detail = e.Detail.Replace("比赛详情", "").Trim();
                if (int.TryParse(detail, out int matchId))
                {
                    BotReply reply = BettingController.GetMatchDetail(matchId);
                    reply.Keyboard ??= new();
                    reply.Keyboard.AppendButtonsWithNewRow(2,
                        Button.CreateCmdButton("📋 赛事列表", "赛事列表"),
                        Button.CreateCmdButton("📅 比赛列表", "比赛列表"),
                        Button.CreateCmdButton("💰 预测领奖", "预测领奖"),
                        Button.CreateCmdButton("❓ 预测帮助", "预测帮助"));
                    BotReply reply2 = BettingController.GetMyBets(uid, mid: matchId);
                    if (reply.Markdown != null && reply.Markdown.Content != null && !(reply2.Markdown?.Content?.Equals("你还没有任何预测记录。") ?? true))
                    {
                        reply.Markdown.Content += "\r\n" + reply2.Markdown.Content;
                    }
                    await SendAsync(e, "赛事预测", reply);
                }
                else
                {
                    await SendAsync(e, "赛事预测", "格式：比赛详情 <比赛ID>");
                }
                return true;
            }

            // 赛事详情：用于查看某一赛事下的所有比赛
            if (e.Detail.StartsWith("赛事详情"))
            {
                string detail = e.Detail["赛事详情".Length..].Trim();
                // 解析事件ID和页码
                string[] parts = detail.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 1 && int.TryParse(parts[0], out int eventId))
                {
                    int page = 1;
                    if (parts.Length > 1 && int.TryParse(parts[1], out int p)) page = p;
                    // 调用控制器获取详情（返回BotReply）
                    BotReply reply = BettingController.GetEventDetail(eventId, page);
                    reply.Keyboard ??= new KeyboardMessage();
                    reply.Keyboard.AppendButtonsWithNewRow(2,
                            Button.CreateCmdButton("📋 赛事列表", "赛事列表"),
                            Button.CreateCmdButton("📅 比赛列表", "比赛列表"),
                            Button.CreateCmdButton("📜 我的预测", "我的预测"),
                            Button.CreateCmdButton("💰 预测领奖", "预测领奖"));
                    await SendAsync(e, "赛事预测", reply);
                }
                else
                {
                    BotReply reply = new()
                    {
                        Markdown = new()
                        {
                            Content = "格式：赛事详情 <赛事ID>"
                        },
                        Keyboard = new KeyboardMessage()
                            .AppendButtons(2,
                                Button.CreateCmdButton("📋 赛事列表", "赛事列表"),
                                Button.CreateCmdButton("📅 比赛列表", "比赛列表"),
                                Button.CreateCmdButton("❓ 预测帮助", "预测帮助"),
                                Button.CreateCmdButton("📜 我的预测", "我的预测"))
                    };
                    await SendAsync(e, "赛事预测", reply);
                }
                return true;
            }

            // 赛事列表
            if (e.Detail.StartsWith("赛事") || e.Detail.StartsWith("赛事列表"))
            {
                int page = 1;
                string detail = e.Detail.Replace("赛事", "").Replace("赛事列表", "").Trim();
                System.Text.RegularExpressions.Match match = GetFirstNumber().Match(detail);
                if (match.Success && int.TryParse(match.Value, out int p)) page = p;
                BotReply reply = BettingController.GetEventsOverview(page);
                await SendAsync(e, "赛事预测", reply);
                return true;
            }

            if (e.Detail.StartsWith("我的预测"))
            {
                int page = 1;
                string detail = e.Detail.Replace("我的预测", "").Trim();
                System.Text.RegularExpressions.Match match = GetFirstNumber().Match(detail);
                if (match.Success && int.TryParse(match.Value, out int p)) page = p;
                BotReply reply = BettingController.GetMyBets(uid, page);
                reply.Keyboard ??= new KeyboardMessage();
                reply.Keyboard.AppendButtonsWithNewRow(2,
                        Button.CreateCmdButton("💰 预测领奖", "预测领奖", enter: true),
                        Button.CreateCmdButton("📋 赛事列表", "赛事列表"),
                        Button.CreateCmdButton("📅 比赛列表", "比赛列表"),
                        Button.CreateCmdButton("❓ 预测帮助", "预测帮助"));
                if (reply.Markdown?.Content?.Contains("创建存档") ?? false)
                {
                    reply.Keyboard.AppendButtons(2, Button.CreateCmdButton("⚙️ 创建存档", "创建存档"));
                }
                await SendAsync(e, "赛事预测", reply);
                return true;
            }

            if (e.Detail == "预测领奖")
            {
                BotReply reply = BettingController.ClaimRewards(uid);
                reply.Keyboard = new KeyboardMessage()
                    .AppendButtons(2,
                        Button.CreateCmdButton("📜 我的预测", "我的预测"),
                        Button.CreateCmdButton("📋 赛事列表", "赛事列表"),
                        Button.CreateCmdButton("📅 比赛列表", "比赛列表"));
                if (reply.Markdown?.Content?.Contains("创建存档") ?? false)
                {
                    reply.Keyboard.AppendButtons(2, Button.CreateCmdButton("⚙️ 创建存档", "创建存档"));
                }
                await SendAsync(e, "赛事预测", reply);
                return true;
            }

            if (e.Detail.StartsWith("预测"))
            {
                string detail = e.Detail.Replace("预测", "").Trim();
                string[] parts = detail.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 3 || !int.TryParse(parts[0], out int mid) || !long.TryParse(parts[^1], out long amt))
                {
                    await SendAsync(e, "赛事预测", $"格式：预测 <比赛ID> <选项> <{General.GameplayEquilibriumConstant.InGameCurrency}数>\r\n选项：team1 / team2 / score:2:0 / mvp:选手UID");
                    return true;
                }
                string option = string.Join(" ", parts[1..^1]).ToLower();

                BotReply reply = BettingController.PlaceBet(uid, mid, option, amt);

                KeyboardMessage kb = new();
                kb.AppendButtons(2,
                    Button.CreateCmdButton("📜 我的预测", "我的预测"),
                    Button.CreateCmdButton("📋 赛事列表", "赛事列表"),
                    Button.CreateCmdButton("📅 比赛列表", "比赛列表"),
                    Button.CreateCmdButton("❓ 预测帮助", "预测帮助"));

                if (reply.Markdown?.Content?.Contains("创建存档") ?? false)
                {
                    kb.AppendButtons(2, Button.CreateCmdButton("⚙️ 创建存档", "创建存档"));
                }
                else
                {
                    kb.AppendButtonsWithNewRow(2,
                        Button.CreateCmdButton("🔄 再次预测", e.Detail, enter: false),
                        Button.CreateCmdButton("🔍 比赛详情", $"比赛详情 {mid}"));
                }

                reply.Keyboard = kb;
                await SendAsync(e, "赛事预测", reply);
                return true;
            }

            // 管理员结算
            if (e.Detail.StartsWith("结算比赛"))
            {
                string detail = e.Detail.Replace("结算比赛", "").Trim();
                string[] parts = detail.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2 && int.TryParse(parts[0], out int mid))
                {
                    string winner = "", mResult = "";
                    foreach (var p in parts[1..])
                    {
                        if (p.StartsWith("winner=")) winner = p[7..];
                        if (p.StartsWith("result=")) mResult = p[7..];
                    }
                    BotReply reply = BettingController.SettleMatch(uid, mid, winner, mResult);
                    reply.Keyboard = new KeyboardMessage()
                        .AppendButtons(2,
                            Button.CreateCmdButton("📋 赛事列表", "赛事列表"),
                            Button.CreateCmdButton("📅 比赛列表", "比赛列表"),
                            Button.CreateCmdButton("⚙️ 继续结算", "结算比赛 ", enter: false),
                            Button.CreateCmdButton("🔍 比赛详情", $"比赛详情 {mid}"));
                    await SendAsync(e, "赛事预测", reply);
                }
                else
                    await SendAsync(e, "赛事预测", "格式：结算比赛 <比赛ID> winner=team1 result=2:0");
                return true;
            }

            // 指令：创建赛事 <名称> <开始时间> <结束时间>
            // 示例：创建赛事 春季赛 2026-03-01 2026-03-10
            // 示例：创建赛事 总决赛 2026-04-01 2026-04-05 1001,1002
            if (e.Detail.StartsWith("创建赛事"))
            {
                e.UseNotice = false;
                if (!FunGameConstant.UserIdAndUsername.TryGetValue(uid, out User? user) || (!user.IsAdmin && !user.IsOperator))
                {
                    await SendAsync(e, "创建赛事", "你没有权限执行此操作。");
                    return true;
                }

                string detail = e.Detail.Replace("创建赛事", "").Trim();
                string[] parts = detail.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 3)
                {
                    await SendAsync(e, "创建赛事", "格式：创建赛事 <名称> <开始时间> <结束时间>\r\n" +
                        "时间格式：yyyy-MM-dd HH:mm 或 yyyy-MM-dd（默认00:00）\r\n" +
                        "示例：创建赛事 春季赛 2026-03-01 2026-03-10\r\n" +
                        "示例：创建赛事 总决赛 2026-04-01 12:00 2026-04-05 18:00");
                    return true;
                }

                string name = parts[0];
                if (name.Length > 100)
                {
                    await SendAsync(e, "创建赛事", "赛事名称不能超过100个字符。");
                    return true;
                }

                // 尝试解析时间（支持 yyyy-MM-dd 或 yyyy-MM-dd HH:mm）
                string startStr = parts[1] + (parts[2].Contains(':') ? " " + parts[2] : " 00:00");
                int nextIndex = parts[2].Contains(':') ? 3 : 2;
                string endStr = parts[nextIndex] + (parts.Length > nextIndex + 1 && parts[nextIndex + 1].Contains(':') ? " " + parts[nextIndex + 1] : " 00:00");

                if (!DateTime.TryParseExact(startStr, CheckDateTimeFormat, null, System.Globalization.DateTimeStyles.None, out DateTime startTime) ||
                    !DateTime.TryParseExact(endStr, CheckDateTimeFormat, null, System.Globalization.DateTimeStyles.None, out DateTime endTime))
                {
                    await SendAsync(e, "创建赛事", "时间格式错误，请使用 yyyy-MM-dd 或 yyyy-MM-dd HH:mm。");
                    return true;
                }

                BotReply reply = BettingController.CreateEvent(new CreateEventRequest
                {
                    Uid = uid,
                    Name = name,
                    StartTime = startTime,
                    EndTime = endTime
                });
                await SendAsync(e, "创建赛事", reply);
                return true;
            }

            // 指令：创建比赛 <赛事ID> <队伍1> <队伍2> <阶段> <开始时间> [选项列表(key=value格式，空格分隔)]
            if (e.Detail.StartsWith("创建比赛"))
            {
                e.UseNotice = false;
                if (!FunGameConstant.UserIdAndUsername.TryGetValue(uid, out User? user) || (!user.IsAdmin && !user.IsOperator))
                {
                    await SendAsync(e, "创建比赛", "你没有权限执行此操作。");
                    return true;
                }

                string detail = e.Detail.Replace("创建比赛", "").Trim();
                string[] parts = detail.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 6) // 最少需要 eventId, team1, team2, stage, start date, start time
                {
                    await SendAsync(e, "创建比赛",
                        "格式：创建比赛 <赛事ID> <队伍1> <队伍2> <阶段> <开始时间> [选项列表(key=value格式，空格分隔)]\r\n" +
                        "开始时间格式：yyyy-MM-dd HH:mm（两段）\r\n" +
                        "可选参数：ddl=截止时间（需要用双引号包围），opts=选项列表（逗号分隔，默认team1_win,team2_win），team1_win=奖励率，team2_win=奖励率，prob=胜率\r\n" +
                        "示例：创建比赛 1 NAVI FaZe Quarter-final 2026-03-05 14:00\r\n" +
                        "示例：创建比赛 1 NAVI FaZe Quarter-final 2026-03-05 14:00 ddl=\"2026-03-05 13:55\" opts=team1_win,team2_win\r\n" +
                        "示例：创建比赛 1 G2 Vitality Semi-final 2026-04-02 18:00 ddl=\"2026-04-02 17:55\" opts=team1_win,team2_win team1_win=2.5 prob=0.6");
                    return true;
                }

                if (!int.TryParse(parts[0], out int eventId))
                {
                    await SendAsync(e, "创建比赛", "赛事ID必须为数字。");
                    return true;
                }

                string team1 = parts[1];
                string team2 = parts[2];
                string stage = parts[3];

                // 开始时间（parts[4] + parts[5]）
                string startDate = parts[4];
                string startTime = parts[5];
                if (!DateTime.TryParseExact(startDate + " " + startTime, "yyyy-MM-dd HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime startDt))
                {
                    await SendAsync(e, "创建比赛", "开始时间格式错误，请使用 yyyy-MM-dd HH:mm（日期和时间分为两段）。");
                    return true;
                }

                // 解析剩余参数（从第6段开始，key=value格式，value支持双引号包围）
                int spaceCount = 0, idx = 0;
                for (; idx < detail.Length && spaceCount < 6; idx++)
                {
                    if (detail[idx] == ' ') spaceCount++;
                }
                string paramString = detail[idx..].Trim();
                System.Text.RegularExpressions.MatchCollection matches = GetParamValue().Matches(paramString);

                // 解析剩余可选参数：key=value 格式，支持双引号包围含空格的value
                Dictionary<string, string> paramDict = new(StringComparer.OrdinalIgnoreCase);
                foreach (System.Text.RegularExpressions.Match m in matches)
                {
                    string key = m.Groups[1].Value;
                    string value = m.Groups[2].Success ? m.Groups[2].Value : m.Groups[3].Value;
                    if (!paramDict.TryAdd(key, value))
                    {
                        await SendAsync(e, "创建比赛", $"参数 '{key}' 重复。");
                        return true;
                    }
                }

                // 解析截止时间 (ddl=)，默认与开始时间相同
                DateTime deadlineDt = startDt;
                if (paramDict.TryGetValue("ddl", out string? ddlStr))
                {
                    if (!DateTime.TryParseExact(ddlStr, "yyyy-MM-dd HH:mm", null, System.Globalization.DateTimeStyles.None, out deadlineDt))
                    {
                        await SendAsync(e, "创建比赛", "截止时间格式错误，请使用 yyyy-MM-dd HH:mm。");
                        return true;
                    }
                }

                // 解析选项列表 (opts=)
                string options = "team1_win,team2_win";
                if (paramDict.TryGetValue("opts", out string? optsStr))
                {
                    options = optsStr; // 预期逗号分隔的列表
                }

                // 解析奖励率与胜率
                decimal? team1Odds = null, team2Odds = null, team1WinProbability = null;
                if (paramDict.TryGetValue("team1_win", out string? t1OddsStr))
                {
                    if (decimal.TryParse(t1OddsStr, out decimal o1))
                    {
                        team1Odds = o1;
                    }
                    else
                    {
                        await SendAsync(e, "创建比赛", "team1_win 奖励率无效。");
                        return true;
                    }
                }
                if (paramDict.TryGetValue("team2_win", out string? t2OddsStr))
                {
                    if (decimal.TryParse(t2OddsStr, out decimal o2))
                    {
                        team2Odds = o2;
                    }
                    else
                    {
                        await SendAsync(e, "创建比赛", "team2_win 奖励率无效。");
                        return true;
                    }
                }
                if (paramDict.TryGetValue("prob", out string? probStr))
                {
                    if (decimal.TryParse(probStr, out decimal prob) && prob > 0 && prob < 1)
                    {
                        team1WinProbability = prob;
                    }
                    else
                    {
                        await SendAsync(e, "创建比赛", "胜率值无效，应为0~1之间的小数。");
                        return true;
                    }
                }
                bool enableBet = true;
                if (paramDict.TryGetValue("be", out string? be))
                {
                    if (be == "0" || be == "1") enableBet = be == "1";
                    else
                    {
                        await SendAsync(e, "创建比赛", "enabled 值必须为 0 或 1。");
                        return true;
                    }
                }
                // 如果队伍包含TBD，则默认不可预测
                if (team1 == "TBD" || team2 == "TBD")
                {
                    enableBet = false;
                }

                BotReply reply = BettingController.CreateMatch(new CreateMatchRequest
                {
                    Uid = uid,
                    EventId = eventId,
                    Team1Name = team1,
                    Team2Name = team2,
                    Stage = stage,
                    StartTime = startDt,
                    BetDeadline = deadlineDt,
                    AvailableOptions = options,
                    Team1WinOdds = team1Odds,
                    Team2WinOdds = team2Odds,
                    Team1WinProbability = team1WinProbability,
                    BettingEnabled = enableBet
                });
                await SendAsync(e, "创建比赛", reply);
                return true;
            }

            // 指令：修改比赛 <比赛ID> [参数=值 ...]
            if (e.Detail.StartsWith("修改比赛"))
            {
                e.UseNotice = false;
                if (!FunGameConstant.UserIdAndUsername.TryGetValue(uid, out User? user) || (!user.IsAdmin && !user.IsOperator))
                {
                    await SendAsync(e, "修改比赛", "你没有权限执行此操作。");
                    return true;
                }

                string detail = e.Detail.Replace("修改比赛", "").Trim();
                string[] parts = detail.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2)
                {
                    await SendAsync(e, "修改比赛",
                        "格式：修改比赛 <比赛ID> [参数=值]\r\n" +
                        "可用参数：t1od=<奖励率> t2od=<奖励率> st=<开始时间> ddl=<截止时间> des=<描述>\r\n" +
                        "时间格式：yyyy-MM-dd HH:mm\r\n" +
                        "示例：修改比赛 10 t1od=2.8 ddl=2026-05-12 18:00 des=\"决赛，Bo3\"");
                    return true;
                }

                if (!int.TryParse(parts[0], out int matchId))
                {
                    await SendAsync(e, "修改比赛", "比赛ID必须为数字。");
                    return true;
                }

                // 解析剩余参数 key=value，支持引号包裹的含空格值
                UpdateMatchRequest request = new() { Uid = uid, MatchId = matchId };
                string remaining = string.Join(" ", parts[1..]);
                // 使用正则匹配 key=value，value 可能带双引号
                System.Text.RegularExpressions.MatchCollection matches = GetParamValue().Matches(remaining);
                foreach (System.Text.RegularExpressions.Match m in matches)
                {
                    string key = m.Groups[1].Value;
                    string val = m.Groups[2].Success ? m.Groups[2].Value : m.Groups[3].Value;

                    switch (key)
                    {
                        case "t1od":
                            if (decimal.TryParse(val, out decimal t1od)) request.Team1WinOdds = t1od;
                            else { await SendAsync(e, "修改比赛", "t1od 值无效。"); return true; }
                            break;
                        case "t2od":
                            if (decimal.TryParse(val, out decimal t2od)) request.Team2WinOdds = t2od;
                            else { await SendAsync(e, "修改比赛", "t2od 值无效。"); return true; }
                            break;
                        case "prob":
                            if (decimal.TryParse(val, out decimal prob)) request.Team1WinProbability = prob;
                            else { await SendAsync(e, "修改比赛", "prob 值无效。"); return true; }
                            break;
                        case "st":
                            if (DateTime.TryParse(val, out DateTime st)) request.StartTime = st;
                            else { await SendAsync(e, "修改比赛", "开始时间格式错误，请用 yyyy-MM-dd HH:mm。"); return true; }
                            break;
                        case "ddl":
                            if (DateTime.TryParse(val, out DateTime ddl)) request.BetDeadline = ddl;
                            else { await SendAsync(e, "修改比赛", "截止时间格式错误，请用 yyyy-MM-dd HH:mm。"); return true; }
                            break;
                        case "des":
                            request.Description = val; // 允许空字符串清空描述
                            break;
                        case "result":
                            request.Result = val;
                            break;
                        case "stage":
                            request.Stage = val;
                            break;
                        case "t1":
                            request.Team1 = val;
                            break;
                        case "t2":
                            request.Team2 = val;
                            break;
                        case "be":
                            if (val == "0" || val == "1")
                                request.BettingEnabled = val == "1";
                            else
                            {
                                await SendAsync(e, "修改比赛", "enabled 值必须为 0 或 1。");
                                return true;
                            }
                            break;
                        default:
                            await SendAsync(e, "修改比赛", $"未知参数：{key}");
                            return true;
                    }
                }

                // 调用控制器API
                BotReply reply = BettingController.UpdateMatch(request);
                reply.Keyboard = new KeyboardMessage()
                    .AppendButtons(2,
                        Button.CreateCmdButton("📋 赛事列表", "赛事列表"),
                        Button.CreateCmdButton("📅 比赛列表", "比赛列表"),
                        Button.CreateCmdButton("🔍 比赛详情", $"比赛详情 {matchId}"));
                await SendAsync(e, "修改比赛", reply);
                return true;
            }

            // 取消比赛指令
            if (e.Detail.StartsWith("取消比赛"))
            {
                e.UseNotice = false;
                string detail = e.Detail.Replace("取消比赛", "").Trim();
                if (!int.TryParse(detail, out int matchId))
                {
                    await SendAsync(e, "取消比赛", "格式：取消比赛 <比赛ID>");
                    return true;
                }

                BotReply reply = BettingController.CancelMatch(uid, matchId);
                reply.Keyboard = new KeyboardMessage()
                .AppendButtons(2,
                    Button.CreateCmdButton("📋 赛事列表", "赛事列表"),
                    Button.CreateCmdButton("📅 比赛列表", "比赛列表"),
                    Button.CreateCmdButton("🔍 比赛详情", $"比赛详情 {matchId}"),
                    Button.CreateCmdButton("💰 预测领奖", "预测领奖"));
                await SendAsync(e, "取消比赛", reply);
                return true;
            }

            if (e.Detail.StartsWith("推迟比赛"))
            {
                e.UseNotice = false;
                if (!FunGameConstant.UserIdAndUsername.TryGetValue(uid, out User? user) || (!user.IsAdmin && !user.IsOperator))
                {
                    await SendAsync(e, "修改比赛", "你没有权限执行此操作。");
                    return true;
                }

                string detail = e.Detail.Replace("推迟比赛", "").Trim();
                string[] parts = detail.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2 || !int.TryParse(parts[0], out int matchId) || !int.TryParse(parts[1], out int minutes) || minutes <= 0)
                {
                    await SendAsync(e, "修改比赛", "格式：推迟比赛 <比赛ID> <分钟数>\n示例：推迟比赛 123 30");
                    return true;
                }

                // 获取当前比赛时间
                if (!CSBettingService.GetMatchTimes(matchId, out DateTime oldStart, out DateTime oldDeadline, out int status, out string error))
                {
                    await SendAsync(e, "修改比赛", error);
                    return true;
                }

                if (status == 2)
                {
                    await SendAsync(e, "修改比赛", "比赛已结束，无法推迟。");
                    return true;
                }

                DateTime newStart = oldStart.AddMinutes(minutes);
                DateTime newDeadline = oldDeadline.AddMinutes(minutes);

                UpdateMatchRequest request = new()
                {
                    Uid = uid,
                    MatchId = matchId,
                    StartTime = newStart,
                    BetDeadline = newDeadline
                };

                BotReply reply = BettingController.UpdateMatch(request);
                reply.Keyboard = new KeyboardMessage()
                    .AppendButtons(2,
                        Button.CreateCmdButton("📋 赛事列表", "赛事列表"),
                        Button.CreateCmdButton("📅 比赛列表", "比赛列表"),
                        Button.CreateCmdButton("🔍 比赛详情", $"比赛详情 {matchId}"));
                await SendAsync(e, "修改比赛", reply);
                return true;
            }

            // 提前比赛
            if (e.Detail.StartsWith("提前比赛"))
            {
                e.UseNotice = false;
                if (!FunGameConstant.UserIdAndUsername.TryGetValue(uid, out User? user) || (!user.IsAdmin && !user.IsOperator))
                {
                    await SendAsync(e, "修改比赛", "你没有权限执行此操作。");
                    return true;
                }

                string detail = e.Detail.Replace("提前比赛", "").Trim();
                string[] parts = detail.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2 || !int.TryParse(parts[0], out int matchId) || !int.TryParse(parts[1], out int minutes) || minutes <= 0)
                {
                    await SendAsync(e, "修改比赛", "格式：提前比赛 <比赛ID> <分钟数>\n示例：提前比赛 123 30");
                    return true;
                }

                // 获取当前比赛时间
                if (!CSBettingService.GetMatchTimes(matchId, out DateTime oldStart, out DateTime oldDeadline, out int status, out string error))
                {
                    await SendAsync(e, "修改比赛", error);
                    return true;
                }

                if (status == 2)
                {
                    await SendAsync(e, "修改比赛", "比赛已结束，无法推迟。");
                    return true;
                }

                DateTime newStart = oldStart.AddMinutes(-minutes);
                DateTime newDeadline = oldDeadline.AddMinutes(-minutes);

                UpdateMatchRequest request = new()
                {
                    Uid = uid,
                    MatchId = matchId,
                    StartTime = newStart,
                    BetDeadline = newDeadline
                };

                BotReply reply = BettingController.UpdateMatch(request);
                reply.Keyboard = new KeyboardMessage()
                    .AppendButtons(2,
                        Button.CreateCmdButton("📋 赛事列表", "赛事列表"),
                        Button.CreateCmdButton("📅 比赛列表", "比赛列表"),
                        Button.CreateCmdButton("🔍 比赛详情", $"比赛详情 {matchId}"));
                await SendAsync(e, "修改比赛", reply);
                return true;
            }

            return false;
        }

        [System.Text.RegularExpressions.GeneratedRegex(@"\d+")]
        private static partial System.Text.RegularExpressions.Regex GetFirstNumber();
        [System.Text.RegularExpressions.GeneratedRegex(@"(\w+)=(?:""([^""]*)""|(\S+))")]
        private static partial System.Text.RegularExpressions.Regex GetParamValue();
    }
}
