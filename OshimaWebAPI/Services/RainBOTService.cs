using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.Core.Configs;
using Oshima.Core.Constant;
using Oshima.FunGame.OshimaModules.Models;
using Oshima.FunGame.OshimaServers.Service;
using Oshima.FunGame.WebAPI.Constant;
using Oshima.FunGame.WebAPI.Controllers;
using Oshima.FunGame.WebAPI.Models;

namespace Oshima.FunGame.WebAPI.Services
{
    public class RainBOTService(FunGameController controller, QQController qqcontroller, QQBotService service, ILogger<RainBOTService> logger, IMemoryCache memoryCache, TestController testController)
    {
        private static List<string> FunGameItemType { get; } = ["卡包", "武器", "防具", "鞋子", "饰品", "消耗品", "魔法卡", "收藏品", "特殊物品", "任务物品", "礼包", "其他"];
        private bool FunGameSimulation { get; set; } = false;
        private FunGameController Controller { get; } = controller;
        private QQController QQController { get; } = qqcontroller;
        private QQBotService Service { get; } = service;
        private ILogger<RainBOTService> Logger { get; } = logger;
        private IMemoryCache MemoryCache { get; set; } = memoryCache;
        private TestController TestController { get; set; } = testController;

        private async Task SendAsync(IBotMessage msg, string title, string content, int msgType = 0, object? media = null, int? msgSeq = null)
        {
            Statics.RunningPlugin?.Controller.WriteLine(title, Milimoe.FunGame.Core.Library.Constant.LogLevel.Debug);
            if (msg is ThirdPartyMessage third)
            {
                third.Result += "\r\n" + content.Trim();
                third.IsCompleted = true;
            }
            else if (msg.IsGroup)
            {
                content = "\r\n" + content.Trim();
                await Service.SendGroupMessageAsync(msg.OpenId, content, msgType, media, msg.Id, msgSeq);
            }
            else
            {
                content = content.Trim();
                await Service.SendC2CMessageAsync(msg.OpenId, content, msgType, media, msg.Id, msgSeq);
            }
            if (msg.UseNotice && msg.FunGameUID > 0 && FunGameService.UserNotice.TryGetValue(msg.FunGameUID, out HashSet<string>? msgs) && msgs != null)
            {
                FunGameService.UserNotice.Remove(msg.FunGameUID);
                await SendAsync(msg, "离线未读信箱", $"☆--- 离线未读信箱 ---☆\r\n{string.Join("\r\n", msgs)}", msgType, null, 5);
            }
        }

        private async Task SendHelp(IBotMessage e, Dictionary<string, string> helpDict, string helpName, int currentPage)
        {
            e.UseNotice = false;
            int pageSize = 15;
            int totalPages = (helpDict.Count + pageSize - 1) / pageSize;

            StringBuilder result = new($"《筽祀牻》{helpName}指令（第 {currentPage}/{totalPages} 页）\n");

            int index = (currentPage - 1) * pageSize + 1;
            foreach ((string cmd, string desc) in FunGameOrderList.GetPage(helpDict, currentPage, pageSize))
            {
                result.AppendLine($"{index}. {cmd}{(desc != "" ? "：" + desc : "")}");
                index++;
            }

            if (currentPage < totalPages)
            {
                result.AppendLine($"发送【{helpName}{currentPage + 1}】查看下一页");
            }

            await SendAsync(e, "筽祀牻", result.ToString());
        }

