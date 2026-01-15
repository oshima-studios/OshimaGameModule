using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model;
using Oshima.FunGame.OshimaModules.Effects.PassiveEffects;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 放监 : Skill
    {
        public override long Id => (long)SuperSkillID.放监;
        public override string Name => "放监";
        public override string Description => Effects.Count > 0 ? ((放监特效)Effects.First()).通用描述 : "";
        public override string DispelDescription => Effects.Count > 0 ? Effects.First().DispelDescription : "";
        public override double EPCost => 100;
        public override double CD => 65;
        public override double HardnessTime { get; set; } = 14;
        public override bool CanSelectSelf => true;
        public override bool CanSelectEnemy => false;
        public 放监被动 Passive { get; set; }

        public 放监(Character? character = null) : base(SkillType.SuperSkill, character)
        {
            Effects.Add(new 放监特效(this));
            Passive = new 放监被动(this);
        }

        public override IEnumerable<Effect> AddPassiveEffectToCharacter()
        {
            return [Passive];
        }
    }

    public class 放监被动(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"{Skill.SkillOwner()}阻止任何来自持有 [ 宫监手标记 ] 的角色所发起的指向性技能攻击。";
        public override bool DurativeWithoutDuration => true;
        public override DispelledType DispelledType => DispelledType.CannotBeDispelled;

        public override bool BeforeSkillCasted(Character caster, Skill skill, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            if (Skill.Character != null && caster.Effects.FirstOrDefault(e => e is 宫监手标记) is 宫监手标记 effect)
            {
                WriteLine($"[ {Skill.Character} ] 高声呼喊：“宫监手，放监！”");
                复制技能 e = new(Skill, Skill.Character, skill)
                {
                    Durative = false,
                    DurationTurn = 4,
                    RemainDurationTurn = 4
                };
                e.CopiedSkill.Values[nameof(时雨标记)] = 1;
                e.CopiedSkill.CurrentCD = 0;
                e.CopiedSkill.Enable = true;
                e.CopiedSkill.IsInEffect = false;
                e.OnEffectGained(Skill.Character);
                Skill.Character.Effects.Add(e);
                WriteLine($"[ {Skill.Character} ] 复制了 [ {caster} ] 的技能：{skill.Name}！！");
                effect.指向性技能任务完成(caster);
                // 返回 false 让框架处理阻止技能对该角色的释放
                return false;
            }
            return true;
        }
    }

    public class 放监特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => 通用描述;
        public override DispelledType DispelledType => DispelledType.CannotBeDispelled;

        public string 通用描述 => $"使场上现有的时雨标记变得不可驱散，并且刷新为持续 3 回合。并给予持有时雨标记的敌方角色 [ 宫监手标记 ]，宫监手标记不可驱散，持续 3 回合。{任务要求}";
        public string 任务要求 => $"持有宫监手标记的角色，必须完成以下两个任务以消除标记，否则将在标记消失时，每个未完成的任务给予角色基于{Skill.SkillOwner()} {核心属性系数 * 100:0.##}% 核心属性 + {攻击力系数 * 100:0.##}% 攻击力 [ {Skill.Character?.PrimaryAttributeValue * 核心属性系数 + Skill.Character?.ATK * 攻击力系数:0.##} ] 的真实伤害：\r\n" +
            $"1. 使用 [ 普通攻击 ] 攻击一次队友，此伤害必定暴击且无视闪避；\r\n2. 对{Skill.SkillOwner()}释放一个指向性技能，{Skill.SkillOwner()}将此技能效果无效化并且复制该技能获得使用权持续 4 回合。\r\n注意：在宫监手标记被消除前，对{Skill.SkillOwner()}释放指向性技能始终会触发无效化和复制效果。杀死{Skill.SkillOwner()}可以终止所有放监任务。";
        public double 核心属性系数 => 0.7 * Skill.Level;
        public double 攻击力系数 => 0.2 + 0.10 * (Skill.Level - 1);

        public void 造成伤害(Character character, int count)
        {
            if (Skill.Character != null)
            {
                WriteLine($"[ {character} ] 未完成「放监」任务！");
                for (int i = 0; i < count; i++)
                {
                    double damage = Skill.Character.PrimaryAttributeValue * 核心属性系数 + Skill.Character.ATK * 攻击力系数;
                    DamageToEnemy(Skill.Character, character, DamageType.True, MagicType, damage);
                }
            }
        }

        public override void OnSkillCasted(Character caster, List<Character> targets, List<Grid> grids, Dictionary<string, object> others)
        {
            if (GamingQueue != null)
            {
                List<Character> enemies = GamingQueue.GetEnemies(caster);
                foreach (Character character in GamingQueue.Queue)
                {
                    if (character.Effects.FirstOrDefault(e => e is 时雨标记) is 时雨标记 e)
                    {
                        e.DispelledType = DispelledType.CannotBeDispelled;
                        e.RemainDurationTurn = 3;
                        if (enemies.Contains(character))
                        {
                            Effect e2 = new 宫监手标记(Skill, caster, character, this)
                            {
                                Durative = false,
                                DurationTurn = 3,
                                RemainDurationTurn = 3
                            };
                            character.Effects.Add(e2);
                            e2.OnEffectGained(character);
                        }
                    }
                }
                GamingQueue.LastRound.AddApplyEffects(caster, EffectType.Focusing);
            }
        }
    }
}
