﻿using System.Data;
using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.Core.Configs;
using Oshima.Core.Constant;
using Oshima.FunGame.OshimaServers.Model;
using Oshima.FunGame.OshimaServers.Service;
using TaskScheduler = Milimoe.FunGame.Core.Api.Utility.TaskScheduler;

namespace Oshima.FunGame.OshimaServers
{
    public class AnonymousServer : GameModuleServer
    {
        public override string Name => OshimaGameModuleConstant.Anonymous;

        public override string Description => OshimaGameModuleConstant.Description;

        public override string Version => OshimaGameModuleConstant.Version;

        public override string Author => OshimaGameModuleConstant.Author;

        public override string DefaultMap => OshimaGameModuleConstant.AnonymousMap;

        public override GameModuleDepend GameModuleDepend => OshimaGameModuleConstant.GameModuleDepend;

        public override bool IsAnonymous => true;

        public static HashSet<AnonymousServer> Instances { get; } = [];

        /// <summary>
        /// 向客户端推送事件
        /// </summary>
        /// <param name="msg"></param>
        public static async Task PushMessageToClients(string openid, string msg)
        {
            AnonymousServer[] servers = [.. Instances];
            foreach (AnonymousServer anonymous in servers)
            {
                try
                {
                    await anonymous.PushMessage(openid, msg);
                }
                catch (Exception e)
                {
                    anonymous.Controller.Error(e);
                }
            }
        }

        protected HashSet<IServerModel> _clientModels = [];

        /// <summary>
        /// 启动匿名服务器
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override bool StartAnonymousServer(IServerModel model, Dictionary<string, object> data)
        {
            // 可以做验证处理
            string access_token = Controller.JSON.GetObject<string>(data, "access_token") ?? "";
            if (GeneralSettings.TokenList.Contains(access_token))
            {
                // 添加当前单例
                Instances.Add(this);
                Controller.WriteLine($"{model.GetClientName()} 连接至匿名服务器", LogLevel.Info);
                // 接收连接匿名服务器的客户端
                _clientModels.Add(model);
                return true;
            }
            else
            {
                Controller.WriteLine($"{model.GetClientName()} 连接匿名服务器失败，访问令牌不匹配", LogLevel.Warning);
            }
            return false;
        }

        /// <summary>
        /// 关闭匿名服务器
        /// </summary>
        /// <param name="model"></param>
        public override void CloseAnonymousServer(IServerModel model)
        {
            // 移除当前单例
            Instances.Remove(this);
            // 移除客户端
            _clientModels.Remove(model);
            Controller.WriteLine($"{model.GetClientName()} 从匿名服务器断开", LogLevel.Info);
        }

