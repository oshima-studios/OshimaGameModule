using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 征服者 : Skill
    {
        public override long Id => (long)PassiveID.征服者;
        public override string Name => "征服者";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";

        public 征服者(Character? character = null) : base(SkillType.Passive, character)
        {
            Effects.Add(new 征服者特效(this));
        }

        public override IEnumerable<Effect> AddPassiveEffectToCharacter()
        {
            return Effects;
        }
    }

    public class 征服者特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"每次普通攻击和技能造成伤害时，叠加 1 层征服者层数，每层提供 {真实伤害:0.##} 点真实伤害和 {额外核心属性:0.##} 点额外核心属性。" +
            $"限制：每个回合只能触发一次征服者叠加层数和一次真实伤害的效果。当叠满 4 层时，为下一次造成伤害提供额外的 {满层伤害:0.##} 点真实伤害，随后重置已叠层数。" +
            $"{(层数 > 0 ? $"（当前 {层数} 层，共 {真实伤害 * 层数:0.##} 点真实伤害和 {累计增加:0.##} 点核心属性）" : "")}";

        public int 层数 { get; set; } = 0;
        private double 真实伤害 => Skill.Character != null ? 35 + Skill.Character.Level * 1.35 : 0;
        private double 额外核心属性 => Skill.Character != null ? 2 + Skill.Character.Level * 0.1 : 0;
        private double 满层伤害 => Skill.Character != null ? 140 + Skill.Character.Level * 5.8 : 0;
        private bool 是否是叠加伤害 = false;
        private bool 是否是满层伤害 = false;
        private bool 允许叠层 = true;
        private PrimaryAttribute 核心属性 = PrimaryAttribute.None;
        private double 累计增加 = 0;

        public override void AfterDamageCalculation(Character character, Character enemy, double damage, double actualDamage, bool isNormalAttack, DamageType damageType, MagicType magicType, DamageResult damageResult)
        {
            if (Skill.Character != null && Skill.Character == character && (damageResult == DamageResult.Normal || damageResult == DamageResult.Critical))
            {
                if (!是否是满层伤害 && 层数 == 4 && enemy.HP > 0)
                {
                    是否是满层伤害 = true;
                    是否是叠加伤害 = true;
                    WriteLine($"[ {character} ] 发动了征服者的满层效果！");
                    DamageToEnemy(character, enemy, DamageType.True, magicType, 满层伤害, new(character)
                    {
                        TriggerEffects = false
                    });
                    ClearExPrimaryAttribute(character);
                    层数 = 0;
                    允许叠层 = false;
                }
                if (!是否是满层伤害 && !是否是叠加伤害 && 层数 < 4)
                {
                    if (允许叠层)
                    {
                        允许叠层 = false;
                        ClearExPrimaryAttribute(character);
                        层数++;
                        核心属性 = character.PrimaryAttribute;
                        累计增加 = 额外核心属性 * 层数;
                        switch (核心属性)
                        {
                            case PrimaryAttribute.AGI:
                                character.ExAGI += 累计增加;
                                break;
                            case PrimaryAttribute.INT:
                                character.ExINT += 累计增加;
                                break;
                            default:
                                character.ExSTR += 累计增加;
                                break;
                        }
                        WriteLine($"[ {character} ] 的征服者层数：{层数}，获得了 {累计增加:0.##} 点核心属性（{CharacterSet.GetPrimaryAttributeName(核心属性)}）加成。");
                        if (层数 > 0 && enemy.HP > 0)
                        {
                            是否是叠加伤害 = true;
                            WriteLine($"[ {character} ] 发动了征服者！");
                            DamageToEnemy(character, enemy, DamageType.True, magicType, 真实伤害 * 层数, new(character)
                            {
                                TriggerEffects = false
                            });
                        }
                    }
                }
            }
        }

        public override void OnTurnEnd(Character character)
        {
            是否是叠加伤害 = false;
            是否是满层伤害 = false;
            允许叠层 = true;
        }

        private void ClearExPrimaryAttribute(Character character)
        {
            if (核心属性 != PrimaryAttribute.None)
            {
                switch (核心属性)
                {
                    case PrimaryAttribute.AGI:
                        character.ExAGI -= 累计增加;
                        break;
                    case PrimaryAttribute.INT:
                        character.ExINT -= 累计增加;
                        break;
                    default:
                        character.ExSTR -= 累计增加;
                        break;
                }
                核心属性 = PrimaryAttribute.None;
                累计增加 = 0;
            }
        }
    }
}
