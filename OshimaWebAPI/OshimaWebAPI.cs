using System.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.SQLScript.Entity;
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
                //FunGameController controller = new(new Logger<FunGameController>(new LoggerFactory()));
                //Controller.WriteLine(Controller.JSON.GetObject<string>(controller.ShowDailyStore(1)) ?? "test");

                using SQLHelper? sql = Controller.GetSQLHelper();
                if (sql != null)
                {
                    sql.NewTransaction();
                    try
                    {
                        //sql.Execute(StoreQuery.Insert_Store(sql, "测试商店1", null, null));
                        //sql.Execute(GoodsQuery.Insert_Goods(sql, "测试商品1", "测试商品描述1", 110));
                        //sql.Execute(GoodsQuery.Insert_Goods(sql, "测试商品2", "测试商品描述2", 120));
                        //sql.Execute(GoodsQuery.Insert_Goods(sql, "测试商品3", "测试商品描述3", 130));
                        //sql.Execute(StoreGoodsQuery.Insert_StoreGoods(sql, 1, 1));
                        //sql.Execute(StoreGoodsQuery.Insert_StoreGoods(sql, 1, 2));
                        //sql.Execute(StoreGoodsQuery.Insert_StoreGoods(sql, 1, 3));
                        //sql.Execute(GoodItemsQuery.Insert_GoodItem(sql, 1, (long)AccessoryID.攻击之爪50));
                        //sql.Execute(GoodItemsQuery.Insert_GoodItem(sql, 2, (long)ConsumableID.小经验书));
                        //sql.Execute(GoodItemsQuery.Insert_GoodItem(sql, 3, (long)SpecialItemID.技能卷轴));
                        //sql.Execute(GoodPricesQuery.Insert_GoodPrice(sql, 1, General.GameplayEquilibriumConstant.InGameCurrency, 110));
                        //sql.Execute(GoodPricesQuery.Insert_GoodPrice(sql, 2, General.GameplayEquilibriumConstant.InGameCurrency, 120));
                        //sql.Execute(GoodPricesQuery.Insert_GoodPrice(sql, 3, General.GameplayEquilibriumConstant.InGameCurrency, 130));
                        
                        //sql.Execute(StoreQuery.Insert_Store(sql, "测试商店2", null, null));
                        //sql.Execute(GoodsQuery.Insert_Goods(sql, "测试商品4", "测试商品描述4", 111));
                        //sql.Execute(GoodsQuery.Insert_Goods(sql, "测试商品5", "测试商品描述5", 122));
                        //sql.Execute(GoodsQuery.Insert_Goods(sql, "测试商品6", "测试商品描述6", 133));
                        //sql.Execute(StoreGoodsQuery.Insert_StoreGoods(sql, 2, 4));
                        //sql.Execute(StoreGoodsQuery.Insert_StoreGoods(sql, 2, 5));
                        //sql.Execute(StoreGoodsQuery.Insert_StoreGoods(sql, 2, 6));
                        //sql.Execute(GoodItemsQuery.Insert_GoodItem(sql, 4, (long)AccessoryID.攻击之爪20));
                        //sql.Execute(GoodItemsQuery.Insert_GoodItem(sql, 5, (long)ConsumableID.中经验书));
                        //sql.Execute(GoodItemsQuery.Insert_GoodItem(sql, 6, (long)SpecialItemID.智慧之果));
                        //sql.Execute(GoodPricesQuery.Insert_GoodPrice(sql, 4, General.GameplayEquilibriumConstant.InGameCurrency, 111));
                        //sql.Execute(GoodPricesQuery.Insert_GoodPrice(sql, 5, General.GameplayEquilibriumConstant.InGameCurrency, 122));
                        //sql.Execute(GoodPricesQuery.Insert_GoodPrice(sql, 6, General.GameplayEquilibriumConstant.InGameCurrency, 133));

                        // 单一商店测试

                        Store store = new("");
                        sql.ExecuteDataSet(StoreQuery.Select_StoreById(sql, 1));
                        if (sql.Success)
                        {
                            DataRow dr = sql.DataSet.Tables[0].Rows[0];
                            store.Id = (long)dr[StoreQuery.Column_Id];
                            store.Name = (string)dr[StoreQuery.Column_StoreName];
                            if (dr[StoreQuery.Column_StartTime] != DBNull.Value)
                            {
                                store.StartTime = (DateTime)dr[StoreQuery.Column_StartTime];
                            }
                            if (dr[StoreQuery.Column_EndTime] != DBNull.Value)
                            {
                                store.EndTime = (DateTime)dr[StoreQuery.Column_EndTime];
                            }
                        }

                        List<Goods> goodsList = [];

                        sql.ExecuteDataSet(GoodsQuery.Select_AllGoodsWithItemAndPrice(sql, 1));
                        if (sql.Success)
                        {
                            goodsList.AddRange(GetGoods(sql.DataSet));
                        }
                        DataSet ds2 = sql.ExecuteDataSet(GoodsQuery.Select_AllGoodsWithItemAndPrice(sql, 2));
                        if (sql.Success)
                        {
                            goodsList.AddRange(GetGoods(sql.DataSet));
                        }
                        DataSet ds3 = sql.ExecuteDataSet(GoodsQuery.Select_AllGoodsWithItemAndPrice(sql, 3));
                        if (sql.Success)
                        {
                            goodsList.AddRange(GetGoods(sql.DataSet));
                        }
                        foreach (Goods goods in goodsList)
                        {
                            store.Goods.Add(goods.Id, goods);
                        }

                        Controller.WriteLine(store.ToString());

                        // 所有商店测试

                        List<Store> stores = [];

                        sql.ExecuteDataSet(StoreQuery.Select_AllGoodsInStore(sql));
                        if (sql.Success)
                        {
                            DataSet ds = sql.DataSet;
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                Store storeTemp = new("")
                                {
                                    Id = (long)dr["StoreId"],
                                    Name = (string)dr[StoreQuery.Column_StoreName]
                                };
                                if (stores.FirstOrDefault(s => s.Id == storeTemp.Id) is Store store2)
                                {
                                    storeTemp = store2;
                                }
                                else
                                {
                                    stores.Add(storeTemp);
                                }

                                Goods goods = new()
                                {
                                    Id = (long)dr["GoodsId"],
                                    Name = (string)dr["GoodsName"],
                                    Description = (string)dr[GoodsQuery.Column_Description],
                                    Stock = Convert.ToInt32(dr[GoodsQuery.Column_Stock])
                                };
                                Item item = Factory.OpenFactory.GetInstance<Item>((long)dr[GoodsItemsQuery.Column_ItemId], "", []);
                                goods.Items.Add(item);
                                string currency = (string)dr[GoodsPricesQuery.Column_Currency];
                                double price = (double)dr[GoodsPricesQuery.Column_Price];
                                goods.Prices.Add(currency, price);

                                storeTemp.Goods.Add(goods.Id, goods);
                            }
                        }

                        foreach (Store store2 in stores)
                        {
                            Controller.WriteLine(store2.ToString());
                        }
                    }
                    catch (Exception e)
                    {
                        sql.Rollback();
                        Controller.Error(e);
                    }
                    finally
                    {
                        sql.Commit();
                    }
                }
            }
        }

        private List<Goods> GetGoods(DataSet ds)
        {
            DataSetToString(ds);
            List<Goods> list = [];

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                Goods goods = new()
                {
                    Id = (long)dr[GoodsQuery.Column_Id],
                    Name = (string)dr[GoodsQuery.Column_Name],
                    Description = (string)dr[GoodsQuery.Column_Description],
                    Stock = Convert.ToInt32(dr[GoodsQuery.Column_Stock])
                };
                Item item = Factory.OpenFactory.GetInstance<Item>((long)dr[GoodsItemsQuery.Column_ItemId], "", []);
                goods.Items.Add(item);
                string currency = (string)dr[GoodsPricesQuery.Column_Currency];
                double price = (double)dr[GoodsPricesQuery.Column_Price];
                goods.Prices.Add(currency, price);
                list.Add(goods);
            }

            return list;
        }

        private string DataSetToString(DataSet ds)
        {
            if (ds == null || ds.Tables.Count == 0)
            {
                return "DataSet is null or empty.";
            }

            string result = "";
            foreach (DataTable table in ds.Tables)
            {
                result += $"Table Name: {table.TableName}\n";
                foreach (DataColumn column in table.Columns)
                {
                    result += $"{column.ColumnName}\t";
                }
                result += "\n";
                foreach (DataRow row in table.Rows)
                {
                    foreach (object? item in row.ItemArray)
                    {
                        result += $"{item}\t";
                    }
                    result += "\n";
                }
            }
            return result;
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
