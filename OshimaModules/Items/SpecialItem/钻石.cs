using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Items
{
    public class 钻石() : Item(ItemType.SpecialItem)
    {
        public override long Id => (long)SpecialItemID.钻石;
        public override string Name => "钻石";
        public override string Description => "钻石是筽祀牻大陆通用的稀有矿物，是第二流通货币。钻石可以以稳定的 1:200 兑换率兑换大陆第一流通货币金币。";
        public override QualityType QualityType => QualityType.White;
    }
}
