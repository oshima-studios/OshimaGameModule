﻿using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Effects.OpenEffects
{
    public class NormalAttackHardTimeReduce : Effect
    {
        public override long Id => (long)EffectID.NormalAttackHardTimeReduce;
        public override string Name => Skill.Name;
        public override string Description => $"{(实际硬直时间减少 < 0 ? "增加" : "减少")}角色的普通攻击 {实际硬直时间减少:0.##} {GameplayEquilibriumConstant.InGameTime}硬直时间。" + (Source != null && (Skill.Character != Source || Skill is not OpenSkill) ? $"来自：[ {Source} ]" + (Skill.Item != null ? $" 的 [ {Skill.Item.Name} ]" : (Skill is OpenSkill ? "" : $" 的 [ {Skill.Name} ]")) : "");

        private readonly double 实际硬直时间减少 = 0;

        public override void OnEffectGained(Character character)
        {
            if (Durative && RemainDuration == 0)
            {
                RemainDuration = Duration;
            }
            else if (RemainDurationTurn == 0)
            {
                RemainDurationTurn = DurationTurn;
            }
            character.NormalAttack.ExHardnessTime -= 实际硬直时间减少;
        }

        public override void OnEffectLost(Character character)
        {
            character.NormalAttack.ExHardnessTime += 实际硬直时间减少;
        }

        public NormalAttackHardTimeReduce(Skill skill, Dictionary<string, object> args, Character? source = null) : base(skill, args)
        {
            EffectType = EffectType.Item;
            GamingQueue = skill.GamingQueue;
            Source = source;
            if (Values.Count > 0)
            {
                string key = Values.Keys.FirstOrDefault(s => s.Equals("nahtr", StringComparison.CurrentCultureIgnoreCase)) ?? "";
                if (key.Length > 0 && double.TryParse(Values[key].ToString(), out double nahtr) && nahtr > 0)
                {
                    实际硬直时间减少 = nahtr;
                }
            }
        }
    }
}
