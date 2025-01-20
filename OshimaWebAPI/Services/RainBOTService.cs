using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Milimoe.FunGame.Core.Api.Utility;
using Oshima.Core.Configs;
using Oshima.FunGame.WebAPI.Constant;
using Oshima.FunGame.WebAPI.Controllers;
using Oshima.FunGame.WebAPI.Models;

namespace Oshima.FunGame.WebAPI.Services
{
    public class RainBOTService(FunGameController controller, QQController qqcontroller, QQBotService service, ILogger<RainBOTService> logger)
    {
        private static List<string> FunGameItemType { get; } = ["卡包", "武器", "防具", "鞋子", "饰品", "消耗品", "魔法卡", "收藏品", "特殊物品", "任务物品", "礼包", "其他"];
        private bool FunGameSimulation { get; set; } = false;
        private FunGameController Controller { get; } = controller;
        private QQController QQController { get; } = qqcontroller;
        private QQBotService Service { get; } = service;
        private ILogger<RainBOTService> Logger { get; } = logger;

        private async Task SendAsync(IBotMessage msg, string title, string content, int msgType = 0, object? media = null, int? msgSeq = null)
        {
            Statics.RunningPlugin?.Controller.WriteLine(title, Milimoe.FunGame.Core.Library.Constant.LogLevel.Debug);
            if (msg.IsGroup)
            {
                await Service.SendGroupMessageAsync(msg.OpenId, content, msgType, media, msg.Id, msgSeq);
            }
            else
            {
                content = content.Trim();
                await Service.SendC2CMessageAsync(msg.OpenId, content, msgType, media, msg.Id, msgSeq);
            }
        }

