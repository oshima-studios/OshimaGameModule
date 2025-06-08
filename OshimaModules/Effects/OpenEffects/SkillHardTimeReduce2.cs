using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Effects.OpenEffects
{
    public class SkillHardTimeReduce2 : Effect
    {
        public override long Id => (long)EffectID.SkillHardTimeReduce2;
        public override string Name => Skill.Name;
        public override string Description => $"减少角色的所有主动技能 {减少比例 * 100:0.##}% 硬直时间。" + (Source != null && (Skill.Character != Source || Skill is not OpenSkill) ? $"来自：[ {Source} ]" + (Skill.Item != null ? $" 的 [ {Skill.Item.Name} ]" : (Skill is OpenSkill ? "" : $" 的 [ {Skill.Name} ]")) : "");

        private readonly double 减少比例 = 0;

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
            foreach (Skill s in character.Skills)
            {
                s.HardnessTime -= s.HardnessTime * 减少比例;
            }
            foreach (Skill? s in character.Items.Select(i => i.Skills.Active))
            {
                if (s != null)
                    s.HardnessTime -= s.HardnessTime * 减少比例;
            }
        }

        public override void OnEffectLost(Character character)
        {
            foreach (Skill s in character.Skills)
            {
                s.HardnessTime += s.HardnessTime * 减少比例;
            }
            foreach (Skill? s in character.Items.Select(i => i.Skills.Active))
            {
                if (s != null)
                    s.HardnessTime += s.HardnessTime * 减少比例;
            }
        }

        public SkillHardTimeReduce2(Skill skill, Dictionary<string, object> args, Character? source = null) : base(skill, args)
        {
            EffectType = EffectType.Item;
            GamingQueue = skill.GamingQueue;
            Source = source;
            if (Values.Count > 0)
            {
                string key = Values.Keys.FirstOrDefault(s => s.Equals("shtr", StringComparison.CurrentCultureIgnoreCase)) ?? "";
                if (key.Length > 0 && double.TryParse(Values[key].ToString(), out double shtr) && shtr > 0)
                {
                    减少比例 = shtr;
                }
            }
        }
    }
}
