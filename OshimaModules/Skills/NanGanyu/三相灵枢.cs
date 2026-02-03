using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 三相灵枢 : Skill
    {
        public override long Id => (long)SuperSkillID.三相灵枢;
        public override string Name => "三相灵枢";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";
        public override double EPCost => 100;
        public override double CD => 80 - 4 * (Level - 1);
        public override double HardnessTime { get; set; } = 0;
        public override bool CanSelectSelf => true;
        public override bool CanSelectEnemy => false;

        public 三相灵枢(Character? character = null) : base(SkillType.SuperSkill, character)
        {
            Effects.Add(new 三相灵枢特效(this));
        }
    }

    public class 三相灵枢特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => "三相灵枢";
        public override string Description => $"{Skill.SkillOwner()}操纵三相之力，使 [ 灵能反射 ] 支持普通攻击，且当前释放魔法次数归零，最大硬直消除次数提高到 {灵能反射次数} 次；在魔法命中和普通攻击命中时能够回复所回复能量值的 {魔法值倍数:0.#} 倍魔法值，持续 {技能持续次数} 次（灵能反射每消除次数达到最大时算一次）。" +
            $"（剩余：{剩余持续次数} 次）";
        public override DispelledType DispelledType => DispelledType.CannotBeDispelled;

        public int 剩余持续次数 { get; set; } = 0;
        private readonly int 灵能反射次数 = 3;
        private double 魔法值倍数
        {
            get
            {
                return Skill.Level * 0.5;
            }
        }
        private int 技能持续次数
        {
            get
            {
                return Skill.Level > 3 ? 2 : 1;
            }
        }

        public override void OnEffectGained(Character character)
        {
            IEnumerable<Effect> effects = character.Effects.Where(e => e is 灵能反射特效);
            if (effects.Any() && effects.First() is 灵能反射特效 e && e.Skill.Character == Skill.Character)
            {
                e.是否支持普攻 = true;
                e.触发硬直次数 = 3;
                e.释放次数 = 0;
            }
        }

        public override void OnEffectLost(Character character)
        {
            IEnumerable<Effect> effects = character.Effects.Where(e => e is 灵能反射特效);
            if (effects.Any() && effects.First() is 灵能反射特效 e && e.Skill.Character == Skill.Character)
            {
                e.是否支持普攻 = false;
                e.触发硬直次数 = 2;
            }
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            剩余持续次数 = 技能持续次数;
            if (!caster.Effects.Contains(this))
            {
                caster.Effects.Add(this);
                OnEffectGained(caster);
            }
            GamingQueue?.LastRound.AddApplyEffects(caster, EffectType.MPRegen, EffectType.Haste);
        }
    }
}
