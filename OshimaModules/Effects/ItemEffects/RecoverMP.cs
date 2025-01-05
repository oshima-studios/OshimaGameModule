﻿using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;

namespace Oshima.FunGame.OshimaModules.Effects.ItemEffects
{
    public class RecoverMP : Effect
    {
        public override long Id => (long)EffectID.RecoverMP;
        public override string Name => "立即回复魔法值";
        public override string Description => $"立即回复角色 {实际回复:0.##} 点魔法值。" + (Source != null && Skill.Character != Source ? $"来自：[ {Source} ]" + (Skill.Item != null ? $" 的 [ {Skill.Item.Name} ]" : "") : "");
        public override EffectType EffectType => EffectType.Item;
        
        private readonly double 实际回复 = 0;

        public RecoverMP(Skill skill, Dictionary<string, object> args, Character? source = null) : base(skill, args)
        {
            GamingQueue = skill.GamingQueue;
            Source = source;
            if (Values.Count > 0)
            {
                string key = Values.Keys.FirstOrDefault(s => s.Equals("mp", StringComparison.CurrentCultureIgnoreCase)) ?? "";
                if (key.Length > 0 && double.TryParse(Values[key].ToString(), out double mp) && mp > 0)
                {
                    实际回复 = mp;
                }
            }
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, Dictionary<string, object> others)
        {
            caster.MP += 实际回复;
        }

        public override void OnSkillCasted(List<Character> targets, Dictionary<string, object> others)
        {
            foreach (Character target in targets)
            {
                target.MP += 实际回复;
            }
        }
    }
}
