using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.PassiveEffects;

namespace Oshima.FunGame.OshimaModules.Effects.SkillEffects
{
    public class 增加魔法护盾_特效持续型 : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"为{TargetDescription}提供 {护盾值:0.##} 点魔法护盾，持续 {持续时间}。";
        public string TargetDescription => Skill.SelectAllTeammates ? "友方全体角色" : $"目标{(Skill.CanSelectTargetCount > 1 ? $"至多 {Skill.CanSelectTargetCount} 个" : "")}友方角色";

        private string 持续时间 => _durative && _duration > 0 ? _duration + $" {GameplayEquilibriumConstant.InGameTime}" : (!_durative && _durationTurn > 0 ? _durationTurn + " 回合" : $"0 {GameplayEquilibriumConstant.InGameTime}");
        private double 护盾值 => Level > 0 ? Math.Abs(基础数值护盾 + 基础护盾等级成长 * (Level - 1)) : Math.Abs(基础数值护盾);
        private double 基础数值护盾 { get; set; } = 200;
        private double 基础护盾等级成长 { get; set; } = 100;
        private readonly bool _durative;
        private readonly double _duration;
        private readonly int _durationTurn;

        public 增加魔法护盾_特效持续型(Skill skill, double 基础数值护盾, double 基础护盾等级成长, bool durative = false, double duration = 0, int durationTurn = 0) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            this.基础数值护盾 = 基础数值护盾;
            this.基础护盾等级成长 = 基础护盾等级成长;
            _durative = durative;
            _duration = duration;
            _durationTurn = durationTurn;
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, Dictionary<string, object> others)
        {
            foreach (Character target in targets)
            {
                WriteLine($"[ {target} ] 获得了 {护盾值:0.##} 点魔法护盾！");
                魔法护盾 e = new(Skill, target, caster, 护盾值, _durative, _duration, _durationTurn)
                {
                    ParentEffect = ParentEffect
                };
                target.Effects.Add(e);
                e.OnEffectGained(target);
                e.DispelledType = DispelledType;
                GamingQueue?.LastRound.ApplyEffects.TryAdd(target, [EffectType.Shield]);
            }
        }
    }
}
