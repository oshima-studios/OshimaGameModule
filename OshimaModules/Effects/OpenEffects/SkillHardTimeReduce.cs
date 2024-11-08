﻿using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Effects.OpenEffects
{
    public class SkillHardTimeReduce : Effect
    {
        public override long Id => (long)EffectID.SkillHardTimeReduce;
        public override string Name => Skill.Name;
        public override string Description => $"减少角色的所有主动技能 {实际硬直时间减少:0.##} 硬直时间。" + (Source != null && Skill.Character != Source ? $"来自：[ {Source} ]" + (Skill.Item != null ? $" 的 [ {Skill.Item.Name} ]" : "") : "");
        public override EffectType EffectType => EffectType.Item;
        
        private readonly double 实际硬直时间减少 = 0;

        public override void OnEffectGained(Character character)
        {
            foreach (Skill s in character.Skills)
            {
                s.HardnessTime -= 实际硬直时间减少;
            }
            foreach (Skill? s in character.Items.Select(i => i.Skills.Active))
            {
                if (s != null)
                    s.HardnessTime -= 实际硬直时间减少;
            }
        }

        public override void OnEffectLost(Character character)
        {
            foreach (Skill s in character.Skills)
            {
                s.HardnessTime += 实际硬直时间减少;
            }
            foreach (Skill? s in character.Items.Select(i => i.Skills.Active))
            {
                if (s != null)
                    s.HardnessTime += 实际硬直时间减少;
            }
        }

        public SkillHardTimeReduce(Skill skill, Dictionary<string, object> args, Character? source = null) : base(skill, args)
        {
            GamingQueue = skill.GamingQueue;
            Source = source;
            if (Values.Count > 0)
            {
                string key = Values.Keys.FirstOrDefault(s => s.Equals("shtr", StringComparison.CurrentCultureIgnoreCase)) ?? "";
                if (key.Length > 0 && double.TryParse(Values[key].ToString(), out double shtr))
                {
                    实际硬直时间减少 = shtr;
                }
            }
        }
    }
}
