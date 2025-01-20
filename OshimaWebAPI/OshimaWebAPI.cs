using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Oshima.Core.Configs;
using Oshima.Core.Constant;
using Oshima.FunGame.WebAPI.Constant;
using Oshima.FunGame.WebAPI.Controllers;
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
            if (input == "test")
            {
                FunGameController controller = new(new Logger<FunGameController>(new LoggerFactory()));
                Controller.WriteLine(Controller.JSON.GetObject<string>(controller.CreateSaved(1, "测试用户")) ?? "test");
                Controller.WriteLine(Controller.JSON.GetObject<string>(controller.GetItemInfo_Name(1, "鸳鸯眼")) ?? "test");
                Controller.WriteLine(Controller.JSON.GetObject<string>(controller.GetCharacterInfoFromInventory(1, 1, false)) ?? "test");
                Controller.WriteLine(string.Join("\r\n", controller.GetBoss(1)));
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
                builder.Services.AddScoped<RainBOTService>();
                builder.Services.AddScoped<FunGameController>();
                builder.Services.AddScoped<QQController>();
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
