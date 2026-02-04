using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;
using Oshima.FunGame.OshimaModules.Skills;

namespace Oshima.FunGame.OshimaModules.Items
{
    public class 糖糖一周年纪念防具 : Item
    {
        public override long Id => (long)ArmorID.糖糖一周年纪念防具;
        public override string Name => "糖之誓约";
        public override string Description => $"{Skills.Passives.FirstOrDefault()?.Description}{(Skills.Active != null ? $"{Skills.Active.Name}：{Skills.Active.Description}" : "")}";
        public override string BackgroundStory => "及膝的米白色复古风衣，在光线下会泛起珍珠母贝般柔和的七彩光泽，如同撒上了一层极细的糖霜。";
        public override string Category => "糖糖一周年限定纪念物品";
        public override QualityType QualityType => QualityType.Gold;

        public 糖糖一周年纪念防具(Character? character = null) : base(ItemType.Armor)
        {
            Price = 0;
            IsSellable = false;
            IsTradable = false;
            IsLock = true;
            Skills.Active = new 神圣祝福复(character)
            {
                Level = 6
            };
            Skills.Passives.Add(new 糖糖一周年纪念防具技能(character, this));
        }
    }

    public class 糖糖一周年纪念防具技能 : Skill
    {
        public override long Id => (long)ItemPassiveID.糖糖一周年纪念防具;
        public override string Name => "糖之誓约";
        public override string Description => string.Join("", Effects.Select(e => e.Description));

        private readonly double 物理护甲加成 = 180;

        public 糖糖一周年纪念防具技能(Character? character = null, Item? item = null) : base(SkillType.Passive, character)
        {
            Level = 1;
            Item = item;
            Dictionary<string, object> values = new()
            {
                { "exdef", 物理护甲加成 }
            };
            Effects.Add(new ExDEF(this, values, character));
        }

        public override IEnumerable<Effect> AddPassiveEffectToCharacter()
        {
            return Effects;
        }
    }
}
