using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Oshima.FunGame.OshimaModules.Skills;

namespace Oshima.FunGame.OshimaModules.Effects.SkillEffects
{
    public class 吸取魔法值 : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"吸取{Skill.TargetDescription()}的{(是否是百分比 ? $" {Calculation.PercentageCheck(Earned) * 100:0.##}% 当前" : $"至多 {Earned:0.##} 点")}魔法值（每个角色），" +
            $"并将吸取总量的 {转化百分比 * 100:0.##}% 转化为自身魔法值。";

        private double Earned => Skill.Level > 0 ? 基础数值 + 等级成长 * (Skill.Level - 1) : 基础数值;
        private double 基础数值 { get; set; } = 150;
        private double 等级成长 { get; set; } = 75;
        private bool 是否是百分比 { get; set; } = false;
        private double 转化百分比 { get; set; } = 1;

        public 吸取魔法值(Skill skill, double 基础数值, double 等级成长, bool 是否是百分比 = false, double 转化百分比 = 1) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            this.基础数值 = 基础数值;
            this.等级成长 = 等级成长;
            this.是否是百分比 = 是否是百分比;
            this.转化百分比 = Calculation.PercentageCheck(转化百分比);
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, Dictionary<string, object> others)
        {
            double total = 0;
            foreach (Character target in targets)
            {
                double earned = Earned;
                if (是否是百分比)
                {
                    earned = target.MP * earned;
                }
                else if (earned > target.MP)
                {
                    earned = target.MP;
                }
                if (earned > 0)
                {
                    target.MP -= earned;
                    total += earned;
                }
            }
            total *= 转化百分比;
            caster.MP += total;
            WriteLine($"[ {caster} ] 吸取了 [ {string.Join(" ] / [ ", targets)} ] 的 {total:0.##} 点魔法值！");
        }
    }
}
