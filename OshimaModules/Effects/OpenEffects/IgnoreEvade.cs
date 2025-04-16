using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Effects.OpenEffects
{
    public class IgnoreEvade : Effect
    {
        public override long Id => (long)EffectID.IgnoreEvade;
        public override string Name => "无视闪避";
        public override string Description => $"普通攻击有 {概率 * 100:0.##}% 概率无视闪避。" + (Source != null && Skill.Character != Source ? $"来自：[ {Source} ]" + (Skill.Item != null ? $" 的 [ {Skill.Item.Name} ]" : "") : "");
        public override EffectType EffectType => EffectType.Item;
        public double Value => 概率;

        private readonly double 概率 = 0;

        public override bool BeforeEvadeCheck(Character actor, Character enemy, ref double throwingBonus)
        {
            if (actor == Skill.Character)
            {
                if (GamingQueue != null) WriteLine($"[ {actor} ] 的普通攻击无视了 [ {enemy} ] 的闪避！");
                return false;
            }
            return true;
        }

        public IgnoreEvade(Skill skill, Dictionary<string, object> args, Character? source = null) : base(skill, args)
        {
            GamingQueue = skill.GamingQueue;
            Source = source;
            if (Values.Count > 0)
            {
                string key = Values.Keys.FirstOrDefault(s => s.Equals("p", StringComparison.CurrentCultureIgnoreCase)) ?? "";
                if (key.Length > 0 && double.TryParse(Values[key].ToString(), out double p) && p >= 0 && p <= 1)
                {
                    概率 = p;
                }
            }
        }
    }
}
