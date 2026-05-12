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
using Oshima.FunGame.OshimaServers.Model;
using Oshima.FunGame.OshimaServers.Service;
using Oshima.FunGame.WebAPI.Constant;
using Oshima.FunGame.WebAPI.Controllers;
using Oshima.FunGame.WebAPI.Models;

namespace Oshima.FunGame.WebAPI.Services
{
    public partial class RainBOTService(FunGameController controller, QQController qqcontroller, QQBotService service, ILogger<RainBOTService> logger, IMemoryCache memoryCache, TestController testController, CSBettingController bettingController)
    {
        private static List<string> FunGameItemType { get; } = ["卡包", "武器", "防具", "鞋子", "饰品", "消耗品", "魔法卡", "收藏品", "特殊物品", "任务物品", "礼包", "其他"];
        private static string[] CheckDateTimeFormat { get; } = ["yyyy-MM-dd HH:mm", "yyyy-MM-dd"];
        private bool FunGameSimulation { get; set; } = false;
        private FunGameController Controller { get; } = controller;
        private QQController QQController { get; } = qqcontroller;
        private QQBotService Service { get; } = service;
        private ILogger<RainBOTService> Logger { get; } = logger;
        private IMemoryCache MemoryCache { get; set; } = memoryCache;
        private TestController TestController { get; set; } = testController;
        private CSBettingController BettingController { get; set; } = bettingController;

        private async Task SendAsync(IBotMessage msg, string title, BotReply reply, int? msgSeq = null)
        {
            if (reply.Markdown != null && !string.IsNullOrWhiteSpace(reply.Markdown.Content))
            {
                // 发送 Markdown + 键盘
                await SendMarkdownAsync(msg, title, reply.Markdown, reply.Keyboard, msgSeq);
            }
            else if (reply.Keyboard != null)
            {
                // 发送 文本 + 键盘
                await SendMarkdownAsync(msg, title, new()
                {
                    Content = reply.Text
                }, reply.Keyboard, msgSeq);
            }
            else if (!string.IsNullOrWhiteSpace(reply.Text))
            {
                // 发送纯文本
                await SendTextAsync(msg, title, reply.Text, msgSeq: msgSeq);
            }
        }

        private async Task SendTextAsync(IBotMessage msg, string title, string content, int msgType = 0, object? media = null, int? msgSeq = null)
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
            await CheckOfflineNotice(msg);
        }

        private async Task SendMarkdownAsync(IBotMessage msg, string title, MarkdownMessage mdMsg, KeyboardMessage? kbMsg = null, int? msgSeq = null)
        {
            Statics.RunningPlugin?.Controller.WriteLine(title, Milimoe.FunGame.Core.Library.Constant.LogLevel.Debug);
            if (msg is ThirdPartyMessage third)
            {
                third.Result += "\r\n" + mdMsg.Content?.Trim();
                third.IsCompleted = true;
            }
            else if (msg.IsGroup)
            {
                await Service.SendGroupMarkdownAsync(msg.OpenId, mdMsg, kbMsg, msg.Id, msgSeq);
            }
            else
            {
                await Service.SendC2CMarkdownAsync(msg.OpenId, mdMsg, kbMsg, msg.Id, msgSeq);
            }
            await CheckOfflineNotice(msg);
        }

        private async Task CheckOfflineNotice(IBotMessage msg)
        {
            if (msg.UseNotice && msg.FunGameUID > 0 && FunGameService.UserNotice.TryGetValue(msg.FunGameUID, out HashSet<string>? msgs) && msgs != null)
            {
                FunGameService.UserNotice.Remove(msg.FunGameUID, out _);
                await SendTextAsync(msg, "离线未读信箱", $"☆--- 离线未读信箱 ---☆\r\n{string.Join("\r\n", msgs)}", 0, null, 5);
            }
        }

        private async Task SendHelp(IBotMessage e, Dictionary<string, string> helpDict, string helpName, int currentPage)
        {
            e.UseNotice = false;
            if (currentPage <= 0) currentPage = 1;
            int pageSize = 15;
            int totalPages = helpDict.MaxPage(pageSize);

            StringBuilder result = new($"《筽祀牻》{helpName}指令（第 {currentPage}/{totalPages} 页）\n");
            
            int index = (currentPage - 1) * pageSize + 1;
            foreach ((string cmd, string desc) in helpDict.GetPage(currentPage, pageSize))
            {
                result.AppendLine($"{index}. {cmd}{(desc != "" ? "：" + desc : "")}");
                index++;
            }

            KeyboardMessage kb = new KeyboardMessage().AddPaginationRow(helpName, currentPage, totalPages);

            await SendAsync(e, "筽祀牻", new BotReply()
            {
                Markdown = new()
                {
                    Content = result.ToString()
                },
                Keyboard = kb
            });
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
                                FunGameService.UIDWithOpenID[uid] = openid;
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
                //    BotReply reply = "";
                //    if (long.TryParse(detail, out temp_qq))
                //    {
                //        msg = QQController.Bind(new(openid, temp_qq));
                //    }
                //    else
                //    {
                //        msg = "绑定失败，请提供一个正确的QQ号！";
                //    }
                //    await SendAsync(e, "绑定", reply);
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

                        // 传统派已离场
                        //string? fi = "";
                        //string? err = "";
                        //try
                        //{
                        //    UploadMediaResult uploadMediaResult = e.IsGroup ? await Service.UploadGroupMediaAsync(e.OpenId, 1, img) : await Service.UploadC2CMediaAsync(e.OpenId, 1, img);
                        //    fi = uploadMediaResult.FileInfo;
                        //    err = uploadMediaResult.Error;
                        //}
                        //catch (Exception ex)
                        //{
                        //    err = ex.ToString();
                        //}
                        //if (string.IsNullOrEmpty(err))
                        //{
                        //    await SendTextAsync(e, "每日运势", daily.daily, 7, new { file_info = fi });
                        //}
                        //else
                        //{
                        //    if (Logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Error)) Logger.LogError("上传图片失败：{error}", err);
                        //    await SendAsync(e, "每日运势", daily.daily);
                        //}

                        // 使用md
                        await SendAsync(e, "每日运势", new BotReply()
                        {
                            Markdown = new()
                            {
                                Content = $"{daily.daily}\r\n![text #256px #256px]({img})"
                            }
                        });

                        BotReply rpy = Controller.GetUserDailyItem(uid, daily.daily);
                        await SendAsync(e, "运势幸运物发放", rpy, msgSeq: 3);
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
                    List<string> msgs = [];
                    DateTime now = DateTime.Now;
                    foreach (NoticeModel notice in FunGameService.Notices.Values)
                    {
                        if (now >= notice.StartTime && now <= notice.EndTime)
                        {
                            msgs.Add(notice.ToString());
                        }
                    }
                    if (msgs.Count > 0)
                    {
                        await SendAsync(e, "公告", new BotReply()
                        {
                            Markdown = new()
                            {
                                Content = $"系统公告列表：\r\n```\r\n{string.Join("\r\n", msgs)}\r\n```"
                            }
                        });
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
                    BotReply reply = TestController.GetLastLoginTime();
                    await SendAsync(e, "查询服务器启动时间", reply);
                    return true;
                }

                if (e.Detail.StartsWith("查询任务计划"))
                {
                    e.UseNotice = false;
                    BotReply reply = TestController.GetTaskScheduler(e.Detail.Replace("查询任务计划", ""));
                    await SendAsync(e, "查询任务计划", reply);
                    return true;
                }

                // 指令处理
                if (e.Detail == "帮助")
                {
                    e.UseNotice = false;
                    await SendAsync(e, "筽祀牻", new BotReply()
                    {
                        Markdown = new()
                        {
                            Content = @$"欢迎使用《筽祀牻》游戏指令帮助系统！
核心库版本号：{FunGameInfo.FunGame_Version}
《筽祀牻》是一款奇幻冒险回合制角色扮演游戏。
在游戏中，你可以和其他角色组成小队，收集物品，在数十个独具风格的地区中冒险并战斗。
因游戏内容、指令较多，我们将按游戏模块对指令分类，请输入以下指令查看具体分类的帮助内容：
1、<qqbot-cmd-input text=""存档帮助 "" show=""存档帮助""/>
2、<qqbot-cmd-input text=""角色帮助 "" show=""角色帮助""/>
3、<qqbot-cmd-input text=""物品帮助 "" show=""物品帮助""/>
4、<qqbot-cmd-input text=""战斗帮助 "" show=""战斗帮助""/>
5、<qqbot-cmd-input text=""玩法帮助 "" show=""玩法帮助""/>
6、<qqbot-cmd-input text=""社团帮助 "" show=""社团帮助""/>
7、<qqbot-cmd-input text=""活动帮助 "" show=""活动帮助""/>
8、<qqbot-cmd-input text=""商店帮助 "" show=""商店帮助""/>
在指令后面加数字即可跳转指定的页码，感谢游玩《筽祀牻》。"
                        }
                    });
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
                        BotReply rpy = MergeToMarkdown("模拟结果如下：", msgs);
                        rpy.Keyboard = new KeyboardMessage().AppendButtons(1, Button.CreateCmdButton("FunGame模拟", $"FunGame模拟"));
                        await SendAsync(e, "筽祀牻", rpy, msgSeq: 1);
                        FunGameSimulation = false;
                    }
                    else
                    {
                        await SendAsync(e, "筽祀牻", "游戏正在模拟中，请勿重复请求！");
                    }
                    return result;
                }

