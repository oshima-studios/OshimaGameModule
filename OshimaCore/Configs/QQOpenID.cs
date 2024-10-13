using Milimoe.FunGame.Core.Api.Utility;

namespace Oshima.Core.Configs
{
    public class QQOpenID
    {
        public static Dictionary<string, long> QQAndOpenID { get; set; } = [];

        public static PluginConfig Configs { get; set; } = new("rainbot", "qqopenid");

        public static void LoadConfig()
        {
            Configs.LoadConfig();
            foreach (string str in Configs.Keys)
            {
                if (Configs.TryGetValue(str, out object? value) && value is long qq && qq != 0)
                {
                    QQAndOpenID.TryAdd(str, qq);
                }
            }
        }

        public static void SaveConfig()
        {
            lock (Configs)
            {
                Configs.Clear();
                foreach (string openid in QQAndOpenID.Keys)
                {
                    Configs.Add(openid, QQAndOpenID[openid]);
                }
                Configs.SaveConfig();
            }
        }
    }
}
