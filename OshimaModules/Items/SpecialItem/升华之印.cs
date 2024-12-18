using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Items
{
    public class 升华之印() : Item(ItemType.SpecialItem)
    {
        public override long Id => (long)SpecialItemID.升华之印;
        public override string Name => "升华之印";
        public override string Description => Skills.Passives.Count > 0 ? Skills.Passives.First().Description : "";
    }
}
