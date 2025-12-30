using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 疾走 : Skill
    {
        public override long Id => (long)SkillID.疾走;
        public override string Name => "疾走";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";
        public override double EPCost => 25;
        public override double CD => 15;
        public override double HardnessTime { get; set; } = 3;
        public override bool CanSelectSelf => true;
        public override bool CanSelectEnemy => false;

        public 疾走(Character? character = null) : base(SkillType.Skill, character)
        {
            Effects.Add(new 疾走特效(this));
        }
    }

    public class 疾走特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"本回合内大幅提升移动距离。";
        public override string DispelDescription => "";
        public override EffectType EffectType => EffectType.Haste;
        public override bool Durative => false;
        public override double Duration => 0;
        public override int DurationTurn => 1;

        private int 移动距离提升
        {
            get
            {
                return Skill.Level switch
                {
                    2 or 3 => 2,
                    4 or 5 => 3,
                    6 => 4,
                    _ => 1,
                };
            }
        }

        public override void OnEffectGained(Character character)
        {
            Skill.IsInEffect = true;
            character.ExMOV += 移动距离提升;
        }

        public override void OnEffectLost(Character character)
        {
            Skill.IsInEffect = false;
            character.ExMOV -= 移动距离提升;
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, Dictionary<string, object> others)
        {
            if (!caster.Effects.Contains(this))
            {
                GamingQueue?.LastRound.AddApplyEffects(caster, EffectType);
                RemainDurationTurn = DurationTurn;
                caster.Effects.Add(this);
                OnEffectGained(caster);
            }
        }
    }
}
