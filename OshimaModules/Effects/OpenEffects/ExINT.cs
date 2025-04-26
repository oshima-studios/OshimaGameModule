using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Effects.OpenEffects
{
    public class ExINT : Effect
    {
        public override long Id => (long)EffectID.ExINT;
        public override string Name => "智力加成";
        public override string Description => $"{(实际加成 >= 0 ? "增加" : "减少")}角色 {Math.Abs(实际加成):0.##} 点智力。" + (Source != null && Skill.Character != Source ? $"来自：[ {Source} ]" + (Skill.Item != null ? $" 的 [ {Skill.Item.Name} ]" : "") : "");
        public double Value => 实际加成;

        private readonly double 实际加成 = 0;

        public override void OnEffectGained(Character character)
        {
            character.ExINT += 实际加成;
        }

        public override void OnEffectLost(Character character)
        {
            character.ExINT -= 实际加成;
        }

        public ExINT(Skill skill, Dictionary<string, object> args, Character? source = null) : base(skill, args)
        {
            EffectType = EffectType.Item;
            GamingQueue = skill.GamingQueue;
            Source = source;
            if (Values.Count > 0)
            {
                string key = Values.Keys.FirstOrDefault(s => s.Equals("exint", StringComparison.CurrentCultureIgnoreCase)) ?? "";
                if (key.Length > 0 && double.TryParse(Values[key].ToString(), out double exINT))
                {
                    实际加成 = exINT;
                }
            }
        }
    }
}
