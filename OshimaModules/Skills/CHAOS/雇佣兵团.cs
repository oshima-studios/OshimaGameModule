using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 雇佣兵团 : Skill
    {
        public override long Id => (long)PassiveID.雇佣兵团;
        public override string Name => "雇佣兵团";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";

        public 雇佣兵团(Character? character = null) : base(SkillType.Passive, character)
        {
            Effects.Add(new 雇佣兵团特效(this));
        }

        public override IEnumerable<Effect> AddPassiveEffectToCharacter()
        {
            return Effects;
        }
    }

    public class 雇佣兵团特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"{Skill.SkillOwner()}在场上时，会召唤数名雇佣兵协助战斗，初始数量为 {最小数量} 名，雇佣兵具有独立的回合，生命值为{Skill.SkillOwner()}的 {生命值比例 * 100:0.##}% [ {Skill.Character?.MaxHP * 生命值比例:0.##} ]，攻击力为{Skill.SkillOwner()}的 {攻击力比例 * 100:0.##}% 基础攻击力 [ {Skill.Character?.BaseATK * 攻击力比例:0.##} ]，" +
            $"完整继承其他能力值（暴击率、闪避率等）。当{Skill.SkillOwner()}参与击杀时，便会临时产生一名额外的雇佣兵，持续 {持续时间} 秒。场上最多可以存在 {最大数量} 名雇佣兵，达到数量后不再产生新的雇佣兵；当不足 {最小数量} 名雇佣兵时，{补充间隔} 秒后会重新补充一名雇佣兵。";

        public const int 最小数量 = 2; 
        public const int 最大数量 = 7; 
        public const int 持续时间 = 30; 
        public const int 补充间隔 = 20; 
        public const double 生命值比例 = 0.3; 
        public const double 攻击力比例 = 0.4; 

        public override void OnEffectGained(Character character)
        {

        }

        public override void OnEffectLost(Character character)
        {
            
        }
    }
}
