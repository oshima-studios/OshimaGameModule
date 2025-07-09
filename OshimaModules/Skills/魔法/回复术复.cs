﻿using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 回复术复 : Skill
    {
        public override long Id => (long)MagicID.回复术复;
        public override string Name => "回复术·复";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override double MPCost => Level > 0 ? 95 + (105 * (Level - 1)) : 95;
        public override double CD => 100;
        public override double CastTime => 6;
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
                    5 => 4,
                    6 => 4,
                    7 => 5,
                    8 => 5,
                    _ => 2
                };
            }
        }

        public 回复术复(Character? character = null) : base(SkillType.Magic, character)
        {
            SelectTargetPredicates.Add(c => c.HP > 0 && c.HP < c.MaxHP);
            Effects.Add(new 百分比回复生命值(this, 0.24, 0.03));
        }
    }
}
