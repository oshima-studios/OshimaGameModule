using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 三重叠加 : Skill
    {
        public override long Id => (long)SuperSkillID.三重叠加;
        public override string Name => "三重叠加";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override double EPCost => 100;
        public override double CD => 35 - 2 * (Level - 1);
        public override double HardnessTime { get; set; } = 10;
        public override bool CanSelectSelf => true;
        public override bool CanSelectEnemy => false;

        public 三重叠加(Character? character = null) : base(SkillType.SuperSkill, character)
        {
            Effects.Add(new 三重叠加特效(this));
        }
    }

    public class 三重叠加特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => "三重叠加";
        public override string Description => $"使 [ 灵能反射 ] 支持普通攻击，且当前释放魔法次数归零，最大硬直消除次数提高到 {灵能反射次数} 次；在魔法命中和普通攻击命中时能够回复所回复能量值的 10 倍魔法值，持续 {技能持续次数} 次（灵能反射每消除次数达到最大时算一次）。" +
            $"（剩余：{剩余持续次数} 次）";
        
        public int 剩余持续次数 { get; set; } = 0;
        private readonly int 灵能反射次数 = 3;
        private readonly int 技能持续次数 = 2;

        public override void OnEffectGained(Character character)
        {
            IEnumerable<Effect> effects = character.Effects.Where(e => e is 灵能反射特效);
            if (effects.Any() && effects.First() is 灵能反射特效 e)
            {
                e.是否支持普攻 = true;
                e.触发硬直次数 = 3;
                e.释放次数 = 0;
            }
        }

        public override void OnEffectLost(Character character)
        {
            IEnumerable<Effect> effects = character.Effects.Where(e => e is 灵能反射特效);
            if (effects.Any() && effects.First() is 灵能反射特效 e)
            {
                e.是否支持普攻 = false;
                e.触发硬直次数 = 2;
            }
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, Dictionary<string, object> others)
        {
            剩余持续次数 = 技能持续次数;
            if (!caster.Effects.Contains(this))
            {
                caster.Effects.Add(this);
                OnEffectGained(caster);
            }
        }
    }
}
