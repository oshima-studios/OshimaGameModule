using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Effects.OpenEffects
{
    public class ExCritRate : Effect
    {
        public override long Id => (long)EffectID.ExCritRate;
        public override string Name => "暴击率加成";
        public override string Description => $"增加角色 {实际加成 * 100:0.##}% 暴击率。" + (Source != null && Skill.Character != Source ? $"来自：[ {Source} ]" + (Skill.Item != null ? $" 的 [ {Skill.Item.Name} ]" : "") : "");
        public override EffectType EffectType => EffectType.Item;
        
        private readonly double 实际加成 = 0;

        public override void OnEffectGained(Character character)
        {
            character.ExCritRate += 实际加成;
        }

        public override void OnEffectLost(Character character)
        {
            character.ExCritRate -= 实际加成;
        }

        public ExCritRate(Skill skill, Dictionary<string, object> args, Character? source = null) : base(skill, args)
        {
            GamingQueue = skill.GamingQueue;
            Source = source;
            if (Values.Count > 0)
            {
                string key = Values.Keys.FirstOrDefault(s => s.Equals("excr", StringComparison.CurrentCultureIgnoreCase)) ?? "";
                if (key.Length > 0 && double.TryParse(Values[key].ToString(), out double exCR))
                {
                    实际加成 = exCR;
                }
            }
        }
    }
}
