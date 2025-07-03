using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;

namespace Oshima.FunGame.OshimaModules.Effects.PassiveEffects
{
    public class 冻结(Skill skill, Character sourceCharacter, bool durative = false, double duration = 0, int durationTurn = 1) : 完全行动不能(nameof(冻结), EffectType.Petrify, skill, sourceCharacter, durative, duration, durationTurn)
    {
        public override long Id => (long)PassiveEffectID.冻结;
    }
}
