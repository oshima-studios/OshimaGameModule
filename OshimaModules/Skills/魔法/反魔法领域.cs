using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 反魔法领域 : Skill
    {
        public override long Id => (long)MagicID.反魔法领域;
        public override string Name => "反魔法领域";
        public override string Description => Effects.Count > 0 ? string.Join("\r\n", Effects.Select(e => e.Description)) : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";
        public override double MPCost
        {
            get
            {
                return Level switch
                {
                    8 => 魔法消耗基础 + 7 * 魔法消耗等级成长,
                    7 => 魔法消耗基础 + 5 * 魔法消耗等级成长,
                    6 => 魔法消耗基础 + 5 * 魔法消耗等级成长,
                    5 => 魔法消耗基础 + 4 * 魔法消耗等级成长,
                    4 => 魔法消耗基础 + 3 * 魔法消耗等级成长,
                    3 => 魔法消耗基础 + 2 * 魔法消耗等级成长,
                    _ => 魔法消耗基础
                };
            }
        }
        public override double CD => Level > 0 ? 90 - (2 * (Level - 1)) : 75;
        public override double CastTime
        {
            get
            {
                return Level switch
                {
                    8 => 20,
                    7 => 18,
                    6 => 14,
                    5 => 16,
                    4 => 14,
                    3 => 12,
                    2 => 8,
                    _ => 10
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
                    8 => 6,
                    7 => 5,
                    6 => 4,
                    5 => 4,
                    4 => 3,
                    3 => 2,
                    _ => 1
                };
            }
        }
        private double 魔法消耗基础 { get; set; } = 85;
        private double 魔法消耗等级成长 { get; set; } = 80;

        public 反魔法领域(Character? character = null) : base(SkillType.Magic, character)
        {
            Effects.Add(new 造成封技(this, true, 15, 0, 1));
        }
    }
}
