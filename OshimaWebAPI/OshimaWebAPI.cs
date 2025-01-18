using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Oshima.Core.Configs;
using Oshima.Core.Constant;
using Oshima.FunGame.OshimaServers.Service;
using Oshima.FunGame.WebAPI.Constant;
using Oshima.FunGame.WebAPI.Models;
using Oshima.FunGame.WebAPI.Services;

namespace Oshima.FunGame.WebAPI
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

        public override void AfterLoad(WebAPIPluginLoader loader, params object[] objs)
        {
            Statics.RunningPlugin = this;
            Controller.NewSQLHelper();
            Controller.NewMailSender();
            if (objs.Length > 0 && objs[0] is WebApplicationBuilder builder)
            {
                builder.Services.AddMemoryCache();
                builder.Services.AddScoped<QQBotService>();
                // 使用 Configure<BotConfig> 从配置源绑定
                builder.Services.Configure<BotConfig>(builder.Configuration.GetSection("Bot"));
            }
            WebAPIAuthenticator.WebAPICustomBearerTokenAuthenticator += CustomBearerTokenAuthenticator;
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
