using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.ItemEffects
{
    public class 物理护甲加成 : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"增加角色 {实际物理护甲加成:0.##} 点物理护甲。" + (!TargetSelf ? $"来自：[ {Source} ]" + (Item != null ? $" 的 [ {Item.Name} ]" : "") : "");
        public override EffectType EffectType => EffectType.Item;
        public override bool TargetSelf => true;

        public Item? Item { get; }
        private readonly double 实际物理护甲加成 = 0;

        public override void OnEffectGained(Character character)
        {
            character.ExDEF2 += 实际物理护甲加成;
        }

        public override void OnEffectLost(Character character)
        {
            character.ExDEF2 -= 实际物理护甲加成;
        }

        public 物理护甲加成(Skill skill, double exDef, Character? source = null, Item? item = null) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            实际物理护甲加成 = exDef;
            Source = source;
            Item = item;
        }
    }
}
