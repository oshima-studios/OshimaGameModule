using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.PassiveEffects;

namespace Oshima.FunGame.OshimaModules.Effects.SkillEffects
{
    public class 造成气绝 : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"对目标{(Skill.CanSelectTargetCount > 1 ? $"至多 {Skill.CanSelectTargetCount} 个" : "")}敌人造成气绝 {气绝时间}。气绝期间，目标行动受限且每{GameplayEquilibriumConstant.InGameTime}{DamageString}，此生命流失效果不会导致角色死亡。";
        public override DispelledType DispelledType => DispelledType.Strong;

        private string 气绝时间 => _durative && _duration > 0 ? 实际气绝时间 + $" {GameplayEquilibriumConstant.InGameTime}" : (!_durative && _durationTurn > 0 ? 实际气绝时间 + " 回合" : $"0 {GameplayEquilibriumConstant.InGameTime}");
        private double 实际气绝时间 => _durative && _duration > 0 ? _duration + _levelGrowth * (Level - 1) : (!_durative && _durationTurn > 0 ? _durationTurn + _levelGrowth * (Level - 1) : 0);
        private readonly bool _durative;
        private readonly double _duration;
        private readonly int _durationTurn;
        private readonly double _levelGrowth;
        private readonly bool _isPercentage;
        private readonly double _durationDamage;
        private readonly double _durationDamagePercent;
        private double Damage => _isPercentage ? _durationDamagePercent : _durationDamage;
        private string DamageString => _isPercentage ? $"流失 {Damage * 100:0.##}% 当前生命值" : $"流失 {Damage:0.##} 点生命值";

        public 造成气绝(Skill skill, bool durative = false, double duration = 0, int durationTurn = 1, double levelGrowth = 0,
            bool isPercentage = true, double durationDamage = 100, double durationDamagePercent = 0.02) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            _durative = durative;
            _duration = duration;
            _durationTurn = durationTurn;
            _levelGrowth = levelGrowth;
            _isPercentage = isPercentage;
            _durationDamage = durationDamage;
            _durationDamagePercent = durationDamagePercent;
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, Dictionary<string, object> others)
        {
            foreach (Character enemy in targets)
            {
                WriteLine($"[ {caster} ] 对 [ {enemy} ] 造成了气绝！持续时间：{气绝时间}！");
                气绝 e = new(Skill, enemy, caster, _durative, _duration + _levelGrowth * (Level - 1), Convert.ToInt32(_durationTurn + _levelGrowth * (Level - 1)), _isPercentage, _durationDamage, _durationDamagePercent);
                enemy.Effects.Add(e);
                e.OnEffectGained(enemy);
                GamingQueue?.LastRound.ApplyEffects.TryAdd(enemy, [e.EffectType]);
            }
        }
    }
}
