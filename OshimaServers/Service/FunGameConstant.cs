using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;
using Oshima.FunGame.OshimaModules.Items;
using Oshima.FunGame.OshimaModules.Regions;

namespace Oshima.FunGame.OshimaServers.Service
{
    public class FunGameConstant
    {
        public const long CustomCharacterId = -1;
        public const int ItemsPerPage1 = 6;
        public const int ItemsPerPage2 = 10;
        public const int ExploreTime = 2;
        public static List<Character> Characters { get; } = [];
        public static List<Skill> Skills { get; } = [];
        public static List<Skill> PassiveSkills { get; } = [];
        public static List<Skill> SuperSkills { get; } = [];
        public static List<Skill> Magics { get; } = [];
        public static List<Item> Equipment { get; } = [];
        public static List<Item> Items { get; } = [];
        public static List<Item> DrawCardItems { get; } = [];
        public static Dictionary<OshimaRegion, List<Item>> ExploreItems { get; } = [];
        public static List<Skill> ItemSkills { get; } = [];
        public static List<Item> AllItems { get; } = [];
        public static List<Skill> AllSkills { get; } = [];
        public static Dictionary<long, User> UserIdAndUsername { get; } = [];
        public static ItemType[] ItemCanUsed => [ItemType.Consumable, ItemType.MagicCard, ItemType.SpecialItem, ItemType.GiftBox, ItemType.Others];
        public static ItemType[] ItemCanNotDrawCard => [ItemType.Collectible, ItemType.QuestItem, ItemType.GiftBox, ItemType.Others];

        public static Dictionary<int, Dictionary<string, int>> LevelBreakNeedyList { get; } = new()
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

        public static Dictionary<int, Dictionary<string, int>> SkillLevelUpList { get; } = new()
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

        public static Dictionary<int, Dictionary<string, int>> NormalAttackLevelUpList { get; } = new()
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

        public static Dictionary<string, string> ContinuousQuestList { get; } = new()
        {
            {
                "悖论引擎的暗涌之息",
                "穿梭于银辉城流淌液态月光的街道，侦测悖论引擎释放的异常能量潮汐，揭开可能撕裂现实结构的危险谜团。"
            },
            {
                "林海深处的记忆协奏曲",
                "在瑟兰薇歌林海水晶化的树冠建立观测站，记录春季地貌重组时树木根系发出的低频共振波，破译其与旋律古龙苏醒周期的关联。"
            },
            {
                "元素裂隙的熵增警告",
                "使用抗魔探针扫描赫菲斯托斯之喉第47层矿道，绘制元素裂缝的扩张轨迹，评估其引发位面坍缩的风险等级。"
            },
            {
                "时间琥珀中的战争回响",
                "在永霜裂痕建立时滞力场实验室，分析冻结在冰壁中的古代战争幻象，还原时霜药剂对观察者认知体系的扭曲机制。"
            },
            {
                "镜像维度的认知污染",
                "佩戴反重力拘束装置潜入千瞳镜湖，测绘镜像城的拓扑结构，警惕瞳孔状传送门对记忆模块的逆向写入现象。"
            },
            {
                "雷霆王座的符文解读",
                "攀登雷霆王座山脉，记录裁决尖碑在月圆之夜投射的泰坦符文，破译其蕴含的宇宙法则。"
            },
            {
                "流沙时计的昨日寻踪",
                "在流沙时计荒漠中寻找海市蜃楼般的昨日之城，收集散落在其中的时间碎片，还原历史真相。"
            },
            {
               "腐萤沼渊的共生调查",
               "深入腐萤沼渊，研究共生母体的生态系统，记录菌类模仿动物叫声的频率和模式。"
            },
            {
                "苍穹碎屿的星锚校准",
                "前往苍穹碎屿的星锚之地，校准引雷柱的能量输出，防止星空巨兽挣脱束缚。"
            },
            {
                "齿与血回廊的改造逆转",
                "潜入齿与血回廊的造物车间，研究其改造机制，寻找逆转改造的方法。"
            },
            {
                "穹顶之泪的星辉观测",
                "在穹顶之泪湖畔建立观测点，记录星辉水母群在午夜重构水体重力法则时的变化。"
            },
            {
                "齿轮坟场的构装残骸",
                "在齿轮坟场搜寻构装巨龙的残骸，分析其动力核心，寻找上古机械文明的线索。"
            },
            {
                "回音棱镜林的记忆共鸣",
                "进入回音棱镜林，收集晶体化红杉中储存的亡者记忆，调查影狼嚎叫产生的空间褶皱。"
            },
            {
                "永燃坩埚的活体金属",
                "在永燃坩埚的火山灰中采集活体金属苔藓样本，研究其生长特性。"
            },
            {
                "骨桥深渊的幽灵航线",
                "调查骨桥深渊的幽灵船航线，收集桥底虚空中的能量波动数据。"
            },
            {
                "时漏沙漠的时凝液",
                "在时漏沙漠中寻找沙漏仙人掌，采集其分泌的时凝液，研究其时间魔法特性。"
            },
            {
                "脉轮圣树的蜜蜡编码",
                "攀登脉轮圣树，收集树液凝结的可编程蜜蜡，分析其编码模式。"
            },
            {
               "悲鸣矿脉的神经宝石",
               "在悲鸣矿脉中采集神经宝石样本，研究其与山体剧痛的关联。"
            },
            {
               "双生月崖的湮灭边界",
               "在双生月崖的边界处进行实验，研究跨越永昼和永夜界限时产生的湮灭现象。"
            },
            {
               "谵妄海市的梦境碎片",
               "进入谵妄海市，收集可食用梦境碎片，分析其对认知的影响。"
            }
        };

