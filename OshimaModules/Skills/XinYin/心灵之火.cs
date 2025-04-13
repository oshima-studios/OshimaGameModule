using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 心灵之火 : Skill
    {
        public override long Id => (long)PassiveID.心灵之火;
        public override string Name => "心灵之火";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";

        public 心灵之火(Character? character = null) : base(SkillType.Passive, character)
        {
            Effects.Add(new 心灵之火特效(this));
        }

        public override IEnumerable<Effect> AddPassiveEffectToCharacter()
        {
            return Effects;
        }
    }

    public class 心灵之火特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"普通攻击硬直时间减少 20%。每次使用普通攻击时，额外再发动一次普通攻击，伤害特效可叠加，冷却 {基础冷却时间:0.##} 时间。" +
            (冷却时间 > 0 ? $"（正在冷却：剩余 {冷却时间:0.##} 时间）" : "");

        public double 冷却时间 { get; set; } = 0;
        public double 基础冷却时间 { get; set; } = 20;
        private bool 是否是嵌套普通攻击 = false;

        public override void AfterDamageCalculation(Character character, Character enemy, double damage, bool isNormalAttack, bool isMagicDamage, MagicType magicType, DamageResult damageResult)
        {
            if (character == Skill.Character && isNormalAttack && 冷却时间 == 0 && !是否是嵌套普通攻击 && GamingQueue != null && enemy.HP > 0)
            {
                WriteLine($"[ {character} ] 发动了心灵之火！额外进行一次普通攻击！");
                冷却时间 = 基础冷却时间;
                是否是嵌套普通攻击 = true;
                character.NormalAttack.Attack(GamingQueue, character, enemy);
            }

            if (character == Skill.Character && 是否是嵌套普通攻击)
            {
                是否是嵌套普通攻击 = false;
            }
        }

        public override void OnTimeElapsed(Character character, double elapsed)
        {
            if (冷却时间 > 0)
            {
                冷却时间 -= elapsed;
                if (冷却时间 <= 0)
                {
                    冷却时间 = 0;
                }
            }
        }

        public override void AlterHardnessTimeAfterNormalAttack(Character character, ref double baseHardnessTime, ref bool isCheckProtected)
        {
            baseHardnessTime *= 0.8;
        }
    }
}
