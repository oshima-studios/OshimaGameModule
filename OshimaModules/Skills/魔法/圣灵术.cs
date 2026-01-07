using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.SkillEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 圣灵术 : Skill
    {
        public override long Id => (long)MagicID.圣灵术;
        public override string Name => "圣灵术";
        public override string Description => string.Join("", Effects.Select(e => e.Description));
        public override string DispelDescription => Effects.FirstOrDefault(e => e is 弱驱散特效)?.DispelDescription ?? "";
        public override double MPCost => Level > 0 ? 110 + (95 * (Level - 1)) : 110;
        public override double CD => Level > 0 ? 65 - (2 * (Level - 1)) : 65;
        public override double CastTime => 3;
        public override double HardnessTime { get; set; } = 7;
        public override bool CanSelectSelf => true;
        public override bool CanSelectEnemy => false;
        public override bool CanSelectTeammate => true;
        public override bool SelectAllTeammates => true;

        public 圣灵术(Character? character = null) : base(SkillType.Magic, character)
        {
            SelectTargetPredicates.Add(c => c.HP >= 0 && c.HP < c.MaxHP);
            Effects.Add(new 强驱散特效(this));
            Effects.Add(new 纯数值回复生命(this, 380, 270, true));
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
