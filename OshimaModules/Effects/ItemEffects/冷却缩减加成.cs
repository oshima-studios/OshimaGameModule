using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Effects.ItemEffects
{
    public class 冷却缩减加成 : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"增加角色 {实际冷却缩减加成 * 100:0.##}% 冷却缩减。" + (!TargetSelf ? $"来自：[ {Source} ]" + (Item != null ? $" 的 [ {Item.Name} ]" : "") : "");
        public override EffectType EffectType => EffectType.Item;
        public override bool TargetSelf => true;

        public Item? Item { get; }
        private readonly double 实际冷却缩减加成 = 0;

        public override void OnEffectGained(Character character)
        {
            character.ExCDR += 实际冷却缩减加成;
        }

        public override void OnEffectLost(Character character)
        {
            character.ExCDR -= 实际冷却缩减加成;
        }

        public 冷却缩减加成(Skill skill, double exCdr, Character? source = null, Item? item = null) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            实际冷却缩减加成 = exCdr;
            Source = source;
            Item = item;
        }
    }
}
