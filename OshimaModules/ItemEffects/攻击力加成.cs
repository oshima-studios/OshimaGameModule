using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.ItemEffects
{
    public class 攻击力加成 : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"增加角色 {实际攻击力加成:0.##} 点攻击力。" + (!TargetSelf ? $"来自：[ {Source} ]" + (Item != null ? $" 的 [ {Item.Name} ]" : "") : "");
        public override EffectType EffectType => EffectType.Item;
        public override bool TargetSelf => true;

        public Item? Item { get; }
        private readonly double 实际攻击力加成 = 0;

        public override void OnEffectGained(Character character)
        {
            character.ExATK2 += 实际攻击力加成;
        }

        public override void OnEffectLost(Character character)
        {
            character.ExATK2 -= 实际攻击力加成;
        }

        public 攻击力加成(Skill skill, double exATK, Character? source = null, Item? item = null) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            实际攻击力加成 = exATK;
            Source = source;
            Item = item;
        }
    }
}
