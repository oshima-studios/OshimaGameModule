using System.Text;
using Milimoe.FunGame.Core.Entity;
using Oshima.FunGame.OshimaModules.Models;

namespace Oshima.FunGame.OshimaServers.Model
{
    public class HorseRacing
    {
        private const int MaxTurns = 100;
        private static readonly Random _random = new();

        public static Dictionary<long, int> RunHorseRacing(List<string> msgs, Room room)
        {
            Dictionary<long, int> userPoints = [];

            StringBuilder builder = new();
            builder.AppendLine("☆--- 参赛选手 ---☆");

            List<Horse> horses = [];
            foreach (User user in room.UserAndIsReady.Keys)
            {
                if (FunGameConstant.UserIdAndUsername.TryGetValue(user.Id, out User? temp) && temp != null)
                {
                    user.Username = temp.Username;
                }
                Horse horse = new(user);
                AssignRandomSkills(horse);
                horses.Add(horse);
                builder.AppendLine($"[ {horse} ] 已准备就绪！初始步数: {horse.Step}, 生命值: {horse.HP}, 每回合恢复生命值: {horse.HPRecovery}");
                if (horse.Skills.Count != 0)
                {
                    builder.AppendLine($"[ {horse} ] 拥有技能: {string.Join("，", horse.Skills.Select(s => $"{s.Name}（持续 {s.Duration} 回合）"))}");
                }
            }

            builder.AppendLine("\r\n☆--- 比赛开始！ ---☆");

            int maxLength = _random.Next(8, 16);
            builder.AppendLine($"本次抽取赛道长度：{maxLength} 步！");

            for (int turn = 1; turn <= MaxTurns; turn++)
            {
                builder.AppendLine($"\r\n\r\n☆--- 第 {turn} 回合 ---☆");
                bool raceFinished = false;
                Dictionary<Horse, int> turnSteps = [];
                Dictionary<Horse, Dictionary<HorseSkill, Horse>> turnSkills = [];

                // 先触发技能，后面一起结算，不然技能下个回合才能触发
                foreach (Horse horse in horses)
                {
                    turnSkills.TryAdd(horse, []);
                    if (horse.HP == 0) continue;
                    // 触发永久技能
                    foreach (HorseSkill skill in horse.Skills)
                    {
                        if (_random.NextDouble() < skill.CastProbability)
                        {
                            Horse target = horse;
                            if (skill.ToEnemy)
                            {
                                target = horses.OrderBy(o => _random.Next(horses.Count)).First(h => h != horse);
                            }
                            turnSkills[horse].Add(skill, target);
                            target.ActiveEffects.Add(new ActiveSkillEffect(skill));
                        }
                    }
                }

                List<string> winners = [];
                foreach (Horse horse in horses)
                {
                    turnSteps[horse] = 0;
                    int effectiveStep = horse.Step; // 从基础步数开始计算
                    int effectiveHPRecovery = horse.HPRecovery; // 从基础HP恢复开始计算
                    List<string> turnEvents = []; // 记录本回合发生的事件

                    // 触发永久技能
                    Dictionary<HorseSkill, Horse> skills = turnSkills[horse];
                    foreach (HorseSkill skill in skills.Keys)
                    {
                        Horse target = skills[skill];
                        // 如果是敌人
                        if (skill.Duration > 0)
                        {
                            turnEvents.Add($"✨ 发动了 [ {skill.Name} ]，持续 {skill.Duration} 回合！{(skill.ToEnemy ? $"[ {target} ] " : "")}{skill}");
                        }
                        else
                        {
                            turnEvents.Add($"✨ 发动了 [ {skill.Name} ]！{(skill.ToEnemy ? $"[ {target} ] " : "")}{skill}");
                        }
                    }

                    // 处理正在生效的持续技能效果
                    List<ActiveSkillEffect> expiredEffects = [];
                    foreach (ActiveSkillEffect activeEffect in horse.ActiveEffects)
                    {
                        HorseSkill skill = activeEffect.Skill;
                        // 应用持续效果
                        effectiveStep += skill.AddStep - skill.ReduceStep;
                        effectiveHPRecovery += skill.AddHR - skill.ReduceHR;
                        horse.HP += skill.AddHP - skill.ReduceHP;
                        horse.CurrentPosition += skill.ChangePosition;
                        turnSteps[horse] += skill.ChangePosition;

                        Horse? source = skill.Horse;
                        if (source != null && source != horse) turnEvents.Add($"💥 受到了 [ {skill.Name}（来自：{source}）] 的影响，{skill}（剩余 {activeEffect.RemainDuration} 回合）");
                        else turnEvents.Add($"💥 受到了 [ {skill.Name} ] 的影响，{skill}（剩余 {activeEffect.RemainDuration} 回合）");

                        activeEffect.RemainDuration--;
                        if (activeEffect.RemainDuration <= 0)
                        {
                            expiredEffects.Add(activeEffect);
                        }
                    }
                    // 移除已结束的持续效果
                    foreach (ActiveSkillEffect expiredEffect in expiredEffects)
                    {
                        horse.ActiveEffects.Remove(expiredEffect);
                    }

                    // 随机事件
                    if (_random.NextDouble() < 0.3)
                    {
                        HorseSkill eventSkill = GenerateRandomEventSkill();
                        // 随机事件技能也可能持续多回合
                        if (eventSkill.Duration > 0)
                        {
                            horse.ActiveEffects.Add(new ActiveSkillEffect(eventSkill));
                            turnEvents.Add($"💥 遭遇随机事件 [ {eventSkill.Name} ]，持续 {eventSkill.Duration} 回合！{eventSkill}");
                        }
                        else
                        {
                            // 如果没有持续时间（Duration=0），则立即应用效果
                            effectiveStep += eventSkill.AddStep - eventSkill.ReduceStep;
                            effectiveHPRecovery += eventSkill.AddHR - eventSkill.ReduceHR;
                            horse.HP += eventSkill.AddHP - eventSkill.ReduceHP;
                            turnEvents.Add($"💥 遭遇随机事件 [ {eventSkill.Name} ]！{eventSkill}");
                        }
                    }

                    // 恢复HP，基于计算后的有效HP恢复值
                    int hp = horse.HP;
                    horse.HP += effectiveHPRecovery;
                    if (hp != horse.HP)
                    {
                        turnEvents.Add($"❤️ 生命值恢复至 {horse.HP} 点（+{effectiveHPRecovery}）。");
                    }

                    if (horse.HP <= 0)
                    {
                        turnSteps[horse] = 0;
                        if (turnEvents.Count != 0)
                        {
                            builder.AppendLine($"[ {horse} ] {string.Join("；", turnEvents)}");
                        }
                        continue;
                    }

                    // 确保步数不为负
                    effectiveStep = Math.Max(0, effectiveStep);
                    horse.CurrentPosition += effectiveStep; // 移动

                    turnSteps[horse] += effectiveStep;
                    //if (effectiveStep > 1) builder.AppendLine($"[ {horse} ] 移动了 {effectiveStep} 步！");
                    if (turnEvents.Count != 0)
                    {
                        builder.AppendLine($"[ {horse} ] {string.Join("；", turnEvents)}");
                    }

                    if (horse.CurrentPosition >= maxLength)
                    {
                        winners.Add(horse.Name);
                        builder.AppendLine($"\r\n🎯 [ {horse} ] 冲过终点线！\r\n");
                        raceFinished = true;
                    }
                }

                builder.AppendLine("\r\n☆--- 赛道状况 ---☆");
                for (int i = 0; i < horses.Count; i++)
                {
                    builder.AppendLine(GenerateTrackString(horses[i], i + 1, maxLength, turnSteps));
                }

                if (raceFinished)
                {
                    builder.AppendLine($"\r\n🎯 赢家：[ {string.Join(" ] / [ ", winners)} ]！");
                    msgs.Add($"{builder.ToString().Trim()}\r\n");
                    builder.Clear();
                    break;
                }

                msgs.Add($"{builder.ToString().Trim()}\r\n");
                builder.Clear();
            }

            builder.Clear();
            builder.AppendLine("☆--- 比赛结果 ---☆");
            List<Horse> finalRanking = [.. horses.OrderByDescending(h => h.CurrentPosition)];
            int rank = 1;
            int horsesAtCurrentRankCount = 0; // 当前名次有多少匹马
            int lastPosition = -1; // 上一匹马的位置
            for (int i = 0; i < finalRanking.Count; i++)
            {
                Horse currentHorse = finalRanking[i];

                if (i == 0)
                {
                    rank = 1;
                    lastPosition = currentHorse.CurrentPosition;
                    horsesAtCurrentRankCount = 1;
                }
                else if (currentHorse.CurrentPosition == lastPosition || currentHorse.CurrentPosition >= maxLength) // 与前一匹马平局或都是冠军
                {
                    horsesAtCurrentRankCount++;
                }
                else
                {
                    // 新的名次是当前名次加上之前平局的马匹数量
                    rank += horsesAtCurrentRankCount;
                    lastPosition = currentHorse.CurrentPosition;
                    horsesAtCurrentRankCount = 1;
                }

                // 根据实际名次计算积分
                int points = CalculatePointsForRank(rank);

                userPoints[currentHorse.Id] = points;
                builder.AppendLine($"{(horsesAtCurrentRankCount > 1 ? "并列" : "")}第 {rank} 名：{currentHorse.Name}（获得 {points} 点赛马积分）");
            }

            builder.AppendLine("\r\n比赛结束，奖励将在稍后发放！");
            msgs.Add($"\r\n{builder.ToString().Trim()}");

            return userPoints;
        }

