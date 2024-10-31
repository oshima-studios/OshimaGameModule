﻿using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Effects.OpenEffects
{
    public class ExMDF : Effect
    {
        public override long Id => (long)EffectID.ExMDF;
        public override string Name => "魔法抗性加成";
        public override string Description => $"增加角色 {实际加成 * 100:0.##}% {CharacterSet.GetMagicResistanceName(魔法类型)}。" + (!TargetSelf ? $"来自：[ {Source} ]" + (Item != null ? $" 的 [ {Item.Name} ]" : "") : "");
        public override EffectType EffectType => EffectType.Item;
        public override bool TargetSelf => true;

        public Item? Item { get; }
        private readonly double 实际加成 = 0;
        private readonly MagicType 魔法类型 = MagicType.None;

        public override void OnEffectGained(Character character)
        {
            switch (魔法类型)
            {
                case MagicType.Starmark:
                    character.MDF.Starmark += 实际加成;
                    break;
                case MagicType.PurityNatural:
                    character.MDF.PurityNatural += 实际加成;
                    break;
                case MagicType.PurityContemporary:
                    character.MDF.PurityContemporary += 实际加成;
                    break;
                case MagicType.Bright:
                    character.MDF.Bright += 实际加成;
                    break;
                case MagicType.Shadow:
                    character.MDF.Shadow += 实际加成;
                    break;
                case MagicType.Element:
                    character.MDF.Element += 实际加成;
                    break;
                case MagicType.Fleabane:
                    character.MDF.Fleabane += 实际加成;
                    break;
                case MagicType.Particle:
                    character.MDF.Particle += 实际加成;
                    break;
                case MagicType.None:
                default:
                    character.MDF.SetAllValue(实际加成, false);
                    break;
            }
        }

        public override void OnEffectLost(Character character)
        {
            switch (魔法类型)
            {
                case MagicType.Starmark:
                    character.MDF.Starmark -= 实际加成;
                    break;
                case MagicType.PurityNatural:
                    character.MDF.PurityNatural -= 实际加成;
                    break;
                case MagicType.PurityContemporary:
                    character.MDF.PurityContemporary -= 实际加成;
                    break;
                case MagicType.Bright:
                    character.MDF.Bright -= 实际加成;
                    break;
                case MagicType.Shadow:
                    character.MDF.Shadow -= 实际加成;
                    break;
                case MagicType.Element:
                    character.MDF.Element -= 实际加成;
                    break;
                case MagicType.Fleabane:
                    character.MDF.Fleabane -= 实际加成;
                    break;
                case MagicType.Particle:
                    character.MDF.Particle -= 实际加成;
                    break;
                case MagicType.None:
                default:
                    character.MDF.SetAllValue(-实际加成, false);
                    break;
            }
        }

        public ExMDF(Skill skill, Dictionary<string, object> args, Character? source = null, Item? item = null) : base(skill, args)
        {
            GamingQueue = skill.GamingQueue;
            Source = source;
            Item = item;
            if (Values.Count > 0)
            {
                string key = Values.Keys.FirstOrDefault(s => s.Equals("mdfType", StringComparison.CurrentCultureIgnoreCase)) ?? "";
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