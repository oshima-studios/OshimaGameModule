﻿using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 时间加速 : Skill
    {
        public override long Id => (long)MagicID.时间加速;
        public override string Name => "时间加速";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override double MPCost => Level > 0 ? 100 + (115 * (Level - 1)) : 100;
        public override double CD => Level > 0 ? 75 - (1 * (Level - 1)) : 75;
        public override double CastTime => Level > 0 ? 2 + (1.5 * (Level - 1)) : 2;
        public override double HardnessTime { get; set; } = 6;
        public override bool CanSelectSelf => true;
        public override bool CanSelectEnemy => false;
        public override bool CanSelectTeammate => true;
        public override int CanSelectTargetCount => 1;

        public 时间加速(Character? character = null) : base(SkillType.Magic, character)
        {
            Effects.Add(new 提升友方行动速度(this, 65, 25));
        }
    }
}
