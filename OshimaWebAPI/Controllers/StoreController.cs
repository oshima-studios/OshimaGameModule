using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.WebAPI.Models;

namespace Oshima.FunGame.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StoreController(ILogger<StoreController> logger) : ControllerBase
    {
        private readonly ILogger<StoreController> _logger = logger;

        [HttpGet("{region}/{id}")]
        public IActionResult GetStore(string region, long id)
        {
            try
            {
                EntityModuleConfig<Store> stores = new("stores", region);
                stores.LoadConfig();
                if (stores.Get(id.ToString()) is Store Store)
                {
                    return Ok($"{Store}");
                }
                return NotFound($"商店编号 {id} 不存在。");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error: ");
                return StatusCode(500, "获取商店信息时发生错误，请检查日志。");
            }
        }

        [HttpGet("{region}/{id}/{goodsId}")]
        public IActionResult GetGoods(string region, long id, long goodsId)
        {
            try
            {
                EntityModuleConfig<Store> stores = new("stores", region);
                stores.LoadConfig();
                Store? store = stores.Values.FirstOrDefault(s => s.Id == id);
                if (store != null)
                {
                    if (store.Goods.Values.FirstOrDefault(g => g.Id == goodsId) is Goods goods)
                    {
                        return Ok($"该商品属于商店【{store.Name}】，详情：\r\n{goods}");
                    }
                    else
                    {
                        return NotFound($"商品编号 {goodsId} 不存在。");
                    }
                }
                return NotFound($"商店编号 {id} 不存在。");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error: ");
                return StatusCode(500, "获取商品信息时发生错误，请检查日志。");
            }
        }

        [Authorize(AuthenticationSchemes = "CustomBearer")]
        [HttpPut("{region}/{id}")]
        public IActionResult AddGoodsToStore(string region, long id, [FromBody] GoodsDTO dto)
        {
            try
            {
                EntityModuleConfig<Store> stores = new("stores", region);
                stores.LoadConfig();
                Store? store = stores.Values.FirstOrDefault(s => s.Id == id);
                if (store != null)
                {
                    if (!store.Goods.Values.Any(g => g.Name == dto.Name))
                    {
                        if (Factory.OpenFactory.GetInstance<Item>(dto.Id, dto.Name, dto.Values) is Item item)
                        {
                            store.AddItem(item, dto.Stock, dto.Name, dto.Description);
                            Goods newGoods = store.Goods.Values.Last();
                            if (dto.CurrencyPrice > 0)
                            {
                                newGoods.SetPrice(General.GameplayEquilibriumConstant.InGameCurrency, dto.CurrencyPrice);
                            }
                            if (dto.MaterialPrice > 0)
                            {
                                newGoods.SetPrice(General.GameplayEquilibriumConstant.InGameMaterial, dto.MaterialPrice);
                            }
                            newGoods.ExpireTime = dto.ExpireTime;
                            newGoods.Quota = dto.Quota;
                            store.CopyGoodsToNextRefreshGoods();
                            stores.Add(store.Id.ToString(), store);
                            stores.SaveConfig();

                            return Ok($"商品：\r\n{newGoods}\r\n已成功添加至商店 {id}（{store.Name}）。");
                        }
                        else
                        {
                            return NotFound($"系统中不存在此物品：{dto.Id}.{dto.Name}。");
                        }
                    }
                    else
                    {
                        return NotFound($"同名商品 {dto.Name} 已存在于该商店 {id}（{store.Name}）。");
                    }
                }
                return NotFound($"商店编号 {id} 不存在。");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error: ");
                return StatusCode(500, "添加商品时发生错误，请检查日志。");
            }
        }

        [Authorize(AuthenticationSchemes = "CustomBearer")]
        [HttpDelete("{region}/{id}")]
        public IActionResult RemoveStore(string region, long id)
        {
            try
            {
                EntityModuleConfig<Store> stores = new("stores", region);
                stores.LoadConfig();
                if (stores.Count > 0)
                {
                    stores.Remove(id.ToString());
                    stores.SaveConfig();
                    return Ok($"商店编号 {id} 已删除。");
                }
                return NotFound($"商店编号 {id} 不存在。");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error: ");
                return StatusCode(500, "删除商店时发生错误，请检查日志。");
            }
        }

        [Authorize(AuthenticationSchemes = "CustomBearer")]
        [HttpPatch("{region}/{id}/{goodsId}")]
        public IActionResult PatchGoodsTime(string region, long id, long goodsId, [FromBody] DateTime? dt)
        {
            try
            {
                EntityModuleConfig<Store> stores = new("stores", region);
                stores.LoadConfig();
                Store? store = stores.Values.FirstOrDefault(s => s.Id == id);
                if (store != null)
                {
                    if (store.Goods.Values.FirstOrDefault(g => g.Id == goodsId) is Goods goods)
                    {
                        goods.ExpireTime = dt;
                        stores.Add(store.Id.ToString(), store);
                        stores.SaveConfig();

                        return Ok($"商品 {goods} 更新成功。");
                    }
                    else
                    {
                        return NotFound($"商品编号 {goodsId} 不存在。");
                    }
                }
                return NotFound($"商店编号 {id} 不存在。");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error: ");
                return StatusCode(500, "更新商品时发生错误，请检查日志。");
            }
        }

        [Authorize(AuthenticationSchemes = "CustomBearer")]
        [HttpDelete("{region}/{id}/{goodsId}")]
        public IActionResult RemoveGoods(string region, long id, long goodsId)
        {
            try
            {
                EntityModuleConfig<Store> stores = new("stores", region);
                stores.LoadConfig();
                Store? store = stores.Values.FirstOrDefault(s => s.Id == id);
                if (store != null)
                {
                    if (store.Goods.Values.FirstOrDefault(g => g.Id == goodsId) is Goods goods)
                    {
                        store.Goods.Remove(goods.Id);
                        stores.Add(store.Id.ToString(), store);
                        stores.SaveConfig();

                        return Ok($"商品 {goods} 删除成功。");
                    }
                    else
                    {
                        return NotFound($"商品编号 {goodsId} 不存在。");
                    }
                }
                return NotFound($"商店编号 {id} 不存在。");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error: ");
                return StatusCode(500, "删除商品时发生错误，请检查日志。");
            }
        }
    }
}
