using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;
using Oshima.FunGame.OshimaModules.Skills;

namespace Oshima.FunGame.OshimaModules.Effects.ItemEffects
{
    public class RecoverHP : Effect
    {
        public override long Id => (long)EffectID.RecoverHP;
        public override string Name => "立即回复生命值";
        public override string Description => $"立即回复{Skill.TargetDescription()} {实际回复:0.##} 点生命值（{(能复活 ? "" : "不")}可用于复活）。" + (Source != null && (Skill.Character != Source || Skill is not OpenSkill) ? $"来自：[ {Source} ]" + (Skill.Item != null ? $" 的 [ {Skill.Item.Name} ]" : (Skill is OpenSkill ? "" : $" 的 [ {Skill.Name} ]")) : "");
        public override EffectType EffectType { get; set; } = EffectType.Item;

        private readonly double 实际回复 = 0;
        private readonly bool 能复活 = false;

        public RecoverHP(Skill skill, Dictionary<string, object> args, Character? source = null) : base(skill, args)
        {
            GamingQueue = skill.GamingQueue;
            Source = source;
            if (Values.Count > 0)
            {
                string key = Values.Keys.FirstOrDefault(s => s.Equals("hp", StringComparison.CurrentCultureIgnoreCase)) ?? "";
                if (key.Length > 0 && double.TryParse(Values[key].ToString(), out double hp) && hp > 0)
                {
                    实际回复 = hp;
                }
                key = Values.Keys.FirstOrDefault(s => s.Equals("respawn", StringComparison.CurrentCultureIgnoreCase)) ?? "";
                if (key.Length > 0 && bool.TryParse(Values[key].ToString(), out bool respawn) && respawn)
                {
                    能复活 = respawn;
                }
            }
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            foreach (Character target in targets)
            {
                HealToTarget(caster, target, 实际回复, 能复活);
            }
        }

        public override void OnSkillCasted(User user, List<Character> targets, Dictionary<string, object> others)
        {
            base.OnSkillCasted(user, targets, others);
            foreach (Character target in targets)
            {
                target.HP += 实际回复;
            }
        }
    }
}
