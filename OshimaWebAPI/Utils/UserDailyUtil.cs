using Milimoe.FunGame.Core.Api.Utility;
using Oshima.Core.Configs;
using Oshima.Core.Constant;
using Oshima.FunGame.WebAPI.Models;

namespace Oshima.FunGame.WebAPI.Utils
{
    public class UserDailyUtil
    {
        public static UserDaily GetUserDaily(long user_id)
        {
            if (Daily.UserDailys.TryGetValue(user_id, out string? value) && value != null && value.Trim() != "")
            {
                string daily = "你已看过你的今日运势：\r\n" + value;
                return new UserDaily(user_id, 0, daily);
            }
            else
            {
                if (Daily.GreatFortune.Count == 0 && Daily.ModerateFortune.Count == 0 && Daily.GoodFortune.Count == 0 &&
                    Daily.MinorFortune.Count == 0 && Daily.Misfortune.Count == 0 && Daily.GreatMisfortune.Count == 0)
                {
                    return new UserDaily(0, 0, "今日运势列表为空，请联系管理员设定。");
                }

                // 抽个运势
                DailyType type = Daily.DailyTypes[Random.Shared.Next(Daily.DailyTypes.Count)];
                string text = type switch
                {
                    DailyType.GreatFortune => Daily.GreatFortune[Random.Shared.Next(Daily.GreatFortune.Count)],
                    DailyType.ModerateFortune => Daily.ModerateFortune[Random.Shared.Next(Daily.ModerateFortune.Count)],
                    DailyType.GoodFortune => Daily.GoodFortune[Random.Shared.Next(Daily.GoodFortune.Count)],
                    DailyType.MinorFortune => Daily.MinorFortune[Random.Shared.Next(Daily.MinorFortune.Count)],
                    DailyType.Misfortune => Daily.Misfortune[Random.Shared.Next(Daily.Misfortune.Count)],
                    DailyType.GreatMisfortune => Daily.GreatMisfortune[Random.Shared.Next(Daily.GreatMisfortune.Count)],
                    _ => "",
                };
                if (text != "")
                {
                    Daily.UserDailys.Add(user_id, text);
                    string daily = "你的今日运势是：\r\n" + text;
                    Daily.SaveDaily();
                    return new UserDaily(user_id, (int)type, daily);
                }
                return new UserDaily(0, 0, "今日运势列表为空，请联系管理员设定。");
            }
        }

        public static UserDaily ViewUserDaily(long user_id)
        {
            if (Daily.UserDailys.TryGetValue(user_id, out string? value) && value != null && value.Trim() != "")
            {
                return new UserDaily(user_id, 0, "TA今天的运势是：\r\n" + value);
            }
            else
            {
                return new UserDaily(0, 0, "TA还没有抽取今日运势哦，快去提醒TA发送【我的运势】抽取运势吧！");
            }
        }

        public static string RemoveDaily(long user_id)
        {
            Daily.UserDailys.Remove(user_id);
            Daily.SaveDaily();
            return NetworkUtility.JsonSerialize("你的今日运势已重置。");
        }
    }
}
