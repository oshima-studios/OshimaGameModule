using Milimoe.FunGame.Core.Entity;
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
        public override string Description => $"当生命值低于 30% 时，智力转化为力量；当生命值高于或等于 30% 时，力量转化为智力。" +
            (Skill.Character != null ? "（当前模式：" + CharacterSet.GetPrimaryAttributeName(Skill.Character.PrimaryAttribute) + "）" : "");
        public override bool TargetSelf => true;

        private double 交换前的额外智力 = 0;
        private double 交换前的额外力量 = 0;

        public override void OnAttributeChanged(Character character)
        {
            if (Skill.Character != null)
            {
                if (Skill.Character.PrimaryAttribute == PrimaryAttribute.INT)
                {
                    double diff = character.ExSTR - 交换前的额外力量;
                    character.ExINT = 交换前的额外力量 + character.BaseSTR + diff;
                }
                else if (Skill.Character.PrimaryAttribute == PrimaryAttribute.STR)
                {
                    double diff = character.ExINT - 交换前的额外智力;
                    character.ExSTR = 交换前的额外智力 + character.BaseINT + diff;
                }
            }
        }

        public override void OnTimeElapsed(Character character, double elapsed)
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
                        交换前的额外智力 = c.ExINT;
                        交换前的额外力量 = c.ExSTR;
                        c.ExINT = -c.BaseINT;
                        c.ExSTR = 交换前的额外智力 + c.BaseINT + 交换前的额外力量;
                        c.Recovery(pastHP, pastMP, pastMaxHP, pastMaxMP);
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
                        交换前的额外智力 = c.ExINT;
                        交换前的额外力量 = c.ExSTR;
                        c.ExINT = 交换前的额外力量 + c.BaseSTR + 交换前的额外智力;
                        c.ExSTR = -c.BaseSTR;
                        c.Recovery(pastHP, pastMP, pastMaxHP, pastMaxMP);
                    }
                }
            }
        }
    }
}
