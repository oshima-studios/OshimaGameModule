using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.ItemEffects;
using Oshima.FunGame.OshimaModules.Skills;

namespace Oshima.FunGame.OshimaModules.Items.Consumable
{
    public class 小经验书 : Item
    {
        public override long Id => (long)ConsumableID.小经验书;
        public override string Name => "小经验书";
        public override string Description => Skills.Active?.Description ?? "";
        public override QualityType QualityType => QualityType.White;

        public 小经验书(User? user = null, int remainUseTimes = 1) : base(ItemType.Consumable)
        {
            User = user;
            Skills.Active = new 经验书技能(this, 200);
            RemainUseTimes = remainUseTimes;
        }
    }

    public class 中经验书 : Item
    {
        public override long Id => (long)ConsumableID.中经验书;
        public override string Name => "中经验书";
        public override string Description => Skills.Active?.Description ?? "";
        public override QualityType QualityType => QualityType.Green;

        public 中经验书(User? user = null, int remainUseTimes = 1) : base(ItemType.Consumable)
        {
            User = user;
            Skills.Active = new 经验书技能(this, 500);
            RemainUseTimes = remainUseTimes;
        }
    }
    
    public class 大经验书 : Item
    {
        public override long Id => (long)ConsumableID.大经验书;
        public override string Name => "大经验书";
        public override string Description => Skills.Active?.Description ?? "";
        public override QualityType QualityType => QualityType.Blue;

        public 大经验书(User? user = null, int remainUseTimes = 1) : base(ItemType.Consumable)
        {
            User = user;
            Skills.Active = new 经验书技能(this, 1000);
            RemainUseTimes = remainUseTimes;
        }
    }

    public class 经验书技能 : Skill
    {
        public override long Id => (long)ItemActiveID.经验书;
        public override string Name => "经验书";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";

        public 经验书技能(Item? item = null, double exp = 0) : base(SkillType.Item)
        {
            Level = 1;
            Item = item;
            Effects.Add(new GetEXP(this, new()
            {
                { "exp", exp }
            }));
        }
    }
}
