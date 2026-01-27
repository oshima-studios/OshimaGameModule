using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model;
using Oshima.FunGame.OshimaModules.Effects.PassiveEffects;
using Oshima.FunGame.OshimaModules.Skills;

namespace Oshima.FunGame.OshimaModules.Effects.SkillEffects
{
    public class 施加概率增益 : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description
        {
            get
            {
                SetDescription();
                return $"{概率文本}对{Skill.TargetDescription()}施加{GetEffectTypeName(_effectType)} {持续时间}。{(_description != "" ? _description : "")}";
            }
        }
        public override EffectType EffectType => _effectType;
        public override DispelledType DispelledType => _dispelledType;
        public override bool ExemptDuration => true;

        private string 概率文本 => ActualProbability == 1 ? "" : $"{ActualProbability * 100:0.##}% 概率";
        private double ActualProbability => Calculation.PercentageCheck(Level > 0 ? (_probability + _probabilityLevelGrowth * (Level - 1) * MagicEfficacy) : _probability);
        private string 持续时间 => _durative && _duration > 0 ? $"{实际持续时间:0.##}" + $" {GameplayEquilibriumConstant.InGameTime}" : (!_durative && _durationTurn > 0 ? 实际持续时间 + " 回合" : $"0 {GameplayEquilibriumConstant.InGameTime}");
        private double 实际持续时间 => _durative && _duration > 0 ? (_duration + _levelGrowth * (Level - 1) * MagicEfficacy) : (!_durative && _durationTurn > 0 ? ((int)Math.Round(_durationTurn + _levelGrowth * (Level - 1) * MagicEfficacy, 0, MidpointRounding.ToPositiveInfinity)) : 0);
        private readonly EffectType _effectType;
        private readonly bool _durative;
        private readonly double _duration;
        private readonly int _durationTurn;
        private readonly double _levelGrowth;
        private readonly double _probability;
        private readonly double _probabilityLevelGrowth;
        private readonly object[] _args;
        private DispelledType _dispelledType = DispelledType.Weak;
        private string _description = "";

        public 施加概率增益(Skill skill, EffectType effectType, bool durative = false, double duration = 0, int durationTurn = 1, double levelGrowth = 0, double probability = 0, double probabilityLevelGrowth = 0, params object[] args) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            _effectType = effectType;
            _durative = durative;
            _duration = duration;
            _durationTurn = durationTurn;
            _levelGrowth = levelGrowth;
            _probability = probability;
            _probabilityLevelGrowth = probabilityLevelGrowth;
            _args = args;
            SetDescription();
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            foreach (Character target in targets)
            {
                if (target.HP <= 0 || Random.Shared.NextDouble() > ActualProbability) continue;
                Effect? e = null;
                double duration = _duration + _levelGrowth * (Level - 1);
                int durationTurn = Convert.ToInt32(_durationTurn + _levelGrowth * (Level - 1));
                string tip = "";
                switch (_effectType)
                {
                    case EffectType.HealOverTime:
                        bool isPercentage = false;
                        double durationHeal = 0;
                        double durationHealPercent = 0;
                        double durationHealLevelGrowth = 0;
                        if (_args.Length > 0 && _args[0] is bool _)
                        {
                            isPercentage = (bool)_args[0];
                        }
                        if (_args.Length > 1 && _args[1] is double _)
                        {
                            durationHeal = (double)_args[1];
                        }
                        if (_args.Length > 2 && _args[2] is double _)
                        {
                            durationHealPercent = (double)_args[2];
                        }
                        if (_args.Length > 3 && _args[3] is double _)
                        {
                            durationHealLevelGrowth = (double)_args[3];
                        }
                        if (isPercentage && durationHealPercent > 0 || !isPercentage && durationHeal > 0)
                        {
                            if (Level > 0)
                            {
                                durationHeal += durationHealLevelGrowth * (Level - 1);
                                durationHealPercent += durationHealLevelGrowth * (Level - 1);
                            }
                            string healString = $"每{GameplayEquilibriumConstant.InGameTime}回复 {(isPercentage ? $"{durationHealPercent * 100:0.##}% 当前生命值" : durationHeal.ToString("0.##"))} 点生命值";
                            tip = $"[ {caster} ] 对 [ {target} ] 施加了{GetEffectTypeName(_effectType)}！ [ {target} ] 每{GameplayEquilibriumConstant.InGameTime}{healString}！持续时间：{持续时间}！";
                            e = new 持续回复(Skill, target, caster, _durative, duration, durationTurn, isPercentage, durationHeal, durationHealPercent)
                            {
                                EffectType = _effectType
                            };
                        }
                        break;
                    default:
                        break;
                }
                if (e != null && !CheckExemption(caster, target, e))
                {
                    WriteLine(tip);
                    target.Effects.Add(e);
                    e.OnEffectGained(target);
                    GamingQueue?.LastRound.AddApplyEffects(target, e.EffectType);
                }
            }
        }

        private void SetDescription()
        {
            switch (_effectType)
            {
                case EffectType.HealOverTime:
                    _dispelledType = DispelledType.Weak;
                    bool isPercentage = false;
                    double durationHeal = 0;
                    double durationHealPercent = 0;
                    double durationHealLevelGrowth = 0;
                    if (_args.Length > 0 && _args[0] is bool _)
                    {
                        isPercentage = (bool)_args[0];
                    }
                    if (_args.Length > 1 && _args[1] is double _)
                    {
                        durationHeal = (double)_args[1];
                    }
                    if (_args.Length > 2 && _args[2] is double _)
                    {
                        durationHealPercent = (double)_args[2];
                    }
                    if (_args.Length > 3 && _args[3] is double _)
                    {
                        durationHealLevelGrowth = (double)_args[3];
                    }
                    if (isPercentage && durationHealPercent > 0 || !isPercentage && durationHeal > 0)
                    {
                        if (Level > 0) durationHeal += durationHealLevelGrowth * (Level - 1);
                        if (Level > 0) durationHealPercent += durationHealLevelGrowth * (Level - 1);
                        string healString = $"每{GameplayEquilibriumConstant.InGameTime}回复 {(isPercentage ? $"{durationHealPercent * 100:0.##}% 当前生命值" : durationHeal.ToString("0.##"))} 点生命值";
                        _description = $"{GetEffectTypeName(_effectType)}：每{GameplayEquilibriumConstant.InGameTime}{healString}！持续时间：{持续时间}！";
                    }
                    break;
                default:
                    break;
            }
        }

        private static string GetEffectTypeName(EffectType type)
        {
            return type switch
            {
                _ => SkillSet.GetEffectTypeName(type)
            };
        }
    }
}
