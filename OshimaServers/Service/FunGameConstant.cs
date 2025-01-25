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
                "司", "钟", "方", "谢"
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
    }
}
