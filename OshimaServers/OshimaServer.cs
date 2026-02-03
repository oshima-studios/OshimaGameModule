using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Interface.Base.Addons;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Common.Event;
using Oshima.Core;
using Oshima.Core.Constant;
using Oshima.FunGame.OshimaServers.Service;

namespace Oshima.FunGame.OshimaServers
{
    public class OshimaServer : ServerPlugin, IHotReloadAware, IOpenStoreEvent
    {
        public override string Name => OshimaGameModuleConstant.Server;

        public override string Description => OshimaGameModuleConstant.Description;

        public override string Version => OshimaGameModuleConstant.Version;

        public override string Author => OshimaGameModuleConstant.Author;

        public override async void ProcessInput(string input)
        {
            // OSM指令
            if (input.StartsWith(".osm", StringComparison.CurrentCultureIgnoreCase))
            {
                //MasterCommand.Execute(read, GeneralSettings.Master, false, GeneralSettings.Master, false);
                Controller.WriteLine("试图使用 .osm 指令：" + input);
            }
        }

        public override void AfterLoad(ServerPluginLoader loader, params object[] objs)
        {
            FunGameService.ServerPluginLoader ??= loader;
            OSMCore.InitOSMCore();
        }

        public void BeforeOpenStoreEvent(object sender, GeneralEventArgs e)
        {
            if (e.EventMsg != "") Controller.WriteLine(e.EventMsg, Milimoe.FunGame.Core.Library.Constant.LogLevel.Debug);
        }

        public void AfterOpenStoreEvent(object sender, GeneralEventArgs e)
        {
            if (e.EventMsg != "") Controller.WriteLine(e.EventMsg, Milimoe.FunGame.Core.Library.Constant.LogLevel.Debug);
        }

        public void OnBeforeUnload()
        {

        }
    }
}
