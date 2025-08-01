using System.Text;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Models;
using Oshima.FunGame.OshimaModules.Regions;
using Oshima.FunGame.OshimaServers.Service;

namespace Oshima.FunGame.OshimaServers.Model
{
    public class Cooperative
    {
        private static readonly Random _random = new();

        public static async Task RunCooperativeGame(List<string> msgs, Room room)
        {
            long[] userIds = [.. room.UserAndIsReady.Keys.Select(user => user.Id)];
            try
            {
                Dictionary<User, Character> characters = [];
                Dictionary<User, PluginConfig> pcs = [];
                try
                {
                    foreach (User user in room.UserAndIsReady.Keys)
                    {
                        PluginConfig pc = FunGameService.GetUserConfig(user.Id, out bool isTimeout);
                        if (isTimeout)
                        {
                            throw new Exception($"获取 {user.Username} 的库存信息超时。");
                        }
                        if (pc.Count > 0)
                        {
                            User userTemp = FunGameService.GetUser(pc);
                            pcs[userTemp] = pc;
                            characters[userTemp] = userTemp.Inventory.MainCharacter;
                        }
                        else
                        {
                            throw new Exception($"获取 {user.Username} 的库存信息失败。");
                        }
                    }
                }
                catch (Exception e)
                {
                    msgs.Add($"发生错误：{e.Message}");
                    return;
                }

                StringBuilder builder = new();
                builder.AppendLine("☆--- 共斗模式玩家列表 ---☆");
                builder.AppendLine($"房间号：{room.Roomid}");

                bool next = true;
                foreach (User user in characters.Keys)
                {
                    Character character = characters[user];
                    double hpPercent = character.HP / character.MaxHP * 100;
                    ISkill[] skills = [character.NormalAttack, .. character.Skills];
                    builder.AppendLine($"[ {user} ] 主战角色：{characters[user].ToStringWithLevelWithOutUser()}，核心属性：{CharacterSet.GetPrimaryAttributeName(character.PrimaryAttribute)}；" +
                        $"攻击力：{character.ATK:0.##}；当前生命值；{hpPercent:0.##}%；拥有技能：{string.Join("，", skills.Select(s => $"{s.Name}({s.Level})"))}。");
                    if (hpPercent < 10)
                    {
                        next = false;
                        builder.AppendLine($"[ {user} ] 的主战角色重伤未愈，当前生命值低于 10%，请先回复生命值或设置其他主战角色！推荐使用【生命之泉】指令快速治疗。");
                    }
                }

                msgs.Add(builder.ToString().Trim() + "\r\n");
                builder.Clear();

                if (next)
                {
                    // 生成敌人
                    OshimaRegion region = FunGameConstant.Regions.OrderBy(o => _random.Next()).First();
                    List<Character> enemys = [];
                    Character? enemy = null;
                    bool isUnit = Random.Shared.Next(2) != 0;
                    if (!isUnit)
                    {
                        enemy = region.Characters.OrderBy(o => Random.Shared.Next()).FirstOrDefault();
                        if (enemy != null)
                        {
                            enemy = enemy.Copy();
                            enemys.Add(enemy);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < Math.Max(1, characters.Count); i++)
                        {
                            enemy = region.Units.OrderBy(o => Random.Shared.Next()).FirstOrDefault();
                            if (enemy != null)
                            {
                                enemy = enemy.Copy();
                                int dcount = enemys.Count(e => e.Name.StartsWith(enemy.Name));
                                if (dcount > 0 && FunGameConstant.GreekAlphabet.Length > dcount) enemy.Name += FunGameConstant.GreekAlphabet[dcount - 1];
                                enemys.Add(enemy);
                            }
                        }
                    }
                    if (enemys.Count == 0)
                    {
                        msgs.Add("没有可用的敌人，返回游戏房间。");
                    }
                    else
                    {
                        Item[] weapons = [.. FunGameConstant.Equipment.Where(i => i.Id.ToString().StartsWith("11") && (int)i.QualityType == 5)];
                        Item[] armors = [.. FunGameConstant.Equipment.Where(i => i.Id.ToString().StartsWith("12") && (int)i.QualityType == 5)];
                        Item[] shoes = [.. FunGameConstant.Equipment.Where(i => i.Id.ToString().StartsWith("13") && (int)i.QualityType == 5)];
                        Item[] accessory = [.. FunGameConstant.Equipment.Where(i => i.Id.ToString().StartsWith("14") && (int)i.QualityType == 5)];
                        Item[] consumables = [.. FunGameConstant.AllItems.Where(i => i.ItemType == ItemType.Consumable && i.IsInGameItem)];
                        // 敌人为动态难度，根据玩家的等级生成
                        int cLevel = (int)characters.Values.Average(c => c.Level) + 5;
                        int sLevel = cLevel / 7;
                        int mLevel = cLevel / 9;
                        int naLevel = mLevel;
                        foreach (Character enemy_loop in enemys)
                        {
                            FunGameService.EnhanceBoss(enemy_loop, weapons, armors, shoes, accessory, consumables, cLevel, sLevel, mLevel, naLevel, false, false, isUnit);
                        }
                        // 开始战斗
                        Team team1 = new($"房间{room.Roomid}", characters.Values);
                        Team team2 = new($"{region.Name}", enemys);
                        FunGameActionQueue actionQueue = new();
                        List<string> gameMsgs = await actionQueue.StartTeamGame([team1, team2], showAllRound: true);
                        if (gameMsgs.Count > 15)
                        {
                            gameMsgs = gameMsgs[^15..];
                        }
                        msgs.AddRange(gameMsgs);

                        // 结算奖励
                        Character? mvp = actionQueue.GamingQueue.CharacterStatistics.OrderByDescending(d => d.Value.Rating).Select(d => d.Key).FirstOrDefault();
                        int award = cLevel;
                        if (enemys.All(e => e.HP == 0))
                        {
                            builder.AppendLine($"☆--- 战斗胜利奖励 ---☆");
                            builder.AppendLine($"击杀了全部敌人，所有玩家获得奖励：{award} {General.GameplayEquilibriumConstant.InGameMaterial} 和 {award} 点共斗积分。");
                            builder.AppendLine($"所有角色获得【生命之泉】奖励！状态已经回复至满。");
                            foreach (Character character in characters.Values)
                            {
                                character.Recovery();
                            }
                        }
                        else
                        {
                            builder.AppendLine($"☆--- 战斗失败奖励 ---☆");
                            int count = enemys.Count(e => e.HP == 0);
                            if (isUnit)
                            {
                                award = cLevel / enemys.Count * count;
                            }
                            else
                            {
                                award = 0;
                            }
                            if (count > 0)
                            {
                                builder.AppendLine($"击杀了 {count} 个敌人，所有玩家获得奖励：{award} {General.GameplayEquilibriumConstant.InGameMaterial} 和 {award} 点共斗积分。");
                            }
                            else
                            {
                                builder.AppendLine($"未能击杀任何敌人，无法获得奖励。");
                            }
                            builder.AppendLine($"角色进入重伤未愈状态，请所有玩家立即使用【生命之泉】指令回复状态！使用【酒馆】指令可以补满角色能量。");
                        }
                        foreach (User user in characters.Keys)
                        {
                            user.Inventory.Materials += award;
                            PluginConfig pc = pcs[user];
                            if (pc.TryGetValue("cooperativePoints", out object? value) && int.TryParse(value.ToString(), out int cooperativePoints))
                            {
                                pc.Add("cooperativePoints", cooperativePoints + award);
                            }
                            else
                            {
                                pc.Add("cooperativePoints", award);
                            }
                        }
                        if (mvp != null && characters.ContainsKey(mvp.User))
                        {
                            int mvpAward = cLevel / 5;
                            builder.AppendLine($"MVP 玩家 [ {mvp.User.Username} ] 额外获得奖励：{mvpAward} {General.GameplayEquilibriumConstant.InGameMaterial}。");
                            mvp.User.Inventory.Materials += mvpAward;
                        }
                        builder.AppendLine($"共斗积分可以在【共斗商店】中兑换物品。");
                        builder.AppendLine($"\r\n☆--- 战斗数据 ---☆");
                        int index = 1;
                        foreach (Character character in actionQueue.GamingQueue.CharacterStatistics.Where(kv => characters.ContainsValue(kv.Key)).
                            OrderByDescending(kv => kv.Value.Rating).Select(kv => kv.Key))
                        {
                            CharacterStatistics stats = actionQueue.GamingQueue.CharacterStatistics[character];
                            builder.AppendLine($"{index + ". "}[ {character.ToStringWithLevel()} ]");
                            builder.AppendLine($"技术得分：{stats.Rating:0.0#} / 击杀数：{stats.Kills} / 助攻数：{stats.Assists} / 死亡数：{stats.Deaths}");
                            builder.AppendLine($"存活时长：{stats.LiveTime:0.##} / 存活回合数：{stats.LiveRound} / 行动回合数：{stats.ActionTurn}");
                            builder.AppendLine($"控制时长：{stats.ControlTime:0.##} / 总计治疗：{stats.TotalHeal:0.##} / 护盾抵消：{stats.TotalShield:0.##}");
                            builder.AppendLine($"总计伤害：{stats.TotalDamage:0.##} / 总计物理伤害：{stats.TotalPhysicalDamage:0.##} / 总计魔法伤害：{stats.TotalMagicDamage:0.##}");
                            builder.AppendLine($"总承受伤害：{stats.TotalTakenDamage:0.##} / 总承受物理伤害：{stats.TotalTakenPhysicalDamage:0.##} / 总承受魔法伤害：{stats.TotalTakenMagicDamage:0.##}");
                            if (stats.TotalTrueDamage > 0 || stats.TotalTakenTrueDamage > 0) builder.AppendLine($"总计真实伤害：{stats.TotalTrueDamage:0.##} / 总承受真实伤害：{stats.TotalTakenTrueDamage:0.##}");
                            builder.AppendLine($"每秒伤害：{stats.DamagePerSecond:0.##} / 每回合伤害：{stats.DamagePerTurn:0.##}");
                            index++;
                        }
                        msgs.Add(builder.ToString().Trim());
                    }
                }
                else
                {
                    msgs.Add("返回游戏房间。");
                }

                // 存档
                foreach (User user in pcs.Keys)
                {
                    FunGameService.SetUserConfigButNotRelease(user.Id, pcs[user], user);
                }
            }
            catch (Exception e)
            {
                msgs.Add($"发生错误：{e.Message}");
            }
            finally
            {
                foreach (long id in userIds)
                {
                    FunGameService.ReleaseUserSemaphoreSlim(id);
                }
            }
        }
    }
}