        public static Dictionary<string, string> ImmediateQuestList { get; } = new()
        {
            {
                "星银警戒协议·弎级响应",
                "星银合金守卫在悖论引擎周边暴走，形成包围核心区的杀戮矩阵，必须在三刻钟内解除警戒协议。"
            },
            {
                "音律囚笼突破作战",
                "苏醒的旋律古龙释放出高频震波，将精灵们困在水晶共振牢笼中，需在下次地貌重组前切断声波共鸣节点。"
            },
            {
                "深渊火种收容危机",
                "矿道底层的深渊火钻因元素污染进入链式裂变，引发全矿道魔能过载，立即部署熵减力场遏制反应。"
            },
            {
                "时霜逆流救援行动",
                "科研小组被困在加速百倍的时间泡内，其肉体正以肉眼可见的速度衰老，必须校准哨塔时钟恢复时间流速。"
            },
            {
                "镜像侵蚀净化指令",
                "镜像守卫突破湖面屏障入侵现实维度，携带的认知病毒正在改写物理法则，启动银辉城防卫协议实施净化。"
            },
            {
                "雷霆风暴紧急预警",
                "雷霆王座山脉的悬浮岩块因磁场紊乱开始崩塌，必须在半小时内稳定磁场。"
            },
            {
                "流沙陷阱救援行动",
                "一支探险队被困在流沙时计荒漠的昨日之城中，必须在沙暴来临前将其救出。"
            },
            {
                "沼泽毒气泄漏警报",
                "腐萤沼渊的毒气浓度超标，必须立即启动通风系统，防止毒气扩散。"
            },
            {
                "星空巨兽挣脱危机",
                "苍穹碎屿的星锚之地出现裂缝，星空巨兽即将挣脱束缚，必须立即加固引雷柱。"
            },
            {
                "活体建筑暴走事件",
                "齿与血回廊的活体建筑开始失控，必须立即关闭其动力系统。"
            },
            {
                "水母重力失控事件",
                "穹顶之泪湖的星辉水母群重力法则失控，必须立即稳定水体。"
            },
            {
                 "构装巨龙苏醒警报",
                 "齿轮坟场的构装巨龙开始苏醒，必须立即启动防御系统。"
            },
            {
                "记忆共鸣失控事件",
                "回音棱镜林的记忆共鸣失控，引发空间褶皱，必须立即稳定空间。"
            },
            {
                "活体金属苔藓异变",
                "永燃坩埚的活体金属苔藓发生异变，开始吞噬周围的金属，必须立即遏制。"
            },
            {
               "幽灵船袭击事件",
               "骨桥深渊的幽灵船开始袭击过往的船只，必须立即击退。"
            },
            {
                "时之蝎暴走事件",
                "时漏沙漠的时之蝎因时凝液泄漏而暴走，必须立即控制。"
            },
            {
                "蜜蜡编码泄露危机",
                "脉轮圣树的可编程蜜蜡编码泄露，引发未知危机，必须立即阻止。"
            },
            {
                "神经宝石共鸣危机",
                "悲鸣矿脉的神经宝石发生共鸣，引发山体震荡，必须立即稳定。"
            },
            {
                "湮灭风暴预警",
                "双生月崖的湮灭边界出现不稳定，引发湮灭风暴，必须立即撤离。"
            },
            {
               "思维寄生虫感染事件",
               "谵妄海市的思维寄生虫开始感染居民，必须立即隔离。"
            }
        };

