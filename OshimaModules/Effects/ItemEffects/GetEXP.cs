using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;

namespace Oshima.FunGame.OshimaModules.Effects.ItemEffects
{
    public class GetEXP : Effect
    {
        public override long Id => (long)EffectID.GetEXP;
        public override string Name => "立即获得经验值";
        public override string Description => $"角色立即获得 {实际获得:0.##} 点经验值。" + (Source != null && Skill.Character != Source || Skill is not OpenSkill ? $"来自：[ {Source} ]" + (Skill.Item != null ? $" 的 [ {Skill.Item.Name} ]" : (Skill is OpenSkill ? "" : $" 的 [ {Skill.Name} ]")) : "");
        public override EffectType EffectType { get; set; } = EffectType.Item;

        private readonly double 实际获得 = 0;

        public GetEXP(Skill skill, Dictionary<string, object> args, Character? source = null) : base(skill, args)
        {
            GamingQueue = skill.GamingQueue;
            Source = source;
            if (Values.Count > 0)
            {
                string key = Values.Keys.FirstOrDefault(s => s.Equals("exp", StringComparison.CurrentCultureIgnoreCase)) ?? "";
                if (key.Length > 0 && double.TryParse(Values[key].ToString(), out double exp) && exp > 0)
                {
                    实际获得 = exp;
                }
            }
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, Dictionary<string, object> others)
        {
            caster.EXP += 实际获得;
        }

        public override void OnSkillCasted(List<Character> targets, Dictionary<string, object> others)
        {
            foreach (Character target in targets)
            {
                target.EXP += 实际获得;
            }
        }
    }
}