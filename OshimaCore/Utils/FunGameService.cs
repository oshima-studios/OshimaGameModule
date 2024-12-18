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
                            { nameof(升华之印), 40 }
                        }
                    },
                    {
                        2, new()
                        {
                            { General.GameplayEquilibriumConstant.InGameMaterial, 1040 },
                            { nameof(升华之印), 75 }
                        }
                    },
                    {
                        3, new()
                        {
                            { General.GameplayEquilibriumConstant.InGameMaterial, 2320 },
                            { nameof(升华之印), 115 }
                        }
                    },
                    {
                        4, new()
                        {
                            { General.GameplayEquilibriumConstant.InGameMaterial, 4880 },
                            { nameof(升华之印), 160 }
                        }
                    },
                    {
                        5, new()
                        {
                            { General.GameplayEquilibriumConstant.InGameMaterial, 10000 },
                            { nameof(升华之印), 210 }
                        }
                    },
                };
            }
        }

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
            Items.AddRange([new 小经验书(), new 中经验书(), new 大经验书(), new 升华之印()]);

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
            user.Inventory.Characters.Clear();
            user.Inventory.Items.Clear();

            foreach (Character inventoryCharacter in characters)
            {
                Character realCharacter = CharacterBuilder.Build(inventoryCharacter, false, AllItems, AllSkills);
                realCharacter.User = user;
                user.Inventory.Characters.Add(realCharacter);
            }

            foreach (Item inventoryItem in items)
            {
                Item realItem = inventoryItem.Copy(true, true, true, AllItems, AllSkills);
                if (realItem.IsEquipment)
                {
                    IEnumerable<Character> has = user.Inventory.Characters.Where(character =>
                    {
                        if (realItem.ItemType == ItemType.MagicCardPack && character.EquipSlot.MagicCardPack != null && realItem.Guid == character.EquipSlot.MagicCardPack.Guid)
                        {
                            return true;
                        }
                        if (realItem.ItemType == ItemType.Weapon && character.EquipSlot.Weapon != null && realItem.Guid == character.EquipSlot.Weapon.Guid)
                        {
                            return true;
                        }
                        if (realItem.ItemType == ItemType.Armor && character.EquipSlot.Armor != null && realItem.Guid == character.EquipSlot.Armor.Guid)
                        {
                            return true;
                        }
                        if (realItem.ItemType == ItemType.Shoes && character.EquipSlot.Shoes != null && realItem.Guid == character.EquipSlot.Shoes.Guid)
                        {
                            return true;
                        }
                        if (realItem.ItemType == ItemType.Accessory)
                        {
                            if (character.EquipSlot.Accessory1 != null && realItem.Guid == character.EquipSlot.Accessory1.Guid)
                            {
                                return true;
                            }
                            else if (character.EquipSlot.Accessory2 != null && realItem.Guid == character.EquipSlot.Accessory2.Guid)
                            {
                                return true;
                            }
                        }
                        return false;
                    });
                    if (has.Any() && has.First() is Character character)
                    {
                        realItem.Character = character;
                    }
                }
                realItem.User = user;
                user.Inventory.Items.Add(realItem);
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
            int r = Random.Shared.Next(7);
            double q = Random.Shared.NextDouble() * 100;
            QualityType type = q switch
            {
                <= 37.21 => QualityType.White,
                <= 37.21 + 27.94 => QualityType.Green,
                <= 37.21 + 27.94 + 16.68 => QualityType.Blue,
                <= 37.21 + 27.94 + 16.68 + 9.79 => QualityType.Purple,
                <= 37.21 + 27.94 + 16.68 + 9.79 + 5.05 => QualityType.Orange,
                <= 37.21 + 27.94 + 16.68 + 9.79 + 5.05 + 2.73 => QualityType.Red,
                _ => QualityType.Gold
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
    }
}
