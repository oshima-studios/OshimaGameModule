using System.Text;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules;
using Oshima.FunGame.OshimaModules.Characters;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;
using Oshima.FunGame.OshimaModules.Items;
using Oshima.FunGame.OshimaModules.Skills;

namespace Oshima.Core.Utils
{
    public class FunGameService
    {
        public static List<Character> Characters { get; } = [];
        public static List<Skill> Skills { get; } = [];
        public static List<Skill> Magics { get; } = [];
        public static List<Item> Equipment { get; } = [];
        public static List<Item> Items { get; } = [];
        public static List<Skill> ItemSkills { get; } = [];
        public static List<Item> AllItems { get; } = [];
        public static List<Skill> AllSkills { get; } = [];
        public static Dictionary<long, string> UserIdAndUsername { get; } = [];
        public static Dictionary<int, Character> Bosses { get; } = [];

        public static void InitFunGame()
        {
            Characters.Add(new OshimaShiya());
            Characters.Add(new XinYin());
            Characters.Add(new Yang());
            Characters.Add(new NanGanYu());
            Characters.Add(new NiuNan());
            Characters.Add(new DokyoMayor());
            Characters.Add(new MagicalGirl());
            Characters.Add(new QingXiang());
            Characters.Add(new QWQAQW());
            Characters.Add(new ColdBlue());
            Characters.Add(new dddovo());
            Characters.Add(new Quduoduo());

            Skills.AddRange([new 疾风步()]);

            Magics.AddRange([new 冰霜攻击(), new 火之矢(), new 水之矢(), new 风之轮(), new 石之锤(), new 心灵之霞(), new 次元上升(), new 暗物质(), new 回复术(), new 治愈术(),
                new 时间加速(), new 时间减速(), new 沉默十字(), new 反魔法领域()]);

            Dictionary<string, Item> exItems = Factory.GetGameModuleInstances<Item>(OshimaGameModuleConstant.General, OshimaGameModuleConstant.Item);
            Equipment.AddRange(exItems.Values.Where(i => (int)i.ItemType >= 0 && (int)i.ItemType < 5));
            Equipment.AddRange([new 攻击之爪5(), new 攻击之爪15(), new 攻击之爪25(), new 攻击之爪35()]);

            Items.AddRange(exItems.Values.Where(i => (int)i.ItemType > 4));
            Items.AddRange([new 小经验书(), new 中经验书(), new 大经验书(), new 升华之印(), new 流光之印(), new 永恒之印(), new 技能卷轴(), new 智慧之果(), new 奥术符文(), new 混沌之核()]);

            AllItems.AddRange(Equipment);
            AllItems.AddRange(Items);

            Skill?[] activeSkills = [.. Equipment.Select(i => i.Skills.Active), .. Items.Select(i => i.Skills.Active)];
            foreach (Skill? skill in activeSkills)
            {
                if (skill != null)
                {
                    ItemSkills.Add(skill);
                }
            }
            ItemSkills.AddRange([.. Equipment.SelectMany(i => i.Skills.Passives), .. Items.SelectMany(i => i.Skills.Passives)]);

            AllSkills.AddRange(Skills);
            AllSkills.AddRange(ItemSkills);
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
                    _ => Random.Shared.Next(1, 7)
                };
                item.QualityType = (QualityType)qualityType;
            }
            else
            {
                total = Random.Shared.Next(1, 31);
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
            }

            GenerateAndAddSkillToMagicCard(item, total);

