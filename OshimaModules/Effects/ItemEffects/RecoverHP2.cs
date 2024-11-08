using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;

namespace Oshima.FunGame.OshimaModules.Effects.ItemEffects
{
    public class RecoverHP2 : Effect
    {
        public override long Id => (long)EffectID.RecoverHP2;
        public override string Name => "立即回复生命值";
        public override string Description => $"立即回复角色 {回复比例 * 100:0.##}% [ {实际回复:0.##} ] 点生命值（不可用于复活）。" + (Source != null && Skill.Character != Source ? $"来自：[ {Source} ]" + (Skill.Item != null ? $" 的 [ {Skill.Item.Name} ]" : "") : "");
        public override EffectType EffectType => EffectType.Item;

        private double 实际回复 => 回复比例 * (Skill.Character?.MaxHP ?? 0);
        private readonly double 回复比例 = 0;

        public RecoverHP2(Skill skill, Dictionary<string, object> args, Character? source = null) : base(skill, args)
        {
            GamingQueue = skill.GamingQueue;
            Source = source;
            if (Values.Count > 0)
            {
                string key = Values.Keys.FirstOrDefault(s => s.Equals("hp", StringComparison.CurrentCultureIgnoreCase)) ?? "";
                if (key.Length > 0 && double.TryParse(Values[key].ToString(), out double hp))
                {
                    回复比例 = hp;
                }
            }
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, Dictionary<string, object> others)
        {
            HealToTarget(caster, caster, 实际回复, false);
        }
    }
}
