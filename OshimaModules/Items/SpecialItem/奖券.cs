using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Items
{
    public class 奖券() : Item(ItemType.SpecialItem)
    {
        public override long Id => (long)SpecialItemID.奖券;
        public override string Name => "奖券";
        public override string Description => "进行抽卡时，优先使用奖券替代金币。";
        public override QualityType QualityType => QualityType.Blue;
    }
}
