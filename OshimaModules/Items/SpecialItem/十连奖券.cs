using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Items
{
    public class 十连奖券() : Item(ItemType.SpecialItem)
    {
        public override long Id => (long)SpecialItemID.十连奖券;
        public override string Name => "十连奖券";
        public override string Description => "进行十连抽卡时，优先使用十连奖券替代金币。";
        public override QualityType QualityType => QualityType.Purple;
    }
}
