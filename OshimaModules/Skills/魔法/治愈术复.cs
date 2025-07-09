﻿using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 治愈术复 : Skill
    {
        public override long Id => (long)MagicID.治愈术复;
        public override string Name => "治愈术·复";
        public override string Description => string.Join("", Effects.Select(e => e.Description));
        public override double MPCost => Level > 0 ? 85 + (90 * (Level - 1)) : 85;
        public override double CD => Level > 0 ? 92 - (1 * (Level - 1)) : 92;
        public override double CastTime => Level > 0 ? 6 + (0.25 * (Level - 1)) : 6;
        public override double HardnessTime { get; set; } = 7;
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

        public 治愈术复(Character? character = null) : base(SkillType.Magic, character)
        {
            SelectTargetPredicates.Add(c => c.HP > 0 && c.HP < c.MaxHP);
            Effects.Add(new 弱驱散特效(this));
            Effects.Add(new 纯数值回复生命(this, 420, 340));
        }
    }
}
