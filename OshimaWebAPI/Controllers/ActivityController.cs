using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Items;
using Oshima.FunGame.OshimaModules.Models;
using Oshima.FunGame.OshimaServers.Service;

namespace Oshima.FunGame.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ActivityController(ILogger<ActivityController> logger) : ControllerBase
    {
        private readonly ILogger<ActivityController> _logger = logger;

        [HttpGet]
        public IActionResult GetActivity()
        {
            try
            {
                EntityModuleConfig<Activity> activities = new("activities", "activities");
                activities.LoadConfig();
                if (activities.Count == 0)
                {
                    return Ok("当前没有任何活动，敬请期待。");
                }
                return Ok($"★☆★ 当前活动列表 ★☆★\r\n{string.Join("\r\n", activities.Values.Select(a => a.GetIdName()))}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error: ");
                return StatusCode(500, "获取活动信息时发生错误，请检查日志。");
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetActivity(long id)
        {
            try
            {
                EntityModuleConfig<Activity> activities = new("activities", "activities");
                activities.LoadConfig();
                if (activities.Get(id.ToString()) is Activity activity)
                {
                    return Ok($"{activity}");
                }
                return NotFound($"活动编号 {id} 不存在。");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error: ");
                return StatusCode(500, "获取活动信息时发生错误，请检查日志。");
            }
        }

        //[HttpPut("{id}")]
        //public IActionResult UpdateActivity(long id, [FromBody] Activity? activity = null)
        //{
        //    try
        //    {
        //        if (activity is null || activity.Id != id)
        //        {
        //            return BadRequest("活动更新失败，活动 ID 与请求体 ID 不匹配或请求体格式错误。");
        //        }

        //        EntityModuleConfig<Activity> activities = new("activities", "activities");
        //        activities.LoadConfig();
        //        activities.Add(activity.Id.ToString(), activity);
        //        activities.SaveConfig();

        //        return Ok($"活动 {activity.GetIdName()} 更新成功。");
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError(e, "Error: ");
        //        return StatusCode(500, "更新活动时发生错误，请检查日志。");
        //    }
        //}

        [Authorize(AuthenticationSchemes = "CustomBearer")]
        [HttpDelete("{id}")]
        public IActionResult RemoveActivity(long id)
        {
            try
            {
                EntityModuleConfig<Activity> activities = new("activities", "activities");
                activities.LoadConfig();
                if (activities.Count > 0)
                {
                    activities.Remove(id.ToString());
                    activities.SaveConfig();
                    return Ok($"活动编号 {id} 已删除。");
                }
                return NotFound($"活动编号 {id} 不存在。");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error: ");
                return StatusCode(500, "删除活动时发生错误，请检查日志。");
            }
        }

        [HttpGet("{id}/{questId}")]
        public IActionResult GetQuest(long id, long questId)
        {
            try
            {
                EntityModuleConfig<Activity> activities = new("activities", "activities");
                activities.LoadConfig();
                Activity? activity = activities.Values.FirstOrDefault(a => a.Id == id);
                if (activity != null)
                {
                    if (activity.Quests.FirstOrDefault(q => q.Id == questId) is Quest quest)
                    {
                        return Ok($"该任务属于活动【{activity.Name}】，详情：\r\n{quest}");
                    }
                    else
                    {
                        return NotFound($"任务编号 {questId} 不存在。");
                    }
                }
                return NotFound($"活动编号 {id} 不存在。");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error: ");
                return StatusCode(500, "获取任务信息时发生错误，请检查日志。");
            }
        }

        //[HttpPut("{id}/{questId}")]
        //public IActionResult UpdateQuest(long id, long questId, [FromBody] Quest? quest = null)
        //{
        //    try
        //    {
        //        if (quest is null || quest.Id != questId)
        //        {
        //            return BadRequest("任务更新失败，任务 ID 与请求体 ID 不匹配或请求体格式错误。");
        //        }

        //        EntityModuleConfig<Activity> activities = new("activities", "activities");
        //        activities.LoadConfig();
        //        Activity? activity = activities.Values.FirstOrDefault(a => a.Id == id);
        //        if (activity != null)
        //        {
        //            if (activity.Quests.FirstOrDefault(q => q.Id == quest.Id) is Quest current)
        //            {
        //                activity.Quests.Remove(quest);
        //            }
        //            activity.Quests.Add(quest);
        //            activities.Add(activity.Id.ToString(), activity);
        //            activities.SaveConfig();
        //            return Ok($"任务 {quest.GetIdName()}（属于活动【{activity.Name}】）更新成功。");
        //        }

        //        return NotFound($"活动编号 {id} 不存在。");
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError(e, "Error: ");
        //        return StatusCode(500, "更新任务时发生错误，请检查日志。");
        //    }
        //}

        [Authorize(AuthenticationSchemes = "CustomBearer")]
        [HttpDelete("{id}/{questId}")]
        public IActionResult RemoveQuest(long id, long questId)
        {
            try
            {
                EntityModuleConfig<Activity> activities = new("activities", "activities");
                activities.LoadConfig();
                Activity? activity = activities.Values.FirstOrDefault(a => a.Id == id);
                if (activity != null)
                {
                    if (activity.Quests.FirstOrDefault(q => q.Id == questId) is Quest quest)
                    {
                        activity.Quests.Remove(quest);
                        activities.Add(activity.Id.ToString(), activity);
                        activities.SaveConfig();

                        return Ok($"任务 {quest} 删除成功。");
                    }
                    else
                    {
                        return NotFound($"任务编号 {questId} 不存在。");
                    }
                }
                return NotFound($"活动编号 {id} 不存在。");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error: ");
                return StatusCode(500, "删除任务时发生错误，请检查日志。");
            }
        }

        [Authorize(AuthenticationSchemes = "CustomBearer")]
        [HttpPut("{id}/user_{uid}")]
        public IActionResult AddToUser(long id, long uid)
        {
            try
            {
                if (uid <= 0)
                {
                    return BadRequest("无效的用户编号。");
                }
                EntityModuleConfig<Activity> activities = new("activities", "activities");
                activities.LoadConfig();
                if (activities.Get(id.ToString()) is Activity activity)
                {
                    EntityModuleConfig<Activity> userActivities = new("activities", uid.ToString());
                    userActivities.LoadConfig();
                    FunGameService.AddEventActivity(activity, userActivities);
                    userActivities.SaveConfig();
                    return Ok($"{activity}");
                }
                return NotFound($"活动编号 {id} 不存在。");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error: ");
                return StatusCode(500, "获取活动信息时发生错误，请检查日志。");
            }
        }

        [Authorize(AuthenticationSchemes = "CustomBearer")]
        [HttpPost("{id}/addusers")]
        public IActionResult AddToUsers(long id)
        {
            try
            {
                EntityModuleConfig<Activity> activities = new("activities", "activities");
                activities.LoadConfig();
                foreach (long uid in FunGameConstant.UserIdAndUsername.Keys)
                {
                    if (activities.Get(id.ToString()) is Activity activity)
                    {
                        EntityModuleConfig<Activity> userActivities = new("activities", uid.ToString());
                        userActivities.LoadConfig();
                        FunGameService.AddEventActivity(activity, userActivities);
                        userActivities.SaveConfig();
                    }
                }
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error: ");
                return StatusCode(500, "获取活动信息时发生错误，请检查日志。");
            }
        }
    }
}
