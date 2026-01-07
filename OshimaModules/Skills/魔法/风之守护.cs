using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 风之守护 : Skill
    {
        public override long Id => (long)MagicID.风之守护;
        public override string Name => "风之守护";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override double MPCost => Level > 0 ? 75 + (80 * (Level - 1)) : 75;
        public override double CD => Level > 0 ? 30 - (0.5 * (Level - 1)) : 30;
        public override double CastTime => Level > 0 ? 4 + (1 * (Level - 1)) : 4;
        public override double HardnessTime { get; set; } = 3;
        public override bool CanSelectSelf => true;
        public override bool CanSelectEnemy => false;
        public override bool CanSelectTeammate => true;
        public override int CanSelectTargetCount => 1;

        public 风之守护(Character? character = null) : base(SkillType.Magic, character)
        {
            Effects.Add(new 风之守护特效(this, false, 0, 4));
        }
    }

    public class 风之守护特效 : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"提升目标{(Skill.CanSelectTargetCount > 1 ? $"至多 {Skill.CanSelectTargetCount} 个" : "")}友方角色 {CritRate * 100:0.##}% 暴击率和 {EvadeRate * 100:0.##}% 闪避率，持续 {持续时间}。";
        public override EffectType EffectType => EffectType.CritBoost;
        public override DispelledType DispelledType => DispelledType.Weak;

        private string 持续时间 => _durative && _duration > 0 ? 实际持续时间 + $" {GameplayEquilibriumConstant.InGameTime}" : (!_durative && _durationTurn > 0 ? 实际持续时间 + " 回合" : $"0 {GameplayEquilibriumConstant.InGameTime}");
        private double 实际持续时间 => _durative && _duration > 0 ? _duration + _levelGrowth * (Level - 1) : (!_durative && _durationTurn > 0 ? _durationTurn + _levelGrowth * (Level - 1) : 0);

        private readonly bool _durative;
        private readonly double _duration;
        private readonly int _durationTurn;
        private readonly double _levelGrowth;

        private double CritRate => Level > 0 ? 0.03 + 0.03 * (Level - 1) : 0.03;
        private double EvadeRate => Level > 0 ? 0.02 + 0.02 * (Level - 1) : 0.02;

        public 风之守护特效(Skill skill, bool durative = false, double duration = 0, int durationTurn = 1, double levelGrowth = 0) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            _durative = durative;
            _duration = duration;
            _durationTurn = durationTurn;
            _levelGrowth = levelGrowth;
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            foreach (Character target in targets)
            {
                WriteLine($"[ {target} ] 的暴击率提升了 {CritRate * 100:0.##}%，闪避率提升了 {EvadeRate * 100:0.##}%！持续时间：{持续时间}！");
                ExCritRate e = new(Skill, new()
                {
                    { "excr", CritRate }
                }, caster);
                target.Effects.Add(e);
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
                e.EffectType = EffectType.CritBoost;
                e.Source = caster;
                e.OnEffectGained(target);
                ExEvadeRate e2 = new(Skill, new()
                {
                    { "exer", EvadeRate }
                }, caster);
                target.Effects.Add(e2);
                if (_durative && _duration > 0)
                {
                    e2.Durative = true;
                    e2.Duration = 实际持续时间;
                    e2.RemainDuration = 实际持续时间;
                }
                else if (!_durative && _durationTurn > 0)
                {
                    e2.Durative = false;
                    e2.DurationTurn = (int)实际持续时间;
                    e2.RemainDurationTurn = (int)实际持续时间;
                }
                e2.EffectType = EffectType.EvadeBoost;
                e2.Source = caster;
                e2.OnEffectGained(target);
                GamingQueue?.LastRound.AddApplyEffects(target, EffectType.CritBoost, EffectType.EvadeBoost);
            }
        }
    }
}