        public override void AfterLoad(GameModuleLoader loader, params object[] args)
        {
            foreach (ItemModule itemModule in loader.Items.Values)
            {
                if (itemModule is OshimaModules.ItemModule items)
                {
                    foreach (string key in items.KnownItems.Keys)
                    {
                        Controller.WriteLine(key + ": " + items.KnownItems[key].BackgroundStory, LogLevel.Debug);
                    }
                }
            }
            Controller.NewSQLHelper();
            Controller.NewMailSender();
            FunGameService.InitFunGame();
            FunGameSimulation.InitFunGameSimulation();
            TaskScheduler.Shared.AddTask("重置每日运势", new TimeSpan(0, 0, 0), () =>
            {
                Controller.WriteLine("已重置所有人的今日运势");
                Daily.ClearDaily();
                // 刷新活动缓存
                FunGameService.GetEventCenter();
            });
            TaskScheduler.Shared.AddTask("上九", new TimeSpan(9, 0, 0), () =>
            {
                Controller.WriteLine("重置物品交易冷却时间/刷新地区天气");
                _ = FunGameService.AllowSellAndTrade();
                _ = FunGameService.UpdateRegionWeather();
            });
            TaskScheduler.Shared.AddTask("下三", new TimeSpan(15, 0, 0), () =>
            {
                Controller.WriteLine("重置物品交易冷却时间/刷新地区天气");
                _ = FunGameService.AllowSellAndTrade();
                _ = FunGameService.UpdateRegionWeather();
            });
            TaskScheduler.Shared.AddRecurringTask("刷新存档缓存", TimeSpan.FromMinutes(1), () =>
            {
                string directoryPath = $@"{AppDomain.CurrentDomain.BaseDirectory}configs/saved";
                if (Directory.Exists(directoryPath))
                {
                    string[] filePaths = Directory.GetFiles(directoryPath);
                    foreach (string filePath in filePaths)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(filePath);
                        PluginConfig pc = new("saved", fileName);
                        pc.LoadConfig();
                        if (pc.Count > 0)
                        {
                            User user = FunGameService.GetUser(pc);
                            // 将用户存入缓存
                            FunGameConstant.UserIdAndUsername[user.Id] = user;
                            bool updateQuest = false;
                            bool updateExplore = false;
                            // 任务结算
                            EntityModuleConfig<Quest> quests = new("quests", user.Id.ToString());
                            quests.LoadConfig();
                            if (quests.Count > 0 && FunGameService.SettleQuest(user, quests))
                            {
                                quests.SaveConfig();
                                updateQuest = true;
                            }
                            // 探索结算
                            PluginConfig pc2 = new("exploring", user.Id.ToString());
                            pc2.LoadConfig();
                            if (pc2.Count > 0 && FunGameService.SettleExploreAll(pc2, user))
                            {
                                pc2.SaveConfig();
                                updateExplore = true;
                            }
                            if (updateQuest || updateExplore)
                            {
                                user.LastTime = DateTime.Now;
                                pc.Add("user", user);
                                pc.SaveConfig();
                            }
                            if (FunGameConstant.UserLastVisitStore.TryGetValue(user.Id, out LastStoreModel? value) && value != null && (DateTime.Now - value.LastTime).TotalMinutes > 2)
                            {
                                FunGameConstant.UserLastVisitStore.Remove(user.Id);
                            }
                        }
                    }
                    Controller.WriteLine("读取 FunGame 存档缓存", LogLevel.Debug);
                }
            }, true);
            TaskScheduler.Shared.AddTask("刷新每日任务", new TimeSpan(4, 0, 0), () =>
            {
                // 刷新每日任务
                Task.Run(() =>
                {
                    string directoryPath = $@"{AppDomain.CurrentDomain.BaseDirectory}configs/quests";
                    if (Directory.Exists(directoryPath))
                    {
                        string[] filePaths = Directory.GetFiles(directoryPath);
                        foreach (string filePath in filePaths)
                        {
                            string fileName = Path.GetFileNameWithoutExtension(filePath);
                            EntityModuleConfig<Quest> quests = new("quests", fileName);
                            quests.Clear();
                            FunGameService.CheckQuestList(quests);
                            quests.SaveConfig();
                        }
                        Controller.WriteLine("刷新每日任务");
                    }
                });
                Task.Run(() =>
                {
                    // 刷新每天登录
                    FunGameService.UserNotice.Clear();
                    // 刷新签到
                    string directoryPath = $@"{AppDomain.CurrentDomain.BaseDirectory}configs/saved";
                    if (Directory.Exists(directoryPath))
                    {
                        string[] filePaths = Directory.GetFiles(directoryPath);
                        foreach (string filePath in filePaths)
                        {
                            string fileName = Path.GetFileNameWithoutExtension(filePath);
                            PluginConfig pc = new("saved", fileName);
                            pc.LoadConfig();
                            pc.Add("signed", false);
                            pc.Add("logon", false);
                            pc.Add("exploreTimes", FunGameConstant.MaxExploreTimes);
                            pc.SaveConfig();
                        }
                        Controller.WriteLine("刷新签到");
                    }
                });
                Task.Run(() =>
                {
                    // 刷新商店
                    string directoryPath = $@"{AppDomain.CurrentDomain.BaseDirectory}configs/stores";
                    if (Directory.Exists(directoryPath))
                    {
                        string[] filePaths = Directory.GetFiles(directoryPath);
                        foreach (string filePath in filePaths)
                        {
                            string fileName = Path.GetFileNameWithoutExtension(filePath);
                            if (long.TryParse(fileName, out long userId) && FunGameConstant.UserIdAndUsername.TryGetValue(userId, out User? user) && user != null)
                            {
                                EntityModuleConfig<Store> store = new("stores", fileName);
                                store.LoadConfig();
                                store.Remove("daily");
                                string[] stores = [.. store.Keys];
                                foreach (string key in stores)
                                {
                                    Store? s = store.Get(key);
                                    if (s != null && s.AutoRefresh && s.NextRefreshDate.Date == DateTime.Today)
                                    {
                                        store.Remove(key);
                                    }
                                }
                                FunGameService.CheckDailyStore(store, user);
                                store.SaveConfig();
                            }
                        }
                        Controller.WriteLine("刷新商店");
                    }
                });
                // 刷新活动缓存
                FunGameService.GetEventCenter();
            });
            TaskScheduler.Shared.AddRecurringTask("刷新boss", TimeSpan.FromHours(1), () =>
            {
                FunGameService.GenerateBoss();
                Controller.WriteLine("刷新boss");
            }, true);
            TaskScheduler.Shared.AddRecurringTask("刷新活动缓存", TimeSpan.FromHours(4), () =>
            {
                FunGameService.GetEventCenter();
                Controller.WriteLine("刷新活动缓存");
            }, true);
        }

