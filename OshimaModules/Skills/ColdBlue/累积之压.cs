using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.PassiveEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 累积之压 : Skill
    {
        public override long Id => (long)PassiveID.累积之压;
        public override string Name => "累积之压";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";

        public 累积之压(Character? character = null) : base(SkillType.Passive, character)
        {
            Effects.Add(new 累积之压特效(this));
        }

        public override IEnumerable<Effect> AddPassiveEffectToCharacter()
        {
            return Effects;
        }
    }

    public class 累积之压特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"造成伤害时会标记目标，攻击具有标记的敌人将对其造成眩晕 1 回合，并回收标记，额外对该角色造成 {系数 * 100:0.##}% 最大生命值的物理伤害。";
        public override string DispelDescription => "被驱散性：眩晕需强驱散，标记可弱驱散";

        public double 系数 { get; set; } = 0.12;
        private bool 是否是嵌套伤害 = false;

        public override bool BeforeCriticalCheck(Character actor, Character enemy, ref double throwingBonus)
        {
            return !是否是嵌套伤害;
        }

        public override void AfterDamageCalculation(Character character, Character enemy, double damage, double actualDamage, bool isNormalAttack, DamageType damageType, MagicType magicType, DamageResult damageResult)
        {
            if (character == Skill.Character && actualDamage > 0 && (damageResult == DamageResult.Normal || damageResult == DamageResult.Critical) && !是否是嵌套伤害 && enemy.HP > 0)
            {
                // 叠标记
                IEnumerable<Effect> effects = enemy.Effects.Where(e => e is 累积之压标记 && e.Source == character);
                if (effects.FirstOrDefault() is 累积之压标记 e)
                {
                    IEnumerable<Effect> effects2 = character.Effects.Where(e => e is 嗜血本能特效);
                    if (effects2.FirstOrDefault() is 嗜血本能特效 e2)
                    {
                        // 移除标记
                        enemy.Effects.Remove(e);
                    }
                    double 额外伤害 = enemy.MaxHP * 系数;
                    WriteLine($"[ {character} ] 发动了累积之压！将对 [ {enemy} ] 造成眩晕和额外伤害！");
                    // 眩晕
                    IEnumerable<Effect> effects3 = enemy.Effects.Where(e => e is 眩晕 && e.Skill == Skill);
                    if (effects3.Any())
                    {
                        effects3.First().RemainDurationTurn++;
                    }
                    else
                    {
                        眩晕 e3 = new(Skill, character, false, 0, 1);
                        enemy.Effects.Add(e3);
                        e3.OnEffectGained(enemy);
                    }
                    是否是嵌套伤害 = true;
                    DamageToEnemy(character, enemy, DamageType.Physical, magicType, 额外伤害);
                }
                else
                {
                    enemy.Effects.Add(new 累积之压标记(Skill, character));
                }
            }

            if (character == Skill.Character && 是否是嵌套伤害)
            {
                是否是嵌套伤害 = false;
            }
        }
    }
}
