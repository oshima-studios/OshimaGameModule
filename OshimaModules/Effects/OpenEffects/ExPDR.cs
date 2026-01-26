using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Effects.OpenEffects
{
    public class ExPDR : Effect
    {
        public override long Id => (long)EffectID.ExPDR;
        public override string Name { get; set; } = "物理伤害减免加成";
        public override string Description => $"{(实际加成 >= 0 ? "增加" : "减少")}角色 {Math.Abs(实际加成) * 100:0.##}% 物理伤害减免。" + (Source != null && (Skill.Character != Source || Skill is not OpenSkill) ? $"来自：[ {Source} ]" + (Skill.Item != null ? $" 的 [ {Skill.Item.Name} ]" : (Skill is OpenSkill ? "" : $" 的 [ {Skill.Name} ]")) : "");
        public double Value => 实际加成;

        private readonly double 实际加成 = 0;

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
            character.ExPDR += 实际加成;
        }

        public override void OnEffectLost(Character character)
        {
            character.ExPDR -= 实际加成;
        }

        public ExPDR(Skill skill, Dictionary<string, object> args, Character? source = null) : base(skill, args)
        {
            EffectType = EffectType.Item;
            GamingQueue = skill.GamingQueue;
            Source = source;
            if (Values.Count > 0)
            {
                string key = Values.Keys.FirstOrDefault(s => s.Equals("expdr", StringComparison.CurrentCultureIgnoreCase)) ?? "";
                if (key.Length > 0 && double.TryParse(Values[key].ToString(), out double exPDR))
                {
                    实际加成 = exPDR;
                }
            }
        }
    }
}
