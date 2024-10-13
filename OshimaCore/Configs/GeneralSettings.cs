using Milimoe.FunGame.Core.Api.Utility;

namespace Oshima.Core.Configs
{
    public class GeneralSettings
    {
        public static bool IsRun { get; set; } = true;

        public static long BotQQ { get; set; } = -1;

        public static long Master { get; set; } = -1;

        public static bool IsDebug { get; set; } = false;

        public static long BlackTimes { get; set; } = 5;

        public static int BlackFrozenTime { get; set; } = 150;

        public static PluginConfig Configs { get; set; } = new("rainbot", "config");

        public static void LoadSetting()
        {
            PluginConfig configs = new("rainbot", "config");
            configs.LoadConfig();
            if (configs.TryGetValue("BotQQ", out object? value) && value != null)
            {
                BotQQ = (long)value;
            }
            if (configs.TryGetValue("Master", out value) && value != null)
            {
                Master = (long)value;
            }
            if (configs.TryGetValue("BlackTimes", out value) && value != null)
            {
                BlackTimes = (long)value;
            }
            if (configs.TryGetValue("BlackFrozenTime", out value) && value != null)
            {
                BlackFrozenTime = Convert.ToInt32((long)value);
            }
        }

        public static void SaveConfig()
        {
            Configs.Add("BotQQ", BotQQ);
            Configs.Add("Master", Master);
            Configs.Add("BlackTimes", BlackTimes);
            Configs.Add("BlackFrozenTime", BlackFrozenTime);
            Configs.SaveConfig();
        }
    }
}
