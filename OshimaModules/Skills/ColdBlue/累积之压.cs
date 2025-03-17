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

        public 累积之压(Character? character = null) : base(SkillType.Passive, character)
        {
            Effects.Add(new 累积之压特效(this));
        }

        public override IEnumerable<Effect> AddInactiveEffectToCharacter()
        {
            return Effects;
        }
    }

    public class 累积之压特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"每次造成伤害都可以叠一层标记，累计 4 层时回收该角色所有标记并造成眩晕 1 回合，额外对该角色造成 {系数 * 100:0.##}% 最大生命值的物理伤害。";

        private readonly double 系数 = 0.12;
        private bool 是否是嵌套伤害 = false;

        public override void AfterDamageCalculation(Character character, Character enemy, double damage, bool isNormalAttack, bool isMagicDamage, MagicType magicType, DamageResult damageResult)
        {
            if (character == Skill.Character && damageResult != DamageResult.Evaded && !是否是嵌套伤害 && enemy.HP > 0)
            {
                // 叠标记
                IEnumerable<Effect> effects = enemy.Effects.Where(e => e is 累积之压标记);
                if (effects.Any() && effects.First() is 累积之压标记 e)
                {
                    e.MarkLevel++;
                    IEnumerable<Effect> effects2 = character.Effects.Where(e => e is 嗜血本能特效);
                    if (effects2.Any() && effects2.First() is 嗜血本能特效 e2)
                    {
                        if (e.MarkLevel >= 4)
                        {
                            e2.角色有第四层.Add(enemy);
                        }
                        else
                        {
                            e2.角色有第四层.Remove(enemy);
                        }
                    }
                    if (e.MarkLevel >= 4)
                    {
                        // 移除标记
                        enemy.Effects.Remove(e);
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
                        DamageToEnemy(character, enemy, false, magicType, 额外伤害);
                    }
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
