using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Models;
using Oshima.FunGame.OshimaServers.Model;
using Oshima.FunGame.OshimaServers.Service;
using Oshima.FunGame.WebAPI.Model;
using Oshima.FunGame.WebAPI.Services;

namespace Oshima.FunGame.WebAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "CustomBearer")]
    [ApiController]
    [Route("[controller]")]
    public class CSBettingController(ILogger<CSBettingController> logger) : ControllerBase
    {
        private ILogger<CSBettingController> Logger { get; set; } = logger;

        private const string noSaved = "你还没有创建存档！请发送【创建存档】创建。";
        private const string busy = "服务器繁忙，请稍后再试。";

        // ---------- 查询类（无需锁）----------
        [AllowAnonymous]
        [HttpGet("events")]
        public BotReply GetEventsOverview([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var (content, totalPages) = CSBettingService.GetEventsOverview(page, pageSize);
            KeyboardMessage kb = new();
            if (totalPages > 1) kb = new KeyboardMessage().AddPaginationRow("赛事列表 ", page, totalPages);
            return new BotReply { Markdown = new MarkdownMessage { Content = content }, Keyboard = kb };
        }

        [AllowAnonymous]
        [HttpGet("event/{eventId:int}")]
        public BotReply GetEventDetail(int eventId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var (content, totalPages) = CSBettingService.GetEventDetail(eventId, page, pageSize);
            KeyboardMessage kb = new();
            if (totalPages > 1) kb = new KeyboardMessage().AddPaginationRow($"赛事详情 {eventId} ", page, totalPages);
            return new BotReply { Markdown = new MarkdownMessage { Content = content }, Keyboard = kb };
        }

        /// <summary>
        /// 获取所有比赛赛程
        /// </summary>
        [AllowAnonymous]
        [HttpGet("matches")]
        public BotReply GetAllMatches([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var (content, totalPages) = CSBettingService.GetAllMatches(page, pageSize);
            KeyboardMessage kb = new();
            if (totalPages > 1) kb = new KeyboardMessage().AddPaginationRow("赛程 ", page, totalPages);
            return new BotReply { Markdown = new MarkdownMessage { Content = content }, Keyboard = kb };
        }

        [AllowAnonymous]
        [HttpGet("match/{matchId:int}")]
        public BotReply GetMatchDetail(int matchId)
        {
            return new BotReply { Markdown = new MarkdownMessage { Content = CSBettingService.GetMatchDetail(matchId, out int status) + (status == 0 ? $"预测指令：{"预测".CreateCmdInput()} <比赛ID> <选项> <{General.GameplayEquilibriumConstant.InGameCurrency}数>\r\n👇🏻 点击下方按钮快速预测" : "")} };
        }

        [HttpGet("mybets/{uid:long}")]
        public BotReply GetMyBets(long uid, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var (content, totalPages) = CSBettingService.GetMyBets(uid, -1, page, pageSize);
            KeyboardMessage kb = new();
            if (totalPages > 1) kb = new KeyboardMessage().AddPaginationRow("我的预测 ", page, totalPages);
            return new BotReply { Markdown = new MarkdownMessage { Content = content }, Keyboard = kb };
        }

        [HttpGet("mybets/{uid:long}/{mid:long}")]
        public BotReply GetMyBets(long uid, long mid)
        {
            var (content, _) = CSBettingService.GetMyBets(uid, mid, 1, int.MaxValue);
            return new BotReply { Markdown = new MarkdownMessage { Content = content } };
        }

        // ---------- 需要用户锁的操作 ----------

        /// <summary>
        /// 投注指令（内部 + API）
        /// </summary>
        [HttpPost("bet")]
        public BotReply PlaceBet([FromQuery] long uid, [FromQuery] int matchId, [FromQuery] string option, [FromQuery] long amount = 1000)
        {
            MarkdownMessage md = new() { Content = busy };
            BotReply reply = new() { Markdown = md };
            try
            {
                PluginConfig pc = FunGameService.GetUserConfig(uid, out bool isTimeout);
                if (isTimeout) return reply;
                if (pc.Count == 0)
                {
                    md.Content = noSaved;
                    return reply;
                }

                User user = FunGameService.GetUser(pc);

                // 校验金额
                if (amount < 1000)
                {
                    md.Content = $"最低助力 1000 {General.GameplayEquilibriumConstant.InGameCurrency}。";
                    FunGameService.SetUserConfigButNotRelease(uid, pc, user);
                    return reply;
                }
                if (user.Inventory.Credits < amount)
                {
                    md.Content = $"{General.GameplayEquilibriumConstant.InGameCurrency}不足，你的{General.GameplayEquilibriumConstant.InGameCurrency}：{user.Inventory.Credits}。";
                    FunGameService.SetUserConfigButNotRelease(uid, pc, user);
                    return reply;
                }

                if (CSBettingService.PlaceBet(uid, matchId, option, amount, out string error))
                {
                    user.Inventory.Credits -= (int)amount;
                    FunGameService.SetUserConfigButNotRelease(uid, pc, user);
                    md.Content = $"{"助力".CreateCmdInput($"比赛详情 {matchId}")}成功！{amount} {General.GameplayEquilibriumConstant.InGameCurrency}已扣除。";
                }
                else
                {
                    FunGameService.SetUserConfigButNotRelease(uid, pc, user);
                    md.Content = error;
                }
                return reply;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "CSBetting PlaceBet 异常");
                return reply;
            }
            finally
            {
                FunGameService.ReleaseUserSemaphoreSlim(uid);
            }
        }

        /// <summary>
        /// 领奖指令（内部 + API）
        /// </summary>
        [HttpPost("claim")]
        public BotReply ClaimRewards([FromQuery] long uid)
        {
            MarkdownMessage md = new() { Content = busy };
            BotReply reply = new() { Markdown = md };
            try
            {
                PluginConfig pc = FunGameService.GetUserConfig(uid, out bool isTimeout);
                if (isTimeout) return reply;
                if (pc.Count == 0)
                {
                    md.Content = noSaved;
                    return reply;
                }

                User user = FunGameService.GetUser(pc);
                long total = CSBettingService.ClaimRewards(uid);
                if (total > 0)
                {
                    user.Inventory.Credits += (int)total;
                    FunGameService.SetUserConfigButNotRelease(uid, pc, user);
                    md.Content = $"领取成功！获得 {total} {General.GameplayEquilibriumConstant.InGameCurrency}。";
                }
                else
                {
                    FunGameService.SetUserConfigButNotRelease(uid, pc, user);
                    md.Content = "没有可领取的奖励。";
                }
                return reply;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "CSBetting ClaimRewards 异常");
                return reply;
            }
            finally
            {
                FunGameService.ReleaseUserSemaphoreSlim(uid);
            }
        }

        // ---------- 管理员操作 ----------

        /// <summary>
        /// 结算比赛（管理员）
        /// </summary>
        [HttpPost("settle")]
        public BotReply SettleMatch([FromQuery] long uid, [FromQuery] int matchId, [FromQuery] string winner, [FromQuery] string result = "")
        {
            // 这里可以添加管理员权限检查，假设调用者已经过授权
            MarkdownMessage md = new() { Content = busy };
            BotReply reply = new() { Markdown = md };
            try
            {
                // 简单的管理员判断（可接入实际权限系统）
                if (!FunGameConstant.UserIdAndUsername.TryGetValue(uid, out User? admin) || (!admin.IsAdmin && !admin.IsOperator))
                {
                    md.Content = "你没有权限执行此操作。";
                    return reply;
                }

                md.Content = CSBettingService.SettleMatch(matchId, winner, result);
                return reply;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "CSBetting SettleMatch 异常");
                return reply;
            }
        }

        /// <summary>
        /// 创建赛事（管理员）
        /// </summary>
        [HttpPost("create-event")]
        public BotReply CreateEvent([FromBody] CreateEventRequest request)
        {
            MarkdownMessage md = new() { Content = busy };
            BotReply reply = new() { Markdown = md };
            try
            {
                if (!FunGameConstant.UserIdAndUsername.TryGetValue(request.Uid, out User? user) || (!user.IsAdmin && !user.IsOperator))
                {
                    md.Content = "你没有权限执行此操作。";
                    return reply;
                }

                if (CSBettingService.CreateEvent(request.Name, request.StartTime, request.EndTime, out string error, out long? newId))
                {
                    md.Content = $"赛事创建成功！新赛事ID：{newId}";
                }
                else
                {
                    md.Content = error;
                }
                return reply;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "CreateEvent 异常");
                return reply;
            }
        }

        /// <summary>
        /// 创建比赛（管理员）
        /// </summary>
        [HttpPost("create-match")]
        public BotReply CreateMatch([FromBody] CreateMatchRequest request)
        {
            MarkdownMessage md = new() { Content = busy };
            BotReply reply = new() { Markdown = md };
            try
            {
                if (!FunGameConstant.UserIdAndUsername.TryGetValue(request.Uid, out User? user) || (!user.IsAdmin && !user.IsOperator))
                {
                    md.Content = "你没有权限执行此操作。";
                    return reply;
                }

                if (CSBettingService.CreateMatch(request.EventId, request.Team1Name, request.Team2Name, request.Stage,
                    request.StartTime, request.BetDeadline, request.AvailableOptions, request.Team1WinOdds, request.Team2WinOdds, request.Team1WinProbability, out string error, out long? newId))
                {
                    md.Content = $"比赛创建成功！新比赛ID：{newId}";
                }
                else
                {
                    md.Content = error;
                }
                return reply;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "CreateMatch 异常");
                return reply;
            }
        }

        [HttpPost("close-betting")]
        public BotReply CloseBetting([FromQuery] long uid, [FromQuery] int matchId)
        {
            MarkdownMessage md = new() { Content = busy };
            BotReply reply = new() { Markdown = md };
            try
            {
                if (!FunGameConstant.UserIdAndUsername.TryGetValue(uid, out User? admin) || (!admin.IsAdmin && !admin.IsOperator))
                {
                    md.Content = "你没有权限执行此操作。";
                    return reply;
                }

                if (CSBettingService.CloseBetting(matchId, out string msg))
                {
                    md.Content = msg;
                }

                return reply;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "CloseBetting 异常");
                return reply;
            }
        }

        [HttpPost("update-match")]
        public BotReply UpdateMatch([FromBody] UpdateMatchRequest request)
        {
            MarkdownMessage md = new() { Content = busy };
            BotReply reply = new() { Markdown = md };
            try
            {
                if (!FunGameConstant.UserIdAndUsername.TryGetValue(request.Uid, out User? user) || (!user.IsAdmin && !user.IsOperator))
                {
                    md.Content = "你没有权限执行此操作。";
                    return reply;
                }

                if (CSBettingService.UpdateMatch(request, out string error))
                {
                    md.Content = $"比赛 {request.MatchId} 属性修改成功。";
                }
                else
                {
                    md.Content = error;
                }
                return reply;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "UpdateMatch 异常");
                return reply;
            }
        }
    }
}
