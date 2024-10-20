using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.ItemEffects
{
    public class 技能硬直时间减少 : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"减少角色的所有主动技能 {实际硬直时间减少:0.##} 硬直时间。" + (!TargetSelf ? $"来自：[ {Source} ]" + (Item != null ? $" 的 [ {Item.Name} ]" : "") : "");
        public override EffectType EffectType => EffectType.Item;
        public override bool TargetSelf => true;

        public Item? Item { get; }
        private readonly double 实际硬直时间减少 = 2;

        public override void OnEffectGained(Character character)
        {
            foreach (Skill s in character.Skills)
            {
                s.HardnessTime -= 实际硬直时间减少;
            }
            foreach (Skill? s in character.Items.Select(i => i.Skills.Active))
            {
                if (s != null)
                    s.HardnessTime -= 实际硬直时间减少;
            }
        }

        public override void OnEffectLost(Character character)
        {
            foreach (Skill s in character.Skills)
            {
                s.HardnessTime += 实际硬直时间减少;
            }
            foreach (Skill? s in character.Items.Select(i => i.Skills.Active))
            {
                if (s != null)
                    s.HardnessTime += 实际硬直时间减少;
            }
        }

        public 技能硬直时间减少(Skill skill, Character? source, Item? item, double reduce) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            Source = source;
            Item = item;
            实际硬直时间减少 = reduce;
        }
    }
}
