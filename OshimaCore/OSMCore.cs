using System.Globalization;
using System.Reflection;
using Oshima.Core.Configs;

namespace Oshima.Core
{
    public class OSMCore
    {
        public const string version = "v2.0";
        public const string version2 = "Release";

        public static string Info => $"OSM Core {version} {version2}\r\nAuthor: Milimoe\r\nBuilt at {GetBuiltTime(Assembly.GetExecutingAssembly().Location)}\r\nSee: https://github.com/milimoe";

        public static string GetBuiltTime(string dll_name)
        {
            DateTime lastWriteTime = File.GetLastWriteTime(dll_name);

            string month = lastWriteTime.ToString("MMM", CultureInfo.InvariantCulture);
            int day = lastWriteTime.Day;
            string time = lastWriteTime.ToString("HH:mm:ss", CultureInfo.InvariantCulture);

            return $"{month}. {day}, {lastWriteTime.Year} {time}";
        }

        public static void InitOSMCore()
        {
            GeneralSettings.LoadSetting();
            GeneralSettings.SaveConfig();
            QQOpenID.LoadConfig();
            QQOpenID.SaveConfig();
            Daily.InitDaily();
            SayNo.InitSayNo();
            Ignore.InitIgnore();
        }
    }
}
