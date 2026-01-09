using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 概念之骰 : Skill
    {
        public override long Id => (long)PassiveID.概念之骰;
        public override string Name => "概念之骰";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";

        public 概念之骰(Character? character = null) : base(SkillType.Passive, character)
        {
            Effects.Add(new 概念之骰特效(this));
        }

        public override IEnumerable<Effect> AddPassiveEffectToCharacter()
        {
            return Effects;
        }
    }

    public class 概念之骰特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"{Skill.SkillOwner()}的每次行动决策结束时，都会为自身投掷一次概念骰子，获得一种增益且维持至下一次行动决策。\r\n" +
            $"骰子效果（六面）：\r\n" +
            $"1.星辉：下次造成的伤害提升 30%。\r\n" +
            $"2.壁垒：下次受到的伤害减少 30%。\r\n" +
            $"3.流转：立即获得 50 点能量值。\r\n" +
            $"4.预读：下次使用的主动技能冷却时间减少 40%。\r\n" +
            $"5.共鸣：本次行动为全场所有其他角色（不论敌友）附加能量涟漪，使其下一次行动的能量消耗增加 20%。\r\n" +
            $"6.干涉：随机使一名敌方角色身上的一个增益效果或一名友方角色身上的一个负面效果的持续时间延长或缩短 1 回合。";

        public override void OnEffectGained(Character character)
        {

        }

        public override void OnEffectLost(Character character)
        {

        }
    }
}