        private static int CalculatePointsForRank(int rank)
        {
            if (rank <= 0) return 0;
            if (rank == 1) return 10;

            int points = 10;
            for (int r = 2; r <= rank; r++)
            {
                points = (int)(points * 0.8);
                if (points == 0) points = 1;
            }
            return points;
        }

        private static void AssignRandomSkills(Horse horse)
        {
            // 技能池
            List<HorseSkill> skillPool = [
                new() { Name = "冲刺", AddStep = 2, CastProbability = 0.3, Duration = 1 }, // 普通冲刺，1回合
                new() { Name = "耐力爆发", AddHP = 2, CastProbability = 0.3, Duration = 1 }, // 瞬间回血
                new() { Name = "神行百变", AddStep = 1, AddHR = 1, CastProbability = 0.3, Duration = 3 }, // 持续3回合的加速和恢复
                new() { Name = "稳健", AddStep = 0, AddHR = 1, CastProbability = 0.3, Duration = 2 }, // 持续2回合的额外恢复
                new() { Name = "疾风步", AddStep = 1, CastProbability = 0.15, Duration = 2 }, // 强大的2回合加速
                new() { Name = "疲惫", ToEnemy = true, ReduceStep = 1, CastProbability = 0.4, Duration = 2 }, // 持续2回合的减速
                new() { Name = "摔跤", ToEnemy = true, ReduceHP = 2, ReduceStep = 1, CastProbability = 0.4, Duration = 1 }, // 瞬间掉血减速
                new() { Name = "干扰", ToEnemy = true, ReduceStep = 2, CastProbability = 0.3, Duration = 1 }, // 瞬间减速
                new() { Name = "石头攻击", ToEnemy = true, ReduceHP = 3, CastProbability = 0.4, Duration = 1 }, // 瞬间掉血
                new() { Name = "台风", ToEnemy = true, ReduceHP = 2, CastProbability = 0.4, Duration = 2 }, // 强大的2回合掉血
                new() { Name = "死刑宣告", ToEnemy = true, ReduceHP = 2, ReduceHR = 1, CastProbability = 0.3, Duration = 2 }, // 瞬间掉血+重伤
                new() { Name = "后退吧！", ToEnemy = true, ChangePosition = -2, CastProbability = 0.3, Duration = 1 }, // 直接改变位置
            ];

            int skillsToAssign = _random.Next(1, 4);

            for (int i = 0; i < skillsToAssign; i++)
            {
                HorseSkill chosenSkill = skillPool[_random.Next(skillPool.Count)];
                while (true)
                {
                    if (!horse.Skills.Any(s => s.Name == chosenSkill.Name))
                    {
                        break;
                    }
                    // 如果技能已存在，重新选择
                    chosenSkill = skillPool[_random.Next(skillPool.Count)];
                }
                horse.Skills.Add(new HorseSkill
                {
                    Horse = horse,
                    Name = chosenSkill.Name,
                    ToEnemy = chosenSkill.ToEnemy,
                    AddStep = chosenSkill.AddStep,
                    ReduceStep = chosenSkill.ReduceStep,
                    AddHP = chosenSkill.AddHP,
                    ReduceHP = chosenSkill.ReduceHP,
                    AddHR = chosenSkill.AddHR,
                    ReduceHR = chosenSkill.ReduceHR,
                    ChangePosition = chosenSkill.ChangePosition,
                    CastProbability = chosenSkill.CastProbability,
                    Duration = chosenSkill.Duration
                });
            }
        }

