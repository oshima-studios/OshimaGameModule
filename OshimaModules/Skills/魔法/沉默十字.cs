using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 沉默十字 : Skill
    {
        public override long Id => (long)MagicID.沉默十字;
        public override string Name => "沉默十字";
        public override string Description => Effects.Count > 0 ? string.Join("\r\n", Effects.Select(e => e.Description)) : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";
        public override double MPCost
        {
            get
            {
                return Level switch
                {
                    8 => 魔法消耗基础 + 5 * 魔法消耗等级成长,
                    7 => 魔法消耗基础 + 4 * 魔法消耗等级成长,
                    6 => 魔法消耗基础 + 4 * 魔法消耗等级成长,
                    5 => 魔法消耗基础 + 3 * 魔法消耗等级成长,
                    4 => 魔法消耗基础 + 3 * 魔法消耗等级成长,
                    3 => 魔法消耗基础 + 2 * 魔法消耗等级成长,
                    _ => 魔法消耗基础
                };
            }
        }
        public override double CD => Level > 0 ? 85 - (3 * (Level - 1)) : 85;
        public override double CastTime
        {
            get
            {
                return Level switch
                {
                    8 => 8,
                    7 => 8,
                    6 => 9,
                    5 => 10,
                    4 => 11,
                    3 => 12,
                    2 => 13,
                    _ => 14
                };
            }
        }
        public override double HardnessTime { get; set; } = 10;
        public override int CanSelectTargetCount
        {
            get
            {
                return Level switch
                {
                    8 => 3,
                    7 => 2,
                    6 => 2,
                    5 => 2,
                    _ => 1
                };
            }
        }
        private double 魔法消耗基础 { get; set; } = 65;
        private double 魔法消耗等级成长 { get; set; } = 65;

        public 沉默十字(Character? character = null) : base(SkillType.Magic, character)
        {
            Effects.Add(new 造成封技(this, false, 0, 2));
        }
    }
}
