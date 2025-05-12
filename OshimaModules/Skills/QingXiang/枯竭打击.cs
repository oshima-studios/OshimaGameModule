using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 枯竭打击 : Skill
    {
        public override long Id => (long)PassiveID.枯竭打击;
        public override string Name => "枯竭打击";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";

        public 枯竭打击(Character? character = null) : base(SkillType.Passive, character)
        {
            Effects.Add(new 枯竭打击特效(this));
        }

        public override IEnumerable<Effect> AddPassiveEffectToCharacter()
        {
            return Effects;
        }
    }

    public class 枯竭打击特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"每次造成伤害都会随机减少对方 [ 7~15 ] 点能量值，对能量值低于一半的角色额外造成 30% 伤害。对于枯竭打击而言，能量值大于100且小于150时，视为低于一半。";

        private bool 是否是嵌套伤害 = false;

        public override void AfterDamageCalculation(Character character, Character enemy, double damage, double actualDamage, bool isNormalAttack, bool isMagicDamage, MagicType magicType, DamageResult damageResult)
        {
            if (character == Skill.Character && (damageResult == DamageResult.Normal || damageResult == DamageResult.Critical) && !是否是嵌套伤害 && enemy.HP > 0)
            {
                // 减少能量
                double EP = Random.Shared.Next(7, 15);
                enemy.EP -= EP;
                WriteLine($"[ {character} ] 发动了枯竭打击！[ {enemy} ] 的能量值被减少了 {EP:0.##} 点！现有能量：{enemy.EP:0.##}。");
                // 额外伤害
                if (enemy.EP >= 0 && enemy.EP < 50 || enemy.EP >= 100 && enemy.EP < 150)
                {
                    double 额外伤害 = damage * 0.3;
                    WriteLine($"[ {character} ] 发动了枯竭打击！将造成额外伤害！");
                    是否是嵌套伤害 = true;
                    DamageToEnemy(character, enemy, isMagicDamage, magicType, 额外伤害);
                }
            }

            if (character == Skill.Character && 是否是嵌套伤害)
            {
                是否是嵌套伤害 = false;
            }
        }
    }
}
