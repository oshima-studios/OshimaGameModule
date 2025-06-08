using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Effects.OpenEffects
{
    public class ExSPD : Effect
    {
        public override long Id => (long)EffectID.ExSPD;
        public override string Name => "行动速度加成";
        public override string Description => $"{(实际加成 >= 0 ? "增加" : "减少")}角色 {Math.Abs(实际加成):0.##} 点行动速度。" + (Source != null && (Skill.Character != Source || Skill is not OpenSkill) ? $"来自：[ {Source} ]" + (Skill.Item != null ? $" 的 [ {Skill.Item.Name} ]" : (Skill is OpenSkill ? "" : $" 的 [ {Skill.Name} ]")) : "");
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
            character.ExSPD += 实际加成;
        }

        public override void OnEffectLost(Character character)
        {
            character.ExSPD -= 实际加成;
        }

        public ExSPD(Skill skill, Dictionary<string, object> args, Character? source = null) : base(skill, args)
        {
            EffectType = EffectType.Item;
            GamingQueue = skill.GamingQueue;
            Source = source;
            if (Values.Count > 0)
            {
                string key = Values.Keys.FirstOrDefault(s => s.Equals("exspd", StringComparison.CurrentCultureIgnoreCase)) ?? "";
                if (key.Length > 0 && double.TryParse(Values[key].ToString(), out double exSPD))
                {
                    实际加成 = exSPD;
                }
            }
        }
    }
}