            return item;
        }

        public static void GenerateAndAddSkillToMagicCard(Item item, int total)
        {
            Skill magic = Magics[Random.Shared.Next(Magics.Count)].Copy();
            magic.Guid = item.Guid;
            magic.Level = (int)item.QualityType switch
            {
                2 => 2,
                3 => 2,
                4 => 3,
                5 => 3,
                6 => 4,
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
            Characters.Clear();
            Equipment.Clear();
            Skills.Clear();
            Magics.Clear();
            AllItems.Clear();
            ItemSkills.Clear();
            AllSkills.Clear();

            InitFunGame();
        }

        public static string GenerateRandomChineseName()
        {
            // 定义一个包含常用汉字的字符串
            string commonChineseCharacters = "云星宝灵梦龙花雨风叶山川月石羽水竹金" +
                "玉海火雷光天地凤虎虹珠华霞鹏雪银沙松桃兰青霜鸿康骏波泉河湖江泽洋林枫" +
                "梅桂樱桐晴韵凌若悠碧涛渊壁剑影霖玄承珍雅耀瑞鹤烟燕霏翼翔璃绮纱绫绣锦" +
                "瑜琼瑾璇璧琳琪瑶瑛芝杏茜荷莉莹菡莲诗瑰翠椒槐榆槿柱梧曜曙晶暖智煌熙霓" +
                "熠嘉琴曼菁蓉菲淑妙惠秋涵映巧慧茹荣菱曦容芬玲澜清湘澄泓润珺晨翠涟洁悠" +
                "霏淑绮润东南西北云山川风月溪雪雨雷天云海霜柏芳春秋夏冬温景寒和竹阳溪" +
                "溪飞风峰阳";

            // 随机生成名字长度，2到5个字
            int nameLength = Random.Shared.Next(2, 6);
            StringBuilder name = new();

            for (int i = 0; i < nameLength; i++)
            {
                // 从常用汉字集中随机选择一个汉字
                char chineseCharacter = commonChineseCharacters[Random.Shared.Next(commonChineseCharacters.Length)];
                name.Append(chineseCharacter);
            }

            return name.ToString();
        }

        public static string GenerateRandomChineseUserName()
        {
            string[] commonSurnames = [
                "顾", "沈", "陆", "楚", "白", "苏", "叶", "萧", "莫", "司马", "欧阳",
                "上官", "慕容", "尉迟", "司徒", "轩辕", "端木", "南宫", "长孙", "百里",
                "东方", "西门", "独孤", "公孙", "令狐", "宇文", "夏侯", "赫连", "皇甫",
                "北堂", "安陵", "东篱", "花容", "夜", "柳", "云", "凌", "寒", "龙",
                "凤", "蓝", "冷", "华", "蓝夜", "叶南", "墨", "君", "月", "子车",
                "澹台", "钟离", "公羊", "闾丘", "仲孙", "司空", "羊舌", "亓官", "公冶",
                "濮阳", "独月", "南风", "凤栖", "南门", "姬", "闻人", "花怜", "若",
                "紫", "卿", "微", "清", "易", "月华", "霜", "兰", "岑", "语", "雪",
                "夜阑", "梦", "洛", "江", "黎", "夜北", "唐", "水", "韩", "庄",
                "夜雪", "夜凌", "君临", "青冥", "漠然", "林", "青", "岑", "容",
                "墨", "柏", "安", "晏", "尉", "南", "轩", "竹", "晨", "桓", "晖",
                "瑾", "溪", "汐", "沐", "玉", "汀", "归", "羽", "颜", "辰", "琦",
                "芷", "尹", "施", "原", "孟", "尧", "荀", "单", "简", "植", "傅",
                "司", "钟", "方", "谢"
            ];

            // 定义一个包含常用汉字的字符串
            string commonChineseCharacters = "云星宝灵梦龙花雨风叶山川月石羽水竹金" +
                "玉海火雷光天地凤虎虹珠华霞鹏雪银沙松桃兰青霜鸿康骏波泉河湖江泽洋林枫" +
                "梅桂樱桐晴韵凌若悠碧涛渊壁剑影霖玄承珍雅耀瑞鹤烟燕霏翼翔璃绮纱绫绣锦" +
                "瑜琼瑾璇璧琳琪瑶瑛芝杏茜荷莉莹菡莲诗瑰翠椒槐榆槿柱梧曜曙晶暖智煌熙霓" +
                "熠嘉琴曼菁蓉菲淑妙惠秋涵映巧慧茹荣菱曦容芬玲澜清湘澄泓润珺晨涟洁东南" +
                "西北溪飞峰阳龄一二三四五六七十";

            StringBuilder name = new();

            // 随机姓
            string lastname = commonSurnames[Random.Shared.Next(commonSurnames.Length)];
            name.Append(lastname);

            // 随机生成名字长度，2到5个字
            int nameLength = Random.Shared.Next(1, 2);

            for (int i = 0; i < nameLength; i++)
            {
                // 从常用汉字集中随机选择一个汉字
                char chineseCharacter = commonChineseCharacters[Random.Shared.Next(commonChineseCharacters.Length)];
                name.Append(chineseCharacter);
            }

            return name.ToString();
        }

        public static User GetUser(PluginConfig pc)
        {
            User user = pc.Get<User>("user") ?? Factory.GetUser();

            List<Character> characters = new(user.Inventory.Characters);
            List<Item> items = new(user.Inventory.Items);
            Character mc = user.Inventory.MainCharacter;
            List<long> squad = [.. user.Inventory.Squad];
            Dictionary<long, DateTime> training = user.Inventory.Training.ToDictionary(kv => kv.Key, kv => kv.Value);
            user.Inventory.Characters.Clear();
            user.Inventory.Items.Clear();
            user.Inventory.Squad.Clear();
            user.Inventory.Training.Clear();

            foreach (Item inventoryItem in items)
            {
                Item realItem = inventoryItem.Copy(true, true, true, AllItems, AllSkills);
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
                Character realCharacter = CharacterBuilder.Build(inventoryCharacter, false, true, user.Inventory, AllItems, AllSkills, false);
                // 自动回血
                DateTime now = DateTime.Now;
                int seconds = (int)(now - user.LastTime).TotalSeconds;
                double recoveryHP = realCharacter.HR * seconds;
                double recoveryMP = realCharacter.MR * seconds;
                double recoveryEP = realCharacter.ER * seconds;
                realCharacter.HP = inventoryCharacter.HP + recoveryHP;
                realCharacter.MP = inventoryCharacter.MP + recoveryMP;
                realCharacter.EP = inventoryCharacter.EP + recoveryEP;
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
                List<Effect> effects = realCharacter.Effects.Where(e => e.Level > 0).ToList();
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

            // 任务结算
            EntityModuleConfig<Quest> quests = new("quests", user.Id.ToString());
            quests.LoadConfig();
            if (quests.Count > 0 && SettleQuest(user, quests))
            {
                quests.SaveConfig();
                user.LastTime = DateTime.Now;
                pc.Add("user", user);
                pc.SaveConfig();
            }

            return user;
        }

        public static IEnumerable<T> GetPage<T>(IEnumerable<T> list, int showPage, int pageSize)
        {
            return list.Skip((showPage - 1) * pageSize).Take(pageSize).ToList();
        }

        public static string GetDrawCardResult(int reduce, User user, bool isMulti = false, int multiCount = 1)
        {
            string msg = "";
            if (!isMulti)
            {
                msg = $"消耗 {reduce} {General.GameplayEquilibriumConstant.InGameCurrency}，恭喜你抽到了：";
            }
            int r = Random.Shared.Next(8);
            double q = Random.Shared.NextDouble() * 100;
            QualityType type = q switch
            {
                <= 69.53 => QualityType.White,
                > 69.53 and <= 69.53 + 15.35 => QualityType.Green,
                > 69.53 + 15.35 and <= 69.53 + 15.35 + 9.48 => QualityType.Blue,
                > 69.53 + 15.35 + 9.48 and <= 69.53 + 15.35 + 9.48 + 4.25 => QualityType.Purple,
                > 69.53 + 15.35 + 9.48 + 4.25 and <= 69.53 + 15.35 + 9.48 + 4.25 + 1.33 => QualityType.Orange,
                > 69.53 + 15.35 + 9.48 + 4.25 + 1.33 and <= 69.53 + 15.35 + 9.48 + 4.25 + 1.33 + 0.06 => QualityType.Red,
                _ => QualityType.White
            };

            switch (r)
            {
                case 1:
                    if ((int)type > (int)QualityType.Orange) type = QualityType.Orange;
                    Item[] 武器 = Equipment.Where(i => i.Id.ToString().StartsWith("11") && i.QualityType == type).ToArray();
                    Item a = 武器[Random.Shared.Next(武器.Length)].Copy();
                    SetSellAndTradeTime(a);
                    user.Inventory.Items.Add(a);
                    msg += ItemSet.GetQualityTypeName(a.QualityType) + ItemSet.GetItemTypeName(a.ItemType) + "【" + a.Name + "】！\r\n" + a.Description;
                    break;

                case 2:
                    if ((int)type > (int)QualityType.Green) type = QualityType.Green;
                    Item[] 防具 = Equipment.Where(i => i.Id.ToString().StartsWith("12") && i.QualityType == type).ToArray();
                    Item b = 防具[Random.Shared.Next(防具.Length)].Copy();
                    SetSellAndTradeTime(b);
                    user.Inventory.Items.Add(b);
                    msg += ItemSet.GetQualityTypeName(b.QualityType) + ItemSet.GetItemTypeName(b.ItemType) + "【" + b.Name + "】！\r\n" + b.Description;
                    break;

                case 3:
                    if ((int)type > (int)QualityType.Green) type = QualityType.Green;
                    Item[] 鞋子 = Equipment.Where(i => i.Id.ToString().StartsWith("13") && i.QualityType == type).ToArray();
                    Item c = 鞋子[Random.Shared.Next(鞋子.Length)].Copy();
                    SetSellAndTradeTime(c);
                    user.Inventory.Items.Add(c);
                    msg += ItemSet.GetQualityTypeName(c.QualityType) + ItemSet.GetItemTypeName(c.ItemType) + "【" + c.Name + "】！\r\n" + c.Description;
                    break;

                case 4:
                    if ((int)type > (int)QualityType.Purple) type = QualityType.Purple;
                    Item[] 饰品 = Equipment.Where(i => i.Id.ToString().StartsWith("14") && i.QualityType == type).ToArray();
                    Item d = 饰品[Random.Shared.Next(饰品.Length)].Copy();
                    SetSellAndTradeTime(d);
                    user.Inventory.Items.Add(d);
                    msg += ItemSet.GetQualityTypeName(d.QualityType) + ItemSet.GetItemTypeName(d.ItemType) + "【" + d.Name + "】！\r\n" + d.Description;
                    break;

                case 5:
                    Character character = Characters[Random.Shared.Next(Characters.Count)].Copy();
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
                    if ((int)type > (int)QualityType.Orange) type = QualityType.Orange;
                    Item mfk = GenerateMagicCard(type);
                    SetSellAndTradeTime(mfk);
                    user.Inventory.Items.Add(mfk);
                    msg += ItemSet.GetQualityTypeName(mfk.QualityType) + ItemSet.GetItemTypeName(mfk.ItemType) + "【" + mfk.Name + "】！\r\n" + mfk.Description;
                    break;
                    
                case 7:
                    Item 物品 = Items[Random.Shared.Next(Items.Count)].Copy();
                    SetSellAndTradeTime(物品);
                    user.Inventory.Items.Add(物品);
                    msg += ItemSet.GetQualityTypeName(物品.QualityType) + ItemSet.GetItemTypeName(物品.ItemType) + "【" + 物品.Name + "】！\r\n" + 物品.Description;
                    break;

                case 0:
                default:
                    if ((int)type > (int)QualityType.Orange) type = QualityType.Orange;
                    Item? mfkb = GenerateMagicCardPack(3, type);
                    if (mfkb != null)
                    {
                        SetSellAndTradeTime(mfkb);
                        user.Inventory.Items.Add(mfkb);
                        msg += ItemSet.GetQualityTypeName(mfkb.QualityType) + ItemSet.GetItemTypeName(mfkb.ItemType) + "【" + mfkb.Name + "】！\r\n" + mfkb.Description;
                    }
                    break;
            }
            if (isMulti) msg = $"{multiCount}. \r\n{msg}";
            return msg;
        }

        public static string GetSignInResult(User user)
        {
            string msg = "签到成功！本次签到获得：";
            int currency = Random.Shared.Next(1000, 3000);
            msg += $"{currency} 金币和 ";
            int material = Random.Shared.Next(5, 15);
            msg += $"{material} 材料！额外获得：";
            user.Inventory.Credits += currency;
            user.Inventory.Materials += material;
            int r = Random.Shared.Next(6);
            double q = Random.Shared.NextDouble() * 100;
            QualityType type = q switch
            {
                <= 69.53 => QualityType.White,
                > 69.53 and <= 69.53 + 15.35 => QualityType.Green,
                > 69.53 + 15.35 and <= 69.53 + 15.35 + 9.48 => QualityType.Blue,
                > 69.53 + 15.35 + 9.48 and <= 69.53 + 15.35 + 9.48 + 4.25 => QualityType.Purple,
                > 69.53 + 15.35 + 9.48 + 4.25 and <= 69.53 + 15.35 + 9.48 + 4.25 + 1.33 => QualityType.Orange,
                > 69.53 + 15.35 + 9.48 + 4.25 + 1.33 and <= 69.53 + 15.35 + 9.48 + 4.25 + 1.33 + 0.06 => QualityType.Red,
                _ => QualityType.White
            };

            switch (r)
            {
                case 1:
                    if ((int)type > (int)QualityType.Orange) type = QualityType.Orange;
                    Item[] 武器 = Equipment.Where(i => i.Id.ToString().StartsWith("11") && i.QualityType == type).ToArray();
                    Item a = 武器[Random.Shared.Next(武器.Length)].Copy();
                    SetSellAndTradeTime(a);
                    user.Inventory.Items.Add(a);
                    msg += ItemSet.GetQualityTypeName(a.QualityType) + ItemSet.GetItemTypeName(a.ItemType) + "【" + a.Name + "】！\r\n" + a.Description;
                    break;

                case 2:
                    if ((int)type > (int)QualityType.Green) type = QualityType.Green;
                    Item[] 防具 = Equipment.Where(i => i.Id.ToString().StartsWith("12") && i.QualityType == type).ToArray();
                    Item b = 防具[Random.Shared.Next(防具.Length)].Copy();
                    SetSellAndTradeTime(b);
                    user.Inventory.Items.Add(b);
                    msg += ItemSet.GetQualityTypeName(b.QualityType) + ItemSet.GetItemTypeName(b.ItemType) + "【" + b.Name + "】！\r\n" + b.Description;
                    break;

                case 3:
                    if ((int)type > (int)QualityType.Green) type = QualityType.Green;
                    Item[] 鞋子 = Equipment.Where(i => i.Id.ToString().StartsWith("13") && i.QualityType == type).ToArray();
                    Item c = 鞋子[Random.Shared.Next(鞋子.Length)].Copy();
                    SetSellAndTradeTime(c);
                    user.Inventory.Items.Add(c);
                    msg += ItemSet.GetQualityTypeName(c.QualityType) + ItemSet.GetItemTypeName(c.ItemType) + "【" + c.Name + "】！\r\n" + c.Description;
                    break;

                case 4:
                    if ((int)type > (int)QualityType.Purple) type = QualityType.Purple;
                    Item[] 饰品 = Equipment.Where(i => i.Id.ToString().StartsWith("14") && i.QualityType == type).ToArray();
                    Item d = 饰品[Random.Shared.Next(饰品.Length)].Copy();
                    SetSellAndTradeTime(d);
                    user.Inventory.Items.Add(d);
                    msg += ItemSet.GetQualityTypeName(d.QualityType) + ItemSet.GetItemTypeName(d.ItemType) + "【" + d.Name + "】！\r\n" + d.Description;
                    break;

                case 5:
                    if ((int)type > (int)QualityType.Orange) type = QualityType.Orange;
                    Item mfk = GenerateMagicCard(type);
                    SetSellAndTradeTime(mfk);
                    user.Inventory.Items.Add(mfk);
                    msg += ItemSet.GetQualityTypeName(mfk.QualityType) + ItemSet.GetItemTypeName(mfk.ItemType) + "【" + mfk.Name + "】！\r\n" + mfk.Description;
                    break;

                case 0:
                default:
                    if ((int)type > (int)QualityType.Orange) type = QualityType.Orange;
                    Item? mfkb = GenerateMagicCardPack(3, type);
                    if (mfkb != null)
                    {
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

        public static bool UseItem(Item item, User user, Character[] targets, out string msg)
        {
            msg = "";
            Dictionary<string, object> args = new()
            {
                { "targets", targets }
            };
            bool result = item.UseItem(args);
            string key = args.Keys.FirstOrDefault(s => s.Equals("msg", StringComparison.CurrentCultureIgnoreCase)) ?? "";
            if (key != "" && args.TryGetValue(key, out object? value) && value is string str)
            {
                msg = str;
            }
            return result;
        }

        public static string GetLevelBreakNeedy(int levelBreak)
        {
            if (LevelBreakNeedyList.TryGetValue(levelBreak, out Dictionary<string, int>? needy) && needy != null && needy.Count > 0)
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
                    return $"魔法【{magic.Name}】在此魔法卡包中不存在或是已经升至满级！";
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

        public static string GetTrainingInfo(TimeSpan diff, bool isPre, out int totalExperience, out int smallBookCount, out int mediumBookCount, out int largeBookCount)
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

            return $"练级时长：{totalMinutes} 分钟，{(isPre ? "预计可" : "")}获得：{totalExperience} 点经验值，{smallBookCount} 本小经验书，{mediumBookCount} 本中经验书，{largeBookCount} 本大经验书。" +
                $"{(isPre ? "练级时间上限 1440 分钟（24小时），超时将不会再产生收益，请按时领取奖励！" : "")}";
        }

        public static string GetSkillLevelUpNeedy(int level)
        {
            if (SkillLevelUpList.TryGetValue(level, out Dictionary<string, int>? needy) && needy != null && needy.Count > 0)
            {
                return GetNeedyInfo(needy);
            }
            return "";
        }

        public static string GetNormalAttackLevelUpNeedy(int level)
        {
            if (NormalAttackLevelUpList.TryGetValue(level, out Dictionary<string, int>? needy) && needy != null && needy.Count > 0)
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

                for (int i = 0; i < genCount; i++)
                {
                    int nowIndex = Bosses.Count > 0 ? Bosses.Keys.Max() + 1 : 1;
                    Character boss = new CustomCharacter(nowIndex, GenerateRandomChineseUserName(), "", "Boss");
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
                    boss.Level = cLevel;
                    boss.NormalAttack.Level = naLevel;
                    boss.NormalAttack.HardnessTime = 6;
                    Item[] weapons = Equipment.Where(i => i.Id.ToString().StartsWith("11") && (int)i.QualityType == 4).ToArray();
                    Item[] armors = Equipment.Where(i => i.Id.ToString().StartsWith("12") && (int)i.QualityType == 1).ToArray();
                    Item[] shoes = Equipment.Where(i => i.Id.ToString().StartsWith("13") && (int)i.QualityType == 1).ToArray();
                    Item[] accessory = Equipment.Where(i => i.Id.ToString().StartsWith("14") && (int)i.QualityType == 3).ToArray();
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
                        magicCardPack.QualityType = QualityType.Red;
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
                    Skill bossSkill = Factory.OpenFactory.GetInstance<Skill>(0, "BOSS专属被动", []);
                    bossSkill.Level = 1;
                    bossSkill.Character = boss;
                    Effect effect = Factory.OpenFactory.GetInstance<Effect>((long)EffectID.DynamicsEffect, "", new()
                    {
                        { "skill", bossSkill },
                        {
                            "values",
                            new Dictionary<string, object>()
                            {
                                { "exatk", 200 / cutRate },
                                { "exdef", 200 / cutRate },
                                { "exhp2", 1.5 },
                                { "exmp2", 0.8 },
                                { "exhr", 8 / cutRate },
                                { "exmr", 4 / cutRate },
                                { "excr", 0.35 },
                                { "excrd", 0.9 },
                                { "excdr", 0.25 },
                                { "exacc", 0.25 }
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

                    Bosses[nowIndex] = boss;
                }
            }
        }

        public static Dictionary<int, Dictionary<string, int>> LevelBreakNeedyList
        {
            get
            {
                return new()
                {
                    {
                        0, new()
                        {
                            { General.GameplayEquilibriumConstant.InGameMaterial, 80 },
                            { nameof(升华之印), 10 }
                        }
                    },
                    {
                        1, new()
                        {
                            { General.GameplayEquilibriumConstant.InGameMaterial, 400 },
                            { nameof(升华之印), 20 }
                        }
                    },
                    {
                        2, new()
                        {
                            { General.GameplayEquilibriumConstant.InGameMaterial, 960 },
                            { nameof(升华之印), 30 },
                            { nameof(流光之印), 10 }
                        }
                    },
                    {
                        3, new()
                        {
                            { General.GameplayEquilibriumConstant.InGameMaterial, 1760 },
                            { nameof(升华之印), 40 },
                            { nameof(流光之印), 20 }
                        }
                    },
                    {
                        4, new()
                        {
                            { General.GameplayEquilibriumConstant.InGameMaterial, 2800 },
                            { nameof(升华之印), 50 },
                            { nameof(流光之印), 30 },
                            { nameof(永恒之印), 10 }
                        }
                    },
                    {
                        5, new()
                        {
                            { General.GameplayEquilibriumConstant.InGameMaterial, 4080 },
                            { nameof(升华之印), 60 },
                            { nameof(流光之印), 40 },
                            { nameof(永恒之印), 20 }
                        }
                    },
                };
            }
        }

        public static Dictionary<int, Dictionary<string, int>> SkillLevelUpList
        {
            get
            {
                return new()
                {
                    {
                        1, new()
                        {
                            { "角色等级", 1 },
                            { General.GameplayEquilibriumConstant.InGameCurrency, 2000 },
                            { General.GameplayEquilibriumConstant.InGameMaterial, 10 },
                            { nameof(技能卷轴), 1 },
                        }
                    },
                    {
                        2, new()
                        {
                            { "角色等级", 12 },
                            { General.GameplayEquilibriumConstant.InGameCurrency, 5000 },
                            { General.GameplayEquilibriumConstant.InGameMaterial, 30 },
                            { nameof(技能卷轴), 2 },
                        }
                    },
                    {
                        3, new()
                        {
                            { "角色等级", 24 },
                            { General.GameplayEquilibriumConstant.InGameCurrency, 10000 },
                            { General.GameplayEquilibriumConstant.InGameMaterial, 60 },
                            { nameof(技能卷轴), 3 },
                            { nameof(智慧之果), 1 },
                        }
                    },
                    {
                        4, new()
                        {
                            { "角色等级", 36 },
                            { General.GameplayEquilibriumConstant.InGameCurrency, 18000 },
                            { General.GameplayEquilibriumConstant.InGameMaterial, 100 },
                            { nameof(技能卷轴), 4 },
                            { nameof(智慧之果), 2 },
                        }
                    },
                    {
                        5, new()
                        {
                            { "角色等级", 48 },
                            { General.GameplayEquilibriumConstant.InGameCurrency, 30000 },
                            { General.GameplayEquilibriumConstant.InGameMaterial, 150 },
                            { nameof(技能卷轴), 5 },
                            { nameof(智慧之果), 3 },
                            { nameof(奥术符文), 1 }
                        }
                    },
                    {
                        6, new()
                        {
                            { "角色等级", 60 },
                            { General.GameplayEquilibriumConstant.InGameCurrency, 47000 },
                            { General.GameplayEquilibriumConstant.InGameMaterial, 210 },
                            { nameof(技能卷轴), 6 },
                            { nameof(智慧之果), 4 },
                            { nameof(奥术符文), 2 }
                        }
                    }
                };
            }
        }

        public static Dictionary<int, Dictionary<string, int>> NormalAttackLevelUpList
        {
            get
            {
                return new()
                {
                    {
                        2, new()
                        {
                            { "角色等级", 8 },
                            { General.GameplayEquilibriumConstant.InGameCurrency, 2000 },
                            { General.GameplayEquilibriumConstant.InGameMaterial, 10 },
                            { nameof(技能卷轴), 1 },
                        }
                    },
                    {
                        3, new()
                        {
                            { "角色等级", 16 },
                            { General.GameplayEquilibriumConstant.InGameCurrency, 5000 },
                            { General.GameplayEquilibriumConstant.InGameMaterial, 30 },
                            { nameof(技能卷轴), 2 },
                        }
                    },
                    {
                        4, new()
                        {
                            { "角色等级", 24 },
                            { General.GameplayEquilibriumConstant.InGameCurrency, 10000 },
                            { General.GameplayEquilibriumConstant.InGameMaterial, 60 },
                            { nameof(技能卷轴), 3 },
                            { nameof(智慧之果), 1 },
                        }
                    },
                    {
                        5, new()
                        {
                            { "角色等级", 32 },
                            { General.GameplayEquilibriumConstant.InGameCurrency, 18000 },
                            { General.GameplayEquilibriumConstant.InGameMaterial, 100 },
                            { nameof(技能卷轴), 4 },
                            { nameof(智慧之果), 2 },
                        }
                    },
                    {
                        6, new()
                        {
                            { "角色等级", 40 },
                            { "角色突破进度", 4 },
                            { General.GameplayEquilibriumConstant.InGameCurrency, 30000 },
                            { General.GameplayEquilibriumConstant.InGameMaterial, 150 },
                            { nameof(技能卷轴), 5 },
                            { nameof(智慧之果), 3 },
                            { nameof(奥术符文), 1 }
                        }
                    },
                    {
                        7, new()
                        {
                            { "角色等级", 48 },
                            { General.GameplayEquilibriumConstant.InGameCurrency, 47000 },
                            { General.GameplayEquilibriumConstant.InGameMaterial, 210 },
                            { nameof(技能卷轴), 6 },
                            { nameof(智慧之果), 4 },
                            { nameof(奥术符文), 2 }
                        }
                    },
                    {
                        8, new()
                        {
                            { "角色等级", 56 },
                            { General.GameplayEquilibriumConstant.InGameCurrency, 70000 },
                            { General.GameplayEquilibriumConstant.InGameMaterial, 280 },
                            { nameof(技能卷轴), 7 },
                            { nameof(智慧之果), 5 },
                            { nameof(奥术符文), 3 },
                            { nameof(混沌之核), 1 }
                        }
                    }
                };
            }
        }

        public static Dictionary<EffectID, Dictionary<string, object>> RoundRewards
        {
            get
            {
                return new()
                {
                    {
                        EffectID.ExATK,
                        new()
                        {
                            { "exatk", Random.Shared.Next(40, 80) }
                        }
                    },
                    {
                        EffectID.ExCritRate,
                        new()
                        {
                            { "excr", Math.Clamp(Random.Shared.NextDouble(), 0.25, 0.5) }
                        }
                    },
                    {
                        EffectID.ExCritDMG,
                        new()
                        {
                            { "excrd", Math.Clamp(Random.Shared.NextDouble(), 0.5, 1) }
                        }
                    },
                    {
                        EffectID.ExATK2,
                        new()
                        {
                            { "exatk", Math.Clamp(Random.Shared.NextDouble(), 0.15, 0.3) }
                        }
                    },
                    {
                        EffectID.RecoverHP,
                        new()
                        {
                            { "hp", Random.Shared.Next(160, 640) }
                        }
                    },
                    {
                        EffectID.RecoverMP,
                        new()
                        {
                            { "mp", Random.Shared.Next(140, 490) }
                        }
                    },
                    {
                        EffectID.RecoverHP2,
                        new()
                        {
                            { "hp", Math.Clamp(Random.Shared.NextDouble(), 0.04, 0.08) }
                        }
                    },
                    {
                        EffectID.RecoverMP2,
                        new()
                        {
                            { "mp", Math.Clamp(Random.Shared.NextDouble(), 0.09, 0.18) }
                        }
                    },
                    {
                        EffectID.GetEP,
                        new()
                        {
                            { "ep", Random.Shared.Next(20, 40) }
                        }
                    }
                };
            }
        }

        public static Dictionary<string, string> QuestList
        {
            get
            {
                return new()
                {
                    {
                        "丢失的共享单车之谜",
                        "寻找被魔法传送走的共享单车。"
                    },
                    {
                        "咖啡店的神秘顾客",
                        "调查每天都点奇怪饮品的神秘顾客。"
                    },
                    {
                        "地铁里的幽灵乘客",
                        "找出在地铁里出没的半透明乘客。"
                    },
                    {
                        "公园的精灵涂鸦",
                        "清除公园里突然出现的精灵涂鸦。"
                    },
                    {
                        "手机信号的干扰源",
                        "找出干扰手机信号的魔法源头。"
                    },
                    {
                        "外卖小哥的奇遇",
                        "帮助外卖小哥找回被偷走的魔法外卖。"
                    },
                    {
                        "广场舞的魔法节奏",
                        "调查广场舞音乐中隐藏的魔法节奏。"
                    },
                    {
                        "自动贩卖机的秘密",
                        "找出自动贩卖机里突然出现的奇怪物品。"
                    },
                    {
                        "便利店的异次元入口",
                        "调查便利店里突然出现的异次元入口。"
                    },
                    {
                        "街头艺人的魔法表演",
                        "调查街头艺人表演中使用的魔法。"
                    },
                    {
                        "午夜电台的幽灵来电",
                        "调查午夜电台收到的奇怪来电。"
                    },
                    {
                        "高楼大厦的秘密通道",
                        "寻找隐藏在高楼大厦里的秘密通道。"
                    },
                    {
                        "城市下水道的神秘生物",
                        "调查城市下水道里出现的神秘生物。"
                    },
                    {
                        "废弃工厂的魔法实验",
                        "调查废弃工厂里进行的秘密魔法实验。"
                    },
                    {
                        "博物馆的活化雕像",
                        "调查博物馆里突然活化的雕像。"
                    },
                    {
                        "公园的都市传说",
                        "调查公园里流传的都市传说。"
                    },
                    {
                        "闹鬼公寓的真相",
                        "调查闹鬼公寓里的真相。"
                    },
                    {
                        "地下酒吧的秘密交易",
                        "调查地下酒吧里进行的秘密魔法交易。"
                    },
                    {
                        "旧书店的魔法书籍",
                        "寻找旧书店里隐藏的魔法书籍。"
                    },
                    {
                        "涂鸦墙的预言",
                        "解读涂鸦墙上出现的神秘预言。"
                    },
                    {
                        "黑客的魔法入侵",
                        "阻止黑客利用魔法入侵城市网络。"
                    },
                    {
                        "高科技魔法装备的测试",
                        "测试新型的高科技魔法装备。"
                    },
                    {
                        "无人机的魔法改造",
                        "改造无人机，使其拥有魔法能力。"
                    },
                    {
                        "人工智能的觉醒",
                        "调查人工智能觉醒的原因。"
                    },
                    {
                        "虚拟现实的魔法世界",
                        "探索虚拟现实中出现的魔法世界。"
                    },
                    {
                        "智能家居的魔法故障",
                        "修复智能家居的魔法故障。"
                    },
                    {
                        "能量饮料的魔法副作用",
                        "调查能量饮料的魔法副作用。"
                    },
                    {
                        "社交媒体的魔法病毒",
                        "清除社交媒体上出现的魔法病毒。"
                    },
                    {
                        "共享汽车的魔法漂移",
                        "调查共享汽车的魔法漂移现象。"
                    },
                    {
                        "城市监控的魔法干扰",
                        "修复城市监控的魔法干扰。"
                    },
                    {
                        "寻找丢失的魔法宠物",
                        "寻找在城市里走失的魔法宠物。"
                    },
                    {
                        "参加魔法美食节",
                        "参加城市举办的魔法美食节。"
                    },
                    {
                        "解开城市谜题",
                        "解开隐藏在城市各处的谜题。"
                    },
                    {
                        "参加魔法cosplay大赛",
                        "参加城市举办的魔法cosplay大赛。"
                    },
                    {
                        "寻找隐藏的魔法商店",
                        "寻找隐藏在城市里的魔法商店。"
                    },
                    {
                        "制作魔法主题的街头艺术",
                        "在城市里创作魔法主题的街头艺术。"
                    },
                    {
                        "举办一场魔法快闪活动",
                        "在城市里举办一场魔法快闪活动。"
                    },
                    {
                        "寻找失落的魔法乐器",
                        "寻找失落的魔法乐器，让城市充满音乐。"
                    },
                    {
                        "参加魔法运动会",
                        "参加城市举办的魔法运动会。"
                    },
                    {
                        "拯救被困在魔法结界里的市民",
                        "拯救被困在城市魔法结界里的市民。"
                    }
                };
            }
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
                    long effectID = (long)RoundRewards.Keys.ToArray()[Random.Shared.Next(RoundRewards.Count)];
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

        public static double CalculateRating(CharacterStatistics stats, Team? team = null)
        {
            // 基础得分
            double baseScore = (stats.Kills + stats.Assists) / (stats.Kills + stats.Assists + stats.Deaths + 0.01);
            if (team is null)
            {
                baseScore += stats.Kills * 0.1;
                if (stats.Deaths == 0)
                {
                    baseScore += 0.5;
                }
            }
            else
            {
                baseScore = baseScore * 0.6 + 0.4 * (stats.Kills / (stats.Kills + stats.Deaths + 0.01));
            }

            // 伤害贡献
            double logDamageContribution = Math.Log(1 + (stats.TotalDamage / (stats.TotalTakenDamage + 1e-6)));

            // 存活时间贡献
            double liveTimeContribution = Math.Log(1 + (stats.LiveTime / (stats.TotalTakenDamage + 0.01) * 100));

            // 团队模式参团率加成
            double teamContribution = 0;
            if (team != null)
            {
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
                if (isTeam)
                {
                    team = teams.Where(t => t.IsOnThisTeam(character)).FirstOrDefault();
                }
                statistics[character].Rating = CalculateRating(statistics[character], team);
            }
        }

        public static string CheckQuestList(EntityModuleConfig<Quest> quests)
        {
            if (quests.Count == 0)
            {
                // 生成任务
                for (int i = 0; i < 6; i++)
                {
                    int minutes = Random.Shared.Next(10, 41);
                    Dictionary<string, int> items = [];
                    items[General.GameplayEquilibriumConstant.InGameCurrency] = minutes * 20;
                    items[General.GameplayEquilibriumConstant.InGameMaterial] = minutes / 8 * 1;
                    int index = Random.Shared.Next(AllItems.Count);
                    Item item = AllItems[index];
                    items.Add(item.Name, 1);
                    while (true)
                    {
                        int index2 = Random.Shared.Next(AllItems.Count);
                        if (index2 != index)
                        {
                            Item item2 = AllItems[index2];
                            items.Add(item2.Name, 1);
                            break;
                        }
                    }
                    string name = QuestList.Keys.OrderBy(o => Random.Shared.Next()).First();
                    Quest quest = new()
                    {
                        Id = quests.Count > 0 ? quests.Values.Max(q => q.Id) + 1 : 1,
                        Name = name,
                        Description = QuestList[name],
                        EstimatedMinutes = minutes,
                        Awards = items
                    };
                    quests.Add(quest.GetIdName(), quest);
                }
                return "☆--- 今日任务列表 ---☆\r\n" + string.Join("\r\n", quests.Values.Select(q => q.ToString())) + "\r\n温馨提示：请务必在次日 4:00 前完成任务结算，未结算的任务都会被取消！";
            }
            else
            {
                if (quests.Count > 0)
                {
                    return "☆--- 今日任务列表 ---☆\r\n" + string.Join("\r\n", quests.Values.Select(q => q.ToString())) + "\r\n温馨提示：请务必在次日 4:00 前完成任务结算，未结算的任务都会被取消！";
                }
                else
                {
                    return "任务列表为空，请等待刷新！";
                }
            }
        }

        public static bool SettleQuest(User user, EntityModuleConfig<Quest> quests)
        {
            bool result = false;
            IEnumerable<Quest> workingQuests = quests.Values.Where(q => q.Status == QuestState.InProgress);
            foreach (Quest quest in workingQuests)
            {
                if (quest.StartTime.HasValue && quest.StartTime.Value.AddMinutes(quest.EstimatedMinutes) <= DateTime.Now)
                {
                    quest.Status = QuestState.Completed;
                    result = true;
                }
            }
            IEnumerable<Quest> finishQuests = quests.Values.Where(q => q.Status == QuestState.Completed);
            foreach (Quest quest in finishQuests)
            {
                quest.Status = QuestState.Settled;
                quest.SettleTime = DateTime.Now;
                foreach (string name in quest.Awards.Keys)
                {
                    if (name == General.GameplayEquilibriumConstant.InGameCurrency)
                    {
                        user.Inventory.Credits += quest.Awards[name];
                    }
                    else if (name == General.GameplayEquilibriumConstant.InGameMaterial)
                    {
                        user.Inventory.Materials += quest.Awards[name];
                    }
                    else if (AllItems.FirstOrDefault(i => i.Name == name) is Item item)
                    {
                        Item newItem = item.Copy();
                        newItem.User = user;
                        SetSellAndTradeTime(newItem);
                        user.Inventory.Items.Add(newItem);
                    }
                }
                result = true;
            }
            return result;
        }
    }
}
