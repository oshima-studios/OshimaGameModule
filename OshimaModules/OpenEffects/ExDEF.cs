using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.OpenEffects
{
    public class ExDEF : Effect
    {
        public override long Id => (long)EffectID.ExDEF;
        public override string Name => "物理护甲加成";
        public override string Description => $"增加角色 {实际加成:0.##} 点物理护甲。" + (!TargetSelf ? $"来自：[ {Source} ]" + (Item != null ? $" 的 [ {Item.Name} ]" : "") : "");
        public override EffectType EffectType => EffectType.Item;
        public override bool TargetSelf => true;

        public Item? Item { get; }
        private readonly double 实际加成 = 0;

        public override void OnEffectGained(Character character)
        {
            character.ExDEF2 += 实际加成;
        }

        public override void OnEffectLost(Character character)
        {
            character.ExDEF2 -= 实际加成;
        }

        public ExDEF(Skill skill, Character? source, Item? item) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            Source = source;
            Item = item;
            if (skill.OtherArgs.Count > 0)
            {
                string key = skill.OtherArgs.Keys.FirstOrDefault(s => s.Equals("exdef", StringComparison.CurrentCultureIgnoreCase)) ?? "";
                if (key.Length > 0 && double.TryParse(skill.OtherArgs[key].ToString(), out double exDEF))
                {
                    实际加成 = exDEF;
                }
            }
        }
    }
}
