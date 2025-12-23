using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.Core.Configs;
using Oshima.Core.Constant;
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

        public WebSocketService Service { get; }

        public AnonymousServer()
        {
            Service = new(this);
        }

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
            FunGameService.RefreshNotice();
            TaskScheduler.Shared.AddTask("重置每日运势", new TimeSpan(0, 0, 0), () =>
            {
                Controller.WriteLine("已重置所有人的今日运势");
                Daily.ClearDaily();
                // 刷新活动缓存
                FunGameService.GetEventCenter(null);
                FunGameService.RefreshNotice();
                FunGameService.PreRefreshStore();
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
                FunGameService.RefreshSavedCache();
                FunGameService.RefreshClubData();
                Controller.WriteLine("读取 FunGame 存档缓存", LogLevel.Debug);
                OnlineService.RoomsAutoDisband();
                Controller.WriteLine("清除空闲房间", LogLevel.Debug);
            }, true);
            TaskScheduler.Shared.AddTask("刷新每日任务", new TimeSpan(4, 0, 0), () =>
            {
                // 刷新每日任务
                Task.Run(() =>
                {
                    FunGameService.RefreshDailyQuest();
                    Controller.WriteLine("刷新每日任务");
                });
                Task.Run(() =>
                {
                    FunGameService.RefreshDailySignIn();
                    Controller.WriteLine("刷新签到");
                });
                Task.Run(() =>
                {
                    FunGameService.RefreshStoreData();
                    Controller.WriteLine("刷新商店");
                });
                Task.Run(() =>
                {
                    FunGameService.RefreshMarketData();
                    Controller.WriteLine("刷新市场");
                });
                // 刷新活动缓存
                FunGameService.GetEventCenter(null);
                FunGameService.RefreshNotice();
            });
            TaskScheduler.Shared.AddRecurringTask("刷新boss", TimeSpan.FromHours(1), () =>
            {
                FunGameService.GenerateBoss();
                Controller.WriteLine("刷新boss");
            }, true);
            TaskScheduler.Shared.AddRecurringTask("刷新活动缓存", TimeSpan.FromHours(4), () =>
            {
                FunGameService.GetEventCenter(null);
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
                        msg = Service.SCAdd(data);
                        break;
                    case "sclist":
                        msg = Service.SCList(data);
                        break;
                    case "sclist_backup":
                        msg = Service.SCList_Backup(data);
                        break;
                    case "screcord":
                        msg = Service.SCRecord(data);
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
