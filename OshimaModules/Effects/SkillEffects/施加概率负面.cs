using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.PassiveEffects;
using Oshima.FunGame.OshimaModules.Skills;

namespace Oshima.FunGame.OshimaModules.Effects.SkillEffects
{
    public class 施加概率负面 : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description
        {
            get
            {
                SetDescription();
                return $"{ActualProbability * 100:0.##}% 概率对{Skill.TargetDescription()}造成{GetEffectTypeName(_effectType)} {持续时间}。{(_description != "" ? _description : "")}";
            }
        }
        public override EffectType EffectType => _effectType;
        public override DispelledType DispelledType => _dispelledType;
        public override bool ExemptDuration => true;

        private double ActualProbability => Level > 0 ? _probability + _probabilityLevelGrowth * (Level - 1) : _probability;
        private string 持续时间 => _durative && _duration > 0 ? $"{实际持续时间:0.##}" + $" {GameplayEquilibriumConstant.InGameTime}" : (!_durative && _durationTurn > 0 ? 实际持续时间 + " 回合" : $"0 {GameplayEquilibriumConstant.InGameTime}");
        private double 实际持续时间 => _durative && _duration > 0 ? _duration + _levelGrowth * (Level - 1) : (!_durative && _durationTurn > 0 ? _durationTurn + _levelGrowth * (Level - 1) : 0);
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

        public 施加概率负面(Skill skill, EffectType effectType, bool durative = false, double duration = 0, int durationTurn = 1, double levelGrowth = 0, double probability = 0, double probabilityLevelGrowth = 0, params object[] args) : base(skill)
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

