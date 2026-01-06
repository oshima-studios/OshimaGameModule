using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 盖亚之盾 : Skill
    {
        public override long Id => (long)MagicID.盖亚之盾;
        public override string Name => "盖亚之盾";
        public override string Description => string.Join("", Effects.Select(e => e.Description));
        public override double MPCost => Level > 0 ? 魔法消耗基础 + (魔法消耗等级成长 * (Level - 1)) : 魔法消耗基础;
        public override double CD => Level > 0 ? 50 - (3 * (Level - 1)) : 50;
        public override double CastTime => 12;
        public override double HardnessTime { get; set; } = 4;
        private double 魔法消耗基础 { get; set; } = 100;
        private double 魔法消耗等级成长 { get; set; } = 100;
        public override bool CanSelectSelf => true;
        public override bool CanSelectEnemy => false;
        public override bool CanSelectTeammate => true;
        public override bool SelectAllTeammates => true;

        public 盖亚之盾(Character? character = null) : base(SkillType.Magic, character)
        {
            Effects.Add(new 增加混合护盾值(this, 160, 120));
        }
    }
}
