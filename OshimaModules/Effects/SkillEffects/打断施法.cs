using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Effects.SkillEffects
{
    public class 打断施法 : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"对{Skill.TargetDescription()}施加打断施法效果：中断其正在进行的吟唱。";
        public override EffectType EffectType => EffectType.InterruptCasting;

        public 打断施法(Skill skill) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            foreach (Character target in targets)
            {
                if (!CheckExemption(caster, target, this))
                {
                    InterruptCasting(target, caster);
                }
            }
        }
    }
}
