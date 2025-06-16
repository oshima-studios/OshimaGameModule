using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.PassiveEffects;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 大地之墙 : Skill
    {
        public override long Id => (long)MagicID.大地之墙;
        public override string Name => "大地之墙";
        public override string Description => string.Join("", Effects.Select(e => e.Description));
        public override string DispelDescription => "驱散性：持续性弱驱散\r\n被驱散性：护盾不可驱散，持续性弱驱散在护盾值用尽前需强驱散，否则可弱驱散";
        public override double MPCost
        {
            get
            {
                return Level switch
                {
                    8 => 魔法消耗基础 + 6 * 魔法消耗等级成长,
                    7 => 魔法消耗基础 + 4 * 魔法消耗等级成长,
                    6 => 魔法消耗基础 + 4 * 魔法消耗等级成长,
                    5 => 魔法消耗基础 + 3 * 魔法消耗等级成长,
                    4 => 魔法消耗基础 + 3 * 魔法消耗等级成长,
                    3 => 魔法消耗基础 + 2.5 * 魔法消耗等级成长,
                    _ => 魔法消耗基础
                };
            }
        }
        public override double CD => Level > 0 ? 100 - (2 * (Level - 1)) : 100;
        public override double CastTime
        {
            get
            {
                return Level switch
                {
                    8 => 7,
                    7 => 7,
                    6 => 8,
                    5 => 8,
                    4 => 9,
                    3 => 10,
                    2 => 11,
                    _ => 12
                };
            }
        }
        public override double HardnessTime { get; set; } = 8;
        private double 魔法消耗基础 { get; set; } = 90;
        private double 魔法消耗等级成长 { get; set; } = 90;
        public override bool CanSelectSelf => true;
        public override bool CanSelectEnemy => false;
        public override bool CanSelectTeammate => true;
        public override int CanSelectTargetCount
        {
            get
            {
                return Level switch
                {
                    8 => 4,
                    7 => 3,
                    6 => 3,
                    5 => 2,
                    4 => 2,
                    3 => 2,
                    _ => 1
                };
            }
        }

        public 大地之墙(Character? character = null) : base(SkillType.Magic, character)
        {
            Effect shield = new 增加物理护盾_特效持续型(this, 120, 160, true, 15)
            {
                DispelledType = DispelledType.CannotBeDispelled
            };
            Effects.Add(shield);
            Effect dispel = new 施加持续性弱驱散(this, durative: true, duration: 12)
            {
                DispelledType = DispelledType.Strong
            };
            Effects.Add(dispel);
            Effects.Add(new 大地之墙特效(this));
        }
    }

    public class 大地之墙特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"";
        public override bool ForceHideInStatusBar => true;

        public override void OnTimeElapsed(Character character, double elapsed)
        {
            if (!character.Effects.Any(e => e is 物理护盾) && character.Effects.FirstOrDefault(e => e is 施加持续性弱驱散) is 施加持续性弱驱散 e)
            {
                e.DispelledType = DispelledType.Weak;
            }
            else
            {
                character.Effects.Remove(this);
            }
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, Dictionary<string, object> others)
        {
            foreach (Character target in targets)
            {
                if (!target.Effects.Any(e => e is 大地之墙特效))
                {
                    target.Effects.Add(new 大地之墙特效(Skill));
                }
            }
        }
    }
}
