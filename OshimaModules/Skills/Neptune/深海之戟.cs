using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.PassiveEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 深海之戟 : Skill
    {
        public override long Id => (long)PassiveID.深海之戟;
        public override string Name => "深海之戟";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override string ExemptionDescription => Effects.Count > 0 ? Effects.First().ExemptionDescription : "";
        public override int CanSelectTargetRange => 5;

        public 深海之戟(Character? character = null) : base(SkillType.Passive, character)
        {
            Effects.Add(new 深海之戟特效(this));
        }

        public override IEnumerable<Effect> AddPassiveEffectToCharacter()
        {
            return Effects;
        }
    }

    public class 深海之戟特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description
        {
            get
            {
                string str = $"分裂伤害：{分裂百分比 * 100:0.##}%。无视免疫。";
                if (野望 != null)
                {
                    return $"技能机制受 [ {nameof(海王星的野望)} ] 影响而改变：{野望.爆炸伤害描述}未攻击标记目标时，按原机制，在普通攻击暴击时分裂。";
                }
                else if (GamingQueue?.Map != null)
                {
                    return $"普通攻击暴击时会自动产生分裂伤害至其附近半径为 {Skill.CanSelectTargetRange} 格的菱形区域内的敌人，但最多只会对两个敌人造成分裂伤害。{str}";
                }
                else
                {
                    return $"普通攻击暴击时会自动产生分裂伤害至其他两个随机的敌人。{str}";
                }
            }
        }
        public override ImmuneType IgnoreImmune => ImmuneType.All;

        public double 分裂百分比 { get; set; } = 0.25;
        public 海王星的野望特效? 野望 { get; set; } = null;

        public override void AfterDamageCalculation(Character character, Character enemy, double damage, double actualDamage, bool isNormalAttack, DamageType damageType, MagicType magicType, DamageResult damageResult)
        {
            if (character == Skill.Character && 野望 != null && (damageResult == DamageResult.Normal || damageResult == DamageResult.Critical) && enemy.Effects.FirstOrDefault(e => e is 海王星的野望标记) is 海王星的野望标记 e)
            {
                野望.分裂伤害(character, enemy, actualDamage, damageType, magicType);
            }
            else if (character == Skill.Character && isNormalAttack && damageResult == DamageResult.Critical && GamingQueue != null)
            {
                List<Character> allEnemys = [.. GamingQueue.GetEnemies(character).Where(c => c != character && c != enemy && c.HP > 0)];
                List<Character> targets = [];
                if (GamingQueue?.Map is GameMap map)
                {
                    List<Grid> grids = [];
                    Grid? enemyGrid = map.GetCharacterCurrentGrid(enemy);
                    if (enemyGrid != null)
                    {
                        grids.AddRange(map.GetGridsByRange(enemyGrid, Skill.CanSelectTargetRange, true));
                        grids = [.. grids.Where(g => g.Characters.Count > 0).OrderBy(g => GameMap.CalculateManhattanDistance(enemyGrid, g)).Take(2)];
                    }
                    targets = Skill.SelectTargetsByRange(character, allEnemys, [], [], grids, false);
                }
                else
                {
                    targets.AddRange(allEnemys.OrderBy(o => Random.Shared.Next()).Take(2));
                }
                double 分裂伤害 = actualDamage * 分裂百分比;
                foreach (Character target in targets)
                {
                    DamageToEnemy(character, target, damageType, magicType, 分裂伤害, new(character)
                    {
                        CalculateCritical = false,
                        CalculateReduction = true,
                        TriggerEffects = false,
                        IgnoreImmune = true
                    });
                }
            }
        }
    }
}
