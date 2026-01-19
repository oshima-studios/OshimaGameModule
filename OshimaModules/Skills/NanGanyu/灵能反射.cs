using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 灵能反射 : Skill
    {
        public override long Id => (long)PassiveID.灵能反射;
        public override string Name => "灵能反射";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";

        public 灵能反射(Character? character = null) : base(SkillType.Passive, character)
        {
            Effects.Add(new 灵能反射特效(this));
        }

        public override IEnumerable<Effect> AddPassiveEffectToCharacter()
        {
            return Effects;
        }
    }

    public class 灵能反射特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"每释放 {触发硬直次数:0.##} 次魔法才会触发硬直时间，且魔法伤害命中时基于 8% 智力 [ {获得额外能量值:0.##} ] 获得额外能量值，并减少所有技能 2 {GameplayEquilibriumConstant.InGameTime}冷却时间。";

        public bool 是否支持普攻 { get; set; } = false;
        public int 触发硬直次数 { get; set; } = 2;
        public int 释放次数 { get; set; } = 0;
        public double 获得额外能量值 => 0.08 * Skill.Character?.INT ?? 0;

        public override void AfterDamageCalculation(Character character, Character enemy, double damage, double actualDamage, bool isNormalAttack, DamageType damageType, MagicType magicType, DamageResult damageResult)
        {
            if (character == Skill.Character && (是否支持普攻 && isNormalAttack || damageType == DamageType.Magical) && (damageResult == DamageResult.Normal || damageResult == DamageResult.Critical) && character.EP < 200)
            {
                double 实际获得能量值 = 获得额外能量值;
                character.EP += 实际获得能量值;
                foreach (Skill scd in character.Skills)
                {
                    scd.CurrentCD -= 2;
                    if (scd.CurrentCD < 0)
                    {
                        scd.CurrentCD = 0;
                        scd.Enable = true;
                    }
                }
                WriteLine($"[ {character} ] 发动了灵能反射！额外获得了 {实际获得能量值:0.##} 能量，并消除了 2 {GameplayEquilibriumConstant.InGameTime}冷却时间！");
                IEnumerable<Effect> effects = character.Effects.Where(e => e is 三相灵枢特效);
                if (effects.Any() && effects.First() is 三相灵枢特效 e && e.Skill == Skill)
                {
                    double 获得的魔法值 = 实际获得能量值 * 3;
                    character.MP += 获得的魔法值;
                    WriteLine($"[ {character} ] 发动了三相灵枢！回复了 {获得的魔法值:0.##} 魔法值！");
                }
            }
        }

        public override void AlterHardnessTimeAfterNormalAttack(Character character, ref double baseHardnessTime, ref bool isCheckProtected)
        {
            if (是否支持普攻)
            {
                AlterHardnessTime(character, ref baseHardnessTime, ref isCheckProtected);
            }
        }

        public override void AlterHardnessTimeAfterCastSkill(Character character, Skill skill, ref double baseHardnessTime, ref bool isCheckProtected)
        {
            if (skill.SkillType == SkillType.Magic)
            {
                AlterHardnessTime(character, ref baseHardnessTime, ref isCheckProtected);
            }
        }

        public void AlterHardnessTime(Character character, ref double baseHardnessTime, ref bool isCheckProtected)
        {
            释放次数++;
            if (释放次数 < 触发硬直次数)
            {
                baseHardnessTime = 0;
                isCheckProtected = false;
                WriteLine($"[ {character} ] 发动了灵能反射，消除了硬直时间！！");
                if (GamingQueue != null && GamingQueue.CharacterDecisionPoints.TryGetValue(character, out DecisionPoints? dp) && dp != null)
                {
                    dp.ActionsTaken = 1;
                    dp.ActionsHardnessTime.Clear();
                }
            }
            else
            {
                释放次数 = 0;
                IEnumerable<Effect> effects = character.Effects.Where(e => e is 三相灵枢特效);
                if (effects.Any() && effects.First() is 三相灵枢特效 e && e.Skill == Skill)
                {
                    baseHardnessTime = 0;
                    isCheckProtected = false;
                    WriteLine($"[ {character} ] 发动了灵能反射，消除了硬直时间！！");
                    if (GamingQueue != null && GamingQueue.CharacterDecisionPoints.TryGetValue(character, out DecisionPoints? dp) && dp != null)
                    {
                        dp.ActionsTaken = 1;
                        dp.ActionsHardnessTime.Clear();
                    }
                    e.剩余持续次数--;
                    if (e.剩余持续次数 == 0)
                    {
                        character.Effects.Remove(e);
                        e.OnEffectLost(character);
                    }
                }
            }
        }
    }
}
