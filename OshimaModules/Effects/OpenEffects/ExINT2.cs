﻿using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Effects.OpenEffects
{
    public class ExINT2 : Effect
    {
        public override long Id => (long)EffectID.ExINT2;
        public override string Name => "智力加成";
        public override string Description => $"增加角色 {加成比例 * 100:0.##}% [ {实际加成:0.##} ] 点智力。" + (!TargetSelf ? $"来自：[ {Source} ]" + (Item != null ? $" 的 [ {Item.Name} ]" : "") : "");
        public override EffectType EffectType => EffectType.Item;
        public override bool TargetSelf => true;

        public Item? Item { get; }
        private readonly double 加成比例 = 0;
        private double 实际加成 = 0;

        public override void OnEffectGained(Character character)
        {
            实际加成 = character.BaseINT * 加成比例;
            character.ExINT += 实际加成;
        }

        public override void OnEffectLost(Character character)
        {
            character.ExINT -= 实际加成;
            实际加成 = 0;
        }

        public ExINT2(Skill skill, Dictionary<string, object> args, Character? source = null, Item? item = null) : base(skill, args)
        {
            GamingQueue = skill.GamingQueue;
            Source = source;
            Item = item;
            if (Values.Count > 0)
            {
                string key = Values.Keys.FirstOrDefault(s => s.Equals("exint", StringComparison.CurrentCultureIgnoreCase)) ?? "";
                if (key.Length > 0 && double.TryParse(Values[key].ToString(), out double exINT))
                {
                    加成比例 = exINT;
                }
            }
        }
    }
}