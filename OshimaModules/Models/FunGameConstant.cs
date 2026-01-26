using System.Collections.Concurrent;
using System.Text;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.Core.Constant;
using Oshima.FunGame.OshimaModules.Characters;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;
using Oshima.FunGame.OshimaModules.Items;
using Oshima.FunGame.OshimaModules.Regions;
using Oshima.FunGame.OshimaModules.Skills;

namespace Oshima.FunGame.OshimaModules.Models
{
    public class FunGameConstant
    {
        public const long CustomCharacterId = -1;
        public const int ItemsPerPage1 = 6;
        public const int ItemsPerPage2 = 10;
        public const int ExploreTime = 2;
        public const int MaxExploreTimes = 60;
        public const int DrawCardReduce = 1000;
        public const int DrawCardReduce_Material = 5;
        public const int SemaphoreSlimTimeout = 5000;
        public const int RoomExpireTime = 10;
        public static List<Character> Characters { get; } = [];
        public static List<Skill> Skills { get; } = [];
        public static List<Skill> PassiveSkills { get; } = [];
        public static List<Skill> CommonPassiveSkills { get; } = [];
        public static List<Skill> SuperSkills { get; } = [];
        public static List<Skill> CommonSuperSkills { get; } = [];
        public static List<Skill> Magics { get; } = [];
        public static List<Item> Equipment { get; } = [];
        public static List<Item> Items { get; } = [];
        public static List<Item> DrawCardItems { get; } = [];
        public static List<Item> CharacterLevelBreakItems { get; } = [];
        public static List<Item> SkillLevelUpItems { get; } = [];
        public static List<Item> NotForSaleItems { get; } = [];
        public static Dictionary<OshimaRegion, List<Item>> ExploreItems { get; } = [];
        public static List<Item> UserDailyItems { get; } = [];
        public static List<Skill> ItemSkills { get; } = [];
        public static List<Item> AllItems { get; } = [];
        public static List<Skill> AllSkills { get; } = [];
        public static Dictionary<long, User> UserIdAndUsername { get; } = [];
        public static Dictionary<long, LastStoreModel> UserLastVisitStore { get; } = [];
        public static ConcurrentDictionary<string, SemaphoreSlim> UserSemaphoreSlims { get; } = [];
        public static SemaphoreSlim MarketSemaphoreSlim { get; } = new(1, 1);
        public static Dictionary<string, Room> Rooms { get; set; } = [];
        public static Dictionary<long, Room> UsersInRoom { get; set; } = [];
        public static ItemType[] ItemCanUsed => [ItemType.Consumable, ItemType.MagicCard, ItemType.SpecialItem, ItemType.GiftBox, ItemType.Others];
        public static ItemType[] ItemCanNotDrawCard => [ItemType.Collectible, ItemType.QuestItem, ItemType.GiftBox, ItemType.Others];
        public static Dictionary<long, double> UserForgingRanking { get; } = [];
        public static Dictionary<long, int> UserHorseRacingRanking { get; } = [];
        public static Dictionary<long, int> UserCooperativeRanking { get; } = [];
        public static Dictionary<long, double> UserCreditsRanking { get; } = [];
        public static Dictionary<long, double> UserMaterialsRanking { get; } = [];
        public static Dictionary<long, double> UserEXPRanking { get; } = [];
        public static Dictionary<long, Club> ClubIdAndClub { get; } = [];
        public static DateTime RankingUpdateTime { get; set; } = DateTime.Now;
        public static char[] SplitChars => [',', ' ', '，', '；', ';'];

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
            Characters.Add(new ShiYu());
            Characters.Add(new XReouni());
            Characters.Add(new Neptune());
            Characters.Add(new CHAOS());
            Characters.Add(new Ryuko());
            Characters.Add(new TheGodK());

            Skills.AddRange([new 疾风步(), new 助威(), new 挑拨(), new 绞丝棍(), new 金刚击(), new 旋风轮(), new 双连击(), new 绝影(), new 胧(), new 魔眼(),
                new 天堂之吻(), new 回复弹(), new 养命功(), new 镜花水月(), new 剑风闪(), new 鲨鱼锚击(), new 疾走(), new 闪现()]);

            SuperSkills.AddRange([new 极寒渴望(), new 身心一境(), new 绝对领域(), new 零式灭杀(), new 三相灵枢(), new 变幻之心(), new 熵灭极诣(), new 残香凋零(), new 饕餮盛宴(),
                new 宿命时律(), new 千羽瞬华(), new 咒怨洪流(), new 放监(), new 归元环(), new 海王星的野望(), new 全军出击(), new 宿命之潮(), new 神之因果()]);

            PassiveSkills.AddRange([new META马(), new 心灵之弦(), new 蚀魂震击(), new 灵能反射(), new 双生流转(), new 零式崩解(), new 少女绮想(), new 暗香疏影(), new 破釜沉舟(),
                new 累积之压(), new 银隼之赐(), new 弱者猎手(), new 开宫(), new 八卦阵(), new 深海之戟(), new 雇佣兵团(), new 不息之流(), new 概念之骰()]);

            CommonPassiveSkills.AddRange([new 征服者(), new 致命节奏(), new 强攻(), new 电刑(), new 黑暗收割(), new 迅捷步法(), new 贪欲猎手()]);

