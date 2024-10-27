using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 嗜血本能 : Skill
    {
        public override long Id => (long)SuperSkillID.嗜血本能;
        public override string Name => "嗜血本能";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override double EPCost => 100;
        public override double CD => 42 - 1 * (Level - 1);
        public override double HardnessTime { get; set; } = 12;

        public 嗜血本能(Character? character = null) : base(SkillType.SuperSkill, character)
        {
            Effects.Add(new 嗜血本能特效(this));
        }
    }

    public class 嗜血本能特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"{Duration} 时间内，攻击拥有标记的角色将根据标记层数获得 {吸血 * 100:0.##}% 吸血每层。";
        public override bool TargetSelf => true;
        public override bool Durative => true;
        public override double Duration => 30;

        public HashSet<Character> 角色有第四层 { get; } = [];
        private double 吸血 => 0.03 * Level;

        public override void AfterDamageCalculation(Character character, Character enemy, double damage, bool isNormalAttack, bool isMagicDamage, MagicType magicType, DamageResult damageResult)
        {
            if (character == Skill.Character && damageResult != DamageResult.Evaded && character.HP < character.MaxHP)
            {
                int 层数 = 0;
                if (enemy.Effects.Where(e => e is 累积之压标记).FirstOrDefault() is 累积之压标记 e)
                {
                    层数 = e.MarkLevel;
                }
                else if (角色有第四层.Remove(enemy))
                {
                    层数 = 4;
                }
                double 实际吸血 = 吸血 * 层数 * damage;
                character.HP += 实际吸血;
                WriteLine($"[ {character} ] 回复了 {实际吸血:0.##} 点生命值！");
            }
        }

        public override void OnSkillCasted(Character caster, List<Character> enemys, List<Character> teammates, Dictionary<string, object> others)
        {
            RemainDuration = Duration;
            if (!caster.Effects.Contains(this))
            {
                角色有第四层.Clear();
                caster.Effects.Add(this);
                OnEffectGained(caster);
            }
        }
    }
}
