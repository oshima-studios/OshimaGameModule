using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 智慧与力量 : Skill
    {
        public override long Id => (long)PassiveID.智慧与力量;
        public override string Name => "智慧与力量";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";

        public 智慧与力量(Character? character = null) : base(SkillType.Passive, character)
        {
            Effects.Add(new 智慧与力量特效(this));
        }

        public override IEnumerable<Effect> AddInactiveEffectToCharacter()
        {
            return Effects;
        }
    }

    public class 智慧与力量特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"当生命值低于 30% 时，进入力量模式，核心属性转为力量，将所有基础智力转化为额外力量；当生命值高于或等于 30% 时，进入智力模式，核心属性转为智力，还原所有基础智力。力量模式下，造成伤害必定暴击；智力模式下，获得 30% 闪避率和 25% 魔法抗性。" +
            (Skill.Character != null ? "（当前模式：" + CharacterSet.GetPrimaryAttributeName(Skill.Character.PrimaryAttribute) + "）" : "");
        
        private double 交换的基础智力 = 0;
        private double 实际增加闪避率 = 0.3;
        private double 实际增加魔法抗性 = 0.25;
        private bool 已经加过 = false;

        public override void OnEffectGained(Character character)
        {
            ResetEffect(character, true);
        }

        public override void OnEffectLost(Character character)
        {
            if (character.PrimaryAttribute == PrimaryAttribute.INT)
            {
                ResetEffect(character, false);
            }
        }

        private void ResetEffect(Character character, bool isAdd)
        {
            if (isAdd)
            {
                已经加过 = true;
                character.ExEvadeRate += 实际增加闪避率;
                character.MDF.None += 实际增加魔法抗性;
                character.MDF.Particle += 实际增加魔法抗性;
                character.MDF.Fleabane += 实际增加魔法抗性;
                character.MDF.Element += 实际增加魔法抗性;
                character.MDF.Shadow += 实际增加魔法抗性;
                character.MDF.Bright += 实际增加魔法抗性;
                character.MDF.PurityContemporary += 实际增加魔法抗性;
                character.MDF.PurityNatural += 实际增加魔法抗性;
                character.MDF.Starmark += 实际增加魔法抗性;
            }
            else
            {
                已经加过 = false;
                character.ExEvadeRate -= 实际增加闪避率;
                character.MDF.None -= 实际增加魔法抗性;
                character.MDF.Particle -= 实际增加魔法抗性;
                character.MDF.Fleabane -= 实际增加魔法抗性;
                character.MDF.Element -= 实际增加魔法抗性;
                character.MDF.Shadow -= 实际增加魔法抗性;
                character.MDF.Bright -= 实际增加魔法抗性;
                character.MDF.PurityContemporary -= 实际增加魔法抗性;
                character.MDF.PurityNatural -= 实际增加魔法抗性;
                character.MDF.Starmark -= 实际增加魔法抗性;
            }
        }

        public override bool BeforeCriticalCheck(Character character, ref double throwingBonus)
        {
            throwingBonus += 100;
            return false;
        }

        public override void OnTimeElapsed(Character character, double elapsed)
        {
            Changed(character);
        }

        public override void OnAttributeChanged(Character character)
        {
            Changed(character);
        }

        private void Changed(Character character)
        {
            if (Skill.Character != null)
            {
                Character c = Skill.Character;
                if (c.HP < c.MaxHP * 0.3)
                {
                    if (c.PrimaryAttribute == PrimaryAttribute.INT)
                    {
                        double pastHP = c.HP;
                        double pastMaxHP = c.MaxHP;
                        double pastMP = c.MP;
                        double pastMaxMP = c.MaxMP;
                        c.PrimaryAttribute = PrimaryAttribute.STR;
                        交换的基础智力 = c.BaseINT;
                        c.ExINT -= 交换的基础智力;
                        c.ExSTR += 交换的基础智力;
                        c.Recovery(pastHP, pastMP, pastMaxHP, pastMaxMP);
                        if (已经加过)
                        {
                            ResetEffect(character, false);
                        }
                    }
                }
                else
                {
                    if (c.PrimaryAttribute == PrimaryAttribute.STR)
                    {
                        double pastHP = c.HP;
                        double pastMaxHP = c.MaxHP;
                        double pastMP = c.MP;
                        double pastMaxMP = c.MaxMP;
                        c.PrimaryAttribute = PrimaryAttribute.INT;
                        c.ExSTR -= 交换的基础智力;
                        c.ExINT += 交换的基础智力;
                        交换的基础智力 = 0;
                        c.Recovery(pastHP, pastMP, pastMaxHP, pastMaxMP);
                        if (!已经加过)
                        {
                            ResetEffect(character, true);
                        }
                    }
                }
            }
        }
    }
}
