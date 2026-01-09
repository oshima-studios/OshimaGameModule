using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 身心一境 : Skill
    {
        public override long Id => (long)SuperSkillID.身心一境;
        public override string Name => "身心一境";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";
        public override double EPCost => 100;
        public override double CD => 55 - (1 * (Level - 1));
        public override double HardnessTime { get; set; } = 9;
        public override bool CanSelectSelf => true;
        public override bool CanSelectEnemy => false;

        public 身心一境(Character? character = null) : base(SkillType.SuperSkill, character)
        {
            Effects.Add(new 身心一境特效(this));
        }
    }

    public class 身心一境特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"敏捷提高 20% [ {敏捷提升:0.##} ] 点，然后将当前力量补充到敏捷的 {平衡系数 * 100:0.##}%{(Skill.Character != null ? $" [ {Skill.Character.AGI * 平衡系数:0.##} ]" : "")}，持续 {Duration:0.##} {GameplayEquilibriumConstant.InGameTime}。";
        public override bool Durative => true;
        public override double Duration => 30;
        public override DispelledType DispelledType => DispelledType.CannotBeDispelled;

        private double 敏捷提升 => (0.2 * Skill.Character?.BaseAGI) ?? 0.2;
        private double 平衡系数 => 0.5 + 0.1 * (Skill.Level - 1);
        private double 本次提升的敏捷 = 0;
        private double 本次提升的力量 = 0;

        public override void OnEffectGained(Character character)
        {
            double pastHP = character.HP;
            double pastMaxHP = character.MaxHP;
            double pastMP = character.MP;
            double pastMaxMP = character.MaxMP;
            本次提升的敏捷 = character.BaseAGI * 0.2;
            character.ExAGI += 本次提升的敏捷;
            本次提升的力量 = 0;
            double 平衡敏捷 = character.AGI * 平衡系数;
            if (character.STR < 平衡敏捷)
            {
                本次提升的力量 = 平衡敏捷 - character.STR;
                character.ExSTR += 本次提升的力量;
            }
            character.Recovery(pastHP, pastMP, pastMaxHP, pastMaxMP);
            WriteLine($"[ {character} ] 敏捷提升了 {本次提升的敏捷:0.##} 点，力量提升了 {本次提升的力量:0.##} 点！");
        }

        public override void OnEffectLost(Character character)
        {
            double pastHP = character.HP;
            double pastMaxHP = character.MaxHP;
            double pastMP = character.MP;
            double pastMaxMP = character.MaxMP;
            character.ExAGI -= character.BaseAGI * 0.2;
            character.ExSTR -= 本次提升的力量;
            character.Recovery(pastHP, pastMP, pastMaxHP, pastMaxMP);
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            RemainDuration = Duration;
            if (!caster.Effects.Contains(this))
            {
                本次提升的敏捷 = 0;
                本次提升的力量 = 0;
                caster.Effects.Add(this);
                OnEffectGained(caster);
            }
            GamingQueue?.LastRound.AddApplyEffects(caster, EffectType.DamageBoost, EffectType.Lifesteal);
        }
    }
}
