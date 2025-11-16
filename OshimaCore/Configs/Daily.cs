using Milimoe.FunGame.Core.Api.Utility;
using Oshima.Core.Constant;

namespace Oshima.Core.Configs
{
    public class Daily
    {
        public static bool DailyNews { get; set; } = false;

        public static bool ClearDailys { get; set; } = true;

        public static Dictionary<long, string> UserDailys { get; } = [];

        public static Dictionary<string, string> OpenUserDailys { get; } = [];

        public static List<string> GreatFortune { get; set; } = [];

        public static List<string> ModerateFortune { get; set; } = [];

        public static List<string> GoodFortune { get; set; } = [];

        public static List<string> MinorFortune { get; set; } = [];

        public static List<string> Misfortune { get; set; } = [];

        public static List<string> GreatMisfortune { get; set; } = [];

        public static List<DailyType> DailyTypes { get; set; } = [];

        public static PluginConfig DailyContent { get; set; } = new("rainbot", "daily");

        public static PluginConfig Configs { get; set; } = new("rainbot", "userdaliys");

        public static PluginConfig OpenConfigs { get; set; } = new("rainbot", "openuserdaliys");

        public static void InitDaily()
        {
            DailyContent.LoadConfig();
            if (DailyContent.TryGetValue("GreatFortune", out object? value) && value != null)
            {
                GreatFortune = (List<string>)value;
            }
            if (DailyContent.TryGetValue("ModerateFortune", out value) && value != null)
            {
                ModerateFortune = (List<string>)value;
            }
            if (DailyContent.TryGetValue("GoodFortune", out value) && value != null)
            {
                GoodFortune = (List<string>)value;
            }
            if (DailyContent.TryGetValue("SmallFortune", out value) && value != null)
            {
                MinorFortune = (List<string>)value;
            }
            if (DailyContent.TryGetValue("Misfortune", out value) && value != null)
            {
                Misfortune = (List<string>)value;
            }
            if (DailyContent.TryGetValue("GreatMisfortune", out value) && value != null)
            {
                GreatMisfortune = (List<string>)value;
            }

            DailyTypes.Clear();
            if (GreatFortune.Count != 0)
            {
                DailyTypes.Add(DailyType.GreatFortune);
            }
            if (ModerateFortune.Count != 0)
            {
                DailyTypes.Add(DailyType.ModerateFortune);
            }
            if (GoodFortune.Count != 0)
            {
                DailyTypes.Add(DailyType.GoodFortune);
            }
            if (MinorFortune.Count != 0)
            {
                DailyTypes.Add(DailyType.MinorFortune);
            }
            if (Misfortune.Count != 0)
            {
                DailyTypes.Add(DailyType.Misfortune);
            }
            if (GreatMisfortune.Count != 0)
            {
                DailyTypes.Add(DailyType.GreatMisfortune);
            }

            Configs.LoadConfig();
            foreach (string str in Configs.Keys)
            {
                if (long.TryParse(str, out long qq) && Configs.TryGetValue(str, out object? value2) && value2 != null && !UserDailys.ContainsKey(qq))
                {
                    UserDailys.Add(qq, value2.ToString() ?? "");
                    if (UserDailys[qq] == "") UserDailys.Remove(qq);
                }
            }
            SaveDaily();
            OpenConfigs.LoadConfig();
            foreach (string str in OpenConfigs.Keys)
            {
                if (OpenConfigs.TryGetValue(str, out object? value2) && value2 != null && !OpenUserDailys.ContainsKey(str))
                {
                    OpenUserDailys.Add(str, value2.ToString() ?? "");
                    if (OpenUserDailys[str] == "") OpenUserDailys.Remove(str);
                }
            }
            SaveOpenDaily();
        }

        public static void SaveDaily()
        {
            lock (Configs)
            {
                Configs.Clear();
                foreach (long qq in UserDailys.Keys)
                {
                    Configs.Add(qq.ToString(), UserDailys[qq]);
                }
                Configs.SaveConfig();
            }
        }

        public static void SaveOpenDaily()
        {
            lock (OpenConfigs)
            {
                OpenConfigs.Clear();
                foreach (string openid in OpenUserDailys.Keys)
                {
                    OpenConfigs.Add(openid.ToString(), OpenUserDailys[openid]);
                }
                OpenConfigs.SaveConfig();
            }
        }

        public static void ClearDaily()
        {
            UserDailys.Clear();
            OpenUserDailys.Clear();
            SaveDaily();
            SaveOpenDaily();
        }
    }
}
