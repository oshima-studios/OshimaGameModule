using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;

namespace Oshima.FunGame.OshimaModules.Effects.PassiveEffects
{
    public class 长期监视 : Effect
    {
        public override long Id => (long)PassiveEffectID.长期监视;
        public override string Name => "长期监视";
        public override string Description => $"此角色正在被长期监视。来自：[ {Source} ]";
        public override EffectType EffectType => EffectType.Mark;
        public override bool IsDebuff => true;
        public override bool DurativeWithoutDuration => true;
        public override Character Source => _sourceCharacter;
        public override DispelledType DispelledType => DispelledType.CannotBeDispelled;

        private readonly Character _sourceCharacter;
        private readonly Character _targetCharacter;

        public 长期监视(Skill skill, Character sourceCharacter, Character targetCharacter) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            _sourceCharacter = sourceCharacter;
            _targetCharacter = targetCharacter;
        }

        public CharacterActionType LastType { get; set; } = CharacterActionType.None;
        public Skill? LastSkill { get; set; } = null;

        public override void OnCharacterActionStart(Character actor, DecisionPoints dp, CharacterActionType type)
        {
            if (type == CharacterActionType.NormalAttack)
            {
                LastType = type;
            }
        }

        public override bool BeforeSkillCasted(Character caster, Skill skill, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            LastType = CharacterActionType.CastSkill;
            LastSkill = skill;
            return true;
        }

        public override void AfterDeathCalculation(Character death, bool hasMaster, Character? killer, Dictionary<Character, int> continuousKilling, Dictionary<Character, int> earnedMoney, Character[] assists)
        {
            if (GamingQueue != null && !hasMaster && killer != null && killer == _targetCharacter && Source != null && death != Source && GamingQueue.Queue.Contains(Source))
            {
                WriteLine($"[ {Source} ] 正在观察 [ {killer} ] 的情绪。");
                if (LastType == CharacterActionType.NormalAttack)
                {
                    Source.NormalAttack.SetMagicType(new(Skill.Effects.First(), true, MagicType, 999), GamingQueue);
                    Effect e = new IgnoreEvade(Skill, new()
                    {
                        { "p", 1 }
                    }, Source)
                    {
                        Name = Name,
                        Durative = false,
                        DurationTurn = 3,
                        RemainDurationTurn = 3
                    };
                    e.OnEffectGained(Source);
                    Source.Effects.Add(e);
                    WriteLine($"[ {Source} ] 获得了无视闪避效果，持续 3 回合！");
                }
                else if (LastType == CharacterActionType.CastSkill && LastSkill != null)
                {
                    复制技能 e = new(Skill, Source, LastSkill)
                    {
                        Durative = false,
                        DurationTurn = 3,
                        RemainDurationTurn = 3
                    };
                    e.CopiedSkill.Values[nameof(时雨标记)] = 1;
                    e.CopiedSkill.CurrentCD = 0;
                    e.CopiedSkill.FreeCostEP = true;
                    e.CopiedSkill.FreeCostMP = true;
                    e.CopiedSkill.Enable = true;
                    e.CopiedSkill.IsInEffect = false;
                    e.OnEffectGained(Source);
                    Source.Effects.Add(e);
                    WriteLine($"[ {Source} ] 复制了 [ {killer} ] 的技能：{LastSkill.Name}！！");
                }
                if (killer.Effects.FirstOrDefault(e => e is 时雨标记 && e.Source == Source) is 时雨标记 e2)
                {
                    e2.RemainDurationTurn = 3;
                }
                else
                {
                    e2 = new 时雨标记(Skill, Source)
                    {
                        Durative = false,
                        DurationTurn = 3,
                        RemainDurationTurn = 3
                    };
                    e2.OnEffectGained(killer);
                    killer.Effects.Add(e2);
                    WriteLine($"[ {Source} ] 给予了 [ {killer} ] 时雨标记！");
                }
            }
        }
    }
}