        public async Task<bool> Handler(GroupAtMessage? groupAt = null, C2CMessage? c2c = null)
        {
            bool result = true;
            try
            {
                IBotMessage? e = null;
                string openid = "";
                long qq = 0;
                if (groupAt != null)
                {
                    e = groupAt;
                }
                else if (c2c != null)
                {
                    e = c2c;
                }

                if (e is null)
                {
                    return false;
                }

                string isGroup = e.IsGroup ? "群聊" : "私聊";

                openid = e.AuthorOpenId;
                if (QQOpenID.QQAndOpenID.TryGetValue(openid, out long temp_qq))
                {
                    qq = temp_qq;
                }

                if (e.Detail.StartsWith("绑定"))
                {
                    string detail = e.Detail.Replace("绑定", "");
                    string msg = "";
                    if (long.TryParse(detail, out temp_qq))
                    {
                        msg = NetworkUtility.JsonDeserialize<string>(QQController.Bind(new(openid, temp_qq))) ?? "";
                    }
                    else
                    {
                        msg = "绑定失败，请提供一个正确的QQ号！";
                    }
                    await SendAsync(e, "绑定", msg);
                }

                if (e.Detail == "帮助" || e.Detail == "帮助1")
                {
                    await SendAsync(e, "饭给木", @"《饭给木》游戏指令列表（第 1 / 6 页）
1、创建存档：创建存档，生成随机一个自建角色（序号固定为1）
2、我的库存/我的背包/查看库存 [页码]：显示所有角色、物品库存，每个角色和物品都有一个专属序号
3、我的库存 <物品类型> [页码]：卡包/武器/防具/鞋子/饰品/消耗品/魔法卡等...
4、分类库存 <物品索引> [页码]：物品索引从0开始，同上...
5、物品库存 [页码]：显示所有角色
* 上述三指令会将物品按品质倒序和数量倒序排序，整合物品序号和数量显示物品库存
6、角色库存 [页码]：显示所有角色
7、我角色 [角色序号]：查看指定序号角色的简略信息，缺省为1
8、我的角色 [角色序号]：查看指定序号角色的详细信息，缺省为1
9、角色重随：重新随机自建角色的属性，需要花材料
10、我的物品 <物品序号>：查看指定序号物品的详细信息
11、设置主战 <角色序号>：将指定序号角色设置为主战
发送【帮助2】查看第 2 页");
                }

                if (e.Detail == "帮助2")
                {
                    await SendAsync(e, "饭给木", @"《饭给木》游戏指令列表（第 2 / 6 页）
12、装备 <角色序号> <物品序号>：装备指定物品给指定角色
13、取消装备 <角色序号> <装备槽序号>：卸下角色指定装备槽上的物品
* 装备槽序号从1开始，卡包/武器/防具/鞋子/饰品1/饰品2
14、角色改名：修改名字，需要金币
15、抽卡/十连抽卡：2000金币一次，还有材料抽卡/材料十连抽卡，10材料1次
16、开启练级 [角色序号]：让指定角色启动练级模式，缺省为1
17、练级结算：收取奖励，最多累计24小时的收益
18、练级信息：查看当前进度
19、角色升级 [角色序号]：升到不能升为止
20、角色突破 [角色序号]：每10/20/30/40/50/60级都要突破才可以继续升级
21、突破信息 [角色序号]：查看下一次突破信息
发送【帮助3】查看第 3 页");
                }

                if (e.Detail == "帮助3")
                {
                    await SendAsync(e, "饭给木", @"《饭给木》游戏指令列表（第 3 / 6 页）
22、普攻升级 [角色序号]：升级普攻等级
23、查看普攻升级 [角色序号]：查看下一次普攻升级信息
23、技能升级 <角色序号> <技能名称>：升级技能等级
24、查看技能升级 <角色序号> <技能名称>：查看下一次技能升级信息
25、使用 <物品名称> <数量> [角色] [角色序号]
26、使用 <物品序号> [角色] [角色序号]
27、使用魔法卡 <魔法卡序号> <魔法卡包序号>
28、合成魔法卡 <{物品序号...}>：要3张魔法卡，空格隔开
29、分解物品 <{物品序号...}>
30、分解 <物品名称> <数量>
31、品质分解 <品质索引>：从0开始，普通/优秀/稀有/史诗/传说/神话/不朽
32、决斗/完整决斗 <@对方>/<QQ号>/<昵称>：和对方切磋
发送【帮助4】查看第 4 页");
                }

                if (e.Detail == "帮助4")
                {
                    await SendAsync(e, "饭给木", @"《饭给木》游戏指令列表（第 4 / 6 页）
33、兑换金币 <材料数>：1材料=200金币
34、还原存档：没有后悔药
35、我的主战：查看当前主战角色
36、我的小队：查看小队角色名单
37、我的存档：查看账号/存档信息
38、设置小队 <{角色序号...}>：设置小队角色（1-4个参数）
39、小队决斗/小队完整决斗 <@对方>/<QQ号>/<昵称>：用小队和对方切磋
40、查询boss [boss序号]：查看指定序号boss的详细信息，缺省为boss名称列表
41、讨伐/小队讨伐boss <boss序号>
42、签到：每日签到
发送【帮助5】查看第 5 页");
                }

                if (e.Detail == "帮助5")
                {
                    await SendAsync(e, "饭给木", @"《饭给木》游戏指令列表（第 5 / 6 页）
43：任务列表：查看今日任务列表
44：开始任务 <任务序号>
45、任务信息：查看进行中任务的详细信息
46、任务结算：对进行中的任务进行结算
47、我的状态：查看主战角色状态
48、小队状态/我的小队状态：查看小队所有角色的状态
49、小队添加 <角色序号>：将角色加入小队
50、小队移除 <角色序号>：将角色移出小队
51、清空小队
发送【帮助6】查看第 6 页");
                }
                
                if (e.Detail == "帮助6")
                {
                    await SendAsync(e, "饭给木", @"《饭给木》游戏指令列表（第 6 / 6 页）
52、我的社团：查看社团信息
53、加入社团 <社团编号>：申请加入社团
54、退出社团
55、创建社团 <社团前缀>：创建一个公开社团，若指令中包含私密一词，将创建私密社团
社团前缀：3-4个字符，允许：英文字母和数字、部分特殊字符
56、查看社团成员/查看社团管理/查看申请人列表：查看对应列表
57、解散社团
58、社团批准 <@对方>/<QQ号
59、社团拒绝 <@对方>/<QQ号
60、社团踢出 <@对方>/<QQ号
61、社团转让 <@对方>/<QQ号
62、社团设置 <设置项> <{参数...}>");
                }

                if (e.Detail.StartsWith("FunGame模拟", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (!FunGameSimulation)
                    {
                        FunGameSimulation = true;
                        List<string> msgs = Controller.GetTest(false);
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
                            await SendAsync(e, "饭给木", msg.Trim(), msgSeq: count++);
                            await Task.Delay(5500);
                        }
                        FunGameSimulation = false;
                    }
                    else
                    {
                        await SendAsync(e, "饭给木", "游戏正在模拟中，请勿重复请求！");
                    }
                    return result;
                }

                if (e.Detail.StartsWith("FunGame团队模拟", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (!FunGameSimulation)
                    {
                        FunGameSimulation = true;
                        List<string> msgs = Controller.GetTest(false, true);
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
                            await SendAsync(e, "饭给木", msg.Trim(), msgSeq: count++);
                            await Task.Delay(5500);
                        }
                        FunGameSimulation = false;
                    }
                    else
                    {
                        await SendAsync(e, "饭给木", "游戏正在模拟中，请勿重复请求！");
                    }
                    return result;
                }

                if (e.Detail.StartsWith("查数据", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("查数据", "").Trim();
                    if (int.TryParse(detail, out int id))
                    {
                        string msg = NetworkUtility.JsonDeserialize<string>(Controller.GetStats(id)) ?? "";
                        if (msg != "")
                        {
                            await SendAsync(e, "查数据", msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("查团队数据", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("查团队数据", "").Trim();
                    if (int.TryParse(detail, out int id))
                    {
                        string msg = NetworkUtility.JsonDeserialize<string>(Controller.GetTeamStats(id)) ?? "";
                        if (msg != "")
                        {
                            await SendAsync(e, "查团队数据", msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("查个人胜率", StringComparison.CurrentCultureIgnoreCase))
                {
                    string msg = NetworkUtility.JsonDeserialize<string>(Controller.GetWinrateRank(false)) ?? "";
                    if (msg.Length > 0)
                    {
                        await SendAsync(e, "查个人胜率", string.Join("\r\n\r\n", msg));
                    }
                    return result;
                }

                if (e.Detail.StartsWith("查团队胜率", StringComparison.CurrentCultureIgnoreCase))
                {
                    string msg = NetworkUtility.JsonDeserialize<string>(Controller.GetWinrateRank(true)) ?? "";
                    if (msg.Length > 0)
                    {
                        await SendAsync(e, "查团队胜率", string.Join("\r\n\r\n", msg));
                    }
                    return result;
                }

                if (e.Detail.StartsWith("查角色", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("查角色", "").Trim();
                    if (int.TryParse(detail, out int id))
                    {
                        string msg = NetworkUtility.JsonDeserialize<string>(Controller.GetCharacterInfo(id)) ?? "";
                        if (msg != "")
                        {
                            await SendAsync(e, "查角色", msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("查技能", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("查技能", "").Trim();
                    if (int.TryParse(detail, out int id))
                    {
                        string msg = NetworkUtility.JsonDeserialize<string>(Controller.GetSkillInfo(qq, id)) ?? "";
                        if (msg != "")
                        {
                            await SendAsync(e, "查技能", msg);
                        }
                    }
                    else
                    {
                        string msg = NetworkUtility.JsonDeserialize<string>(Controller.GetSkillInfo_Name(qq, detail)) ?? "";
                        if (msg != "")
                        {
                            await SendAsync(e, "查技能", msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("查物品", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("查物品", "").Trim();
                    if (int.TryParse(detail, out int id))
                    {
                        string msg = NetworkUtility.JsonDeserialize<string>(Controller.GetItemInfo(qq, id)) ?? "";
                        if (msg != "")
                        {
                            await SendAsync(e, "查物品", msg);
                        }
                    }
                    else
                    {
                        string msg = NetworkUtility.JsonDeserialize<string>(Controller.GetItemInfo_Name(qq, detail)) ?? "";
                        if (msg != "")
                        {
                            await SendAsync(e, "查物品", msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("生成"))
                {
                    string pattern = @"生成\s*(\d+)\s*个\s*([\s\S]+)(?:\s*给\s*(\d+))";
                    Regex regex = new(pattern, RegexOptions.IgnoreCase);
                    Match match = regex.Match(e.Detail);

                    if (match.Success)
                    {
                        int count = int.Parse(match.Groups[1].Value);
                        string name = match.Groups[2].Value.Trim();
                        string target = match.Groups[3].Value;
                        long userid = qq;

                        if (!string.IsNullOrEmpty(target))
                        {
                            userid = long.Parse(target);
                        }

                        if (count > 0)
                        {
                            string msg = NetworkUtility.JsonDeserialize<string>(Controller.CreateItem(qq, name, count, userid)) ?? "";
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
                    string msg = NetworkUtility.JsonDeserialize<string>(Controller.GenerateMagicCardPack()) ?? "";
                    if (msg != "")
                    {
                        await SendAsync(e, "生成魔法卡包", msg);
                    }
                    return result;
                }
                else if (e.Detail == "生成魔法卡")
                {
                    string msg = NetworkUtility.JsonDeserialize<string>(Controller.GenerateMagicCard()) ?? "";
                    if (msg != "")
                    {
                        await SendAsync(e, "生成魔法卡", msg);
                    }
                    return result;
                }

                if (e.Detail == "创建存档")
                {
                    string msg = NetworkUtility.JsonDeserialize<string>(Controller.CreateSaved(qq)) ?? "";
                    if (msg != "")
                    {
                        await SendAsync(e, "创建存档", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "我的存档")
                {
                    string msg = NetworkUtility.JsonDeserialize<string>(Controller.ShowSaved(qq)) ?? "";
                    if (msg != "")
                    {
                        await SendAsync(e, "我的存档", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "我的主战")
                {
                    string msg = NetworkUtility.JsonDeserialize<string>(Controller.GetCharacterInfoFromInventory(qq, 0)) ?? "";
                    if (msg != "")
                    {
                        await SendAsync(e, "我的主战", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "我的状态")
                {
                    string msg = NetworkUtility.JsonDeserialize<string>(Controller.ShowMainCharacterOrSquadStatus(qq)) ?? "";
                    if (msg != "")
                    {
                        await SendAsync(e, "我的状态", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "小队状态" || e.Detail == "我的小队状态")
                {
                    string msg = NetworkUtility.JsonDeserialize<string>(Controller.ShowMainCharacterOrSquadStatus(qq, true)) ?? "";
                    if (msg != "")
                    {
                        await SendAsync(e, "我的小队状态", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "我的小队")
                {
                    string msg = NetworkUtility.JsonDeserialize<string>(Controller.ShowSquad(qq)) ?? "";
                    if (msg != "")
                    {
                        await SendAsync(e, "我的小队", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "清空小队")
                {
                    string msg = NetworkUtility.JsonDeserialize<string>(Controller.ClearSquad(qq)) ?? "";
                    if (msg != "")
                    {
                        await SendAsync(e, "清空小队", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "还原存档")
                {
                    string msg = NetworkUtility.JsonDeserialize<string>(Controller.RestoreSaved(qq)) ?? "";
                    if (msg != "")
                    {
                        await SendAsync(e, "还原存档", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "生成自建角色")
                {
                    string msg = NetworkUtility.JsonDeserialize<string>(Controller.NewCustomCharacter(qq)) ?? "";
                    if (msg != "")
                    {
                        await SendAsync(e, "抽卡", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "角色改名")
                {
                    string msg = NetworkUtility.JsonDeserialize<string>(Controller.ReName(qq)) ?? "";
                    if (msg != "")
                    {
                        await SendAsync(e, "改名", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "角色重随")
                {
                    string msg = NetworkUtility.JsonDeserialize<string>(Controller.RandomCustomCharacter(qq, false)) ?? "";
                    if (msg != "")
                    {
                        await SendAsync(e, "角色重随", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "确认角色重随")
                {
                    string msg = NetworkUtility.JsonDeserialize<string>(Controller.RandomCustomCharacter(qq, true)) ?? "";
                    if (msg != "")
                    {
                        await SendAsync(e, "角色重随", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "取消角色重随")
                {
                    string msg = NetworkUtility.JsonDeserialize<string>(Controller.CancelRandomCustomCharacter(qq)) ?? "";
                    if (msg != "")
                    {
                        await SendAsync(e, "角色重随", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "抽卡")
                {
                    string msg = NetworkUtility.JsonDeserialize<string>(Controller.DrawCard(qq)) ?? "";
                    if (msg != "")
                    {
                        await SendAsync(e, "抽卡", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "十连抽卡")
                {
                    List<string> msgs = Controller.DrawCards(qq);
                    if (msgs.Count > 0)
                    {
                        await SendAsync(e, "十连抽卡", "\r\n" + string.Join("\r\n", msgs));
                    }
                    return result;
                }

                if (e.Detail == "材料抽卡")
                {
                    string msg = NetworkUtility.JsonDeserialize<string>(Controller.DrawCard_Material(qq)) ?? "";
                    if (msg != "")
                    {
                        await SendAsync(e, "材料抽卡", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "材料十连抽卡")
                {
                    List<string> msgs = Controller.DrawCards_Material(qq);
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
                        msgs = Controller.GetInventoryInfo2(qq, page);
                    }
                    else if (FunGameItemType.FirstOrDefault(detail.Contains) is string matchedType)
                    {
                        int typeIndex = FunGameItemType.IndexOf(matchedType);
                        string remain = detail.Replace(matchedType, "").Trim();
                        if (int.TryParse(remain, out page))
                        {
                            msgs = Controller.GetInventoryInfo4(qq, page, typeIndex);
                        }
                        else
                        {
                            msgs = Controller.GetInventoryInfo4(qq, 1, typeIndex);
                        }
                    }
                    else
                    {
                        msgs = Controller.GetInventoryInfo2(qq, 1);
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
                        msgs = Controller.GetInventoryInfo3(qq, page, 2, 2);
                    }
                    else
                    {
                        msgs = Controller.GetInventoryInfo3(qq, 1, 2, 2);
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
                        msgs = Controller.GetInventoryInfo5(qq, page);
                    }
                    else
                    {
                        msgs = Controller.GetInventoryInfo5(qq, 1);
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
                    string[] strings = detail.Split(" ");
                    int t = -1;
                    if (strings.Length > 0 && int.TryParse(strings[0].Trim(), out t))
                    {
                        List<string> msgs = [];
                        if (strings.Length > 1 && int.TryParse(strings[1].Trim(), out int page))
                        {
                            msgs = Controller.GetInventoryInfo4(qq, page, t);
                        }
                        else
                        {
                            msgs = Controller.GetInventoryInfo4(qq, 1, t);
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
                        msg = NetworkUtility.JsonDeserialize<string>(Controller.GetCharacterInfoFromInventory(qq, seq, true)) ?? "";
                    }
                    else
                    {
                        msg = NetworkUtility.JsonDeserialize<string>(Controller.GetCharacterInfoFromInventory(qq, 1, true)) ?? "";
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
                        msg = NetworkUtility.JsonDeserialize<string>(Controller.GetCharacterInfoFromInventory(qq, seq, false)) ?? "";
                    }
                    else
                    {
                        msg = NetworkUtility.JsonDeserialize<string>(Controller.GetCharacterInfoFromInventory(qq, 1, false)) ?? "";
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
                        msg = NetworkUtility.JsonDeserialize<string>(Controller.GetCharacterSkills(qq, seq)) ?? "";
                    }
                    else
                    {
                        msg = NetworkUtility.JsonDeserialize<string>(Controller.GetCharacterSkills(qq, 1)) ?? "";
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
                        msg = NetworkUtility.JsonDeserialize<string>(Controller.GetCharacterItems(qq, seq)) ?? "";
                    }
                    else
                    {
                        msg = NetworkUtility.JsonDeserialize<string>(Controller.GetCharacterItems(qq, 1)) ?? "";
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
                        msg = NetworkUtility.JsonDeserialize<string>(Controller.SetMain(qq, cid)) ?? "";
                    }
                    else
                    {
                        msg = NetworkUtility.JsonDeserialize<string>(Controller.SetMain(qq, 1)) ?? "";
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
                        msg = NetworkUtility.JsonDeserialize<string>(Controller.StartTraining(qq, cid)) ?? "";
                    }
                    else
                    {
                        msg = NetworkUtility.JsonDeserialize<string>(Controller.StartTraining(qq, 1)) ?? "";
                    }
                    if (msg != "")
                    {
                        await SendAsync(e, "开启练级", msg);
                    }
                    return result;
                }

                if (e.Detail == "练级信息")
                {
                    string msg = NetworkUtility.JsonDeserialize<string>(Controller.GetTrainingInfo(qq)) ?? "";
                    if (msg != "")
                    {
                        await SendAsync(e, "练级信息", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "练级结算")
                {
                    string msg = NetworkUtility.JsonDeserialize<string>(Controller.StopTraining(qq)) ?? "";
                    if (msg != "")
                    {
                        await SendAsync(e, "练级结算", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "任务列表")
                {
                    string msg = NetworkUtility.JsonDeserialize<string>(Controller.CheckQuestList(qq)) ?? "";
                    if (msg != "")
                    {
                        await SendAsync(e, "任务列表", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "任务信息")
                {
                    string msg = NetworkUtility.JsonDeserialize<string>(Controller.CheckWorkingQuest(qq)) ?? "";
                    if (msg != "")
                    {
                        await SendAsync(e, "任务信息", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "任务结算")
                {
                    string msg = NetworkUtility.JsonDeserialize<string>(Controller.SettleQuest(qq)) ?? "";
                    if (msg != "")
                    {
                        await SendAsync(e, "任务结算", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "签到")
                {
                    string msg = NetworkUtility.JsonDeserialize<string>(Controller.SignIn(qq)) ?? "";
                    if (msg != "")
                    {
                        await SendAsync(e, "签到", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("开始任务", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("开始任务", "").Trim();
                    if (int.TryParse(detail, out int index))
                    {
                        string msg = NetworkUtility.JsonDeserialize<string>(Controller.AcceptQuest(qq, index)) ?? "";
                        if (msg != "")
                        {
                            await SendAsync(e, "开始任务", msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("我的物品", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("我的物品", "").Trim();
                    if (int.TryParse(detail, out int index))
                    {
                        string msg = NetworkUtility.JsonDeserialize<string>(Controller.GetItemInfoFromInventory(qq, index)) ?? "";
                        if (msg != "")
                        {
                            await SendAsync(e, "查库存物品", msg);
                        }
                    }
                    return result;
                }

                if (e.Detail.StartsWith("兑换金币", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("兑换金币", "").Trim();
                    if (int.TryParse(detail, out int materials))
                    {
                        string msg = NetworkUtility.JsonDeserialize<string>(Controller.ExchangeCredits(qq, materials)) ?? "";
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
                    string[] strings = detail.Split(" ");
                    int c = -1, i = -1;
                    if (strings.Length > 0 && int.TryParse(strings[0].Trim(), out c) && strings.Length > 1 && int.TryParse(strings[1].Trim(), out i))
                    {
                        if (c != -1 && i != -1)
                        {
                            string msg = NetworkUtility.JsonDeserialize<string>(Controller.UnEquipItem(qq, c, i)) ?? "";
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
                    string[] strings = detail.Split(" ");
                    int c = -1, i = -1;
                    if (strings.Length > 0 && int.TryParse(strings[0].Trim(), out c) && strings.Length > 1 && int.TryParse(strings[1].Trim(), out i))
                    {
                        if (c != -1 && i != -1)
                        {
                            string msg = NetworkUtility.JsonDeserialize<string>(Controller.EquipItem(qq, c, i)) ?? "";
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
                    string[] strings = detail.Split(" ");
                    int c = -1;
                    if (strings.Length > 0 && int.TryParse(strings[0].Trim(), out c) && strings.Length > 1)
                    {
                        string s = strings[1].Trim();
                        if (c != -1 && s != "")
                        {
                            string msg = NetworkUtility.JsonDeserialize<string>(Controller.GetSkillLevelUpNeedy(qq, c, s)) ?? "";
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
                    string[] strings = detail.Split(" ");
                    int c = -1;
                    if (strings.Length > 0 && int.TryParse(strings[0].Trim(), out c) && strings.Length > 1)
                    {
                        string s = strings[1].Trim();
                        if (c != -1 && s != "")
                        {
                            string msg = NetworkUtility.JsonDeserialize<string>(Controller.SkillLevelUp(qq, c, s)) ?? "";
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
                    string[] strings = detail.Split(" ");
                    int id1 = -1, id2 = -1, id3 = -1;
                    if (strings.Length > 0 && int.TryParse(strings[0].Trim(), out id1) && strings.Length > 1 && int.TryParse(strings[1].Trim(), out id2) && strings.Length > 2 && int.TryParse(strings[2].Trim(), out id3))
                    {
                        if (id1 != -1 && id2 != -1 && id3 != -1)
                        {
                            string msg = NetworkUtility.JsonDeserialize<string>(Controller.ConflateMagicCardPack(qq, [id1, id2, id3])) ?? "";
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
                        msg = NetworkUtility.JsonDeserialize<string>(Controller.CharacterLevelUp(qq, cid)) ?? "";
                    }
                    else
                    {
                        msg = NetworkUtility.JsonDeserialize<string>(Controller.CharacterLevelUp(qq, 1)) ?? "";
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
                        msg = NetworkUtility.JsonDeserialize<string>(Controller.GetNormalAttackLevelUpNeedy(qq, cid)) ?? "";
                    }
                    else
                    {
                        msg = NetworkUtility.JsonDeserialize<string>(Controller.GetNormalAttackLevelUpNeedy(qq, 1)) ?? "";
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
                        msg = NetworkUtility.JsonDeserialize<string>(Controller.NormalAttackLevelUp(qq, cid)) ?? "";
                    }
                    else
                    {
                        msg = NetworkUtility.JsonDeserialize<string>(Controller.NormalAttackLevelUp(qq, 1)) ?? "";
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
                        msg = NetworkUtility.JsonDeserialize<string>(Controller.CharacterLevelBreak(qq, cid)) ?? "";
                    }
                    else
                    {
                        msg = NetworkUtility.JsonDeserialize<string>(Controller.CharacterLevelBreak(qq, 1)) ?? "";
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
                        msg = NetworkUtility.JsonDeserialize<string>(Controller.GetLevelBreakNeedy(qq, cid)) ?? "";
                    }
                    else
                    {
                        msg = NetworkUtility.JsonDeserialize<string>(Controller.GetLevelBreakNeedy(qq, 1)) ?? "";
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
                                    string msg = NetworkUtility.JsonDeserialize<string>(Controller.UseItem3(qq, id, id2, isCharacter)) ?? "";
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
                                int[] characterIds = characterIdsString != "" ? [.. characterIdsString.Split(chars, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse)] : [1];
                                string msg = NetworkUtility.JsonDeserialize<string>(Controller.UseItem2(qq, itemName, count, characterIds)) ?? "";
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
                                    int[] characterIds = characterIdsString != "" ? [.. characterIdsString.Split(chars, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse)] : [1];
                                    string msg = NetworkUtility.JsonDeserialize<string>(Controller.UseItem(qq, itemId, characterIds)) ?? "";
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
                                        string msg = NetworkUtility.JsonDeserialize<string>(Controller.UseItem2(qq, itemName, count)) ?? "";
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
                                            string msg = NetworkUtility.JsonDeserialize<string>(Controller.UseItem(qq, itemId)) ?? "";
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
                    foreach (string str in detail.Split(' '))
                    {
                        if (int.TryParse(str, out int id))
                        {
                            ids.Add(id);
                        }
                    }
                    string msg = NetworkUtility.JsonDeserialize<string>(Controller.DecomposeItem(qq, [.. ids])) ?? "";
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
                            string msg = NetworkUtility.JsonDeserialize<string>(Controller.DecomposeItem2(qq, itemName, count)) ?? "";
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
                        string msg = NetworkUtility.JsonDeserialize<string>(Controller.DecomposeItem3(qq, q)) ?? "";
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
                    string[] strings = detail.Split(" ");
                    int count = -1;
                    if (strings.Length > 1 && int.TryParse(strings[1].Trim(), out count))
                    {
                        string name = strings[0].Trim();
                        if (count > 0)
                        {
                            long userid = qq;
                            if (strings.Length > 2 && long.TryParse(strings[2].Replace("@", "").Trim(), out long temp))
                            {
                                userid = temp;
                            }
                            string msg = NetworkUtility.JsonDeserialize<string>(Controller.CreateItem(qq, name, count, userid)) ?? "";
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
                        msgs = Controller.FightCustom(qq, eqq, true);
                    }
                    else
                    {
                        msgs = Controller.FightCustom2(qq, detail.Trim(), true);
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
                        msgs = Controller.FightCustom(qq, eqq, false);
                    }
                    else
                    {
                        msgs = Controller.FightCustom2(qq, detail.Trim(), false);
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
                        msgs = Controller.FightCustomTeam(qq, eqq, true);
                    }
                    else
                    {
                        msgs = Controller.FightCustomTeam2(qq, detail.Trim(), true);
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
                        msgs = Controller.FightBossTeam(qq, index, true);
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
                        msgs = Controller.FightBoss(qq, index, true);
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
                        string msg = NetworkUtility.JsonDeserialize<string>(Controller.AddSquad(qq, c)) ?? "";
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
                        string msg = NetworkUtility.JsonDeserialize<string>(Controller.RemoveSquad(qq, c)) ?? "";
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
                    string[] strings = detail.Split(' ');
                    List<int> cindexs = [];
                    foreach (string s in strings)
                    {
                        if (int.TryParse(s, out int c))
                        {
                            cindexs.Add(c);
                        }
                    }
                    string msg = NetworkUtility.JsonDeserialize<string>(Controller.SetSquad(qq, [.. cindexs])) ?? "";
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
                        string msg = NetworkUtility.JsonDeserialize<string>(Controller.JoinClub(qq, c)) ?? "";
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
                    string msg = NetworkUtility.JsonDeserialize<string>(Controller.CreateClub(qq, isPublic, detail)) ?? "";
                    if (msg != "")
                    {
                        await SendAsync(e, "社团", msg);
                    }
                    return result;
                }

                if (e.Detail == "退出社团")
                {
                    string msg = NetworkUtility.JsonDeserialize<string>(Controller.QuitClub(qq)) ?? "";
                    if (msg != "")
                    {
                        await SendAsync(e, "社团", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "我的社团")
                {
                    string msg = NetworkUtility.JsonDeserialize<string>(Controller.ShowClubInfo(qq)) ?? "";
                    if (msg != "")
                    {
                        await SendAsync(e, "社团", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "解散社团")
                {
                    string msg = NetworkUtility.JsonDeserialize<string>(Controller.DisbandClub(qq)) ?? "";
                    if (msg != "")
                    {
                        await SendAsync(e, "社团", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "查看社团成员")
                {
                    string msg = NetworkUtility.JsonDeserialize<string>(Controller.ShowClubMemberList(qq, 0)) ?? "";
                    if (msg != "")
                    {
                        await SendAsync(e, "社团", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "查看社团管理")
                {
                    string msg = NetworkUtility.JsonDeserialize<string>(Controller.ShowClubMemberList(qq, 1)) ?? "";
                    if (msg != "")
                    {
                        await SendAsync(e, "社团", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail == "查看申请人列表")
                {
                    string msg = NetworkUtility.JsonDeserialize<string>(Controller.ShowClubMemberList(qq, 2)) ?? "";
                    if (msg != "")
                    {
                        await SendAsync(e, "社团", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("社团批准", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("社团批准", "").Replace("@", "").Trim();
                    if (long.TryParse(detail, out long id))
                    {
                        string msg = NetworkUtility.JsonDeserialize<string>(Controller.ApproveClub(qq, id, true)) ?? "";
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
                        string msg = NetworkUtility.JsonDeserialize<string>(Controller.ApproveClub(qq, id, false)) ?? "";
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
                        string msg = NetworkUtility.JsonDeserialize<string>(Controller.KickClub(qq, id)) ?? "";
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
                    string[] strings = detail.Split(' ');
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
                        string msg = NetworkUtility.JsonDeserialize<string>(Controller.ChangeClub(qq, part, [.. args])) ?? "";
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
                    string msg = NetworkUtility.JsonDeserialize<string>(Controller.ChangeClub(qq, "setmaster", [.. args])) ?? "";
                    if (msg != "")
                    {
                        await SendAsync(e, "社团", msg);
                    }
                    return result;
                }

                if (e.Detail == "每日商店")
                {
                    string msg = NetworkUtility.JsonDeserialize<string>(Controller.ShowDailyStore(qq)) ?? "";
                    if (msg != "")
                    {
                        await SendAsync(e, "商店", "\r\n" + msg);
                    }
                    return result;
                }

                if (e.Detail.StartsWith("商店购买", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("商店购买", "").Trim();
                    if (int.TryParse(detail, out int id))
                    {
                        string msg = NetworkUtility.JsonDeserialize<string>(Controller.DailyStoreBuy(qq, id)) ?? "";
                        if (msg != "")
                        {
                            await SendAsync(e, "商店", msg);
                        }
                    }
                    return result;
                }
                
                if (e.Detail.StartsWith("商店查看", StringComparison.CurrentCultureIgnoreCase))
                {
                    string detail = e.Detail.Replace("商店查看", "").Trim();
                    if (int.TryParse(detail, out int id))
                    {
                        string msg = NetworkUtility.JsonDeserialize<string>(Controller.DailyStoreShowInfo(qq, id)) ?? "";
                        if (msg != "")
                        {
                            await SendAsync(e, "商店", msg);
                        }
                    }
                    return result;
                }

                if (qq == GeneralSettings.Master && e.Detail.StartsWith("重载FunGame", StringComparison.CurrentCultureIgnoreCase))
                {
                    string msg = NetworkUtility.JsonDeserialize<string>(Controller.Relaod(qq)) ?? "";
                    if (msg != "")
                    {
                        await SendAsync(e, "重载FunGame", msg);
                    }
                    return result;
                }
            }
            catch (Exception e)
            {
                Logger.LogError("Error: {e}", e);
            }

            return false;
        }
    }
}
