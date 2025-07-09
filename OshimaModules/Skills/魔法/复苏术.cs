using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 复苏术 : Skill
    {
        public override long Id => (long)MagicID.复苏术;
        public override string Name => "复苏术";
        public override string Description => string.Join("", Effects.Select(e => e.Description));
        public override string DispelDescription => Effects.FirstOrDefault(e => e is 弱驱散特效)?.DispelDescription ?? "";
        public override double MPCost => Level > 0 ? 95 + (85 * (Level - 1)) : 95;
        public override double CD => 85;
        public override double CastTime => 6;
        public override double HardnessTime { get; set; } = 8;
        public override bool CanSelectSelf => true;
        public override bool CanSelectEnemy => false;
        public override bool CanSelectTeammate => true;
        public override int CanSelectTargetCount => 1;

        public 复苏术(Character? character = null) : base(SkillType.Magic, character)
        {
            SelectTargetPredicates.Add(c => c.HP >= 0 && c.HP < c.MaxHP);
            Effects.Add(new 强驱散特效(this));
            Effects.Add(new 百分比回复生命值(this, 0.21, 0.02, true));
        }

        public override List<Character> GetSelectableTargets(Character caster, List<Character> enemys, List<Character> teammates)
        {
            List<Character> targets = base.GetSelectableTargets(caster, enemys, teammates);
            if (GamingQueue != null)
            {
                // 从死亡队列中获取队友，加入目标列表。
                Dictionary<Character, bool> deaths = GamingQueue.GetIsTeammateDictionary(caster, GamingQueue.Eliminated);
                targets = [.. targets.Union(deaths.Where(kv => kv.Value).Select(kv => kv.Key)).Distinct()];
            }
            return targets;
        }
    }
}
