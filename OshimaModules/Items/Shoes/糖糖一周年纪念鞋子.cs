using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;
using Oshima.FunGame.OshimaModules.Skills;

namespace Oshima.FunGame.OshimaModules.Items
{
    public class 糖糖一周年纪念鞋子 : Item
    {
        public override long Id => (long)ShoesID.糖糖一周年纪念鞋子;
        public override string Name => "蜜步流心";
        public override string Description => $"{Skills.Passives.FirstOrDefault()?.Description}{(Skills.Active != null ? $"{Skills.Active.Name}：{Skills.Active.Description}" : "")}";
        public override string BackgroundStory => "一双棕色及踝短靴，靴面有蜂窝状暗纹，靴底内嵌彩色糖豆，每走一步糖豆轻轻碰撞，发出微不可闻的清脆声响。";
        public override string Category => "糖糖一周年限定纪念物品";
        public override QualityType QualityType => QualityType.Gold;

        public 糖糖一周年纪念鞋子(Character? character = null) : base(ItemType.Shoes)
        {
            Price = 0;
            IsSellable = false;
            IsTradable = false;
            IsLock = true;
            Skills.Active = new 疾风步(character)
            {
                Level = 5
            };
            Skills.Passives.Add(new 糖糖一周年纪念鞋子技能(character, this));
        }
    }

    public class 糖糖一周年纪念鞋子技能 : Skill
    {
        public override long Id => (long)ItemPassiveID.糖糖一周年纪念鞋子;
        public override string Name => "蜜步流心";
        public override string Description => string.Join("", Effects.Select(e => e.Description));

        private readonly double 行动速度加成 = 180;
        private readonly double 加速系数加成 = 0.2;

        public 糖糖一周年纪念鞋子技能(Character? character = null, Item? item = null) : base(SkillType.Passive, character)
        {
            Level = 1;
            Item = item;
            Dictionary<string, object> values = new()
            {
                { "exspd", 行动速度加成 },
                { "exacc", 加速系数加成 }
            };
            Effects.Add(new ExSPD(this, values, character));
            Effects.Add(new AccelerationCoefficient(this, values, character));
        }

        public override IEnumerable<Effect> AddPassiveEffectToCharacter()
        {
            return Effects;
        }
    }
}
