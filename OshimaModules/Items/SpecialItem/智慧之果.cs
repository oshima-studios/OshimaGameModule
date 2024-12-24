using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Items
{
    public class 智慧之果() : Item(ItemType.SpecialItem)
    {
        public override long Id => (long)SpecialItemID.智慧之果;
        public override string Name => "智慧之果";
        public override string Description => "升级技能必备的中级材料。";
        public override QualityType QualityType => QualityType.Green;
    }
}
