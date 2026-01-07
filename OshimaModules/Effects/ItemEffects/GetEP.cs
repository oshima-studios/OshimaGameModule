using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;
using Oshima.FunGame.OshimaModules.Skills;

namespace Oshima.FunGame.OshimaModules.Effects.ItemEffects
{
    public class GetEP : Effect
    {
        public override long Id => (long)EffectID.GetEP;
        public override string Name => "立即获得能量值";
        public override string Description => $"{Skill.TargetDescription()}立即获得 {实际获得:0.##} 点能量值。" + (Source != null && (Skill.Character != Source || Skill is not OpenSkill) ? $"来自：[ {Source} ]" + (Skill.Item != null ? $" 的 [ {Skill.Item.Name} ]" : (Skill is OpenSkill ? "" : $" 的 [ {Skill.Name} ]")) : "");
        public override EffectType EffectType { get; set; } = EffectType.Item;

        private readonly double 实际获得 = 0;

        public GetEP(Skill skill, Dictionary<string, object> args, Character? source = null) : base(skill, args)
        {
            GamingQueue = skill.GamingQueue;
            Source = source;
            if (Values.Count > 0)
            {
                string key = Values.Keys.FirstOrDefault(s => s.Equals("ep", StringComparison.CurrentCultureIgnoreCase)) ?? "";
                if (key.Length > 0 && double.TryParse(Values[key].ToString(), out double ep) && ep > 0)
                {
                    实际获得 = ep;
                }
            }
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            foreach (Character target in targets)
            {
                target.EP += 实际获得;
                WriteLine($"[ {target} ] 获得了 {实际获得:0.##} 点能量值！");
            }
        }

        public override void OnSkillCasted(User user, List<Character> targets, Dictionary<string, object> others)
        {
            base.OnSkillCasted(user, targets, others);
            foreach (Character target in targets)
            {
                target.EP += 实际获得;
            }
        }
    }
}
