using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaServers.Model
{
    public class NoticeModel : BaseEntity
    {
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public string Author { get; set; } = "";
        public DateTime StartTime { get; set; } = DateTime.MinValue;
        public DateTime EndTime { get; set; } = DateTime.MaxValue;

        public override string ToString()
        {
            return $"系统公告【{Title}】{Author} 发布于 {StartTime.ToString(General.GeneralDateTimeFormatChinese)}\r\n{Content}";
        }

        public override bool Equals(IBaseEntity? other) => other is NoticeModel && other.GetIdName() == GetIdName();
    }
}
