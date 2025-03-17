using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 弱者猎手 : Skill
    {
        public override long Id => (long)PassiveID.弱者猎手;
        public override string Name => "弱者猎手";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";

        public 弱者猎手(Character? character = null) : base(SkillType.Passive, character)
        {
            Effects.Add(new 弱者猎手特效(this));
        }

        public override IEnumerable<Effect> AddInactiveEffectToCharacter()
        {
            return Effects;
        }
    }

    public class 弱者猎手特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"优先攻击血量更低的角色，对生命值百分比低于自己的角色造成 150% 伤害。";

        public override double AlterExpectedDamageBeforeCalculation(Character character, Character enemy, double damage, bool isNormalAttack, bool isMagicDamage, MagicType magicType, Dictionary<Effect, double> totalDamageBonus)
        {
            if (character == Skill.Character && (enemy.HP / enemy.MaxHP) <= (character.HP / character.MaxHP))
            {
                double 额外伤害 = damage * 0.5;
                return 额外伤害;
            }
            return 0;
        }

        public override bool AlterEnemyListBeforeAction(Character character, List<Character> enemys, List<Character> teammates, List<Skill> skills, Dictionary<Character, int> continuousKilling, Dictionary<Character, int> earnedMoney)
        {
            IEnumerable<Character> list = [.. enemys.OrderBy(e => e.HP / e.MaxHP)];
            if (list.Any())
            {
                enemys.Clear();
                enemys.Add(list.First());
                WriteLine($"[ {character} ] 发动了弱者猎手！[ {list.First()} ] 被盯上了！");
            }
            return true;
        }
    }
}
