using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;

namespace Oshima.FunGame.OshimaModules.Effects.PassiveEffects
{
    public class 长期监视 : Effect
    {
        public override long Id => (long)PassiveEffectID.长期监视;
        public override string Name => "长期监视";
        public override string Description => $"此角色正在被长期监视。来自：[ {Source} ]";
        public override EffectType EffectType => EffectType.Mark;
        public override bool IsDebuff => true;
        public override bool DurativeWithoutDuration => true;
        public override Character Source => _sourceCharacter;
        public override DispelledType DispelledType => DispelledType.CannotBeDispelled;

        private readonly Character _sourceCharacter;

        public 长期监视(Skill skill, Character sourceCharacter) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            _sourceCharacter = sourceCharacter;
        }
    }
}
