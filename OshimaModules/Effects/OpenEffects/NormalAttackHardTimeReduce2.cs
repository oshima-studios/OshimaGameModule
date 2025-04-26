using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Effects.OpenEffects
{
    public class NormalAttackHardTimeReduce2 : Effect
    {
        public override long Id => (long)EffectID.NormalAttackHardTimeReduce2;
        public override string Name => Skill.Name;
        public override string Description => $"减少角色的普通攻击 {减少比例 * 100:0.##}% 硬直时间。" + (Source != null && Skill.Character != Source ? $"来自：[ {Source} ]" + (Skill.Item != null ? $" 的 [ {Skill.Item.Name} ]" : "") : "");

        private readonly double 减少比例 = 0;

        public override void OnEffectGained(Character character)
        {
            character.NormalAttack.HardnessTime -= character.NormalAttack.HardnessTime * 减少比例;
        }

        public override void OnEffectLost(Character character)
        {
            character.NormalAttack.HardnessTime += character.NormalAttack.HardnessTime * 减少比例;
        }

        public NormalAttackHardTimeReduce2(Skill skill, Dictionary<string, object> args, Character? source = null) : base(skill, args)
        {
            EffectType = EffectType.Item;
            GamingQueue = skill.GamingQueue;
            Source = source;
            if (Values.Count > 0)
            {
                string key = Values.Keys.FirstOrDefault(s => s.Equals("nahtr", StringComparison.CurrentCultureIgnoreCase)) ?? "";
                if (key.Length > 0 && double.TryParse(Values[key].ToString(), out double nahtr) && nahtr > 0)
                {
                    减少比例 = nahtr;
                }
            }
        }
    }
}
