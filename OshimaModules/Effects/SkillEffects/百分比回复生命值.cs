using Milimoe.FunGame.Core.Entity;

namespace Oshima.FunGame.OshimaModules.Effects.SkillEffects
{
    public class 百分比回复生命值 : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"为{(TargetCount > 1 ? $"至多 {TargetCount} 个" : "")}目标回复其最大生命值 {百分比 * 100:0.##}% [ {Heal:0.##} ] 点生命值。";
        public override bool TargetSelf => true;
        public override int TargetCount { get; set; } = 1;

        private double Heal => Skill.Level > 0 ? 百分比 * (Skill.Character?.MaxHP ?? 0) : 0;
        private double 基础回复 { get; set; } = 0.03;
        private double 回复成长 { get; set; } = 0.03;
        private double 百分比 => Skill.Level > 0 ? 基础回复 + 回复成长 * (Skill.Level - 1) : 基础回复;
        private bool CanRespawn { get; set; } = false;

        public 百分比回复生命值(Skill skill, double 基础回复, double 回复成长, int targetCount = 1, bool canRespawn = false) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            this.基础回复 = 基础回复;
            this.回复成长 = 回复成长;
            TargetCount = targetCount;
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
