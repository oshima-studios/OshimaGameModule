using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 结晶防护复 : Skill
    {
        public override long Id => (long)MagicID.结晶防护复;
        public override string Name => "结晶防护·复";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override double MPCost => Level > 0 ? 90 + (95 * (Level - 1)) : 90;
        public override double CD => Level > 0 ? 35 - (0.5 * (Level - 1)) : 35;
        public override double CastTime => Level > 0 ? 4 + (1.5 * (Level - 1)) : 4;
        public override double HardnessTime { get; set; } = 5;
        public override bool CanSelectSelf => true;
        public override bool CanSelectEnemy => false;
        public override bool CanSelectTeammate => true;
        public override int CanSelectTargetCount
        {
            get
            {
                return Level switch
                {
                    3 => 3,
                    4 => 3,
                    5 => 3,
                    6 => 4,
                    7 => 4,
                    8 => 4,
                    _ => 2
                };
            }
        }

        public 结晶防护复(Character? character = null) : base(SkillType.Magic, character)
        {
            Effects.Add(new 结晶防护复特效(this, false, 0, 3));
        }
    }

    public class 结晶防护复特效 : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"提升目标{(Skill.CanSelectTargetCount > 1 ? $"至多 {Skill.CanSelectTargetCount} 个" : "")}友方角色 {ExDEF * 100:0.##}% 物理护甲和 {ExMDF * 100:0.##}% 魔法抗性，持续 {持续时间}。";
        public override EffectType EffectType => EffectType.DefenseBoost;
        public override DispelledType DispelledType => DispelledType.Weak;

        private string 持续时间 => _durative && _duration > 0 ? 实际持续时间 + $" {GameplayEquilibriumConstant.InGameTime}" : (!_durative && _durationTurn > 0 ? 实际持续时间 + " 回合" : $"0 {GameplayEquilibriumConstant.InGameTime}");
        private double 实际持续时间 => _durative && _duration > 0 ? _duration + _levelGrowth * (Level - 1) : (!_durative && _durationTurn > 0 ? _durationTurn + _levelGrowth * (Level - 1) : 0);

        private readonly bool _durative;
        private readonly double _duration;
        private readonly int _durationTurn;
        private readonly double _levelGrowth;

        private double ExDEF => Level > 0 ? 0.15 + 0.15 * (Level - 1) : 0.15;
        private double ExMDF => Level > 0 ? 0.02 + 0.02 * (Level - 1) : 0.02;

        public 结晶防护复特效(Skill skill, bool durative = false, double duration = 0, int durationTurn = 1, double levelGrowth = 0) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            _durative = durative;
            _duration = duration;
            _durationTurn = durationTurn;
            _levelGrowth = levelGrowth;
        }

        public override async Task OnSkillCasted(Character caster, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            foreach (Character target in targets)
            {
                WriteLine($"[ {target} ] 的物理护甲提升了 {ExDEF * 100:0.##}%，魔法抗性提升了 {ExMDF * 100:0.##}%！持续时间：{持续时间}！");
                ExDEF2 e = new(Skill, new()
                {
                    { "exdef", ExDEF }
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
                e.EffectType = EffectType.DefenseBoost;
                e.Source = caster;
                e.OnEffectGained(target);
                ExMDF e2 = new(Skill, new()
                {
                    { "mdftype", 0 },
                    { "mdfvalue", ExMDF }
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
                e2.EffectType = EffectType.DefenseBoost;
                e2.Source = caster;
                e2.OnEffectGained(target);
                GamingQueue?.LastRound.AddApplyEffects(target, EffectType.DefenseBoost);
            }
        }
    }
}
