using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Common.Event;
using Oshima.Core.Configs;
using Oshima.Core.Constant;
using Oshima.FunGame.OshimaModules.Characters;
using Oshima.FunGame.OshimaModules.Items;
using Oshima.FunGame.OshimaServers.Service;
using Oshima.FunGame.WebAPI.Constant;
using Oshima.FunGame.WebAPI.Controllers;
using Oshima.FunGame.WebAPI.Models;
using Oshima.FunGame.WebAPI.Services;
using ProjectRedbud.FunGame.SQLQueryExtension;

namespace Oshima.FunGame.WebAPI
{
    public class OshimaWebAPI : WebAPIPlugin, ILoginEvent
    {
        public override string Name => OshimaGameModuleConstant.WebAPI;

        public override string Description => OshimaGameModuleConstant.Description;

        public override string Version => OshimaGameModuleConstant.Version;

        public override string Author => OshimaGameModuleConstant.Author;

        private IServiceScopeFactory? _serviceScopeFactory = null;

        public override void ProcessInput(string input)
        {
            // RainBOT 测试
            using (IServiceScope? scope = _serviceScopeFactory?.CreateScope())
            {
                if (scope != null)
                {
                    // 从作用域中获取 IServiceProvider
                    IServiceProvider serviceProvider = scope.ServiceProvider;

                    try
                    {
                        if (input.Trim() != "")
                        {
                            // 获取 RainBOTService 实例
                            RainBOTService bot = serviceProvider.GetRequiredService<RainBOTService>();
                            ThirdPartyMessage message = new()
                            {
                                IsGroup = true,
                                GroupOpenId = "1",
                                AuthorOpenId = "1",
                                OpenId = "1",
                                Detail = input,
                                Id = "1",
                                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                            };

                            bool result = bot.Handler(message).GetAwaiter().GetResult();

                            if (!result || message.IsCompleted)
                            {
                                Controller.WriteLine(message.Result);
                            }
                        }

                        if (input == "test")
                        {
                            //FunGameController funGameController = serviceProvider.GetRequiredService<FunGameController>();
                            //Controller.WriteLine(Controller.JSON.GetObject<string>(funGameController.ShowDailyStore(1)) ?? "test");
                        }
                    }
                    catch (Exception e)
                    {
                        Controller.Error(e);
                    }
                }
            }

            if (input == "testuser")
            {
                using SQLHelper? sql = Controller.GetSQLHelper();
                if (sql != null)
                {
                    try
                    {
                        sql.NewTransaction();

                        // 注册用户
                        sql.RegisterUser("un", "pw", "1@2.3", "");
                        User? user = sql.GetUserByUsernameAndPassword("un", "pw");

                        if (user != null)
                        {
                            Controller.WriteLine($"User found: ID = {user.Id}, Username = {user.Username}, Email = {user.Email}");

                            // 测试更新密码
                            string newPassword = "new_pw";
                            sql.UpdatePassword("un", newPassword);
                            User? updatedUser = sql.GetUserByUsernameAndPassword("un", newPassword);

                            if (updatedUser != null)
                            {
                                Controller.WriteLine($"Password updated successfully for user: {updatedUser.Username}");
                            }
                            else
                            {
                                Controller.WriteLine("Failed to update password.");
                            }

                            // 测试更新游戏时间
                            int gameTimeToAdd = 60; // 分钟
                            sql.UpdateGameTime("un", gameTimeToAdd);
                            User? gameTimeUser = sql.GetUserByUsernameAndPassword("un", newPassword); // 注意：密码已更新

                            if (gameTimeUser != null)
                            {
                                Controller.WriteLine($"Game time updated successfully. New game time: {gameTimeUser.GameTime} minutes");
                            }
                            else
                            {
                                Controller.WriteLine("Failed to update game time.");
                            }

                            // 测试加载库存
                            if (sql.LoadInventory(user))
                            {
                                Controller.WriteLine($"Inventory loaded successfully for user: {user.Username}");
                                Controller.WriteLine($"Inventory Name: {user.Inventory.Name}, Credits: {user.Inventory.Credits}, Materials: {user.Inventory.Materials}");

                                // 测试更新库存 Credits
                                double newCredits = 100.50;
                                sql.UpdateInventoryCredits(user.Id, newCredits);
                                sql.LoadInventory(user); // 重新加载库存

                                if (user.Inventory.Credits == (double)newCredits)
                                {
                                    Controller.WriteLine($"Credits updated successfully. New credits: {user.Inventory.Credits}");
                                }
                                else
                                {
                                    Controller.WriteLine("Failed to update credits.");
                                }

                                // 测试更新库存 Materials
                                double newMaterials = 50.75;
                                sql.UpdateInventoryMaterials(user.Id, newMaterials);
                                sql.LoadInventory(user); // 重新加载库存

                                if (user.Inventory.Materials == (double)newMaterials)
                                {
                                    Controller.WriteLine($"Materials updated successfully. New materials: {user.Inventory.Materials}");
                                }
                                else
                                {
                                    Controller.WriteLine("Failed to update materials.");
                                }

                                // 测试角色
                                Character character = new OshimaShiya();
                                user.Inventory.Characters.Add(character);

                                // 测试物品
                                Item item = new 攻击之爪25();
                                user.Inventory.Items.Add(item);

                                sql.UpdateInventory(user.Inventory); // 更新库存

                                sql.LoadInventory(user); // 重新加载库存
                                if (user.Inventory.Characters.Count > 0 && user.Inventory.Items.Count > 0)
                                {
                                    Controller.WriteLine($"Characters and Items updated successfully.");
                                }
                                else
                                {
                                    Controller.WriteLine("Failed to update characters and items.");
                                }

                            }
                            else
                            {
                                Controller.WriteLine("Failed to load inventory.");
                            }
                        }
                        else
                        {
                            Controller.WriteLine("User not found");
                        }
                    }
                    catch (Exception e)
                    {
                        Controller.Error(e);
                    }
                    finally
                    {
                        sql.Rollback();
                        Controller.WriteLine("Transaction rolled back in finally block.");
                    }
                }
                else
                {
                    Controller.WriteLine("SQLHelper is null.");
                }
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
                builder.Services.AddScoped<TestController>();
                builder.Services.AddTransient(provider =>
                {
                    SQLHelper? sql = Factory.OpenFactory.GetSQLHelper();
                    if (sql != null) return sql;
                    throw new Milimoe.FunGame.SQLServiceException();
                });
                // 使用 Configure<BotConfig> 从配置源绑定
                builder.Services.Configure<BotConfig>(builder.Configuration.GetSection("Bot"));
            }
            WebAPIAuthenticator.WebAPICustomBearerTokenAuthenticator += CustomBearerTokenAuthenticator;
        }

        public override void OnWebAPIStarted(params object[] objs)
        {
            if (objs.Length > 0 && objs[0] is WebApplication app)
            {
                _serviceScopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
                Controller.WriteLine("获取到：IServiceScopeFactory");
            }
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