        /// <summary>
        /// 向客户端推送事件
        /// </summary>
        /// <param name="msg"></param>
        public async Task PushMessage(string openid, string msg)
        {
            Dictionary<string, object> data = [];
            data.Add(nameof(openid), openid);
            data.Add(nameof(msg), msg);
            Controller.WriteLine("向客户端推送事件", LogLevel.Debug);
            List<IServerModel> failedModels = await SendAnonymousGameServerMessage(_clientModels, data);
            failedModels.ForEach(model => _clientModels.Remove(model));
        }

        /// <summary>
        /// 接收并处理匿名服务器消息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public override async Task<Dictionary<string, object>> AnonymousGameServerHandler(IServerModel model, Dictionary<string, object> data)
        {
            Dictionary<string, object> result = [];
            Controller.WriteLine("接收匿名服务器消息", LogLevel.Debug);

            long groupid = Controller.JSON.GetObject<long>(data, "groupid");
            if (groupid > 0)
            {
                result["groupid"] = groupid;
            }
            string msg = "";
            if (data.Count > 0)
            {
                // 根据服务器和客户端的数据传输约定，自行处理 data，并返回。
                string command = Controller.JSON.GetObject<string>(data, "command") ?? "";
                switch (command.Trim().ToLower())
                {
                    case "scadd":
                        msg = SCAdd(data);
                        break;
                    case "sclist":
                        msg = SCList(data);
                        break;
                    case "screcord":
                        msg = SCRecord(data);
                        break;
                    case "att":
                        break;
                    default:
                        msg = "匿名服务器已经收到消息了";
                        break;
                }
                await Task.Delay(1);
            }
            if (msg.Trim() != "")
            {
                result["msg"] = msg;
            }

            return result;
        }

        public string SCAdd(Dictionary<string, object> data)
        {
            string result = "";

            using SQLHelper? sql = Controller.GetSQLHelper();
            if (sql != null)
            {
                try
                {
                    long qq = Controller.JSON.GetObject<long>(data, "qq");
                    long groupid = Controller.JSON.GetObject<long>(data, "groupid");
                    double sc = Controller.JSON.GetObject<double>(data, "sc");
                    sql.NewTransaction();
                    sql.Script = "select * from saints where qq = @qq and `group` = @group";
                    sql.Parameters.Add("qq", qq);
                    sql.Parameters.Add("group", groupid);
                    sql.ExecuteDataSet();
                    string content = Controller.JSON.GetObject<string>(data, "content") ?? "";
                    string record = "";
                    if (sql.Success)
                    {
                        record = Convert.ToString(sql.DataSet.Tables[0].Rows[0]["record"]) ?? "";
                    }
                    record = $"{DateTime.Now:MM/dd HH:mm}：{content}（{(sc < 0 ? "-" : "+") + Math.Abs(sc)}）\r\n{record}";
                    record = string.Join("\r\n", record.Split("\r\n", StringSplitOptions.RemoveEmptyEntries).Take(10));
                    if (sql.Success)
                    {
                        sql.Script = "update saints set sc = sc + @sc, record = @record where qq = @qq and `group` = @group";
                    }
                    else
                    {
                        sql.Script = "insert into saints(qq, sc, `group`, record) values(@qq, @sc, @group, @record)";
                    }
                    sql.Parameters.Add("sc", sc);
                    sql.Parameters.Add("qq", qq);
                    sql.Parameters.Add("group", groupid);
                    sql.Parameters.Add("record", record);
                    sql.Execute();
                    if (sql.Success)
                    {
                        Controller.WriteLine($"用户 {qq} 的圣人点数增加了 {sc}", LogLevel.Debug);
                        sql.Commit();
                    }
                    else
                    {
                        sql.Rollback();
                    }
                }
                catch (Exception e)
                {
                    result = e.ToString();
                    sql.Rollback();
                }
            }
            else result = "无法调用此接口：SQL 服务不可用。";

            return result;
        }

