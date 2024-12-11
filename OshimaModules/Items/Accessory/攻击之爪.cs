using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;
using Oshima.FunGame.OshimaModules.Skills;

namespace Oshima.FunGame.OshimaModules.Items
{
    public class 攻击之爪5 : Item
    {
        public override long Id => (long)AccessoryID.攻击之爪5;
        public override string Name => "攻击之爪 +5";
        public override string Description => Skills.Passives.Count > 0 ? Skills.Passives.First().Description : "";
        public override QualityType QualityType => QualityType.White;

        public 攻击之爪5(Character? character = null) : base(ItemType.Accessory)
        {
            Skills.Passives.Add(new 攻击之爪技能(character, this, 5));
        }
    }

    public class 攻击之爪15 : Item
    {
        public override long Id => (long)AccessoryID.攻击之爪15;
        public override string Name => "攻击之爪 +15";
        public override string Description => Skills.Passives.Count > 0 ? Skills.Passives.First().Description : "";
        public override QualityType QualityType => QualityType.Green;

        public 攻击之爪15(Character? character = null) : base(ItemType.Accessory)
        {
            Skills.Passives.Add(new 攻击之爪技能(character, this, 15));
        }
    }

    public class 攻击之爪25 : Item
    {
        public override long Id => (long)AccessoryID.攻击之爪25;
        public override string Name => "攻击之爪 +25";
        public override string Description => Skills.Passives.Count > 0 ? Skills.Passives.First().Description : "";
        public override QualityType QualityType => QualityType.Blue;

        public 攻击之爪25(Character? character = null) : base(ItemType.Accessory)
        {
            Skills.Passives.Add(new 攻击之爪技能(character, this, 25));
        }
    }

    public class 攻击之爪35 : Item
    {
        public override long Id => (long)AccessoryID.攻击之爪35;
        public override string Name => "攻击之爪 +35";
        public override string Description => Skills.Passives.Count > 0 ? Skills.Passives.First().Description : "";
        public override QualityType QualityType => QualityType.Purple;

        public 攻击之爪35(Character? character = null) : base(ItemType.Accessory)
        {
            Skills.Passives.Add(new 攻击之爪技能(character, this, 35));
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
            Effects.Add(new ExATK(this, values, character));
        }

        public override IEnumerable<Effect> AddInactiveEffectToCharacter()
        {
            return Effects;
        }
    }
}
