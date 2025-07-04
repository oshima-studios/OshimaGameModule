﻿using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Effects.OpenEffects
{
    public class ExMDF : Effect
    {
        public override long Id => (long)EffectID.ExMDF;
        public override string Name => "魔法抗性加成";
        public override string Description => $"{(实际加成 >= 0 ? "增加" : "减少")}角色 {Math.Abs(实际加成) * 100:0.##}% {CharacterSet.GetMagicResistanceName(魔法类型)}。" + (Source != null && (Skill.Character != Source || Skill is not OpenSkill) ? $"来自：[ {Source} ]" + (Skill.Item != null ? $" 的 [ {Skill.Item.Name} ]" : (Skill is OpenSkill ? "" : $" 的 [ {Skill.Name} ]")) : "");
        public double Value => 实际加成;

        private readonly double 实际加成 = 0;
        private readonly MagicType 魔法类型 = MagicType.None;

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
            character.MDF[魔法类型] += 实际加成;
        }

        public override void OnEffectLost(Character character)
        {
            character.MDF[魔法类型] -= 实际加成;
        }

        public ExMDF(Skill skill, Dictionary<string, object> args, Character? source = null) : base(skill, args)
        {
            EffectType = EffectType.Item;
            GamingQueue = skill.GamingQueue;
            Source = source;
            if (Values.Count > 0)
            {
                string key = Values.Keys.FirstOrDefault(s => s.Equals("mdftype", StringComparison.CurrentCultureIgnoreCase)) ?? "";
                if (key.Length > 0 && int.TryParse(Values[key].ToString(), out int mdfType))
                {
                    if (Enum.IsDefined(typeof(MagicType), mdfType))
                    {
                        魔法类型 = (MagicType)mdfType;
                    }
                    else
                    {
                        魔法类型 = MagicType.None;
                    }
                }
                key = Values.Keys.FirstOrDefault(s => s.Equals("mdfvalue", StringComparison.CurrentCultureIgnoreCase)) ?? "";
                if (key.Length > 0 && double.TryParse(Values[key].ToString(), out double mdfValue))
                {
                    实际加成 = mdfValue;
                }
            }
        }
    }
}