            Magics.AddRange([new 冰霜攻击(), new 火之矢(), new 水之矢(), new 风之轮(), new 石之锤(), new 心灵之霞(), new 次元上升(), new 暗物质(),
                new 回复术(), new 治愈术(), new 复苏术(), new 圣灵术(), new 时间加速(), new 时间减速(), new 反魔法领域(), new 沉默十字(), new 虚弱领域(), new 混沌烙印(), new 凝胶稠絮(),
                new 大地之墙(), new 盖亚之盾(), new 风之守护(), new 结晶防护(), new 强音之力(), new 神圣祝福(), new 根源屏障(), new 灾难冲击波(), new 银色荆棘(), new 等离子之波(),
                new 地狱之门(), new 钻石星尘(), new 死亡咆哮(), new 鬼魅之痛(), new 导力停止(), new 冰狱冥嚎(), new 火山咆哮(), new 水蓝轰炸(), new 岩石之息(), new 弧形日珥(), new 苍白地狱(), new 破碎虚空(),
                new 弧光消耗(), new 回复术改(), new 回复术复(), new 治愈术复(), new 风之守护复(), new 强音之力复(), new 结晶防护复(), new 神圣祝福复(), new 时间加速改(), new 时间减速改(), new 时间加速复(), new 时间减速复()]);

            Dictionary<string, Item> exItems = Factory.GetGameModuleInstances<Item>(OshimaGameModuleConstant.General, OshimaGameModuleConstant.Item);
            Equipment.AddRange(exItems.Values.Where(i => (int)i.ItemType >= 0 && (int)i.ItemType < 5));
            Equipment.AddRange([new 攻击之爪10(), new 攻击之爪25(), new 攻击之爪40(), new 攻击之爪55(), new 攻击之爪70(), new 攻击之爪85(), new 糖糖一周年纪念武器(),
                new 糖糖一周年纪念防具(), new 糖糖一周年纪念鞋子(), new 糖糖一周年纪念饰品1(), new 糖糖一周年纪念饰品2()]);

            Items.AddRange(exItems.Values.Where(i => (int)i.ItemType > 4));
            Items.AddRange([new 小经验书(), new 中经验书(), new 大经验书(), new 升华之印(), new 流光之印(), new 永恒之印(), new 技能卷轴(), new 智慧之果(), new 奥术符文(), new 混沌之核(),
                new 小回复药(), new 中回复药(), new 大回复药(), new 魔力填充剂1(), new 魔力填充剂2(), new 魔力填充剂3(), new 能量饮料1(), new 能量饮料2(), new 能量饮料3(), new 年夜饭(), new 蛇年大吉(), new 新春快乐(), new 毕业礼包(),
                new 复苏药1(), new 复苏药2(), new 复苏药3(), new 全回复药(), new 魔法卡礼包(), new 奖券(), new 十连奖券(), new 改名卡(), new 原初之印(), new 创生之印(), new 法则精粹(), new 大师锻造券(),
                new 一周年纪念礼包(), new 一周年纪念套装(), new 冬至快乐(), new 圣诞礼包(), new 元旦快乐()
            ]);

            UserDailyItems.AddRange([new 青松(), new 流星石(), new 向日葵(), new 金铃花(), new 琉璃珠(), new 鸣草(), new 马尾(), new 鬼兜虫(), new 烈焰花花蕊(), new 堇瓜(), new 水晶球(), new 薰衣草(),
                new 青石(), new 莲花(), new 陶罐(), new 海灵芝(), new 四叶草(), new 露珠(), new 茉莉花(), new 绿萝(), new 檀木扇(), new 鸟蛋(), new 竹笋(), new 晶核(), new 手工围巾(), new 柳条篮(), new 风筝(), new 羽毛(), new 发光髓(),
                new 紫罗兰(), new 松果(), new 电气水晶(), new 薄荷(), new 竹节(), new 铁砧(), new 冰雾花(), new 海草(), new 磐石(), new 砂砾(), new 铁甲贝壳(), new 蜥蜴尾巴(), new 古老钟摆(), new 枯藤()]);

            NotForSaleItems.AddRange([new 一周年纪念礼包(), new 一周年纪念套装(), new 毕业礼包(), new 糖糖一周年纪念武器(), new 糖糖一周年纪念防具(), new 糖糖一周年纪念鞋子(),
                new 糖糖一周年纪念饰品1(), new 糖糖一周年纪念饰品2()]);

            AllItems.AddRange(Equipment);
            AllItems.AddRange(Items);
            AllItems.AddRange(UserDailyItems);
            AllItems.AddRange(NotForSaleItems);

            foreach (OshimaRegion region in Regions)
            {
                List<Item> items = [.. region.Crops.Select(i => i.Copy())];
                ExploreItems.Add(region, items);
            }

            long[] notDrawIds = [.. UserDailyItems.Union(NotForSaleItems).Select(i => i.Id)];
            DrawCardItems.AddRange(AllItems.Where(i => !ItemCanNotDrawCard.Contains(i.ItemType) && !notDrawIds.Contains(i.Id)));
            CharacterLevelBreakItems.AddRange([new 升华之印(), new 流光之印(), new 永恒之印(), new 原初之印(), new 创生之印()]);
            SkillLevelUpItems.AddRange([new 技能卷轴(), new 智慧之果(), new 奥术符文(), new 混沌之核(), new 法则精粹()]);

            AllItems.AddRange(ExploreItems.Values.SelectMany(list => list));

            foreach (OshimaRegion region in Regions)
            {
                RegionsName[region.Id] = region.Name;
                RegionsDifficulty[region.Id] = region.Difficulty;
                IEnumerable<Item> items = AllItems.Where(i => i.Others.TryGetValue("region", out object? value) && int.TryParse(value.ToString(), out int rid) && rid == region.Id).Select(i => i.Copy()).OrderByDescending(i => i.QualityType);
                foreach (Item item in items)
                {
                    region.Items.Add(item);
                }
            }

