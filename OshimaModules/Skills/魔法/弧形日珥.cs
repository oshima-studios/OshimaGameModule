﻿using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 弧形日珥 : Skill
    {
        public override long Id => (long)MagicID.弧形日珥;
        public override string Name => "弧形日珥";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override double MPCost => Level > 0 ? 55 + (55 * (Level - 1)) : 55;
        public override double CD => 35;
        public override double CastTime => 8;
        public override double HardnessTime { get; set; } = 6;
        public override int CanSelectTargetCount => 3;

        public 弧形日珥(Character? character = null) : base(SkillType.Magic, character)
        {
            Effects.Add(new 基于属性的伤害(this, PrimaryAttribute.AGI, 45, 25, 0.2, 0.12));
        }
    }
}