        public static Dictionary<string, string> ProgressiveQuestList { get; } = new()
        {
            {
                "月光萃取计划",
                "前往【银辉城】，在星银合金建筑的沟壑中采集 {0} 份液态月光（注意避开月光洪流的高潮时段/每夜丑时三刻）。"
            },
            {
                "灵脉汁液采收行动",
                "使用抗腐蚀容器收集 {0} 份【瑟兰薇歌林海】的荧蓝汁液（树木自卫系统激活时汁液会转化为神经毒素）。"
            },
            {
                "火钻精炼协议",
                "在【赫菲斯托斯之喉】矿工灵魂烙印的指引下获取 {0} 颗深渊火钻（未烙印者触碰火钻将引发元素爆燃）。"
            },
            {
                "时霜逆向工程",
                "通过时间镜像收集 {0} 份来自【永霜裂痕】不同历史断片的时霜药剂样本（注意时空回响对记忆的覆盖效应）。"
            },
            {
                "瞳孔密钥重构计划",
                "前往【千瞳镜湖】，从 {0} 个瞳孔状传送门提取量子纠缠碎片（每个采集点需保持镜像对称操作以避免维度塌缩）。"
            },
            {
                "泰坦符文拓印",
                "在【雷霆王座山脉】的裁决尖碑上拓印 {0} 份不同的泰坦符文（注意避开雷暴时段/每逢子时）。"
            },
            {
                "时间碎片收集",
                "在【流沙时计荒漠】的昨日之城中收集 {0} 份不同的时间碎片（注意时间碎片会随机重组）。"
            },
            {
                "共生母体样本采集",
                "从【腐萤沼渊】的共生母体上采集 {0} 份不同的菌类样本（注意菌类会释放麻痹毒素）。"
            },
            {
                "星锚能量校准",
                "在【苍穹碎屿】的星锚之地校准 {0} 个不同的引雷柱（注意引雷柱会释放高压电流）。"
            },
            {
                "改造逆转实验",
                "在【齿与血回廊】的造物车间进行 {0} 次不同的改造逆转实验（注意改造实验会引发身体异变）。"
            },
            {
                "星辉水母观测记录",
                "在【穹顶之泪湖】记录 {0} 次星辉水母重构水体重力法则的完整过程（注意水母重构时会产生重力波动）。"
            },
            {
                "构装巨龙残骸分析",
                "在【齿轮坟场】分析 {0} 个不同的构装巨龙残骸（注意残骸可能带有自毁装置）。"
            },
            {
                "亡者记忆提取",
                "在【回音棱镜林】提取 {0} 份不同的亡者记忆（注意记忆提取会引发共感）。"
            },
            {
                "活体金属苔藓培养",
                "在【永燃坩埚】培养 {0} 份不同的活体金属苔藓样本（注意苔藓会吸收金属）。"
            },
            {
                "幽灵船能量分析",
               "在【骨桥深渊】收集 {0} 份幽灵船的能量波动数据（注意幽灵船会释放虚空能量）。"
            },
            {
                "时凝液提纯",
                "在【时漏沙漠】提纯 {0} 份时凝液（注意时凝液会加速时间流速）。"
            },
            {
                "蜜蜡编码破解",
                "在【脉轮圣树】破解 {0} 份不同的蜜蜡编码（注意编码会引发精神干扰）。"
            },
            {
                "神经宝石能量分析",
                "在【悲鸣矿脉】分析 {0} 份不同的神经宝石能量（注意宝石会引发山体剧痛）。"
            },
            {
                "湮灭边界观察",
                "在【双生月崖】观察 {0} 次不同的湮灭边界现象（注意湮灭边界会吞噬物质）。"
            },
            {
                "梦境碎片分析",
                "在【谵妄海市】分析 {0} 份不同的梦境碎片（注意梦境碎片会引发幻觉）。"
            }
        };

