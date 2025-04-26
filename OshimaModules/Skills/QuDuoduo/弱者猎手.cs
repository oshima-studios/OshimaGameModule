﻿using Milimoe.FunGame.Core.Entity;
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

        public override IEnumerable<Effect> AddPassiveEffectToCharacter()
        {
            return Effects;
        }
    }

    public class 弱者猎手特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"在 AI 控制下，任何目标都将则优先选择血量更低的角色。行动开始时，弱者猎手会盯上一名角色，然后标记所有生命值百分比低于自己的角色。在此回合内攻击被盯上或者被标记的角色，将造成 150% 伤害。";

        public HashSet<Character> 猎手标记 { get; set; } = [];

        public override double AlterExpectedDamageBeforeCalculation(Character character, Character enemy, double damage, bool isNormalAttack, bool isMagicDamage, MagicType magicType, Dictionary<Effect, double> totalDamageBonus)
        {
            if (character == Skill.Character && 猎手标记.Contains(enemy))
            {
                double 额外伤害 = damage * 0.5;
                return 额外伤害;
            }
            return 0;
        }

        public override void AlterSelectListBeforeAction(Character character, List<Character> enemys, List<Character> teammates, List<Skill> skills, Dictionary<Character, int> continuousKilling, Dictionary<Character, int> earnedMoney)
        {
            猎手标记.Clear();
            IEnumerable<Character> list = [.. enemys.OrderBy(e => e.HP / e.MaxHP)];
            if (list.Any())
            {
                Character first = list.First();
                if (IsCharacterInAIControlling(character))
                {
                    enemys.Clear();
                    enemys.Add(first);
                }
                猎手标记.Add(first);
                WriteLine($"[ {character} ] 发动了弱者猎手！[ {first} ] 被盯上了！");
                AddHalfOfMe(enemys);
                if (猎手标记.Count > 0) WriteLine($"[ {character} ] 的弱者猎手标记了以下角色：[ {string.Join(" ] / [ ", 猎手标记)} ] ！");
            }
        }

        private void AddHalfOfMe(params IEnumerable<Character> enemys)
        {
            foreach (Character enemy in enemys)
            {
                if (Skill.Character != null && ((enemy.HP / enemy.MaxHP) < (Skill.Character.HP / Skill.Character.MaxHP)))
                {
                    猎手标记.Add(enemy);
                }
            }
        }
    }
}
