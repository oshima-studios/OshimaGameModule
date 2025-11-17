using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;
using Oshima.FunGame.OshimaModules.Skills;

namespace Oshima.FunGame.OshimaModules.Items
{
    public class 攻击之爪10 : Item
    {
        public override long Id => (long)AccessoryID.攻击之爪10;
        public override string Name => "攻击之爪 +10";
        public override string Description => Skills.Passives.Count > 0 ? Skills.Passives.First().Description : "";
        public override string BackgroundStory => "里面藏着的小尖刃不容小觑。";
        public override QualityType QualityType => QualityType.White;

        public 攻击之爪10(Character? character = null) : base(ItemType.Accessory)
        {
            Skills.Passives.Add(new 攻击之爪技能(character, this, 10));
        }
    }

    public class 攻击之爪25 : Item
    {
        public override long Id => (long)AccessoryID.攻击之爪25;
        public override string Name => "攻击之爪 +25";
        public override string Description => Skills.Passives.Count > 0 ? Skills.Passives.First().Description : "";
        public override string BackgroundStory => "里面藏着的小尖刃不容小觑。";
        public override QualityType QualityType => QualityType.Green;

        public 攻击之爪25(Character? character = null) : base(ItemType.Accessory)
        {
            Skills.Passives.Add(new 攻击之爪技能(character, this, 25));
        }
    }

    public class 攻击之爪40 : Item
    {
        public override long Id => (long)AccessoryID.攻击之爪40;
        public override string Name => "攻击之爪 +40";
        public override string Description => Skills.Passives.Count > 0 ? Skills.Passives.First().Description : "";
        public override string BackgroundStory => "里面藏着的小尖刃不容小觑。";
        public override QualityType QualityType => QualityType.Blue;

        public 攻击之爪40(Character? character = null) : base(ItemType.Accessory)
        {
            Skills.Passives.Add(new 攻击之爪技能(character, this, 40));
        }
    }

    public class 攻击之爪55 : Item
    {
        public override long Id => (long)AccessoryID.攻击之爪55;
        public override string Name => "攻击之爪 +55";
        public override string Description => Skills.Passives.Count > 0 ? Skills.Passives.First().Description : "";
        public override string BackgroundStory => "里面藏着的小尖刃不容小觑。";
        public override QualityType QualityType => QualityType.Purple;

        public 攻击之爪55(Character? character = null) : base(ItemType.Accessory)
        {
            Skills.Passives.Add(new 攻击之爪技能(character, this, 55));
        }
    }

    public class 攻击之爪70 : Item
    {
        public override long Id => (long)AccessoryID.攻击之爪70;
        public override string Name => "攻击之爪 +70";
        public override string Description => Skills.Passives.Count > 0 ? Skills.Passives.First().Description : "";
        public override string BackgroundStory => "里面藏着的小尖刃不容小觑。";
        public override QualityType QualityType => QualityType.Orange;

        public 攻击之爪70(Character? character = null) : base(ItemType.Accessory)
        {
            Skills.Passives.Add(new 攻击之爪技能(character, this, 70));
        }
    }

    public class 攻击之爪85 : Item
    {
        public override long Id => (long)AccessoryID.攻击之爪85;
        public override string Name => "攻击之爪 +85";
        public override string Description => Skills.Passives.Count > 0 ? Skills.Passives.First().Description : "";
        public override string BackgroundStory => "里面藏着的小尖刃不容小觑。";
        public override QualityType QualityType => QualityType.Red;

        public 攻击之爪85(Character? character = null) : base(ItemType.Accessory)
        {
            Skills.Passives.Add(new 攻击之爪技能(character, this, 85));
        }
    }

    public class 攻击之爪100 : Item
    {
        public override long Id => (long)AccessoryID.攻击之爪100;
        public override string Name => "攻击之爪 +100";
        public override string Description => Skills.Passives.Count > 0 ? Skills.Passives.First().Description : "";
        public override string BackgroundStory => "里面藏着的小尖刃不容小觑。";
        public override QualityType QualityType => QualityType.Gold;

        public 攻击之爪100(Character? character = null) : base(ItemType.Accessory)
        {
            Skills.Passives.Add(new 攻击之爪技能(character, this, 100));
        }
    }

    public class 攻击之爪技能 : Skill
    {
        public override long Id => (long)ItemPassiveID.攻击之爪;
        public override string Name => "攻击之爪";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";

        public 攻击之爪技能(Character? character = null, Item? item = null, double exATK = 0) : base(SkillType.Passive, character)
        {
            Level = 1;
            Item = item;
            Dictionary<string, object> values = new()
            {
                { "exatk", exATK }
            };
            Effects.Add(new ExATK(this, values));
        }

        public override IEnumerable<Effect> AddPassiveEffectToCharacter()
        {
            return Effects;
        }
    }
}
