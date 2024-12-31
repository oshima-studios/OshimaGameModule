using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Oshima.Core.Configs;
using Oshima.Core.Constant;
using Oshima.Core.Utils;
using Oshima.FunGame.OshimaModules;
using TaskScheduler = Milimoe.FunGame.Core.Api.Utility.TaskScheduler;

namespace Oshima.Core.WebAPI
{
    public class OshimaWebAPI : WebAPIPlugin
    {
        public override string Name => OshimaGameModuleConstant.WebAPI;

        public override string Description => OshimaGameModuleConstant.Description;

        public override string Version => OshimaGameModuleConstant.Version;

        public override string Author => OshimaGameModuleConstant.Author;

        public override void ProcessInput(string input)
        {
            if (input.StartsWith("fungametest"))
            {
                FunGameSimulation.StartSimulationGame(true, true);
            }
            // OSM指令
            if (input.Length >= 4 && input[..4].Equals(".osm", StringComparison.CurrentCultureIgnoreCase))
            {
                //MasterCommand.Execute(read, GeneralSettings.Master, false, GeneralSettings.Master, false);
                Controller.WriteLine("试图使用 .osm 指令：" + input);
            }
        }

        public override void AfterLoad(params object[] objs)
        {
            Statics.RunningPlugin = this;
            Controller.NewSQLHelper();
            Controller.NewMailSender();
            GeneralSettings.LoadSetting();
            GeneralSettings.SaveConfig();
            QQOpenID.LoadConfig();
            QQOpenID.SaveConfig();
            Daily.InitDaily();
            SayNo.InitSayNo();
            Ignore.InitIgnore();
            FunGameService.InitFunGame();
            FunGameSimulation.InitFunGameSimulation();
            WebAPIAuthenticator.WebAPICustomBearerTokenAuthenticator += CustomBearerTokenAuthenticator;
            TaskScheduler.Shared.AddTask("重置每日运势", new TimeSpan(0, 0, 0), () =>
            {
                Controller.WriteLine("已重置所有人的今日运势");
                Daily.ClearDaily();
            });
            TaskScheduler.Shared.AddTask("重置交易冷却1", new TimeSpan(9, 0, 0), () =>
            {
                Controller.WriteLine("重置物品交易冷却时间");
                _ = FunGameService.AllowSellAndTrade();
            });
            TaskScheduler.Shared.AddTask("重置交易冷却2", new TimeSpan(15, 0, 0), () =>
            {
                Controller.WriteLine("重置物品交易冷却时间");
                _ = FunGameService.AllowSellAndTrade();
            });
            TaskScheduler.Shared.AddRecurringTask("刷新存档缓存", TimeSpan.FromSeconds(20), () =>
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
                            FunGameService.UserIdAndUsername[user.Id] = user.Username;
                        }
                    }
                    Controller.WriteLine("读取 FunGame 存档缓存");
                }
            }, true);
            TaskScheduler.Shared.AddTask("刷新每日任务", new TimeSpan(4, 0, 0), () =>
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
            TaskScheduler.Shared.AddRecurringTask("刷新boss", TimeSpan.FromHours(1), () =>
            {
                FunGameService.GenerateBoss();
                Controller.WriteLine("刷新boss");
            }, true);
        }

        private string CustomBearerTokenAuthenticator(string token)
        {
            if (GeneralSettings.TokenList.Contains(token))
            {
                return "APIUser"; 
            }
            return "";
        }
    }
}
