using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Items
{
    public class 法则精粹() : Item(ItemType.SpecialItem)
    {
        public override long Id => (long)SpecialItemID.法则精粹;
        public override string Name => "法则精粹";
        public override string Description => "升级技能必备的终级材料。";
        public override QualityType QualityType => QualityType.Orange;
    }
}
