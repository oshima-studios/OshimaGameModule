﻿using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 火之矢 : Skill
    {
        public override long Id => (long)MagicID.火之矢;
        public override string Name => "火之矢";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override double MPCost => Level > 0 ? 30 + (40 * (Level - 1)) : 30;
        public override double CD => 20;
        public override double CastTime => 6;
        public override double HardnessTime { get; set; } = 3;

        public 火之矢(Character? character = null) : base(SkillType.Magic, character)
        {
            Effects.Add(new 基于核心属性的伤害(this, 45, 65, 0.45, 0.35));
        }
    }
}
