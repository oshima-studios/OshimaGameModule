using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;

namespace Oshima.FunGame.OshimaModules.Effects.PassiveEffects
{
    public class 电刑标记 : Effect
    {
        public override long Id => (long)PassiveEffectID.电刑标记;
        public override string Name => "电刑标记";
        public override string Description => $"此角色持有电刑标记，层数：{层数} 层。来自：[ {Source} ]";
        public override EffectType EffectType => EffectType.Mark;
        public override bool IsDebuff => true;
        public override Character Source => _sourceCharacter;
        public int 层数 { get; set; } = 1;

        private readonly Character _sourceCharacter;

        public 电刑标记(Skill skill, Character sourceCharacter) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            _sourceCharacter = sourceCharacter;
        }
    }
}