        public override async Task OnSkillCasted(Character caster, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            foreach (Character target in targets)
            {
                if (target.HP <= 0 || Random.Shared.NextDouble() > ActualProbability) continue;
                Effect? e = null;
                double duration = _duration + _levelGrowth * (Level - 1);
                int durationTurn = Convert.ToInt32(_durationTurn + _levelGrowth * (Level - 1));
                switch (_effectType)
                {
                    case EffectType.Silence:
                        WriteLine($"[ {caster} ] 对 [ {target} ] 造成了封技和施法解除！持续时间：{持续时间}！");
                        e = new 封技(Skill, caster, _durative, duration, durationTurn);
                        break;
                    case EffectType.Confusion:
                        WriteLine($"[ {target} ] 陷入了混乱！！持续时间：{持续时间}！");
                        e = new 混乱(Skill, caster, _durative, duration, durationTurn);
                        break;
                    case EffectType.Taunt:
                        WriteLine($"[ {target} ] 被 [ {caster} ] 嘲讽了！持续时间：{持续时间}！");
                        e = new 愤怒(Skill, caster, target, _durative, duration, durationTurn);
                        break;
                    case EffectType.Delay:
                        double healingReductionPercent = 0.3;
                        if (_args.Length > 0 && _args[0] is double healingReduce)
                        {
                            healingReductionPercent = healingReduce;
                        }
                        WriteLine($"[ {caster} ] 对 [ {target} ] 造成了迟滞！普通攻击和技能的硬直时间、当前行动等待时间延长了 {healingReductionPercent * 100:0.##}%！持续时间：{持续时间}！");
                        e = new 迟滞(Skill, caster, _durative, duration, durationTurn, healingReductionPercent);
                        break;
                    case EffectType.Stun:
                        WriteLine($"[ {caster} ] 对 [ {target} ] 造成了眩晕！持续时间：{持续时间}！");
                        e = new 眩晕(Skill, caster, _durative, duration, durationTurn);
                        break;
                    case EffectType.Freeze:
                        WriteLine($"[ {caster} ] 对 [ {target} ] 造成了冻结！持续时间：{持续时间}！");
                        e = new 冻结(Skill, caster, _durative, duration, durationTurn);
                        break;
                    case EffectType.Petrify:
                        WriteLine($"[ {caster} ] 对 [ {target} ] 造成了石化！持续时间：{持续时间}！");
                        e = new 石化(Skill, caster, _durative, duration, durationTurn);
                        break;
                    case EffectType.Vulnerable:
                        DamageType damageType = DamageType.Magical;
                        if (_args.Length > 0 && _args[0] is DamageType dt)
                        {
                            damageType = dt;
                        }
                        double exDamagePercent = 0;
                        if (_args.Length > 1 && _args[1] is double percent)
                        {
                            exDamagePercent = percent;
                        }
                        if (exDamagePercent > 0)
                        {
                            WriteLine($"[ {caster} ] 对 [ {target} ] 造成了易伤，额外受到 {exDamagePercent * 100:0.##}% {CharacterSet.GetDamageTypeName(damageType)}！持续时间：{持续时间}！");
                            e = new 易伤(Skill, target, caster, _durative, duration, durationTurn, damageType, exDamagePercent);
                        }
                        break;
                    case EffectType.Bleed:
                        bool isPercentage = false;
                        double durationDamage = 0;
                        double durationDamagePercent = 0;
                        double durationDamageLevelGrowth = 0;
                        if (_args.Length > 0 && _args[0] is bool isPerc)
                        {
                            isPercentage = isPerc;
                        }
                        if (_args.Length > 1 && _args[1] is double durDamage)
                        {
                            durationDamage = durDamage;
                        }
                        if (_args.Length > 2 && _args[2] is double durDamagePercent)
                        {
                            durationDamagePercent = durDamagePercent;
                        }
                        if (_args.Length > 3 && _args[3] is double durDamageLevelGrowth)
                        {
                            durationDamageLevelGrowth = durDamageLevelGrowth;
                        }
                        if (isPercentage && durationDamagePercent > 0 || !isPercentage && durationDamage > 0)
                        {
                            if (Level > 0) durationDamage += durationDamageLevelGrowth * (Level - 1);
                            if (Level > 0) durationDamagePercent += durationDamageLevelGrowth * (Level - 1);
                            string damageString = isPercentage ? $"流失 {durationDamagePercent * 100:0.##}% 当前生命值" : $"流失 {durationDamage:0.##} 点生命值";
                            WriteLine($"[ {caster} ] 对 [ {target} ] 造成了气绝！ [ {target} ] 进入行动受限状态且每{GameplayEquilibriumConstant.InGameTime}{damageString}！持续时间：{持续时间}！");
                            e = new 气绝(Skill, target, caster, _durative, duration, durationTurn, isPercentage, durationDamage, durationDamagePercent);
                        }
                        break;
                    case EffectType.Cripple:
                        WriteLine($"[ {caster} ] 对 [ {target} ] 造成了战斗不能，禁止普通攻击和使用技能（魔法、战技和爆发技）！持续时间：{持续时间}！");
                        e = new 战斗不能(Skill, caster, _durative, duration, durationTurn);
                        break;
                    default:
                        break;
                }
                if (e != null)
                {
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
                case EffectType.Silence:
                    _dispelledType = DispelledType.Weak;
                    _description = "封技：不能使用技能（魔法、战技和爆发技），并解除当前施法。";
                    break;
                case EffectType.Confusion:
                    _dispelledType = DispelledType.Strong;
                    _description = "混乱：进入行动受限状态，失控并随机行动，且在进行攻击指令时，可能会选取友方角色为目标。";
                    break;
                case EffectType.Taunt:
                    _dispelledType = DispelledType.Strong;
                    _description = "愤怒：进入行动受限状态，失控并随机行动，行动回合内仅能对嘲讽者发起普通攻击。";
                    break;
                case EffectType.Delay:
                    double healingReductionPercent = 0.3;
                    if (_args.Length > 0 && _args[0] is double healingReduce)
                    {
                        healingReductionPercent = healingReduce;
                    }
                    _dispelledType = DispelledType.Weak;
                    _description = $"迟滞：普通攻击和技能的硬直时间、当前行动等待时间延长 {healingReductionPercent * 100:0.##}%。";
                    break;
                case EffectType.Stun:
                    _dispelledType = DispelledType.Strong;
                    _description = "眩晕：进入完全行动不能状态。";
                    break;
                case EffectType.Freeze:
                    _dispelledType = DispelledType.Strong;
                    _description = "冻结：进入完全行动不能状态。";
                    break;
                case EffectType.Petrify:
                    _dispelledType = DispelledType.Strong;
                    _description = "石化：进入完全行动不能状态。";
                    break;
                case EffectType.Vulnerable:
                    DamageType damageType = DamageType.Magical;
                    if (_args.Length > 0 && _args[0] is DamageType dt)
                    {
                        damageType = dt;
                    }
                    double exDamagePercent = 0;
                    if (_args.Length > 1 && _args[1] is double percent)
                    {
                        exDamagePercent = percent;
                    }
                    if (exDamagePercent > 0)
                    {
                        _dispelledType = DispelledType.Weak;
                        _description = $"易伤：额外受到 {exDamagePercent * 100:0.##}% {CharacterSet.GetDamageTypeName(damageType)}。";
                    }
                    break;
                case EffectType.Bleed:
                    _dispelledType = DispelledType.Strong;
                    bool isPercentage = false;
                    double durationDamage = 0;
                    double durationDamagePercent = 0;
                    double durationDamageLevelGrowth = 0;
                    if (_args.Length > 0 && _args[0] is bool isPerc)
                    {
                        isPercentage = isPerc;
                    }
                    if (_args.Length > 1 && _args[1] is double durDamage)
                    {
                        durationDamage = durDamage;
                    }
                    if (_args.Length > 2 && _args[2] is double durDamagePercent)
                    {
                        durationDamagePercent = durDamagePercent;
                    }
                    if (_args.Length > 3 && _args[3] is double durDamageLevelGrowth)
                    {
                        durationDamageLevelGrowth = durDamageLevelGrowth;
                    }
                    if (isPercentage && durationDamagePercent > 0 || !isPercentage && durationDamage > 0)
                    {
                        if (Level > 0) durationDamage += durationDamageLevelGrowth * (Level - 1);
                        if (Level > 0) durationDamagePercent += durationDamageLevelGrowth * (Level - 1);
                        string damageString = isPercentage ? $"流失 {durationDamagePercent * 100:0.##}% 当前生命值" : $"流失 {durationDamage:0.##} 点生命值";
                        _description = $"气绝：进入行动受限状态并每{GameplayEquilibriumConstant.InGameTime}{damageString}，此效果不会导致角色死亡。";
                    }
                    break;
                case EffectType.Cripple:
                    _dispelledType = DispelledType.Strong;
                    _description = "战斗不能：无法普通攻击和使用技能（魔法、战技和爆发技）。";
                    break;
                default:
                    break;
            }
        }

        private static string GetEffectTypeName(EffectType type)
        {
            return type switch
            {
                EffectType.Taunt => "愤怒",
                EffectType.Silence => "封技",
                EffectType.Bleed => "气绝",
                EffectType.Cripple => "战斗不能",
                _ => SkillSet.GetEffectTypeName(type)
            };
        }
    }
}
