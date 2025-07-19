using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Items
{
    public class 探索许可() : Item(ItemType.SpecialItem)
    {
        public override long Id => (long)SpecialItemID.探索许可;
        public override string Name => "探索许可";
        public override string Description => "铎京探索者协会为每位探索者发放的地区准入证明，拥有此证明可以接取各种各样的探索任务，并且享受铎京探索者协会的救援和协助服务。";
        public override QualityType QualityType => QualityType.White;
    }
}
