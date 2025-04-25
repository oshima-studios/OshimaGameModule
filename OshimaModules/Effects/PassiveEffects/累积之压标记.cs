using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Effects.PassiveEffects
{
    public class 累积之压标记 : Effect
    {
        public override long Id => 4102;
        public override string Name => "累积之压标记";
        public override string Description => $"此角色持有累积之压标记。来自：[ {Source} ]";
        public override EffectType EffectType => EffectType.Mark;
        public override bool IsDebuff => true;
        public override bool DurativeWithoutDuration => true;
        public override Character Source => _sourceCharacter;

        private readonly Character _sourceCharacter;

        public 累积之压标记(Skill skill, Character sourceCharacter) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            _sourceCharacter = sourceCharacter;
        }
    }
}
