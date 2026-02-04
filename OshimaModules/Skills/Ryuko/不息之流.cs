using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 不息之流 : Skill
    {
        public override long Id => (long)PassiveID.不息之流;
        public override string Name => "不息之流";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";

        public 不息之流(Character? character = null) : base(SkillType.Passive, character)
        {
            Effects.Add(new 不息之流特效(this));
        }

        public override IEnumerable<Effect> AddPassiveEffectToCharacter()
        {
            return Effects;
        }
    }

    public class 不息之流特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"{Skill.SkillOwner()}处于正常态和吟唱态时，每秒回复 {回复系数 * 100:0.##}% 最大生命值 [ {Skill.Character?.MaxHP * 回复系数:0.##} ] 并获得 {能量获取:0.##} 点能量。";

        public double 回复系数 { get; set; } = 0.01;
        public double 能量获取 { get; set; } = 1;

        public override void OnTimeElapsed(Character character, double elapsed)
        {
            if (character.CharacterState == CharacterState.Actionable || character.CharacterState == CharacterState.Casting || character.CharacterState == CharacterState.PreCastSuperSkill)
            {
                character.HP += (Skill.Character?.MaxHP ?? 0) * 回复系数 * elapsed;
                character.EP += 能量获取 * elapsed;
            }
        }
    }
}
