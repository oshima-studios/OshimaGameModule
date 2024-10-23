using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.OpenEffects
{
    public class SkillHardTimeReduce : Effect
    {
        public override long Id => (long)EffectID.SkillHardTimeReduce;
        public override string Name => Skill.Name;
        public override string Description => $"减少角色的所有主动技能 {实际硬直时间减少:0.##} 硬直时间。" + (!TargetSelf ? $"来自：[ {Source} ]" + (Item != null ? $" 的 [ {Item.Name} ]" : "") : "");
        public override EffectType EffectType => EffectType.Item;
        public override bool TargetSelf => true;

        public Item? Item { get; }
        private readonly double 实际硬直时间减少 = 0;

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

        public SkillHardTimeReduce(Skill skill, Character? source, Item? item) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            Source = source;
            Item = item;
            if (skill.OtherArgs.Count > 0)
            {
                string key = skill.OtherArgs.Keys.FirstOrDefault(s => s.Equals("shtr", StringComparison.CurrentCultureIgnoreCase)) ?? "";
                if (key.Length > 0 && double.TryParse(skill.OtherArgs[key].ToString(), out double shtr))
                {
                    实际硬直时间减少 = shtr;
                }
            }
        }
    }
}
