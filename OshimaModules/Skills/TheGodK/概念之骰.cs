using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 概念之骰 : Skill
    {
        public override long Id => (long)PassiveID.概念之骰;
        public override string Name => "概念之骰";
        public override string Description => Effects.Count > 0 ? ((概念之骰特效)Effects.First()).GeneralDescription : "";
        public int 力层数 { get; set; } = 0;
        public int 暴层数 { get; set; } = 0;
        public int 噬层数 { get; set; } = 0;
        public int 御层数 { get; set; } = 0;

        public 概念之骰(Character? character = null) : base(SkillType.Passive, character)
        {
            Effects.Add(new 概念之骰特效(this));
        }

        public override IEnumerable<Effect> AddPassiveEffectToCharacter()
        {
            return Effects;
        }

        public override void OnCharacterRespawn(Skill newSkill)
        {
            if (newSkill is 概念之骰 skill)
            {
                skill.力层数 = 力层数;
                skill.暴层数 = 暴层数;
                skill.噬层数 = 噬层数;
                skill.御层数 = 御层数;
            }
        }
    }

    public class 概念之骰特效 : Effect
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description { get; set; } = "";
        public string GeneralDescription => $"{Skill.SkillOwner()}的助攻窗口期永远不会过期。{Skill.SkillOwner()}每次参与击杀时都会进行一次概念投掷，获得一层永久持续的增益效果，且每个概念都最多可以叠加 {最多层数} 层。每次概念投掷可取得以下任意一项概念增益：\r\n" +
            $"「力」：攻击力提升 {攻击力提升 * 100:0.##}% [ {Skill.Character?.BaseATK * 攻击力提升:0.##} ]。\r\n「暴」：暴击率提升 {暴击率提升 * 100:0.##}%，暴击伤害提升 {暴击伤害提升 * 100:0.##}%。\r\n「噬」：生命偷取提升 {生命偷取提升 * 100:0.##}%。\r\n「御」：受到的伤害减少 {攻击力提升 * 100:0.##}%。\r\n" +
            $"取得第一层「噬」后，同时获得「恒」效果：当总生命偷取超过 {生命偷取阈值 * 100:0.##}% 后，超出部分每 {生命偷取每单位 * 100:0.##}% 转化为 {生命偷取转化 * 100:0.##}% 攻击力。";

        public static int 最多层数 => 8;
        public static double 攻击力提升 => 0.08;
        public static double 暴击率提升 => 0.04;
        public static double 暴击伤害提升 => 0.08;
        public static double 生命偷取提升 => 0.05;
        public static double 受到伤害减少 => 0.06;
        public static double 生命偷取阈值 => 0.3;
        public static double 生命偷取每单位 => 0.01;
        public static double 生命偷取转化 => 0.005;

        private double 实际攻击力提升 = 0;
        private double 实际暴击率提升 = 0;
        private double 实际暴击伤害提升 = 0;
        private double 实际生命偷取提升 = 0;
        private double 实际受到伤害减少 = 0;
        private double 实际生命偷取转化 = 0;

        public 概念之骰特效(Skill skill) : base(skill)
        {
            Description = GeneralDescription;
        }

        public override double AlterActualDamageAfterCalculation(Character character, Character enemy, double damage, bool isNormalAttack, DamageType damageType, MagicType magicType, DamageResult damageResult, ref bool isEvaded, Dictionary<Effect, double> totalDamageBonus)
        {
            if (enemy == Skill.Character && 实际受到伤害减少 > 0)
            {
                double reduce = damage * 实际受到伤害减少;
                WriteLine($"[ {Skill.Character} ] 发动了概念之骰！伤害减少了 {reduce:0.##} 点！");
                return -reduce;
            }
            return 0;
        }

        public override void AfterDeathCalculation(Character death, bool hasMaster, Character? killer, Dictionary<Character, int> continuousKilling, Dictionary<Character, int> earnedMoney, Character[] assists)
        {
            if (Skill.Character != null && death != Skill.Character && (killer == Skill.Character || assists.Contains(Skill.Character)) && Skill is 概念之骰 skill)
            {
                WriteLine($"[ {Skill.Character} ] 进行概念投掷：“此乃，神之概念。”");
                bool result = false;
                do
                {
                    switch (Random.Shared.Next(4))
                    {
                        case 0:
                            if (skill.力层数 < 最多层数)
                            {
                                skill.力层数++;
                                WriteLine($"[ {Skill.Character} ] 获得了「力」概念效果，当前「力」层数：{skill.力层数}。");
                                result = true;
                            }
                            break;
                        case 1:
                            if (skill.暴层数 < 最多层数)
                            {
                                skill.暴层数++;
                                WriteLine($"[ {Skill.Character} ] 获得了「暴」概念效果，当前「暴」层数：{skill.暴层数}。");
                                result = true;
                            }
                            break;
                        case 2:
                            if (skill.噬层数 < 最多层数)
                            {
                                skill.噬层数++;
                                WriteLine($"[ {Skill.Character} ] 获得了「噬」概念效果，当前「噬」层数：{skill.噬层数}。");
                                result = true;
                            }
                            break;
                        case 3:
                            if (skill.御层数 < 最多层数)
                            {
                                skill.御层数++;
                                WriteLine($"[ {Skill.Character} ] 获得了「御」概念效果，当前「御」层数：{skill.御层数}。");
                                result = true;
                            }
                            break;
                    }
                    if (skill.力层数 + skill.暴层数 + skill.噬层数 + skill.御层数 >= 最多层数 * 4)
                    {
                        WriteLine($"[ {Skill.Character} ] 已经完成了「概念之骰」的全部概念收集！！");
                        result = true;
                    }
                }
                while (!result);
                if (result)
                {
                    刷新技能效果(Skill.Character);
                }
            }
        }

        public override void OnEffectGained(Character character)
        {
            刷新技能效果(character);
        }

        public override void OnTimeElapsed(Character character, double elapsed)
        {
            刷新技能效果(character);
            if (GamingQueue != null && GamingQueue.AssistDetails.TryGetValue(character, out AssistDetail? ad) && ad != null)
            {
                foreach (Character enemy in ad.DamageLastTime.Keys)
                {
                    ad.DamageLastTime[enemy] = GamingQueue.TotalTime;
                }
                foreach (Character teammate in ad.NotDamageAssistLastTime.Keys)
                {
                    ad.NotDamageAssistLastTime[teammate] = GamingQueue.TotalTime;
                }
            }
        }

        public void 刷新技能效果(Character character)
        {
            if (实际攻击力提升 != 0)
            {
                character.ExATKPercentage -= 实际攻击力提升;
                实际攻击力提升 = 0;
            }
            if (实际暴击率提升 != 0)
            {
                character.ExCritRate -= 实际暴击率提升;
                实际暴击率提升 = 0;
            }
            if (实际暴击伤害提升 != 0)
            {
                character.ExCritDMG -= 实际暴击伤害提升;
                实际暴击伤害提升 = 0;
            }
            if (实际生命偷取提升 != 0)
            {
                character.Lifesteal -= 实际生命偷取提升;
                实际生命偷取提升 = 0;
            }
            if (实际受到伤害减少 != 0)
            {
                实际受到伤害减少 = 0;
            }
            if (实际生命偷取转化 != 0)
            {
                character.ExATKPercentage -= 实际生命偷取转化;
                实际生命偷取转化 = 0;
            }
            if (Skill is 概念之骰 skill)
            {
                实际攻击力提升 = 攻击力提升 * skill.力层数;
                实际暴击率提升 = 暴击率提升 * skill.暴层数;
                实际暴击伤害提升 = 暴击伤害提升 * skill.暴层数;
                实际生命偷取提升 = 生命偷取提升 * skill.噬层数;
                实际受到伤害减少 = 受到伤害减少 * skill.御层数;
                character.ExATKPercentage += 实际攻击力提升;
                character.ExCritRate += 实际暴击率提升;
                character.ExCritDMG += 实际暴击伤害提升;
                character.Lifesteal += 实际生命偷取提升;
                if (skill.噬层数 > 0 && character.Lifesteal > 生命偷取阈值)
                {
                    double 超出部分 = character.Lifesteal - 生命偷取阈值;
                    实际生命偷取转化 = 超出部分 / 生命偷取每单位 * 生命偷取转化;
                }
                character.ExATKPercentage += 实际生命偷取转化;
                Description = $"{Skill.SkillOwner()}的助攻窗口期永远不会过期。{Skill.SkillOwner()}每次参与击杀时都会进行一次概念投掷，获得一层永久持续的增益效果，且每个概念都最多可以叠加 {最多层数} 层。（当前层数：「力」× {skill.力层数}、「暴」× {skill.暴层数}、「噬」× {skill.噬层数}、「御」× {skill.御层数}，攻击力提升：{(实际攻击力提升 + 实际生命偷取转化) * 100:0.##}% [ {character.BaseATK * (实际攻击力提升 + 实际生命偷取转化):0.##} ] 点，暴击率提升：{实际暴击率提升 * 100:0.##}%，暴击伤害提升：{实际暴击伤害提升 * 100:0.##}%，生命偷取提升：{实际生命偷取提升 * 100:0.##}%，受到伤害减少：{实际受到伤害减少 * 100:0.##}%）";
            }
        }
    }
}
