using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 致命节奏 : Skill
    {
        public override long Id => (long)PassiveID.致命节奏;
        public override string Name => "致命节奏";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";

        public 致命节奏(Character? character = null) : base(SkillType.Passive, character)
        {
            Effects.Add(new 致命节奏特效(this));
        }

        public override IEnumerable<Effect> AddPassiveEffectToCharacter()
        {
            return Effects;
        }
    }

    public class 致命节奏特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"每次普通攻击造成伤害后，提升 {行动速度提升:0.##} 点行动速度，持续 {行动速度持续时间:0.##} {GameplayEquilibriumConstant.InGameTime}。" +
            $"当行动系数达到 {行动系数阈值:0.##}% 时，获得 {额外攻击力:0.##} 点额外攻击力，持续 {额外攻击力持续时间:0.##} {GameplayEquilibriumConstant.InGameTime}。" +
            $"获得额外攻击力后，致命节奏进入冷却时间 {冷却时间} {GameplayEquilibriumConstant.InGameTime}。";

        private double 行动系数阈值 => Skill.Character != null ? 25 + Skill.Character.Level * 0.6 : 25;
        private double 行动速度持续时间 => Skill.Character != null ? 18 - Skill.Character.Level * 0.1 : 18;
        private double 行动速度提升 => Skill.Character != null ? 40 + Skill.Character.Level * 1.1 : 40;
        private double 额外攻击力持续时间 => Skill.Character != null ? 12 + Skill.Character.Level * 0.15 : 12;
        private double 额外攻击力 => Skill.Character != null ? 6 + Skill.Character.Level * 1.7 : 6;
        private double 冷却时间 => Skill.Character != null ? 24 + Skill.Character.Level * 0.1 : 24;
        private bool 冷却中 => Skill.CurrentCD > 0;

        public override void AfterDamageCalculation(Character character, Character enemy, double damage, double actualDamage, bool isNormalAttack, DamageType damageType, MagicType magicType, DamageResult damageResult)
        {
            if (Skill.Character != null && Skill.Character == character && !冷却中 && isNormalAttack && (damageResult == DamageResult.Normal || damageResult == DamageResult.Critical))
            {
                WriteLine($"[ {character} ] 发动了致命节奏，提升了 {行动速度提升:0.##} 点行动速度！");
                Effect e1 = new ExSPD(Skill, new Dictionary<string, object>()
                {
                    { "exspd", 行动速度提升 }
                }, character)
                {
                    Durative = true,
                    Duration = 行动速度持续时间
                };
                character.Effects.Add(e1);
                e1.OnEffectGained(character);
                e1.IsDebuff = false;
                RecordCharacterApplyEffects(character, EffectType.Haste);

                // 检查是否达到阈值
                if (character.ActionCoefficient * 100 >= 行动系数阈值)
                {
                    Skill.Enable = false;
                    Skill.CurrentCD = 冷却时间;
                    WriteLine($"[ {character} ] 发动了致命节奏，获得了 {额外攻击力:0.##} 点额外攻击力！");
                    Effect e2 = new ExATK(Skill, new Dictionary<string, object>()
                    {
                        { "exatk", 额外攻击力 }
                    }, character)
                    {
                        Durative = true,
                        Duration = 额外攻击力持续时间
                    };
                    character.Effects.Add(e2);
                    e2.OnEffectGained(character);
                    e2.IsDebuff = false;
                    RecordCharacterApplyEffects(character, EffectType.DamageBoost);
                }
            }
        }
    }
}