            foreach (OshimaRegion region in PlayerRegions)
            {
                RegionsName[region.Id] = region.Name;
                RegionsDifficulty[region.Id] = region.Difficulty;
                IEnumerable<Item> items;
                if (region.Id == 0)
                {
                    items = Equipment.Where(i => !i.Others.ContainsKey("region")).Select(i => i.Copy()).OrderByDescending(i => i.QualityType);
                }
                else
                {
                    items = AllItems.Where(i => i.Others.TryGetValue("region", out object? value) && int.TryParse(value.ToString(), out int rid) && rid == region.Id).Select(i => i.Copy()).OrderByDescending(i => i.QualityType);
                }
                foreach (Item item in items)
                {
                    region.Items.Add(item);
                }
            }

            Skill?[] activeSkills = [.. Equipment.Select(i => i.Skills.Active), .. Items.Select(i => i.Skills.Active)];
            foreach (Skill? skill in activeSkills)
            {
                if (skill != null)
                {
                    ItemSkills.Add(skill);
                }
            }
            ItemSkills.AddRange([.. Equipment.SelectMany(i => i.Skills.Passives), .. Items.SelectMany(i => i.Skills.Passives)]);

            AllSkills.AddRange(Magics);
            AllSkills.AddRange(Skills);
            AllSkills.AddRange(PassiveSkills);
            AllSkills.AddRange(CommonPassiveSkills);
            AllSkills.AddRange(ItemSkills);
            AllSkills.AddRange(SuperSkills);
            AllSkills.AddRange(CommonSuperSkills);
        }

        public static void Reload()
        {
            Characters.Clear();
            Equipment.Clear();
            Skills.Clear();
            SuperSkills.Clear();
            CommonSuperSkills.Clear();
            PassiveSkills.Clear();
            CommonPassiveSkills.Clear();
            Magics.Clear();
            DrawCardItems.Clear();
            ExploreItems.Clear();
            AllItems.Clear();
            ItemSkills.Clear();
            AllSkills.Clear();
            InitFunGame();
        }

        public static Dictionary<int, Dictionary<string, int>> LevelBreakNeedyList { get; } = new()
        {
            {
                0, new()
                {
                    { General.GameplayEquilibriumConstant.InGameMaterial, 40 },
                    { nameof(升华之印), 2 }
                }
            },
            {
                1, new()
                {
                    { General.GameplayEquilibriumConstant.InGameMaterial, 80 },
                    { nameof(升华之印), 4 },
                    { nameof(流光之印), 2 }
                }
            },
            {
                2, new()
                {
                    { General.GameplayEquilibriumConstant.InGameMaterial, 160 },
                    { nameof(升华之印), 8 },
                    { nameof(流光之印), 4 },
                    { nameof(永恒之印), 3 }
                }
            },
            {
                3, new()
                {
                    { General.GameplayEquilibriumConstant.InGameMaterial, 320 },
                    { nameof(升华之印), 16 },
                    { nameof(流光之印), 8 },
                    { nameof(永恒之印), 6 },
                    { nameof(原初之印), 5 }
                }
            },
            {
                4, new()
                {
                    { General.GameplayEquilibriumConstant.InGameMaterial, 640 },
                    { nameof(升华之印), 32 },
                    { nameof(流光之印), 16 },
                    { nameof(永恒之印), 12 },
                    { nameof(原初之印), 10 },
                    { nameof(创生之印), 5 }
                }
            },
            {
                5, new()
                {
                    { General.GameplayEquilibriumConstant.InGameMaterial, 1280 },
                    { nameof(升华之印), 64 },
                    { nameof(流光之印), 32 },
                    { nameof(永恒之印), 24 },
                    { nameof(原初之印), 20 },
                    { nameof(创生之印), 10 }
                }
            },
        };

        public static Dictionary<int, Dictionary<string, int>> SkillLevelUpList { get; } = new()
        {
            {
                1, new()
                {
                    { "角色等级", 1 },
                    { General.GameplayEquilibriumConstant.InGameCurrency, 5000 },
                    { General.GameplayEquilibriumConstant.InGameMaterial, 50 },
                    { nameof(技能卷轴), 1 }
                }
            },
            {
                2, new()
                {
                    { "角色等级", 12 },
                    { General.GameplayEquilibriumConstant.InGameCurrency, 10000 },
                    { General.GameplayEquilibriumConstant.InGameMaterial, 100 },
                    { nameof(技能卷轴), 2 },
                    { nameof(智慧之果), 1 }
                }
            },
            {
                3, new()
                {
                    { "角色等级", 24 },
                    { General.GameplayEquilibriumConstant.InGameCurrency, 20000 },
                    { General.GameplayEquilibriumConstant.InGameMaterial, 150 },
                    { nameof(技能卷轴), 4 },
                    { nameof(智慧之果), 2 },
                    { nameof(奥术符文), 1 }
                }
            },
            {
                4, new()
                {
                    { "角色等级", 36 },
                    { General.GameplayEquilibriumConstant.InGameCurrency, 40000 },
                    { General.GameplayEquilibriumConstant.InGameMaterial, 250 },
                    { nameof(技能卷轴), 8 },
                    { nameof(智慧之果), 4 },
                    { nameof(奥术符文), 3 },
                    { nameof(混沌之核), 1 }
                }
            },
            {
                5, new()
                {
                    { "角色等级", 48 },
                    { General.GameplayEquilibriumConstant.InGameCurrency, 70000 },
                    { General.GameplayEquilibriumConstant.InGameMaterial, 400 },
                    { nameof(技能卷轴), 16 },
                    { nameof(智慧之果), 8 },
                    { nameof(奥术符文), 6 },
                    { nameof(混沌之核), 4 },
                    { nameof(法则精粹), 1 }
                }
            },
            {
                6, new()
                {
                    { "角色等级", 60 },
                    { General.GameplayEquilibriumConstant.InGameCurrency, 110000 },
                    { General.GameplayEquilibriumConstant.InGameMaterial, 600 },
                    { nameof(技能卷轴), 32 },
                    { nameof(智慧之果), 16 },
                    { nameof(奥术符文), 12 },
                    { nameof(混沌之核), 8 },
                    { nameof(法则精粹), 5 }
                }
            }
        };

