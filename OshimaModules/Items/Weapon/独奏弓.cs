using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;

namespace Oshima.FunGame.OshimaModules.Items
{
    [Obsolete("测试物品请勿使用")]
    public class 独奏弓 : Item
    {
        public override long Id => (long)WeaponID.独奏弓;
        public override string Name => "独奏弓";
        public override string Description => Skills.Passives.Count > 0 ? Skills.Passives.First().Description : "";

        public 独奏弓(Character? character = null) : base(ItemType.Weapon)
        {
            WeaponType = WeaponType.Bow;
            Skills.Passives.Add(new 独奏弓技能(character, this));
        }
    }

    public class 独奏弓技能 : Skill
    {
        public override long Id => (long)WeaponID.独奏弓;
        public override string Name => "独奏弓";
        public override string Description => string.Join("\r\n", Effects.Select(e => e.Description));

        private readonly double 攻击力加成 = 80;
        private readonly double 硬直时间减少 = 2;

        public 独奏弓技能(Character? character, Item item) : base(SkillType.Passive, character)
        {
            Level = 1;
            Item = item;
            Dictionary<string, object> values = new()
            {
                { "exatk", 攻击力加成 },
                { "hatr", 硬直时间减少 }
            };
            Effects.Add(new ExATK(this, values, character));
            Effects.Add(new NormalAttackHardTimeReduce(this, values, character));
        }

        public override IEnumerable<Effect> AddPassiveEffectToCharacter()
        {
            return Effects;
        }
    }
}
