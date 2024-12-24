using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Items
{
    public class 技能卷轴() : Item(ItemType.SpecialItem)
    {
        public override long Id => (long)SpecialItemID.技能卷轴;
        public override string Name => "技能卷轴";
        public override string Description => "升级技能必备的初级材料。";
        public override QualityType QualityType => QualityType.White;
    }
}
