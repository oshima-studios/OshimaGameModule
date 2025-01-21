using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;
using Oshima.FunGame.OshimaModules.Items;

namespace Oshima.FunGame.OshimaServers.Service
{
    public class FunGameConstant
    {
        public const long CustomCharacterId = -1;
        public static List<Character> Characters { get; } = [];
        public static List<Skill> Skills { get; } = [];
        public static List<Skill> PassiveSkills { get; } = [];
        public static List<Skill> SuperSkills { get; } = [];
        public static List<Skill> Magics { get; } = [];
        public static List<Item> Equipment { get; } = [];
        public static List<Item> Items { get; } = [];
        public static List<Skill> ItemSkills { get; } = [];
        public static List<Item> AllItems { get; } = [];
        public static List<Skill> AllSkills { get; } = [];
        public static Dictionary<long, User> UserIdAndUsername { get; } = [];
        public static ItemType[] ItemCanUsed => [ItemType.Consumable, ItemType.MagicCard, ItemType.SpecialItem, ItemType.GiftBox, ItemType.Others];

        public static Dictionary<int, Dictionary<string, int>> LevelBreakNeedyList
        {
            get
            {
                return new()
                {
                    {
                        0, new()
                        {
                            { General.GameplayEquilibriumConstant.InGameMaterial, 20 },
                            { nameof(升华之印), 2 }
                        }
                    },
                    {
                        1, new()
                        {
                            { General.GameplayEquilibriumConstant.InGameMaterial, 40 },
                            { nameof(升华之印), 4 }
                        }
                    },
                    {
                        2, new()
                        {
                            { General.GameplayEquilibriumConstant.InGameMaterial, 80 },
                            { nameof(升华之印), 6 },
                            { nameof(流光之印), 2 }
                        }
                    },
                    {
                        3, new()
                        {
                            { General.GameplayEquilibriumConstant.InGameMaterial, 160 },
                            { nameof(升华之印), 9 },
                            { nameof(流光之印), 4 }
                        }
                    },
                    {
                        4, new()
                        {
                            { General.GameplayEquilibriumConstant.InGameMaterial, 320 },
                            { nameof(升华之印), 12 },
                            { nameof(流光之印), 6 },
                            { nameof(永恒之印), 2 }
                        }
                    },
                    {
                        5, new()
                        {
                            { General.GameplayEquilibriumConstant.InGameMaterial, 640 },
                            { nameof(升华之印), 16 },
                            { nameof(流光之印), 9 },
                            { nameof(永恒之印), 4 }
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
                            { nameof(技能卷轴), 4 },
                            { nameof(智慧之果), 1 },
                        }
                    },
                    {
                        4, new()
                        {
                            { "角色等级", 36 },
                            { General.GameplayEquilibriumConstant.InGameCurrency, 18000 },
                            { General.GameplayEquilibriumConstant.InGameMaterial, 100 },
                            { nameof(技能卷轴), 6 },
                            { nameof(智慧之果), 2 },
                        }
                    },
                    {
                        5, new()
                        {
                            { "角色等级", 48 },
                            { General.GameplayEquilibriumConstant.InGameCurrency, 30000 },
                            { General.GameplayEquilibriumConstant.InGameMaterial, 150 },
                            { nameof(技能卷轴), 9 },
                            { nameof(智慧之果), 4 },
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
                            { nameof(智慧之果), 6 },
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
                            { nameof(技能卷轴), 4 },
                            { nameof(智慧之果), 1 },
                        }
                    },
                    {
                        5, new()
                        {
                            { "角色等级", 32 },
                            { General.GameplayEquilibriumConstant.InGameCurrency, 18000 },
                            { General.GameplayEquilibriumConstant.InGameMaterial, 100 },
                            { nameof(技能卷轴), 6 },
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
                            { nameof(技能卷轴), 9 },
                            { nameof(智慧之果), 4 },
                            { nameof(奥术符文), 1 }
                        }
                    },
                    {
                        7, new()
                        {
                            { "角色等级", 48 },
                            { General.GameplayEquilibriumConstant.InGameCurrency, 47000 },
                            { General.GameplayEquilibriumConstant.InGameMaterial, 210 },
                            { nameof(技能卷轴), 12 },
                            { nameof(智慧之果), 6 },
                            { nameof(奥术符文), 2 }
                        }
                    },
                    {
                        8, new()
                        {
                            { "角色等级", 56 },
                            { General.GameplayEquilibriumConstant.InGameCurrency, 70000 },
                            { General.GameplayEquilibriumConstant.InGameMaterial, 280 },
                            { nameof(技能卷轴), 16 },
                            { nameof(智慧之果), 9 },
                            { nameof(奥术符文), 4 },
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

        public static Dictionary<QualityType, double> DrawCardProbabilities
        {
            get
            {
                return new()
                {
                    { QualityType.White, 69.53 },
                    { QualityType.Green, 15.35 },
                    { QualityType.Blue, 9.48 },
                    { QualityType.Purple, 4.25 },
                    { QualityType.Orange, 1.33 },
                    { QualityType.Red, 0.06 }
                };
            }
        }

        public static Dictionary<QualityType, (int Min, int Max)> PriceRanges
        {
            get
            {
                return new()
                {
                    { QualityType.White, (200, 2000) },
                    { QualityType.Green, (1500, 15000) },
                    { QualityType.Blue, (5000, 50000) },
                    { QualityType.Purple, (10000, 100000) },
                    { QualityType.Orange, (40000, 400000) },
                    { QualityType.Red, (100000, 1000000) },
                    { QualityType.Gold, (500000, 5000000) }
                };
            }
        }
    }
}