                if (e.Detail.StartsWith("个人地图模拟", StringComparison.CurrentCultureIgnoreCase))
                {
                    e.UseNotice = false;
                    if (!FunGameSimulation)
                    {
                        FunGameSimulation = true;
                        List<string> msgs = await Controller.GetTest(false, maxRespawnTimesMix: 0, hasMap: true);
                        BotReply rpy = MergeToMarkdown("模拟结果如下：", msgs);
                        rpy.Keyboard = new KeyboardMessage().AppendButtons(1, Button.CreateCmdButton("个人地图模拟", $"个人地图模拟"));
                        await SendAsync(e, "筽祀牻", rpy, msgSeq: 1);
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
                        BotReply rpy = MergeToMarkdown("模拟结果如下：", msgs);
                        rpy.Keyboard = new KeyboardMessage().AppendButtons(1, Button.CreateCmdButton("混战模拟", $"混战模拟"));
                        await SendAsync(e, "筽祀牻", rpy, msgSeq: 1);
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
                        BotReply rpy = MergeToMarkdown("模拟结果如下：", msgs);
                        rpy.Keyboard = new KeyboardMessage().AppendButtons(1, Button.CreateCmdButton("FunGame团队模拟", $"FunGame团队模拟"));
                        await SendAsync(e, "筽祀牻", rpy, msgSeq: 1);
                        FunGameSimulation = false;
                    }
                    else
                    {
                        await SendAsync(e, "筽祀牻", "游戏正在模拟中，请勿重复请求！");
                    }
                    return result;
                }

                if (e.Detail.StartsWith("团队地图模拟", StringComparison.CurrentCultureIgnoreCase))
                {
                    e.UseNotice = false;
                    if (!FunGameSimulation)
                    {
                        FunGameSimulation = true;
                        List<string> msgs = await Controller.GetTest(false, true, hasMap: true);
                        BotReply rpy = MergeToMarkdown("模拟结果如下：", msgs);
                        rpy.Keyboard = new KeyboardMessage().AppendButtons(1, Button.CreateCmdButton("团队地图模拟", $"团队地图模拟"));
                        await SendAsync(e, "筽祀牻", rpy, msgSeq: 1);
                        FunGameSimulation = false;
                    }
                    else
                    {
                        await SendAsync(e, "筽祀牻", "游戏正在模拟中，请勿重复请求！");
                    }
                    return result;
                }

