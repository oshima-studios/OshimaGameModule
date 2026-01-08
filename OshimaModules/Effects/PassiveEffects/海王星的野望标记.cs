using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;

namespace Oshima.FunGame.OshimaModules.Effects.PassiveEffects
{
    public class 海王星的野望标记 : Effect
    {
        public override long Id => (long)PassiveEffectID.海王星的野望标记;
        public override string Name => "海王星的野望标记";
        public override string Description => $"此角色持有海王星的野望标记。来自：[ {Source} ]";
        public override EffectType EffectType => EffectType.Mark;
        public override bool IsDebuff => true;
        public override Character Source => _sourceCharacter;
        public override DispelledType DispelledType => DispelledType.CannotBeDispelled;

        private readonly Character _sourceCharacter;

        public 海王星的野望标记(Skill skill, Character sourceCharacter) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            _sourceCharacter = sourceCharacter;
        }
    }
}
