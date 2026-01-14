using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;
using Oshima.FunGame.OshimaModules.Skills;

namespace Oshima.FunGame.OshimaModules.Effects.PassiveEffects
{
    public class 宫监手标记 : Effect
    {
        public override long Id => (long)PassiveEffectID.宫监手标记;
        public override string Name => "宫监手标记";
        public override string Description => $"{放监.任务要求}。来自：[ {Source} ]";
        public override EffectType EffectType => EffectType.Mark;
        public override bool IsDebuff => true;
        public override Character Source => _sourceCharacter;
        public override DispelledType DispelledType { get; set; } = DispelledType.CannotBeDispelled;

        private readonly Character _sourceCharacter;
        private readonly Character _targetCharacter;
        private readonly 放监特效 放监;
        private bool 已完成普攻任务 = false;
        private bool 已完成指向性技能任务 = false;

        public 宫监手标记(Skill skill, Character sourceCharacter, Character targetCharacter, 放监特效 effect) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            _sourceCharacter = sourceCharacter;
            _targetCharacter = targetCharacter;
            放监 = effect;
        }

        public void 普攻任务完成(Character character)
        {
            WriteLine($"[ {character} ] 的「放监」任务 [ 对友方角色普通攻击 ] 完成！");
            已完成普攻任务 = true;
            CheckComplete(character);
        }

        public void 指向性技能任务完成(Character character)
        {
            WriteLine($"[ {character} ] 的「放监」任务 [ 对{Source}释放指向性技能 ] 完成！");
            已完成指向性技能任务 = true;
            CheckComplete(character);
        }

        public void CheckComplete(Character character)
        {
            if (已完成普攻任务 && 已完成指向性技能任务)
            {
                character.Effects.Remove(this);
                WriteLine($"[ {character} ] 已消除宫监手标记！");
            }
        }

        public override void AlterSelectListBeforeSelection(Character character, ISkill skill, List<Character> enemys, List<Character> teammates)
        {
            if (skill is NormalAttack)
            {
                enemys.AddRange(teammates);
            }
        }

        public override bool BeforeCriticalCheck(Character actor, Character enemy, bool isNormalAttack, ref double throwingBonus)
        {
            if (actor == _targetCharacter && isNormalAttack)
            {
                throwingBonus += 300;
            }
            return true;
        }

        public override bool BeforeEvadeCheck(Character actor, Character enemy, ref double throwingBonus)
        {
            if (actor == _targetCharacter)
            {
                return false;
            }
            return true;
        }

        public override void AfterDamageCalculation(Character character, Character enemy, double damage, double actualDamage, bool isNormalAttack, DamageType damageType, MagicType magicType, DamageResult damageResult)
        {
            if (character == _targetCharacter && isNormalAttack && (damageResult == DamageResult.Normal || damageResult == DamageResult.Critical))
            {
                if (GamingQueue != null && GamingQueue.IsTeammate(character, enemy))
                {
                    普攻任务完成(character);
                }
            }
        }

        public override void AfterDeathCalculation(Character death, Character? killer, Dictionary<Character, int> continuousKilling, Dictionary<Character, int> earnedMoney, Character[] assists)
        {
            if (death == _targetCharacter)
            {
                death.Effects.Remove(this);
            }
            if (death == _sourceCharacter)
            {
                _targetCharacter.Effects.Remove(this);
            }
        }

        public override void OnEffectLost(Character character)
        {
            if (!已完成普攻任务 || !已完成指向性技能任务)
            {
                放监.造成伤害(character, !已完成普攻任务 && !已完成指向性技能任务 ? 2 : 1);
            }
        }
    }
}
