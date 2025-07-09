using System.Text;
using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.Core.Constant;
using Oshima.FunGame.OshimaModules.Characters;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;
using Oshima.FunGame.OshimaModules.Items;
using Oshima.FunGame.OshimaModules.Regions;
using Oshima.FunGame.OshimaModules.Skills;
using Oshima.FunGame.OshimaModules.Units;
using Oshima.FunGame.OshimaServers.Model;
using ProjectRedbud.FunGame.SQLQueryExtension;

namespace Oshima.FunGame.OshimaServers.Service
{
    public class FunGameService
    {
        public static Dictionary<long, List<string>> UserExploreCharacterCache { get; } = [];
        public static Dictionary<long, List<string>> UserExploreItemCache { get; } = [];
        public static Dictionary<long, List<string>> UserExploreEventCache { get; } = [];
        public static HashSet<Activity> Activities { get; } = [];
        public static List<string> ActivitiesCharacterCache { get; } = [];
        public static List<string> ActivitiesItemCache { get; } = [];
        public static List<string> ActivitiesEventCache { get; } = [];
        public static Dictionary<long, HashSet<string>> UserNotice { get; } = [];
        public static Dictionary<int, Character> Bosses { get; } = [];
        public static ServerPluginLoader? ServerPluginLoader { get; set; } = null;
        public static WebAPIPluginLoader? WebAPIPluginLoader { get; set; } = null;

        public static void InitFunGame()
        {
            FunGameConstant.Characters.Add(new OshimaShiya());
            FunGameConstant.Characters.Add(new XinYin());
            FunGameConstant.Characters.Add(new Yang());
            FunGameConstant.Characters.Add(new NanGanYu());
            FunGameConstant.Characters.Add(new NiuNan());
            FunGameConstant.Characters.Add(new DokyoMayor());
            FunGameConstant.Characters.Add(new MagicalGirl());
            FunGameConstant.Characters.Add(new QingXiang());
            FunGameConstant.Characters.Add(new QWQAQW());
            FunGameConstant.Characters.Add(new ColdBlue());
            FunGameConstant.Characters.Add(new dddovo());
            FunGameConstant.Characters.Add(new Quduoduo());

            FunGameConstant.Skills.AddRange([new 疾风步()]);

            FunGameConstant.SuperSkills.AddRange([new 嗜血本能(), new 平衡强化(), new 绝对领域(), new 精准打击(), new 三重叠加(), new 变幻之心(), new 力量爆发(), new 能量毁灭(), new 血之狂欢(), new 迅捷之势(), new 天赐之力(), new 魔法涌流()]);

            FunGameConstant.PassiveSkills.AddRange([new META马(), new 心灵之火(), new 魔法震荡(), new 灵能反射(), new 智慧与力量(), new 致命打击(), new 毁灭之势(), new 枯竭打击(), new 破釜沉舟(), new 累积之压(), new 敏捷之刃(), new 弱者猎手()]);

            FunGameConstant.Magics.AddRange([new 冰霜攻击(), new 火之矢(), new 水之矢(), new 风之轮(), new 石之锤(), new 心灵之霞(), new 次元上升(), new 暗物质(),
                new 回复术(), new 治愈术(), new 复苏术(), new 圣灵术(), new 时间加速(), new 时间减速(), new 反魔法领域(), new 沉默十字(), new 虚弱领域(), new 混沌烙印(), new 凝胶稠絮(),
                new 大地之墙(), new 盖亚之盾(), new 风之守护(), new 结晶防护(), new 强音之力(), new 神圣祝福(), new 根源屏障(), new 灾难冲击波(), new 银色荆棘(), new 等离子之波(),
                new 地狱之门(), new 钻石星尘(), new 死亡咆哮(), new 鬼魅之痛(), new 导力停止(), new 冰狱冥嚎(), new 火山咆哮(), new 水蓝轰炸(), new 岩石之息(), new 弧形日珥(), new 苍白地狱(), new 破碎虚空(),
                new 弧光消耗(), new 回复术改(), new 回复术复(), new 治愈术复(), new 风之守护复(), new 强音之力复(), new 结晶防护复(), new 神圣祝福复(), new 时间加速改(), new 时间减速改(), new 时间加速复(), new 时间减速复()]);

            Dictionary<string, Item> exItems = Factory.GetGameModuleInstances<Item>(OshimaGameModuleConstant.General, OshimaGameModuleConstant.Item);
            FunGameConstant.Equipment.AddRange(exItems.Values.Where(i => (int)i.ItemType >= 0 && (int)i.ItemType < 5));
            FunGameConstant.Equipment.AddRange([new 攻击之爪10(), new 攻击之爪25(), new 攻击之爪40(), new 攻击之爪55(), new 攻击之爪70(), new 攻击之爪85()]);

            FunGameConstant.Items.AddRange(exItems.Values.Where(i => (int)i.ItemType > 4));
            FunGameConstant.Items.AddRange([new 小经验书(), new 中经验书(), new 大经验书(), new 升华之印(), new 流光之印(), new 永恒之印(), new 技能卷轴(), new 智慧之果(), new 奥术符文(), new 混沌之核(),
                new 小回复药(), new 中回复药(), new 大回复药(), new 魔力填充剂1(), new 魔力填充剂2(), new 魔力填充剂3(), new 能量饮料1(), new 能量饮料2(), new 能量饮料3(), new 年夜饭(), new 蛇年大吉(), new 新春快乐(), new 毕业礼包(),
                new 复苏药1(), new 复苏药2(), new 复苏药3(), new 全回复药(), new 魔法卡礼包(), new 奖券(), new 十连奖券()
            ]);

            FunGameConstant.AllItems.AddRange(FunGameConstant.Equipment);
            FunGameConstant.AllItems.AddRange(FunGameConstant.Items);

            foreach (OshimaRegion region in FunGameConstant.Regions)
            {
                List<Item> items = [.. region.Crops.Select(i => i.Copy())];
                FunGameConstant.ExploreItems.Add(region, items);
            }

            FunGameConstant.DrawCardItems.AddRange(FunGameConstant.AllItems.Where(i => !FunGameConstant.ItemCanNotDrawCard.Contains(i.ItemType)));

            FunGameConstant.AllItems.AddRange(FunGameConstant.ExploreItems.Values.SelectMany(list => list));

            foreach (OshimaRegion region in FunGameConstant.Regions)
            {
                IEnumerable<Item> items = FunGameConstant.AllItems.Where(i => i.Others.TryGetValue("region", out object? value) && int.TryParse(value.ToString(), out int rid) && rid == region.Id).Select(i => i.Copy()).OrderByDescending(i => i.QualityType);
                foreach (Item item in items)
                {
                    region.Items.Add(item);
                }
            }
            
            foreach (OshimaRegion region in FunGameConstant.PlayerRegions)
            {
                IEnumerable<Item> items;
                if (region.Id == 0)
                {
                    items = FunGameConstant.Equipment.Where(i => !i.Others.ContainsKey("region")).Select(i => i.Copy()).OrderByDescending(i => i.QualityType);
                }
                else
                {
                    items = FunGameConstant.AllItems.Where(i => i.Others.TryGetValue("region", out object? value) && int.TryParse(value.ToString(), out int rid) && rid == region.Id).Select(i => i.Copy()).OrderByDescending(i => i.QualityType);
                }
                foreach (Item item in items)
                {
                    region.Items.Add(item);
                }
            }

            Skill?[] activeSkills = [.. FunGameConstant.Equipment.Select(i => i.Skills.Active), .. FunGameConstant.Items.Select(i => i.Skills.Active)];
            foreach (Skill? skill in activeSkills)
            {
                if (skill != null)
                {
                    FunGameConstant.ItemSkills.Add(skill);
                }
            }
            FunGameConstant.ItemSkills.AddRange([.. FunGameConstant.Equipment.SelectMany(i => i.Skills.Passives), .. FunGameConstant.Items.SelectMany(i => i.Skills.Passives)]);

            FunGameConstant.AllSkills.AddRange(FunGameConstant.Magics);
            FunGameConstant.AllSkills.AddRange(FunGameConstant.Skills);
            FunGameConstant.AllSkills.AddRange(FunGameConstant.PassiveSkills);
            FunGameConstant.AllSkills.AddRange(FunGameConstant.ItemSkills);
            FunGameConstant.AllSkills.AddRange(FunGameConstant.SuperSkills);
        }

        public static List<Item> GenerateMagicCards(int count, QualityType? qualityType = null)
        {
            List<Item> items = [];

            for (int i = 0; i < count; i++)
            {
                items.Add(GenerateMagicCard(qualityType));
            }

            return items;
        }

        public static Item GenerateMagicCard(QualityType? qualityType = null)
        {
            Item item = Factory.GetItem();
            item.Id = Convert.ToInt64("16" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 8));
            item.Name = GenerateRandomChineseName();
            item.ItemType = ItemType.MagicCard;
            item.RemainUseTimes = 1;

            int total;
            if (qualityType != null)
            {
                total = qualityType switch
                {
                    QualityType.Green => Random.Shared.Next(7, 13),
                    QualityType.Blue => Random.Shared.Next(13, 19),
                    QualityType.Purple => Random.Shared.Next(19, 25),
                    QualityType.Orange => Random.Shared.Next(25, 31),
                    QualityType.Red => Random.Shared.Next(31, 37),
                    QualityType.Gold => Random.Shared.Next(37, 43),
                    _ => Random.Shared.Next(1, 7)
                };
                item.QualityType = (QualityType)qualityType;
            }
            else
            {
                total = Random.Shared.Next(1, 43);
                if (total > 6 && total <= 12)
                {
                    item.QualityType = QualityType.Green;
                }
                else if (total > 12 && total <= 18)
                {
                    item.QualityType = QualityType.Blue;
                }
                else if (total > 18 && total <= 24)
                {
                    item.QualityType = QualityType.Purple;
                }
                else if (total > 24 && total <= 30)
                {
                    item.QualityType = QualityType.Orange;
                }
                else if (total > 30 && total <= 36)
                {
                    item.QualityType = QualityType.Red;
                }
                else if (total > 36 && total <= 42)
                {
                    item.QualityType = QualityType.Gold;
                }
            }

            GenerateAndAddSkillToMagicCard(item, total);

            return item;
        }

        public static void GenerateAndAddSkillToMagicCard(Item item, int total)
        {
            Skill magic = FunGameConstant.Magics[Random.Shared.Next(FunGameConstant.Magics.Count)].Copy();
            magic.Guid = item.Guid;
            magic.Level = (int)item.QualityType switch
            {
                2 => 2,
                3 => 2,
                4 => 3,
                5 => 4,
                6 => 5,
                _ => 1
            };
            if (magic.Level > 1)
            {
                item.Name += $" +{magic.Level - 1}";
            }
            item.Skills.Active = magic;

            // 初始化属性值
            int str = 0, agi = 0, intelligence = 0;

            // 随机决定将多少个属性赋给其中一个属性，确保至少一个不为零
            int nonZeroAttributes = Random.Shared.Next(1, Math.Min(4, total + 1)); // 随机决定非零属性的数量，确保在 total = 1 时最多只有1个非零属性

            // 根据非零属性数量分配属性点
            if (nonZeroAttributes == 1)
            {
                // 只有一个属性不为零
                int attribute = Random.Shared.Next(0, 3);
                if (attribute == 0) str = total;
                else if (attribute == 1) agi = total;
                else intelligence = total;
            }
            else if (nonZeroAttributes == 2 && total >= 2)
            {
                // 两个属性不为零
                int first = Random.Shared.Next(1, total); // 第一个属性的值
                int second = total - first; // 第二个属性的值

                int attribute = Random.Shared.Next(0, 3);
                if (attribute == 0)
                {
                    str = first;
                }
                else if (attribute == 1)
                {
                    agi = first;
                }
                else
                {
                    intelligence = first;
                }

                attribute = Random.Shared.Next(0, 3);
                while ((attribute == 0 && str > 0) || (attribute == 1 && agi > 0) || (attribute == 2 && intelligence > 0))
                {
                    attribute = Random.Shared.Next(0, 3);
                }

                if (attribute == 0)
                {
                    str = second;
                }
                else if (attribute == 1)
                {
                    agi = second;
                }
                else
                {
                    intelligence = second;
                }
            }
            else if (total >= 3)
            {
                // 三个属性都不为零
                str = Random.Shared.Next(1, total - 1); // 第一个属性的值
                agi = Random.Shared.Next(1, total - str); // 第二个属性的值
                intelligence = total - str - agi; // 剩下的值给第三个属性
            }

            Skill skill = Factory.OpenFactory.GetInstance<Skill>(item.Id, item.Name, []);
            GenerateAndAddEffectsToMagicCard(skill, str, agi, intelligence);

            skill.Level = 1;
            List<string> strings = [];
            if (str > 0) strings.Add($"{str:0.##} 点力量");
            if (agi > 0) strings.Add($"{agi:0.##} 点敏捷");
            if (intelligence > 0) strings.Add($"{intelligence:0.##} 点智力");
            item.Description = $"包含魔法：{item.Skills.Active.Name + (item.Skills.Active.Level > 1 ? $" +{item.Skills.Active.Level - 1}" : "")}\r\n" +
                $"增加角色属性：{string.Join("，", strings)}";
            item.Skills.Passives.Add(skill);
        }

        public static void GenerateAndAddEffectsToMagicCard(Skill skill, int str, int agi, int intelligence)
        {
            if (str > 0)
            {
                skill.Effects.Add(Factory.OpenFactory.GetInstance<Effect>((long)EffectID.ExSTR, "", new()
                    {
                        { "skill", skill },
                        {
                            "values", new Dictionary<string, object>()
                            {
                                { "exstr", str }
                            }
                        }
                    }));
            }

            if (agi > 0)
            {
                skill.Effects.Add(Factory.OpenFactory.GetInstance<Effect>((long)EffectID.ExAGI, "", new()
                    {
                        { "skill", skill },
                        {
                            "values", new Dictionary<string, object>()
                            {
                                { "exagi", agi }
                            }
                        }
                    }));
            }

            if (intelligence > 0)
            {
                skill.Effects.Add(Factory.OpenFactory.GetInstance<Effect>((long)EffectID.ExINT, "", new()
                    {
                        { "skill", skill },
                        {
                            "values", new Dictionary<string, object>()
                            {
                                { "exint", intelligence }
                            }
                        }
                    }));
            }
        }

