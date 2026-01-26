using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 强音之力复 : Skill
    {
        public override long Id => (long)MagicID.强音之力复;
        public override string Name => "强音之力·复";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override double MPCost => Level > 0 ? 85 + (85 * (Level - 1)) : 85;
        public override double CD => Level > 0 ? 45 - (0.5 * (Level - 1)) : 45;
        public override double CastTime => Level > 0 ? 3 + (0.5 * (Level - 1)) : 3;
        public override double HardnessTime { get; set; } = 6;
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
        public override double MagicBottleneck => 14 + 15 * (Level - 1);

        public 强音之力复(Character? character = null) : base(SkillType.Magic, character)
        {
            Effects.Add(new 强音之力复特效(this, false, 0, 3));
        }
    }

    public class 强音之力复特效 : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"提升目标{(Skill.CanSelectTargetCount > 1 ? $"至多 {Skill.CanSelectTargetCount} 个" : "")}友方角色 {ExATK * 100:0.##}% 攻击力，持续 {持续时间}。";
        public override EffectType EffectType => EffectType.DamageBoost;
        public override DispelledType DispelledType => DispelledType.Weak;

        private string 持续时间 => _durative && _duration > 0 ? 实际持续时间 + $" {GameplayEquilibriumConstant.InGameTime}" : (!_durative && _durationTurn > 0 ? 实际持续时间 + " 回合" : $"0 {GameplayEquilibriumConstant.InGameTime}");
        private double 实际持续时间 => _durative && _duration > 0 ? (_duration + _levelGrowth * (Level - 1) * MagicEfficacy) : (!_durative && _durationTurn > 0 ? ((int)Math.Round(_durationTurn + _levelGrowth * (Level - 1) * MagicEfficacy, 0, MidpointRounding.ToPositiveInfinity)) : 0);

        private readonly bool _durative;
        private readonly double _duration;
        private readonly int _durationTurn;
        private readonly double _levelGrowth;

        private double ExATK => Level > 0 ? 0.045 + 0.045 * (Level - 1) : 0.045;

        public 强音之力复特效(Skill skill, bool durative = false, double duration = 0, int durationTurn = 1, double levelGrowth = 0) : base(skill)
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
                WriteLine($"[ {target} ] 的攻击力提升了 {ExATK * 100:0.##}%！持续时间：{持续时间}！");
                ExATK2 e = new(Skill, new()
                {
                    { "exatk", ExATK }
                }, caster)
                {
                    Name = Name
                };
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
                e.EffectType = EffectType.DamageBoost;
                e.Source = caster;
                e.OnEffectGained(target);
                GamingQueue?.LastRound.AddApplyEffects(target, EffectType.DamageBoost);
            }
        }
    }
}
