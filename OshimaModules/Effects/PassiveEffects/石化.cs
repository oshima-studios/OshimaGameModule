using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;

namespace Oshima.FunGame.OshimaModules.Effects.PassiveEffects
{
    public class 石化(Skill skill, Character sourceCharacter, bool durative = false, double duration = 0, int durationTurn = 1) : 完全行动不能(nameof(石化), EffectType.Petrify, skill, sourceCharacter, durative, duration, durationTurn)
    {
        public override long Id => (long)PassiveEffectID.石化;
    }
}
