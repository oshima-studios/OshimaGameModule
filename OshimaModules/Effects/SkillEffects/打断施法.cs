using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;

namespace Oshima.FunGame.OshimaModules.Effects.SkillEffects
{
    public class 打断施法 : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"打断施法：中断目标正在进行的吟唱。";

        public 打断施法(Skill skill) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
        }

        public override async Task OnSkillCasted(Character caster, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            foreach (Character target in targets)
            {
                InterruptCasting(target, caster);
            }
        }
    }
}
