using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 神之因果 : Skill
    {
        public override long Id => (long)SuperSkillID.神之因果;
        public override string Name => "神之因果";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";
        public override double EPCost => 100;
        public override bool CostAllEP => true;
        public override double CD => 90;
        public override double HardnessTime { get; set; } = 1;
        public override bool CanSelectSelf => true;
        public override bool CanSelectEnemy => false;

        public 神之因果(Character? character = null) : base(SkillType.SuperSkill, character)
        {
            Effects.Add(new 神之因果特效(this));
        }
    }

    public class 神之因果特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"{Skill.SkillOwner()}短暂显现其「神」之本质，开启持续 4 回合的「神之领域」：在持续时间内，{Skill.SkillOwner()}对任意敌方目标造成的伤害，都将记录为「因果伤害值」。" +
            $"在持续时间结束后，所有敌方角色都会受到 [ 基于总因果伤害值的 600% 除以当前在场敌方角色数量 ] 的真实伤害。在持续时间内，{Skill.SkillOwner()}可以对任何负面效果进行豁免。" +
            (因果伤害值 > 0 ? $"（当前累计因果：{因果伤害值:0.##} 点）" : "");
        public override DispelledType DispelledType => DispelledType.CannotBeDispelled;
        public override bool Durative => false;
        public override int DurationTurn => 4;

        public double 因果伤害值 { get; set; } = 0;

        public override bool OnExemptionCheck(Character character, Character? source, Effect effect, bool isEvade, ref double throwingBonus)
        {
            if (character == Skill.Character)
            {
                throwingBonus += 300;
            }
            return true;
        }

        public override void AfterDamageCalculation(Character character, Character enemy, double damage, double actualDamage, bool isNormalAttack, DamageType damageType, MagicType magicType, DamageResult damageResult)
        {
            if (character == Skill.Character && (damageResult == DamageResult.Normal || damageResult == DamageResult.Critical))
            {
                因果伤害值 += actualDamage;
            }
        }

        public override void AfterDeathCalculation(Character death, bool hasMaster, Character? killer, Dictionary<Character, int> continuousKilling, Dictionary<Character, int> earnedMoney, Character[] assists)
        {
            if (death == Skill.Character)
            {
                因果伤害值 = 0;
            }
        }

        public override void OnEffectGained(Character character)
        {
            因果伤害值 = 0;
        }

        public override void OnEffectLost(Character character)
        {
            if (GamingQueue != null && 因果伤害值 > 0)
            {
                WriteLine($"[ {character} ] 发动了神之因果！万象因果，命运既定！！！");
                List<Character> enemies = [.. GamingQueue.GetEnemies(character).Where(GamingQueue.Queue.Contains)];
                double damage = 因果伤害值 * 2 / enemies.Count;
                foreach (Character enemy in enemies)
                {
                    DamageToEnemy(character, enemy, DamageType.True, MagicType, damage, new(character)
                    {
                        CalculateShield = false,
                        IgnoreImmune = true,
                        TriggerEffects = false
                    });
                }
                因果伤害值 = 0;
            }
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            RemainDuration = Duration;
            if (!caster.Effects.Contains(this))
            {
                caster.Effects.Add(this);
                OnEffectGained(caster);
            }
            GamingQueue?.LastRound.AddApplyEffects(caster, EffectType.Focusing);
        }
    }
}
