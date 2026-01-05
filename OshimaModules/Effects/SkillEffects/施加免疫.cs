using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.PassiveEffects;
using Oshima.FunGame.OshimaModules.Skills;

namespace Oshima.FunGame.OshimaModules.Effects.SkillEffects
{
    public class 施加免疫 : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"对{Skill.TargetDescription()}施加{CharacterSet.GetImmuneTypeName(ImmuneType)}，持续 {持续时间}。";
        public override DispelledType DispelledType => _dispelledType;

        private ImmuneType ImmuneType { get; set; } = ImmuneType.None;
        private string 持续时间 => _durative && _duration > 0 ? 实际持续时间 + $" {GameplayEquilibriumConstant.InGameTime}" : (!_durative && _durationTurn > 0 ? 实际持续时间 + " 回合" : $"0 {GameplayEquilibriumConstant.InGameTime}");
        private double 实际持续时间 => _durative && _duration > 0 ? _duration + _levelGrowth * (Level - 1) : (!_durative && _durationTurn > 0 ? _durationTurn + _levelGrowth * (Level - 1) : 0);
        private DispelledType _dispelledType = DispelledType.Weak;
        private readonly bool _durative;
        private readonly double _duration;
        private readonly int _durationTurn;
        private readonly double _levelGrowth;

        public 施加免疫(Skill skill, ImmuneType type, bool durative = false, double duration = 0, int durationTurn = 1, double levelGrowth = 0) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            ImmuneType = type;
            _durative = durative;
            _duration = duration;
            _durationTurn = durationTurn;
            _levelGrowth = levelGrowth;
            _dispelledType = type switch
            {
                ImmuneType.All => DispelledType.Strong,
                ImmuneType.Special => DispelledType.Special,
                _ => DispelledType.Weak
            };
        }

        public override async Task OnSkillCasted(Character caster, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            foreach (Character target in targets)
            {
                if (target.HP <= 0) continue;
                WriteLine($"[ {caster} ] 获得了{CharacterSet.GetImmuneTypeName(ImmuneType)}！持续 {持续时间}！");
                switch (ImmuneType)
                {
                    case ImmuneType.Physical:
                        {
                            EffectType = EffectType.PhysicalImmune;
                            物理免疫 e = new(Skill, caster, _durative, _duration + _levelGrowth * (Level - 1), Convert.ToInt32(_durationTurn + _levelGrowth * (Level - 1)));
                            _dispelledType = DispelledType.Weak;
                            target.Effects.Add(e);
                            e.OnEffectGained(target);
                            break;
                        }
                    case ImmuneType.Magical:
                        {
                            EffectType = EffectType.MagicalImmune;
                            魔法免疫 e = new(Skill, caster, _durative, _duration + _levelGrowth * (Level - 1), Convert.ToInt32(_durationTurn + _levelGrowth * (Level - 1)));
                            _dispelledType = DispelledType.Weak;
                            target.Effects.Add(e);
                            e.OnEffectGained(target);
                            break;
                        }
                    case ImmuneType.Skilled:
                        {
                            EffectType = EffectType.SkilledImmune;
                            技能免疫 e = new(Skill, caster, _durative, _duration + _levelGrowth * (Level - 1), Convert.ToInt32(_durationTurn + _levelGrowth * (Level - 1)));
                            _dispelledType = DispelledType.Weak;
                            target.Effects.Add(e);
                            e.OnEffectGained(target);
                            break;
                        }
                    case ImmuneType.All:
                        {
                            EffectType = EffectType.AllImmune;
                            完全免疫 e = new(Skill, caster, _durative, _duration + _levelGrowth * (Level - 1), Convert.ToInt32(_durationTurn + _levelGrowth * (Level - 1)));
                            _dispelledType = DispelledType.Strong;
                            target.Effects.Add(e);
                            e.OnEffectGained(target);
                            break;
                        }
                }
                GamingQueue?.LastRound.AddApplyEffects(target, EffectType);
            }
        }
    }
}
