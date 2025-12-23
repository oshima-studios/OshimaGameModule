using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;
using Oshima.FunGame.OshimaModules.Skills;

namespace Oshima.FunGame.OshimaModules.Items
{
    public class 糖糖一周年纪念武器 : Item
    {
        public override long Id => (long)WeaponID.糖糖一周年纪念武器;
        public override string Name => "粉糖雾蝶";
        public override string Description => $"{Skills.Passives.First().Description}{Skills.Passives.Last().Name}：{Skills.Passives.Last().Description}";
        public override string BackgroundStory => "可长达1.5米的权杖，通体呈半透明的琥珀金色，内部可见如星河般缓缓流动的魔法糖浆。";
        public override string Category => "糖糖一周年限定纪念物品";
        public override QualityType QualityType => QualityType.Gold;

        public 糖糖一周年纪念武器(Character? character = null) : base(ItemType.Weapon)
        {
            Price = 0;
            IsSellable = false;
            IsTradable = false;
            IsLock = true;
            WeaponType = WeaponType.Staff;
            Skills.Passives.Add(new 糖糖一周年纪念武器技能(character, this));
            Skills.Passives.Add(new 致命节奏(character)
            {
                Level = 1
            });
        }
    }

    public class 糖糖一周年纪念武器技能 : Skill
    {
        public override long Id => (long)ItemPassiveID.糖糖一周年纪念武器;
        public override string Name => "粉糖雾蝶";
        public override string Description => string.Join("", Effects.Select(e => e.Description));

        private readonly double 攻击力加成 = 0.46;

        public 糖糖一周年纪念武器技能(Character? character = null, Item? item = null) : base(SkillType.Passive, character)
        {
            Level = 1;
            Item = item;
            Dictionary<string, object> values = new()
            {
                { "exatk", 攻击力加成 }
            };
            Effects.Add(new ExATK2(this, values, character));
        }

        public override IEnumerable<Effect> AddPassiveEffectToCharacter()
        {
            return Effects;
        }
    }
}
