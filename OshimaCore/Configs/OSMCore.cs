using System.Globalization;
using System.Reflection;

namespace Oshima.Core.Configs
{
    public class OSMCore
    {
        public const string version = "v1.0";
        public const string version2 = "Beta4";

        public static string Info => $"OSM Core {version} {version2}\r\nAuthor: Milimoe\r\nBuilt on {GetBuiltTime(Assembly.GetExecutingAssembly().Location)}\r\nSee: https://github.com/milimoe";

        public static string GetBuiltTime(string dll_name)
        {
            DateTime lastWriteTime = File.GetLastWriteTime(dll_name);

            string month = lastWriteTime.ToString("MMM", CultureInfo.InvariantCulture);
            int day = lastWriteTime.Day;
            string time = lastWriteTime.ToString("HH:mm:ss", CultureInfo.InvariantCulture);

            return $"{month}. {day}, {lastWriteTime.Year} {time}";
        }
    }
}
