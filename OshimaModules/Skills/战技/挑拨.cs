using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.PassiveEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 挑拨 : Skill
    {
        public override long Id => (long)SkillID.挑拨;
        public override string Name => "挑拨";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";
        public override double EPCost => 65;
        public override double CD => 55;
        public override double HardnessTime { get; set; } = 10;
        public override bool CanSelectSelf => false;
        public override bool CanSelectTeammate => false;
        public override bool CanSelectEnemy => true;
        public override int CanSelectTargetCount => 3;

        public 挑拨(Character? character = null) : base(SkillType.Skill, character)
        {
            CastRange = 4;
            Effects.Add(new 挑拨特效(this));
        }
    }

    public class 挑拨特效 : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"对目标{(Skill.CanSelectTargetCount > 1 ? $"至多 {Skill.CanSelectTargetCount} 个" : "")}敌方角色施加愤怒状态。愤怒：行动受限且失控，行动回合中无法自主行动，仅能对施法者发起普通攻击。持续 {持续时间}。";
        public override EffectType EffectType => EffectType.Taunt;
        public override DispelledType DispelledType => DispelledType.Strong;
        public override bool ExemptDuration => true;

        private string 持续时间 => _durative && _duration > 0 ? 实际持续时间 + $" {GameplayEquilibriumConstant.InGameTime}" : (!_durative && _durationTurn > 0 ? 实际持续时间 + " 回合" : $"0 {GameplayEquilibriumConstant.InGameTime}");
        private double 实际持续时间 => _durative && _duration > 0 ? _duration + _levelGrowth * (Level - 1) : (!_durative && _durationTurn > 0 ? _durationTurn + _levelGrowth * (Level - 1) : 0);

        private readonly bool _durative = false;
        private readonly double _duration = 0;
        private readonly int _durationTurn = 2;
        private readonly double _levelGrowth = 0;

        public 挑拨特效(Skill skill) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
        }

        public override async Task OnSkillCasted(Character caster, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            foreach (Character target in targets)
            {
                WriteLine($"[ {caster} ] 嘲讽了 [ {target} ] ，[ {target} ] 愤怒了！！持续时间：{持续时间}！");
                愤怒 e = new(Skill, caster, caster);
                if (_durative && _duration > 0)
                {
                    e.Durative = true;
                    e.Duration = 实际持续时间;
                    e.RemainDuration = 实际持续时间;
                }
                else if (!_durative && _durationTurn > 0)
                {
                    e.Durative = false;
                    e.DurationTurn = (int)实际持续时间;
                    e.RemainDurationTurn = (int)实际持续时间;
                }
                target.Effects.Add(e);
                e.OnEffectGained(target);
                GamingQueue?.LastRound.AddApplyEffects(target, e.EffectType);
            }
        }
    }
}
