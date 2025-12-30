using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 助威 : Skill
    {
        public override long Id => (long)SkillID.助威;
        public override string Name => "助威";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";
        public override double EPCost => 75;
        public override double CD => 35;
        public override double HardnessTime { get; set; } = 7;
        public override bool CanSelectSelf => true;
        public override bool CanSelectTeammate => true;
        public override bool CanSelectEnemy => false;
        public override int CanSelectTargetCount => 3;

        public 助威(Character? character = null) : base(SkillType.Skill, character)
        {
            CastRange = 5;
            Effects.Add(new 助威特效(this));
        }
    }

    public class 助威特效 : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"提升目标{(Skill.CanSelectTargetCount > 1 ? $"至多 {Skill.CanSelectTargetCount} 个" : "")}友方角色 {ATK * 100:0.##}% 攻击力，持续 {持续时间}。";
        public override EffectType EffectType => EffectType.DamageBoost;
        public override DispelledType DispelledType => DispelledType.Weak;

        private string 持续时间 => _durative && _duration > 0 ? 实际持续时间 + $" {GameplayEquilibriumConstant.InGameTime}" : (!_durative && _durationTurn > 0 ? 实际持续时间 + " 回合" : $"0 {GameplayEquilibriumConstant.InGameTime}");
        private double 实际持续时间 => _durative && _duration > 0 ? _duration + _levelGrowth * (Level - 1) : (!_durative && _durationTurn > 0 ? _durationTurn + _levelGrowth * (Level - 1) : 0);

        private readonly bool _durative = false;
        private readonly double _duration = 0;
        private readonly int _durationTurn = 3;
        private readonly double _levelGrowth = 0;

        private double ATK => Level > 0 ? 0.07 + 0.03 * (Level - 1) : 0.03;

        public 助威特效(Skill skill) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, Dictionary<string, object> others)
        {
            foreach (Character target in targets)
            {
                WriteLine($"[ {target} ] 的攻击力提升了 {ATK * 100:0.##}% [ {target.BaseATK * ATK:0.##} ] 点！持续时间：{持续时间}！");
                ExATK2 e = new(Skill, new()
                {
                    { "exatk", ATK }
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
                e.EffectType = EffectType;
                e.Source = caster;
                e.OnEffectGained(target);
                GamingQueue?.LastRound.AddApplyEffects(target, EffectType);
            }
        }
    }
}
