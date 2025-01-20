using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Oshima.Core;
using Oshima.Core.Constant;
using Oshima.FunGame.OshimaServers.Service;

namespace Oshima.FunGame.OshimaServers
{
    public class OshimaServer : ServerPlugin
    {
        public override string Name => OshimaGameModuleConstant.Server;

        public override string Description => OshimaGameModuleConstant.Description;

        public override string Version => OshimaGameModuleConstant.Version;

        public override string Author => OshimaGameModuleConstant.Author;

        public override void ProcessInput(string input)
        {
            if (input == "fungametest")
            {
                FunGameSimulation.StartSimulationGame(true, true);
            }
            // OSM指令
            if (input.StartsWith(".osm", StringComparison.CurrentCultureIgnoreCase))
            {
                //MasterCommand.Execute(read, GeneralSettings.Master, false, GeneralSettings.Master, false);
                Controller.WriteLine("试图使用 .osm 指令：" + input);
            }
        }

        public override void AfterLoad(ServerPluginLoader loader, params object[] objs)
        {
            OSMCore.InitOSMCore();
        }
    }
}
