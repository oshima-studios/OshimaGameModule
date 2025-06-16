﻿using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Effects.SkillEffects
{
    public class 增加混合护盾值 : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"为{TargetDescription}提供 {护盾值:0.##} 点混合护盾值。";
        public string TargetDescription => Skill.SelectAllTeammates ? "友方全体角色" : $"目标{(Skill.CanSelectTargetCount > 1 ? $"至多 {Skill.CanSelectTargetCount} 个" : "")}友方角色";

        private double 护盾值 => Level > 0 ? Math.Abs(基础数值护盾 + 基础护盾等级成长 * (Level - 1)) : Math.Abs(基础数值护盾);
        private double 基础数值护盾 { get; set; } = 200;
        private double 基础护盾等级成长 { get; set; } = 100;

        public 增加混合护盾值(Skill skill, double 基础数值护盾, double 基础护盾等级成长) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            this.基础数值护盾 = 基础数值护盾;
            this.基础护盾等级成长 = 基础护盾等级成长;
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, Dictionary<string, object> others)
        {
            foreach (Character target in targets)
            {
                target.Shield.Mix += 护盾值;
                WriteLine($"[ {target} ] 获得了 {护盾值:0.##} 点混合护盾值！");
                GamingQueue?.LastRound.ApplyEffects.TryAdd(target, [EffectType.Shield]);
            }
        }
    }
}
