using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 根源屏障 : Skill
    {
        public override long Id => (long)MagicID.根源屏障;
        public override string Name => "根源屏障";
        public override string Description => string.Join("", Effects.Select(e => e.Description));
        public override string DispelDescription => "被驱散性：可弱驱散";
        public override double MPCost => Level > 0 ? 200 + (75 * (Level - 1)) : 200;
        public override double CD => Level > 0 ? 120 - (3 * (Level - 1)) : 120;
        public override double CastTime => Level > 0 ? 12 - (0.5 * (Level - 1)) : 12;
        public override double HardnessTime { get; set; } = 8;
        public override bool CanSelectSelf => true;
        public override bool CanSelectEnemy => false;
        public override bool CanSelectTeammate => true;
        public override int CanSelectTargetCount => 1;

        public 根源屏障(Character? character = null) : base(SkillType.Magic, character)
        {
            Effects.Add(new 根源屏障特效(this));
        }
    }

    public class 根源屏障特效 : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"为{Skill.TargetDescription()}提供{CharacterSet.GetImmuneTypeName(ImmuneType.Magical)}，持续 {持续时间}。" + (Skill.Level > 4 ? $"为{Skill.TargetDescription()}提供{CharacterSet.GetImmuneTypeName(ImmuneType.Skilled)}，持续 1 回合。" : "");
        public override EffectType EffectType => EffectType.MagicalImmune;
        public override DispelledType DispelledType => DispelledType.Weak;

        private string 持续时间 => $"{实际持续时间} 回合";
        private int 实际持续时间
        {
            get
            {
                return Level switch
                {
                    3 => 2,
                    4 => 2,
                    5 => 2,
                    6 => 3,
                    7 => 3,
                    8 => 3,
                    _ => 1
                };
            }
        }

        public 根源屏障特效(Skill skill) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, Dictionary<string, object> others)
        {
            new 施加免疫(Skill, ImmuneType.Magical, false, 0, 实际持续时间).OnSkillCasted(caster, targets, others);
            if (Level > 4)
            {
                new 施加免疫(Skill, ImmuneType.Skilled, false, 0, 1).OnSkillCasted(caster, targets, others);
            }
        }
    }
}
