using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.PassiveEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 开宫 : Skill
    {
        public override long Id => (long)PassiveID.开宫;
        public override string Name => "开宫";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";

        public 开宫(Character? character = null) : base(SkillType.Passive, character)
        {
            Effects.Add(new 开宫特效(this));
        }

        public override IEnumerable<Effect> AddPassiveEffectToCharacter()
        {
            return Effects;
        }
    }

    public class 开宫特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"{Skill.SkillOwner()}在游戏开始时进入 [ {nameof(长期监视)} ] 状态，时刻监视着场上的一举一动。当场上有角色死亡时，如果该角色死于技能，则{Skill.SkillOwner()}复制该技能获得使用权，持续 3 回合；如果该角色死于普通攻击，则{Skill.SkillOwner()}的普通攻击将转为魔法伤害并且无视闪避，持续 3 回合。" +
            $"接着，{Skill.SkillOwner()}给予击杀者 [ {nameof(时雨标记)} ]。{Skill.SkillOwner()}在造成魔法伤害时，会基于伤害值的 15% 治疗持有标记的友方角色；{Skill.SkillOwner()}与所有持有标记的友方角色对持有标记的敌方角色的伤害加成提升 25%，并且使持有标记的敌方角色在持续时间内的回合开始阶段，有 65% 概率陷入混乱。" +
            $"混乱：进入行动受限状态，失控并随机行动，且在进行攻击指令时，可能会选取友方角色为目标。时雨标记持续 3 回合。";

        private bool 激活 = false;

        public override void OnGameStart()
        {
            if (!激活)
            {
                激活 = true;
                发送他们标记();
            }
        }

        public override void OnTimeElapsed(Character character, double elapsed)
        {
            if (激活)
            {
                发送他们标记();
            }
        }

        public override void AfterDamageCalculation(Character character, Character enemy, double damage, double actualDamage, bool isNormalAttack, DamageType damageType, MagicType magicType, DamageResult damageResult)
        {
            if (character != Skill.Character || GamingQueue is null || damageType != DamageType.Magical || (damageResult != DamageResult.Normal && damageResult != DamageResult.Critical))
            {
                return;
            }
            Character[] characters = [.. GamingQueue.GetTeammates(character).Where(c => c.Effects.Any(e => e is 时雨标记 && e.Source == character))];
            if (characters.Length > 0)
            {
                WriteLine($"[ {character} ] 发动了开宫！");
                double heal = actualDamage * 0.15;
                foreach (Character target in characters)
                {
                    HealToTarget(character, target, heal);
                }
            }
        }

        public void 发送他们标记()
        {
            if (GamingQueue is null || Skill.Character is null)
            {
                return;
            }
            foreach (Character character in GamingQueue.Queue)
            {
                if (character == Skill.Character)
                {
                    continue;
                }
                if (!character.Effects.Any(e => e is 长期监视 && e.Source == Skill.Character))
                {
                    Effect e = new 长期监视(Skill, Skill.Character, character);
                    character.Effects.Add(e);
                    e.OnEffectGained(character);
                }
            }
        }
    }
}