        public string SCList(Dictionary<string, object> data)
        {
            string result = "";

            SQLHelper? sql = Controller.SQLHelper;
            if (sql != null)
            {
                long userQQ = Controller.JSON.GetObject<long>(data, "qq");
                (bool userHas, double userSC, int userTop, string userRemark) = (false, 0, 0, "");
                long groupid = Controller.JSON.GetObject<long>(data, "groupid");
                bool reverse = Controller.JSON.GetObject<bool>(data, "reverse");
                if (!reverse)
                {
                    result = $"☆--- OSMTV 圣人排行榜 TOP10 ---☆\r\n统计时间：{DateTime.Now.ToString(General.GeneralDateTimeFormatChinese)}\r\n";
                }
                else
                {
                    result = $"☆--- OSMTV 出生排行榜 TOP10 ---☆\r\n统计时间：{DateTime.Now.ToString(General.GeneralDateTimeFormatChinese)}\r\n";
                }
                sql.Script = "select * from saints where `group` = @group order by sc" + (!reverse ? " desc" : "");
                sql.Parameters.Add("group", groupid);
                sql.ExecuteDataSet();
                if (sql.Success && sql.DataSet.Tables.Count > 0)
                {
                    int count = 0;
                    foreach (DataRow dr in sql.DataSet.Tables[0].Rows)
                    {
                        count++;
                        long qq = Convert.ToInt64(dr["qq"]);
                        double sc = Convert.ToDouble(dr["sc"]);
                        string remark = Convert.ToString(dr["remark"]) ?? "";
                        if (reverse)
                        {
                            sc = -sc;
                            remark = remark.Replace("+", "-");
                        }
                        if (qq == userQQ)
                        {
                            userHas = true;
                            userSC = sc;
                            userTop = count;
                            userRemark = remark;
                        }
                        if (count > 10) continue;
                        if (!reverse)
                        {
                            result += $"{count}. 用户：{qq}，圣人点数：{sc} 分{(remark.Trim() != "" ? $" ({remark})" : "")}\r\n";
                        }
                        else
                        {
                            result += $"{count}. 用户：{qq}，出生点数：{sc} 分{(remark.Trim() != "" ? $" ({remark})" : "")}\r\n";
                        }
                    }
                    if (!reverse && userHas)
                    {
                        result += $"你的圣人点数为：{userSC} 分{(userRemark.Trim() != "" ? $"（{userRemark}）" : "")}，排在第 {userTop} / {sql.DataSet.Tables[0].Rows.Count} 名。\r\n" +
                            $"本排行榜仅供娱乐，不代表任何官方立场或真实情况。";
                    }
                    if (reverse && userHas)
                    {
                        result += $"你的出生点数为：{userSC} 分{(userRemark.Trim() != "" ? $"（{userRemark}）" : "")}，排在出生榜第 {userTop} / {sql.DataSet.Tables[0].Rows.Count} 名。\r\n" +
                            $"本排行榜仅供娱乐，不代表任何官方立场或真实情况。";
                    }
                }
                else
                {
                    if (reverse)
                    {
                        result = "出生榜目前没有任何数据。";
                    }
                    else
                    {
                        result = "圣人榜目前没有任何数据。";
                    }
                }
            }
            else result = "无法调用此接口：SQL 服务不可用。";

            return result.Trim();
        }

        public string SCRecord(Dictionary<string, object> data)
        {
            string result = "";

            SQLHelper? sql = Controller.SQLHelper;
            if (sql != null)
            {
                long userQQ = Controller.JSON.GetObject<long>(data, "qq");
                long groupid = Controller.JSON.GetObject<long>(data, "groupid");
                result = $"☆--- 圣人点数信息 ---☆\r\n统计时间：{DateTime.Now.ToString(General.GeneralDateTimeFormatChinese)}\r\n";
                sql.Script = "select * from saints where `group` = @group order by sc desc";
                sql.Parameters.Add("group", groupid);
                sql.Parameters.Add("qq", userQQ);
                sql.ExecuteDataSet();
                if (sql.Success && sql.DataSet.Tables.Count > 0)
                {
                    Dictionary<int, DataRow> dict = sql.DataSet.Tables[0].AsEnumerable().Select((r, i) => new { Index = i + 1, Row = r }).ToDictionary(c => c.Index, c => c.Row);
                    int index = dict.Where(kv => Convert.ToInt64(kv.Value["qq"]) == userQQ).Select(r => r.Key).FirstOrDefault();
                    if (index != 0 && dict.TryGetValue(index, out DataRow? dr) && dr != null)
                    {
                        long qq = Convert.ToInt64(dr["qq"]);
                        double sc = Convert.ToDouble(dr["sc"]);
                        string remark = Convert.ToString(dr["remark"]) ?? "";
                        string record = Convert.ToString(dr["record"]) ?? "";
                        result += $"用户：{qq}，圣人点数：{sc} 分{(remark.Trim() != "" ? $" ({remark})" : "")}，排在圣人榜第 {index} / {sql.DataSet.Tables[0].Rows.Count} 名。\r\n" +
                            $"{(record != "" ? "显示近期点数变动信息：\r\n" + record + "\r\n" : "")}本系统仅供娱乐，不代表任何官方立场或真实情况。";
                    }
                    else
                    {
                        result = "你在这个群没有任何历史记录。";
                    }
                }
                else
                {
                    result = "你在这个群没有任何历史记录。";
                }
            }
            else result = "无法调用此接口：SQL 服务不可用。";

            return result.Trim();
        }

        public override Task<Dictionary<string, object>> GamingMessageHandler(IServerModel model, GamingType type, Dictionary<string, object> data)
        {
            throw new NotImplementedException();
        }

        public override bool StartServer(GamingObject obj, params object[] args)
        {
            throw new NotImplementedException();
        }
    }
}