        public static Dictionary<int, Dictionary<string, int>> NormalAttackLevelUpList { get; } = new()
        {
            {
                2, new()
                {
                    { "角色等级", 8 },
                    { General.GameplayEquilibriumConstant.InGameCurrency, 2000 },
                    { General.GameplayEquilibriumConstant.InGameMaterial, 30 },
                    { nameof(技能卷轴), 1 }
                }
            },
            {
                3, new()
                {
                    { "角色等级", 16 },
                    { General.GameplayEquilibriumConstant.InGameCurrency, 10000 },
                    { General.GameplayEquilibriumConstant.InGameMaterial, 100 },
                    { nameof(技能卷轴), 2 },
                    { nameof(智慧之果), 1 }
                }
            },
            {
                4, new()
                {
                    { "角色等级", 24 },
                    { General.GameplayEquilibriumConstant.InGameCurrency, 20000 },
                    { General.GameplayEquilibriumConstant.InGameMaterial, 150 },
                    { nameof(技能卷轴), 4 },
                    { nameof(智慧之果), 2 },
                    { nameof(奥术符文), 1 }
                }
            },
            {
                5, new()
                {
                    { "角色等级", 32 },
                    { General.GameplayEquilibriumConstant.InGameCurrency, 40000 },
                    { General.GameplayEquilibriumConstant.InGameMaterial, 250 },
                    { nameof(技能卷轴), 8 },
                    { nameof(智慧之果), 4 },
                    { nameof(奥术符文), 3 },
                    { nameof(混沌之核), 1 }
                }
            },
            {
                6, new()
                {
                    { "角色等级", 40 },
                    { "角色突破进度", 4 },
                    { General.GameplayEquilibriumConstant.InGameCurrency, 70000 },
                    { General.GameplayEquilibriumConstant.InGameMaterial, 400 },
                    { nameof(技能卷轴), 16 },
                    { nameof(智慧之果), 8 },
                    { nameof(奥术符文), 6 },
                    { nameof(混沌之核), 4 },
                    { nameof(法则精粹), 1 }
                }
            },
            {
                7, new()
                {
                    { "角色等级", 48 },
                    { General.GameplayEquilibriumConstant.InGameCurrency, 110000 },
                    { General.GameplayEquilibriumConstant.InGameMaterial, 600 },
                    { nameof(技能卷轴), 32 },
                    { nameof(智慧之果), 16 },
                    { nameof(奥术符文), 12 },
                    { nameof(混沌之核), 8 },
                    { nameof(法则精粹), 5 }
                }
            },
            {
                8, new()
                {
                    { "角色等级", 56 },
                    { General.GameplayEquilibriumConstant.InGameCurrency, 160000 },
                    { General.GameplayEquilibriumConstant.InGameMaterial, 850 },
                    { nameof(技能卷轴), 64 },
                    { nameof(智慧之果), 32 },
                    { nameof(奥术符文), 24 },
                    { nameof(混沌之核), 16 },
                    { nameof(法则精粹), 10 }
                }
            }
        };