        private static HorseSkill GenerateRandomEventSkill()
        {
            // 随机事件
            List<HorseSkill> eventPool = [
                new() { Name = "加速带", AddStep = 3, Duration = 1 }, // 瞬间加速
                new() { Name = "泥泞区", ReduceStep = 2, Duration = 2 }, // 持续2回合减速
                new() { Name = "观众欢呼", AddHP = 2, Duration = 1 }, // 瞬间回血
                new() { Name = "小石子", ReduceHP = 1, Duration = 1 }, // 瞬间掉血
                new() { Name = "顺风", AddStep = 1, Duration = 3 }, // 持续3回合微加速
                new() { Name = "逆风", ReduceStep = 1, Duration = 2 }, // 持续2回合微减速
                new() { Name = "兴奋剂", AddStep = 2, Duration = 4, ReduceHP = 2 } // 兴奋剂，有副作用
            ];

            return eventPool[_random.Next(eventPool.Count)];
        }

        private static string GenerateTrackString(Horse horse, int trackNumber, int maxLength, Dictionary<Horse, int> turnSteps)
        {
            StringBuilder builder = new();

            builder.Append($"[{trackNumber}]|");

            if (horse.CurrentPosition < 0) horse.CurrentPosition = 0;
            int dashesBeforeHorse = Math.Min(horse.CurrentPosition, maxLength);
            int dashesAfterHorse = Math.Max(0, maxLength - horse.CurrentPosition);
            builder.Append(new string('=', dashesAfterHorse));

            string horseMarker = $"<{horse}({horse.HP})>";
            if (horse.ActiveEffects.Count > 0 || horse.HP == 0)
            {
                if (horse.HP == 0)
                {
                    horseMarker = $"[💀死亡]<{horse}>";
                }
                if (horse.ActiveEffects.Count > 0)
                {
                    horseMarker += $"[{string.Join("][", horse.ActiveEffects.Select(e => e.Skill.Name))}]";
                }
            }

            builder.Append(horseMarker);
            builder.Append(new string('=', dashesBeforeHorse));

            int turnStep = 0;
            if (turnSteps.TryGetValue(horse, out int step))
            {
                turnStep = step;
            }
            builder.Append($"|({horse.CurrentPosition})({(turnStep >= 0 ? "+" : "")}{turnStep})");

            return builder.ToString();
        }
    }
}
