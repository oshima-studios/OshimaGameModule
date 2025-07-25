using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Items
{
    public class 大师锻造券() : Item(ItemType.SpecialItem)
    {
        public override long Id => (long)SpecialItemID.大师锻造券;
        public override string Name => "大师锻造券";
        public override string Description => "锻造大师为你的锻造护航。当库存存在任意一张大师锻造券时，你可以在锻造时指定锻造的出货地区和品质，当锻造失败或锻造结果的出货地区和品质与你指定的不符时，将消耗一张大师锻造券并返还所有材料。\r\n" +
            "使用指令：大师锻造 <目标地区编号> <目标品质序号>\r\n地区编号请通过【世界地图】查询，品质序号为 0～5（对应：普通/优秀/稀有/史诗/传说/神话）。";
        public override QualityType QualityType => QualityType.Orange;
    }
}
