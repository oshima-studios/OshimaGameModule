using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;
using Oshima.FunGame.OshimaModules.Skills;

namespace Oshima.FunGame.OshimaModules.Items
{
    public class 糖糖一周年纪念饰品1 : Item
    {
        public override long Id => (long)AccessoryID.糖糖一周年纪念饰品1;
        public override string Name => "回忆糖纸";
        public override string Description => Skills.Passives.Count > 0 ? Skills.Passives.First().Description : "";
        public override string BackgroundStory => "在魔法视觉下，可见其由无数层半透明糖纸压缩而成，每层糖纸都印着微缩画面。";
        public override string Category => "糖糖一周年限定纪念物品";
        public override QualityType QualityType => QualityType.Gold;

        public 糖糖一周年纪念饰品1(Character? character = null) : base(ItemType.Accessory)
        {
            Price = 0;
            IsSellable = false;
            IsTradable = false;
            IsLock = true;
            Skills.Passives.Add(new 糖糖一周年纪念饰品技能1(character, this));
        }
    }

    public class 糖糖一周年纪念饰品技能1 : Skill
    {
        public override long Id => (long)ItemPassiveID.糖糖一周年纪念饰品1;
        public override string Name => "回忆糖纸";
        public override string Description => string.Join("", Effects.Select(e => e.Description));

        public 糖糖一周年纪念饰品技能1(Character? character = null, Item? item = null) : base(SkillType.Passive, character)
        {
            Level = 1;
            Item = item;
            Dictionary<string, object> values = new()
            {
                { "exls", 0.25 },
                { "shtr", 0.25 }
            };
            Effects.Add(new ExLifesteal(this, values, character));
            Effects.Add(new SkillHardTimeReduce2(this, values, character));
        }

        public override IEnumerable<Effect> AddPassiveEffectToCharacter()
        {
            return Effects;
        }
    }

    public class 糖糖一周年纪念饰品2 : Item
    {
        public override long Id => (long)AccessoryID.糖糖一周年纪念饰品2;
        public override string Name => "蜂糖蜜酿";
        public override string Description => Skills.Passives.Count > 0 ? Skills.Passives.First().Description : "";
        public override string BackgroundStory => "一对以秘银细链连接，随着光线折射散发极微弱糖霜光泽的耳坠。左耳为蜂糖，右耳为蜜酿。";
        public override string Category => "糖糖一周年限定纪念物品";
        public override QualityType QualityType => QualityType.Gold;

        public 糖糖一周年纪念饰品2(Character? character = null) : base(ItemType.Accessory)
        {
            Price = 0;
            IsSellable = false;
            IsTradable = false;
            IsLock = true;
            Skills.Passives.Add(new 糖糖一周年纪念饰品技能2(character, this));
        }
    }

    public class 糖糖一周年纪念饰品技能2 : Skill
    {
        public override long Id => (long)ItemPassiveID.糖糖一周年纪念饰品2;
        public override string Name => "蜂糖蜜酿";
        public override string Description => string.Join("", Effects.Select(e => e.Description));

        public 糖糖一周年纪念饰品技能2(Character? character = null, Item? item = null) : base(SkillType.Passive, character)
        {
            Level = 1;
            Item = item;
            Dictionary<string, object> values = new()
            {
                { "exhp", 1550 },
                { "exmp", 800 }
            };
            Effects.Add(new ExMaxHP(this, values, character));
            Effects.Add(new ExMaxMP(this, values, character));
        }

        public override IEnumerable<Effect> AddPassiveEffectToCharacter()
        {
            return Effects;
        }
    }
}
