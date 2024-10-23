using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.OpenEffects
{
    public class ExCDR : Effect
    {
        public override long Id => (long)EffectID.ExCDR;
        public override string Name => "冷却缩减加成";
        public override string Description => $"增加角色 {实际加成 * 100:0.##}% 冷却缩减。" + (!TargetSelf ? $"来自：[ {Source} ]" + (Item != null ? $" 的 [ {Item.Name} ]" : "") : "");
        public override EffectType EffectType => EffectType.Item;
        public override bool TargetSelf => true;

        public Item? Item { get; }
        private readonly double 实际加成 = 0;

        public override void OnEffectGained(Character character)
        {
            character.ExCDR += 实际加成;
        }

        public override void OnEffectLost(Character character)
        {
            character.ExCDR -= 实际加成;
        }

        public ExCDR(Skill skill, Character? source, Item? item) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            Source = source;
            Item = item;
            if (skill.OtherArgs.Count > 0)
            {
                string key = skill.OtherArgs.Keys.FirstOrDefault(s => s.Equals("excdr", StringComparison.CurrentCultureIgnoreCase)) ?? "";
                if (key.Length > 0 && double.TryParse(skill.OtherArgs[key].ToString(), out double exCDR))
                {
                    实际加成 = exCDR;
                }
            }
        }
    }
}
