using Milimoe.FunGame.Core.Entity;

namespace Oshima.FunGame.OshimaModules.Effects.SkillEffects
{
    public class 纯数值回复生命 : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"为{(Skill.CanSelectTargetCount > 1 ? $"至多 {Skill.CanSelectTargetCount} 个" : "")}目标回复 {Heal:0.##} 点生命值。";

        private double Heal => Skill.Level > 0 ? 基础回复 + 回复成长 * (Skill.Level - 1) : 基础回复;
        private double 基础回复 { get; set; } = 100;
        private double 回复成长 { get; set; } = 30;
        private bool CanRespawn { get; set; } = false;

        public 纯数值回复生命(Skill skill, double 基础回复, double 回复成长, bool canRespawn = false) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            this.基础回复 = 基础回复;
            this.回复成长 = 回复成长;
            CanRespawn = canRespawn;
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, Dictionary<string, object> others)
        {
            foreach (Character target in targets)
            {
                HealToTarget(caster, target, Heal, CanRespawn);
            }
        }
    }
}
