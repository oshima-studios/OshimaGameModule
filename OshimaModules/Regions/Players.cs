using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Oshima.FunGame.OshimaModules.Items;
using Oshima.FunGame.OshimaModules.Models;

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
                if (storeName == "dokyo_forge")
                {
                    template = CreateNewForgeStore();
                }
                else if (storeName == "dokyo_horseracing")
                {
                    template = CreateNewHorseRacingStore();
                }
                else if (storeName == "dokyo_cooperative")
                {
                    template = CreateNewCooperativeStore();
                }
                else return null;
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

        public override string GetCurrencyInfo(PluginConfig pc, User user, string storeName)
        {
            if (storeName == "dokyo_forge")
            {
                if (pc.TryGetValue("forgepoints", out object? value) && double.TryParse(value.ToString(), out double points))
                {
                    return $"现有锻造积分：{points:0.##}";
                }
            }
            else if (storeName == "dokyo_horseracing")
            {
                if (pc.TryGetValue("horseRacingPoints", out object? value) && int.TryParse(value.ToString(), out int points))
                {
                    return $"现有赛马积分：{points:0.##}";
                }
            }
            else if (storeName == "dokyo_cooperative")
            {
                if (pc.TryGetValue("cooperativePoints", out object? value) && int.TryParse(value.ToString(), out int points))
                {
                    return $"现有共斗积分：{points:0.##}";
                }
            }
            return "";
        }

        public override void SaveGlobalStore(Store store, string storeName)
        {
            EntityModuleConfig<Store> storeTemplate = new("stores", "dokyo");
            storeTemplate.LoadConfig();
            storeTemplate.Add(storeName, store);
            storeTemplate.SaveConfig();
        }

        public override void UpdateNextRefreshGoods()
        {
            EntityModuleConfig<Store> storeTemplate = new("stores", "dokyo");
            storeTemplate.LoadConfig();

            Store? store = storeTemplate.Get("dokyo_forge");
            if (store is null)
            {
                store = CreateNewForgeStore();
            }
            else
            {
                Store newStore = CreateNewForgeStore();
                store.NextRefreshGoods.Clear();
                store.CopyGoodsToNextRefreshGoods(newStore.Goods);
            }
            storeTemplate.Add("dokyo_forge", store);

            store = storeTemplate.Get("dokyo_horseracing");
            if (store is null)
            {
                store = CreateNewHorseRacingStore();
            }
            else
            {
                Store newStore = CreateNewHorseRacingStore();
                store.NextRefreshGoods.Clear();
                store.CopyGoodsToNextRefreshGoods(newStore.Goods);
            }
            storeTemplate.Add("dokyo_horseracing", store);
            
            store = storeTemplate.Get("dokyo_cooperative");
            if (store is null)
            {
                store = CreateNewCooperativeStore();
            }
            else
            {
                Store newStore = CreateNewCooperativeStore();
                store.NextRefreshGoods.Clear();
                store.CopyGoodsToNextRefreshGoods(newStore.Goods);
            }
            storeTemplate.Add("dokyo_cooperative", store);

            storeTemplate.SaveConfig();
        }

        private static Store CreateNewForgeStore()
        {
            Store store = new("锻造积分商店")
            {
                GetNewerGoodsOnVisiting = true,
                AutoRefresh = true,
                RefreshInterval = 3,
                NextRefreshDate = DateTime.Today.AddHours(4),
                GlobalStock = true,
            };
            Item item = new 大师锻造券();
            store.AddItem(item, -1);
            store.SetPrice(1, "锻造积分", 400);
            Dictionary<OshimaRegion, Item> items = FunGameConstant.ExploreItems.OrderBy(o => Random.Shared.Next()).Take(5).ToDictionary(kv => kv.Key, kv => kv.Value.OrderBy(o => Random.Shared.Next()).First());
            int i = 2;
            foreach (OshimaRegion region in items.Keys)
            {
                store.AddItem(items[region], -1);
                store.SetPrice(i, "锻造积分", 2 * ((int)region.Difficulty + 1));
                i++;
            }
            return store;
        }

        private static Store CreateNewHorseRacingStore()
        {
            Store store = new("赛马积分商店")
            {
                GetNewerGoodsOnVisiting = true,
                AutoRefresh = true,
                RefreshInterval = 1,
                NextRefreshDate = DateTime.Today.AddHours(4),
                GlobalStock = true,
            };
            Item item = new 钻石();
            store.AddItem(item, -1);
            store.SetPrice(1, "赛马积分", 5);
            store.Goods[1].Quota = 300;
            return store;
        }

        private static Store CreateNewCooperativeStore()
        {
            Store store = new("共斗积分商店")
            {
                GetNewerGoodsOnVisiting = true,
                AutoRefresh = true,
                RefreshInterval = 1,
                NextRefreshDate = DateTime.Today.AddHours(4),
                GlobalStock = true,
            };
            Item item = new 奖券();
            store.AddItem(item, -1);
            store.SetPrice(1, "共斗积分", 15);
            store.Goods[1].Quota = 300;
            item = new 十连奖券();
            store.AddItem(item, -1);
            store.SetPrice(2, "共斗积分", 120);
            store.Goods[2].Quota = 100;
            Item[] items = [.. FunGameConstant.CharacterLevelBreakItems.Union(FunGameConstant.SkillLevelUpItems).OrderBy(o => Random.Shared.Next()).Take(5)];
            int i = 3;
            foreach (Item cItem in items)
            {
                store.AddItem(cItem, -1);
                store.SetPrice(i, "共斗积分", 4 * ((int)cItem.QualityType + 1));
                i++;
            }
            return store;
        }
    }
}
