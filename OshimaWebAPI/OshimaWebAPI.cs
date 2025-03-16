using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Common.Event;
using Oshima.Core.Configs;
using Oshima.Core.Constant;
using Oshima.FunGame.OshimaServers.Service;
using Oshima.FunGame.WebAPI.Constant;
using Oshima.FunGame.WebAPI.Controllers;
using Oshima.FunGame.WebAPI.Models;
using Oshima.FunGame.WebAPI.Services;

namespace Oshima.FunGame.WebAPI
{
    public class OshimaWebAPI : WebAPIPlugin, ILoginEvent
    {
        public override string Name => OshimaGameModuleConstant.WebAPI;

        public override string Description => OshimaGameModuleConstant.Description;

        public override string Version => OshimaGameModuleConstant.Version;

        public override string Author => OshimaGameModuleConstant.Author;

        public override void ProcessInput(string input)
        {
            if (input == "test")
            {
                FunGameController controller = new(new Logger<FunGameController>(new LoggerFactory()));
                Controller.WriteLine(Controller.JSON.GetObject<string>(controller.ShowDailyStore(1)) ?? "test");
            }
        }

        public override void AfterLoad(WebAPIPluginLoader loader, params object[] objs)
        {
            Statics.RunningPlugin = this;
            FunGameService.WebAPIPluginLoader ??= loader;
            Controller.NewSQLHelper();
            Controller.NewMailSender();
            if (objs.Length > 0 && objs[0] is WebApplicationBuilder builder)
            {
                builder.Services.AddScoped<QQBotService>();
                builder.Services.AddScoped<RainBOTService>();
                builder.Services.AddScoped<FunGameController>();
                builder.Services.AddScoped<QQController>();
                builder.Services.AddTransient(provider => {
                    SQLHelper? sql = Factory.OpenFactory.GetSQLHelper();
                    if (sql != null) return sql;
                    throw new Milimoe.FunGame.SQLServiceException();
                });
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

        public void BeforeLoginEvent(object sender, LoginEventArgs e)
        {
            Controller.WriteLine(e.Password);
        }

        public void AfterLoginEvent(object sender, LoginEventArgs e)
        {

        }
    }
}