        public static Dictionary<EffectID, Dictionary<string, object>> RoundRewards => new()
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
                EffectID.ExMaxMP2,
                new()
                {
                    { "exmp", 5 }
                }
            },
            {
                EffectID.AccelerationCoefficient,
                new()
                {
                    { "exacc", 1 }
                }
            },
            {
                EffectID.IgnoreEvade,
                new()
                {
                    { "p", 1 }
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

        public static List<OshimaRegion> Regions { get; } = [
            new 银辉城(), new 瑟兰薇歌林海(), new 永燃坩埚(), new 永霜裂痕(), new 齿轮坟场(), new 千瞳镜湖(),
            new 时之荒漠(), new 谵妄海市(), new 腐萤沼渊(), new 双生月崖(), new 棱镜骨桥(), new 泰坦遗迹()
        ];

        public static List<OshimaRegion> PlayerRegions { get; } = [
            new 铎京城()
        ];

        public static Dictionary<long, string> RegionsName { get; } = [];

        public static Dictionary<long, RarityType> RegionsDifficulty { get; } = [];

        private readonly static Dictionary<int, double> _precomputeTotalExperience = [];
        public static Dictionary<int, double> PrecomputeTotalExperience
        {
            get
            {
                if (_precomputeTotalExperience.Count == 0)
                {
                    double sum = 0;
                    _precomputeTotalExperience[1] = 0;
                    for (int i = 2; i <= General.GameplayEquilibriumConstant.MaxLevel; i++)
                    {
                        sum += General.GameplayEquilibriumConstant.EXPUpperLimit[i - 1];
                        _precomputeTotalExperience[i] = sum;
                    }
                    return _precomputeTotalExperience;
                }
                return _precomputeTotalExperience;
            }
        }

        public static Dictionary<ExploreResult, double> ExploreResultProbabilities { get; } = new()
        {
            { ExploreResult.General, 0.45 },
            { ExploreResult.Nothing, 0.2 },
            { ExploreResult.Fight, 0.25 },
            { ExploreResult.Earned, 0.1 },
            { ExploreResult.Event, 0 }
        };

        /// <summary>
        /// 参数说明：{0} 奖励内容字符串，{1} 敌人名称，{2} NPC名称，{3} 物品名称1，{4} 地点名称1，{5} 物品名称1，{6} 地点名称2
        /// </summary>
        public static Dictionary<ExploreResult, List<string>> ExploreString { get; } = new()
        {
            {
                ExploreResult.General, [
                    "当{3}的光芒洒满{4}时，{6}显露出了被封印的{0}，你快马加鞭冲了上去夺走！",
                    "{2}的低语在风中消散，但留在你手中的是闪耀的{0}！",
                    "恭喜你！成功在荒野中迷路！奖励…等等，好像是：{0}？至少不是空手而归…",
                    "你凝视着远方，远方也凝视着你…然后，你获得了：{0}！这大概就是命运吧。",
                    "探索结果：空气，阳光，还有…奖励：{0}！看来今天运气还不错？",
                    "你随手拨开{4}的草丛，惊喜地发现{0}在阳光下闪闪发光！",
                    "{3}在{4}的微光中低语，指引你走向了意外的{0}！"
                ]
            },
            {
                ExploreResult.Nothing, [
                    "风沙抹去了所有痕迹，只有{3}见证过你的到来。（什么也没有获得）",
                    "{4}地牢里的封印纹丝未动，仿佛在嘲笑着你的徒劳……（什么也没有获得）",
                    "在你的注视下，{4}的宝藏已被{2}掠夺一空。（什么也没有获得）",
                    "你对着空地发呆了半天，只剩冰冷的{4}和你的失望。（什么也没有获得）",
                    "在空荡的回响中传来讥笑，原来{4}的秘宝不过是个传说。（什么也没有获得）",
                    "{4}的废墟中只有风声作伴，{3}也无法指引你找到任何东西。（什么也没有获得）",
                    "你翻遍了{4}的每个角落，{2}的笑声仿佛在嘲笑你的空手而归。（什么也没有获得）",
                    "{6}的阴影笼罩着{4}，但无论你如何寻找，宝藏依然无踪。（什么也没有获得）",
                    "在{4}的荒凉中，你只找到了一堆无用的碎石和失望。（什么也没有获得）",
                    "{3}的光芒在{4}中黯淡，仿佛连它都不愿理会你的探索。（什么也没有获得）"
                ]
            },
            {
                ExploreResult.Fight, [
                    "{4}的地面突然裂开，{1}扑向了你！迎接战斗吧！",
                    "当你触碰{3}时，{1}的咆哮震撼着{4}！",
                    "原来这里真的有危险……是{1}守卫着{4}，不得不战了！",
                    "在探索{4}的某处时，身旁的墙突然破裂，{1}从阴影中降临！",
                    "你惊动了{1}！{4}瞬间化作战场！",
                    "{1}从{4}的黑暗中冲出，{3}的光芒成了战斗的信号！",
                    "{4}的宁静被打破，{1}的怒吼在{6}回荡，战斗一触即发！",
                    "当你靠近{3}时，{1}从{4}的迷雾中现身，剑拔弩张！",
                    "{4}的深处传来低吼，{1}正守候着，你别无选择只能应战！",
                    "你踏入{4}的禁地，{1}的咆哮宣告了战斗的开始！"
                ]
            },
            {
                ExploreResult.Earned, [
                    "当{3}发出诡异的光芒照亮{4}时，藏在其中的{0}突然落入你手中！",
                    "祝福应验！{4}深处的{0}为你所有！",
                    "解开{4}地牢中的谜题后，{0}终于显现！",
                    "屏障消散，至宝{0}光芒万丈，自觉地飞进了你的口袋！",
                    "在偶遇{1}和{2}的遭遇战时，你渔翁得利抢到了：{0}！",
                    "{4}的古老祭坛发出光芒，{0}从{3}中缓缓升起，归于你手！",
                    "你无意间触碰{4}的机关，{0}如流星般落入你的怀抱！",
                    "{2}的指引让你在{4}的废墟中发现了闪耀的{0}！",
                    "在{6}的星光下，{3}为你揭示了{0}的藏身之处！",
                    "{4}的秘密通道在{3}的指引下开启，{0}赫然出现在你面前！"
                ]
            }
        };

        public static Dictionary<QualityType, double> DrawCardProbabilities { get; } = new()
        {
            { QualityType.White, 49.53 },
            { QualityType.Green, 19.35 },
            { QualityType.Blue, 13.48 },
            { QualityType.Purple, 8.25 },
            { QualityType.Orange, 6.33 },
            { QualityType.Red, 3.06 }
        };

        public static Dictionary<QualityType, (int Min, int Max)> PriceRanges { get; } = new()
        {
            { QualityType.White, (200, 800) },
            { QualityType.Green, (800, 4200) },
            { QualityType.Blue, (4200, 11000) },
            { QualityType.Purple, (11000, 32200) },
            { QualityType.Orange, (32200, 67000) },
            { QualityType.Red, (67000, 130000) },
            { QualityType.Gold, (130000, 240000) }
        };

        public static Dictionary<QualityType, double> DecomposedMaterials { get; } = new()
        {
            { QualityType.Gold, 128 },
            { QualityType.Red, 64 },
            { QualityType.Orange, 32 },
            { QualityType.Purple, 16 },
            { QualityType.Blue, 8 },
            { QualityType.Green, 3 },
            { QualityType.White, 1 }
        };

        public static Dictionary<QualityType, double> ForgeNeedy { get; } = new()
        {
            { QualityType.Red, 230 },
            { QualityType.Orange, 170 },
            { QualityType.Purple, 120 },
            { QualityType.Blue, 80 },
            { QualityType.Green, 50 },
            { QualityType.White, 30 }
        };

        public static Dictionary<RarityType, double> ForgeRegionCoefficient { get; } = new()
        {
            { RarityType.OneStar, 0.5 },
            { RarityType.TwoStar, 0.6 },
            { RarityType.ThreeStar, 0.7 },
            { RarityType.FourStar, 0.85 },
            { RarityType.FiveStar, 1 }
        };

        public static string[] GreekAlphabet { get; } = ["α", "β", "γ", "δ", "ε", "ζ", "η", "θ", "ι", "κ", "λ", "μ", "ν", "ξ", "ο", "π", "ρ", "σ", "τ", "υ", "φ", "χ", "ψ", "ω"];

        public static string[] CommonSurnames { get; } = [
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
            "司", "钟", "方", "谢",
            "赵", "钱", "孙", "李", "周", "吴", "郑", "王", "冯", "陈", "卫", "蒋", "沈", "韩",
            "杨", "朱", "秦", "许", "何", "吕", "张", "孔", "曹", "严", "华", "金", "魏", "陶",
            "姜", "谢", "罗", "徐", "林", "范", "方", "唐", "柳", "宋", "元", "萧", "程", "陆",
            "顾", "楚", "白", "苏", "叶", "萧", "莫", "凌", "寒", "龙", "凤", "蓝", "冷", "华",
            "唐", "韩", "庄", "青", "安", "晏", "尹", "施", "孟", "荀", "傅", "钟", "方", "谢",
            "司马", "欧阳", "上官", "慕容", "尉迟", "司徒", "轩辕", "端木", "南宫", "长孙",
            "百里", "东方", "西门", "独孤", "公孙", "令狐", "宇文", "夏侯", "赫连", "皇甫",
            "墨", "君", "月", "紫", "卿", "微", "清", "易", "霜", "兰", "语", "雪", "璃",
            "镜", "弦", "珏", "瑾", "璇", "绯", "霁", "溟", "澈", "归", "羽", "辰", "芷",
            "风", "花", "江", "河", "湖", "海", "山", "川", "松", "竹", "梅", "菊", "枫",
            "梧", "泉", "溪", "岚", "雾", "露", "霓", "霰", "星", "辰",
            "沧", "溟", "无", "绝", "孤", "隐", "斩", "破", "惊", "鸿", "御", "玄", "冥",
            "烬", "夙", "离",
            "东篱", "南笙", "西楼", "北冥", "九歌", "长离", "扶摇", "青丘", "凌霄", "重光",
            "子车", "亓官", "巫马", "拓跋", "叱干", "斛律", "沮渠", "秃发", "万俟", "仆固"
        ];

        public static string CommonChineseCharacters { get; } =
                "云星宝灵梦龙花雨风叶山川月石羽水竹金" +
                "玉海火雷光天地凤虎虹珠华霞鹏雪银沙松桃兰青霜鸿康骏波泉河湖江泽洋林枫" +
                "梅桂樱桐晴韵凌若悠碧涛渊壁剑影霖玄承珍雅耀瑞鹤烟燕霏翼翔璃绮纱绫绣锦" +
                "瑜琼瑾璇璧琳琪瑶瑛芝杏茜荷莉莹菡莲诗瑰翠椒槐榆槿柱梧曜曙晶暖智煌熙霓" +
                "熠嘉琴曼菁蓉菲淑妙惠秋涵映巧慧茹荣菱曦容芬玲澜清湘澄泓润珺晨翠涟洁悠" +
                "霏淑绮润东南西北云山川风月溪雪雨雷天云海霜柏芳春秋夏冬温景寒和竹阳溪" +
                "溪飞风峰阳一乙二十丁厂七卜八人入儿匕几九刁了刀力乃又三干于亏工土士才" +
                "下寸大丈与万上小口山巾千乞川亿个夕久么勺凡丸及广亡门丫义之尸己已巳弓" +
                "子卫也女刃飞习叉马乡丰王开井天夫元无云专丐扎艺木五支厅不犬太区历歹友" +
                "尤匹车巨牙屯戈比互切瓦止少曰日中贝冈内水见午牛手气毛壬升夭长仁什片仆" +
                "化仇币仍仅斤爪反介父从仑今凶分乏公仓月氏勿欠风丹匀乌勾凤六文亢方火为" +
                "斗忆计订户认冗讥心尺引丑巴孔队办以允予邓劝双书幻玉刊未末示击打巧正扑" +
                "卉扒功扔去甘世艾古节本术可丙左厉石右布夯戊龙平灭轧东卡北占凸卢业旧帅" +
                "归旦目且叶甲申叮电号田由只叭史央兄叽叼叫叩叨另叹冉皿凹囚四生矢失乍禾" +
                "丘付仗代仙们仪白仔他斥瓜乎丛令用甩印尔乐句匆册卯犯外处冬鸟务包饥主市" +
                "立冯玄闪兰半汁汇头汉宁穴它讨写让礼训议必讯记永司尼民弗弘出辽奶奴召加" +
                "皮边孕发圣对台矛纠母幼丝邦式迂刑戎动扛寺吉扣考托老巩圾执扩扫地场扬耳" +
                "芋共芒亚芝朽朴机权过臣吏再协西压厌戌在百有存而页匠夸夺灰达列死成夹夷" +
                "轨邪尧划迈毕至此贞师尘尖劣光当早吁吐吓虫曲团吕同吊吃因吸吗吆屿屹岁帆" +
                "回岂则刚网肉年朱先丢廷舌竹迁乔迄伟传乒乓休伍伏优臼伐延仲件任伤价伦份" +
                "华仰仿伙伪自伊血向似后行舟全会杀合兆企众爷伞创肌肋朵杂危旬旨旭负匈名" +
                "各多争色壮冲妆冰庄庆亦刘齐交衣次产决亥充妄闭问闯羊并关米灯州汗污江汛" +
                "池汝汤忙兴宇守宅字安讲讳军讶许讹论讼农讽设访诀寻那迅尽导异弛孙阵阳收" +
                "阶阴防奸如妇妃好她妈戏羽观欢买红驮纤驯约级纪驰纫巡寿弄麦玖玛形进戒吞" +
                "远违韧运扶抚坛技坏抠扰扼拒找批址扯走抄贡汞坝攻赤折抓扳抡扮抢孝坎均抑" +
                "抛投坟坑抗坊抖护壳志块扭声把报拟却抒劫芙芜苇芽花芹芥芬苍芳严芦芯劳克" +
                "芭苏杆杠杜材村杖杏杉巫极李杨求甫匣更束吾豆两酉丽医辰励否还尬歼来连轩" +
                "步卤坚肖旱盯呈时吴助县里呆吱吠呕园旷围呀吨足邮男困吵串员呐听吟吩呛吻" +
                "吹呜吭吧邑吼囤别吮岖岗帐财针钉牡告我乱利秃秀私每兵估体何佐佑但伸佃作" +
                "伯伶佣低你住位伴身皂伺佛囱近彻役返余希坐谷妥含邻岔肝肛肚肘肠龟甸免狂" +
                "犹狈角删条彤卵灸岛刨迎饭饮系言冻状亩况床库庇疗吝应这冷庐序辛弃冶忘闰" +
                "闲间闷判兑灶灿灼弟汪沐沛汰沥沙汽沃沦汹泛沧没沟沪沈沉沁怀忧忱快完宋宏" +
                "牢究穷灾良证启评补初社祀识诈诉罕诊词译君灵即层屁尿尾迟局改张忌际陆阿" +
                "陈阻附坠妓妙妖姊妨妒努忍劲矣鸡纬驱纯纱纲纳驳纵纷纸纹纺驴纽奉玩环武青" +
                "责现玫表规抹卦坷坯拓拢拔坪拣坦担坤押抽拐拖者拍顶拆拎拥抵拘势抱拄垃拉" +
                "拦幸拌拧拂拙招坡披拨择抬拇拗其取茉苦昔苛若茂苹苗英苟苑苞范直茁茄茎苔" +
                "茅枉林枝杯枢柜枚析板松枪枫构杭杰述枕丧或画卧事刺枣雨卖郁矾矿码厕奈奔" +
                "奇奋态欧殴垄妻轰顷转斩轮软到非叔歧肯齿些卓虎虏肾贤尚旺具味果昆国哎咕" +
                "昌呵畅明易咙昂迪典固忠呻咒咋咐呼鸣咏呢咄咖岸岩帖罗帜帕岭凯败账贩贬购" +
                "贮图钓制知迭氛垂牧物乖刮秆和季委秉佳侍岳供使例侠侥版侄侦侣侧凭侨佩货" +
                "侈依卑的迫质欣征往爬彼径所舍金刹命肴斧爸采觅受乳贪念贫忿肤肺肢肿胀朋" +
                "股肮肪肥服胁周昏鱼兔狐忽狗狞备饰饱饲变京享庞店夜庙府底疟疙疚剂卒郊庚" +
                "废净盲放刻育氓闸闹郑券卷单炬炒炊炕炎炉沫浅法泄沽河沾泪沮油泊沿泡注泣" +
                "泞泻泌泳泥沸沼波泼泽治怔怯怖性怕怜怪怡学宝宗定宠宜审宙官空帘宛实试郎" +
                "诗肩房诚衬衫视祈话诞诡询该详建肃录隶帚屉居届刷屈弧弥弦承孟陋陌孤陕降" +
                "函限妹姑姐姓妮始姆迢驾叁参艰线练组绅细驶织驹终驻绊驼绍绎经贯契贰奏春" +
                "帮玷珍玲珊玻毒型拭挂封持拷拱项垮挎城挟挠政赴赵挡拽哉挺括垢拴拾挑垛指" +
                "垫挣挤拼挖按挥挪拯某甚荆茸革茬荐巷带草茧茵茶荒茫荡荣荤荧故胡荫荔南药" +
                "标栈柑枯柄栋相查柏栅柳柱柿栏柠树勃要柬咸威歪研砖厘厚砌砂泵砚砍面耐耍" +
                "牵鸥残殃轴轻鸦皆韭背战点虐临览竖省削尝昧盹是盼眨哇哄哑显冒映星昨咧昭" +
                "畏趴胃贵界虹虾蚁思蚂虽品咽骂勋哗咱响哈哆咬咳咪哪哟炭峡罚贱贴贻骨幽钙" +
                "钝钞钟钢钠钥钦钧钩钮卸缸拜看矩毡氢怎牲选适秒香种秋科重复竿段便俩贷顺" +
                "修俏保促俄俐侮俭俗俘信皇泉鬼侵禹侯追俊盾待徊衍律很须叙剑逃食盆胚胧胆" +
                "胜胞胖脉胎勉狭狮独狰狡狱狠贸怨急饵饶蚀饺饼峦弯将奖哀亭亮度迹庭疮疯疫" +
                "疤咨姿亲音帝施闺闻闽阀阁差养美姜叛送类迷籽娄前首逆兹总炼炸烁炮炫烂剃" +
                "洼洁洪洒柒浇浊洞测洗活派洽染洛浏济洋洲浑浓津恃恒恢恍恬恤恰恼恨举觉宣" +
                "宦室宫宪突穿窃客诫冠诬语扁袄祖神祝祠误诱诲说诵垦退既屋昼屏屎费陡逊眉" +
                "孩陨除险院娃姥姨姻娇姚娜怒架贺盈勇怠癸蚤柔垒绑绒结绕骄绘给绚骆络绝绞" +
                "骇统耕耘耗耙艳泰秦珠班素匿蚕顽盏匪捞栽捕埂捂振载赶起盐捎捍捏埋捉捆捐" +
                "损袁捌都哲逝捡挫换挽挚热恐捣壶捅埃挨耻耿耽聂恭莽莱莲莫莉荷获晋恶莹莺" +
                "真框梆桂桔栖档桐株桥桦栓桃格桩校核样根索哥速逗栗贾酌配翅辱唇夏砸砰砾" +
                "础破原套逐烈殊殉顾轿较顿毙致柴桌虑监紧党逞晒眠晓哮唠鸭晃哺晌剔晕蚌畔" +
                "蚣蚊蚪蚓哨哩圃哭哦恩鸯唤唁哼唧啊唉唆罢峭峨峰圆峻贼贿赂赃钱钳钻钾铁铃" +
                "铅缺氧氨特牺造乘敌秤租积秧秩称秘透笔笑笋债借值倚俺倾倒倘俱倡候赁俯倍" +
                "倦健臭射躬息倔徒徐殷舰舱般航途拿耸爹舀爱豺豹颁颂翁胰脆脂胸胳脏脐胶脑" +
                "脓逛狸狼卿逢鸵留鸳皱饿馁凌凄恋桨浆衰衷高郭席准座症病疾斋疹疼疲脊效离" +
                "紊唐瓷资凉站剖竞部旁旅畜阅羞羔瓶拳粉料益兼烤烘烦烧烛烟烙递涛浙涝浦酒" +
                "涉消涡浩海涂浴浮涣涤流润涧涕浪浸涨烫涩涌悖悟悄悍悔悯悦害宽家宵宴宾窍" +
                "窄容宰案请朗诸诺读扇诽袜袖袍被祥课冥谁调冤谅谆谈谊剥恳展剧屑弱陵祟陶" +
                "陷陪娱娟恕娥娘通能难预桑绢绣验继骏球琐理琉琅捧堵措描域捺掩捷排焉掉捶" +
                "赦堆推埠掀授捻教掏掐掠掂培接掷控探据掘掺职基聆勘聊娶著菱勒黄菲萌萝菌" +
                "萎菜萄菊菩萍菠萤营乾萧萨菇械彬梦婪梗梧梢梅检梳梯桶梭救曹副票酝酗厢戚" +
                "硅硕奢盔爽聋袭盛匾雪辅辆颅虚彪雀堂常眶匙晨睁眯眼悬野啪啦曼晦晚啄啡距" +
                "趾啃跃略蚯蛀蛇唬累鄂唱患啰唾唯啤啥啸崖崎崭逻崔帷崩崇崛婴圈铐铛铝铜铭" +
                "铲银矫甜秸梨犁秽移笨笼笛笙符第敏做袋悠偿偶偎偷您售停偏躯兜假衅徘徙得" +
                "衔盘舶船舵斜盒鸽敛悉欲彩领脚脖脯豚脸脱象够逸猜猪猎猫凰猖猛祭馅馆凑减" +
                "毫烹庶麻庵痊痒痕廊康庸鹿盗章竟商族旋望率阎阐着羚盖眷粘粗粒断剪兽焊焕" +
                "清添鸿淋涯淹渠渐淑淌混淮淆渊淫渔淘淳液淤淡淀深涮涵婆梁渗情惜惭悼惧惕" +
                "惟惊惦悴惋惨惯寇寅寄寂宿窒窑密谋谍谎谐袱祷祸谓谚谜逮敢尉屠弹隋堕随蛋" +
                "隅隆隐婚婶婉颇颈绩绪续骑绰绳维绵绷绸综绽绿缀巢琴琳琢琼斑替揍款堪塔搭" +
                "堰揩越趁趋超揽堤提博揭喜彭揣插揪搜煮援搀裁搁搓搂搅壹握搔揉斯期欺联葫" +
                "散惹葬募葛董葡敬葱蒋蒂落韩朝辜葵棒棱棋椰植森焚椅椒棵棍椎棉";

        public static string GenerateRandomChineseName()
        {
            // 随机生成名字长度，2到5个字
            int nameLength = Random.Shared.Next(2, 6);
            StringBuilder name = new();

            for (int i = 0; i < nameLength; i++)
            {
                // 从常用汉字集中随机选择一个汉字
                char chineseCharacter = CommonChineseCharacters[Random.Shared.Next(CommonChineseCharacters.Length)];
                name.Append(chineseCharacter);
            }

            return name.ToString();
        }

        public static string GenerateRandomChineseUserName()
        {
            StringBuilder name = new();

            // 随机姓
            string lastname = CommonSurnames[Random.Shared.Next(CommonSurnames.Length)];
            name.Append(lastname);

            // 随机生成名字长度，2到5个字
            int nameLength = Random.Shared.Next(1, 2);

            for (int i = 0; i < nameLength; i++)
            {
                // 从常用汉字集中随机选择一个汉字
                char chineseCharacter = CommonChineseCharacters[Random.Shared.Next(CommonChineseCharacters.Length)];
                name.Append(chineseCharacter);
            }

            return name.ToString();
        }
    }
}