        public async Task<bool> Handler(IBotMessage e, OtherData data)
        {
            bool result = true;
            try
            {
                if (e is null)
                {
                    return false;
                }

                string isGroup = e.IsGroup ? "群聊" : "私聊";
                string openid = e.AuthorOpenId;
                long uid = 0;

                if (openid != "")
                {
                    if (MemoryCache.TryGetValue(openid, out object? value) && value is long uidTemp)
                    {
                        uid = uidTemp;
                        e.FunGameUID = uid;
                    }
                    else
                    {
                        using SQLHelper? sql = Factory.OpenFactory.GetSQLHelper();
                        if (sql != null)
                        {
                            sql.ExecuteDataSet(FunGameService.Select_CheckAutoKey(sql, openid));
                            if (sql.Success)
                            {
                                User user = Factory.GetUser(sql.DataSet);
                                uid = user.Id;
                                e.FunGameUID = uid;
                                MemoryCache.Set(openid, uid, TimeSpan.FromMinutes(10));
                            }
                        }
                    }
                }

                if (e.Detail == "重置状态")
                {
                    FunGameService.ReleaseUserSemaphoreSlim(uid);
                    await SendAsync(e, "筽祀牻", "Done");
                    return result;
                }

                if (FunGameService.CheckSemaphoreSlim(uid))
                {
                    await SendAsync(e, "筽祀牻", "检测到上一条指令尚未完成，若出现异常情况，请等待其执行完成，或者使用【重置状态】指令重置当前的指令执行状态。", msgSeq: 999);
                    return result;
                }

                //if (QQOpenID.QQAndOpenID.TryGetValue(openid, out long temp_qq))
                //{
                //    qq = temp_qq;
                //}

                //if (e.Detail.StartsWith("绑定"))
                //{
                //    string detail = e.Detail.Replace("绑定", "");
                //    string msg = "";
                //    if (long.TryParse(detail, out temp_qq))
                //    {
                //        msg = QQController.Bind(new(openid, temp_qq));
                //    }
                //    else
                //    {
                //        msg = "绑定失败，请提供一个正确的QQ号！";
                //    }
                //    await SendAsync(e, "绑定", msg);
                //}

                if (!e.IsGroup && e.Detail == "获取接入码")
                {
                    e.UseNotice = false;
                    await SendAsync(e, "获取接入码", $"你的接入码为 {openid}，请妥善保存！");
                    return true;
                }

                if (e.Detail == "我的运势")
                {
                    OpenUserDaily daily = new(openid, 0, "今日运势列表为空，请联系管理员设定");
                    if (QQOpenID.QQAndOpenID.TryGetValue(openid, out long qq) && qq != 0)
                    {
                        UserDaily qqDaily = UserDailyService.GetUserDaily(qq);
                        daily.type = qqDaily.type;
                        daily.daily = qqDaily.daily;
                    }
                    else
                    {
                        daily = UserDailyService.GetOpenUserDaily(openid);
                    }

                    if (daily.type != 0)
                    {
                        // 上传图片
                        string img = $@"{GeneralSettings.DailyImageServerUrl}/images/zi/";
                        img += daily.type switch
                        {
                            1 => "dj" + (new Random().Next(3) + 1) + ".png",
                            2 => "zj" + (new Random().Next(2) + 1) + ".png",
                            3 => "j" + (new Random().Next(4) + 1) + ".png",
                            4 => "mj" + (new Random().Next(2) + 1) + ".png",
                            5 => "x" + (new Random().Next(2) + 1) + ".png",
                            6 => "dx" + (new Random().Next(2) + 1) + ".png",
                            _ => ""
                        };

                        string? fi = "";
                        string? err = "";
                        try
                        {
                            var (fileUuid, fileInfo, ttl, error) = e.IsGroup ? await Service.UploadGroupMediaAsync(e.OpenId, 1, img) : await Service.UploadC2CMediaAsync(e.OpenId, 1, img);
                            fi = fileInfo;
                            err = error;
                        }
                        catch (Exception ex)
                        {
                            err = ex.ToString();
                        }
                        if (string.IsNullOrEmpty(err))
                        {
                            await SendAsync(e, "每日运势", daily.daily, 7, new { file_info = fi });
                        }
                        else
                        {
                            if (Logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Error)) Logger.LogError("上传图片失败：{error}", err);
                            await SendAsync(e, "每日运势", daily.daily);
                        }
                        string msg = Controller.GetUserDailyItem(uid, daily.daily);
                        if (msg != "")
                        {
                            await SendAsync(e, "运势幸运物发放", msg, msgSeq: 3);
                        }
                    }
                    else
                    {
                        await SendAsync(e, "每日运势", daily.daily);
                    }
                }

                if (e.Detail == "公告")
                {
                    e.UseNotice = false;
                    FunGameService.RefreshNotice();
                    if (FunGameService.Notices.Count > 0)
                    {
                        List<string> msgs = [];
                        DateTime now = DateTime.Now;
                        foreach (NoticeModel notice in FunGameService.Notices.Values)
                        {
                            if (now >= notice.StartTime && now <= notice.EndTime)
                            {
                                msgs.Add(notice.ToString());
                            }
                        }
                        await SendAsync(e, "公告", string.Join("\r\n", msgs));
                    }
                    else
                    {
                        await SendAsync(e, "公告", "当前没有任何系统公告。");
                    }
                    return true;
                }

                if (e.Detail == "刷新公告")
                {
                    e.UseNotice = false;
                    FunGameService.RefreshNotice();
                    return true;
                }

                if (e.Detail.StartsWith("添加公告"))
                {
                    e.UseNotice = false;
                    string author = "FunGame";
                    FunGameConstant.UserIdAndUsername.TryGetValue(uid, out User? user);
                    if (user is null || (!user.IsAdmin && !user.IsOperator))
                    {
                        await SendAsync(e, "公告", "你没有权限使用此指令。");
                        return true;
                    }
                    else
                    {
                        author = user.Username;
                    }
                    string detail = e.Detail.Replace("添加公告", "").Trim();
                    string[] strings = detail.Split(["\r\n", "\r", "\n"], StringSplitOptions.RemoveEmptyEntries);
                    string title = $"#Unknown";
                    if (strings.Length > 1)
                    {
                        title = strings[0].Trim();
                        detail = strings[1].Trim();
                    }
                    else
                    {
                        await SendAsync(e, "添加公告", $"格式错误：添加公告 <标题>\r\n<内容> [有效期 <天数>]。");
                        return true;
                    }
                    strings = detail.Split("有效期");
                    int days = 1;
                    if (strings.Length > 1)
                    {
                        if (int.TryParse(strings[1].Trim(), out int d))
                        {
                            days = d;
                        }
                        detail = strings[0].Trim();
                    }
                    FunGameService.Notices.Add(title, new NoticeModel()
                    {
                        Title = title,
                        Author = author,
                        Content = detail,
                        StartTime = DateTime.Now,
                        EndTime = DateTime.Today.AddHours(3).AddMinutes(59).AddSeconds(59).AddDays(days)
                    });
                    FunGameService.Notices.SaveConfig();
                    FunGameService.RefreshNotice();
                    await SendAsync(e, "添加公告", $"添加完毕，请查看【公告】列表！");
                    return true;
                }

                if (e.Detail == "查询服务器启动时间")
                {
                    e.UseNotice = false;
                    string msg = TestController.GetLastLoginTime();
                    await SendAsync(e, "查询服务器启动时间", msg);
                    return true;
                }

                if (e.Detail.StartsWith("查询任务计划"))
                {
                    e.UseNotice = false;
                    string msg = TestController.GetTaskScheduler(e.Detail.Replace("查询任务计划", ""));
                    await SendAsync(e, "查询任务计划", msg);
                    return true;
                }

                // 指令处理
                if (e.Detail == "帮助")
                {
                    e.UseNotice = false;
                    await SendAsync(e, "筽祀牻", @$"欢迎使用《筽祀牻》游戏指令帮助系统！
核心库版本号：{FunGameInfo.FunGame_Version}
《筽祀牻》是一款奇幻冒险回合制角色扮演游戏。
在游戏中，你可以和其他角色组成小队，收集物品，在数十个独具风格的地区中冒险并战斗。
因游戏内容、指令较多，我们将按游戏模块对指令分类，请输入以下指令查看具体分类的帮助内容：
1、存档帮助
2、角色帮助
3、物品帮助
4、战斗帮助
5、玩法帮助
6、社团帮助
7、活动帮助
8、商店帮助
在指令后面加数字即可跳转指定的页码，感谢游玩《筽祀牻》。");
                }

                if (e.Detail.StartsWith("存档帮助"))
                {
                    int page = e.Detail.Length > 4 && int.TryParse(e.Detail[4..], out int p) ? p : 1;
                    await SendHelp(e, FunGameOrderList.ArchiveHelp, "存档帮助", page);
                }
                else if (e.Detail.StartsWith("角色帮助"))
                {
                    int page = e.Detail.Length > 4 && int.TryParse(e.Detail[4..], out int p) ? p : 1;
                    await SendHelp(e, FunGameOrderList.CharacterHelp, "角色帮助", page);
                }
                else if (e.Detail.StartsWith("物品帮助"))
                {
                    int page = e.Detail.Length > 4 && int.TryParse(e.Detail[4..], out int p) ? p : 1;
                    await SendHelp(e, FunGameOrderList.ItemHelp, "物品帮助", page);
                }
                else if (e.Detail.StartsWith("战斗帮助"))
                {
                    int page = e.Detail.Length > 4 && int.TryParse(e.Detail[4..], out int p) ? p : 1;
                    await SendHelp(e, FunGameOrderList.BattleHelp, "战斗帮助", page);
                }
                else if (e.Detail.StartsWith("玩法帮助"))
                {
                    int page = e.Detail.Length > 4 && int.TryParse(e.Detail[4..], out int p) ? p : 1;
                    await SendHelp(e, FunGameOrderList.PlayHelp, "玩法帮助", page);
                }
                else if (e.Detail.StartsWith("社团帮助"))
                {
                    int page = e.Detail.Length > 4 && int.TryParse(e.Detail[4..], out int p) ? p : 1;
                    await SendHelp(e, FunGameOrderList.ClubHelp, "社团帮助", page);
                }
                else if (e.Detail.StartsWith("活动帮助"))
                {
                    int page = e.Detail.Length > 4 && int.TryParse(e.Detail[4..], out int p) ? p : 1;
                    await SendHelp(e, FunGameOrderList.ActivityHelp, "活动帮助", page);
                }
                else if (e.Detail.StartsWith("商店帮助"))
                {
                    int page = e.Detail.Length > 4 && int.TryParse(e.Detail[4..], out int p) ? p : 1;
                    await SendHelp(e, FunGameOrderList.StoreHelp, "商店帮助", page);
                }

                if (e.Detail.StartsWith("FunGame模拟", StringComparison.CurrentCultureIgnoreCase))
                {
                    e.UseNotice = false;
                    if (!FunGameSimulation)
                    {
                        FunGameSimulation = true;
                        List<string> msgs = await Controller.GetTest(false, maxRespawnTimesMix: 0);
                        List<string> real = MergeMessages(msgs);
                        int count = 1;
                        foreach (string msg in real)
                        {
                            await SendAsync(e, "筽祀牻", msg.Trim(), msgSeq: count++);
                            if (count != real.Count) await Task.Delay(5500);
                        }
                        FunGameSimulation = false;
                    }
                    else
                    {
                        await SendAsync(e, "筽祀牻", "游戏正在模拟中，请勿重复请求！");
                    }
                    return result;
                }

                if (e.Detail.StartsWith("混战模拟"))
                {
                    e.UseNotice = false;
                    int maxRespawnTimesMix = 1;
                    string detail = e.Detail.Replace("混战模拟", "").Trim();
                    if (int.TryParse(detail, out int times))
                    {
                        maxRespawnTimesMix = times;
                    }
                    if (!FunGameSimulation)
                    {
                        FunGameSimulation = true;
                        List<string> msgs = await Controller.GetTest(false, maxRespawnTimesMix: maxRespawnTimesMix);
                        List<string> real = MergeMessages(msgs);
                        int count = 1;
                        foreach (string msg in real)
                        {
                            await SendAsync(e, "筽祀牻", msg.Trim(), msgSeq: count++);
                            if (count != real.Count) await Task.Delay(5500);
                        }
                        FunGameSimulation = false;
                    }
                    else
                    {
                        await SendAsync(e, "筽祀牻", "游戏正在模拟中，请勿重复请求！");
                    }
                    return result;
                }

                if (e.Detail == "上次的完整日志")
                {
                    e.UseNotice = false;
                    await SendAsync(e, "筽祀牻", string.Join("\r\n", Controller.GetLast()));
                    return result;
                }

                if (e.Detail.StartsWith("FunGame团队模拟", StringComparison.CurrentCultureIgnoreCase))
                {
                    e.UseNotice = false;
                    if (!FunGameSimulation)
                    {
                        FunGameSimulation = true;
                        List<string> msgs = await Controller.GetTest(false, true);
                        List<string> real = MergeMessages(msgs);
                        int count = 1;
                        foreach (string msg in real)
                        {
                            await SendAsync(e, "筽祀牻", msg.Trim(), msgSeq: count++);
                            if (count != real.Count) await Task.Delay(5500);
                        }
                        FunGameSimulation = false;
                    }
                    else
                    {
                        await SendAsync(e, "筽祀牻", "游戏正在模拟中，请勿重复请求！");
                    }
                    return result;
                }

                if (e.Detail.StartsWith("查数据", StringComparison.CurrentCultureIgnoreCase))
                {
                    e.UseNotice = false;
                    string detail = e.Detail.Replace("查数据", "").Trim();
                    if (int.TryParse(detail, out int id))
                    {
                        string msg = Controller.GetStats(id);
                        if (msg != "")
                        {
                            await SendAsync(e, "查数据", msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("查团队数据", StringComparison.CurrentCultureIgnoreCase))
                {
                    e.UseNotice = false;
                    string detail = e.Detail.Replace("查团队数据", "").Trim();
                    if (int.TryParse(detail, out int id))
                    {
                        string msg = Controller.GetTeamStats(id);
                        if (msg != "")
                        {
                            await SendAsync(e, "查团队数据", msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("查个人胜率", StringComparison.CurrentCultureIgnoreCase))
                {
                    e.UseNotice = false;
                    List<string> msgs = Controller.GetWinrateRank();
                    if (msgs.Count > 0)
                    {
                        await SendAsync(e, "查个人胜率", string.Join("\r\n\r\n", msgs));
                    }
                    return result;
                }

                if (e.Detail.StartsWith("查团队胜率", StringComparison.CurrentCultureIgnoreCase))
                {
                    e.UseNotice = false;
                    List<string> msgs = Controller.GetWinrateRank(true);
                    if (msgs.Count > 0)
                    {
                        await SendAsync(e, "查团队胜率", string.Join("\r\n\r\n", msgs));
                    }
                    return result;
                }

                if (e.Detail.StartsWith("查角色", StringComparison.CurrentCultureIgnoreCase))
                {
                    e.UseNotice = false;
                    string detail = e.Detail.Replace("查角色", "").Trim();
                    if (int.TryParse(detail, out int id))
                    {
                        string msg = Controller.GetCharacterInfo(id);
                        if (msg != "")
                        {
                            await SendAsync(e, "查角色", msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("查技能", StringComparison.CurrentCultureIgnoreCase))
                {
                    e.UseNotice = false;
                    string detail = e.Detail.Replace("查技能", "").Trim();
                    if (int.TryParse(detail, out int id))
                    {
                        string msg = Controller.GetSkillInfo(uid, id);
                        if (msg != "")
                        {
                            await SendAsync(e, "查技能", msg);
                        }
                    }
                    else
                    {
                        string msg = Controller.GetSkillInfo_Name(uid, detail);
                        if (msg != "")
                        {
                            await SendAsync(e, "查技能", msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("查物品", StringComparison.CurrentCultureIgnoreCase))
                {
                    e.UseNotice = false;
                    string detail = e.Detail.Replace("查物品", "").Trim();
                    if (int.TryParse(detail, out int id))
                    {
                        string msg = Controller.GetItemInfo(uid, id);
                        if (msg != "")
                        {
                            await SendAsync(e, "查物品", msg);
                        }
                    }
                    else
                    {
                        string msg = Controller.GetItemInfo_Name(uid, detail);
                        if (msg != "")
                        {
                            await SendAsync(e, "查物品", msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("生成指定"))
                {
                    e.UseNotice = false;
                    string pattern = @"生成指定(\w+)魔法卡\s*(\d+)(?:(?:\s*(\d+)\s+(\d+)\s+(\d+)(?:\s+(\d+))?)|(?:\s*(\d+)(?!\s*\d)))?(?:\s*给\s*(\d+))?";
                    Regex regex = new(pattern, RegexOptions.IgnoreCase);
                    Match match = regex.Match(e.Detail);

                    if (match.Success && ((match.Groups[3].Success && match.Groups[4].Success && match.Groups[5].Success) || match.Groups[7].Success))
                    {
                        string quality = match.Groups[1].Value;
                        long magicID = long.Parse(match.Groups[2].Value);
                        int str = 0, agi = 0, intelligence = 0, count = 1;
                        long targetUserID = uid;

                        // 检查是否匹配到 str agi intelligence 模式 (Group 3, 4, 5)
                        if (match.Groups[3].Success && match.Groups[4].Success && match.Groups[5].Success)
                        {
                            str = int.Parse(match.Groups[3].Value);
                            agi = int.Parse(match.Groups[4].Value);
                            intelligence = int.Parse(match.Groups[5].Value);

                            // 如果此模式下有 count (Group 6)
                            if (match.Groups[6].Success)
                            {
                                count = int.Parse(match.Groups[6].Value);
                            }
                        }
                        // 否则，检查是否匹配到单独的 count 模式 (Group 7)
                        else if (match.Groups[7].Success)
                        {
                            count = int.Parse(match.Groups[7].Value);
                        }

                        if (count <= 0)
                        {
                            await SendAsync(e, "熟圣之力", "数量不能为 0 或负数，请重新输入。");
                            return result;
                        }

                        if (match.Groups[8].Success)
                        {
                            targetUserID = long.Parse(match.Groups[8].Value);
                        }

                        string msg = Controller.CreateMagicCard(uid, targetUserID, quality, magicID, count, str, agi, intelligence);
                        if (msg != "")
                        {
                            await SendAsync(e, "熟圣之力", msg);
                        }
                    }
                    else
                    {
                        await SendAsync(e, "熟圣之力", "指令格式错误！请检查格式：\r\n1、生成指定<品质>魔法卡 <MagicID> [str agi intelligence] [count] [给 <TargetID>]\r\n" +
                            "2、生成指定<品质>魔法卡 <MagicID> [count] [给 <TargetID>]\r\n注意：[str agi intelligence] 可选块的三个参数都必须完整提供。");
                    }
                    return result;
                }

                if (e.Detail.StartsWith("生成"))
                {
                    e.UseNotice = false;
                    string pattern = @"生成\s*(\d+)\s*个\s*([^给\s]+)(?:\s*给\s*(\d+))?";
                    Regex regex = new(pattern, RegexOptions.IgnoreCase);
                    Match match = regex.Match(e.Detail);

                    if (match.Success)
                    {
                        int count = int.Parse(match.Groups[1].Value);
                        string name = match.Groups[2].Value.Trim();
                        string target = match.Groups[3].Value;
                        long userid = uid;

                        if (!string.IsNullOrEmpty(target))
                        {
                            userid = long.Parse(target);
                        }

                        if (count > 0)
                        {
                            string msg = Controller.CreateItem(uid, name, count, userid);
                            if (msg != "")
                            {
                                await SendAsync(e, "熟圣之力", msg);
                            }
                        }
                        else
                        {
                            await SendAsync(e, "熟圣之力", "数量不能为 0，请重新输入。");
                        }
                        return result;
                    }
                }

                if (e.Detail == "预览魔法卡包")
                {
                    e.UseNotice = false;
                    string msg = Controller.GenerateMagicCardPack();
                    if (msg != "")
                    {
                        await SendAsync(e, "预览魔法卡包", msg);
                    }
                    return result;
                }
                else if (e.Detail == "预览魔法卡")
                {
                    e.UseNotice = false;
                    string msg = Controller.GenerateMagicCard();
                    if (msg != "")
                    {
                        await SendAsync(e, "预览魔法卡", msg);
                    }
                    return result;
                }

                if (e.Detail == "创建存档")
                {
                    e.UseNotice = false;
                    string msg = Controller.CreateSaved(uid, openid);
                    if (msg != "")
                    {
                        await SendAsync(e, "创建存档", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "我的存档")
                {
                    string msg = Controller.ShowSaved(uid);
                    if (msg != "")
                    {
                        await SendAsync(e, "我的存档", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "我的主战")
                {
                    e.UseNotice = false;
                    string msg = Controller.GetCharacterInfoFromInventory(uid, 0);
                    if (msg != "")
                    {
                        await SendAsync(e, "我的主战", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "我的状态")
                {
                    e.UseNotice = false;
                    string msg = Controller.ShowMainCharacterOrSquadStatus(uid);
                    if (msg != "")
                    {
                        await SendAsync(e, "我的状态", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "小队状态" || e.Detail == "我的小队状态")
                {
                    e.UseNotice = false;
                    string msg = Controller.ShowMainCharacterOrSquadStatus(uid, true);
                    if (msg != "")
                    {
                        await SendAsync(e, "我的小队状态", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "我的小队")
                {
                    e.UseNotice = false;
                    string msg = Controller.ShowSquad(uid);
                    if (msg != "")
                    {
                        await SendAsync(e, "我的小队", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "清空小队")
                {
                    string msg = Controller.ClearSquad(uid);
                    if (msg != "")
                    {
                        await SendAsync(e, "清空小队", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "还原存档")
                {
                    e.UseNotice = false;
                    await SendAsync(e, "还原存档", "\r\n请在该指令前添加【确认】二字，即使用【确认还原存档】指令。");
                    return result;
                }

                if (e.Detail == "确认还原存档")
                {
                    string msg = Controller.RestoreSaved(uid);
                    if (msg != "")
                    {
                        await SendAsync(e, "还原存档", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "生成自建角色")
                {
                    string msg = Controller.NewCustomCharacter(uid);
                    if (msg != "")
                    {
                        await SendAsync(e, "生成自建角色", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "角色改名")
                {
                    e.UseNotice = false;
                    await SendAsync(e, "改名", "\r\n为防止玩家手误更改自己的昵称，请在该指令前添加【确认】二字，即使用【确认角色改名】指令。");
                    return result;
                }

                if (e.Detail == "确认角色改名")
                {
                    string msg = Controller.ReName(uid);
                    if (msg != "")
                    {
                        await SendAsync(e, "改名", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "查询改名")
                {
                    string msg = Controller.GetReNameInfo(uid);
                    if (msg != "")
                    {
                        await SendAsync(e, "查询改名", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("改名审核"))
                {
                    string detail = e.Detail.Replace("改名审核", "").Trim();
                    string msg = "";
                    if (int.TryParse(detail, out int page))
                    {
                        msg = Controller.GetReNameExamines(uid, page);
                    }
                    else
                    {
                        msg = Controller.GetReNameExamines(uid);
                    }
                    if (msg != "")
                    {
                        await SendAsync(e, "改名审核", msg);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("改名拒绝"))
                {
                    string detail = e.Detail.Replace("改名拒绝", "").Trim();
                    string msg = "";
                    string[] strings = detail.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (strings.Length > 0)
                    {
                        string reason = "";
                        if (strings.Length > 1)
                        {
                            reason = string.Join(" ", strings[1..]).Trim();
                        }
                        if (long.TryParse(strings[0], out long target))
                        {
                            msg = Controller.ApproveReName(uid, target, false, false, reason);
                        }
                        if (msg != "")
                        {
                            await SendAsync(e, "改名拒绝", msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("改名批准"))
                {
                    string detail = e.Detail.Replace("改名批准", "").Trim();
                    string msg = "";
                    if (long.TryParse(detail, out long target))
                    {
                        msg = Controller.ApproveReName(uid, target);
                    }
                    if (msg != "")
                    {
                        await SendAsync(e, "改名批准", msg);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("社团改名拒绝"))
                {
                    string detail = e.Detail.Replace("社团改名拒绝", "").Trim();
                    string msg = "";
                    string[] strings = detail.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (strings.Length > 0)
                    {
                        string reason = "";
                        if (strings.Length > 1)
                        {
                            reason = string.Join(" ", strings[1..]).Trim();
                        }
                        if (long.TryParse(strings[0], out long target))
                        {
                            msg = Controller.ApproveReName(uid, target, false, true, reason);
                        }
                        if (msg != "")
                        {
                            await SendAsync(e, "社团改名拒绝", msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("社团改名批准"))
                {
                    string detail = e.Detail.Replace("社团改名批准", "").Trim();
                    string msg = "";
                    if (long.TryParse(detail, out long target))
                    {
                        msg = Controller.ApproveReName(uid, target, isClub: true);
                    }
                    if (msg != "")
                    {
                        await SendAsync(e, "社团改名批准", msg);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("自定义改名"))
                {
                    e.UseNotice = false;
                    await SendAsync(e, "改名", "\r\n自定义改名说明：自定义改名需要库存中存在至少一张未上锁、且未处于交易、市场出售状态的改名卡，提交改名申请后需要等待审核。" +
                        "在审核期间，改名卡将会被系统锁定，无法取消、重复提交申请，也不能解锁、分解、交易、出售该改名卡。如已知悉请在该指令前添加【确认】二字，即使用【确认自定义改名】指令。");
                    return result;
                }

                if (e.Detail.StartsWith("确认自定义改名"))
                {
                    e.UseNotice = false;
                    string detail = e.Detail.Replace("确认自定义改名", "").Trim();
                    string msg = Controller.ReName_Custom(uid, detail);
                    if (msg != "")
                    {
                        await SendAsync(e, "改名", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "角色重随")
                {
                    string msg = Controller.RandomCustomCharacter(uid, false);
                    if (msg != "")
                    {
                        await SendAsync(e, "角色重随", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "确认角色重随")
                {
                    string msg = Controller.RandomCustomCharacter(uid, true);
                    if (msg != "")
                    {
                        await SendAsync(e, "角色重随", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "取消角色重随")
                {
                    string msg = Controller.CancelRandomCustomCharacter(uid);
                    if (msg != "")
                    {
                        await SendAsync(e, "角色重随", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "抽卡")
                {
                    List<string> msgs = Controller.DrawCard(uid);
                    if (msgs.Count > 0)
                    {
                        await SendAsync(e, "抽卡", "\r\n" + string.Join("\r\n", msgs));
                    }
                    return result;
                }

                if (e.Detail == "十连抽卡")
                {
                    List<string> msgs = Controller.DrawCards(uid);
                    if (msgs.Count > 0)
                    {
                        await SendAsync(e, "十连抽卡", "\r\n" + string.Join("\r\n", msgs));
                    }
                    return result;
                }

                if (e.Detail == "钻石抽卡")
                {
                    List<string> msgs = Controller.DrawCard_Material(uid);
                    if (msgs.Count > 0)
                    {
                        await SendAsync(e, "钻石抽卡", "\r\n" + string.Join("\r\n", msgs));
                    }
                    return result;
                }

                if (e.Detail == "钻石十连抽卡")
                {
                    List<string> msgs = Controller.DrawCards_Material(uid);
                    if (msgs.Count > 0)
                    {
                        await SendAsync(e, "钻石十连抽卡", "\r\n" + string.Join("\r\n", msgs));
                    }
                    return result;
                }

                if (e.Detail.StartsWith("查看库存") || e.Detail.StartsWith("我的库存") || e.Detail.StartsWith("我的背包"))
                {
                    string detail = e.Detail.Replace("查看库存", "").Replace("我的库存", "").Replace("我的背包", "").Trim();
                    List<string> msgs = [];
                    if (int.TryParse(detail, out int page))
                    {
                        msgs = Controller.GetInventoryInfo2(uid, page);
                    }
                    else if (FunGameItemType.FirstOrDefault(detail.Contains) is string matchedType)
                    {
                        int typeIndex = FunGameItemType.IndexOf(matchedType);
                        string remain = detail.Replace(matchedType, "").Trim();
                        if (int.TryParse(remain, out page))
                        {
                            msgs = Controller.GetInventoryInfo4(uid, page, typeIndex);
                        }
                        else
                        {
                            msgs = Controller.GetInventoryInfo4(uid, 1, typeIndex);
                        }
                    }
                    else
                    {
                        msgs = Controller.GetInventoryInfo2(uid, 1);
                    }
                    if (msgs.Count > 0)
                    {
                        await SendAsync(e, "查看库存", "\r\n" + string.Join("\r\n", msgs));
                    }
                    return result;
                }

                if (e.Detail.StartsWith("物品库存"))
                {
                    string detail = e.Detail.Replace("物品库存", "").Trim();
                    List<string> msgs = [];
                    if (int.TryParse(detail, out int page))
                    {
                        msgs = Controller.GetInventoryInfo3(uid, page, 2, 2);
                    }
                    else
                    {
                        msgs = Controller.GetInventoryInfo3(uid, 1, 2, 2);
                    }
                    if (msgs.Count > 0)
                    {
                        await SendAsync(e, "查看分类库存", "\r\n" + string.Join("\r\n", msgs));
                    }
                    return result;
                }

                if (e.Detail.StartsWith("角色库存"))
                {
                    string detail = e.Detail.Replace("角色库存", "").Trim();
                    List<string> msgs = [];
                    if (int.TryParse(detail, out int page))
                    {
                        msgs = Controller.GetInventoryInfo5(uid, page);
                    }
                    else
                    {
                        msgs = Controller.GetInventoryInfo5(uid, 1);
                    }
                    if (msgs.Count > 0)
                    {
                        await SendAsync(e, "查看角色库存", "\r\n" + string.Join("\r\n", msgs));
                    }
                    return result;
                }

                if (e.Detail.StartsWith("分类库存"))
                {
                    string detail = e.Detail.Replace("分类库存", "").Trim();
                    string[] strings = detail.Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    int t = -1;
                    if (strings.Length > 0 && int.TryParse(strings[0].Trim(), out t))
                    {
                        List<string> msgs = [];
                        if (strings.Length > 1 && int.TryParse(strings[1].Trim(), out int page))
                        {
                            msgs = Controller.GetInventoryInfo4(uid, page, t);
                        }
                        else
                        {
                            msgs = Controller.GetInventoryInfo4(uid, 1, t);
                        }
                        if (msgs.Count > 0)
                        {
                            await SendAsync(e, "查看分类库存", "\r\n" + string.Join("\r\n", msgs));
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("库存搜索2", StringComparison.CurrentCultureIgnoreCase) || e.Detail.StartsWith("库存查询2", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("库存搜索2", "").Replace("库存查询2", "").Trim();
                    string[] strings = detail.Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    string search = strings[0];
                    int page = 1;
                    if (strings.Length > 1 && int.TryParse(strings[1], out int p))
                    {
                        page = p;
                    }
                    List<string> msgs = Controller.GetInventoryInfo6(uid, page, search, false);
                    if (msgs.Count > 0)
                    {
                        await SendAsync(e, "搜索库存物品（带描述）", "\r\n" + string.Join("\r\n", msgs));
                    }
                    return result;
                }

                if (e.Detail.StartsWith("库存搜索", StringComparison.CurrentCultureIgnoreCase) || e.Detail.StartsWith("库存查询", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("库存搜索", "").Replace("库存查询", "").Trim();
                    string[] strings = detail.Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    string search = strings[0];
                    int page = 1;
                    if (strings.Length > 1 && int.TryParse(strings[1], out int p))
                    {
                        page = p;
                    }
                    List<string> msgs = Controller.GetInventoryInfo6(uid, page, search, true);
                    if (msgs.Count > 0)
                    {
                        await SendAsync(e, "搜索库存物品", "\r\n" + string.Join("\r\n", msgs));
                    }
                    return result;
                }

                if (e.Detail.StartsWith("我角色", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("我角色", "").Trim();
                    string msg = "";
                    if (int.TryParse(detail, out int seq))
                    {
                        msg = Controller.GetCharacterInfoFromInventory(uid, seq, true);
                    }
                    else
                    {
                        msg = Controller.GetCharacterInfoFromInventory(uid, 1, true);
                    }
                    if (msg != "")
                    {
                        await SendAsync(e, "查库存角色", msg);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("我的角色", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("我的角色", "").Trim();
                    string msg = "";
                    if (int.TryParse(detail, out int seq))
                    {
                        msg = Controller.GetCharacterInfoFromInventory(uid, seq, false);
                    }
                    else
                    {
                        msg = Controller.GetCharacterInfoFromInventory(uid, 1, false);
                    }
                    if (msg != "")
                    {
                        await SendAsync(e, "查库存角色", msg);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("角色技能", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("角色技能", "").Trim();
                    string msg = "";
                    if (int.TryParse(detail, out int seq))
                    {
                        msg = Controller.GetCharacterSkills(uid, seq);
                    }
                    else
                    {
                        msg = Controller.GetCharacterSkills(uid, 1);
                    }
                    if (msg != "")
                    {
                        await SendAsync(e, "角色技能", msg);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("角色物品", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("角色物品", "").Trim();
                    string msg = "";
                    if (int.TryParse(detail, out int seq))
                    {
                        msg = Controller.GetCharacterItems(uid, seq);
                    }
                    else
                    {
                        msg = Controller.GetCharacterItems(uid, 1);
                    }
                    if (msg != "")
                    {
                        await SendAsync(e, "角色物品", msg);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("设置主战", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("设置主战", "").Trim();
                    string msg = "";
                    if (int.TryParse(detail, out int cid))
                    {
                        msg = Controller.SetMain(uid, cid);
                    }
                    else
                    {
                        msg = Controller.SetMain(uid, 1);
                    }
                    if (msg != "")
                    {
                        await SendAsync(e, "设置主战角色", msg);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("开启练级", StringComparison.CurrentCultureIgnoreCase) || e.Detail.StartsWith("开始练级", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("开启练级", "").Replace("开始练级", "").Trim();
                    string msg = "";
                    if (int.TryParse(detail, out int cid))
                    {
                        msg = Controller.StartTraining(uid, cid);
                    }
                    else
                    {
                        msg = Controller.StartTraining(uid, 1);
                    }
                    if (msg != "")
                    {
                        await SendAsync(e, "开启练级", msg);
                    }
                    return result;
                }

                if (e.Detail == "练级信息")
                {
                    string msg = Controller.GetTrainingInfo(uid);
                    if (msg != "")
                    {
                        await SendAsync(e, "练级信息", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "练级结算")
                {
                    string msg = Controller.StopTraining(uid);
                    if (msg != "")
                    {
                        await SendAsync(e, "练级结算", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "任务列表")
                {
                    string msg = Controller.CheckQuestList(uid);
                    if (msg != "")
                    {
                        await SendAsync(e, "任务列表", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "任务信息")
                {
                    string msg = Controller.CheckWorkingQuest(uid);
                    if (msg != "")
                    {
                        await SendAsync(e, "任务信息", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "任务结算")
                {
                    string msg = Controller.SettleQuest(uid);
                    if (msg != "")
                    {
                        await SendAsync(e, "任务结算", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "签到")
                {
                    string msg = Controller.SignIn(uid);
                    if (msg != "")
                    {
                        await SendAsync(e, "签到", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("开始任务", StringComparison.CurrentCultureIgnoreCase) || e.Detail.StartsWith("做任务", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("开始任务", "").Replace("做任务", "").Trim();
                    if (int.TryParse(detail, out int index))
                    {
                        List<string> msgs = Controller.AcceptQuest(uid, index);
                        await SendAsync(e, "开始任务", string.Join("\r\n", msgs));
                    }
                    return result;
                }

                if (e.Detail.StartsWith("我的物品", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("我的物品", "").Trim();
                    string msg;
                    if (int.TryParse(detail, out int index))
                    {
                        msg = Controller.GetItemInfoFromInventory(uid, index);
                        if (msg != "")
                        {
                            await SendAsync(e, "查库存物品", msg);
                            return result;
                        }
                    }
                    msg = Controller.GetItemInfoFromInventory_Name(uid, detail);
                    if (msg != "")
                    {
                        await SendAsync(e, "查库存物品", msg);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("兑换金币", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("兑换金币", "").Trim();
                    if (int.TryParse(detail, out int materials))
                    {
                        string msg = Controller.ExchangeCredits(uid, materials);
                        if (msg != "")
                        {
                            await SendAsync(e, "兑换金币", msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("取消装备", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("取消装备", "").Trim();
                    string[] strings = detail.Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    int c = -1, i = -1;
                    if (strings.Length > 0 && int.TryParse(strings[0].Trim(), out c) && strings.Length > 1 && int.TryParse(strings[1].Trim(), out i))
                    {
                        if (c != -1 && i != -1)
                        {
                            string msg = Controller.UnEquipItem(uid, c, i);
                            if (msg != "")
                            {
                                await SendAsync(e, "取消装备", msg);
                            }
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("装备", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("装备", "").Trim();
                    string[] strings = detail.Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    int c = -1, i = -1;
                    if (strings.Length > 0 && int.TryParse(strings[0].Trim(), out c) && strings.Length > 1 && int.TryParse(strings[1].Trim(), out i))
                    {
                        if (c != -1 && i != -1)
                        {
                            string msg = Controller.EquipItem(uid, c, i);
                            if (msg != "")
                            {
                                await SendAsync(e, "装备", msg);
                            }
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("查看技能升级", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("查看技能升级", "").Trim();
                    string[] strings = detail.Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    int c = -1;
                    if (strings.Length > 0 && int.TryParse(strings[0].Trim(), out c) && strings.Length > 1)
                    {
                        string s = strings[1].Trim();
                        if (c != -1 && s != "")
                        {
                            string msg = Controller.GetSkillLevelUpNeedy(uid, c, s);
                            if (msg != "")
                            {
                                await SendAsync(e, "查看技能升级", msg);
                            }
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("技能升级", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("技能升级", "").Trim();
                    string[] strings = detail.Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    int c = -1;
                    if (strings.Length > 0 && int.TryParse(strings[0].Trim(), out c) && strings.Length > 1)
                    {
                        string s = strings[1].Trim();
                        if (c != -1 && s != "")
                        {
                            string msg = Controller.SkillLevelUp(uid, c, s);
                            if (msg != "")
                            {
                                await SendAsync(e, "技能升级", msg);
                            }
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("合成魔法卡", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("合成魔法卡", "").Trim();
                    string[] strings = detail.Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    int id1 = -1, id2 = -1, id3 = -1;
                    if (strings.Length > 0 && int.TryParse(strings[0].Trim(), out id1) && strings.Length > 1 && int.TryParse(strings[1].Trim(), out id2) && strings.Length > 2 && int.TryParse(strings[2].Trim(), out id3))
                    {
                        if (id1 != -1 && id2 != -1 && id3 != -1)
                        {
                            string msg = Controller.ConflateMagicCardPack(uid, [id1, id2, id3]);
                            if (msg != "")
                            {
                                await SendAsync(e, "合成魔法卡", msg);
                            }
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("角色升级", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("角色升级", "").Trim();
                    string msg = "";
                    if (int.TryParse(detail, out int cid))
                    {
                        msg = Controller.CharacterLevelUp(uid, cid);
                    }
                    else
                    {
                        msg = Controller.CharacterLevelUp(uid, 1);
                    }
                    if (msg != "")
                    {
                        await SendAsync(e, "角色升级", msg);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("查看普攻升级", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("查看普攻升级", "").Trim();
                    string msg = "";
                    if (int.TryParse(detail, out int cid))
                    {
                        msg = Controller.GetNormalAttackLevelUpNeedy(uid, cid);
                    }
                    else
                    {
                        msg = Controller.GetNormalAttackLevelUpNeedy(uid, 1);
                    }
                    if (msg != "")
                    {
                        await SendAsync(e, "查看普攻升级", msg);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("普攻升级", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("普攻升级", "").Trim();
                    string msg = "";
                    if (int.TryParse(detail, out int cid))
                    {
                        msg = Controller.NormalAttackLevelUp(uid, cid);
                    }
                    else
                    {
                        msg = Controller.NormalAttackLevelUp(uid, 1);
                    }
                    if (msg != "")
                    {
                        await SendAsync(e, "普攻升级", msg);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("角色突破", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("角色突破", "").Trim();
                    string msg = "";
                    if (int.TryParse(detail, out int cid))
                    {
                        msg = Controller.CharacterLevelBreak(uid, cid);
                    }
                    else
                    {
                        msg = Controller.CharacterLevelBreak(uid, 1);
                    }
                    if (msg != "")
                    {
                        await SendAsync(e, "角色突破", msg);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("突破信息", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("突破信息", "").Trim();
                    string msg = "";
                    if (int.TryParse(detail, out int cid))
                    {
                        msg = Controller.GetLevelBreakNeedy(uid, cid);
                    }
                    else
                    {
                        msg = Controller.GetLevelBreakNeedy(uid, 1);
                    }
                    if (msg != "")
                    {
                        await SendAsync(e, "突破信息", msg);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("使用") || e.Detail.StartsWith("批量使用"))
                {
                    if (e.Detail.StartsWith("批量使用"))
                    {
                        string detail = e.Detail.Replace("批量使用", "").Trim();
                        string pattern = @"\s*(?:角色\s*(?<characterId>\d+))?\s*(?<itemIds>[\d\s,，;；]+)";
                        Match match = Regex.Match(detail, pattern);
                        if (match.Success)
                        {
                            string itemIdsString = match.Groups["itemIds"].Value;
                            string characterId = match.Groups["characterId"].Value;
                            int[] characterIds = characterId != "" ? [int.Parse(characterId)] : [1];
                            int[] itemIds = [.. itemIdsString.Split(FunGameConstant.SplitChars, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(int.Parse)];
                            if (itemIds.Length > 0)
                            {
                                string msg = Controller.UseItem4(uid, (itemIds, characterIds));
                                if (msg != "")
                                {
                                    await SendAsync(e, "批量使用", msg);
                                }
                            }
                        }
                    }
                    else if (e.Detail.StartsWith("使用魔法卡"))
                    {
                        string detail = e.Detail.Replace("使用", "").Trim();
                        string pattern = @"\s*魔法卡\s*(?<itemId>\d+)(?:\s+(?:(?:角色\s*(?<characterId>\d+))|(?<packageId>\d+)))?";
                        Match match = Regex.Match(detail, pattern);
                        if (match.Success)
                        {
                            string itemId = match.Groups["itemId"].Value;
                            string characterId = match.Groups["characterId"].Value;
                            bool isCharacter = match.Groups["characterId"].Success;
                            string packageId = match.Groups["packageId"].Value;
                            if (int.TryParse(itemId, out int id) && id > 0)
                            {
                                int targetId = 0;
                                if (isCharacter && int.TryParse(characterId, out int charId) && charId > 0)
                                {
                                    targetId = charId;
                                }
                                else if (!isCharacter && int.TryParse(packageId, out int pkgId) && pkgId > 0)
                                {
                                    targetId = pkgId;
                                    isCharacter = false;
                                }
                                if (targetId > 0)
                                {
                                    string msg = Controller.UseItem3(uid, id, targetId, isCharacter);
                                    if (msg != "")
                                    {
                                        await SendAsync(e, "使用魔法卡", msg);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        string detail = e.Detail.Replace("使用", "").Trim();
                        string pattern = @"^\s*(?:(?<itemId>\d+)|(?<itemPart>[^\d\s].*?))(?:\s+(?<countPart>\d+))?(?:\s*角色\s*(?<characterIds>[\d\s,，;；]*))?$";
                        Match match = Regex.Match(detail, pattern);
                        if (match.Success)
                        {
                            string itemId = match.Groups["itemId"].Value;
                            string itemPart = match.Groups["itemPart"].Value.Trim();
                            string countStr = match.Groups["countPart"].Value;
                            string characterIdsString = match.Groups["characterIds"].Value;
                            int[] characterIds = characterIdsString != "" ? [.. characterIdsString.Split(FunGameConstant.SplitChars, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(int.Parse)] : [1];
                            int count = string.IsNullOrEmpty(countStr) ? 1 : int.Parse(countStr);

                            if (!string.IsNullOrEmpty(itemId) && int.TryParse(itemId, out int id))
                            {
                                string msg = Controller.UseItem(uid, id, count, characterIds);
                                if (!string.IsNullOrEmpty(msg))
                                {
                                    await SendAsync(e, "使用", msg);
                                }
                            }
                            else if (!string.IsNullOrEmpty(itemPart))
                            {
                                string msg = Controller.UseItem2(uid, itemPart, count, characterIds);
                                if (msg != "")
                                {
                                    await SendAsync(e, "使用", msg);
                                }
                            }
                        }
                    }

                    return result;
                }

                if (e.Detail.StartsWith("分解物品", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("分解物品", "").Trim();
                    List<int> ids = [];
                    foreach (string str in detail.Split(FunGameConstant.SplitChars, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (int.TryParse(str, out int id))
                        {
                            ids.Add(id);
                        }
                    }
                    string msg = Controller.DecomposeItem(uid, [.. ids]);
                    if (msg != "")
                    {
                        await SendAsync(e, "分解物品", msg);
                    }

                    return result;
                }

                if (e.Detail.StartsWith("强制分解", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("强制分解", "").Trim();
                    string pattern = @"\s*(?<itemName>[^\d]+)\s*(?<count>\d+)\s*";
                    Match match = Regex.Match(detail, pattern);
                    if (match.Success)
                    {
                        string itemName = match.Groups["itemName"].Value.Trim();
                        if (int.TryParse(match.Groups["count"].Value, out int count))
                        {
                            string msg = Controller.DecomposeItem2(uid, itemName, count, true);
                            if (msg != "")
                            {
                                await SendAsync(e, "分解", msg);
                            }
                        }
                    }

                    return result;
                }

                if (e.Detail.StartsWith("分解", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("分解", "").Trim();
                    string pattern = @"\s*(?<itemName>[^\d]+)\s*(?<count>\d+)\s*";
                    Match match = Regex.Match(detail, pattern);
                    if (match.Success)
                    {
                        string itemName = match.Groups["itemName"].Value.Trim();
                        if (int.TryParse(match.Groups["count"].Value, out int count))
                        {
                            string msg = Controller.DecomposeItem2(uid, itemName, count);
                            if (msg != "")
                            {
                                await SendAsync(e, "分解", msg);
                            }
                        }
                    }

                    return result;
                }

                if (e.Detail.StartsWith("品质分解", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("品质分解", "").Trim();
                    if (int.TryParse(detail, out int q))
                    {
                        string msg = Controller.DecomposeItem3(uid, q);
                        if (msg != "")
                        {
                            await SendAsync(e, "品质分解", msg);
                        }
                    }

                    return result;
                }

                if (e.Detail.StartsWith("熟圣之力", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("熟圣之力", "").Trim();
                    string[] strings = detail.Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    int count = -1;
                    if (strings.Length > 1 && int.TryParse(strings[1].Trim(), out count))
                    {
                        string name = strings[0].Trim();
                        if (count > 0)
                        {
                            long userid = uid;
                            if (strings.Length > 2 && long.TryParse(strings[2].Replace("@", "").Trim(), out long temp))
                            {
                                userid = temp;
                            }
                            string msg = Controller.CreateItem(uid, name, count, userid);
                            if (msg != "")
                            {
                                await SendAsync(e, "熟圣之力", msg);
                            }
                        }
                        else
                        {
                            await SendAsync(e, "熟圣之力", "数量不能为0，请重新输入。");
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("完整决斗", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("完整决斗", "").Replace("@", "").Trim();
                    List<string> msgs = [];
                    if (long.TryParse(detail.Trim(), out long eqq))
                    {
                        msgs = await Controller.FightCustom(uid, eqq, true);
                    }
                    else
                    {
                        msgs = await Controller.FightCustom2(uid, detail.Trim(), true);
                    }
                    List<string> real = MergeMessages(msgs);
                    int count = 1;
                    foreach (string msg in real)
                    {
                        await SendAsync(e, "完整决斗", msg.Trim(), msgSeq: count++);
                        if (count != real.Count) await Task.Delay(1500);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("决斗", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("决斗", "").Replace("@", "").Trim();
                    List<string> msgs = [];
                    if (long.TryParse(detail.Trim(), out long eqq))
                    {
                        msgs = await Controller.FightCustom(uid, eqq, false);
                    }
                    else
                    {
                        msgs = await Controller.FightCustom2(uid, detail.Trim(), false);
                    }
                    List<string> real = MergeMessages(msgs);
                    int count = 1;
                    foreach (string msg in real)
                    {
                        await SendAsync(e, "决斗", msg.Trim(), msgSeq: count++);
                        if (count != real.Count) await Task.Delay(1500);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("小队决斗", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("小队决斗", "").Replace("@", "").Trim();
                    List<string> msgs = [];
                    if (long.TryParse(detail.Trim(), out long eqq))
                    {
                        msgs = await Controller.FightCustomTeam(uid, eqq, true);
                    }
                    else
                    {
                        msgs = await Controller.FightCustomTeam2(uid, detail.Trim(), true);
                    }
                    List<string> real = MergeMessages(msgs);
                    int count = 1;
                    foreach (string msg in real)
                    {
                        await SendAsync(e, "完整决斗", msg.Trim(), msgSeq: count++);
                        if (count != real.Count) await Task.Delay(1500);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("查询boss", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("查询boss", "").Trim();
                    List<string> msgs = [];
                    if (int.TryParse(detail, out int cid))
                    {
                        msgs = Controller.GetBoss(cid);
                    }
                    else
                    {
                        msgs = Controller.GetBoss();
                    }
                    if (msgs.Count > 0)
                    {
                        await SendAsync(e, "BOSS", string.Join("\r\n", msgs));
                    }
                    return result;
                }

                if (e.Detail.StartsWith("小队讨伐boss", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("小队讨伐boss", "").Trim();
                    List<string> msgs = [];
                    if (int.TryParse(detail.Trim(), out int index))
                    {
                        msgs = await Controller.FightBossTeam(uid, index, true);
                        List<string> real = MergeMessages(msgs);
                        int count = 1;
                        foreach (string msg in real)
                        {
                            await SendAsync(e, "BOSS", msg.Trim(), msgSeq: count++);
                            if (count != real.Count) await Task.Delay(1500);
                        }
                    }
                    else
                    {
                        await SendAsync(e, "BOSS", "请输入正确的编号！");
                    }
                    return result;
                }

                if (e.Detail.StartsWith("讨伐boss", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("讨伐boss", "").Trim();
                    List<string> msgs = [];
                    if (int.TryParse(detail.Trim(), out int index))
                    {
                        msgs = await Controller.FightBoss(uid, index, true);
                        List<string> real = MergeMessages(msgs);
                        int count = 1;
                        foreach (string msg in real)
                        {
                            await SendAsync(e, "BOSS", msg.Trim(), msgSeq: count++);
                            if (count != real.Count) await Task.Delay(1500);
                        }
                    }
                    else
                    {
                        await SendAsync(e, "BOSS", "请输入正确的编号！");
                    }
                    return result;
                }

                if (e.Detail.StartsWith("小队添加", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("小队添加", "").Trim();
                    if (int.TryParse(detail, out int c))
                    {
                        string msg = Controller.AddSquad(uid, c);
                        if (msg != "")
                        {
                            await SendAsync(e, "小队", msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("小队移除", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("小队移除", "").Trim();
                    if (int.TryParse(detail, out int c))
                    {
                        string msg = Controller.RemoveSquad(uid, c);
                        if (msg != "")
                        {
                            await SendAsync(e, "小队", msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("设置小队") || e.Detail.StartsWith("重组小队"))
                {
                    string detail = e.Detail.Replace("设置小队", "").Replace("重组小队", "").Trim();
                    string[] strings = detail.Split(FunGameConstant.SplitChars, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    List<int> cindexs = [];
                    foreach (string s in strings)
                    {
                        if (int.TryParse(s, out int c))
                        {
                            cindexs.Add(c);
                        }
                    }
                    string msg = Controller.SetSquad(uid, [.. cindexs]);
                    if (msg != "")
                    {
                        await SendAsync(e, "小队", msg);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("加入社团", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("加入社团", "").Trim();
                    if (int.TryParse(detail, out int c))
                    {
                        string msg = Controller.ClubJoin(uid, c);
                        if (msg != "")
                        {
                            await SendAsync(e, "社团", msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("邀请加入", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("邀请加入", "").Trim();
                    if (int.TryParse(detail, out int id))
                    {
                        string msg = Controller.ClubInvite(uid, id);
                        if (msg != "")
                        {
                            await SendAsync(e, "社团", msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("取消邀请加入", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("取消邀请加入", "").Trim();
                    if (int.TryParse(detail, out int id))
                    {
                        string msg = Controller.ClubInvite(uid, id, true);
                        if (msg != "")
                        {
                            await SendAsync(e, "社团", msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("创建社团", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("创建社团", "").Trim();
                    bool isPublic = true;
                    if (detail.Contains("私密"))
                    {
                        isPublic = false;
                    }
                    detail = detail.Replace("私密", "").Trim();
                    string msg = Controller.ClubCreate(uid, isPublic, detail);
                    if (msg != "")
                    {
                        await SendAsync(e, "社团", msg);
                    }
                    return result;
                }

                if (e.Detail == "退出社团")
                {
                    string msg = Controller.ClubQuit(uid);
                    if (msg != "")
                    {
                        await SendAsync(e, "社团", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "我的社团")
                {
                    string msg = Controller.ClubShowInfo(uid);
                    if (msg != "")
                    {
                        await SendAsync(e, "社团", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("社团列表"))
                {
                    string detail = e.Detail.Replace("社团列表", "").Trim();
                    string msg = "";
                    if (int.TryParse(detail, out int page))
                    {
                        msg = Controller.ClubShowList(uid, page);
                    }
                    else
                    {
                        msg = Controller.ClubShowList(uid, 1);
                    }
                    if (msg.Trim() != "")
                    {
                        await SendAsync(e, "社团", msg);
                    }
                    return result;
                }

                if (e.Detail == "解散社团")
                {
                    string msg = Controller.ClubDisband(uid);
                    if (msg != "")
                    {
                        await SendAsync(e, "社团", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("查看社团成员"))
                {
                    string detail = e.Detail.Replace("查看社团成员", "").Trim();
                    if (int.TryParse(detail, out int page) && page > 0)
                    {
                        string msg = Controller.ClubShowMemberList(uid, 0, page);
                        if (msg != "")
                        {
                            await SendAsync(e, "社团", "\r\n" + msg);
                        }
                    }
                    else
                    {
                        string msg = Controller.ClubShowMemberList(uid, 0, 1);
                        if (msg != "")
                        {
                            await SendAsync(e, "社团", "\r\n" + msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("查看社团管理"))
                {
                    string detail = e.Detail.Replace("查看社团管理", "").Trim();
                    if (int.TryParse(detail, out int page) && page > 0)
                    {
                        string msg = Controller.ClubShowMemberList(uid, 1, page);
                        if (msg != "")
                        {
                            await SendAsync(e, "社团", "\r\n" + msg);
                        }
                    }
                    else
                    {
                        string msg = Controller.ClubShowMemberList(uid, 1, 1);
                        if (msg != "")
                        {
                            await SendAsync(e, "社团", "\r\n" + msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("查看申请人列表"))
                {
                    string detail = e.Detail.Replace("查看申请人列表", "").Trim();
                    if (int.TryParse(detail, out int page) && page > 0)
                    {
                        string msg = Controller.ClubShowMemberList(uid, 2, page);
                        if (msg != "")
                        {
                            await SendAsync(e, "社团", "\r\n" + msg);
                        }
                    }
                    else
                    {
                        string msg = Controller.ClubShowMemberList(uid, 2, 1);
                        if (msg != "")
                        {
                            await SendAsync(e, "社团", "\r\n" + msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("查看受邀人列表"))
                {
                    string detail = e.Detail.Replace("查看受邀人列表", "").Trim();
                    if (int.TryParse(detail, out int page) && page > 0)
                    {
                        string msg = Controller.ClubShowMemberList(uid, 3, page);
                        if (msg != "")
                        {
                            await SendAsync(e, "社团", "\r\n" + msg);
                        }
                    }
                    else
                    {
                        string msg = Controller.ClubShowMemberList(uid, 3, 1);
                        if (msg != "")
                        {
                            await SendAsync(e, "社团", "\r\n" + msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("社团批准", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("社团批准", "").Replace("@", "").Trim();
                    if (long.TryParse(detail, out long id))
                    {
                        string msg = Controller.ClubApprove(uid, id, true);
                        if (msg != "")
                        {
                            await SendAsync(e, "社团", msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("社团拒绝", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("社团拒绝", "").Replace("@", "").Trim();
                    if (long.TryParse(detail, out long id))
                    {
                        string msg = Controller.ClubApprove(uid, id, false);
                        if (msg != "")
                        {
                            await SendAsync(e, "社团", msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("社团踢出", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("社团踢出", "").Replace("@", "").Trim();
                    if (long.TryParse(detail, out long id))
                    {
                        string msg = Controller.ClubKick(uid, id);
                        if (msg != "")
                        {
                            await SendAsync(e, "社团", msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("社团设置", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("社团设置", "").Trim();
                    string[] strings = detail.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    if (strings.Length > 0)
                    {
                        string part = strings[0].Trim() switch
                        {
                            "名称" => "name",
                            "前缀" => "prefix",
                            "描述" => "description",
                            "批准" => "isneedapproval",
                            "公开" => "ispublic",
                            "管理" => "setadmin",
                            "取消管理" => "setnotadmin",
                            "新社长" => "setmaster",
                            _ => "",
                        };
                        List<string> args = [];
                        if (strings.Length > 1)
                        {
                            args = [.. strings[1..]];
                        }
                        string msg = Controller.ClubChange(uid, part, [.. args]);
                        if (msg != "")
                        {
                            await SendAsync(e, "社团", msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("社团转让", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("社团转让", "").Replace("@", "").Trim();
                    List<string> args = [detail];
                    string msg = Controller.ClubChange(uid, "setmaster", [.. args]);
                    if (msg != "")
                    {
                        await SendAsync(e, "社团", msg);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("社团捐献"))
                {
                    string detail = e.Detail.Replace("社团捐献", "").Trim();
                    string msg = "";
                    if (double.TryParse(detail, out double credits))
                    {
                        msg = Controller.ClubContribution(uid, credits);
                    }
                    if (msg.Trim() != "")
                    {
                        await SendAsync(e, "社团", msg);
                    }
                    return result;
                }

                if (e.Detail == "每日商店")
                {
                    string msg = Controller.ShowDailyStore(uid);
                    if (msg != "")
                    {
                        await SendAsync(e, "商店", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("商店购买", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (!FunGameConstant.UserLastVisitStore.TryGetValue(uid, out LastStoreModel? model) || model is null || (DateTime.Now - model.LastTime).TotalMinutes > 2)
                    {
                        await SendAsync(e, "商店购买", "为防止错误购买，请先打开一个商店，随后在 2 分钟内进行购买操作。");
                        return result;
                    }
                    string detail = e.Detail.Replace("商店购买", "").Trim();
                    string[] strings = detail.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    if (strings.Length > 0 && int.TryParse(strings[0].Trim(), out int id))
                    {
                        int count = 1;
                        if (strings.Length > 1 && int.TryParse(strings[1].Trim(), out int temp))
                        {
                            count = temp;
                        }
                        string msg = "";
                        if (model.IsDaily)
                        {
                            msg = Controller.DailyStoreBuy(uid, id, count);
                        }
                        else
                        {
                            msg = Controller.SystemStoreBuy(uid, model.StoreRegion, model.StoreName, id, count);
                        }
                        if (msg != "")
                        {
                            await SendAsync(e, "商店购买", msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("商店查看", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (!FunGameConstant.UserLastVisitStore.TryGetValue(uid, out LastStoreModel? model) || model is null || (DateTime.Now - model.LastTime).TotalMinutes > 2)
                    {
                        await SendAsync(e, "商店购买", "请先打开一个商店，随后在 2 分钟内进行查看操作。");
                        return result;
                    }
                    string detail = e.Detail.Replace("商店查看", "").Trim();
                    if (int.TryParse(detail, out int id))
                    {
                        string msg = "";
                        if (model.IsDaily)
                        {
                            msg = Controller.DailyStoreShowInfo(uid, id);
                        }
                        else
                        {
                            msg = Controller.SystemStoreShowInfo(uid, model.StoreRegion, model.StoreName, id);
                        }
                        if (msg != "")
                        {
                            await SendAsync(e, "商店", msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("查地区", StringComparison.CurrentCultureIgnoreCase) || e.Detail.StartsWith("查询地区", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("查地区", "").Replace("查询地区", "").Trim();
                    List<string> msgs = [];
                    if (int.TryParse(detail, out int cid))
                    {
                        if (cid > 0 && cid <= 12)
                        {
                            msgs = Controller.GetRegion(cid);
                        }
                        else if (cid == 0 || cid > 12)
                        {
                            msgs = Controller.GetPlayerRegion(cid);
                        }
                    }
                    else
                    {
                        msgs = Controller.GetRegion();
                    }
                    if (msgs.Count > 0)
                    {
                        await SendAsync(e, "查地区", string.Join("\r\n", msgs));
                    }
                    return result;
                }

                if (e.Detail == "主城" || e.Detail == "铎京")
                {
                    List<string> msgs = Controller.GetPlayerRegion();
                    if (msgs.Count > 0)
                    {
                        await SendAsync(e, "铎京", string.Join("\r\n", msgs));
                    }
                    return result;
                }

                if (e.Detail == "世界地图")
                {
                    List<string> msgs = Controller.GetRegion();
                    if (msgs.Count > 0)
                    {
                        await SendAsync(e, "世界地图", string.Join("\r\n", msgs));
                    }
                    return result;
                }

                if (e.Detail == "探索信息")
                {
                    string msg = Controller.GetExploreInfo(uid);
                    if (msg != "")
                    {
                        await SendAsync(e, "探索信息", string.Join("\r\n", msg));
                    }
                    return result;
                }

                if (e.Detail == "探索结算")
                {
                    string msg = Controller.SettleExploreAll(uid);
                    if (msg != "")
                    {
                        await SendAsync(e, "探索结算", string.Join("\r\n", msg));
                    }
                    return result;
                }

                if (e.Detail.StartsWith("探索") || e.Detail.StartsWith("前往"))
                {
                    string detail = e.Detail.Replace("探索", "").Replace("前往", "").Trim();
                    string msg = "";
                    string eid = "";
                    string[] strings = detail.Split(FunGameConstant.SplitChars, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    List<int> cindexs = [];
                    foreach (string s in strings)
                    {
                        if (int.TryParse(s, out int c))
                        {
                            cindexs.Add(c);
                        }
                    }
                    if (cindexs.Count > 1 && cindexs.Count <= 5)
                    {
                        (msg, eid) = Controller.ExploreRegion(uid, cindexs[0], false, [.. cindexs.Skip(1).Select(id => (long)id)]);
                        if (msg.Trim() != "")
                        {
                            await SendAsync(e, "探索", msg);
                        }
                        _ = Task.Run(async () =>
                        {
                            await Task.Delay(FunGameConstant.ExploreTime * 60 * 1000);
                            msg = Controller.SettleExplore(eid, uid);
                            if (msg.Trim() != "")
                            {
                                await SendAsync(e, "探索", msg, msgSeq: 2);
                            }
                        });
                    }
                    else if (cindexs.Count > 5)
                    {
                        await SendAsync(e, "探索", "一次探索只能指定至多 4 个角色，需要注意：探索角色越多，奖励越多，但是会扣除相应的探索次数。");
                    }
                    else
                    {
                        await SendAsync(e, "探索", "探索指令格式错误，正确格式为：探索 <地区序号> <{角色序号...}>");
                    }
                    return result;
                }

                if (e.Detail.StartsWith("小队探索"))
                {
                    string detail = e.Detail.Replace("小队探索", "").Trim();
                    string msg = "";
                    string eid = "";
                    string[] strings = detail.Split(FunGameConstant.SplitChars, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    List<int> cindexs = [];
                    foreach (string s in strings)
                    {
                        if (int.TryParse(s, out int c))
                        {
                            cindexs.Add(c);
                        }
                    }
                    if (cindexs.Count > 0)
                    {
                        (msg, eid) = Controller.ExploreRegion(uid, cindexs[0], true);
                        if (msg.Trim() != "")
                        {
                            await SendAsync(e, "探索", msg);
                        }
                        _ = Task.Run(async () =>
                        {
                            await Task.Delay(FunGameConstant.ExploreTime * 60 * 1000);
                            msg = Controller.SettleExplore(eid, uid);
                            if (msg.Trim() != "")
                            {
                                await SendAsync(e, "探索", msg, msgSeq: 2);
                            }
                        });
                    }
                    else
                    {
                        await SendAsync(e, "探索", "探索指令格式错误，正确格式为：小队探索 <地区序号>");
                    }
                    return result;
                }

                if (e.Detail == "生命之泉")
                {
                    string msg = Controller.SpringOfLife(uid);
                    if (msg != "")
                    {
                        await SendAsync(e, "生命之泉", string.Join("\r\n", msg));
                    }
                    return result;
                }

                if (e.Detail == "酒馆" || e.Detail == "上酒")
                {
                    string msg = Controller.Pub(uid);
                    if (msg != "")
                    {
                        await SendAsync(e, "酒馆", string.Join("\r\n", msg));
                    }
                    return result;
                }

                if (e.Detail == "毕业礼包")
                {
                    if (FunGameService.Activities.FirstOrDefault(a => a.Name == "毕业季") is Activity activity && activity.Status == ActivityState.InProgress)
                    {
                        string msg = Controller.CreateGiftBox(uid, "毕业礼包", true, 2);
                        if (msg != "")
                        {
                            await SendAsync(e, "毕业礼包", string.Join("\r\n", msg));
                        }
                    }
                    else
                    {
                        await SendAsync(e, "毕业礼包", "活动不存在或已过期。");
                    }
                    return result;
                }

                if (e.Detail == "活动" || e.Detail == "活动中心")
                {
                    string msg = Controller.GetEvents(uid);
                    if (msg != "")
                    {
                        await SendAsync(e, "活动中心", string.Join("\r\n", msg));
                    }
                    return result;
                }

                if (e.Detail.StartsWith("查活动"))
                {
                    string detail = e.Detail.Replace("查活动", "").Trim();
                    string msg = "";
                    if (int.TryParse(detail, out int eid))
                    {
                        msg = Controller.GetEvents(uid, eid);
                        if (msg.Trim() != "")
                        {
                            await SendAsync(e, "查活动", msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("批量锁定", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("批量锁定", "").Trim();
                    string pattern = @"\s*(?<itemName>[^\d]+)\s*(?<count>\d+)\s*";
                    Match match = Regex.Match(detail, pattern);
                    if (match.Success)
                    {
                        string itemName = match.Groups["itemName"].Value.Trim();
                        if (int.TryParse(match.Groups["count"].Value, out int count))
                        {
                            string msg = Controller.LockItems(uid, itemName, count, false);
                            if (msg != "")
                            {
                                await SendAsync(e, "批量锁定", msg);
                            }
                        }
                    }

                    return result;
                }

                if (e.Detail.StartsWith("批量解锁", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("批量解锁", "").Trim();
                    string pattern = @"\s*(?<itemName>[^\d]+)\s*(?<count>\d+)\s*";
                    Match match = Regex.Match(detail, pattern);
                    if (match.Success)
                    {
                        string itemName = match.Groups["itemName"].Value.Trim();
                        if (int.TryParse(match.Groups["count"].Value, out int count))
                        {
                            string msg = Controller.LockItems(uid, itemName, count, true);
                            if (msg != "")
                            {
                                await SendAsync(e, "批量解锁", msg);
                            }
                        }
                    }

                    return result;
                }

                if (e.Detail.StartsWith("上锁") || e.Detail.StartsWith("锁定"))
                {
                    string detail = e.Detail.Replace("上锁", "").Replace("锁定", "").Trim();
                    string msg = "";
                    string[] strings = detail.Split(FunGameConstant.SplitChars, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    List<int> indexs = [];
                    foreach (string s in strings)
                    {
                        if (int.TryParse(s, out int c))
                        {
                            indexs.Add(c);
                        }
                    }
                    if (indexs.Count > 0)
                    {
                        msg = Controller.LockItem(uid, false, [.. indexs]);
                        if (msg.Trim() != "")
                        {
                            await SendAsync(e, "上锁", msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("解锁"))
                {
                    string detail = e.Detail.Replace("解锁", "").Trim();
                    string msg = "";
                    string[] strings = detail.Split(FunGameConstant.SplitChars, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    List<int> indexs = [];
                    foreach (string s in strings)
                    {
                        if (int.TryParse(s, out int c))
                        {
                            indexs.Add(c);
                        }
                    }
                    if (indexs.Count > 0)
                    {
                        msg = Controller.LockItem(uid, true, [.. indexs]);
                        if (msg.Trim() != "")
                        {
                            await SendAsync(e, "解锁", msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith(value: "创建报价"))
                {
                    string detail = e.Detail.Replace("创建报价", "").Trim();
                    string msg = "";
                    if (long.TryParse(detail, out long target))
                    {
                        msg = Controller.MakeOffer(uid, target);
                        if (msg.Trim() != "")
                        {
                            await SendAsync(e, "创建报价", msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith(value: "我的报价"))
                {
                    string detail = e.Detail.Replace("我的报价", "").Trim();
                    string msg = "";
                    if (int.TryParse(detail, out int page))
                    {
                        msg = Controller.GetOffer(uid, null, page);
                    }
                    else
                    {
                        msg = Controller.GetOffer(uid);
                    }
                    if (msg.Trim() != "")
                    {
                        await SendAsync(e, "我的报价", msg);
                    }
                    return result;
                }

                if (e.Detail.StartsWith(value: "查报价"))
                {
                    string detail = e.Detail.Replace("查报价", "").Trim();
                    string msg = "";
                    if (long.TryParse(detail, out long id))
                    {
                        msg = Controller.GetOffer(uid, id);
                        if (msg.Trim() != "")
                        {
                            await SendAsync(e, "查报价", msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith(value: "发送报价"))
                {
                    string detail = e.Detail.Replace("发送报价", "").Trim();
                    string msg = "";
                    if (long.TryParse(detail, out long id))
                    {
                        msg = Controller.SendOffer(uid, id);
                        if (msg.Trim() != "")
                        {
                            await SendAsync(e, "发送报价", msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith(value: "取消报价"))
                {
                    string detail = e.Detail.Replace("取消报价", "").Trim();
                    string msg = "";
                    if (long.TryParse(detail, out long id))
                    {
                        msg = Controller.CancelOffer(uid, id);
                        if (msg.Trim() != "")
                        {
                            await SendAsync(e, "取消报价", msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith(value: "接受报价"))
                {
                    string detail = e.Detail.Replace("接受报价", "").Trim();
                    string msg = "";
                    if (long.TryParse(detail, out long id))
                    {
                        msg = Controller.RespondOffer(uid, id, true);
                        if (msg.Trim() != "")
                        {
                            await SendAsync(e, "接受报价", msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith(value: "拒绝报价"))
                {
                    string detail = e.Detail.Replace("拒绝报价", "").Trim();
                    string msg = "";
                    if (long.TryParse(detail, out long id))
                    {
                        msg = Controller.RespondOffer(uid, id, false);
                        if (msg.Trim() != "")
                        {
                            await SendAsync(e, "拒绝报价", msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith(value: "报价添加物品"))
                {
                    string detail = e.Detail.Replace("报价添加物品", "").Trim();
                    string msg = "";
                    string[] strings = detail.Split(FunGameConstant.SplitChars, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    List<int> indexs = [];
                    foreach (string s in strings)
                    {
                        if (int.TryParse(s, out int c))
                        {
                            indexs.Add(c);
                        }
                    }
                    if (indexs.Count > 1)
                    {
                        msg = Controller.AddOfferItems(uid, indexs[0], false, [.. indexs[1..]]);
                        if (msg.Trim() != "")
                        {
                            await SendAsync(e, "报价添加物品", msg);
                        }
                    }
                    else
                    {
                        msg = "格式不正确，请先输入报价序号再输入物品序号，使用空格隔开。";
                    }
                    return result;
                }

                if (e.Detail.StartsWith(value: "报价添加对方物品"))
                {
                    string detail = e.Detail.Replace("报价添加对方物品", "").Trim();
                    string msg = "";
                    string[] strings = detail.Split(FunGameConstant.SplitChars, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    List<int> indexs = [];
                    foreach (string s in strings)
                    {
                        if (int.TryParse(s, out int c))
                        {
                            indexs.Add(c);
                        }
                    }
                    if (indexs.Count > 1)
                    {
                        msg = Controller.AddOfferItems(uid, indexs[0], true, [.. indexs[1..]]);
                        if (msg.Trim() != "")
                        {
                            await SendAsync(e, "报价添加对方物品", msg);
                        }
                    }
                    else
                    {
                        msg = "格式不正确，请先输入报价序号再输入物品序号，使用空格隔开。";
                    }
                    return result;
                }

                if (e.Detail.StartsWith(value: "报价移除物品"))
                {
                    string detail = e.Detail.Replace("报价移除物品", "").Trim();
                    string msg = "";
                    string[] strings = detail.Split(FunGameConstant.SplitChars, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    List<int> indexs = [];
                    foreach (string s in strings)
                    {
                        if (int.TryParse(s, out int c))
                        {
                            indexs.Add(c);
                        }
                    }
                    if (indexs.Count > 1)
                    {
                        msg = Controller.RemoveOfferItems(uid, indexs[0], false, [.. indexs[1..]]);
                        if (msg.Trim() != "")
                        {
                            await SendAsync(e, "报价移除物品", msg);
                        }
                    }
                    else
                    {
                        msg = "格式不正确，请先输入报价序号再输入物品序号，使用空格隔开。";
                    }
                    return result;
                }

                if (e.Detail.StartsWith(value: "报价移除对方物品"))
                {
                    string detail = e.Detail.Replace("报价移除对方物品", "").Trim();
                    string msg = "";
                    string[] strings = detail.Split(FunGameConstant.SplitChars, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    List<int> indexs = [];
                    foreach (string s in strings)
                    {
                        if (int.TryParse(s, out int c))
                        {
                            indexs.Add(c);
                        }
                    }
                    if (indexs.Count > 1)
                    {
                        msg = Controller.RemoveOfferItems(uid, indexs[0], true, [.. indexs[1..]]);
                        if (msg.Trim() != "")
                        {
                            await SendAsync(e, "报价移除对方物品", msg);
                        }
                    }
                    else
                    {
                        msg = "格式不正确，请先输入报价序号再输入物品序号，使用空格隔开。";
                    }
                    return result;
                }

                if (e.Detail.StartsWith("商店出售", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("商店出售", "").Trim();
                    List<int> ids = [];
                    foreach (string str in detail.Split(FunGameConstant.SplitChars, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (int.TryParse(str, out int id))
                        {
                            ids.Add(id);
                        }
                    }
                    string msg = Controller.StoreSellItem(uid, [.. ids]);
                    if (msg != "")
                    {
                        await SendAsync(e, "商店出售", msg);
                    }

                    return result;
                }

                if (e.Detail.StartsWith("挑战金币秘境"))
                {
                    string detail = e.Detail.Replace("挑战金币秘境", "").Trim();
                    string msg = "";
                    if (int.TryParse(detail, out int diff))
                    {
                        msg = await Controller.FightInstance(uid, (int)InstanceType.Currency, diff);
                        if (msg.Trim() != "")
                        {
                            await SendAsync(e, "挑战金币秘境", msg);
                        }
                    }
                    else
                    {
                        await SendAsync(e, "挑战秘境", "请在指令后面输入难度系数（1-5）");
                    }
                    return result;
                }

                if (e.Detail.StartsWith("挑战钻石秘境"))
                {
                    string detail = e.Detail.Replace("挑战钻石秘境", "").Trim();
                    string msg = "";
                    if (int.TryParse(detail, out int diff))
                    {
                        msg = await Controller.FightInstance(uid, (int)InstanceType.Material, diff);
                        if (msg.Trim() != "")
                        {
                            await SendAsync(e, "挑战钻石秘境", msg);
                        }
                    }
                    else
                    {
                        await SendAsync(e, "挑战秘境", "请在指令后面输入难度系数（1-5）");
                    }
                    return result;
                }

                if (e.Detail.StartsWith("挑战经验秘境"))
                {
                    string detail = e.Detail.Replace("挑战经验秘境", "").Trim();
                    string msg = "";
                    if (int.TryParse(detail, out int diff))
                    {
                        msg = await Controller.FightInstance(uid, (int)InstanceType.EXP, diff);
                        if (msg.Trim() != "")
                        {
                            await SendAsync(e, "挑战经验秘境", msg);
                        }
                    }
                    else
                    {
                        await SendAsync(e, "挑战秘境", "请在指令后面输入难度系数（1-5）");
                    }
                    return result;
                }

                if (e.Detail.StartsWith("挑战地区秘境"))
                {
                    string detail = e.Detail.Replace("挑战地区秘境", "").Trim();
                    string msg = "";
                    if (int.TryParse(detail, out int diff))
                    {
                        msg = await Controller.FightInstance(uid, (int)InstanceType.RegionItem, diff);
                        if (msg.Trim() != "")
                        {
                            await SendAsync(e, "挑战地区秘境", msg);
                        }
                    }
                    else
                    {
                        await SendAsync(e, "挑战秘境", "请在指令后面输入难度系数（1-5）");
                    }
                    return result;
                }

                if (e.Detail.StartsWith("挑战突破秘境"))
                {
                    string detail = e.Detail.Replace("挑战突破秘境", "").Trim();
                    string msg = "";
                    if (int.TryParse(detail, out int diff))
                    {
                        msg = await Controller.FightInstance(uid, (int)InstanceType.CharacterLevelBreak, diff);
                        if (msg.Trim() != "")
                        {
                            await SendAsync(e, "挑战突破秘境", msg);
                        }
                    }
                    else
                    {
                        await SendAsync(e, "挑战秘境", "请在指令后面输入难度系数（1-5）");
                    }
                    return result;
                }

                if (e.Detail.StartsWith("挑战技能秘境"))
                {
                    string detail = e.Detail.Replace("挑战技能秘境", "").Trim();
                    string msg = "";
                    if (int.TryParse(detail, out int diff))
                    {
                        msg = await Controller.FightInstance(uid, (int)InstanceType.SkillLevelUp, diff);
                        if (msg.Trim() != "")
                        {
                            await SendAsync(e, "挑战技能秘境", msg);
                        }
                    }
                    else
                    {
                        await SendAsync(e, "挑战秘境", "请在指令后面输入难度系数（1-5）");
                    }
                    return result;
                }

                if (e.Detail.StartsWith("挑战魔法卡秘境"))
                {
                    string detail = e.Detail.Replace("挑战魔法卡秘境", "").Trim();
                    string msg = "";
                    if (int.TryParse(detail, out int diff))
                    {
                        msg = await Controller.FightInstance(uid, (int)InstanceType.MagicCard, diff);
                        if (msg.Trim() != "")
                        {
                            await SendAsync(e, "挑战魔法卡秘境", msg);
                        }
                    }
                    else
                    {
                        await SendAsync(e, "挑战秘境", "请在指令后面输入难度系数（1-5）");
                    }
                    return result;
                }

                if (e.Detail == "后勤部")
                {
                    string msg = Controller.ShowSystemStore(uid, "铎京城", "dokyo_logistics");
                    if (msg.Trim() != "")
                    {
                        await SendAsync(e, "商店", msg);
                    }
                    return result;
                }

                if (e.Detail == "武器商会")
                {
                    string msg = Controller.ShowSystemStore(uid, "铎京城", "dokyo_weapons");
                    if (msg.Trim() != "")
                    {
                        await SendAsync(e, "商店", msg);
                    }
                    return result;
                }

                if (e.Detail == "杂货铺")
                {
                    string msg = Controller.ShowSystemStore(uid, "铎京城", "dokyo_yuki");
                    if (msg.Trim() != "")
                    {
                        await SendAsync(e, "商店", msg);
                    }
                    return result;
                }

                if (e.Detail == "基金会")
                {
                    string msg = Controller.ShowSystemStore(uid, "铎京城", "dokyo_welfare");
                    if (msg.Trim() != "")
                    {
                        await SendAsync(e, "商店", msg);
                    }
                    return result;
                }

                if (e.Detail == "锻造商店")
                {
                    string msg = Controller.ShowSystemStore(uid, "铎京城", "dokyo_forge");
                    if (msg.Trim() != "")
                    {
                        await SendAsync(e, "商店", msg);
                    }
                    return result;
                }

                if (e.Detail == "赛马商店")
                {
                    string msg = Controller.ShowSystemStore(uid, "铎京城", "dokyo_horseracing");
                    if (msg.Trim() != "")
                    {
                        await SendAsync(e, "商店", msg);
                    }
                    return result;
                }

                if (e.Detail == "共斗商店")
                {
                    string msg = Controller.ShowSystemStore(uid, "铎京城", "dokyo_cooperative");
                    if (msg.Trim() != "")
                    {
                        await SendAsync(e, "商店", msg);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("商店"))
                {
                    string detail = e.Detail.Replace("商店", "").Trim();
                    string msg = "";
                    if (int.TryParse(detail, out int storeId))
                    {
                        switch (storeId)
                        {
                            case 1:
                                msg = Controller.ShowSystemStore(uid, "铎京城", "dokyo_logistics");
                                break;
                            case 2:
                                msg = Controller.ShowSystemStore(uid, "铎京城", "dokyo_weapons");
                                break;
                            case 3:
                                msg = Controller.ShowSystemStore(uid, "铎京城", "dokyo_yuki");
                                break;
                            case 4:
                                msg = Controller.ShowSystemStore(uid, "铎京城", "dokyo_welfare");
                                break;
                            case 5:
                                msg = Controller.ShowSystemStore(uid, "铎京城", "dokyo_forge");
                                break;
                            case 6:
                                msg = Controller.ShowSystemStore(uid, "铎京城", "dokyo_horseracing");
                                break;
                            case 7:
                                msg = Controller.ShowSystemStore(uid, "铎京城", "dokyo_cooperative");
                                break;
                            default:
                                break;
                        }
                        if (msg.Trim() != "")
                        {
                            await SendAsync(e, "商店", msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("模拟锻造配方"))
                {
                    string detail = e.Detail.Replace("模拟", "").Trim();
                    string pattern = @"锻造配方\s*(?:(?<itemName>[^\d]+?)\s*(?<count>\d+)\s*)+";
                    Dictionary<string, int> recipeItems = [];

                    MatchCollection matches = Regex.Matches(detail, pattern, RegexOptions.ExplicitCapture);
                    foreach (Match match in matches)
                    {
                        CaptureCollection itemNames = match.Groups["itemName"].Captures;
                        CaptureCollection counts = match.Groups["count"].Captures;

                        for (int i = 0; i < itemNames.Count; i++)
                        {
                            string itemName = itemNames[i].Value.Trim();
                            if (int.TryParse(counts[i].Value, out int count))
                            {
                                recipeItems[itemName] = count;
                            }
                        }
                    }

                    User user = Factory.GetUser();
                    ForgeModel model = new()
                    {
                        ForgeMaterials = recipeItems
                    };
                    FunGameService.GenerateForgeResult(user, model, true);
                    if (model.ResultString != "")
                    {
                        await SendAsync(e, "模拟锻造配方", model.ResultString);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("锻造配方"))
                {
                    string pattern = @"锻造配方\s*(?:(?<itemName>[^\d]+?)\s*(?<count>\d+)\s*)+";
                    Dictionary<string, int> recipeItems = [];

                    MatchCollection matches = Regex.Matches(e.Detail, pattern, RegexOptions.ExplicitCapture);
                    foreach (Match match in matches)
                    {
                        CaptureCollection itemNames = match.Groups["itemName"].Captures;
                        CaptureCollection counts = match.Groups["count"].Captures;

                        for (int i = 0; i < itemNames.Count; i++)
                        {
                            string itemName = itemNames[i].Value.Trim();
                            if (int.TryParse(counts[i].Value, out int count))
                            {
                                recipeItems[itemName] = count;
                            }
                        }
                    }

                    string msg = Controller.ForgeItem_Create(uid, recipeItems);
                    if (msg != "")
                    {
                        await SendAsync(e, "锻造配方", msg);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("模拟锻造"))
                {
                    string msg = Controller.ForgeItem_Simulate(uid);
                    if (msg != "")
                    {
                        await SendAsync(e, "模拟锻造", msg);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("取消锻造"))
                {
                    string msg = Controller.ForgeItem_Cancel(uid);
                    if (msg != "")
                    {
                        await SendAsync(e, "取消锻造", msg);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("确认开始锻造"))
                {
                    string msg = Controller.ForgeItem_Complete(uid);
                    if (msg != "")
                    {
                        await SendAsync(e, "确认开始锻造", msg);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("锻造信息"))
                {
                    string msg = Controller.ForgeItem_Info(uid);
                    if (msg != "")
                    {
                        await SendAsync(e, "锻造信息", msg);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("大师锻造"))
                {
                    string detail = e.Detail.Replace("大师锻造", "").Trim();
                    string[] strings = detail.Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    int r = -1, q = -1;
                    if (strings.Length > 0 && int.TryParse(strings[0].Trim(), out r) && strings.Length > 1 && int.TryParse(strings[1].Trim(), out q))
                    {
                        if (r != -1 && q != -1)
                        {
                            string msg = Controller.ForgeItem_Master(uid, r, q);
                            if (msg != "")
                            {
                                await SendAsync(e, "大师锻造", msg);
                            }
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("市场出售"))
                {
                    string detail = e.Detail.Replace("市场出售", "").Trim();
                    string[] strings = detail.Split(FunGameConstant.SplitChars, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    if (strings.Length < 2 || !double.TryParse(strings[^1], out double price))
                    {
                        await SendAsync(e, "市场出售", "格式不正确，请使用：市场出售 <{物品序号...}> <价格>。多个物品序号使用空格隔开。");
                        return result;
                    }
                    List<int> ids = [];
                    for (int i = 0; i < strings.Length; i++)
                    {
                        if (i != strings.Length - 1 && int.TryParse(strings[i], out int id))
                        {
                            ids.Add(id);
                        }
                    }
                    string msg = Controller.MarketSellItem(uid, price, [.. ids]);
                    if (msg != "")
                    {
                        await SendAsync(e, "市场出售", msg);
                    }

                    return result;
                }

                if (e.Detail.StartsWith("市场购买"))
                {
                    string detail = e.Detail.Replace("市场购买", "").Trim();
                    string[] strings = detail.Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    int id = -1;
                    if (strings.Length > 0 && int.TryParse(strings[0].Trim(), out id))
                    {
                        int count = 1;
                        if (strings.Length > 1) _ = int.TryParse(strings[1].Trim(), out count);
                        if (id != -1)
                        {
                            string msg = Controller.MarketBuyItem(uid, id, count);
                            if (msg != "")
                            {
                                await SendAsync(e, "市场购买", msg);
                            }
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("市场查看"))
                {
                    string detail = e.Detail.Replace("市场查看", "").Trim();
                    string msg = "";
                    if (int.TryParse(detail, out int id))
                    {
                        msg = Controller.MarketItemInfo(uid, id);
                        if (msg.Trim() != "")
                        {
                            await SendAsync(e, "市场查看", msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("市场下架"))
                {
                    string detail = e.Detail.Replace("市场下架", "").Trim();
                    string msg = "";
                    if (int.TryParse(detail, out int id))
                    {
                        msg = Controller.MarketDelistItem(uid, id);
                        if (msg.Trim() != "")
                        {
                            await SendAsync(e, "市场下架", msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("我的市场"))
                {
                    string detail = e.Detail.Replace("我的市场", "").Trim();
                    string msg = "";
                    if (int.TryParse(detail, out int page))
                    {
                        msg = Controller.MarketShowListMySells(uid, page);
                    }
                    else
                    {
                        msg = Controller.MarketShowListMySells(uid, 1);
                    }
                    if (msg.Trim() != "")
                    {
                        await SendAsync(e, "我的市场", msg);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("我的购买"))
                {
                    string detail = e.Detail.Replace("我的购买", "").Trim();
                    string msg = "";
                    if (int.TryParse(detail, out int page))
                    {
                        msg = Controller.MarketShowListMyBuys(uid, page);
                    }
                    else
                    {
                        msg = Controller.MarketShowListMyBuys(uid, 1);
                    }
                    if (msg.Trim() != "")
                    {
                        await SendAsync(e, "我的购买", msg);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("市场"))
                {
                    string detail = e.Detail.Replace("市场", "").Trim();
                    string msg = "";
                    if (int.TryParse(detail, out int page))
                    {
                        msg = Controller.MarketShowList(uid, page);
                    }
                    else
                    {
                        msg = Controller.MarketShowList(uid, 1);
                    }
                    if (msg.Trim() != "")
                    {
                        await SendAsync(e, "市场", msg);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("私信"))
                {
                    string detail = e.Detail.Replace("私信", "").Trim();
                    string[] strings = detail.Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    long id = -1;
                    string name = "";
                    bool useId = false;
                    if (strings.Length > 0)
                    {
                        if (long.TryParse(strings[0].Trim(), out id))
                        {
                            useId = true;
                        }
                        else
                        {
                            name = strings[0].Trim();
                        }
                    }
                    else
                    {
                        await SendAsync(e, "私信", "你输入的格式不正确，正确格式：私信 <对方UID/昵称> <内容>");
                        return result;
                    }
                    string content = "";
                    if (strings.Length > 1) content = string.Join(" ", strings[1..]);
                    string msg = "";
                    if (useId)
                    {
                        msg = Controller.Chat(uid, id, content);
                    }
                    else
                    {
                        msg = Controller.Chat_Name(uid, name, content);
                    }
                    if (msg != "")
                    {
                        await SendAsync(e, "私信", msg);
                    }
                    return result;
                }

                if (e.Detail == "创建赛马")
                {
                    string groupId = "";
                    if (e.IsGroup && e is GroupAtMessage groupAtMessage && groupAtMessage.GroupOpenId != "")
                    {
                        groupId = groupAtMessage.GroupOpenId;
                    }
                    else if (e.IsGroup && e is ThirdPartyMessage thirdPartyMessage && thirdPartyMessage.GroupOpenId != "")
                    {
                        groupId = thirdPartyMessage.GroupOpenId;
                    }
                    if (groupId != "")
                    {
                        string msg = Controller.RoomCreate(uid, "horseracing", "", groupId);
                        if (msg.Trim() != "")
                        {
                            await SendAsync(e, "赛马", msg);
                        }
                    }
                    else
                    {
                        await SendAsync(e, "赛马", "请在群聊中进行多人游戏。");
                    }
                    return result;
                }

                if (e.Detail == "创建共斗")
                {
                    string groupId = "";
                    if (e.IsGroup && e is GroupAtMessage groupAtMessage && groupAtMessage.GroupOpenId != "")
                    {
                        groupId = groupAtMessage.GroupOpenId;
                    }
                    else if (e.IsGroup && e is ThirdPartyMessage thirdPartyMessage && thirdPartyMessage.GroupOpenId != "")
                    {
                        groupId = thirdPartyMessage.GroupOpenId;
                    }
                    if (groupId != "")
                    {
                        string msg = Controller.RoomCreate(uid, "cooperative", "", groupId);
                        if (msg.Trim() != "")
                        {
                            await SendAsync(e, "房间", msg);
                        }
                    }
                    else
                    {
                        await SendAsync(e, "房间", "请在群聊中进行多人游戏。");
                    }
                    return result;
                }

                if (e.Detail == "创建混战")
                {
                    string groupId = "";
                    if (e.IsGroup && e is GroupAtMessage groupAtMessage && groupAtMessage.GroupOpenId != "")
                    {
                        groupId = groupAtMessage.GroupOpenId;
                    }
                    else if (e.IsGroup && e is ThirdPartyMessage thirdPartyMessage && thirdPartyMessage.GroupOpenId != "")
                    {
                        groupId = thirdPartyMessage.GroupOpenId;
                    }
                    if (groupId != "")
                    {
                        string msg = Controller.RoomCreate(uid, "mix", "", groupId);
                        if (msg.Trim() != "")
                        {
                            await SendAsync(e, "房间", msg);
                        }
                    }
                    else
                    {
                        await SendAsync(e, "房间", "请在群聊中进行多人游戏。");
                    }
                    return result;
                }

                if (e.Detail == "创建团战")
                {
                    string groupId = "";
                    if (e.IsGroup && e is GroupAtMessage groupAtMessage && groupAtMessage.GroupOpenId != "")
                    {
                        groupId = groupAtMessage.GroupOpenId;
                    }
                    else if (e.IsGroup && e is ThirdPartyMessage thirdPartyMessage && thirdPartyMessage.GroupOpenId != "")
                    {
                        groupId = thirdPartyMessage.GroupOpenId;
                    }
                    if (groupId != "")
                    {
                        string msg = Controller.RoomCreate(uid, "team", "", groupId);
                        if (msg.Trim() != "")
                        {
                            await SendAsync(e, "房间", msg);
                        }
                    }
                    else
                    {
                        await SendAsync(e, "房间", "请在群聊中进行多人游戏。");
                    }
                    return result;
                }

                if (e.Detail == "加入赛马")
                {
                    string groupId = "";
                    if (e.IsGroup && e is GroupAtMessage groupAtMessage && groupAtMessage.GroupOpenId != "")
                    {
                        groupId = groupAtMessage.GroupOpenId;
                    }
                    else if (e.IsGroup && e is ThirdPartyMessage thirdPartyMessage && thirdPartyMessage.GroupOpenId != "")
                    {
                        groupId = thirdPartyMessage.GroupOpenId;
                    }
                    if (groupId != "")
                    {
                        if (FunGameConstant.Rooms.Values.FirstOrDefault(r => r.GameMap == groupId) is Room room)
                        {
                            string msg = Controller.RoomInto(uid, room.Roomid, "");
                            if (msg.Trim() != "")
                            {
                                await SendAsync(e, "赛马", msg);
                            }
                        }
                        else
                        {
                            await SendAsync(e, "赛马", "本群还没有创建赛马房间，请使用【创建赛马】指令来创建一个房间。");
                        }
                    }
                    else
                    {
                        await SendAsync(e, "赛马", "请在群聊中进行多人游戏。");
                    }
                    return result;
                }

                if (e.Detail.StartsWith("创建房间"))
                {
                    string groupId = "";
                    if (e.IsGroup && e is GroupAtMessage groupAtMessage && groupAtMessage.GroupOpenId != "")
                    {
                        groupId = groupAtMessage.GroupOpenId;
                    }
                    else if (e.IsGroup && e is ThirdPartyMessage thirdPartyMessage && thirdPartyMessage.GroupOpenId != "")
                    {
                        groupId = thirdPartyMessage.GroupOpenId;
                    }
                    if (groupId != "")
                    {
                        string detail = e.Detail.Replace("创建房间", "").Trim();
                        string[] strings = detail.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                        string roomType = "", password = "";
                        if (strings.Length > 0) roomType = strings[0];
                        if (strings.Length > 1)
                        {
                            int firstSpaceIndex = detail.IndexOf(' ');
                            if (firstSpaceIndex != -1 && firstSpaceIndex + 1 < detail.Length)
                            {
                                password = detail[(firstSpaceIndex + 1)..].Trim();
                            }
                        }
                        string msg = Controller.RoomCreate(uid, roomType, password, groupId);
                        if (msg.Trim() != "")
                        {
                            await SendAsync(e, "房间", msg);
                        }
                    }
                    else
                    {
                        await SendAsync(e, "房间", "请在群聊中进行多人游戏。");
                    }
                    return result;
                }

                if (e.Detail.StartsWith("加入房间"))
                {
                    string groupId = "";
                    if (e.IsGroup && e is GroupAtMessage groupAtMessage && groupAtMessage.GroupOpenId != "")
                    {
                        groupId = groupAtMessage.GroupOpenId;
                    }
                    else if (e.IsGroup && e is ThirdPartyMessage thirdPartyMessage && thirdPartyMessage.GroupOpenId != "")
                    {
                        groupId = thirdPartyMessage.GroupOpenId;
                    }
                    if (groupId != "")
                    {
                        string detail = e.Detail.Replace("加入房间", "").Trim();
                        string[] strings = detail.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                        string roomid = "", password = "";
                        if (strings.Length > 0) roomid = strings[0];
                        if (strings.Length > 1)
                        {
                            int firstSpaceIndex = detail.IndexOf(' ');
                            if (firstSpaceIndex != -1 && firstSpaceIndex + 1 < detail.Length)
                            {
                                password = detail[(firstSpaceIndex + 1)..].Trim();
                            }
                        }
                        string msg = Controller.RoomInto(uid, roomid, password);
                        if (msg.Trim() != "")
                        {
                            await SendAsync(e, "房间", msg);
                        }
                    }
                    else
                    {
                        await SendAsync(e, "房间", "请在群聊中进行多人游戏。");
                    }
                    return result;
                }

                if (e.Detail == "开始游戏" || e.Detail == "开始赛马")
                {
                    string groupId = "";
                    if (e.IsGroup && e is GroupAtMessage groupAtMessage && groupAtMessage.GroupOpenId != "")
                    {
                        groupId = groupAtMessage.GroupOpenId;
                    }
                    else if (e.IsGroup && e is ThirdPartyMessage thirdPartyMessage && thirdPartyMessage.GroupOpenId != "")
                    {
                        groupId = thirdPartyMessage.GroupOpenId;
                    }
                    if (groupId != "")
                    {
                        if (e.Detail == "开始赛马" && (!FunGameConstant.UsersInRoom.TryGetValue(uid, out Room? value) || value is null || value.Name != "赛马房间"))
                        {
                            await SendAsync(e, "房间", "你不在房间中或者所在的房间不是赛马房间，请使用【开始游戏】指令。注意：只有房主才可以开始游戏。");
                            return result;
                        }
                        (Room room, List<string> msgs) = await Controller.RoomRunGame(uid);
                        List<string> real = MergeMessages(msgs);
                        int count = 1;
                        foreach (string msg in real)
                        {
                            await SendAsync(e, "房间", msg.Trim(), msgSeq: count++);
                            if (count <= real.Count) await Task.Delay(1500);
                        }
                        OnlineService.ReSetRoomState(room.Roomid);
                    }
                    else
                    {
                        await SendAsync(e, "房间", "请在群聊中进行多人游戏。");
                    }
                    return result;
                }

                if (e.Detail == "退出房间")
                {
                    string groupId = "";
                    if (e.IsGroup && e is GroupAtMessage groupAtMessage && groupAtMessage.GroupOpenId != "")
                    {
                        groupId = groupAtMessage.GroupOpenId;
                    }
                    else if (e.IsGroup && e is ThirdPartyMessage thirdPartyMessage && thirdPartyMessage.GroupOpenId != "")
                    {
                        groupId = thirdPartyMessage.GroupOpenId;
                    }
                    if (groupId != "")
                    {
                        string msg = Controller.RoomQuit(uid);
                        if (msg.Trim() != "")
                        {
                            await SendAsync(e, "房间", msg);
                        }
                    }
                    else
                    {
                        await SendAsync(e, "房间", "请在群聊中进行多人游戏。");
                    }
                    return result;
                }

                if (e.Detail == "房间列表")
                {
                    string groupId = "";
                    if (e.IsGroup && e is GroupAtMessage groupAtMessage && groupAtMessage.GroupOpenId != "")
                    {
                        groupId = groupAtMessage.GroupOpenId;
                    }
                    else if (e.IsGroup && e is ThirdPartyMessage thirdPartyMessage && thirdPartyMessage.GroupOpenId != "")
                    {
                        groupId = thirdPartyMessage.GroupOpenId;
                    }
                    if (groupId != "")
                    {
                        string msg = Controller.RoomShowList(uid, groupId);
                        if (msg.Trim() != "")
                        {
                            await SendAsync(e, "房间", msg);
                        }
                    }
                    else
                    {
                        await SendAsync(e, "房间", "请在群聊中进行多人游戏。");
                    }
                    return result;
                }

                if (e.Detail == "房间信息")
                {
                    string msg = Controller.RoomInfo(uid);
                    if (msg.Trim() != "")
                    {
                        await SendAsync(e, "房间", msg);
                    }
                    return result;
                }

                if (e.Detail == "排行榜" || e.Detail == "养成排行榜")
                {
                    string msg = Controller.GetRanking(uid, 2);
                    if (msg.Trim() != "")
                    {
                        await SendAsync(e, "排行榜", msg);
                    }
                    return result;
                }

                if (e.Detail == $"{General.GameplayEquilibriumConstant.InGameCurrency}排行榜")
                {
                    string msg = Controller.GetRanking(uid, 0);
                    if (msg.Trim() != "")
                    {
                        await SendAsync(e, "排行榜", msg);
                    }
                    return result;
                }

                if (e.Detail == $"{General.GameplayEquilibriumConstant.InGameMaterial}排行榜")
                {
                    string msg = Controller.GetRanking(uid, 1);
                    if (msg.Trim() != "")
                    {
                        await SendAsync(e, "排行榜", msg);
                    }
                    return result;
                }

                if (e.Detail == $"赛马排行榜")
                {
                    string msg = Controller.GetRanking(uid, 3);
                    if (msg.Trim() != "")
                    {
                        await SendAsync(e, "排行榜", msg);
                    }
                    return result;
                }

                if (e.Detail == $"共斗排行榜")
                {
                    string msg = Controller.GetRanking(uid, 4);
                    if (msg.Trim() != "")
                    {
                        await SendAsync(e, "排行榜", msg);
                    }
                    return result;
                }

                if (e.Detail == $"锻造排行榜")
                {
                    string msg = Controller.GetRanking(uid, 5);
                    if (msg.Trim() != "")
                    {
                        await SendAsync(e, "排行榜", msg);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("加物") || e.Detail.StartsWith("添加背包物品"))
                {
                    string detail = e.Detail.Replace("加物", "").Replace("添加背包物品", "").Trim();
                    string[] strings = detail.Split(FunGameConstant.SplitChars, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    if (strings.Length < 2)
                    {
                        await SendAsync(e, "加物", "格式不正确，请使用：加物 <角色> <{库存物品序号...}>。多个物品序号使用空格隔开。");
                        return result;
                    }
                    List<int> ids = [];
                    int cid = -999;
                    for (int i = 0; i < strings.Length; i++)
                    {
                        if (int.TryParse(strings[i], out int id))
                        {
                            if (i != 0) ids.Add(id);
                            else cid = id;
                        }
                    }
                    string msg = Controller.AddItemsToCharacter(uid, cid, [.. ids]);
                    if (msg != "")
                    {
                        await SendAsync(e, "加物", msg);
                    }

                    return result;
                }

                if (e.Detail.StartsWith("减物") || e.Detail.StartsWith("移除背包物品"))
                {
                    string detail = e.Detail.Replace("减物", "").Replace("移除背包物品", "").Trim();
                    string[] strings = detail.Split(FunGameConstant.SplitChars, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    if (strings.Length < 2)
                    {
                        await SendAsync(e, "减物", "格式不正确，请使用：减物 <角色> <{库存物品序号...}>。多个物品序号使用空格隔开。");
                        return result;
                    }
                    List<int> ids = [];
                    int cid = -999;
                    for (int i = 0; i < strings.Length; i++)
                    {
                        if (int.TryParse(strings[i], out int id))
                        {
                            if (i != 0) ids.Add(id);
                            else cid = id;
                        }
                    }
                    string msg = Controller.RemoveItemsFromCharacter(uid, cid, [.. ids]);
                    if (msg != "")
                    {
                        await SendAsync(e, "减物", msg);
                    }

                    return result;
                }

                if (uid == GeneralSettings.Master && e.Detail.StartsWith("重载FunGame", StringComparison.CurrentCultureIgnoreCase))
                {
                    string msg = Controller.Relaod(uid);
                    if (msg != "")
                    {
                        await SendAsync(e, "重载FunGame", msg);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                if (Logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Error)) Logger.LogError("Error: {ex}", ex);
            }

            return false;
        }

        public async Task<bool> HandlerByConsole(IBotMessage e, OtherData data)
        {
            if (MemoryCache.Get(e.AuthorOpenId) is null)
            {
                MemoryCache.Set(e.AuthorOpenId, 1L, TimeSpan.FromMinutes(10));
            }
            return await Handler(e, data);
        }

        public List<string> MergeMessages(List<string> msgs)
        {
            List<string> real = [];
            if (msgs.Count > 1)
            {
                if (msgs.Count > 20)
                {
                    msgs = [msgs[0], .. msgs[^20..]];
                }
                int perMergeLength = msgs.Count > 5 ? 5 : msgs.Count;
                int remain = perMergeLength;
                string merge = "";
                for (int i = 0; i < msgs.Count; i++)
                {
                    remain--;
                    merge += msgs[i] + "\r\n";
                    if (remain == 0 || i == msgs.Count - 1)
                    {
                        real.Add(merge);
                        merge = "";
                        if (msgs.Count < perMergeLength)
                        {
                            remain = msgs.Count;
                        }
                        else remain = perMergeLength;
                    }
                }
            }
            else
            {
                real = msgs;
            }
            if (real.Count >= 3)
            {
                real = [real[0], .. real[^2..]];
            }
            return real;
        }
    }
}