                if (e.Detail.StartsWith("团队调试模拟", StringComparison.CurrentCultureIgnoreCase))
                {
                    e.UseNotice = false;
                    if (!FunGameSimulation)
                    {
                        FunGameSimulation = true;
                        List<string> msgs = await Controller.GetTestDebug(false, true, hasMap: true);
                        BotReply rpy = MergeToMarkdown("模拟结果如下：", msgs);
                        rpy.Keyboard = new KeyboardMessage().AppendButtons(1, Button.CreateCmdButton("团队调试模拟", $"团队调试模拟"));
                        await SendAsync(e, "筽祀牻", rpy, msgSeq: 1);
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
                        BotReply reply = Controller.GetStats(id);
                        await SendAsync(e, "查数据", reply);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("查团队数据", StringComparison.CurrentCultureIgnoreCase))
                {
                    e.UseNotice = false;
                    string detail = e.Detail.Replace("查团队数据", "").Trim();
                    if (int.TryParse(detail, out int id))
                    {
                        BotReply reply = Controller.GetTeamStats(id);
                        await SendAsync(e, "查团队数据", reply);
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
                        BotReply reply = Controller.GetCharacterInfo(id);
                        await SendAsync(e, "查角色", reply);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("查技能", StringComparison.CurrentCultureIgnoreCase))
                {
                    e.UseNotice = false;
                    string detail = e.Detail.Replace("查技能", "").Trim();
                    if (int.TryParse(detail, out int id))
                    {
                        BotReply reply = Controller.GetSkillInfo(uid, id);
                        await SendAsync(e, "查技能", reply);
                    }
                    else
                    {
                        BotReply reply = Controller.GetSkillInfo_Name(uid, detail);
                        await SendAsync(e, "查技能", reply);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("查物品", StringComparison.CurrentCultureIgnoreCase))
                {
                    e.UseNotice = false;
                    string detail = e.Detail.Replace("查物品", "").Trim();
                    if (int.TryParse(detail, out int id))
                    {
                        BotReply reply = Controller.GetItemInfo(uid, id);
                        await SendAsync(e, "查物品", reply);
                    }
                    else
                    {
                        BotReply reply = Controller.GetItemInfo_Name(uid, detail);
                        await SendAsync(e, "查物品", reply);
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

                        BotReply reply = Controller.CreateMagicCard(uid, targetUserID, quality, magicID, count, str, agi, intelligence);
                        await SendAsync(e, "熟圣之力", reply);
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
                            BotReply reply = Controller.CreateItem(uid, name, count, userid);
                            await SendAsync(e, "熟圣之力", reply);
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
                    BotReply reply = Controller.GenerateMagicCardPack();
                    await SendAsync(e, "预览魔法卡包", reply);
                    return result;
                }
                else if (e.Detail == "预览魔法卡")
                {
                    e.UseNotice = false;
                    BotReply reply = Controller.GenerateMagicCard();
                    await SendAsync(e, "预览魔法卡", reply);
                    return result;
                }

                if (e.Detail == "创建存档")
                {
                    e.UseNotice = false;
                    BotReply reply = Controller.CreateSaved(uid, openid);
                    await SendAsync(e, "创建存档", reply);
                    return result;
                }

                if (e.Detail == "我的存档")
                {
                    BotReply rpy = Controller.ShowSaved(uid);
                    rpy.Keyboard = new KeyboardMessage().AppendButtons(3, [
                        Button.CreateCmdButton("签到", "签到"),
                        Button.CreateCmdButton("帮助", "帮助"),
                        Button.CreateCmdButton("运势", "我的运势"),
                        Button.CreateCmdButton("库存", "我的库存"),
                        Button.CreateCmdButton("任务", "任务列表"),
                        Button.CreateCmdButton("商店", "每日商店")
                    ]);
                    await SendAsync(e, "我的存档", rpy);
                    return result;
                }

                if (e.Detail == "我的主战")
                {
                    e.UseNotice = false;
                    BotReply reply = Controller.GetCharacterInfoFromInventory(uid, 0);
                    await SendAsync(e, "我的主战", reply);
                    return result;
                }

                if (e.Detail == "我的状态")
                {
                    e.UseNotice = false;
                    BotReply reply = Controller.ShowMainCharacterOrSquadStatus(uid);
                    await SendAsync(e, "我的状态", reply);
                    return result;
                }

                if (e.Detail == "小队状态" || e.Detail == "我的小队状态")
                {
                    e.UseNotice = false;
                    BotReply reply = Controller.ShowMainCharacterOrSquadStatus(uid, true);
                    await SendAsync(e, "我的小队状态", reply);
                    return result;
                }

                if (e.Detail == "我的小队")
                {
                    e.UseNotice = false;
                    await SendAsync(e, "我的小队", Controller.ShowSquad(uid));
                    return result;
                }

                if (e.Detail == "清空小队")
                {
                    BotReply reply = Controller.ClearSquad(uid);
                    await SendAsync(e, "清空小队", reply);
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
                    BotReply reply = Controller.RestoreSaved(uid);
                    await SendAsync(e, "还原存档", reply);
                    return result;
                }

                if (e.Detail == "生成自建角色")
                {
                    BotReply reply = Controller.NewCustomCharacter(uid);
                    await SendAsync(e, "生成自建角色", reply);
                    return result;
                }

                if (e.Detail == "角色改名")
                {
                    e.UseNotice = false;
                    BotReply rpy = "为防止玩家手误更改自己的昵称，请在该指令前添加【确认】二字，即使用【确认角色改名】指令。";
                    rpy.Keyboard = new KeyboardMessage().AppendButtons(1, Button.CreateCmdButton("确认", "确认角色改名", false));
                    await SendAsync(e, "改名", rpy);
                    return result;
                }

                if (e.Detail == "确认角色改名")
                {
                    BotReply rpy = Controller.ReName(uid);
                    rpy.Keyboard = new KeyboardMessage().AppendButtons(1, Button.CreateCmdButton("查询改名", "查询改名"));
                    await SendAsync(e, "改名", rpy);
                    return result;
                }

                if (e.Detail == "查询改名")
                {
                    BotReply reply = Controller.GetReNameInfo(uid);
                    await SendAsync(e, "查询改名", reply);
                    return result;
                }

                if (e.Detail.StartsWith("改名审核"))
                {
                    string detail = e.Detail.Replace("改名审核", "").Trim();
                    BotReply reply = "";
                    if (int.TryParse(detail, out int page))
                    {
                        reply = Controller.GetReNameExamines(uid, page);
                    }
                    else
                    {
                        reply = Controller.GetReNameExamines(uid);
                    }
                    await SendAsync(e, "改名审核", reply);
                    return result;
                }

                if (e.Detail.StartsWith("改名拒绝"))
                {
                    string detail = e.Detail.Replace("改名拒绝", "").Trim();
                    BotReply reply = "";
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
                            reply = Controller.ApproveReName(uid, target, false, false, reason);
                        }
                        await SendAsync(e, "改名拒绝", reply);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("改名批准"))
                {
                    string detail = e.Detail.Replace("改名批准", "").Trim();
                    BotReply reply = "";
                    if (long.TryParse(detail, out long target))
                    {
                        reply = Controller.ApproveReName(uid, target);
                    }
                    await SendAsync(e, "改名批准", reply);
                    return result;
                }

                if (e.Detail.StartsWith("社团改名拒绝"))
                {
                    string detail = e.Detail.Replace("社团改名拒绝", "").Trim();
                    BotReply reply = "";
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
                            reply = Controller.ApproveReName(uid, target, false, true, reason);
                        }
                        await SendAsync(e, "社团改名拒绝", reply);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("社团改名批准"))
                {
                    string detail = e.Detail.Replace("社团改名批准", "").Trim();
                    BotReply reply = "";
                    if (long.TryParse(detail, out long target))
                    {
                        reply = Controller.ApproveReName(uid, target, isClub: true);
                    }
                    await SendAsync(e, "社团改名批准", reply);
                    return result;
                }

                if (e.Detail.StartsWith("自定义改名"))
                {
                    e.UseNotice = false;
                    BotReply rpy = "自定义改名说明：自定义改名需要库存中存在至少一张未上锁、且未处于交易、市场出售状态的改名卡，提交改名申请后需要等待审核。" +
                        "在审核期间，改名卡将会被系统锁定，无法取消、重复提交申请，也不能解锁、分解、交易、出售该改名卡。如已知悉请在该指令前添加【确认】二字，即使用【确认自定义改名】指令。";
                    rpy.Keyboard = new KeyboardMessage().AppendButtons(1, Button.CreateCmdButton("确认", "确认自定义改名", false));
                    await SendAsync(e, "改名", rpy);
                    return result;
                }

                if (e.Detail.StartsWith("确认自定义改名"))
                {
                    e.UseNotice = false;
                    string detail = e.Detail.Replace("确认自定义改名", "").Trim();
                    BotReply reply = Controller.ReName_Custom(uid, detail);
                    await SendAsync(e, "改名", reply);
                    return result;
                }

                if (e.Detail == "角色重随")
                {
                    BotReply rpy = Controller.RandomCustomCharacter(uid, false);
                    rpy.Keyboard = new KeyboardMessage().AppendButtons(2, [
                        Button.CreateCmdButton("取消角色重随", "取消角色重随", false, style: 0),
                        Button.CreateCmdButton("确认角色重随", "确认角色重随", false),
                    ]);
                    await SendAsync(e, "角色重随", rpy);
                    return result;
                }

                if (e.Detail == "确认角色重随")
                {
                    BotReply reply = Controller.RandomCustomCharacter(uid, true);
                    await SendAsync(e, "角色重随", reply);
                    return result;
                }

                if (e.Detail == "取消角色重随")
                {
                    BotReply rpy = Controller.CancelRandomCustomCharacter(uid);
                    rpy.Keyboard = new KeyboardMessage().AppendButtons(1, Button.CreateCmdButton("角色重随", "角色重随", false));
                    await SendAsync(e, "角色重随", rpy);
                    return result;
                }

                if (e.Detail == "抽卡")
                {
                    List<string> msgs = Controller.DrawCard(uid);
                    if (msgs.Count > 0)
                    {
                        BotReply rpy = MergeToMarkdown("抽卡结果如下：", msgs);
                        rpy.Keyboard = new KeyboardMessage().AppendButtons(2, [
                            Button.CreateCmdButton("抽卡", "抽卡"),
                            Button.CreateCmdButton("十连抽卡", "十连抽卡"),
                        ]).AppendButtonsWithNewRow(2, [
                            Button.CreateCmdButton("钻石抽卡", "钻石抽卡"),
                            Button.CreateCmdButton("钻石十连抽卡", "钻石十连抽卡"),
                        ]);
                        await SendAsync(e, "抽卡", rpy);
                    }
                    return result;
                }

                if (e.Detail == "十连抽卡")
                {
                    List<string> msgs = Controller.DrawCards(uid);
                    if (msgs.Count > 0)
                    {
                        BotReply rpy = MergeToMarkdown("抽卡结果如下：", msgs);
                        rpy.Keyboard = new KeyboardMessage().AppendButtons(2, [
                            Button.CreateCmdButton("抽卡", "抽卡"),
                            Button.CreateCmdButton("十连抽卡", "十连抽卡"),
                        ]).AppendButtonsWithNewRow(2, [
                            Button.CreateCmdButton("钻石抽卡", "钻石抽卡"),
                            Button.CreateCmdButton("钻石十连抽卡", "钻石十连抽卡"),
                        ]);
                        await SendAsync(e, "十连抽卡", rpy);
                    }
                    return result;
                }

                if (e.Detail == "钻石抽卡")
                {
                    List<string> msgs = Controller.DrawCard_Material(uid);
                    if (msgs.Count > 0)
                    {
                        BotReply rpy = MergeToMarkdown("抽卡结果如下：", msgs);
                        rpy.Keyboard = new KeyboardMessage().AppendButtons(2, [
                            Button.CreateCmdButton("抽卡", "抽卡"),
                            Button.CreateCmdButton("十连抽卡", "十连抽卡"),
                        ]).AppendButtonsWithNewRow(2, [
                            Button.CreateCmdButton("钻石抽卡", "钻石抽卡"),
                            Button.CreateCmdButton("钻石十连抽卡", "钻石十连抽卡"),
                        ]);
                        await SendAsync(e, "钻石抽卡", rpy);
                    }
                    return result;
                }

                if (e.Detail == "钻石十连抽卡")
                {
                    List<string> msgs = Controller.DrawCards_Material(uid);
                    if (msgs.Count > 0)
                    {
                        BotReply rpy = MergeToMarkdown("抽卡结果如下：", msgs);
                        rpy.Keyboard = new KeyboardMessage().AppendButtons(2, [
                            Button.CreateCmdButton("抽卡", "抽卡"),
                            Button.CreateCmdButton("十连抽卡", "十连抽卡"),
                        ]).AppendButtonsWithNewRow(2, [
                            Button.CreateCmdButton("钻石抽卡", "钻石抽卡"),
                            Button.CreateCmdButton("钻石十连抽卡", "钻石十连抽卡"),
                        ]);
                        await SendAsync(e, "钻石十连抽卡", rpy);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("查看库存") || e.Detail.StartsWith("我的库存") || e.Detail.StartsWith("我的背包"))
                {
                    string detail = e.Detail.Replace("查看库存", "").Replace("我的库存", "").Replace("我的背包", "").Trim();
                    BotReply reply;
                    if (int.TryParse(detail, out int page))
                    {
                        reply = Controller.GetInventoryInfo2(uid, page, command: "我的库存");
                    }
                    else if (FunGameItemType.FirstOrDefault(detail.Contains) is string matchedType)
                    {
                        int typeIndex = FunGameItemType.IndexOf(matchedType);
                        string remain = detail.Replace(matchedType, "").Trim();
                        if (int.TryParse(remain, out page))
                        {
                            reply = Controller.GetInventoryInfo4(uid, page, typeIndex, command: "我的库存");
                        }
                        else
                        {
                            reply = Controller.GetInventoryInfo4(uid, 1, typeIndex, command: "我的库存");
                        }
                    }
                    else
                    {
                        reply = Controller.GetInventoryInfo2(uid, 1, command: "我的库存");
                    }
                    await SendAsync(e, "查看库存", reply);
                    return result;
                }

                if (e.Detail.StartsWith("物品库存"))
                {
                    string detail = e.Detail.Replace("物品库存", "").Trim();
                    BotReply reply;
                    if (int.TryParse(detail, out int page))
                    {
                        reply = Controller.GetInventoryInfo3(uid, page, 2, 2, command: "物品库存");
                    }
                    else
                    {
                        reply = Controller.GetInventoryInfo3(uid, 1, 2, 2, command: "物品库存");
                    }
                    await SendAsync(e, "查看分类库存", reply);
                    return result;
                }

                if (e.Detail.StartsWith("角色库存"))
                {
                    string detail = e.Detail.Replace("角色库存", "").Trim();
                    BotReply reply;
                    if (int.TryParse(detail, out int page))
                    {
                        reply = Controller.GetInventoryInfo5(uid, page, command: "角色库存");
                    }
                    else
                    {
                        reply = Controller.GetInventoryInfo5(uid, 1, command: "角色库存");
                    }
                    await SendAsync(e, "查看角色库存", reply);
                    return result;
                }

                if (e.Detail.StartsWith("分类库存"))
                {
                    string detail = e.Detail.Replace("分类库存", "").Trim();
                    string[] strings = detail.Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    int t = -1;
                    if (strings.Length > 0 && int.TryParse(strings[0].Trim(), out t))
                    {
                        BotReply reply;
                        if (strings.Length > 1 && int.TryParse(strings[1].Trim(), out int page))
                        {
                            reply = Controller.GetInventoryInfo4(uid, page, t, command: "分类库存");
                        }
                        else
                        {
                            reply = Controller.GetInventoryInfo4(uid, 1, t, command: "分类库存");
                        }
                        await SendAsync(e, "查看分类库存", reply);
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
                    BotReply reply = Controller.GetInventoryInfo6(uid, page, search, false, command: "库存搜索2 ");
                    await SendAsync(e, "搜索库存物品（带描述）", reply);
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
                    BotReply reply = Controller.GetInventoryInfo6(uid, page, search, true, command: "库存搜索");
                    await SendAsync(e, "搜索库存物品", reply);
                    return result;
                }

                if (e.Detail.StartsWith("我角色", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("我角色", "").Trim();
                    BotReply reply = "";
                    if (int.TryParse(detail, out int seq))
                    {
                        reply = Controller.GetCharacterInfoFromInventory(uid, seq, true);
                    }
                    else
                    {
                        reply = Controller.GetCharacterInfoFromInventory(uid, 1, true);
                    }
                    await SendAsync(e, "查库存角色", reply);
                    return result;
                }

                if (e.Detail.StartsWith("我的角色", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("我的角色", "").Trim();
                    BotReply reply = "";
                    if (int.TryParse(detail, out int seq))
                    {
                        reply = Controller.GetCharacterInfoFromInventory(uid, seq, false);
                    }
                    else
                    {
                        reply = Controller.GetCharacterInfoFromInventory(uid, 1, false);
                    }
                    await SendAsync(e, "查库存角色", reply);
                    return result;
                }

                if (e.Detail.StartsWith("角色技能", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("角色技能", "").Trim();
                    BotReply reply = "";
                    if (int.TryParse(detail, out int seq))
                    {
                        reply = Controller.GetCharacterSkills(uid, seq);
                    }
                    else
                    {
                        reply = Controller.GetCharacterSkills(uid, 1);
                    }
                    await SendAsync(e, "角色技能", reply);
                    return result;
                }

                if (e.Detail.StartsWith("角色物品", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("角色物品", "").Trim();
                    BotReply reply = "";
                    if (int.TryParse(detail, out int seq))
                    {
                        reply = Controller.GetCharacterItems(uid, seq);
                    }
                    else
                    {
                        reply = Controller.GetCharacterItems(uid, 1);
                    }
                    await SendAsync(e, "角色物品", reply);
                    return result;
                }

                if (e.Detail.StartsWith("设置主战", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("设置主战", "").Trim();
                    BotReply reply = "";
                    if (int.TryParse(detail, out int cid))
                    {
                        reply = Controller.SetMain(uid, cid);
                    }
                    else
                    {
                        reply = Controller.SetMain(uid, 1);
                    }
                    await SendAsync(e, "设置主战角色", reply);
                    return result;
                }

                if (e.Detail.StartsWith("开启练级", StringComparison.CurrentCultureIgnoreCase) || e.Detail.StartsWith("开始练级", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("开启练级", "").Replace("开始练级", "").Trim();
                    BotReply reply = "";
                    if (int.TryParse(detail, out int cid))
                    {
                        reply = Controller.StartTraining(uid, cid);
                    }
                    else
                    {
                        reply = Controller.StartTraining(uid, 1);
                    }
                    await SendAsync(e, "开启练级", reply);
                    return result;
                }

                if (e.Detail == "练级信息")
                {
                    BotReply reply = Controller.GetTrainingInfo(uid);
                    await SendAsync(e, "练级信息", reply);
                    return result;
                }

                if (e.Detail == "练级结算")
                {
                    BotReply reply = Controller.StopTraining(uid);
                    await SendAsync(e, "练级结算", reply);
                    return result;
                }

                if (e.Detail == "任务列表")
                {
                    BotReply rpy = Controller.CheckQuestList(uid);
                    rpy.Keyboard = new KeyboardMessage().AppendButtons(1, Button.CreateCmdButton("做任务", "做任务", false));
                    await SendAsync(e, "任务列表", rpy);
                    return result;
                }

                if (e.Detail == "任务信息")
                {
                    BotReply reply = Controller.CheckWorkingQuest(uid);
                    await SendAsync(e, "任务信息", reply);
                    return result;
                }

                if (e.Detail == "任务结算")
                {
                    BotReply reply = Controller.SettleQuest(uid);
                    await SendAsync(e, "任务结算", reply);
                    return result;
                }

                if (e.Detail == "签到")
                {
                    BotReply reply = Controller.SignIn(uid);
                    reply.Keyboard = new KeyboardMessage().AppendButtons(1, Button.CreateCmdButton("帮助", "帮助"));
                    await SendAsync(e, "签到", reply);
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
                    BotReply reply;
                    if (int.TryParse(detail, out int index))
                    {
                        reply = Controller.GetItemInfoFromInventory(uid, index);
                        await SendAsync(e, "查库存物品", reply);
                        return result;
                    }
                    reply = Controller.GetItemInfoFromInventory_Name(uid, detail);
                    await SendAsync(e, "查库存物品", reply);
                    return result;
                }

                if (e.Detail.StartsWith("兑换金币", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("兑换金币", "").Trim();
                    if (int.TryParse(detail, out int materials))
                    {
                        BotReply rpy = Controller.ExchangeCredits(uid, materials);
                        rpy.Keyboard = new KeyboardMessage().AppendButtons(1, Button.CreateCmdButton("继续兑换", "兑换金币", false));
                        await SendAsync(e, "兑换金币", rpy);
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
                            BotReply reply = Controller.UnEquipItem(uid, c, i);
                            await SendAsync(e, "取消装备", reply);
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
                            BotReply reply = Controller.EquipItem(uid, c, i);
                            await SendAsync(e, "装备", reply);
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
                            BotReply reply = Controller.GetSkillLevelUpNeedy(uid, c, s);
                            await SendAsync(e, "查看技能升级", reply);
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
                            BotReply reply = Controller.SkillLevelUp(uid, c, s);
                            await SendAsync(e, "技能升级", reply);
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
                            BotReply reply = Controller.ConflateMagicCardPack(uid, [id1, id2, id3]);
                            await SendAsync(e, "合成魔法卡", reply);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("角色升级", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("角色升级", "").Trim();
                    BotReply reply = "";
                    if (int.TryParse(detail, out int cid))
                    {
                        reply = Controller.CharacterLevelUp(uid, cid);
                    }
                    else
                    {
                        reply = Controller.CharacterLevelUp(uid, 1);
                    }
                    await SendAsync(e, "角色升级", reply);
                    return result;
                }

                if (e.Detail.StartsWith("查看普攻升级", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("查看普攻升级", "").Trim();
                    BotReply reply = "";
                    if (int.TryParse(detail, out int cid))
                    {
                        reply = Controller.GetNormalAttackLevelUpNeedy(uid, cid);
                    }
                    else
                    {
                        reply = Controller.GetNormalAttackLevelUpNeedy(uid, 1);
                    }
                    await SendAsync(e, "查看普攻升级", reply);
                    return result;
                }

                if (e.Detail.StartsWith("普攻升级", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("普攻升级", "").Trim();
                    BotReply reply = "";
                    if (int.TryParse(detail, out int cid))
                    {
                        reply = Controller.NormalAttackLevelUp(uid, cid);
                    }
                    else
                    {
                        reply = Controller.NormalAttackLevelUp(uid, 1);
                    }
                    await SendAsync(e, "普攻升级", reply);
                    return result;
                }

                if (e.Detail.StartsWith("角色突破", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("角色突破", "").Trim();
                    BotReply reply = "";
                    if (int.TryParse(detail, out int cid))
                    {
                        reply = Controller.CharacterLevelBreak(uid, cid);
                    }
                    else
                    {
                        reply = Controller.CharacterLevelBreak(uid, 1);
                    }
                    await SendAsync(e, "角色突破", reply);
                    return result;
                }

                if (e.Detail.StartsWith("突破信息", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("突破信息", "").Trim();
                    BotReply reply = "";
                    if (int.TryParse(detail, out int cid))
                    {
                        reply = Controller.GetLevelBreakNeedy(uid, cid);
                    }
                    else
                    {
                        reply = Controller.GetLevelBreakNeedy(uid, 1);
                    }
                    await SendAsync(e, "突破信息", reply);
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
                                BotReply reply = Controller.UseItem4(uid, (itemIds, characterIds));
                                await SendAsync(e, "批量使用", reply);
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
                                    BotReply reply = Controller.UseItem3(uid, id, targetId, isCharacter);
                                    await SendAsync(e, "使用魔法卡", reply);
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
                                BotReply reply = Controller.UseItem(uid, id, count, characterIds);
                                await SendAsync(e, "使用", reply);
                            }
                            else if (!string.IsNullOrEmpty(itemPart))
                            {
                                BotReply reply = Controller.UseItem2(uid, itemPart, count, characterIds);
                                await SendAsync(e, "使用", reply);
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
                    BotReply reply = Controller.DecomposeItem(uid, [.. ids]);
                    await SendAsync(e, "分解物品", reply);

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
                            BotReply reply = Controller.DecomposeItem2(uid, itemName, count, true);
                            await SendAsync(e, "分解", reply);
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
                            BotReply reply = Controller.DecomposeItem2(uid, itemName, count);
                            await SendAsync(e, "分解", reply);
                        }
                    }

                    return result;
                }

                if (e.Detail.StartsWith("品质分解", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("品质分解", "").Trim();
                    if (int.TryParse(detail, out int q))
                    {
                        BotReply reply = Controller.DecomposeItem3(uid, q);
                        await SendAsync(e, "品质分解", reply);
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
                            BotReply reply = Controller.CreateItem(uid, name, count, userid);
                            await SendAsync(e, "熟圣之力", reply);
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
                    BotReply rpy = MergeToMarkdown("战斗结果如下：", msgs);
                    rpy.Keyboard = new KeyboardMessage().AppendButtons(1, Button.CreateCmdButton("再次决斗", $"完整决斗{eqq}"));
                    await SendAsync(e, "完整决斗", rpy, msgSeq: 1);
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
                    BotReply rpy = MergeToMarkdown("战斗结果如下：", msgs);
                    rpy.Keyboard = new KeyboardMessage().AppendButtons(1, Button.CreateCmdButton("再次决斗", $"决斗{eqq}"));
                    await SendAsync(e, "决斗", rpy, msgSeq: 1);
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
                    BotReply rpy = MergeToMarkdown("战斗结果如下：", msgs);
                    rpy.Keyboard = new KeyboardMessage().AppendButtons(1, Button.CreateCmdButton("再次决斗", $"小队决斗{eqq}"));
                    await SendAsync(e, "小队决斗", rpy, msgSeq: 1);
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
                        await SendAsync(e, "BOSS", await Controller.FightBossTeam(uid, index, true), msgSeq: 1);
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
                        BotReply rpy = MergeToMarkdown("战斗结果如下：", msgs);
                        rpy.Keyboard = new KeyboardMessage().AppendButtons(1, Button.CreateCmdButton("讨伐boss", $"讨伐boss"));
                        await SendAsync(e, "BOSS", rpy, msgSeq: 1);
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
                        await SendAsync(e, "小队", Controller.AddSquad(uid, c));
                    }
                    return result;
                }

                if (e.Detail.StartsWith("小队移除", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("小队移除", "").Trim();
                    if (int.TryParse(detail, out int c))
                    {
                        await SendAsync(e, "小队", Controller.RemoveSquad(uid, c));
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
                    await SendAsync(e, "小队", Controller.SetSquad(uid, [.. cindexs]));
                    return result;
                }

                if (e.Detail.StartsWith("加入社团", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("加入社团", "").Trim();
                    if (int.TryParse(detail, out int c))
                    {
                        BotReply reply = Controller.ClubJoin(uid, c);
                        await SendAsync(e, "社团", reply);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("邀请加入", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("邀请加入", "").Trim();
                    if (int.TryParse(detail, out int id))
                    {
                        BotReply reply = Controller.ClubInvite(uid, id);
                        await SendAsync(e, "社团", reply);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("取消邀请加入", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("取消邀请加入", "").Trim();
                    if (int.TryParse(detail, out int id))
                    {
                        BotReply reply = Controller.ClubInvite(uid, id, true);
                        await SendAsync(e, "社团", reply);
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
                    BotReply reply = Controller.ClubCreate(uid, isPublic, detail);
                    await SendAsync(e, "社团", reply);
                    return result;
                }

                if (e.Detail == "退出社团")
                {
                    BotReply reply = Controller.ClubQuit(uid);
                    await SendAsync(e, "社团", reply);
                    return result;
                }

                if (e.Detail == "我的社团")
                {
                    BotReply reply = Controller.ClubShowInfo(uid);
                    await SendAsync(e, "社团", reply);
                    return result;
                }

                if (e.Detail.StartsWith("社团列表"))
                {
                    string detail = e.Detail.Replace("社团列表", "").Trim();
                    BotReply reply = "";
                    if (int.TryParse(detail, out int page))
                    {
                        reply = Controller.ClubShowList(uid, page);
                    }
                    else
                    {
                        reply = Controller.ClubShowList(uid, 1);
                    }
                    await SendAsync(e, "社团", reply);
                    return result;
                }

                if (e.Detail == "解散社团")
                {
                    BotReply reply = Controller.ClubDisband(uid);
                    await SendAsync(e, "社团", reply);
                    return result;
                }

                if (e.Detail.StartsWith("查看社团成员"))
                {
                    string detail = e.Detail.Replace("查看社团成员", "").Trim();
                    if (int.TryParse(detail, out int page) && page > 0)
                    {
                        BotReply reply = Controller.ClubShowMemberList(uid, 0, page);
                        await SendAsync(e, "社团", reply);
                    }
                    else
                    {
                        BotReply reply = Controller.ClubShowMemberList(uid, 0, 1);
                        await SendAsync(e, "社团", reply);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("查看社团管理"))
                {
                    string detail = e.Detail.Replace("查看社团管理", "").Trim();
                    if (int.TryParse(detail, out int page) && page > 0)
                    {
                        BotReply reply = Controller.ClubShowMemberList(uid, 1, page);
                        await SendAsync(e, "社团", reply);
                    }
                    else
                    {
                        BotReply reply = Controller.ClubShowMemberList(uid, 1, 1);
                        await SendAsync(e, "社团", reply);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("查看申请人列表"))
                {
                    string detail = e.Detail.Replace("查看申请人列表", "").Trim();
                    if (int.TryParse(detail, out int page) && page > 0)
                    {
                        BotReply reply = Controller.ClubShowMemberList(uid, 2, page);
                        await SendAsync(e, "社团", reply);
                    }
                    else
                    {
                        BotReply reply = Controller.ClubShowMemberList(uid, 2, 1);
                        await SendAsync(e, "社团", reply);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("查看受邀人列表"))
                {
                    string detail = e.Detail.Replace("查看受邀人列表", "").Trim();
                    if (int.TryParse(detail, out int page) && page > 0)
                    {
                        BotReply reply = Controller.ClubShowMemberList(uid, 3, page);
                        await SendAsync(e, "社团", reply);
                    }
                    else
                    {
                        BotReply reply = Controller.ClubShowMemberList(uid, 3, 1);
                        await SendAsync(e, "社团", reply);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("社团批准", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("社团批准", "").Replace("@", "").Trim();
                    if (long.TryParse(detail, out long id))
                    {
                        BotReply reply = Controller.ClubApprove(uid, id, true);
                        await SendAsync(e, "社团", reply);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("社团拒绝", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("社团拒绝", "").Replace("@", "").Trim();
                    if (long.TryParse(detail, out long id))
                    {
                        BotReply reply = Controller.ClubApprove(uid, id, false);
                        await SendAsync(e, "社团", reply);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("社团踢出", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("社团踢出", "").Replace("@", "").Trim();
                    if (long.TryParse(detail, out long id))
                    {
                        BotReply reply = Controller.ClubKick(uid, id);
                        await SendAsync(e, "社团", reply);
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
                        BotReply reply = Controller.ClubChange(uid, part, [.. args]);
                        await SendAsync(e, "社团", reply);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("社团转让", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("社团转让", "").Replace("@", "").Trim();
                    List<string> args = [detail];
                    BotReply reply = Controller.ClubChange(uid, "setmaster", [.. args]);
                    await SendAsync(e, "社团", reply);
                    return result;
                }

                if (e.Detail.StartsWith("社团捐献"))
                {
                    string detail = e.Detail.Replace("社团捐献", "").Trim();
                    BotReply reply = "";
                    if (double.TryParse(detail, out double credits))
                    {
                        reply = Controller.ClubContribution(uid, credits);
                    }
                    await SendAsync(e, "社团", reply);
                    return result;
                }

                if (e.Detail == "每日商店")
                {
                    BotReply rpy = Controller.ShowDailyStore(uid);
                    rpy.Keyboard = new KeyboardMessage().AppendButtons(4, [
                        Button.CreateCmdButton("查看1", "商店查看 1", false),
                        Button.CreateCmdButton("查看2", "商店查看 2", false),
                        Button.CreateCmdButton("查看3", "商店查看 3", false),
                        Button.CreateCmdButton("查看4", "商店查看 4", false),
                        Button.CreateCmdButton("购买1", "商店购买 1", false),
                        Button.CreateCmdButton("购买2", "商店购买 2", false),
                        Button.CreateCmdButton("购买3", "商店购买 3", false),
                        Button.CreateCmdButton("购买4", "商店购买 4", false)
                    ]);
                    await SendAsync(e, "商店", rpy);
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
                        BotReply reply = "";
                        if (model.IsDaily)
                        {
                            reply = Controller.DailyStoreBuy(uid, id, count);
                        }
                        else
                        {
                            reply = Controller.SystemStoreBuy(uid, model.StoreRegion, model.StoreName, id, count);
                        }
                        await SendAsync(e, "商店购买", reply);
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
                        BotReply reply = "";
                        if (model.IsDaily)
                        {
                            reply = Controller.DailyStoreShowInfo(uid, id);
                        }
                        else
                        {
                            reply = Controller.SystemStoreShowInfo(uid, model.StoreRegion, model.StoreName, id);
                        }
                        reply.Keyboard = new KeyboardMessage().AppendButtons(1, Button.CreateCmdButton("购买此商品", $"商店购买 {id}", false));
                        await SendAsync(e, "商店", reply);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("添加商品", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail["添加商品".Length..].Trim();

                    // 匹配必需参数：地区ID（数字），商店名称（可带双引号），物品名称（可带双引号），以及剩余可选参数部分
                    string pattern = @"^(?<region>\d+)\s+(?<storeName>(""[^""]*""|\S+))\s+(?<itemName>(""[^""]*""|\S+))\s*(?<options>.*)$";
                    Match match = Regex.Match(detail, pattern);

                    if (match.Success)
                    {
                        long region = long.Parse(match.Groups["region"].Value);
                        string storeName = match.Groups["storeName"].Value.Trim('"');
                        string itemName = match.Groups["itemName"].Value.Trim('"');
                        string optionsStr = match.Groups["options"].Value.Trim();

                        // 默认值
                        double price = 0;
                        int stock = -1;
                        int quota = 0;
                        bool addToNextRefreshGoods = true;

                        // 解析可选键值对
                        if (!string.IsNullOrEmpty(optionsStr))
                        {
                            string optionPattern = @"(?<key>\w+)=(?<value>[^\s]+)";
                            foreach (Match opt in Regex.Matches(optionsStr, optionPattern))
                            {
                                string key = opt.Groups["key"].Value.ToLower();
                                string val = opt.Groups["value"].Value;
                                switch (key)
                                {
                                    case "price":
                                        _ = double.TryParse(val, out price);
                                        break;
                                    case "stock":
                                        _ = int.TryParse(val, out stock);
                                        break;
                                    case "quota":
                                        _ = int.TryParse(val, out quota);
                                        break;
                                    case "refresh":
                                        // 支持 是/否 或 true/false 或 1/0
                                        addToNextRefreshGoods = val == "1" ||
                                            bool.TryParse(val, out bool b) && b ||
                                            val.Equals("是", StringComparison.OrdinalIgnoreCase) ||
                                            val.Equals("yes", StringComparison.OrdinalIgnoreCase);
                                        break;
                                }
                            }
                        }

                        BotReply reply = Controller.SystemStoreAddGoods(uid, region, storeName, itemName, price, stock, quota, addToNextRefreshGoods);
                        await SendAsync(e, "添加商品", reply);
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
                    await SendAsync(e, "探索信息", Controller.GetExploreInfo(uid));
                    return result;
                }

                if (e.Detail == "探索结算")
                {
                    BotReply reply = Controller.SettleExploreAll(uid);
                    await SendAsync(e, "探索结算", reply);
                    return result;
                }

                if (e.Detail.StartsWith("探索") || e.Detail.StartsWith("前往"))
                {
                    string originalDetail = e.Detail;
                    string detail = e.Detail.Replace("探索", "").Replace("前往", "").Trim();
                    BotReply reply = new();
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
                        (reply, eid) = Controller.ExploreRegion(uid, cindexs[0], false, [.. cindexs.Skip(1).Select(id => (long)id)]);
                        await SendAsync(e, "探索", reply);
                        _ = Task.Run(async () =>
                        {
                            await Task.Delay(FunGameConstant.ExploreTime * 60 * 1000);
                            BotReply rpy = Controller.SettleExplore(eid, uid, originalDetail);
                            await SendAsync(e, "探索", rpy, msgSeq: 2);
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
                    string originalDetail = e.Detail;
                    string detail = e.Detail.Replace("小队探索", "").Trim();
                    BotReply reply = "";
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
                        (reply, eid) = Controller.ExploreRegion(uid, cindexs[0], true);
                        await SendAsync(e, "探索", reply);
                        _ = Task.Run(async () =>
                        {
                            await Task.Delay(FunGameConstant.ExploreTime * 60 * 1000);
                            BotReply rpy = Controller.SettleExplore(eid, uid, originalDetail);
                            await SendAsync(e, "探索", rpy, msgSeq: 2);
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
                    BotReply reply = Controller.SpringOfLife(uid);
                    await SendAsync(e, "生命之泉", reply);
                    return result;
                }

                if (e.Detail == "酒馆" || e.Detail == "上酒")
                {
                    BotReply reply = Controller.Pub(uid);
                    await SendAsync(e, "酒馆", reply);
                    return result;
                }

                if (e.Detail == "毕业礼包")
                {
                    if (FunGameService.Activities.FirstOrDefault(a => a.Name == "毕业季") is Activity activity && activity.Status == ActivityState.InProgress)
                    {
                        BotReply reply = Controller.CreateGiftBox(uid, "毕业礼包", true, 2);
                        await SendAsync(e, "毕业礼包", reply);
                    }
                    else
                    {
                        await SendAsync(e, "毕业礼包", "活动不存在或已过期。");
                    }
                    return result;
                }

                if (e.Detail == "活动" || e.Detail == "活动中心")
                {
                    BotReply reply = Controller.GetEvents(uid);
                    await SendAsync(e, "活动中心", reply);
                    return result;
                }

                if (e.Detail.StartsWith("查活动"))
                {
                    string detail = e.Detail.Replace("查活动", "").Trim();
                    BotReply reply = "";
                    if (int.TryParse(detail, out int eid))
                    {
                        reply = Controller.GetEvents(uid, eid);
                        await SendAsync(e, "查活动", reply);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("做活动任务", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("做活动任务", "").Trim();
                    string[] strings = detail.Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    int aid = -1, qid = -1;
                    if (strings.Length > 0 && int.TryParse(strings[0].Trim(), out aid) && strings.Length > 1 && int.TryParse(strings[1].Trim(), out qid))
                    {
                        if (aid != -1 && qid != -1)
                        {
                            BotReply reply = Controller.PerformEvent(uid, aid, qid);
                            await SendAsync(e, "做活动任务", reply);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("领取奖励", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("领取奖励", "").Trim();
                    string[] strings = detail.Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    int aid = -1, qid = -1;
                    if (strings.Length > 0 && int.TryParse(strings[0].Trim(), out aid) && strings.Length > 1 && int.TryParse(strings[1].Trim(), out qid))
                    {
                        if (aid != -1 && qid != -1)
                        {
                            BotReply reply = Controller.ClaimEventPrize(uid, aid, qid);
                            await SendAsync(e, "领取奖励", reply);
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
                            BotReply reply = Controller.LockItems(uid, itemName, count, false);
                            await SendAsync(e, "批量锁定", reply);
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
                            BotReply reply = Controller.LockItems(uid, itemName, count, true);
                            await SendAsync(e, "批量解锁", reply);
                        }
                    }

                    return result;
                }

                if (e.Detail.StartsWith("上锁") || e.Detail.StartsWith("锁定"))
                {
                    string detail = e.Detail.Replace("上锁", "").Replace("锁定", "").Trim();
                    BotReply reply = "";
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
                        reply = Controller.LockItem(uid, false, [.. indexs]);
                        await SendAsync(e, "上锁", reply);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("解锁"))
                {
                    string detail = e.Detail.Replace("解锁", "").Trim();
                    BotReply reply = "";
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
                        reply = Controller.LockItem(uid, true, [.. indexs]);
                        await SendAsync(e, "解锁", reply);
                    }
                    return result;
                }

                if (e.Detail.StartsWith(value: "创建报价"))
                {
                    string detail = e.Detail.Replace("创建报价", "").Trim();
                    BotReply reply = "";
                    if (long.TryParse(detail, out long target))
                    {
                        reply = Controller.MakeOffer(uid, target);
                        await SendAsync(e, "创建报价", reply);
                    }
                    return result;
                }

                if (e.Detail.StartsWith(value: "我的报价"))
                {
                    string detail = e.Detail.Replace("我的报价", "").Trim();
                    BotReply reply = "";
                    if (int.TryParse(detail, out int page))
                    {
                        reply = Controller.GetOffer(uid, null, page);
                    }
                    else
                    {
                        reply = Controller.GetOffer(uid);
                    }
                    await SendAsync(e, "我的报价", reply);
                    return result;
                }

                if (e.Detail.StartsWith(value: "查报价"))
                {
                    string detail = e.Detail.Replace("查报价", "").Trim();
                    BotReply reply = "";
                    if (long.TryParse(detail, out long id))
                    {
                        reply = Controller.GetOffer(uid, id);
                        await SendAsync(e, "查报价", reply);
                    }
                    return result;
                }

                if (e.Detail.StartsWith(value: "发送报价"))
                {
                    string detail = e.Detail.Replace("发送报价", "").Trim();
                    BotReply reply = "";
                    if (long.TryParse(detail, out long id))
                    {
                        reply = Controller.SendOffer(uid, id);
                        await SendAsync(e, "发送报价", reply);
                    }
                    return result;
                }

                if (e.Detail.StartsWith(value: "取消报价"))
                {
                    string detail = e.Detail.Replace("取消报价", "").Trim();
                    BotReply reply = "";
                    if (long.TryParse(detail, out long id))
                    {
                        reply = Controller.CancelOffer(uid, id);
                        await SendAsync(e, "取消报价", reply);
                    }
                    return result;
                }

                if (e.Detail.StartsWith(value: "接受报价"))
                {
                    string detail = e.Detail.Replace("接受报价", "").Trim();
                    BotReply reply = "";
                    if (long.TryParse(detail, out long id))
                    {
                        reply = Controller.RespondOffer(uid, id, true);
                        await SendAsync(e, "接受报价", reply);
                    }
                    return result;
                }

                if (e.Detail.StartsWith(value: "拒绝报价"))
                {
                    string detail = e.Detail.Replace("拒绝报价", "").Trim();
                    BotReply reply = "";
                    if (long.TryParse(detail, out long id))
                    {
                        reply = Controller.RespondOffer(uid, id, false);
                        await SendAsync(e, "拒绝报价", reply);
                    }
                    return result;
                }

                if (e.Detail.StartsWith(value: "报价添加物品"))
                {
                    string detail = e.Detail.Replace("报价添加物品", "").Trim();
                    BotReply reply = "";
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
                        reply = Controller.AddOfferItems(uid, indexs[0], false, [.. indexs[1..]]);
                        await SendAsync(e, "报价添加物品", reply);
                    }
                    else
                    {
                        reply = "格式不正确，请先输入报价序号再输入物品序号，使用空格隔开。";
                    }
                    return result;
                }

                if (e.Detail.StartsWith(value: "报价添加对方物品"))
                {
                    string detail = e.Detail.Replace("报价添加对方物品", "").Trim();
                    BotReply reply = "";
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
                        reply = Controller.AddOfferItems(uid, indexs[0], true, [.. indexs[1..]]);
                        await SendAsync(e, "报价添加对方物品", reply);
                    }
                    else
                    {
                        reply = "格式不正确，请先输入报价序号再输入物品序号，使用空格隔开。";
                    }
                    return result;
                }

                if (e.Detail.StartsWith(value: "报价移除物品"))
                {
                    string detail = e.Detail.Replace("报价移除物品", "").Trim();
                    BotReply reply = "";
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
                        reply = Controller.RemoveOfferItems(uid, indexs[0], false, [.. indexs[1..]]);
                        await SendAsync(e, "报价移除物品", reply);
                    }
                    else
                    {
                        reply = "格式不正确，请先输入报价序号再输入物品序号，使用空格隔开。";
                    }
                    return result;
                }

                if (e.Detail.StartsWith(value: "报价移除对方物品"))
                {
                    string detail = e.Detail.Replace("报价移除对方物品", "").Trim();
                    BotReply reply = "";
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
                        reply = Controller.RemoveOfferItems(uid, indexs[0], true, [.. indexs[1..]]);
                        await SendAsync(e, "报价移除对方物品", reply);
                    }
                    else
                    {
                        reply = "格式不正确，请先输入报价序号再输入物品序号，使用空格隔开。";
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
                    BotReply reply = Controller.StoreSellItem(uid, [.. ids]);
                    await SendAsync(e, "商店出售", reply);

                    return result;
                }

                if (e.Detail.StartsWith("挑战金币秘境"))
                {
                    string detail = e.Detail.Replace("挑战金币秘境", "").Trim();
                    if (int.TryParse(detail, out int diff))
                    {
                        await SendAsync(e, "挑战金币秘境", await Controller.FightInstance(uid, (int)InstanceType.Currency, diff));
                    }
                    else
                    {
                        await SendAsync(e, "挑战秘境", CreateMarkdownFromText("请在" + "指令".CreateCmdInput(e.Detail) + "后面输入难度系数（1-5）"));
                    }
                    return result;
                }

                if (e.Detail.StartsWith("挑战钻石秘境"))
                {
                    string detail = e.Detail.Replace("挑战钻石秘境", "").Trim();
                    if (int.TryParse(detail, out int diff))
                    {
                        await SendAsync(e, "挑战钻石秘境", await Controller.FightInstance(uid, (int)InstanceType.Material, diff));
                    }
                    else
                    {
                        await SendAsync(e, "挑战秘境", CreateMarkdownFromText("请在" + "指令".CreateCmdInput(e.Detail) + "后面输入难度系数（1-5）"));
                    }
                    return result;
                }

                if (e.Detail.StartsWith("挑战经验秘境"))
                {
                    string detail = e.Detail.Replace("挑战经验秘境", "").Trim();
                    if (int.TryParse(detail, out int diff))
                    {
                        await SendAsync(e, "挑战经验秘境", await Controller.FightInstance(uid, (int)InstanceType.EXP, diff));
                    }
                    else
                    {
                        await SendAsync(e, "挑战秘境", CreateMarkdownFromText("请在" + "指令".CreateCmdInput(e.Detail) + "后面输入难度系数（1-5）"));
                    }
                    return result;
                }

                if (e.Detail.StartsWith("挑战地区秘境"))
                {
                    string detail = e.Detail.Replace("挑战地区秘境", "").Trim();
                    if (int.TryParse(detail, out int diff))
                    {
                        await SendAsync(e, "挑战地区秘境", await Controller.FightInstance(uid, (int)InstanceType.RegionItem, diff));
                    }
                    else
                    {
                        await SendAsync(e, "挑战秘境", CreateMarkdownFromText("请在" + "指令".CreateCmdInput(e.Detail) + "后面输入难度系数（1-5）"));
                    }
                    return result;
                }

                if (e.Detail.StartsWith("挑战突破秘境"))
                {
                    string detail = e.Detail.Replace("挑战突破秘境", "").Trim();
                    if (int.TryParse(detail, out int diff))
                    {
                        await SendAsync(e, "挑战突破秘境", await Controller.FightInstance(uid, (int)InstanceType.CharacterLevelBreak, diff));
                    }
                    else
                    {
                        await SendAsync(e, "挑战秘境", CreateMarkdownFromText("请在" + "指令".CreateCmdInput(e.Detail) + "后面输入难度系数（1-5）"));
                    }
                    return result;
                }

                if (e.Detail.StartsWith("挑战技能秘境"))
                {
                    string detail = e.Detail.Replace("挑战技能秘境", "").Trim();
                    if (int.TryParse(detail, out int diff))
                    {
                        await SendAsync(e, "挑战技能秘境", await Controller.FightInstance(uid, (int)InstanceType.SkillLevelUp, diff));
                    }
                    else
                    {
                        await SendAsync(e, "挑战秘境", CreateMarkdownFromText("请在" + "指令".CreateCmdInput(e.Detail) + "后面输入难度系数（1-5）"));
                    }
                    return result;
                }

                if (e.Detail.StartsWith("挑战魔法卡秘境"))
                {
                    string detail = e.Detail.Replace("挑战魔法卡秘境", "").Trim();
                    if (int.TryParse(detail, out int diff))
                    {
                        await SendAsync(e, "挑战魔法卡秘境", await Controller.FightInstance(uid, (int)InstanceType.MagicCard, diff));
                    }
                    else
                    {
                        await SendAsync(e, "挑战秘境", CreateMarkdownFromText("请在" + "指令".CreateCmdInput(e.Detail) + "后面输入难度系数（1-5）"));
                    }
                    return result;
                }

                if (e.Detail == "后勤部")
                {
                    BotReply reply = Controller.ShowSystemStore(uid, "铎京城", "dokyo_logistics");
                    reply.Keyboard = new KeyboardMessage().AppendButtons(2, [
                        Button.CreateCmdButton("商店查看", "商店查看", false),
                        Button.CreateCmdButton("商店购买", "商店购买", false)
                    ]);
                    await SendAsync(e, "商店", reply);
                    return result;
                }

                if (e.Detail == "武器商会")
                {
                    BotReply reply = Controller.ShowSystemStore(uid, "铎京城", "dokyo_weapons");
                    reply.Keyboard = new KeyboardMessage().AppendButtons(2, [
                        Button.CreateCmdButton("商店查看", "商店查看", false),
                        Button.CreateCmdButton("商店购买", "商店购买", false)
                    ]);
                    await SendAsync(e, "商店", reply);
                    return result;
                }

                if (e.Detail == "杂货铺")
                {
                    BotReply reply = Controller.ShowSystemStore(uid, "铎京城", "dokyo_yuki");
                    reply.Keyboard = new KeyboardMessage().AppendButtons(2, [
                        Button.CreateCmdButton("商店查看", "商店查看", false),
                        Button.CreateCmdButton("商店购买", "商店购买", false)
                    ]);
                    await SendAsync(e, "商店", reply);
                    return result;
                }

                if (e.Detail == "基金会")
                {
                    BotReply reply = Controller.ShowSystemStore(uid, "铎京城", "dokyo_welfare");
                    reply.Keyboard = new KeyboardMessage().AppendButtons(2, [
                        Button.CreateCmdButton("商店查看", "商店查看", false),
                        Button.CreateCmdButton("商店购买", "商店购买", false)
                    ]);
                    await SendAsync(e, "商店", reply);
                    return result;
                }

                if (e.Detail == "锻造商店")
                {
                    BotReply reply = Controller.ShowSystemStore(uid, "铎京城", "dokyo_forge");
                    reply.Keyboard = new KeyboardMessage().AppendButtons(2, [
                        Button.CreateCmdButton("商店查看", "商店查看", false),
                        Button.CreateCmdButton("商店购买", "商店购买", false)
                    ]);
                    await SendAsync(e, "商店", reply);
                    return result;
                }

                if (e.Detail == "赛马商店")
                {
                    BotReply reply = Controller.ShowSystemStore(uid, "铎京城", "dokyo_horseracing");
                    reply.Keyboard = new KeyboardMessage().AppendButtons(2, [
                        Button.CreateCmdButton("商店查看", "商店查看", false),
                        Button.CreateCmdButton("商店购买", "商店购买", false)
                    ]);
                    await SendAsync(e, "商店", reply);
                    return result;
                }

                if (e.Detail == "共斗商店")
                {
                    BotReply reply = Controller.ShowSystemStore(uid, "铎京城", "dokyo_cooperative");
                    reply.Keyboard = new KeyboardMessage().AppendButtons(2, [
                        Button.CreateCmdButton("商店查看", "商店查看", false),
                        Button.CreateCmdButton("商店购买", "商店购买", false)
                    ]);
                    await SendAsync(e, "商店", reply);
                    return result;
                }

                if (e.Detail.StartsWith("商店"))
                {
                    string detail = e.Detail.Replace("商店", "").Trim();
                    BotReply reply = "";
                    if (int.TryParse(detail, out int storeId))
                    {
                        switch (storeId)
                        {
                            case 1:
                                reply = Controller.ShowSystemStore(uid, "铎京城", "dokyo_logistics");
                                break;
                            case 2:
                                reply = Controller.ShowSystemStore(uid, "铎京城", "dokyo_weapons");
                                break;
                            case 3:
                                reply = Controller.ShowSystemStore(uid, "铎京城", "dokyo_yuki");
                                break;
                            case 4:
                                reply = Controller.ShowSystemStore(uid, "铎京城", "dokyo_welfare");
                                break;
                            case 5:
                                reply = Controller.ShowSystemStore(uid, "铎京城", "dokyo_forge");
                                break;
                            case 6:
                                reply = Controller.ShowSystemStore(uid, "铎京城", "dokyo_horseracing");
                                break;
                            case 7:
                                reply = Controller.ShowSystemStore(uid, "铎京城", "dokyo_cooperative");
                                break;
                            default:
                                break;
                        }
                        reply.Keyboard = new KeyboardMessage().AppendButtons(2, [
                            Button.CreateCmdButton("商店查看", "商店查看", false),
                            Button.CreateCmdButton("商店购买", "商店购买", false)
                        ]);
                        await SendAsync(e, "商店", reply);
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
                        BotReply reply = new()
                        {
                            Markdown = new() { Content = model.ResultString }
                        };
                        reply = CreateForgeSystemButtons(reply);
                        await SendAsync(e, "模拟锻造配方", reply);
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

                    BotReply reply = Controller.ForgeItem_Create(uid, recipeItems);
                    reply = CreateForgeSystemButtons(reply);
                    await SendAsync(e, "锻造配方", reply);
                    return result;
                }

                if (e.Detail.StartsWith("模拟锻造"))
                {
                    BotReply reply = Controller.ForgeItem_Simulate(uid);
                    reply = CreateForgeSystemButtons(reply);
                    await SendAsync(e, "模拟锻造", reply);
                    return result;
                }

                if (e.Detail.StartsWith("取消锻造"))
                {
                    BotReply reply = Controller.ForgeItem_Cancel(uid);
                    reply = CreateForgeSystemButtons(reply);
                    await SendAsync(e, "取消锻造", reply);
                    return result;
                }

                if (e.Detail.StartsWith("确认开始锻造"))
                {
                    BotReply reply = Controller.ForgeItem_Complete(uid);
                    reply = CreateForgeSystemButtons(reply);
                    await SendAsync(e, "确认开始锻造", reply);
                    return result;
                }

                if (e.Detail.StartsWith("锻造信息"))
                {
                    BotReply reply = Controller.ForgeItem_Info(uid);
                    reply = CreateForgeSystemButtons(reply);
                    await SendAsync(e, "锻造信息", reply);
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
                            BotReply reply = Controller.ForgeItem_Master(uid, r, q);
                            reply = CreateForgeSystemButtons(reply);
                            await SendAsync(e, "大师锻造", reply);
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
                    BotReply reply = Controller.MarketSellItem(uid, price, [.. ids]);
                    await SendAsync(e, "市场出售", reply);

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
                            BotReply reply = Controller.MarketBuyItem(uid, id, count);
                            await SendAsync(e, "市场购买", reply);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("市场查看"))
                {
                    string detail = e.Detail.Replace("市场查看", "").Trim();
                    BotReply reply = "";
                    if (int.TryParse(detail, out int id))
                    {
                        reply = Controller.MarketItemInfo(uid, id);
                        await SendAsync(e, "市场查看", reply);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("市场下架"))
                {
                    string detail = e.Detail.Replace("市场下架", "").Trim();
                    BotReply reply = "";
                    if (int.TryParse(detail, out int id))
                    {
                        reply = Controller.MarketDelistItem(uid, id);
                        await SendAsync(e, "市场下架", reply);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("我的市场"))
                {
                    string detail = e.Detail.Replace("我的市场", "").Trim();
                    BotReply reply = "";
                    if (int.TryParse(detail, out int page))
                    {
                        reply = Controller.MarketShowListMySells(uid, page);
                    }
                    else
                    {
                        reply = Controller.MarketShowListMySells(uid, 1);
                    }
                    await SendAsync(e, "我的市场", reply);
                    return result;
                }

                if (e.Detail.StartsWith("我的购买"))
                {
                    string detail = e.Detail.Replace("我的购买", "").Trim();
                    BotReply reply = "";
                    if (int.TryParse(detail, out int page))
                    {
                        reply = Controller.MarketShowListMyBuys(uid, page);
                    }
                    else
                    {
                        reply = Controller.MarketShowListMyBuys(uid, 1);
                    }
                    await SendAsync(e, "我的购买", reply);
                    return result;
                }

                if (e.Detail.StartsWith("市场"))
                {
                    string detail = e.Detail.Replace("市场", "").Trim();
                    BotReply reply = "";
                    if (int.TryParse(detail, out int page))
                    {
                        reply = Controller.MarketShowList(uid, page);
                    }
                    else
                    {
                        reply = Controller.MarketShowList(uid, 1);
                    }
                    await SendAsync(e, "市场", reply);
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
                    BotReply reply = "";
                    if (useId)
                    {
                        reply = Controller.Chat(uid, id, content);
                    }
                    else
                    {
                        reply = Controller.Chat_Name(uid, name, content);
                    }
                    await SendAsync(e, "私信", reply);
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
                        BotReply reply = Controller.RoomCreate(uid, "horseracing", "", groupId);
                        await SendAsync(e, "赛马", reply);
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
                        BotReply reply = Controller.RoomCreate(uid, "cooperative", "", groupId);
                        await SendAsync(e, "房间", reply);
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
                        BotReply reply = Controller.RoomCreate(uid, "mix", "", groupId);
                        await SendAsync(e, "房间", reply);
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
                        BotReply reply = Controller.RoomCreate(uid, "team", "", groupId);
                        await SendAsync(e, "房间", reply);
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
                            BotReply reply = Controller.RoomInto(uid, room.Roomid, "");
                            await SendAsync(e, "赛马", reply);
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
                        BotReply reply = Controller.RoomCreate(uid, roomType, password, groupId);
                        await SendAsync(e, "房间", reply);
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
                        BotReply reply = Controller.RoomInto(uid, roomid, password);
                        await SendAsync(e, "房间", reply);
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
                        BotReply rpy = MergeToMarkdown("该局游戏结果如下：", msgs);
                        rpy.Keyboard = new KeyboardMessage().AppendButtons(2, [
                            Button.CreateCmdButton("退出房间", $"退出房间", false),
                            Button.CreateCmdButton("房间列表", $"房间列表"),
                            Button.CreateCmdButton("生命之泉", $"生命之泉"),
                            Button.CreateCmdButton("快速重新开始", $"开始游戏")
                        ]);
                        await SendAsync(e, "房间", rpy, msgSeq: 1);
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
                        BotReply reply = Controller.RoomQuit(uid);
                        await SendAsync(e, "房间", reply);
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
                        BotReply reply = Controller.RoomShowList(uid, groupId);
                        await SendAsync(e, "房间", reply);
                    }
                    else
                    {
                        await SendAsync(e, "房间", "请在群聊中进行多人游戏。");
                    }
                    return result;
                }

                if (e.Detail == "房间信息")
                {
                    BotReply reply = Controller.RoomInfo(uid);
                    await SendAsync(e, "房间", reply);
                    return result;
                }

                if (e.Detail == "排行榜" || e.Detail == "养成排行榜")
                {
                    BotReply reply = Controller.GetRanking(uid, 2);
                    await SendAsync(e, "排行榜", reply);
                    return result;
                }

                if (e.Detail == $"{General.GameplayEquilibriumConstant.InGameCurrency}排行榜")
                {
                    BotReply reply = Controller.GetRanking(uid, 0);
                    await SendAsync(e, "排行榜", reply);
                    return result;
                }

                if (e.Detail == $"{General.GameplayEquilibriumConstant.InGameMaterial}排行榜")
                {
                    BotReply reply = Controller.GetRanking(uid, 1);
                    await SendAsync(e, "排行榜", reply);
                    return result;
                }

                if (e.Detail == $"赛马排行榜")
                {
                    BotReply reply = Controller.GetRanking(uid, 3);
                    await SendAsync(e, "排行榜", reply);
                    return result;
                }

                if (e.Detail == $"共斗排行榜")
                {
                    BotReply reply = Controller.GetRanking(uid, 4);
                    await SendAsync(e, "排行榜", reply);
                    return result;
                }

                if (e.Detail == $"锻造排行榜")
                {
                    BotReply reply = Controller.GetRanking(uid, 5);
                    await SendAsync(e, "排行榜", reply);
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
                    BotReply reply = Controller.AddItemsToCharacter(uid, cid, [.. ids]);
                    await SendAsync(e, "加物", reply);

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
                    BotReply reply = Controller.RemoveItemsFromCharacter(uid, cid, [.. ids]);
                    await SendAsync(e, "减物", reply);

                    return result;
                }

                if (await HandleCSBettingInput(e, uid))
                {
                    return true;
                }

                if (uid == GeneralSettings.Master && e.Detail.StartsWith("重载FunGame", StringComparison.CurrentCultureIgnoreCase))
                {
                    BotReply reply = Controller.Relaod(uid);
                    await SendAsync(e, "重载FunGame", reply);
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

        public BotReply MergeToMarkdown(string subtitle, List<string> msgs)
        {
            if (msgs.Count > 30)
            {
                msgs = [.. msgs[..15], .. msgs[^15..]];
            }
            return new()
            {
                Markdown = new()
                {
                    Content = $"{subtitle}\r\n```\r\n{string.Join("\r\n", msgs)}\r\n```"
                }
            };
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

        public BotReply CreateForgeSystemButtons(BotReply reply)
        {
            reply.Keyboard ??= new KeyboardMessage();
            reply.Keyboard.AppendButtonsWithNewRow(2, [
                Button.CreateCmdButton("创建配方", "锻造配方", false),
                Button.CreateCmdButton("模拟配方", "模拟锻造配方", false),
                Button.CreateCmdButton("锻造信息", "锻造信息", false),
                Button.CreateCmdButton("取消锻造", "取消锻造", false),
                Button.CreateCmdButton("模拟锻造", "模拟锻造", false),
                Button.CreateCmdButton("确认锻造", "确认开始锻造", false)
            ]);
            return reply;
        }

        public BotReply CreateMarkdownFromText(string text)
        {
            return new()
            {
                Markdown = new()
                {
                    Content = text
                }
            };
        }
    }
}
