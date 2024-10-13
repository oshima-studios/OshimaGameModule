using Milimoe.FunGame.Core.Api.Utility;

namespace Oshima.Core.Configs
{
    public class SayNo
    {
        public static HashSet<string> Trigger { get; set; } = [];

        public static HashSet<string> TriggerBeforeNo { get; set; } = [];

        public static HashSet<string> IgnoreTriggerAfterNo { get; set; } = [];

        public static HashSet<string> IgnoreTriggerBeforeCan { get; set; } = [];

        public static HashSet<string> TriggerAfterYes { get; set; } = [];

        public static HashSet<string> WillNotSayNo { get; set; } = [];

        public static List<string> SayNoWords { get; set; } = [];

        public static List<string> SayDontHaveWords { get; set; } = [];

        public static List<string> SayNotYesWords { get; set; } = [];

        public static List<string> SayDontWords { get; set; } = [];

        public static List<string> SayWantWords { get; set; } = [];

        public static List<string> SayThinkWords { get; set; } = [];

        public static List<string> SaySpecialNoWords { get; set; } = [];

        public static PluginConfig Configs { get; set; } = new("rainbot", "sayno");

        public static void InitSayNo()
        {
            PluginConfig configs = new("rainbot", "sayno");
            configs.LoadConfig();
            foreach (string key in configs.Keys)
            {
                if (configs.TryGetValue(key, out object? value) && value != null)
                {
                    switch (key)
                    {
                        case "Trigger":
                            Trigger = new HashSet<string>((List<string>)value);
                            break;
                        case "TriggerBeforeNo":
                            TriggerBeforeNo = new HashSet<string>((List<string>)value);
                            break;
                        case "IgnoreTriggerAfterNo":
                            IgnoreTriggerAfterNo = new HashSet<string>((List<string>)value);
                            break;
                        case "IgnoreTriggerBeforeCan":
                            IgnoreTriggerBeforeCan = new HashSet<string>((List<string>)value);
                            break;
                        case "TriggerAfterYes":
                            TriggerAfterYes = new HashSet<string>((List<string>)value);
                            break;
                        case "WillNotSayNo":
                            WillNotSayNo = new HashSet<string>((List<string>)value);
                            break;
                        case "SayNoWords":
                            SayNoWords = (List<string>)value;
                            break;
                        case "SayDontHaveWords":
                            SayDontHaveWords = (List<string>)value;
                            break;
                        case "SayNotYesWords":
                            SayNotYesWords = (List<string>)value;
                            break;
                        case "SayDontWords":
                            SayDontWords = (List<string>)value;
                            break;
                        case "SayWantWords":
                            SayWantWords = (List<string>)value;
                            break;
                        case "SayThinkWords":
                            SayThinkWords = (List<string>)value;
                            break;
                        case "SaySpecialNoWords":
                            SaySpecialNoWords = (List<string>)value;
                            break;
                    }
                }
            }
        }

        public static void SaveConfig()
        {
            Configs.Add("Trigger", Trigger);
            Configs.Add("TriggerBeforeNo", TriggerBeforeNo);
            Configs.Add("IgnoreTriggerAfterNo", IgnoreTriggerAfterNo);
            Configs.Add("IgnoreTriggerBeforeCan", IgnoreTriggerBeforeCan);
            Configs.Add("TriggerAfterYes", TriggerAfterYes);
            Configs.Add("WillNotSayNo", WillNotSayNo);
            Configs.Add("SayNoWords", SayNoWords);
            Configs.Add("SayDontHaveWords", SayDontHaveWords);
            Configs.Add("SayNotYesWords", SayNotYesWords);
            Configs.Add("SayDontWords", SayDontWords);
            Configs.Add("SayWantWords", SayWantWords);
            Configs.Add("SayThinkWords", SayThinkWords);
            Configs.Add("SaySpecialNoWords", SaySpecialNoWords);
            Configs.SaveConfig();
        }

        public static bool AddWord(string part, bool isadd, string value)
        {
            HashSet<string> set = [];
            List<string> list = [];
            bool islist = false;
            switch (part.ToLower())
            {
                case "trigger":
                    set = Trigger;
                    break;
                case "triggerbeforeno":
                    set = TriggerBeforeNo;
                    break;
                case "ignoretriggerafterno":
                    set = IgnoreTriggerAfterNo;
                    break;
                case "ignoretriggerbeforecan":
                    set = IgnoreTriggerBeforeCan;
                    break;
                case "triggerafteryes":
                    set = TriggerAfterYes;
                    break;
                case "willnotsayno":
                    set = WillNotSayNo;
                    break;
                case "saynowords":
                    islist = true;
                    list = SayNoWords;
                    break;
                case "saydonthavewords":
                    islist = true;
                    list = SayDontHaveWords;
                    break;
                case "saynotyeswords":
                    islist = true;
                    list = SayNotYesWords;
                    break;
                case "saydontwords":
                    islist = true;
                    list = SayDontWords;
                    break;
                case "saywantwords":
                    islist = true;
                    list = SayWantWords;
                    break;
                case "saythinkwords":
                    islist = true;
                    list = SayThinkWords;
                    break;
                case "sayspecialnowords":
                    islist = true;
                    list = SaySpecialNoWords;
                    break;
                default:
                    return false;
            }
            if (isadd)
            {
                if (islist) list.Add(value);
                else set.Add(value);
            }
            else
            {
                if (islist) list.Remove(value);
                else set.Remove(value);
            }
            return true;
        }

        public static string ShowList(string part)
        {
            HashSet<string> set = [];
            List<string> list = [];
            bool islist = false;
            switch (part.ToLower())
            {
                case "trigger":
                    set = Trigger;
                    break;
                case "triggerbeforeno":
                    set = TriggerBeforeNo;
                    break;
                case "ignoretriggerafterno":
                    set = IgnoreTriggerAfterNo;
                    break;
                case "ignoretriggerbeforecan":
                    set = IgnoreTriggerBeforeCan;
                    break;
                case "triggerafteryes":
                    set = TriggerAfterYes;
                    break;
                case "willnotsayno":
                    set = WillNotSayNo;
                    break;
                case "saynowords":
                    islist = true;
                    list = SayNoWords;
                    break;
                case "saydonthavewords":
                    islist = true;
                    list = SayDontHaveWords;
                    break;
                case "saynotyeswords":
                    islist = true;
                    list = SayNotYesWords;
                    break;
                case "saydontwords":
                    islist = true;
                    list = SayDontWords;
                    break;
                case "saywantwords":
                    islist = true;
                    list = SayWantWords;
                    break;
                case "saythinkwords":
                    islist = true;
                    list = SayThinkWords;
                    break;
                case "sayspecialnowords":
                    islist = true;
                    list = SaySpecialNoWords;
                    break;
                default:
                    break;
            }
            string msg;
            if (islist)
            {
                msg = list.Count > 0 ? "列表" + part + "拥有以下成员：" + "\r\n" + string.Join("\r\n", list) : "此列表不存在或没有任何成员。";
            }
            else
            {
                msg = set.Count > 0 ? "列表" + part + "拥有以下成员：" + "\r\n" + string.Join("\r\n", set) : "此列表不存在或没有任何成员。";
            }
            return msg;
        }
    }
}