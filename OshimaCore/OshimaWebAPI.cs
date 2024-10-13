using Milimoe.FunGame.Core.Library.Common.Addon;
using Oshima.Core.Configs;
using Oshima.Core.Utils;
using Oshima.FunGame.OshimaModules;

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
            // OSM指令
            if (input.Length >= 4 && input[..4].Equals(".osm", StringComparison.CurrentCultureIgnoreCase))
            {
                //MasterCommand.Execute(read, GeneralSettings.Master, false, GeneralSettings.Master, false);
                Controller.WriteLine("试图使用 .osm 指令：" + input);
            }
        }

        public override void AfterLoad(params object[] objs)
        {
            GeneralSettings.LoadSetting();
            GeneralSettings.SaveConfig();
            QQOpenID.LoadConfig();
            QQOpenID.SaveConfig();
            Daily.InitDaily();
            SayNo.InitSayNo();
            Ignore.InitIgnore();
            FunGameSimulation.InitCharacter();
            Task taskTime = Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    try
                    {
                        DateTime now = DateTime.Now;
                        if (now.Hour == 8 && now.Minute == 30 && !Daily.DailyNews)
                        {
                            Daily.DailyNews = true;
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }
                        if (now.Hour == 8 && now.Minute == 31)
                        {
                            Daily.DailyNews = false;
                        }
                        if (now.Hour == 0 && now.Minute == 0 && Daily.ClearDailys)
                        {
                            Daily.ClearDailys = false;
                            // 清空运势
                            Daily.ClearDaily();
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.WriteLine("\r已重置所有人的今日运势。");
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.Write("\r> ");
                        }
                        if (now.Hour == 0 && now.Minute == 1)
                        {
                            Daily.ClearDailys = true;
                        }
                        await Task.Delay(1000);
                    }
                    catch (Exception e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\r" + e);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write("\r> ");
                    }
                }
            });
        }
    }
}
