using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Effects.OpenEffects
{
    public class ExATK2 : Effect
    {
        public override long Id => (long)EffectID.ExATK2;
        public override string Name { get; set; } = "攻击力加成";
        public override string Description => $"{(实际加成 >= 0 ? "增加" : "减少")}角色 {Math.Abs(加成比例) * 100:0.##}% [ {(实际加成 == 0 ? "基于基础攻击力" : $"{Math.Abs(实际加成):0.##}")} ] 点攻击力。" + (Source != null && (Skill.Character != Source || Skill is not OpenSkill) ? $"来自：[ {Source} ]" + (Skill.Item != null ? $" 的 [ {Skill.Item.Name} ]" : (Skill is OpenSkill ? "" : $" 的 [ {Skill.Name} ]")) : "");
        public double Value => 实际加成;

        private readonly double 加成比例 = 0;
        private double 实际加成 = 0;

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
            实际加成 = character.BaseATK * 加成比例;
            character.ExATKPercentage += 加成比例;
        }

        public override void OnEffectLost(Character character)
        {
            character.ExATKPercentage -= 加成比例;
        }

        public override void OnAttributeChanged(Character character)
        {
            // 刷新加成
            OnEffectLost(character);
            OnEffectGained(character);
        }

        public ExATK2(Skill skill, Dictionary<string, object> args, Character? source = null) : base(skill, args)
        {
            EffectType = EffectType.Item;
            GamingQueue = skill.GamingQueue;
            Source = source;
            if (Values.Count > 0)
            {
                string key = Values.Keys.FirstOrDefault(s => s.Equals("exatk", StringComparison.CurrentCultureIgnoreCase)) ?? "";
                if (key.Length > 0 && double.TryParse(Values[key].ToString(), out double exATK))
                {
                    加成比例 = exATK;
                }
            }
        }
    }
}
