using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class META马 : Skill
    {
        public override long Id => (long)PassiveID.META马;
        public override string Name => "META马";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";

        public META马(Character? character = null) : base(SkillType.Passive, character)
        {
            Effects.Add(new META马特效(this));
        }

        public override IEnumerable<Effect> AddInactiveEffectToCharacter()
        {
            return Effects;
        }
    }

    public class META马特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"META马专属被动：力量+5，力量成长+0.5；在受到伤害时，获得的能量提升50%，每回合开始还能获得额外的 [ {EP} ] 能量值。";
        public override bool TargetSelf => true;
        public static double EP => 10;

        public override void AlterEPAfterGetDamage(Character character, ref double baseEP)
        {
            baseEP = Calculation.Round2Digits(baseEP * 1.5);
            if (Skill.Character != null) WriteLine("[ " + Skill.Character + " ] 发动了META马专属被动！本次获得了 " + baseEP + " 能量！");
        }

        public override void OnTurnStart(Character character)
        {
            if (character.EP < 200)
            {
                character.EP += EP;
                WriteLine("[ " + character + " ] 发动了META马专属被动！本次获得了 " + EP + " 能量！");
            }
        }
    }
}