        public static List<OshimaRegion> Regions { get; } = [
            new 银辉城(), new 瑟兰薇歌林海(), new 赫菲斯托斯之喉(), new 永霜裂痕(), new 千瞳镜湖(), new 雷霆王座山脉(), new 流沙时计荒漠(), new 腐萤沼渊(),
            new 苍穹碎屿(), new 齿与血回廊(), new 穹顶之泪湖(), new 齿轮坟场(), new 回音棱镜林(), new 永燃坩埚(), new 骨桥深渊(), new 时漏沙漠(),
            new 脉轮圣树(), new 悲鸣矿脉(), new 双生月崖(), new 谵妄海市()
        ];

        /// <summary>
        /// 参数说明：{0} 奖励内容字符串，{1} 出现的敌人名称，{2} 出现的NPC名称，{3} 出现的物品名称
        /// </summary>
        public static Dictionary<string, ExploreResult> ExploreString { get; } = new()
        {
            { "哎呀，这波啊，这波是探了个寂寞！不过…好像也不是完全没有收获？奖励：{0}！", ExploreResult.General },
            { "你以为会一无所获？哼，天真！虽然也没啥大用，但至少…获得了：{0}！", ExploreResult.General },
            { "恭喜你！成功在荒野中迷路！奖励…等等，好像是：{0}？算了，凑合着用吧！", ExploreResult.General },
            { "你凝视着远方…远方也凝视着你…然后，你获得了：{ 0}！这大概就是命运吧。", ExploreResult.General },
            { "探索结果：空气，阳光，还有…奖励：{ 0}！看来今天运气还不错？", ExploreResult.General },

            { "啥也没找到，白跑一趟！下次记得带上指南针！", ExploreResult.Nothing },
            { "空空如也，一无所获。看来这地方已经被搜刮干净了！", ExploreResult.Nothing },
            { "你对着空地发呆了半天，然后决定回家。今天就当无事发生。", ExploreResult.Nothing },
            { "探索失败！你被自己的影子吓了一跳，然后落荒而逃。", ExploreResult.Nothing },
            { "你努力寻找着什么，但最终只找到了自己的寂寞。", ExploreResult.Nothing },

            { "前方高能！遭遇了{ 1}！准备好迎接一场史诗般的…菜鸡互啄！", ExploreResult.Fight },
            { "警告！{ 1} 正在接近！是时候展现真正的技术了…逃跑技术！", ExploreResult.Fight },
            { "战斗警报！{ 1} 想要和你一较高下！拿出你的勇气…或者直接认输吧！", ExploreResult.Fight },
            { "不好了！{ 1} 出现了！快使用你的绝招…装死！", ExploreResult.Fight },
            { "危险！{ 1} 来袭！但是…你好像忘了带武器？", ExploreResult.Fight },

            { "发财了！竟然捡到了{ 0}！看来今天出门踩到狗屎了！", ExploreResult.Earned },
            { "哇！{ 0}！这一定是上天赐予我的！感谢老天爷！", ExploreResult.Earned },
            { "你简直不敢相信自己的眼睛！{ 0}！这运气也太好了吧！", ExploreResult.Earned },
            { "天降横财！{ 0}！看来以后要多出门走走了！", ExploreResult.Earned },
            { "惊喜！{ 0}！这一定是隐藏的宝藏！", ExploreResult.Earned }
        };

        public static Dictionary<QualityType, double> DrawCardProbabilities { get; } = new()
        {
            { QualityType.White, 69.53 },
            { QualityType.Green, 15.35 },
            { QualityType.Blue, 9.48 },
            { QualityType.Purple, 4.25 },
            { QualityType.Orange, 1.33 },
            { QualityType.Red, 0.06 }
        };

        public static Dictionary<QualityType, (int Min, int Max)> PriceRanges { get; } = new()
        {
            { QualityType.White, (200, 2000) },
            { QualityType.Green, (1500, 15000) },
            { QualityType.Blue, (5000, 50000) },
            { QualityType.Purple, (10000, 100000) },
            { QualityType.Orange, (40000, 400000) },
            { QualityType.Red, (100000, 1000000) },
            { QualityType.Gold, (500000, 5000000) }
        };

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
    }

    public enum ExploreResult
    {
        General,
        Nothing,
        Fight,
        Earned,
        Event
    }
}
