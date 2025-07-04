﻿using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.Core.Configs;
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
                third.Result = "\r\n" + content.Trim();
                third.IsCompleted = true;
                return;
            }
            if (msg.IsGroup)
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
                await SendAsync(msg, "每日登录提醒", string.Join("\r\n", msgs), msgType, media, 5);
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

        public async Task<bool> Handler(IBotMessage e)
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
5、任务帮助
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
                else if (e.Detail.StartsWith("任务帮助"))
                {
                    int page = e.Detail.Length > 4 && int.TryParse(e.Detail[4..], out int p) ? p : 1;
                    await SendHelp(e, FunGameOrderList.QuestHelp, "任务帮助", page);
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
                        List<string> real = [];
                        int remain = 7;
                        string merge = "";
                        for (int i = 0; i < msgs.Count - 2; i++)
                        {
                            remain--;
                            merge += msgs[i] + "\r\n";
                            if (remain == 0)
                            {
                                real.Add(merge);
                                merge = "";
                                if ((msgs.Count - i - 3) < 7)
                                {
                                    remain = msgs.Count - i - 3;
                                }
                                else remain = 7;
                            }
                        }
                        if (msgs.Count > 2)
                        {
                            real.Add(msgs[^2]);
                            real.Add(msgs[^1]);
                        }
                        if (real.Count >= 3)
                        {
                            real = real[^3..];
                        }
                        int count = 1;
                        foreach (string msg in real)
                        {
                            await SendAsync(e, "筽祀牻", msg.Trim(), msgSeq: count++);
                            await Task.Delay(5500);
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
                        List<string> real = [];
                        int remain = 7;
                        string merge = "";
                        for (int i = 0; i < msgs.Count - 2; i++)
                        {
                            remain--;
                            merge += msgs[i] + "\r\n";
                            if (remain == 0)
                            {
                                real.Add(merge);
                                merge = "";
                                if ((msgs.Count - i - 3) < 7)
                                {
                                    remain = msgs.Count - i - 3;
                                }
                                else remain = 7;
                            }
                        }
                        if (msgs.Count > 2)
                        {
                            real.Add(msgs[^2]);
                            real.Add(msgs[^1]);
                        }
                        if (real.Count >= 3)
                        {
                            real = real[^3..];
                        }
                        int count = 1;
                        foreach (string msg in real)
                        {
                            await SendAsync(e, "筽祀牻", msg.Trim(), msgSeq: count++);
                            await Task.Delay(5500);
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
                        List<string> real = [];
                        if (msgs.Count > 0)
                        {
                            real.Add(msgs[0]);
                        }
                        int remain = 7;
                        string merge = "";
                        for (int i = 1; i < msgs.Count - 2; i++)
                        {
                            remain--;
                            merge += msgs[i] + "\r\n";
                            if (remain == 0)
                            {
                                real.Add(merge);
                                merge = "";
                                if ((msgs.Count - i - 3) < 7)
                                {
                                    remain = msgs.Count - i - 3;
                                }
                                else remain = 7;
                            }
                        }
                        if (msgs.Count > 2)
                        {
                            real.Add(msgs[^2]);
                            real.Add(msgs[^1]);
                        }
                        if (real.Count >= 3)
                        {
                            real = real[^3..];
                        }
                        int count = 1;
                        foreach (string msg in real)
                        {
                            await SendAsync(e, "筽祀牻", msg.Trim(), msgSeq: count++);
                            await Task.Delay(5500);
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

                if (e.Detail.StartsWith("生成"))
				{
					e.UseNotice = false;
					string pattern = @"生成\s*(\d+)\s*个\s*([\s\S]+)(?:\s*给\s*(\d+))";
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
                            await SendAsync(e, "熟圣之力", "数量不能为0，请重新输入。");
                        }
                        return result;
                    }
                }

                if (e.Detail == "生成魔法卡包")
				{
					e.UseNotice = false;
					string msg = Controller.GenerateMagicCardPack();
                    if (msg != "")
                    {
                        await SendAsync(e, "生成魔法卡包", msg);
                    }
                    return result;
                }
                else if (e.Detail == "生成魔法卡")
				{
					e.UseNotice = false;
					string msg = Controller.GenerateMagicCard();
                    if (msg != "")
                    {
                        await SendAsync(e, "生成魔法卡", msg);
                    }
                    return result;
                }

                if (e.Detail == "创建存档")
				{
					e.UseNotice = false;
					string msg = Controller.CreateSaved(name: openid);
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
                        await SendAsync(e, "抽卡", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "角色改名")
                {
                    string msg = Controller.ReName(uid);
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
                    string msg = Controller.DrawCard(uid);
                    if (msg != "")
                    {
                        await SendAsync(e, "抽卡", "\r\n" + msg);
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

                if (e.Detail == "材料抽卡")
                {
                    string msg = Controller.DrawCard_Material(uid);
                    if (msg != "")
                    {
                        await SendAsync(e, "材料抽卡", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "材料十连抽卡")
                {
                    List<string> msgs = Controller.DrawCards_Material(uid);
                    if (msgs.Count > 0)
                    {
                        await SendAsync(e, "材料十连抽卡", "\r\n" + string.Join("\r\n", msgs));
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
                        int count = 1;
                        foreach (string msg in msgs)
                        {
                            await SendAsync(e, "开始任务", msg, msgSeq: count++);
                            await Task.Delay(2000);
                        }
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

                if (e.Detail.StartsWith("使用", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("使用", "").Trim();
                    if (detail.StartsWith("魔法卡"))
                    {
                        string pattern = @"\s*魔法卡\s*(?<itemId>\d+)(?:\s*(?:角色\s*)?(?<characterId>\d+))?\s*";
                        Match match = Regex.Match(detail, pattern);
                        if (match.Success)
                        {
                            string itemId = match.Groups["itemId"].Value;
                            string characterId = match.Groups["characterId"].Value;
                            bool isCharacter = detail.Contains("角色");
                            if (int.TryParse(itemId, out int id) && int.TryParse(characterId, out int id2))
                            {
                                if (id > 0 && id2 > 0)
                                {
                                    string msg = Controller.UseItem3(uid, id, id2, isCharacter);
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
                        char[] chars = [',', ' '];
                        string pattern = @"\s*(?<itemName>[^\d]+)\s*(?<count>\d+)\s*(?:角色\s*(?<characterIds>[\d\s]*))?";
                        Match match = Regex.Match(detail, pattern);
                        if (match.Success)
                        {
                            string itemName = match.Groups["itemName"].Value.Trim();
                            if (int.TryParse(match.Groups["count"].Value, out int count))
                            {
                                string characterIdsString = match.Groups["characterIds"].Value;
                                int[] characterIds = characterIdsString != "" ? [.. characterIdsString.Split(chars, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(int.Parse)] : [1];
                                string msg = Controller.UseItem2(uid, itemName, count, characterIds);
                                if (msg != "")
                                {
                                    await SendAsync(e, "使用", msg);
                                }
                            }
                        }
                        else
                        {
                            pattern = @"\s*(?<itemId>\d+)\s*(?:角色\s*(?<characterIds>[\d\s]*))?";
                            match = Regex.Match(detail, pattern);
                            if (match.Success)
                            {
                                if (int.TryParse(match.Groups["itemId"].Value, out int itemId))
                                {
                                    string characterIdsString = match.Groups["characterIds"].Value;
                                    int[] characterIds = characterIdsString != "" ? [.. characterIdsString.Split(chars, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(int.Parse)] : [1];
                                    string msg = Controller.UseItem(uid, itemId, characterIds);
                                    if (msg != "")
                                    {
                                        await SendAsync(e, "使用", msg);
                                    }
                                }
                            }
                            else
                            {
                                pattern = @"\s*(?<itemName>[^\d]+)\s*(?<count>\d+)\s*";
                                match = Regex.Match(detail, pattern);
                                if (match.Success)
                                {
                                    string itemName = match.Groups["itemName"].Value.Trim();
                                    if (int.TryParse(match.Groups["count"].Value, out int count))
                                    {
                                        string msg = Controller.UseItem2(uid, itemName, count);
                                        if (msg != "")
                                        {
                                            await SendAsync(e, "使用", msg);
                                        }
                                    }
                                }
                                else
                                {
                                    pattern = @"\s*(?<itemId>\d+)\s*";
                                    match = Regex.Match(detail, pattern);
                                    if (match.Success)
                                    {
                                        if (int.TryParse(match.Groups["itemId"].Value, out int itemId))
                                        {
                                            string msg = Controller.UseItem(uid, itemId);
                                            if (msg != "")
                                            {
                                                await SendAsync(e, "使用", msg);
                                            }
                                        }
                                    }
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
                    foreach (string str in detail.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
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
                    List<string> real = [];
                    if (msgs.Count >= 2)
                    {
                        if (msgs.Count < 20)
                        {
                            int remain = 7;
                            string merge = "";
                            for (int i = 0; i < msgs.Count - 1; i++)
                            {
                                remain--;
                                merge += msgs[i] + "\r\n";
                                if (remain == 0)
                                {
                                    real.Add(merge);
                                    merge = "";
                                    if ((msgs.Count - i - 2) < 7)
                                    {
                                        remain = msgs.Count - i - 2;
                                    }
                                    else remain = 7;
                                }
                            }
                        }
                        else
                        {
                            real.Add(msgs[^2]);
                        }
                        real.Add(msgs[^1]);
                    }
                    else
                    {
                        real = msgs;
                    }
                    if (real.Count >= 3)
                    {
                        real = real[^3..];
                    }
                    int count = 1;
                    foreach (string msg in real)
                    {
                        await SendAsync(e, "完整决斗", msg.Trim(), msgSeq: count++);
                        await Task.Delay(1500);
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
                    List<string> real = [];
                    if (msgs.Count > 2)
                    {
                        int remain = 7;
                        string merge = "";
                        for (int i = 0; i < msgs.Count - 1; i++)
                        {
                            remain--;
                            merge += msgs[i] + "\r\n";
                            if (remain == 0)
                            {
                                real.Add(merge);
                                merge = "";
                                if ((msgs.Count - i - 3) < 7)
                                {
                                    remain = msgs.Count - i - 3;
                                }
                                else remain = 7;
                            }
                        }
                        real.Add(msgs[^1]);
                    }
                    else
                    {
                        real = msgs;
                    }
                    if (real.Count >= 3)
                    {
                        real = real[^3..];
                    }
                    int count = 1;
                    foreach (string msg in real)
                    {
                        await SendAsync(e, "决斗", msg.Trim(), msgSeq: count++);
                        await Task.Delay(1500);
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
                    List<string> real = [];
                    if (msgs.Count >= 3)
                    {
                        if (msgs.Count < 20)
                        {
                            int remain = 7;
                            string merge = "";
                            for (int i = 0; i < msgs.Count - 1; i++)
                            {
                                remain--;
                                merge += msgs[i] + "\r\n";
                                if (remain == 0)
                                {
                                    real.Add(merge);
                                    merge = "";
                                    if ((msgs.Count - i - 3) < 7)
                                    {
                                        remain = msgs.Count - i - 3;
                                    }
                                    else remain = 7;
                                }
                            }
                        }
                        else
                        {
                            real.Add(msgs[^3]);
                        }
                        real.Add(msgs[^2]);
                        real.Add(msgs[^1]);
                    }
                    else
                    {
                        real = msgs;
                    }
                    if (real.Count >= 3)
                    {
                        real = real[^3..];
                    }
                    int count = 1;
                    foreach (string msg in real)
                    {
                        await SendAsync(e, "完整决斗", msg.Trim(), msgSeq: count++);
                        await Task.Delay(1500);
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
                        List<string> real = [];
                        if (msgs.Count >= 3)
                        {
                            if (msgs.Count < 20)
                            {
                                int remain = 7;
                                string merge = "";
                                for (int i = 0; i < msgs.Count - 1; i++)
                                {
                                    remain--;
                                    merge += msgs[i] + "\r\n";
                                    if (remain == 0)
                                    {
                                        real.Add(merge);
                                        merge = "";
                                        if ((msgs.Count - i - 3) < 7)
                                        {
                                            remain = msgs.Count - i - 3;
                                        }
                                        else remain = 7;
                                    }
                                }
                            }
                            else
                            {
                                real.Add(msgs[^3]);
                            }
                            real.Add(msgs[^2]);
                            real.Add(msgs[^1]);
                        }
                        else
                        {
                            real = msgs;
                        }
                        if (real.Count >= 3)
                        {
                            real = real[^3..];
                        }
                        int count = 1;
                        foreach (string msg in real)
                        {
                            await SendAsync(e, "BOSS", msg.Trim(), msgSeq: count++);
                            await Task.Delay(1500);
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
                        List<string> real = [];
                        if (msgs.Count >= 3)
                        {
                            if (msgs.Count < 20)
                            {
                                int remain = 7;
                                string merge = "";
                                for (int i = 0; i < msgs.Count - 1; i++)
                                {
                                    remain--;
                                    merge += msgs[i] + "\r\n";
                                    if (remain == 0)
                                    {
                                        real.Add(merge);
                                        merge = "";
                                        if ((msgs.Count - i - 3) < 7)
                                        {
                                            remain = msgs.Count - i - 3;
                                        }
                                        else remain = 7;
                                    }
                                }
                            }
                            else
                            {
                                real.Add(msgs[^3]);
                            }
                            real.Add(msgs[^2]);
                            real.Add(msgs[^1]);
                        }
                        else
                        {
                            real = msgs;
                        }
                        if (real.Count >= 3)
                        {
                            real = real[^3..];
                        }
                        int count = 1;
                        foreach (string msg in real)
                        {
                            await SendAsync(e, "BOSS", msg.Trim(), msgSeq: count++);
                            await Task.Delay(1500);
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
                    string[] strings = detail.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
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
                        string msg = Controller.JoinClub(uid, c);
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
                    string msg = Controller.CreateClub(uid, isPublic, detail);
                    if (msg != "")
                    {
                        await SendAsync(e, "社团", msg);
                    }
                    return result;
                }

                if (e.Detail == "退出社团")
                {
                    string msg = Controller.QuitClub(uid);
                    if (msg != "")
                    {
                        await SendAsync(e, "社团", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "我的社团")
                {
                    string msg = Controller.ShowClubInfo(uid);
                    if (msg != "")
                    {
                        await SendAsync(e, "社团", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "解散社团")
                {
                    string msg = Controller.DisbandClub(uid);
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
                        string msg = Controller.ShowClubMemberList(uid, 0, page);
                        if (msg != "")
                        {
                            await SendAsync(e, "社团", "\r\n" + msg);
                        }
                    }
                    else
                    {
                        string msg = Controller.ShowClubMemberList(uid, 0, 1);
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
                        string msg = Controller.ShowClubMemberList(uid, 1, page);
                        if (msg != "")
                        {
                            await SendAsync(e, "社团", "\r\n" + msg);
                        }
                    }
                    else
                    {
                        string msg = Controller.ShowClubMemberList(uid, 1, 1);
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
                        string msg = Controller.ShowClubMemberList(uid, 2, page);
                        if (msg != "")
                        {
                            await SendAsync(e, "社团", "\r\n" + msg);
                        }
                    }
                    else
                    {
                        string msg = Controller.ShowClubMemberList(uid, 2, 1);
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
                        string msg = Controller.ApproveClub(uid, id, true);
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
                        string msg = Controller.ApproveClub(uid, id, false);
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
                        string msg = Controller.KickClub(uid, id);
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
                        string msg = Controller.ChangeClub(uid, part, [.. args]);
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
                    string msg = Controller.ChangeClub(uid, "setmaster", [.. args]);
                    if (msg != "")
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
                    string detail = e.Detail.Replace("商店购买", "").Trim();
                    string[] strings = detail.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    if (strings.Length > 0 && int.TryParse(strings[0].Trim(), out int id))
                    {
                        int count = 1;
                        if (strings.Length > 1 && int.TryParse(strings[1].Trim(), out int temp))
                        {
                            count = temp;
                        }
                        string msg = Controller.DailyStoreBuy(uid, id, count);
                        if (msg != "")
                        {
                            await SendAsync(e, "商店购买", msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("商店查看", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("商店查看", "").Trim();
                    if (int.TryParse(detail, out int id))
                    {
                        string msg = Controller.DailyStoreShowInfo(uid, id);
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
                    Guid eid = Guid.Empty;
                    string[] strings = detail.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
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
                        (msg, eid) = await Controller.ExploreRegion(uid, cindexs[0], [.. cindexs.Skip(1).Select(id => (long)id)]);
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

                if (e.Detail == "生命之泉")
                {
                    string msg = Controller.SpringOfLife(uid);
                    if (msg != "")
                    {
                        await SendAsync(e, "生命之泉", string.Join("\r\n", msg));
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
                    string msg = Controller.GetEvents();
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
                        msg = Controller.GetEvents(eid);
                        if (msg.Trim() != "")
                        {
                            await SendAsync(e, "查活动", msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("上锁") || e.Detail.StartsWith("锁定"))
                {
                    string detail = e.Detail.Replace("上锁", "").Replace("锁定", "").Trim();
                    string msg = "";
                    string[] strings = detail.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
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
                            await SendAsync(e, "上锁", msg);
                        }
                    }
                    return result;
                }
                
                if (e.Detail.StartsWith("解锁"))
                {
                    string detail = e.Detail.Replace("解锁", "").Trim();
                    string msg = "";
                    string[] strings = detail.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
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
                Logger.LogError("Error: {ex}", ex);
            }

            return false;
        }
    }
}
