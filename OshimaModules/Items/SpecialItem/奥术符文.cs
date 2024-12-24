using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Items
{
    public class 奥术符文() : Item(ItemType.SpecialItem)
    {
        public override long Id => (long)SpecialItemID.奥术符文;
        public override string Name => "奥术符文";
        public override string Description => "升级技能必备的高级材料。";
        public override QualityType QualityType => QualityType.Blue;
    }
}
