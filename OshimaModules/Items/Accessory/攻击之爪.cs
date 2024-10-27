using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.ItemEffects;
using Oshima.FunGame.OshimaModules.Skills;

namespace Oshima.FunGame.OshimaModules.Items
{
    public class 攻击之爪10 : Item
    {
        public override long Id => (long)AccessoryID.攻击之爪10;
        public override string Name => "攻击之爪 +10";
        public override string Description => Skills.Passives.Count > 0 ? Skills.Passives.First().Description : "";

        public 攻击之爪10(Character? character = null) : base(ItemType.Accessory, slot: EquipSlotType.Accessory)
        {
            Skills.Passives.Add(new 攻击之爪技能(character, this, 10));
        }
    }

    public class 攻击之爪30 : Item
    {
        public override long Id => (long)AccessoryID.攻击之爪30;
        public override string Name => "攻击之爪 +30";
        public override string Description => Skills.Passives.Count > 0 ? Skills.Passives.First().Description : "";

        public 攻击之爪30(Character? character = null) : base(ItemType.Accessory, slot: EquipSlotType.Accessory)
        {
            Skills.Passives.Add(new 攻击之爪技能(character, this, 30));
        }
    }

    public class 攻击之爪50 : Item
    {
        public override long Id => (long)AccessoryID.攻击之爪50;
        public override string Name => "攻击之爪 +50";
        public override string Description => Skills.Passives.Count > 0 ? Skills.Passives.First().Description : "";

        public 攻击之爪50(Character? character = null) : base(ItemType.Accessory, slot: EquipSlotType.Accessory)
        {
            Skills.Passives.Add(new 攻击之爪技能(character, this, 50));
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
            Effects.Add(new 攻击力加成(this, exATK, character, item));
        }

        public override IEnumerable<Effect> AddInactiveEffectToCharacter()
        {
            return Effects;
        }
    }
}
