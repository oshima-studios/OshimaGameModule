using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Effects.OpenEffects
{
    public class ExMDF : Effect
    {
        public override long Id => (long)EffectID.ExMDF;
        public override string Name => "魔法抗性加成";
        public override string Description => $"{(实际加成 >= 0 ? "增加" : "减少")}角色 {Math.Abs(实际加成) * 100:0.##}% {CharacterSet.GetMagicResistanceName(魔法类型)}。" + (Source != null && Skill.Character != Source ? $"来自：[ {Source} ]" + (Skill.Item != null ? $" 的 [ {Skill.Item.Name} ]" : "") : "");
        public override EffectType EffectType => EffectType.Item;
        
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
                    character.MDF.AddAllValue(实际加成);
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
                    character.MDF.AddAllValue(-实际加成);
                    break;
            }
        }

        public ExMDF(Skill skill, Dictionary<string, object> args, Character? source = null) : base(skill, args)
        {
            GamingQueue = skill.GamingQueue;
            Source = source;
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
