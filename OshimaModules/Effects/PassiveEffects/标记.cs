using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;

namespace Oshima.FunGame.OshimaModules.Effects.PassiveEffects
{
    public class 标记 : Effect
    {
        public override long Id => (long)PassiveEffectID.标记;
        public override string Name => $"{_name}标记";
        public override string Description => $"此角色持有{Name}。{OtherDescription}来自：[ {Source} ]";
        public override EffectType EffectType => EffectType.Mark;
        public override bool IsDebuff => true;
        public override Character Source => _sourceCharacter;
        public string OtherDescription { get; set; } = "";

        private readonly string _name;
        private readonly Character _sourceCharacter;

        public 标记(Skill skill, string name, Character sourceCharacter) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            _name = name;
            _sourceCharacter = sourceCharacter;
        }
    }
}
