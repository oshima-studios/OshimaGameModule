using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;

namespace Oshima.FunGame.OshimaModules.Effects.PassiveEffects
{
    public class 时雨标记 : Effect
    {
        public override long Id => (long)PassiveEffectID.时雨标记;
        public override string Name => "时雨标记";
        public override string Description => $"此角色持有时雨标记。来自：[ {Source} ]";
        public override EffectType EffectType => EffectType.Mark;
        public override bool IsDebuff => true;
        public override Character Source => _sourceCharacter;
        public override DispelledType DispelledType { get; set; } = DispelledType.Weak;

        private readonly Character _sourceCharacter;

        public 时雨标记(Skill skill, Character sourceCharacter) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            _sourceCharacter = sourceCharacter;
        }

        public override void OnTurnStart(Character character, List<Character> enemys, List<Character> teammates, List<Skill> skills, List<Item> items)
        {
            if (GamingQueue is null)
            {
                return;
            }
            List<Character> enemies = GamingQueue.GetEnemies(character);
            if (enemies.Contains(Source) && Random.Shared.NextDouble() < 0.65)
            {
                WriteLine($"[ {character} ] 受到了{nameof(时雨标记)}的影响，陷入了混乱！！！");
                Effect e = new 混乱(Skill, character, false, 0, 1);
                character.Effects.Add(e);
                e.OnEffectGained(character);
            }
        }

        public override double AlterActualDamageAfterCalculation(Character character, Character enemy, double damage, bool isNormalAttack, DamageType damageType, MagicType magicType, DamageResult damageResult, ref bool isEvaded, Dictionary<Effect, double> totalDamageBonus)
        {
            if (GamingQueue is null)
            {
                return 0;
            }
            List<Character> teammates = GamingQueue.GetTeammates(character);
            if ((character == Source || teammates.Contains(Source)) && enemy.Effects.Any(e => e is 时雨标记))
            {
                double bonus = damage * 0.25;
                WriteLine($"[ {character} ] 受到了{nameof(时雨标记)}的影响，伤害提高了 {bonus:0.##} 点！");
                return bonus;
            }
            return 0;
        }
    }
}