        public static Item? ConflateMagicCardPack(IEnumerable<Item> magicCards)
        {
            if (magicCards.Any())
            {
                List<Skill> magics = [.. magicCards.Where(i => i.Skills.Active != null).Select(i => i.Skills.Active)];
                List<Skill> passives = [.. magicCards.SelectMany(i => i.Skills.Passives)];
                Item item = Factory.GetItem();
                item.Id = Convert.ToInt64("10" + Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 8));
                item.Name = GenerateRandomChineseName();
                item.ItemType = ItemType.MagicCardPack;
                double str = 0, agi = 0, intelligence = 0;
                foreach (Skill skill in passives)
                {
                    Skill newSkill = skill.Copy();
                    foreach (Effect effect in newSkill.Effects)
                    {
                        switch ((EffectID)effect.Id)
                        {
                            case EffectID.ExSTR:
                                if (effect is ExSTR exstr)
                                {
                                    str += exstr.Value;
                                }
                                break;
                            case EffectID.ExAGI:
                                if (effect is ExAGI exagi)
                                {
                                    agi += exagi.Value;
                                }
                                break;
                            case EffectID.ExINT:
                                if (effect is ExINT exint)
                                {
                                    intelligence += exint.Value;
                                }
                                break;
                        }
                    }
                    newSkill.Level = skill.Level;
                    newSkill.Item = item;
                    item.Skills.Passives.Add(newSkill);
                }
                List<string> strings = [];
                if (str > 0) strings.Add($"{str:0.##} 点力量");
                if (agi > 0) strings.Add($"{agi:0.##} 点敏捷");
                if (intelligence > 0) strings.Add($"{intelligence:0.##} 点智力");
                foreach (Skill skill in magics)
                {
                    IEnumerable<Skill> has = item.Skills.Magics.Where(m => m.Id == skill.Id);
                    if (has.Any() && has.First() is Skill s)
                    {
                        s.Level += skill.Level;
                        if (s.Level > 1) s.Name = s.Name.Split(' ')[0] + $" +{s.Level - 1}";
                    }
                    else
                    {
                        Skill magic = skill.Copy();
                        magic.Guid = item.Guid;
                        magic.Level = skill.Level;
                        item.Skills.Magics.Add(magic);
                    }
                }
                item.Description = $"包含魔法：{string.Join("，", item.Skills.Magics.Select(m => m.Name + (m.Level > 1 ? $" +{m.Level - 1}" : "")))}\r\n" +
                    $"增加角色属性：{string.Join("，", strings)}";
                double total = str + agi + intelligence;
                if (total > 18 && total <= 36)
                {
                    item.QualityType = QualityType.Green;
                }
                else if (total > 36 && total <= 54)
                {
                    item.QualityType = QualityType.Blue;
                }
                else if (total > 54 && total <= 72)
                {
                    item.QualityType = QualityType.Purple;
                }
                else if (total > 72 && total <= 90)
                {
                    item.QualityType = QualityType.Orange;
                }
                else if (total > 90 && total <= 108)
                {
                    item.QualityType = QualityType.Red;
                }
                else if (total > 108)
                {
                    item.QualityType = QualityType.Gold;
                }
                return item;
            }
            return null;
        }

        public static Item? GenerateMagicCardPack(int magicCardCount, QualityType? qualityType = null)
        {
            List<Item> magicCards = GenerateMagicCards(magicCardCount, qualityType);
            Item? magicCardPack = ConflateMagicCardPack(magicCards);
            return magicCardPack;
        }

        public static void Reload()
        {
            FunGameConstant.Characters.Clear();
            FunGameConstant.Equipment.Clear();
            FunGameConstant.Skills.Clear();
            FunGameConstant.SuperSkills.Clear();
            FunGameConstant.PassiveSkills.Clear();
            FunGameConstant.Magics.Clear();
            FunGameConstant.DrawCardItems.Clear();
            FunGameConstant.ExploreItems.Clear();
            FunGameConstant.AllItems.Clear();
            FunGameConstant.ItemSkills.Clear();
            FunGameConstant.AllSkills.Clear();

            InitFunGame();
        }

        public static string GenerateRandomChineseName()
        {
            // 随机生成名字长度，2到5个字
            int nameLength = Random.Shared.Next(2, 6);
            StringBuilder name = new();

            for (int i = 0; i < nameLength; i++)
            {
                // 从常用汉字集中随机选择一个汉字
                char chineseCharacter = FunGameConstant.CommonChineseCharacters[Random.Shared.Next(FunGameConstant.CommonChineseCharacters.Length)];
                name.Append(chineseCharacter);
            }

            return name.ToString();
        }

        public static string GenerateRandomChineseUserName()
        {
            StringBuilder name = new();

            // 随机姓
            string lastname = FunGameConstant.CommonSurnames[Random.Shared.Next(FunGameConstant.CommonSurnames.Length)];
            name.Append(lastname);

            // 随机生成名字长度，2到5个字
            int nameLength = Random.Shared.Next(1, 2);

            for (int i = 0; i < nameLength; i++)
            {
                // 从常用汉字集中随机选择一个汉字
                char chineseCharacter = FunGameConstant.CommonChineseCharacters[Random.Shared.Next(FunGameConstant.CommonChineseCharacters.Length)];
                name.Append(chineseCharacter);
            }

            return name.ToString();
        }

        public static User GetUser(PluginConfig pc)
        {
            User user = pc.Get<User>("user") ?? Factory.GetUser();

            List<Character> characters = [.. user.Inventory.Characters];
            List<Item> items = [.. user.Inventory.Items];
            Character mc = user.Inventory.MainCharacter;
            List<long> squad = [.. user.Inventory.Squad];
            Dictionary<long, DateTime> training = user.Inventory.Training.ToDictionary(kv => kv.Key, kv => kv.Value);
            user.Inventory.Characters.Clear();
            user.Inventory.Items.Clear();
            user.Inventory.Squad.Clear();
            user.Inventory.Training.Clear();

            foreach (Item inventoryItem in items)
            {
                Item realItem = inventoryItem.Copy(true, true, true, true, FunGameConstant.AllItems, FunGameConstant.AllSkills);
                realItem.User = user;
                user.Inventory.Items.Add(realItem);
            }

            foreach (Character inventoryCharacter in characters)
            {
                Character tempCharacter = Factory.OpenFactory.GetInstance<Character>(inventoryCharacter.Id, inventoryCharacter.Name, []);
                if (tempCharacter.Id != 0)
                {
                    inventoryCharacter.InitialATK = tempCharacter.InitialATK;
                    inventoryCharacter.InitialDEF = tempCharacter.InitialDEF;
                    inventoryCharacter.InitialHP = tempCharacter.InitialHP;
                    inventoryCharacter.InitialMP = tempCharacter.InitialMP;
                    inventoryCharacter.InitialSTR = tempCharacter.InitialSTR;
                    inventoryCharacter.STRGrowth = tempCharacter.STRGrowth;
                    inventoryCharacter.InitialAGI = tempCharacter.InitialAGI;
                    inventoryCharacter.AGIGrowth = tempCharacter.AGIGrowth;
                    inventoryCharacter.InitialINT = tempCharacter.InitialINT;
                    inventoryCharacter.INTGrowth = tempCharacter.INTGrowth;
                    inventoryCharacter.InitialSPD = tempCharacter.InitialSPD;
                    inventoryCharacter.InitialHR = tempCharacter.InitialHR;
                    inventoryCharacter.InitialMR = tempCharacter.InitialMR;
                }
                Character realCharacter = CharacterBuilder.Build(inventoryCharacter, false, true, user.Inventory, FunGameConstant.AllItems, FunGameConstant.AllSkills, false);
                // 自动回血
                DateTime now = DateTime.Now;
                int seconds = (int)(now - user.LastTime).TotalSeconds;
                // 死了不回，要去治疗
                if (realCharacter.HP > 0)
                {
                    double recoveryHP = realCharacter.HR * seconds;
                    double recoveryMP = realCharacter.MR * seconds;
                    double recoveryEP = realCharacter.ER * seconds;
                    realCharacter.HP += recoveryHP;
                    realCharacter.MP += recoveryMP;
                    realCharacter.EP += recoveryEP;
                }
                // 减少所有技能的冷却时间
                foreach (Skill skill in realCharacter.Skills)
                {
                    skill.CurrentCD -= seconds;
                    if (skill.CurrentCD <= 0)
                    {
                        skill.CurrentCD = 0;
                        skill.Enable = true;
                    }
                }
                // 移除到时间的特效
                List<Effect> effects = [.. realCharacter.Effects];
                foreach (Effect effect in effects)
                {
                    if (effect.Level == 0)
                    {
                        realCharacter.Effects.Remove(effect);
                        continue;
                    }
                    effect.OnTimeElapsed(realCharacter, seconds);
                    // 自身被动不会考虑
                    if (effect.EffectType == EffectType.None && effect.Skill.SkillType == SkillType.Passive)
                    {
                        continue;
                    }

                    if (effect.Durative)
                    {
                        effect.RemainDuration -= seconds;
                        if (effect.RemainDuration <= 0)
                        {
                            effect.RemainDuration = 0;
                            realCharacter.Effects.Remove(effect);
                            effect.OnEffectLost(realCharacter);
                        }
                    }
                }
                realCharacter.User = user;
                user.Inventory.Characters.Add(realCharacter);
            }

            if (user.Inventory.Characters.FirstOrDefault(c => c.Id == mc.Id) is Character newMC)
            {
                user.Inventory.MainCharacter = newMC;
            }

            foreach (long id in squad)
            {
                if (user.Inventory.Characters.FirstOrDefault(c => c.Id == id) is Character s)
                {
                    user.Inventory.Squad.Add(id);
                }
            }

            foreach (long cid in training.Keys)
            {
                if (user.Inventory.Characters.FirstOrDefault(c => c.Id == cid) is Character t)
                {
                    user.Inventory.Training[t.Id] = training[cid];
                }
            }

            if (!pc.TryGetValue("logon", out object? value) || (value is bool logon && !logon))
            {
                pc.Add("logon", true);
                AddNotice(user.Id, "欢迎回到筽祀牻大陆！请发送【帮助】获取更多玩法指令哦～");
                AddNotice(user.Id, GetEvents());
            }

            return user;
        }

        public static void AddNotice(long userId, params string[] notices)
        {
            if (UserNotice.TryGetValue(userId, out HashSet<string>? list) && list != null)
            {
                foreach (string notice in notices)
                {
                    list.Add(notice);
                }
            }
            else UserNotice[userId] = [.. notices];
        }

        public static IEnumerable<T> GetPage<T>(IEnumerable<T> list, int showPage, int pageSize)
        {
            return [.. list.Skip((showPage - 1) * pageSize).Take(pageSize)];
        }

        public static List<string> DrawCards(User user, bool is10 = false, bool useCurrency = true)
        {
            List<string> msgs = [];
            int reduce;
            string reduceUnit;
            IEnumerable<Item>? items = null;
            if (useCurrency)
            {
                if (is10)
                {
                    items = user.Inventory.Items.Where(i => i is 十连奖券);
                }
                else
                {
                    items = user.Inventory.Items.Where(i => i is 奖券);
                }
                if (items.Any())
                {
                    reduceUnit = items.First().Name;
                    reduce = 1;
                    user.Inventory.Items.Remove(items.First());
                }
                else
                {
                    reduceUnit = General.GameplayEquilibriumConstant.InGameCurrency;
                    reduce = is10 ? FunGameConstant.DrawCardReduce * 10 : FunGameConstant.DrawCardReduce;
                    if (user.Inventory.Credits < reduce)
                    {
                        msgs.Add($"你的{reduceUnit}不足 {reduce} 呢，无法抽卡！");
                        return msgs;
                    }
                    user.Inventory.Credits -= reduce;
                }
            }
            else
            {
                reduceUnit = General.GameplayEquilibriumConstant.InGameMaterial;
                reduce = is10 ? FunGameConstant.DrawCardReduce_Material * 10 : FunGameConstant.DrawCardReduce_Material;
                if (user.Inventory.Materials < reduce)
                {
                    msgs.Add($"你的{reduceUnit}不足 {reduce} 呢，无法抽卡！");
                    return msgs;
                }
                user.Inventory.Materials -= reduce;
            }

            if (is10)
            {
                int count = 0;
                for (int i = 1; i <= 10; i++)
                {
                    double dice = Random.Shared.NextDouble();
                    if (dice > 0.8)
                    {
                        count++;
                        msgs.Add(GetDrawCardResult(reduce, reduceUnit, user, is10, count));
                    }
                }
                if (msgs.Count <= 1)
                {
                    msgs[0] = $"消耗 {reduce} {reduceUnit}，你什么也没抽中……";
                }
                else
                {
                    msgs.Insert(0, $"消耗 {reduce} {reduceUnit}，恭喜你抽到了：");
                }
            }
            else
            {
                double dice = Random.Shared.NextDouble();
                if (dice > 0.8)
                {
                    msgs.Add(GetDrawCardResult(reduce, reduceUnit, user));
                }
                else
                {
                    msgs.Add($"消耗 {reduce} {reduceUnit}，你什么也没抽中……");
                }
            }

            if (items != null && items.Any())
            {
                msgs.Insert(0, $"本次抽卡使用{reduceUnit}代替金币抽卡！你的库存剩余 {items.Count()} 张{reduceUnit}。");
            }

            return msgs;
        }

        public static string GetDrawCardResult(int reduce, string reduceUnit, User user, bool isMulti = false, int multiCount = 1)
        {
            string msg = "";
            if (!isMulti)
            {
                msg = $"消耗 {reduce} {reduceUnit}，恭喜你抽到了：";
            }

            int r = Random.Shared.Next(8);
            double q = Random.Shared.NextDouble() * 100;
            QualityType type = QualityType.White;
            foreach (QualityType typeTemp in FunGameConstant.DrawCardProbabilities.Keys.OrderByDescending(o => (int)o))
            {
                if (q <= FunGameConstant.DrawCardProbabilities[typeTemp])
                {
                    type = typeTemp;
                    break;
                }
            }

            switch (r)
            {
                case 1:
                    Item[] 武器 = [.. FunGameConstant.Equipment.Where(i => i.ItemType == ItemType.Weapon && i.QualityType == type)];
                    Item a = 武器[Random.Shared.Next(武器.Length)].Copy();
                    if (a.QualityType >= QualityType.Orange) a.IsLock = true;
                    SetSellAndTradeTime(a);
                    user.Inventory.Items.Add(a);
                    msg += ItemSet.GetQualityTypeName(a.QualityType) + ItemSet.GetItemTypeName(a.ItemType) + "【" + a.Name + "】！\r\n" + a.Description;
                    break;

                case 2:
                    Item[] 防具 = [.. FunGameConstant.Equipment.Where(i => i.ItemType == ItemType.Armor && i.QualityType == type)];
                    Item b = 防具[Random.Shared.Next(防具.Length)].Copy();
                    if (b.QualityType >= QualityType.Orange) b.IsLock = true;
                    SetSellAndTradeTime(b);
                    user.Inventory.Items.Add(b);
                    msg += ItemSet.GetQualityTypeName(b.QualityType) + ItemSet.GetItemTypeName(b.ItemType) + "【" + b.Name + "】！\r\n" + b.Description;
                    break;

                case 3:
                    Item[] 鞋子 = [.. FunGameConstant.Equipment.Where(i => i.ItemType == ItemType.Shoes && i.QualityType == type)];
                    Item c = 鞋子[Random.Shared.Next(鞋子.Length)].Copy();
                    if (c.QualityType >= QualityType.Orange) c.IsLock = true;
                    SetSellAndTradeTime(c);
                    user.Inventory.Items.Add(c);
                    msg += ItemSet.GetQualityTypeName(c.QualityType) + ItemSet.GetItemTypeName(c.ItemType) + "【" + c.Name + "】！\r\n" + c.Description;
                    break;

                case 4:
                    Item[] 饰品 = [.. FunGameConstant.Equipment.Where(i => i.ItemType == ItemType.Accessory && i.QualityType == type)];
                    Item d = 饰品[Random.Shared.Next(饰品.Length)].Copy();
                    if (d.QualityType >= QualityType.Orange) d.IsLock = true;
                    SetSellAndTradeTime(d);
                    user.Inventory.Items.Add(d);
                    msg += ItemSet.GetQualityTypeName(d.QualityType) + ItemSet.GetItemTypeName(d.ItemType) + "【" + d.Name + "】！\r\n" + d.Description;
                    break;

                case 5:
                    Character character = FunGameConstant.Characters[Random.Shared.Next(FunGameConstant.Characters.Count)].Copy();
                    AddCharacterSkills(character, 1, 0, 0);
                    if (user.Inventory.Characters.Any(c => c.Id == character.Id))
                    {
                        user.Inventory.Materials += 50;
                        msg += "【" + character.ToStringWithOutUser() + "】！\r\n但是你已经拥有此角色，转换为【50】" + General.GameplayEquilibriumConstant.InGameMaterial + "！";
                    }
                    else
                    {
                        user.Inventory.Characters.Add(character);
                        msg += "【" + character.ToStringWithOutUser() + "】！\r\n输入【查角色" + character.Id + "】可以获取此角色完整信息。";
                    }
                    break;

                case 6:
                    Item mfk = GenerateMagicCard(type);
                    if (mfk.QualityType >= QualityType.Orange) mfk.IsLock = true;
                    SetSellAndTradeTime(mfk);
                    user.Inventory.Items.Add(mfk);
                    msg += ItemSet.GetQualityTypeName(mfk.QualityType) + ItemSet.GetItemTypeName(mfk.ItemType) + "【" + mfk.Name + "】！\r\n" + mfk.Description;
                    break;

                case 7:
                    Item 物品 = FunGameConstant.DrawCardItems[Random.Shared.Next(FunGameConstant.DrawCardItems.Count)].Copy();
                    if (物品.QualityType >= QualityType.Orange) 物品.IsLock = true;
                    SetSellAndTradeTime(物品);
                    user.Inventory.Items.Add(物品);
                    msg += ItemSet.GetQualityTypeName(物品.QualityType) + ItemSet.GetItemTypeName(物品.ItemType) + "【" + 物品.Name + "】！\r\n" + 物品.Description;
                    // 连接到任务系统
                    AddExploreItemCache(user.Id, 物品.Name);
                    // 连接到活动系统
                    ActivitiesItemCache.Add(物品.Name);
                    break;

                case 0:
                default:
                    Item? mfkb = GenerateMagicCardPack(3, type);
                    if (mfkb != null)
                    {
                        if (mfkb.QualityType >= QualityType.Orange) mfkb.IsLock = true;
                        SetSellAndTradeTime(mfkb);
                        user.Inventory.Items.Add(mfkb);
                        msg += ItemSet.GetQualityTypeName(mfkb.QualityType) + ItemSet.GetItemTypeName(mfkb.ItemType) + "【" + mfkb.Name + "】！\r\n" + mfkb.Description;
                    }
                    break;
            }
            if (isMulti) msg = $"{multiCount}. \r\n{msg}";
            return msg;
        }

        public static string GetSignInResult(User user, int days)
        {
            string msg = $"签到成功，你已连续签到 {days + 1} 天！\r\n本次签到获得：";
            int currency = Random.Shared.Next(1000, 3000) + 10 * days;
            msg += $"{currency} {General.GameplayEquilibriumConstant.InGameCurrency} 和 ";
            int material = Random.Shared.Next(5, 15) + days / 7;
            msg += $"{material} {General.GameplayEquilibriumConstant.InGameMaterial}！额外获得：";
            user.Inventory.Credits += currency;
            user.Inventory.Materials += material;
            int r = Random.Shared.Next(6);
            double q = Random.Shared.NextDouble() * 100;

            // 根据签到天数调整概率
            double daysFactor = Math.Min(days * 0.03, 30);
            Dictionary<QualityType, double> adjustedProbabilities = new(FunGameConstant.DrawCardProbabilities);
            foreach (QualityType typeTemp in adjustedProbabilities.Keys)
            {
                adjustedProbabilities[typeTemp] += daysFactor;
            }

            // 生成随机数并确定品质
            double randomValue = Random.Shared.NextDouble() * 100;
            QualityType type = QualityType.White;
            foreach (QualityType typeTemp in adjustedProbabilities.Keys.OrderByDescending(o => (int)o))
            {
                if (randomValue <= adjustedProbabilities[typeTemp])
                {
                    type = typeTemp;
                    break;
                }
            }

            switch (r)
            {
                case 1:
                    Item[] 武器 = [.. FunGameConstant.Equipment.Where(i => i.Id.ToString().StartsWith("11") && i.QualityType == type)];
                    Item a = 武器[Random.Shared.Next(武器.Length)].Copy();
                    if (a.QualityType >= QualityType.Orange) a.IsLock = true;
                    SetSellAndTradeTime(a);
                    user.Inventory.Items.Add(a);
                    msg += ItemSet.GetQualityTypeName(a.QualityType) + ItemSet.GetItemTypeName(a.ItemType) + "【" + a.Name + "】！\r\n" + a.Description;
                    break;

                case 2:
                    Item[] 防具 = [.. FunGameConstant.Equipment.Where(i => i.Id.ToString().StartsWith("12") && i.QualityType == type)];
                    Item b = 防具[Random.Shared.Next(防具.Length)].Copy();
                    if (b.QualityType >= QualityType.Orange) b.IsLock = true;
                    SetSellAndTradeTime(b);
                    user.Inventory.Items.Add(b);
                    msg += ItemSet.GetQualityTypeName(b.QualityType) + ItemSet.GetItemTypeName(b.ItemType) + "【" + b.Name + "】！\r\n" + b.Description;
                    break;

                case 3:
                    Item[] 鞋子 = [.. FunGameConstant.Equipment.Where(i => i.Id.ToString().StartsWith("13") && i.QualityType == type)];
                    Item c = 鞋子[Random.Shared.Next(鞋子.Length)].Copy();
                    if (c.QualityType >= QualityType.Orange) c.IsLock = true;
                    SetSellAndTradeTime(c);
                    user.Inventory.Items.Add(c);
                    msg += ItemSet.GetQualityTypeName(c.QualityType) + ItemSet.GetItemTypeName(c.ItemType) + "【" + c.Name + "】！\r\n" + c.Description;
                    break;

                case 4:
                    Item[] 饰品 = [.. FunGameConstant.Equipment.Where(i => i.Id.ToString().StartsWith("14") && i.QualityType == type)];
                    Item d = 饰品[Random.Shared.Next(饰品.Length)].Copy();
                    if (d.QualityType >= QualityType.Orange) d.IsLock = true;
                    SetSellAndTradeTime(d);
                    user.Inventory.Items.Add(d);
                    msg += ItemSet.GetQualityTypeName(d.QualityType) + ItemSet.GetItemTypeName(d.ItemType) + "【" + d.Name + "】！\r\n" + d.Description;
                    break;

                case 5:
                    Item mfk = GenerateMagicCard(type);
                    if (mfk.QualityType >= QualityType.Orange) mfk.IsLock = true;
                    SetSellAndTradeTime(mfk);
                    user.Inventory.Items.Add(mfk);
                    msg += ItemSet.GetQualityTypeName(mfk.QualityType) + ItemSet.GetItemTypeName(mfk.ItemType) + "【" + mfk.Name + "】！\r\n" + mfk.Description;
                    break;

                case 0:
                default:
                    Item? mfkb = GenerateMagicCardPack(3, type);
                    if (mfkb != null)
                    {
                        if (mfkb.QualityType >= QualityType.Orange) mfkb.IsLock = true;
                        SetSellAndTradeTime(mfkb);
                        user.Inventory.Items.Add(mfkb);
                        msg += ItemSet.GetQualityTypeName(mfkb.QualityType) + ItemSet.GetItemTypeName(mfkb.ItemType) + "【" + mfkb.Name + "】！\r\n" + mfkb.Description;
                    }
                    break;
            }
            return msg;
        }

        public static void SetSellAndTradeTime(Item item, bool sell = false, bool trade = true, DateTime? nextSell = null, DateTime? nextTrade = null)
        {
            if (sell)
            {
                item.IsSellable = false;
                item.NextSellableTime = DateTimeUtility.GetTradableTime(nextSell);
            }
            if (trade)
            {
                item.IsTradable = false;
                item.NextTradableTime = DateTimeUtility.GetTradableTime(nextTrade);
            }
        }

        public static async Task<string> AllowSellAndTrade()
        {
            string msg;
            string dpath = $@"{AppDomain.CurrentDomain.BaseDirectory}configs/saved";
            if (Directory.Exists(dpath))
            {
                string[] jsonFiles = Directory.GetFiles(dpath, "*.json");

                List<Task> tasks = [];
                foreach (string file in jsonFiles)
                {
                    tasks.Add(Task.Run(() =>
                    {
                        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
                        PluginConfig pc = new("saved", fileNameWithoutExtension);
                        pc.LoadConfig();
                        if (pc.Count > 0)
                        {
                            User user = GetUser(pc);
                            foreach (Item item in user.Inventory.Items)
                            {
                                if (!item.IsSellable && item.NextSellableTime != DateTime.MinValue && DateTime.Now >= item.NextSellableTime)
                                {
                                    item.NextSellableTime = DateTime.MinValue;
                                    item.IsSellable = true;
                                }
                                if (!item.IsTradable && item.NextTradableTime != DateTime.MinValue && DateTime.Now >= item.NextTradableTime)
                                {
                                    item.NextTradableTime = DateTime.MinValue;
                                    item.IsTradable = true;
                                }
                            }
                            pc.Add("user", user);
                            pc.SaveConfig();
                        }
                    }));
                }
                await Task.WhenAll(tasks);
                msg = "已清理所有玩家的物品交易时间。";
            }
            else
            {
                msg = "存档目录不存在，无法清理交易时间。";
            }
            return msg;
        }

        public static void AddCharacterSkills(Character character, int passiveLevel, int skillLevel, int superLevel)
        {
            long id = character.Id;
            Math.Sign(skillLevel);
            if (id == 1)
            {
                Skill META马 = new META马(character)
                {
                    Level = passiveLevel
                };
                character.Skills.Add(META马);

                Skill 力量爆发 = new 力量爆发(character)
                {
                    Level = superLevel
                };
                character.Skills.Add(力量爆发);
            }

            if (id == 2)
            {
                Skill 心灵之火 = new 心灵之火(character)
                {
                    Level = passiveLevel
                };
                character.Skills.Add(心灵之火);

                Skill 天赐之力 = new 天赐之力(character)
                {
                    Level = superLevel
                };
                character.Skills.Add(天赐之力);
            }

            if (id == 3)
            {
                Skill 魔法震荡 = new 魔法震荡(character)
                {
                    Level = passiveLevel
                };
                character.Skills.Add(魔法震荡);

                Skill 魔法涌流 = new 魔法涌流(character)
                {
                    Level = superLevel
                };
                character.Skills.Add(魔法涌流);
            }

            if (id == 4)
            {
                Skill 灵能反射 = new 灵能反射(character)
                {
                    Level = passiveLevel
                };
                character.Skills.Add(灵能反射);

                Skill 三重叠加 = new 三重叠加(character)
                {
                    Level = superLevel
                };
                character.Skills.Add(三重叠加);
            }

            if (id == 5)
            {
                Skill 智慧与力量 = new 智慧与力量(character)
                {
                    Level = passiveLevel
                };
                character.Skills.Add(智慧与力量);

                Skill 变幻之心 = new 变幻之心(character)
                {
                    Level = superLevel
                };
                character.Skills.Add(变幻之心);
            }

            if (id == 6)
            {
                Skill 致命打击 = new 致命打击(character)
                {
                    Level = passiveLevel
                };
                character.Skills.Add(致命打击);

                Skill 精准打击 = new 精准打击(character)
                {
                    Level = superLevel
                };
                character.Skills.Add(精准打击);
            }

            if (id == 7)
            {
                Skill 毁灭之势 = new 毁灭之势(character)
                {
                    Level = passiveLevel
                };
                character.Skills.Add(毁灭之势);

                Skill 绝对领域 = new 绝对领域(character)
                {
                    Level = superLevel
                };
                character.Skills.Add(绝对领域);
            }

            if (id == 8)
            {
                Skill 枯竭打击 = new 枯竭打击(character)
                {
                    Level = passiveLevel
                };
                character.Skills.Add(枯竭打击);

                Skill 能量毁灭 = new 能量毁灭(character)
                {
                    Level = superLevel
                };
                character.Skills.Add(能量毁灭);
            }

            if (id == 9)
            {
                Skill 破釜沉舟 = new 破釜沉舟(character)
                {
                    Level = passiveLevel
                };
                character.Skills.Add(破釜沉舟);

                Skill 迅捷之势 = new 迅捷之势(character)
                {
                    Level = superLevel
                };
                character.Skills.Add(迅捷之势);
            }

            if (id == 10)
            {
                Skill 累积之压 = new 累积之压(character)
                {
                    Level = passiveLevel
                };
                character.Skills.Add(累积之压);

                Skill 嗜血本能 = new 嗜血本能(character)
                {
                    Level = superLevel
                };
                character.Skills.Add(嗜血本能);
            }

            if (id == 11)
            {
                Skill 敏捷之刃 = new 敏捷之刃(character)
                {
                    Level = passiveLevel
                };
                character.Skills.Add(敏捷之刃);

                Skill 平衡强化 = new 平衡强化(character)
                {
                    Level = superLevel
                };
                character.Skills.Add(平衡强化);
            }

            if (id == 12)
            {
                Skill 弱者猎手 = new 弱者猎手(character)
                {
                    Level = passiveLevel
                };
                character.Skills.Add(弱者猎手);

                Skill 血之狂欢 = new 血之狂欢(character)
                {
                    Level = superLevel
                };
                character.Skills.Add(血之狂欢);
            }
        }

        public static bool UseItem(Item item, int times, User user, IEnumerable<Character> targets, out string msg)
        {
            msg = "";
            Dictionary<string, object> args = new()
            {
                { "targets", targets.ToArray() }
            };
            bool result = item.UseItem(user, times, args);
            if (item.EntityState == EntityState.Deleted)
            {
                user.Inventory.Items.Remove(item);
            }
            string key = args.Keys.FirstOrDefault(s => s.Equals("msg", StringComparison.CurrentCultureIgnoreCase)) ?? "";
            if (key != "" && args.TryGetValue(key, out object? value) && value is string str)
            {
                msg = str;
            }
            if (msg.Trim() == "" && !result)
            {
                result = UseItemCustom(item, user, targets, out msg);
            }
            return result;
        }

        public static bool UseItems(IEnumerable<Item> items, User user, IEnumerable<Character> targets, List<string> msgs)
        {
            Dictionary<string, object> args = new()
            {
                { "targets", targets.ToArray() },
                { "useCount", items.Count() }
            };
            bool result = true;
            foreach (Item item in items)
            {
                if (!result)
                {
                    break;
                }
                if (FunGameConstant.ItemCanUsed.Contains(item.ItemType))
                {
                    if (item.RemainUseTimes <= 0)
                    {
                        msgs.Add($"{item.Name} 的剩余使用次数为 0，无法使用！");
                        result = false;
                    }
                    if (!result)
                    {
                        continue;
                    }
                    bool tempResult = item.UseItem(user, 1, args);
                    if (item.EntityState == EntityState.Deleted)
                    {
                        user.Inventory.Items.Remove(item);
                    }
                    string tempStr = "";
                    string key = args.Keys.FirstOrDefault(s => s.Equals("msg", StringComparison.CurrentCultureIgnoreCase)) ?? "";
                    if (key != "" && args.TryGetValue(key, out object? value) && value is string str)
                    {
                        if (str != "") msgs.Add(str);
                        tempStr = str;
                    }
                    if (tempStr.Trim() == "" && !tempResult)
                    {
                        // 使用自定义使用方法
                        tempResult = UseItemCustom(item, user, targets, out tempStr);
                    }
                    if (!tempResult)
                    {
                        result = false;
                    }
                    msgs.Add(tempStr);
                    // 这个参数会覆盖掉原消息
                    key = args.Keys.FirstOrDefault(s => s.Equals("truemsg", StringComparison.CurrentCultureIgnoreCase)) ?? "";
                    if (key != "" && args.TryGetValue(key, out value) && value is string truemsg)
                    {
                        msgs.Clear();
                        msgs.Add(truemsg);
                    }
                }
                else
                {
                    msgs.Add($"这个物品无法使用！");
                }
            }
            return result;
        }

        public static bool UseItemCustom(Item item, User user, IEnumerable<Character> targets, out string msg)
        {
            msg = "";
            if (item.ItemType == ItemType.GiftBox)
            {
                if (item is 魔法卡礼包 cardBox)
                {
                    List<string> cards = [];
                    for (int i = 0; i< cardBox.Count; i++)
                    {
                        Item newItem = GenerateMagicCard(item.QualityType);
                        if (newItem.QualityType >= QualityType.Orange) newItem.IsLock = true;
                        SetSellAndTradeTime(newItem);
                        newItem.User = user;
                        user.Inventory.Items.Add(newItem);
                        cards.Add($"[{ItemSet.GetQualityTypeName(item.QualityType)}|魔法卡] {newItem.Name}\r\n{newItem.ToStringInventory(false)}");
                    }
                    msg = "打开礼包成功！获得了以下物品：\r\n" + string.Join("\r\n", cards);
                    item.RemainUseTimes--;
                    if (item.RemainUseTimes < 0) item.RemainUseTimes = 0;
                    if (item.RemainUseTimes == 0)
                    {
                        user.Inventory.Items.Remove(item);
                    }
                    return true;
                }
                else if (item is 礼包.GiftBox box && box.Gifts.Count > 0)
                {
                    foreach (string name in box.Gifts.Keys)
                    {
                        if (name == General.GameplayEquilibriumConstant.InGameCurrency)
                        {
                            user.Inventory.Credits += box.Gifts[name];
                        }
                        if (name == General.GameplayEquilibriumConstant.InGameMaterial)
                        {
                            user.Inventory.Materials += box.Gifts[name];
                        }
                        if (FunGameConstant.AllItems.FirstOrDefault(i => i.Name == name) is Item currentItem)
                        {
                            for (int i = 0; i < box.Gifts[name]; i++)
                            {
                                Item newItem = currentItem.Copy();
                                if (newItem.QualityType >= QualityType.Orange) newItem.IsLock = true;
                                SetSellAndTradeTime(newItem);
                                newItem.User = user;
                                user.Inventory.Items.Add(newItem);
                            }
                        }
                    }
                    msg = "打开礼包成功！获得了以下物品：\r\n" + string.Join("，", box.Gifts.Select(kv => $"{kv.Key} * {kv.Value}"));
                    if (item.Name == nameof(年夜饭))
                    {
                        msg += "\r\n" + "热腾腾的除夕年夜饭，祝您阖家团圆，年味浓浓！";
                    }
                    else if (item.Name == nameof(蛇年大吉))
                    {
                        msg += "\r\n" + "金蛇送福，好运连连！！";
                    }
                    else if (item.Name == nameof(新春快乐))
                    {
                        msg += "\r\n" + "新春纳福，喜乐安康！！";
                    }
                    else if (item.Name == nameof(毕业礼包))
                    {
                        msg += "\r\n" + "咦？！！啊咧！！！";
                    }
                    item.RemainUseTimes--;
                    if (item.RemainUseTimes < 0) item.RemainUseTimes = 0;
                    if (item.RemainUseTimes == 0)
                    {
                        user.Inventory.Items.Remove(item);
                    }
                    return true;
                }
            }
            switch (item.Name)
            {
                default:
                    break;
            }
            return false;
        }

        public static string GetLevelBreakNeedy(int levelBreak)
        {
            if (FunGameConstant.LevelBreakNeedyList.TryGetValue(levelBreak, out Dictionary<string, int>? needy) && needy != null && needy.Count > 0)
            {
                return string.Join("，", needy.Select(kv => kv.Key + " * " + kv.Value));
            }
            return "";
        }

        public static string UseMagicCard(User user, Item magicCard, Item magicCardPack)
        {
            if (magicCard.QualityType != magicCardPack.QualityType)
            {
                return $"只能对相同品质的魔法卡包使用魔法卡！";
            }
            if (magicCard.Skills.Active != null)
            {
                string msg = "";
                Skill magic = magicCard.Skills.Active;
                if (magicCardPack.Skills.Magics.FirstOrDefault(m => m.GetIdName() == magic.GetIdName()) is Skill has && has.Level < 8)
                {
                    int original = has.Level;
                    // 添加技能等级
                    has.Level += magic.Level;
                    // 补偿材料，1级10材料
                    int diff = magic.Level - (has.Level - original);
                    if (diff != 0)
                    {
                        user.Inventory.Materials += diff * 10;
                        msg = $"由于魔法卡的技能等级数尚未用完，技能便已经升至满级，特此补偿 {diff * 10} {General.GameplayEquilibriumConstant.InGameMaterial}！\r\n";
                    }
                }
                else
                {
                    if (magicCardPack.Skills.Magics.Count < 3)
                    {
                        // 添加技能
                        magicCardPack.Skills.Magics.Add(magic);
                        magic.Guid = magicCard.Guid;
                        msg = $"此魔法卡的技能已经添加到未满三个魔法的卡包上。\r\n";
                    }
                    else return $"魔法【{magic.Name}】在此魔法卡包中不存在或是已经升至满级！";
                }
                string containMagics = magicCardPack.Description.Split("增加角色属性")[0];
                magicCardPack.Description = $"包含魔法：{string.Join("，", magicCardPack.Skills.Magics.Select(m => m.Name + (m.Level > 1 ? $" +{m.Level - 1}" : "")))}\r\n" + magicCardPack.Description.Replace(containMagics, "");
                magicCard.RemainUseTimes--;
                if (magicCard.RemainUseTimes < 0) magicCard.RemainUseTimes = 0;
                if (magicCard.RemainUseTimes == 0)
                {
                    user.Inventory.Items.Remove(magicCard);
                }
                return $"目标魔法卡包的力量已经被此魔法卡显著地提升了！！！\r\n{msg}{magicCardPack.ToStringInventory(true)}";
            }
            else
            {
                return "此魔法卡不存在任何魔法！";
            }
        }

        public static string GetTrainingInfo(TimeSpan diff, Character character, bool isPre, out int totalExperience, out int smallBookCount, out int mediumBookCount, out int largeBookCount)
        {
            int totalMinutes = (int)diff.TotalMinutes;

            // 每分钟经验
            int experiencePerMinute = 1;

            // 最大练级时间
            int dailyTrainingMinutes = 1440;

            // 计算总经验奖励
            totalExperience = Math.Min(totalMinutes, dailyTrainingMinutes) * experiencePerMinute;

            // 计算经验书奖励
            smallBookCount = 0;
            mediumBookCount = 0;
            largeBookCount = 0;

            // 计算总训练小时数
            int trainingHours = totalMinutes / 60;

            if (trainingHours >= 8)
            {
                smallBookCount = Math.Min(1, trainingHours);
            }

            if (trainingHours >= 16)
            {
                mediumBookCount = Math.Min(1, (trainingHours - 16) / 1);
            }

            if (trainingHours >= 24)
            {
                largeBookCount = Math.Min(1, (trainingHours - 24) / 1);
            }

            double TotalHR = Math.Min(character.MaxHP, character.HR * diff.TotalSeconds);
            double TotalMR = Math.Min(character.MaxMP, character.MR * diff.TotalSeconds);

            return $"练级时长：{totalMinutes} 分钟，{(isPre ? "预计可" : "")}获得：{totalExperience} 点经验值，{smallBookCount} 本小经验书，{mediumBookCount} 本中经验书，{largeBookCount} 本大经验书。" +
                $"回复角色 {TotalHR:0.##} 点生命值和 {TotalMR:0.##} 点魔法值。" +
                $"{(isPre ? "练级时间上限 1440 分钟（24小时），超时将不会再产生收益，请按时领取奖励！" : "")}";
        }

        public static string GetSkillLevelUpNeedy(int level)
        {
            if (FunGameConstant.SkillLevelUpList.TryGetValue(level, out Dictionary<string, int>? needy) && needy != null && needy.Count > 0)
            {
                return GetNeedyInfo(needy);
            }
            return "";
        }

        public static string GetNormalAttackLevelUpNeedy(int level)
        {
            if (FunGameConstant.NormalAttackLevelUpList.TryGetValue(level, out Dictionary<string, int>? needy) && needy != null && needy.Count > 0)
            {
                return GetNeedyInfo(needy);
            }
            return "";
        }

        public static string GetNeedyInfo(Dictionary<string, int> needy)
        {
            string str = "";
            foreach (string key in needy.Keys)
            {
                int needCount = needy[key];
                if (str != "")
                {
                    str += "，";
                }
                if (key == "角色等级")
                {
                    str += $"角色等级 {needCount} 级";
                }
                else if (key == "角色突破进度")
                {
                    str += $"角色突破进度 {needCount} 等阶";
                }
                else
                {
                    str += $"{key} * {needCount}";
                }
            }
            return str;
        }

        public static void GenerateBoss()
        {
            if (Bosses.Count < 10)
            {
                int genCount = 10 - Bosses.Count;

                Item[] weapons = [.. FunGameConstant.Equipment.Where(i => i.Id.ToString().StartsWith("11") && (int)i.QualityType == 5)];
                Item[] armors = [.. FunGameConstant.Equipment.Where(i => i.Id.ToString().StartsWith("12") && (int)i.QualityType == 5)];
                Item[] shoes = [.. FunGameConstant.Equipment.Where(i => i.Id.ToString().StartsWith("13") && (int)i.QualityType == 5)];
                Item[] accessory = [.. FunGameConstant.Equipment.Where(i => i.Id.ToString().StartsWith("14") && (int)i.QualityType == 5)];
                Item[] consumables = [.. FunGameConstant.AllItems.Where(i => i.ItemType == ItemType.Consumable && i.IsInGameItem)];
                for (int i = 0; i < genCount; i++)
                {
                    int nowIndex = Bosses.Count > 0 ? Bosses.Keys.Max() + 1 : 1;
                    string bossName = GenerateRandomChineseUserName();
                    CustomCharacter boss = new(nowIndex, bossName, "", bossName);
                    int cutRate = Random.Shared.Next(3) switch
                    {
                        0 => 1,
                        1 => 2,
                        _ => 4,
                    };
                    int cLevel = General.GameplayEquilibriumConstant.MaxLevel / cutRate;
                    int sLevel = General.GameplayEquilibriumConstant.MaxSkillLevel / cutRate;
                    int mLevel = General.GameplayEquilibriumConstant.MaxMagicLevel / cutRate;
                    int naLevel = General.GameplayEquilibriumConstant.MaxNormalAttackLevel / cutRate;
                    boss.NormalAttack.ExHardnessTime = -4;
                    EnhanceBoss(boss, weapons, armors, shoes, accessory, consumables, cLevel, sLevel, mLevel, naLevel);

                    Bosses[nowIndex] = boss;
                }
            }
        }

        private static void EnhanceBoss(Character boss, Item[] weapons, Item[] armors, Item[] shoes, Item[] accessory, Item[] consumables,
            int cLevel, int sLevel, int mLevel, int naLevel, bool enhanceHPMP = true, bool enhanceCRCRD = true, bool isUnit = false)
        {
            boss.Level = cLevel;
            boss.NormalAttack.Level = naLevel;
            Item? a = null, b = null, c = null, d = null, d2 = null;
            if (weapons.Length > 0)
            {
                a = weapons[Random.Shared.Next(weapons.Length)];
            }
            if (armors.Length > 0)
            {
                b = armors[Random.Shared.Next(armors.Length)];
            }
            if (shoes.Length > 0)
            {
                c = shoes[Random.Shared.Next(shoes.Length)];
            }
            if (accessory.Length > 0)
            {
                d = accessory[Random.Shared.Next(accessory.Length)];
            }
            if (accessory.Length > 0)
            {
                d2 = accessory[Random.Shared.Next(accessory.Length)];
            }
            List<Item> dropItems = [];
            if (a != null) dropItems.Add(a);
            if (b != null) dropItems.Add(b);
            if (c != null) dropItems.Add(c);
            if (d != null) dropItems.Add(d);
            if (d2 != null) dropItems.Add(d2);
            Item? magicCardPack = GenerateMagicCardPack(5, (QualityType)4);
            if (magicCardPack != null)
            {
                magicCardPack.QualityType = QualityType.Gold;
                foreach (Skill magic in magicCardPack.Skills.Magics)
                {
                    magic.Level = mLevel;
                }
                boss.Equip(magicCardPack);
            }
            foreach (Item item in dropItems)
            {
                Item realItem = item.Copy();
                boss.Equip(realItem);
            }
            if (consumables.Length > 0 && boss.Items.Count < 5)
            {
                for (int j = 0; j < 2; j++)
                {
                    Item consumable = consumables[Random.Shared.Next(consumables.Length)].Copy();
                    boss.Items.Add(consumable);
                }
            }
            Skill bossSkill = Factory.OpenFactory.GetInstance<Skill>(0, "BOSS专属被动", []);
            bossSkill.Level = 1;
            bossSkill.Character = boss;
            double exHP2 = 1.5;
            double exMP2 = 0.8;
            if (!enhanceHPMP)
            {
                if (isUnit) exHP2 = -0.1;
                else exHP2 = 0.5;
                exMP2 = 0.2;
            }
            double exCR = 0.35;
            double exCRD = 0.9;
            if (!enhanceCRCRD)
            {
                exCR = 0.15;
                exCRD = 0.4;
            }
            Effect effect = Factory.OpenFactory.GetInstance<Effect>((long)EffectID.DynamicsEffect, "", new()
            {
                { "skill", bossSkill },
                {
                    "values",
                    new Dictionary<string, object>()
                    {
                        { "exatk", isUnit ? 1.4 * cLevel : 3.4 * cLevel },
                        { "exdef", isUnit ? 1.4 * cLevel : 3.4 * cLevel },
                        { "exhp2", exHP2 },
                        { "exmp2", exMP2 },
                        { "exhr", 0.15 * cLevel },
                        { "exmr", 0.1 * cLevel },
                        { "excr", exCR },
                        { "excrd", exCRD },
                        { "excdr", isUnit ? 0.2 : 0.25 },
                        { "exacc", isUnit ? 0.15 : 0.25 }
                    }
                }
            });
            effect.OnEffectGained(boss);
            bossSkill.Effects.Add(effect);
            boss.Skills.Add(bossSkill);
            effect = Factory.OpenFactory.GetInstance<Effect>((long)EffectID.ExMDF, "", new()
            {
                { "skill", bossSkill },
                {
                    "values",
                    new Dictionary<string, object>()
                    {
                        { "mdftype", 0 },
                        { "mdfvalue", 0.15 }
                    }
                }
            });
            effect.OnEffectGained(boss);
            bossSkill.Effects.Add(effect);
            boss.Skills.Add(bossSkill);
            Skill passive = Factory.OpenFactory.GetInstance<Skill>(Random.Shared.Next(4001, 4013), "", []);
            passive.Character = boss;
            passive.Level = 1;
            boss.Skills.Add(passive);
            Skill super = Factory.OpenFactory.GetInstance<Skill>(Random.Shared.Next(3001, 3013), "", []);
            super.Character = boss;
            super.Level = sLevel;
            boss.Skills.Add(super);

            boss.Recovery();
            SetCharacterPrimaryAttribute(boss);
        }

        public static Dictionary<int, List<Skill>> GenerateRoundRewards(int maxRound)
        {
            Dictionary<int, List<Skill>> roundRewards = [];

            int currentRound = 1;
            while (currentRound <= maxRound)
            {
                currentRound += Random.Shared.Next(1, 9);

                if (currentRound <= maxRound)
                {
                    List<Skill> skills = [];

                    // 添加回合奖励特效
                    long effectID = (long)FunGameConstant.RoundRewards.Keys.ToArray()[Random.Shared.Next(FunGameConstant.RoundRewards.Count)];
                    Dictionary<string, object> args = [];
                    if (effectID > (long)EffectID.Active_Start)
                    {
                        args.Add("active", true);
                        args.Add("self", true);
                        args.Add("enemy", false);
                    }

                    skills.Add(Factory.OpenFactory.GetInstance<Skill>(effectID, "回合奖励", args));

                    roundRewards[currentRound] = skills;
                }
            }

            return roundRewards;
        }

        public static double CalculateRating(CharacterStatistics stats, Team? team = null, CharacterStatistics[]? teammateStats = null)
        {
            // 基础得分
            double baseScore = (stats.Kills + stats.Assists) / (stats.Kills + stats.Assists + stats.Deaths + 0.01);
            if (team is null)
            {
                baseScore += stats.Kills * 0.1;
                baseScore -= stats.Deaths * 0.05;
            }
            else
            {
                baseScore = baseScore * 0.6 + 0.4 * (stats.Kills / (stats.Kills + stats.Deaths + 0.01));
            }

            // 伤害贡献
            double logDamageContribution = Math.Log(1 + (stats.TotalDamage / (stats.TotalTakenDamage + 1.75)));
            if (team != null && teammateStats != null)
            {
                // 考虑团队伤害排名，优先高伤害的
                int damageRank = teammateStats.OrderByDescending(s => s.TotalDamage).ToList().IndexOf(stats) + 1;
                if (damageRank > 1)
                {
                    double d = 1 - (0.1 * (damageRank - 1));
                    if (d < 0.1) d = 0.1;
                    logDamageContribution *= d;
                }
            }

            // 存活时间贡献
            double liveTimeContribution = Math.Log(1 + (stats.LiveTime / (stats.TotalTakenDamage + 0.01) * 100));

            // 团队模式参团率加成
            double teamContribution = 0;
            if (team != null)
            {
                if (stats.Assists > team.Score) stats.Assists = team.Score;
                teamContribution = (stats.Kills + stats.Assists) / (team.Score + 0.01);
                if (team.IsWinner)
                {
                    teamContribution += 0.15;
                }
            }

            // 权重设置
            double k = stats.Deaths > 0 ? 0.2 : 0.075; // 伤害贡献权重
            double l = stats.Deaths > 0 ? 0.2 : 0.05; // 存活时间权重
            double t = stats.Deaths > 0 ? 0.2 : 0.075; // 参团率权重

            // 计算最终评分
            double rating = baseScore + k * logDamageContribution + l * liveTimeContribution + t * teamContribution;

            // 确保评分在合理范围内
            return Math.Max(0.01, rating);
        }

        public static void GetCharacterRating(Dictionary<Character, CharacterStatistics> statistics, bool isTeam, List<Team> teams)
        {
            foreach (Character character in statistics.Keys)
            {
                Team? team = null;
                CharacterStatistics[]? teammateStats = null;
                if (isTeam)
                {
                    team = teams.Where(t => t.IsOnThisTeam(character)).FirstOrDefault();
                    if (team != null)
                    {
                        teammateStats = [.. statistics.Where(kv => team.Members.Contains(kv.Key)).Select(kv => kv.Value)];
                    }
                }
                statistics[character].Rating = CalculateRating(statistics[character], team, teammateStats);
            }
        }

        public static string CheckQuestList(EntityModuleConfig<Quest> quests)
        {
            if (quests.Count == 0)
            {
                // 生成任务
                for (int i = 0; i < 6; i++)
                {
                    QuestType type = (QuestType)Random.Shared.Next(3);
                    long id = quests.Count > 0 ? quests.Values.Max(q => q.Id) + 1 : 1;

                    // 定义概率
                    Dictionary<QualityType, double> pE = new()
                    {
                        { QualityType.Blue, 0.5 },
                        { QualityType.Purple, 0.37 },
                        { QualityType.Orange, 0.1 },
                        { QualityType.Red, 0.03 },
                    };

                    // 生成任务奖励物品
                    QualityType qualityType = QualityType.Blue;
                    foreach (QualityType qt in pE.Keys.OrderByDescending(q => (int)q))
                    {
                        if (Random.Shared.NextDouble() <= pE[qt])
                        {
                            qualityType = qt;
                            break;
                        }
                    }

                    HashSet<Item> items = [];
                    Dictionary<string, int> itemsCount = [];
                    Item? item = FunGameConstant.DrawCardItems.Where(i => qualityType == QualityType.Blue ? (int)i.QualityType <= (int)qualityType : (int)i.QualityType == (int)qualityType).OrderBy(o => Random.Shared.Next()).FirstOrDefault();
                    item ??= FunGameConstant.DrawCardItems.OrderBy(o => Random.Shared.Next()).First();
                    items.Add(item);
                    itemsCount[item.Name] = 1;

                    foreach (QualityType qt in pE.Keys.OrderByDescending(q => (int)q))
                    {
                        if (Random.Shared.NextDouble() <= pE[qt])
                        {
                            qualityType = qt;
                            break;
                        }
                    }

                    Item? item2 = FunGameConstant.DrawCardItems.Where(i => qualityType == QualityType.Blue ? (int)i.QualityType <= (int)qualityType : (int)i.QualityType == (int)qualityType).OrderBy(o => Random.Shared.Next()).FirstOrDefault();
                    item2 ??= FunGameConstant.DrawCardItems.OrderBy(o => Random.Shared.Next()).First();
                    items.Add(item2);
                    if (!itemsCount.TryAdd(item2.Name, 1))
                    {
                        itemsCount[item2.Name]++;
                    }

                    // 随机筛选地区
                    OshimaRegion region = FunGameConstant.Regions.OrderBy(o => Random.Shared.Next()).First();

                    Quest quest;
                    if (type == QuestType.Continuous)
                    {
                        string name = region.ContinuousQuestList.Keys.OrderBy(o => Random.Shared.Next()).First();
                        QuestExploration exploration = region.ContinuousQuestList[name];
                        int minutes = Random.Shared.Next(10, 41);
                        quest = new()
                        {
                            Id = id,
                            Name = name,
                            Description = exploration.Description,
                            RegionId = region.Id,
                            NeedyExploreItemName = exploration.Item,
                            QuestType = QuestType.Continuous,
                            EstimatedMinutes = minutes,
                            CreditsAward = minutes * 40,
                            MaterialsAward = minutes / 4,
                            Awards = items,
                            AwardsCount = itemsCount
                        };
                    }
                    else if (type == QuestType.Immediate)
                    {
                        string name = region.ImmediateQuestList.Keys.OrderBy(o => Random.Shared.Next()).First();
                        QuestExploration exploration = region.ImmediateQuestList[name];
                        int difficulty = Random.Shared.Next(3, 11);
                        quest = new()
                        {
                            Id = id,
                            Name = name,
                            Description = exploration.Description,
                            RegionId = region.Id,
                            NeedyExploreItemName = exploration.Item,
                            QuestType = QuestType.Immediate,
                            CreditsAward = difficulty * 160,
                            MaterialsAward = difficulty,
                            Awards = items,
                            AwardsCount = itemsCount
                        };
                    }
                    else
                    {
                        string name = region.ProgressiveQuestList.Keys.OrderBy(o => Random.Shared.Next()).First();
                        QuestExploration exploration = region.ProgressiveQuestList[name];
                        int maxProgress = Random.Shared.Next(3, 11);
                        quest = new()
                        {
                            Id = id,
                            Name = name,
                            Description = string.Format(exploration.Description, maxProgress),
                            RegionId = region.Id,
                            NeedyExploreItemName = exploration.Item,
                            QuestType = QuestType.Progressive,
                            Progress = 0,
                            MaxProgress = maxProgress,
                            CreditsAward = maxProgress * 160,
                            MaterialsAward = maxProgress,
                            Awards = items,
                            AwardsCount = itemsCount,
                            Status = QuestState.InProgress
                        };
                    }

                    quests.Add(quest.GetIdName(), quest);
                }
                return "☆--- 今日任务列表 ---☆\r\n" + string.Join("\r\n", quests.Values.Select(q => q.ToString())) + "\r\n温馨提示：使用【做任务+任务序号】指令来进行任务。\r\n请务必在次日 4:00 前完成任务结算，未结算的任务都会被取消！";
            }
            else
            {
                if (quests.Count > 0)
                {
                    return "☆--- 今日任务列表 ---☆\r\n" + string.Join("\r\n", quests.Values.Select(q => q.ToString())) + "\r\n温馨提示：使用【做任务+任务序号】指令来进行任务。\r\n请务必在次日 4:00 前完成任务结算，未结算的任务都会被取消！";
                }
                else
                {
                    return "任务列表为空，请使用【任务列表】指令来获取任务列表！";
                }
            }
        }

        public static bool SettleQuest(User user, EntityModuleConfig<Quest> quests)
        {
            bool result = false;
            IEnumerable<Quest> workingQuests = quests.Values.Where(q => q.QuestType == QuestType.Continuous && q.Status == QuestState.InProgress);
            foreach (Quest quest in workingQuests)
            {
                if (quest.StartTime.HasValue && quest.StartTime.Value.AddMinutes(quest.EstimatedMinutes) <= DateTime.Now)
                {
                    quest.Status = QuestState.Completed;
                    result = true;
                }
            }
            if (UserExploreItemCache.TryGetValue(user.Id, out List<string>? value) && value != null && value.Count > 0)
            {
                // 从缓存中获取收集的物品
                List<string> willRemove = [];
                foreach (string item in value.Distinct())
                {
                    IEnumerable<string> items = value.Where(str => str == item);
                    IEnumerable<Quest> progressiveQuests = quests.Values.Where(q => q.QuestType == QuestType.Progressive && q.Status == QuestState.InProgress);
                    foreach (Quest quest in progressiveQuests)
                    {
                        if (quest.NeedyExploreItemName == item)
                        {
                            result = true;
                            quest.Progress += items.Count();
                            if (quest.Progress >= quest.MaxProgress)
                            {
                                quest.Progress = quest.MaxProgress;
                                quest.Status = QuestState.Completed;
                            }
                        }
                    }
                    willRemove.Add(item);
                }
                value.RemoveAll(willRemove.Contains);
            }
            IEnumerable<Quest> finishQuests = quests.Values.Where(q => q.Status == QuestState.Completed);
            foreach (Quest quest in finishQuests)
            {
                quest.Status = QuestState.Settled;
                quest.SettleTime = DateTime.Now;
                if (quest.CreditsAward > 0)
                {
                    user.Inventory.Credits += quest.CreditsAward;
                }
                if (quest.MaterialsAward > 0)
                {
                    user.Inventory.Materials += quest.MaterialsAward;
                }
                foreach (Item item in quest.Awards)
                {
                    if (quest.AwardsCount.TryGetValue(item.Name, out int qty))
                    {
                        for (int i = 0; i < qty; i++)
                        {
                            if (FunGameConstant.AllItems.FirstOrDefault(i => i.Name == item.Name) != null)
                            {
                                Item newItem = item.Copy();
                                newItem.User = user;
                                if (newItem.QualityType >= QualityType.Orange) newItem.IsLock = true;
                                SetSellAndTradeTime(newItem);
                                user.Inventory.Items.Add(newItem);
                                // 连接到任务系统
                                AddExploreItemCache(user.Id, item.Name);
                                // 连接到活动系统
                                ActivitiesItemCache.Add(item.Name);
                            }
                        }
                    }
                }
                TaskUtility.NewTask(async () => await AnonymousServer.PushMessageToClients(user.AutoKey, $"FunGame Web API 推送：你的任务【{quest.Name}】已结算，" +
                    $"获得奖励：【{quest.AwardsString}】!"));
                result = true;
            }
            return result;
        }

        public static void AddExploreItemCache(long userid, string item)
        {
            if (UserExploreItemCache.TryGetValue(userid, out List<string>? value) && value != null)
            {
                value.Add(item);
            }
            else
            {
                UserExploreItemCache[userid] = [item];
            }
        }

        public static string CheckDailyStore(EntityModuleConfig<Store> store, User? user = null)
        {
            if (store.Count == 0)
            {
                // 生成每日商店
                Store daily = new($"{(user != null ? user.Username + "的" : "")}每日商店");
                for (int i = 0; i < 4; i++)
                {
                    Item item;
                    if (Random.Shared.Next(3) < 1)
                    {
                        item = GenerateMagicCard((QualityType)Random.Shared.Next((int)QualityType.Gold + 1));
                    }
                    else
                    {
                        int index = Random.Shared.Next(FunGameConstant.AllItems.Count);
                        item = FunGameConstant.AllItems[index].Copy();
                    }
                    item.Character = null;
                    (int min, int max) = (0, 0);
                    if (FunGameConstant.PriceRanges.TryGetValue(item.QualityType, out (int Min, int Max) range))
                    {
                        (min, max) = (range.Min, range.Max);
                    }
                    double price = Random.Shared.Next(min, max);
                    int stock = Random.Shared.Next(1, 3);
                    if (item.ItemType == ItemType.MagicCard)
                    {
                        price *= 0.7;
                        stock = 1;
                    }
                    else if (item.ItemType == ItemType.Consumable)
                    {
                        int prev = (int)item.QualityType - 1;
                        int current = (int)item.QualityType;
                        min = 300 * (1 + (prev * prev - prev));
                        max = 300 * (1 + (current * current - current));
                        price = Random.Shared.Next(min, max);
                        stock += 3;
                    }
                    if (price == 0)
                    {
                        price = (Random.Shared.NextDouble() + 0.1) * Random.Shared.Next(1000, 5000) * Random.Shared.Next((int)item.QualityType + 2, 6 + ((int)item.QualityType));
                    }
                    item.Price = (int)price;
                    daily.AddItem(item, stock);
                }
                store.Add("daily", daily);
                return daily.ToString() + "\r\n温馨提示：使用【商店查看+序号】查看物品详细信息，使用【商店购买+序号】购买物品！每天 4:00 刷新每日商店。";
            }
            else
            {
                if (store.Count > 0 && store.Where(kv => kv.Key == "daily").Select(kv => kv.Value).FirstOrDefault() is Store daily)
                {
                    return daily.ToString() + "\r\n温馨提示：使用【商店查看+序号】查看物品详细信息，使用【商店购买+序号】购买物品！每天 4:00 刷新每日商店。";
                }
                else
                {
                    return "商品列表为空，请使用【每日商店】指令来获取商品列表！";
                }
            }
        }

        public static string StoreBuyItem(Store store, Goods goods, User user, int count)
        {
            string msg = "";
            if (goods.Stock - count < 0)
            {
                return msg = $"此商品【{goods.Name}】库存不足，无法购买！\r\n你想要购买 {count} 件，但库存只有 {goods.Stock} 件。";
            }

            foreach (string needy in goods.Prices.Keys)
            {
                if (needy == General.GameplayEquilibriumConstant.InGameCurrency)
                {
                    double reduce = Calculation.Round2Digits(goods.Prices[needy] * count);
                    if (user.Inventory.Credits >= reduce)
                    {
                        user.Inventory.Credits -= reduce;
                    }
                    else
                    {
                        return NetworkUtility.JsonSerialize($"你的{General.GameplayEquilibriumConstant.InGameCurrency}不足 {reduce} 呢，无法购买【{goods.Name}】！");
                    }
                }
                else if (needy == General.GameplayEquilibriumConstant.InGameMaterial)
                {
                    double reduce = Calculation.Round2Digits(goods.Prices[needy] * count);
                    if (user.Inventory.Materials >= reduce)
                    {
                        user.Inventory.Materials -= reduce;
                    }
                    else
                    {
                        return NetworkUtility.JsonSerialize($"你的{General.GameplayEquilibriumConstant.InGameMaterial}不足 {reduce} 呢，无法购买【{goods.Name}】！");
                    }
                }
            }

            foreach (Item item in goods.Items)
            {
                for (int i = 0; i < count; i++)
                {
                    Item newItem = item.Copy(true);
                    if (newItem.QualityType >= QualityType.Orange) newItem.IsLock = true;
                    SetSellAndTradeTime(newItem);
                    if (goods.GetPrice(General.GameplayEquilibriumConstant.InGameCurrency, out double price) && price > 0)
                    {
                        newItem.Price = Calculation.Round2Digits(price * 0.35);
                    }
                    newItem.User = user;
                    user.Inventory.Items.Add(newItem);
                    // 连接到任务系统
                    AddExploreItemCache(user.Id, item.Name);
                    // 连接到活动系统
                    ActivitiesItemCache.Add(item.Name);
                }
            }

            goods.Stock -= count;

            msg += $"恭喜你成功购买 {count} 件【{goods.Name}】！\r\n" +
                $"总计消费：{(goods.Prices.Count > 0 ? string.Join("、", goods.Prices.Select(kv => $"{kv.Value * count:0.##} {kv.Key}")) : "免单")}\r\n" +
                $"包含物品：{string.Join("、", goods.Items.Select(i => $"[{ItemSet.GetQualityTypeName(i.QualityType)}|{ItemSet.GetItemTypeName(i.ItemType)}] {i.Name} * {count}"))}";

            return msg;
        }

        public static string Select_CheckAutoKey(SQLHelper SQLHelper, string AutoKey)
        {
            SQLHelper.Parameters["@AutoKey"] = AutoKey;
            return $"{Milimoe.FunGame.Core.Library.SQLScript.Entity.UserQuery.Select_Users} {Milimoe.FunGame.Core.Library.SQLScript.Constant.Command_Where} {Milimoe.FunGame.Core.Library.SQLScript.Entity.UserQuery.Column_AutoKey} = @AutoKey";
        }

        public static void SetCharacterPrimaryAttribute(Character character, PrimaryAttribute? value = null)
        {
            if (value != null && value.HasValue)
            {
                character.PrimaryAttribute = value.Value;
            }
            else
            {
                double max = Math.Max(Math.Max(character.STR, character.AGI), character.INT);
                if (max == character.STR)
                {
                    character.PrimaryAttribute = PrimaryAttribute.STR;
                }
                else if (max == character.AGI)
                {
                    character.PrimaryAttribute = PrimaryAttribute.AGI;
                }
                else if (max == character.INT)
                {
                    character.PrimaryAttribute = PrimaryAttribute.INT;
                }
            }
        }

        public static async Task UpdateRegionWeather()
        {
            foreach (Region region in FunGameConstant.Regions.Union(FunGameConstant.PlayerRegions))
            {
                region.ChangeRandomWeather();
            }
            await Task.CompletedTask;
        }

        public static string GetEventCenter()
        {
            EntityModuleConfig<Activity> activities = new("activities", "activities");
            activities.LoadConfig();
            if (activities.Count == 0)
            {
                return "当前没有任何活动，敬请期待。";
            }
            lock (Activities)
            {
                Activities.Clear();
                foreach (Activity activity in activities.Values)
                {
                    Activities.Add(activity);
                }
                bool update = false;
                if (ActivitiesCharacterCache.Count > 0)
                {
                    List<string> willRemove = [];
                    IEnumerable<Activity> activityList = Activities.Where(a => a.Status == ActivityState.InProgress);
                    foreach (string item in ActivitiesCharacterCache.Distinct())
                    {
                        IEnumerable<string> items = ActivitiesCharacterCache.Where(str => str == item);
                        foreach (Quest quest in activityList.SelectMany(a => a.Quests).Where(q => q.Status == QuestState.InProgress))
                        {
                            if (quest.NeedyExploreCharacterName == item)
                            {
                                update = true;
                                quest.Progress += items.Count();
                                if (quest.Progress >= quest.MaxProgress)
                                {
                                    quest.Progress = quest.MaxProgress;
                                    quest.Status = QuestState.Completed;
                                }
                            }
                        }
                        willRemove.Add(item);
                    }
                    ActivitiesCharacterCache.RemoveAll(willRemove.Contains);
                }
                if (ActivitiesItemCache.Count > 0)
                {
                    List<string> willRemove = [];
                    IEnumerable<Activity> activityList = Activities.Where(a => a.Status == ActivityState.InProgress);
                    foreach (string item in ActivitiesItemCache.Distinct())
                    {
                        IEnumerable<string> items = ActivitiesItemCache.Where(str => str == item);
                        foreach (Quest quest in activityList.SelectMany(a => a.Quests).Where(q => q.Status == QuestState.InProgress))
                        {
                            if (quest.NeedyExploreItemName == item)
                            {
                                update = true;
                                quest.Progress += items.Count();
                                if (quest.Progress >= quest.MaxProgress)
                                {
                                    quest.Progress = quest.MaxProgress;
                                    quest.Status = QuestState.Completed;
                                }
                            }
                        }
                        willRemove.Add(item);
                    }
                    ActivitiesItemCache.RemoveAll(willRemove.Contains);
                }
                if (ActivitiesEventCache.Count > 0)
                {
                    List<string> willRemove = [];
                    IEnumerable<Activity> activityList = Activities.Where(a => a.Status == ActivityState.InProgress);
                    foreach (string item in ActivitiesEventCache.Distinct())
                    {
                        IEnumerable<string> items = ActivitiesEventCache.Where(str => str == item);
                        foreach (Quest quest in activityList.SelectMany(a => a.Quests).Where(q => q.Status == QuestState.InProgress))
                        {
                            if (quest.NeedyExploreEventName == item)
                            {
                                update = true;
                                quest.Progress += items.Count();
                                if (quest.Progress >= quest.MaxProgress)
                                {
                                    quest.Progress = quest.MaxProgress;
                                    quest.Status = QuestState.Completed;
                                }
                            }
                        }
                        willRemove.Add(item);
                    }
                    ActivitiesEventCache.RemoveAll(willRemove.Contains);
                }
                if (update)
                {
                    foreach (Activity activity in Activities)
                    {
                        activities.Add(activity.Id.ToString(), activity);
                    }
                    activities.SaveConfig();
                }
            }
            StringBuilder builder = new();
            builder.AppendLine("★☆★ 活动中心 ★☆★");

            ActivityState[] status = [ActivityState.InProgress, ActivityState.Upcoming, ActivityState.Future, ActivityState.Ended];
            foreach (ActivityState state in status)
            {
                IEnumerable<Activity> filteredActivities = activities.Values.Where(a => a.Status == state);
                if (filteredActivities.Any())
                {
                    builder.AppendLine($"【{CommonSet.GetActivityStatus(state)}】");
                    builder.AppendLine($"{string.Join("\r\n", filteredActivities.Select(a => a.GetIdName() + $"（{a.GetTimeString(false)}）"))}");
                }
            }

            builder.AppendLine("请使用【查活动+活动序号】指令查询活动详细信息。");

            return builder.ToString().Trim();
        }

        public static string GetEvents(ActivityState status = ActivityState.InProgress)
        {
            GetEventCenter();
            IEnumerable<Activity> filteredActivities = Activities.Where(a => a.Status == status);
            if (!filteredActivities.Any())
            {
                return $"当前没有任何{CommonSet.GetActivityStatus(status)}的活动，敬请期待。";
            }
            return $"★☆★ {CommonSet.GetActivityStatus(status)}活动列表 ★☆★\r\n" +
                $"{string.Join("\r\n", filteredActivities.Select(a => a.GetIdName() + $"（{a.GetTimeString(false)}）"))}\r\n" +
                $"请使用【查活动+活动序号】指令查询活动详细信息。";
        }

        public static string GetEvent(long id)
        {
            GetEventCenter();
            if (Activities.FirstOrDefault(a => a.Id == id) is Activity activity)
            {
                return $"{activity}";
            }
            return "该活动不存在。";
        }

        public static string AddEvent(Activity activity)
        {
            EntityModuleConfig<Activity> activities = new("activities", "activities");
            activities.LoadConfig();
            activity.UpdateState();
            activities.Add(activity.Id.ToString(), activity);
            activities.SaveConfig();
            return "该活动已添加！";
        }

        public static string RemoveEvent(Activity activity)
        {
            EntityModuleConfig<Activity> activities = new("activities", "activities");
            activities.LoadConfig();
            activities.Remove(activity.Id.ToString());
            activities.SaveConfig();
            return "该活动已删除！";
        }

        public static string GetSquadInfo(IEnumerable<Character> inventory, IEnumerable<long> squadIds, string separator = "\r\n")
        {
            Character[] squad = [.. inventory.Where(c => squadIds.Contains(c.Id))];
            Dictionary<Character, int> characters = inventory
                .Select((character, index) => new { character, index })
                .ToDictionary(x => x.character, x => x.index + 1);
            return $"{(squad.Length > 0 ? string.Join(separator, squad.Select(c => $"#{characters[c]}. {c.ToStringWithLevelWithOutUser()}")) : "空")}";
        }

        public static string GetCharacterGroupInfoByInventorySequence(IEnumerable<Character> inventory, IEnumerable<long> characterIds, string separator = "\r\n")
        {
            Dictionary<Character, int> characters = [];
            Character[] loop = [.. inventory];
            for (int i = 1; i <= loop.Length; i++)
            {
                if (characterIds.Contains(i))
                {
                    characters[loop[i - 1]] = i;
                }
            }
            return $"{(characters.Count > 0 ? string.Join(separator, characters.Keys.Select(c => $"#{characters[c]}. {c.ToStringWithLevelWithOutUser()}")) : "空")}";
        }

        public static async Task GenerateExploreModel(ExploreModel model, OshimaRegion region, long[] characterIds, User user)
        {
            int characterCount = characterIds.Length;
            int diff = region.Difficulty switch
            {
                RarityType.OneStar => 1,
                RarityType.TwoStar => 2,
                RarityType.ThreeStar => 3,
                RarityType.FourStar => 4,
                _ => 5
            };

            // 直接保存探索奖励，但是要等到探索结束后发放
            double randomDouble = Random.Shared.NextDouble();
            Dictionary<ExploreResult, double> probabilities = new(FunGameConstant.ExploreResultProbabilities);
            switch (diff)
            {
                case 2:
                    probabilities[ExploreResult.General] -= 0.1;
                    probabilities[ExploreResult.Fight] += 0.1;
                    break;
                case 3:
                    probabilities[ExploreResult.General] -= 0.15;
                    probabilities[ExploreResult.Earned] -= 0.05;
                    probabilities[ExploreResult.Nothing] += 0.05;
                    probabilities[ExploreResult.Fight] += 0.15;
                    break;
                case 4:
                    probabilities[ExploreResult.General] -= 0.3;
                    probabilities[ExploreResult.Earned] -= 0.05;
                    probabilities[ExploreResult.Fight] += 0.35;
                    break;
                case 5:
                    probabilities[ExploreResult.General] -= 0.35;
                    probabilities[ExploreResult.Earned] -= 0.05;
                    probabilities[ExploreResult.Nothing] -= 0.05;
                    probabilities[ExploreResult.Fight] += 0.45;
                    break;
                default:
                    break;
            }
            double cumulative = 0;
            model.Result = ExploreResult.Nothing;
            foreach (ExploreResult key in probabilities.Keys)
            {
                cumulative += probabilities[key];
                if (randomDouble <= cumulative)
                {
                    model.Result = key;
                    break;
                }
            }
            string exploreString = FunGameConstant.ExploreString[model.Result].OrderBy(o => Random.Shared.Next()).First();

            // 出现的NPC
            int random = Random.Shared.Next(region.NPCs.Count + 1);
            string npc = random == region.NPCs.Count ? GenerateRandomChineseUserName() : region.NPCs[random];

            // 探索的子区域
            random = Random.Shared.Next(region.Areas.Count);
            string area1 = region.Areas[random];
            random = Random.Shared.Next(region.Areas.Count);
            string area2 = region.Areas[random];

            // 出现的物品
            List<Item> items = [.. region.Crops.Union(region.Items)];
            random = Random.Shared.Next(items.Count);
            string item1 = items[random].Name;
            random = Random.Shared.Next(items.Count);
            string item2 = items[random].Name;

            // 筛选敌人
            List<Character> enemys = [];
            Character enemy;
            bool isUnit = Random.Shared.Next(2) != 0;
            if (region.Characters.Count > 0 && region.Units.Count > 0)
            {
                if (!isUnit)
                {
                    enemy = region.Characters.OrderBy(o => Random.Shared.Next()).First().Copy();
                    enemy.ExHPPercentage += 0.5;
                    enemys.Add(enemy);
                }
                else
                {
                    switch (diff)
                    {
                        case 1:
                        case 2:
                            enemy = region.Units.OrderBy(o => Random.Shared.Next()).First().Copy();
                            enemys.Add(enemy);
                            break;
                        case 3:
                        case 4:
                            enemy = region.Units.OrderBy(o => Random.Shared.Next()).First().Copy();
                            enemys.Add(enemy);
                            enemy = region.Units.OrderBy(o => Random.Shared.Next()).First().Copy();
                            enemy.FirstName = enemys.Any(e => e.Name == enemy.Name) ? "2" : "";
                            enemys.Add(enemy);
                            break;
                        case 5:
                        default:
                            enemy = region.Units.OrderBy(o => Random.Shared.Next()).First().Copy();
                            enemys.Add(enemy);
                            enemy = region.Units.OrderBy(o => Random.Shared.Next()).First().Copy();
                            enemy.FirstName = enemys.Any(e => e.Name == enemy.Name) ? "α" : "";
                            enemys.Add(enemy);
                            enemy = region.Units.OrderBy(o => Random.Shared.Next()).First().Copy();
                            enemy.FirstName = enemys.Any(e => e.Name == enemy.Name) ? "β" : "";
                            enemys.Add(enemy);
                            break;
                    }
                }
            }
            else
            {
                if (!isUnit)
                {
                    enemy = new RegionCharacter(long.Parse(Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 8)), GenerateRandomChineseUserName());
                    enemys.Add(enemy);
                }
                else
                {
                    switch (diff)
                    {
                        case 1:
                        case 2:
                            enemy = new RegionUnit(long.Parse(Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 8)), GenerateRandomChineseUserName());
                            enemys.Add(enemy);
                            break;
                        case 3:
                        case 4:
                            enemy = new RegionUnit(long.Parse(Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 8)), GenerateRandomChineseUserName());
                            enemys.Add(enemy);
                            enemy = new RegionUnit(long.Parse(Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 8)), GenerateRandomChineseUserName());
                            enemys.Add(enemy);
                            break;
                        case 5:
                        default:
                            enemy = new RegionUnit(long.Parse(Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 8)), GenerateRandomChineseUserName());
                            enemys.Add(enemy);
                            enemy = new RegionUnit(long.Parse(Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 8)), GenerateRandomChineseUserName());
                            enemys.Add(enemy);
                            enemy = new RegionUnit(long.Parse(Verification.CreateVerifyCode(VerifyCodeType.NumberVerifyCode, 8)), GenerateRandomChineseUserName());
                            enemys.Add(enemy);
                            break;
                    }
                }
            }

            // 初始化掉落装备的概率
            Dictionary<QualityType, double> pE = new()
            {
                { QualityType.Blue, 0.5 },
                { QualityType.Purple, 0.37 + (0.01 * (characterCount - 1)) },
                { QualityType.Orange, 0.1 + (0.01 * (characterCount - 1)) },
                { QualityType.Red, 0.03 + (0.0075 * (characterCount - 1)) },
            };

            // 生成奖励
            string award = "";
            switch (model.Result)
            {
                case ExploreResult.General:
                    switch (Random.Shared.Next(3))
                    {
                        case 0:
                            int credits = 0;
                            for (int i = 0; i < characterCount; i++)
                            {
                                credits += Random.Shared.Next(250, 400) * diff;
                            }
                            model.CreditsAward = credits;
                            award = $" {credits} {General.GameplayEquilibriumConstant.InGameCurrency}";
                            break;
                        case 1:
                            int materials = 0;
                            for (int i = 0; i < characterCount; i++)
                            {
                                materials += 2 * diff;
                            }
                            model.MaterialsAward = materials;
                            award = $" {materials} {General.GameplayEquilibriumConstant.InGameMaterial}";
                            break;
                        case 2:
                            Item item = FunGameConstant.ExploreItems[region][Random.Shared.Next(FunGameConstant.ExploreItems[region].Count)];
                            int count = 0;
                            for (int i = 0; i < characterCount; i++)
                            {
                                count += Math.Max(1, Random.Shared.Next(1, 4) * diff / 2);
                            }
                            model.Awards[item.Name] = count;
                            award = $" {count} 个{item.Name}";
                            break;
                        default:
                            break;
                    }
                    int exp = 0;
                    for (int i = 0; i < characterCount; i++)
                    {
                        exp += Random.Shared.Next(100, 300) * diff;
                    }
                    model.Awards["exp"] = exp;
                    exploreString += $"额外获得了：{exp} 点经验值（探索队员们平分）！";
                    break;
                case ExploreResult.Fight:
                    // 小队信息
                    Character[] squad = [.. user.Inventory.Characters.Where((c, index) => characterIds.Contains(index + 1)).Select(c => CharacterBuilder.Build(c, true, true, null, FunGameConstant.AllItems, FunGameConstant.AllSkills, false))];
                    if (squad.All(c => c.HP <= 0))
                    {
                        model.Result = ExploreResult.Nothing;
                        exploreString = $"探索小队遭遇强大的敌人{enemy.Name}偷袭，狼狈而逃！（什么也没有获得，请检查角色的状态）";
                    }
                    else
                    {
                        // 生成敌人
                        Item[] weapons = [.. FunGameConstant.Equipment.Where(i => i.Id.ToString().StartsWith("11") && (int)i.QualityType == 5)];
                        Item[] armors = [.. FunGameConstant.Equipment.Where(i => i.Id.ToString().StartsWith("12") && (int)i.QualityType == 5)];
                        Item[] shoes = [.. FunGameConstant.Equipment.Where(i => i.Id.ToString().StartsWith("13") && (int)i.QualityType == 5)];
                        Item[] accessory = [.. FunGameConstant.Equipment.Where(i => i.Id.ToString().StartsWith("14") && (int)i.QualityType == 5)];
                        Item[] consumables = [.. FunGameConstant.AllItems.Where(i => i.ItemType == ItemType.Consumable && i.IsInGameItem)];
                        int cLevel = diff * 12;
                        int sLevel = diff + 1;
                        int mLevel = region.Difficulty switch
                        {
                            RarityType.OneStar => 1,
                            RarityType.TwoStar => 2,
                            RarityType.ThreeStar => 4,
                            RarityType.FourStar => 6,
                            _ => 8
                        };
                        int naLevel = mLevel;
                        foreach (Character enemy_loop in enemys)
                        {
                            EnhanceBoss(enemy_loop, weapons, armors, shoes, accessory, consumables, cLevel, sLevel, mLevel, naLevel, false, false, isUnit);
                        }
                        // 开始战斗
                        Team team1 = new($"{user.Username}的探索小队", squad);
                        Team team2 = new($"{region.Name}", enemys);
                        List<string> msgs = await FunGameActionQueue.NewAndStartTeamGame([team1, team2], showAllRound: true);
                        if (msgs.Count > 2)
                        {
                            msgs = msgs[^2..];
                        }
                        if (enemys.All(c => c.HP <= 0))
                        {
                            model.FightWin = true;
                            int credits = 0;
                            for (int i = 0; i < characterCount; i++)
                            {
                                credits += Random.Shared.Next(400, 650) * diff;
                            }
                            model.CreditsAward = credits;
                            exp = 0;
                            for (int i = 0; i < characterCount; i++)
                            {
                                exp += Random.Shared.Next(250, 520) * diff;
                            }
                            model.Awards["exp"] = exp;
                            int materials = 0;
                            for (int i = 0; i < characterCount; i++)
                            {
                                materials += Random.Shared.Next(3, 7) * diff;
                            }
                            model.MaterialsAward = materials;
                            Item item = FunGameConstant.ExploreItems[region][Random.Shared.Next(FunGameConstant.ExploreItems[region].Count)];
                            int count = 0;
                            for (int i = 0; i < characterCount; i++)
                            {
                                count += Math.Max(1, Random.Shared.Next(1, 4) * diff / 2);
                            }
                            model.Awards[item.Name] = count;
                            award = $"{credits} {General.GameplayEquilibriumConstant.InGameCurrency}，" + $"{exp} 点经验值（探索队员们平分），" +
                                $"{materials} {General.GameplayEquilibriumConstant.InGameMaterial}，" + $"以及 {count} 个{item.Name}";
                            if (Random.Shared.NextDouble() > 0.6)
                            {
                                QualityType qualityType = QualityType.Blue;
                                foreach (QualityType type in pE.Keys.OrderByDescending(q => (int)q))
                                {
                                    if (Random.Shared.NextDouble() <= pE[type])
                                    {
                                        qualityType = type;
                                        break;
                                    }
                                }
                                Item? itemDrop = region.Items.Where(i => qualityType == QualityType.Blue ? (int)i.QualityType <= (int)qualityType : (int)i.QualityType == (int)qualityType).OrderBy(o => Random.Shared.Next()).FirstOrDefault();
                                if (itemDrop != null)
                                {
                                    string itemquality = ItemSet.GetQualityTypeName(itemDrop.QualityType);
                                    string itemtype = ItemSet.GetItemTypeName(itemDrop.ItemType) + (itemDrop.ItemType == ItemType.Weapon && itemDrop.WeaponType != WeaponType.None ? "-" + ItemSet.GetWeaponTypeName(itemDrop.WeaponType) : "");
                                    if (itemtype != "") itemtype = $"|{itemtype}";
                                    award += $"！额外获得了：[{itemquality + itemtype}]{itemDrop.Name}";
                                }
                            }
                            exploreString = $"{exploreString}\r\n{string.Join("\r\n", msgs)}\r\n探索小队战胜了{enemy.Name}！获得了：{award}！";
                        }
                        else
                        {
                            exploreString = $"{exploreString}\r\n{string.Join("\r\n", msgs)}\r\n探索小队未能战胜{enemy.Name}，";
                            IEnumerable<Character> deadEnemys = enemys.Where(c => c.HP <= 0);
                            if (!deadEnemys.Any())
                            {
                                exploreString += "探索宣告失败！（什么也没有获得）";
                            }
                            else
                            {
                                Item item = FunGameConstant.ExploreItems[region][Random.Shared.Next(FunGameConstant.ExploreItems[region].Count)];
                                model.Awards[item.Name] = deadEnemys.Count();
                                award = $"{deadEnemys.Count()} 个{item.Name}";
                                exploreString += $"但是获得了补偿：{award}！";
                            }
                        }
                        model.AfterFightHPs = [.. squad.Select(c => c.HP)];
                    }
                    break;
                case ExploreResult.Earned:
                    QualityType quality = QualityType.Blue;
                    foreach (QualityType type in pE.Keys.OrderByDescending(q => (int)q))
                    {
                        if (Random.Shared.NextDouble() <= pE[type])
                        {
                            quality = type;
                            break;
                        }
                    }
                    Item? itemEarned = region.Items.Where(i => quality == QualityType.Blue ? (int)i.QualityType <= (int)quality : (int)i.QualityType == (int)quality).OrderBy(o => Random.Shared.Next()).FirstOrDefault();
                    if (itemEarned is null)
                    {
                        model.Result = ExploreResult.Nothing;
                        exploreString = "你在探索中发现了一个神秘的物品，但它似乎无法辨认。（什么也没有获得）";
                    }
                    else
                    {
                        model.Awards[itemEarned.Name] = 1;
                        string itemquality = ItemSet.GetQualityTypeName(itemEarned.QualityType);
                        string itemtype = ItemSet.GetItemTypeName(itemEarned.ItemType) + (itemEarned.ItemType == ItemType.Weapon && itemEarned.WeaponType != WeaponType.None ? "-" + ItemSet.GetWeaponTypeName(itemEarned.WeaponType) : "");
                        if (itemtype != "") itemtype = $"|{itemtype}";
                        award += $"[{itemquality + itemtype}]{itemEarned.Name}";
                    }
                    break;
                case ExploreResult.Event:
                    break;
                case ExploreResult.Nothing:
                default:
                    break;
            }

            model.String = string.Format(exploreString, award, enemy.Name, npc, item1, area1, item2, area2);

            PluginConfig pc = new("exploring", user.Id.ToString());
            pc.LoadConfig();
            pc.Add(model.Guid.ToString(), model);
            pc.SaveConfig();
        }

        public static bool SettleExplore(string exploreId, PluginConfig pc, User user, out string msg)
        {
            bool result = false;
            msg = "";
            ExploreModel? model = pc.Get<ExploreModel>(exploreId);
            if (model != null)
            {
                result = true;
                msg = model.String;
                if (model.CreditsAward > 0)
                {
                    user.Inventory.Credits += model.CreditsAward;
                }
                if (model.MaterialsAward > 0)
                {
                    user.Inventory.Materials += model.MaterialsAward;
                }
                Character[] inventory = [.. user.Inventory.Characters];
                foreach (string name in model.Awards.Keys)
                {
                    if (name == "exp")
                    {
                        double exp = (double)model.Awards[name] / model.CharacterIds.Count();
                        foreach (long cid in model.CharacterIds)
                        {
                            if (cid > 0 && cid <= inventory.Length)
                            {
                                Character character = inventory[(int)cid - 1];
                                character.EXP += exp;
                            }
                        }
                        continue;
                    }
                    Item? item = FunGameConstant.AllItems.FirstOrDefault(i => i.Name == name);
                    if (item != null)
                    {
                        for (int i = 0; i < model.Awards[name]; i++)
                        {
                            Item newItem = item.Copy();
                            newItem.User = user;
                            if (newItem.QualityType >= QualityType.Orange) newItem.IsLock = true;
                            SetSellAndTradeTime(newItem);
                            user.Inventory.Items.Add(newItem);
                            // 连接到任务系统
                            AddExploreItemCache(user.Id, item.Name);
                            // 连接到活动系统
                            ActivitiesItemCache.Add(item.Name);
                        }
                    }
                }
                if (model.AfterFightHPs.Length > 0)
                {
                    int hpIndex = 0;
                    foreach (long cid in model.CharacterIds)
                    {
                        if (cid > 0 && cid <= inventory.Length && hpIndex < model.AfterFightHPs.Length)
                        {
                            Character character = inventory[(int)cid - 1];
                            character.HP = model.AfterFightHPs[hpIndex++];
                        }
                    }
                }
            }
            return result;
        }

        public static bool SettleExploreAll(PluginConfig pc, User user, bool skip = false)
        {
            bool settle = false;
            List<string> remove = [];
            foreach (string guid in pc.Keys)
            {
                ExploreModel? model = pc.Get<ExploreModel>(guid);
                if (model is null) continue;
                if (skip || (model.StartTime.HasValue && (DateTime.Now - model.StartTime.Value).TotalMinutes > FunGameConstant.ExploreTime + 2))
                {
                    if (SettleExplore(guid, pc, user, out string msg))
                    {
                        settle = true;
                        AddNotice(user.Id, $"你上次未完成的探索已被自动结算：{msg}");
                        remove.Add(guid);
                    }
                }
            }
            foreach (string guid in remove)
            {
                pc.Remove(guid);
            }
            return settle;
        }

        public static long MakeOffer(User user, User offeree)
        {
            using SQLHelper? sql = Factory.OpenFactory.GetSQLHelper();
            if (sql != null)
            {
                sql.AddOffer(user.Id, offeree.Id);
                if (sql.Success)
                {
                    long offerId = sql.LastInsertId;
                    Offer? offer = SQLService.GetOffer(sql, offerId);
                    if (offer != null)
                    {
                        return offerId;
                    }
                }
            }
            return -1;
        }
        
        public static string AddOfferItems(User user, long offerId, bool isOpposite, int[] itemIds)
        {
            using SQLHelper? sql = Factory.OpenFactory.GetSQLHelper();
            if (sql != null)
            {
                bool result = true;
                string msg = "";
                Offer? offer = SQLService.GetOffer(sql, offerId);
                if (offer != null && offer.Offeror == user.Id)
                {
                    try
                    {
                        if (offer.Status != OfferState.Created)
                        {
                            msg = "当前状态不允许修改报价内容。";
                            return msg;
                        }

                        User user2;
                        PluginConfig pc2 = new("saved", offer.Offeree.ToString());
                        pc2.LoadConfig();

                        if (pc2.Count > 0)
                        {
                            user2 = GetUser(pc2);
                        }
                        else
                        {
                            return "无法找到报价的接收方，请稍后再试。";
                        }
                        User addUser = isOpposite ? user2: user;
                        offer.OfferorItems = [.. SQLService.GetOfferItemsByOfferIdAndUserId(sql, offerId, user)];
                        offer.OffereeItems = [.. SQLService.GetOfferItemsByOfferIdAndUserId(sql, offerId, user2)];

                        sql.NewTransaction();
                        List<int> failedItems = [];
                        foreach (int itemIndex in itemIds)
                        {
                            if (itemIndex > 0 && itemIndex <= addUser.Inventory.Items.Count)
                            {
                                Item item = addUser.Inventory.Items.ToList()[itemIndex - 1];

                                if (addUser.Id == offer.Offeror && offer.OfferorItems.Contains(item.Guid) || addUser.Id == offer.Offeree && offer.OffereeItems.Contains(item.Guid))
                                {
                                    result = false;
                                    if (msg != "") msg += "\r\n";
                                    msg += $"物品 {itemIndex}. {item.Name}：此物品已在报价中，无需重复添加。";
                                    continue;
                                }

                                if (item.IsLock)
                                {
                                    result = false;
                                    if (msg != "") msg += "\r\n";
                                    msg += $"物品 {itemIndex}. {item.Name}：此物品已锁定，无法进行交易。";
                                    break;
                                }
                                
                                if (item.Character != null)
                                {
                                    result = false;
                                    if (msg != "") msg += "\r\n";
                                    msg += $"物品 {itemIndex}. {item.Name}：此物品已被 {item.Character} 装备中，无法进行交易。";
                                    break;
                                }
                                
                                if (!item.IsTradable)
                                {
                                    result = false;
                                    if (msg != "") msg += "\r\n";
                                    msg += $"物品 {itemIndex}. {item.Name}：此物品无法交易{(item.NextTradableTime != DateTime.MinValue ? $"，此物品将在 {item.NextTradableTime.ToString(General.GeneralDateTimeFormatChinese)} 后可交易" : "")}。";
                                    break;
                                }

                                sql.AddOfferItem(offerId, addUser.Id, item.Guid);
                                if (msg != "") msg += "\r\n";
                                if (sql.Success)
                                {
                                    msg += $"物品添加成功：{itemIndex}. {item.Name}";
                                }
                                else
                                {
                                    result = false;
                                    msg += $"物品添加失败：{itemIndex}. {item.Name}";
                                    break;
                                }
                            }
                            else
                            {
                                failedItems.Add(itemIndex);
                            }
                        }

                        if (failedItems.Count > 0)
                        {
                            if (msg != "") msg += "\r\n";
                            msg += "没有找到与这个序号相对应的物品：" + string.Join("，", failedItems);
                        }

                        if (result)
                        {
                            offer = SQLService.GetOffer(sql, offerId);
                            if (offer != null)
                            {
                                sql.Commit();
                            }
                        }
                        else
                        {
                            sql.Rollback();
                        }
                    }
                    catch
                    {
                        sql.Rollback();
                        msg = "修改报价时发生错误，请稍后再试。";
                        throw;
                    }
                }
                else
                {
                    msg = "报价不存在或你不是该报价的发起方。";
                }

                return msg;
            }
            return "服务器繁忙，请稍后再试。";
        }

        public static string SendOffer(User user, long offerId)
        {
            using SQLHelper? sql = Factory.OpenFactory.GetSQLHelper();
            if (sql != null)
            {
                string msg = "";
                Offer? offer = SQLService.GetOffer(sql, offerId);
                if (offer != null && offer.Offeror == user.Id)
                {
                    try
                    {
                        if (offer.Status != OfferState.Created)
                        {
                            msg = "此报价已被处理。";
                            return msg;
                        }

                        bool result = true;
                        sql.NewTransaction();
                        sql.UpdateOfferStatus(offerId, OfferState.Sent);

                        if (result)
                        {
                            offer = SQLService.GetOffer(sql, offerId);
                            if (offer != null)
                            {
                                sql.Commit();
                                AddNotice(offer.Offeree, $"你收到了一个报价！请通过【查报价{offerId}】查询报价记录。");
                                return $"报价编号 {offerId} 已发送。";
                            }
                        }
                        else
                        {
                            sql.Rollback();
                        }
                    }
                    catch
                    {
                        sql.Rollback();
                        msg = "修改报价时发生错误，请稍后再试。";
                        throw;
                    }
                }
                else
                {
                    msg = "报价不存在或你不是该报价的发起方。";
                }
                
                return msg;
            }
            return "服务器繁忙，请稍后再试。";
        }

        public static List<Offer> GetOffer(User user, out string msg, long offerId = -1)
        {
            msg = "";
            List<Offer> offers = [];
            using SQLHelper? sql = Factory.OpenFactory.GetSQLHelper();
            if (sql != null)
            {
                if (offerId > 0)
                {
                    Offer? offer = SQLService.GetOffer(sql, offerId);
                    if (offer != null)
                    {
                        if (offer.Offeror == user.Id || offer.Offeree == user.Id)
                        {
                            offers.Add(offer);
                        }
                        else
                        {
                            msg = $"你无权查看报价编号 {offerId}。";
                        }
                    }
                    else msg = $"报价编号 {offerId} 不存在。";
                }
                else
                {
                    offers = sql.GetOffersByOfferor(user.Id);
                    offers = [.. offers, ..sql.GetOffersByOfferee(user.Id)];
                    offers = [.. offers.DistinctBy(o => o.Id)];
                }
            }
            return offers;
        }

        public static string RemoveOfferItems(User user, long offerId, bool isOpposite, int[] itemIds)
        {
            using SQLHelper? sql = Factory.OpenFactory.GetSQLHelper();
            if (sql != null)
            {
                bool result = true;
                string msg = "";
                Offer? offer = SQLService.GetOffer(sql, offerId);
                if (offer != null && offer.Offeror == user.Id)
                {
                    try
                    {
                        if (offer.Status != OfferState.Created)
                        {
                            msg = "当前状态不允许修改报价内容。";
                            return msg;
                        }

                        User removeUser;
                        List<Guid> guids = [];

                        if (isOpposite)
                        {
                            PluginConfig pc2 = new("saved", offer.Offeree.ToString());
                            pc2.LoadConfig();

                            if (pc2.Count > 0)
                            {
                                removeUser = GetUser(pc2);
                            }
                            else
                            {
                                return "无法找到报价的接收方，请稍后再试。";
                            }
                        }
                        else
                        {
                            removeUser = user;
                        }

                        guids = [.. SQLService.GetOfferItemsByOfferIdAndUserId(sql, offerId, removeUser)];

                        sql.NewTransaction();
                        List<int> failedItems = [];
                        foreach (int itemIndex in itemIds)
                        {
                            if (itemIndex > 0 && itemIndex <= guids.Count)
                            {
                                Guid itemGuid = guids[itemIndex - 1];
                                SQLService.DeleteOfferItemsByOfferIdAndItemGuid(sql, offerId, itemGuid);
                            }
                            else
                            {
                                failedItems.Add(itemIndex);
                            }
                        }

                        if (failedItems.Count > 0)
                        {
                            if (msg != "") msg += "\r\n";
                            msg += $"在报价的{(isOpposite ? "接收方" : "发起方")}物品列表中没有找到与这个序号相对应的物品：{string.Join("，", failedItems)}";
                        }

                        if (result)
                        {
                            offer = SQLService.GetOffer(sql, offerId);
                            if (offer != null)
                            {
                                sql.Commit();
                            }
                        }
                        else
                        {
                            sql.Rollback();
                        }
                    }
                    catch
                    {
                        sql.Rollback();
                        msg = "修改报价时发生错误，请稍后再试。";
                        throw;
                    }
                }
                else
                {
                    msg = "报价不存在或你不是该报价的发起方。";
                }

                return msg;
            }
            return "服务器繁忙，请稍后再试。";
        }

        public static string RespondOffer(PluginConfig pc, User user, long offerId, OfferActionType action)
        {
            using SQLHelper? sql = Factory.OpenFactory.GetSQLHelper();
            if (sql != null)
            {
                string msg = "";
                Offer? offer = SQLService.GetOffer(sql, offerId);
                if (offer != null && offer.Offeree == user.Id)
                {
                    bool canProceed = false;

                    try
                    {
                        sql.NewTransaction();

                        // 根据 action 处理状态
                        switch (action)
                        {
                            case OfferActionType.OffereeAccept:
                                if (offer.Status == OfferState.Sent || offer.Status == OfferState.Negotiating || offer.Status == OfferState.NegotiationAccepted)
                                {
                                    sql.UpdateOfferStatus(offerId, OfferState.Completed);
                                    sql.UpdateOfferFinishTime(offerId, DateTime.Now);
                                    canProceed = true;
                                }
                                else msg = "当前状态不允许接受。";
                                break;

                            case OfferActionType.OffereeReject:
                                if (offer.Status == OfferState.Sent || offer.Status == OfferState.Negotiating || offer.Status == OfferState.NegotiationAccepted)
                                {
                                    sql.UpdateOfferStatus(offerId, OfferState.Rejected);
                                    sql.UpdateOfferFinishTime(offerId, DateTime.Now);
                                    canProceed = true;
                                }
                                else msg = "当前状态不允许拒绝。";
                                break;

                            default:
                                msg = "无效的操作类型。";
                                break;
                        }

                        if (canProceed)
                        {
                            offer = SQLService.GetOffer(sql, offerId);
                            if (offer != null)
                            {
                                if (offer.Status == OfferState.Completed)
                                {
                                    PluginConfig pc2 = new("saved", offer.Offeror.ToString());
                                    pc2.LoadConfig();

                                    if (pc2.Count > 0)
                                    {
                                        User user2 = GetUser(pc2);

                                        offer.OffereeItems = [.. SQLService.GetOfferItemsByOfferIdAndUserId(sql, offerId, user)];
                                        offer.OfferorItems = [.. SQLService.GetOfferItemsByOfferIdAndUserId(sql, offerId, user2)];

                                        foreach (Guid itemGuid in offer.OffereeItems)
                                        {
                                            if (user.Inventory.Items.FirstOrDefault(i => i.Guid == itemGuid) is Item item)
                                            {
                                                user.Inventory.Items.Remove(item);

                                                Item newItem = item.Copy(copyGuid: true);
                                                newItem.User = user2;
                                                newItem.IsSellable = false;
                                                newItem.IsTradable = false;
                                                newItem.NextSellableTime = DateTimeUtility.GetTradableTime();
                                                newItem.NextTradableTime = DateTimeUtility.GetTradableTime();
                                                user2.Inventory.Items.Add(newItem);
                                            }
                                        }
                                        foreach (Guid itemGuid in offer.OfferorItems)
                                        {
                                            if (user2.Inventory.Items.FirstOrDefault(i => i.Guid == itemGuid) is Item item)
                                            {
                                                user2.Inventory.Items.Remove(item);

                                                Item newItem = item.Copy(copyGuid: true);
                                                newItem.User = user;
                                                newItem.IsSellable = false;
                                                newItem.IsTradable = false;
                                                newItem.NextSellableTime = DateTimeUtility.GetTradableTime();
                                                newItem.NextTradableTime = DateTimeUtility.GetTradableTime();
                                                user.Inventory.Items.Add(newItem);
                                            }
                                        }
                                        user.LastTime = DateTime.Now;
                                        pc.Add("user", user);
                                        pc.SaveConfig();

                                        user2.LastTime = DateTime.Now;
                                        pc2.Add("user", user2);
                                        pc2.SaveConfig();

                                        AddNotice(offer.Offeror, $"报价编号 {offerId} 已交易完成，请通过【查报价{offerId}】查询报价记录。");

                                        msg = "";
                                    }
                                    else
                                    {
                                        msg = "目标玩家不存在，请稍后再试。";
                                    }
                                }
                                else if (offer.Status == OfferState.Rejected)
                                {
                                    AddNotice(offer.Offeror, $"报价编号 {offerId} 已被拒绝，请通过【查报价{offerId}】查询报价记录。");
                                }
                                sql.Commit();
                            }
                        }

                        if (msg != "")
                        {
                            sql.Rollback();
                        }
                        else
                        {
                            return $"报价编号 {offerId} 已交易完成，请通过【查报价{offerId}】查询报价记录。";
                        }
                    }
                    catch
                    {
                        sql.Rollback();
                        msg = "回应报价时发生错误，请稍后再试。";
                        throw;
                    }
                }
                else
                {
                    msg = "报价不存在或你不是该报价的接收方。";
                }
                return msg;
            }
            return "服务器繁忙，请稍后再试。";
        }

        public static string CancelOffer(User user, long offerId)
        {
            using SQLHelper? sql = Factory.OpenFactory.GetSQLHelper();
            if (sql != null)
            {
                string msg = "";
                Offer? offer = SQLService.GetOffer(sql, offerId);
                if (offer != null && offer.Offeror == user.Id)
                {
                    try
                    {
                        if (offer.Status != OfferState.Created && offer.Status != OfferState.Sent)
                        {
                            msg = "此报价已被处理。";
                            return msg;
                        }

                        bool result = true;
                        sql.NewTransaction();
                        sql.UpdateOfferStatus(offerId, OfferState.Cancelled);

                        if (result)
                        {
                            offer = SQLService.GetOffer(sql, offerId);
                            if (offer != null)
                            {
                                sql.Commit();
                                return $"报价编号 {offerId} 已取消。";
                            }
                        }
                        else
                        {
                            sql.Rollback();
                        }
                    }
                    catch
                    {
                        sql.Rollback();
                        msg = "修改报价时发生错误，请稍后再试。";
                        throw;
                    }
                }
                else
                {
                    msg = "报价不存在或你不是该报价的发起方。";
                }

                return msg;
            }
            return "服务器繁忙，请稍后再试。";
        }
    }
}
