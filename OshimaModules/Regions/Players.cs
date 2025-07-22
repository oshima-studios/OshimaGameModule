using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;

namespace Oshima.FunGame.OshimaModules.Regions
{
    public class 铎京城 : OshimaRegion
    {
        public 铎京城()
        {
            Id = 0;
            Name = "铎京城";
            Description = "铎京是大陆的科技中枢，是大陆中的一个城邦国家。其首都铎京市是一座具有独立权势的商业城市，以精密机械和能量研究、强大的现代科技与军事实力闻名。";
            Category = "此地";
            Weathers.Add("酷暑", 39);
            Weathers.Add("炙热", 33);
            Weathers.Add("晴朗", 26);
            Weathers.Add("多云", 20);
            Weathers.Add("暴雨", 14);
            Weathers.Add("寒冷", 6);
            Weathers.Add("严寒", -7);
            Weathers.Add("霜冻", -18);
            ChangeRandomWeather();
        }

        public override Store? VisitStore(EntityModuleConfig<Store> stores, User user, string storeName)
        {
            EntityModuleConfig<Store> storeTemplate = new("stores", "dokyo");
            storeTemplate.LoadConfig();
            Store? template = storeTemplate.Get(storeName);

            if (template is null)
            {
                return null;
            }

            if (template.NextRefreshDate < DateTime.Now)
            {
                template.NextRefreshDate = DateTime.Today.AddHours(4);
                template.UpdateRefreshTime(template.NextRefreshDate);
                storeTemplate.Add(storeName, template);
                storeTemplate.SaveConfig();
            }

            if (template.GlobalStock)
            {
                return template;
            }

            Store? store = stores.Get(storeName);

            if (store is null)
            {
                template.NextRefreshGoods.Clear();
                stores.Add(storeName, template);
                stores.SaveConfig();
                stores.LoadConfig();
                store = stores.Get(storeName);
            }
            else
            {
                if (template.GetNewerGoodsOnVisiting)
                {
                    Dictionary<string, int> goodsNameAndStock = store.Goods.Values.ToDictionary(g => g.Name, g => g.Stock);
                    Dictionary<string, Dictionary<long, int>> usersBuyCount = store.Goods.Values.ToDictionary(g => g.Name, g => g.UsersBuyCount);
                    template.NextRefreshGoods.Clear();
                    stores.Add(storeName, template);
                    stores.SaveConfig();
                    stores.LoadConfig();
                    store = stores.Get(storeName);
                    if (store != null)
                    {
                        foreach (Goods goods in store.Goods.Values)
                        {
                            if (goodsNameAndStock.TryGetValue(goods.Name, out int stock) && stock < goods.Stock)
                            {
                                goods.Stock = stock;
                            }
                            if (usersBuyCount.TryGetValue(goods.Name, out Dictionary<long, int>? userBuyCount) && userBuyCount != null)
                            {
                                foreach (long uid in userBuyCount.Keys)
                                {
                                    goods.UsersBuyCount[uid] = userBuyCount[uid];
                                }
                            }
                        }
                        stores.Add(storeName, store);
                        stores.SaveConfig();
                    }
                }
            }

            return store;
        }

        public override void SaveGlobalStore(Store store, string storeName)
        {
            EntityModuleConfig<Store> storeTemplate = new("stores", "dokyo");
            storeTemplate.LoadConfig();
            storeTemplate.Add(storeName, store);
            storeTemplate.SaveConfig();
        }
    }
}
