﻿using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Regions
{
    public class 瑟兰薇歌林海 : OshimaRegion
    {
        public 瑟兰薇歌林海()
        {
            Id = 2;
            Name = "瑟兰薇歌林海";
            Description = "树木枝干中流淌荧蓝汁液的广袤林海，深处沉睡着被精灵封印的「旋律古龙」。脉轮圣树屹立在林海中央，其十公里直径的树干上构筑着立体城市，树液能凝结可编程蜜蜡。";
            Category = "生态";
            Weathers.Add("多云", 15);
            Weathers.Add("晴朗", 24);
            ChangeRandomWeather();
            Difficulty = RarityType.FourStar;
            Characters.Add(new(10201, "旋律古龙"));
            Characters.Add(new(10202, "圣树守护者"));
            Units.Add(new(20201, "荧光精灵"));
            Units.Add(new(20202, "蜜蜡蜂"));
            Crops.Add(new(180201, "荧蓝汁液", "锻造物品的材料。", "林海特有树木的汁液，拥有荧蓝色的光泽。能够吸收和储存能量，是珍贵的炼金材料。"));
            Crops.Add(new(180202, "可编程蜜蜡", "锻造物品的材料。", "脉轮圣树分泌的树脂，拥有记忆塑性能力，能够编程实现特定功能。"));
            NPCs.Add("老祭司苔藓须");
            NPCs.Add("封喉歌者");
            Areas.Add("古龙沉眠地穴");
            Areas.Add("立体蜂巢市集");
            ContinuousQuestList.Add("林海深处的记忆协奏曲", new("在瑟兰薇歌林海水晶化的树冠建立观测站，记录春季地貌重组时树木根系发出的低频共振波，破译其与旋律古龙苏醒周期的关联。"));
            ContinuousQuestList.Add("圣树蜜蜡的数据加密", new("在瑟兰薇歌林海脉轮圣树区部署安全协议，防止可编程蜜蜡的数据泄露事件再次发生。"));
            ImmediateQuestList.Add("音律囚笼突破作战", new("苏醒的旋律古龙释放出高频震波，将精灵们困在水晶共振牢笼中，需在下次地貌重组前切断声波共鸣节点。"));
            ImmediateQuestList.Add("圣树蜜蜡数据泄漏", new("瑟兰薇歌林海脉轮圣树区的可编程蜜蜡核心数据被盗，需在加密密钥失效前追回数据包。"));
            ProgressiveQuestList.Add("林海汁液采收行动", new("使用抗腐蚀容器收集瑟兰薇歌林海的 {0} 份荧蓝汁液（树木自卫系统激活时汁液会转化为神经毒素）。", item: "荧蓝汁液"));
            ProgressiveQuestList.Add("圣树蜜蜡编程实验", new("在瑟兰薇歌林海的脉轮圣树区进行 {0} 次蜜蜡编程（收集可编程蜜蜡）。", item: "可编程蜜蜡"));
        }
    }

    public class 腐萤沼渊 : OshimaRegion
    {
        public 腐萤沼渊()
        {
            Id = 9;
            Name = "腐萤沼渊";
            Description = "荧光毒气沼泽，中心生长直径三公里的脑状肉瘤「共生母体」，菌类模仿动物叫声诱捕猎物。";
            Category = "生态";
            Weathers.Add("潮湿", 22);
            ChangeRandomWeather();
            Difficulty = RarityType.FourStar;
            Characters.Add(new(10901, "共生母体"));
            Units.Add(new(20901, "沼泽毒虫"));
            Crops.Add(new(180901, "菌类样本", "锻造物品的材料。", "共生母体上生长的奇异菌类，能释放麻痹毒素，具有高度研究价值。"));
            NPCs.Add("伊芙琳");
            NPCs.Add("溃烂猎人");
            Areas.Add("母体神经丛");
            Areas.Add("诱捕菌林");
            ContinuousQuestList.Add("腐萤沼渊的共生调查", new("深入腐萤沼渊，研究共生母体的生态系统，记录菌类模仿动物叫声的频率和模式。"));
            ContinuousQuestList.Add("腐沼菌群的声波分析", new("在腐萤沼渊建立声纹实验室，解析共生母体菌类发出的动物模拟叫声，建立声波防御系统。"));
            ImmediateQuestList.Add("腐沼毒气中和行动", new("腐萤沼渊共生母体排出的毒气云正在快速扩散，需在十分钟内启动应急中和装置保护周边生态。"));
            ProgressiveQuestList.Add("共生母体样本采集", new("从腐萤沼渊的共生母体采集 {0} 份不同的菌类样本（注意菌类会释放麻痹毒素）。", item: "菌类样本"));
        }
    }

    public class 双生月崖 : OshimaRegion
    {
        public 双生月崖()
        {
            Id = 10;
            Name = "双生月崖";
            Description = "撕裂的悬浮山脉，永昼侧栖光鹰，永夜侧布满影玫瑰。山崖之下为天穹碎屿，其星锚之地矗立着束缚星空巨兽的引雷柱。切换昼夜时若未能撤离，将触发空间湮灭。";
            Category = "生态";
            Weathers.Add("永昼", 15);
            Weathers.Add("永夜", -10);
            ChangeRandomWeather();
            Difficulty = RarityType.FourStar;
            Characters.Add(new(11001, "昼夜守护者"));
            Characters.Add(new(11002, "星空巨兽"));
            Units.Add(new(21001, "光鹰", [(r => r.Weather == "永昼")]));
            Units.Add(new(21002, "浮空岛灵", [(r => r.Weather == "永夜")]));
            Crops.Add(new(181001, "影玫瑰", "锻造物品的材料。", "永夜侧绽放的奇异植物，散发幽光，只在绝对黑暗中盛开。", QualityType.White, [(r => r.Weather == "永夜")]));
            Crops.Add(new(181002, "星锚晶石", "锻造物品的材料。", "引雷柱上脱落的晶体碎片，蕴含空间束缚能量，能暂时固定空间结构。", QualityType.White, [(r => r.Weather == "永昼")]));
            NPCs.Add("守夜人卡尔");
            NPCs.Add("星锚祭司");
            Areas.Add("永昼庭园");
            Areas.Add("永夜墓园");
            Areas.Add("星锚之地");
            Areas.Add("中央之岛");
            ContinuousQuestList.Add("星锚之地的束缚警戒", new("前往双生月崖永夜侧的星锚之地，加固引雷柱的能量场，防止星空巨兽挣脱时空束缚。"));
            ContinuousQuestList.Add("双生湮灭的边界探测", new("在双生月崖建立空间稳定站，研究昼夜切换时湮灭边界产生的空间畸变现象。"));
            ImmediateQuestList.Add("星空巨兽狂怒预警", new("双生月崖星锚之地的引雷柱遭到破坏，星空巨兽即将挣脱束缚，必须立即修复能量矩阵。"));
            ImmediateQuestList.Add("湮灭边界扩张警报", new("双生月崖昼夜切换窗口期异常延迟，湮灭边界以每分钟5米速度扩张，必须立即撤离滞留学生团。"));
            ProgressiveQuestList.Add("星锚晶石加固任务", new("在双生月崖的星锚之地校准 {0} 个不同的引雷柱，收集星锚晶石。", item: "星锚晶石"));
            ProgressiveQuestList.Add("湮灭边界观测记录", new("在双生月崖收集 {0} 份影玫瑰。", item: "影玫瑰"));
        }
    }

    public class 棱镜骨桥 : OshimaRegion
    {
        public 棱镜骨桥()
        {
            Id = 11;
            Name = "棱镜骨桥";
            Description = "由巨型骸骨构筑的呼吸桥梁，晶体化红杉沿桥生长储存亡者记忆。幽灵船在桥底虚空航行，影狼嚎叫产生空间褶皱。";
            Category = "生态";
            Weathers.Add("雾气", 14);
            Weathers.Add("阴森", 8);
            ChangeRandomWeather();
            Difficulty = RarityType.FiveStar;
            Characters.Add(new(11101, "远古影狼"));
            Characters.Add(new(11102, "骸骨巨龙"));
            Units.Add(new(21101, "棱镜幽灵"));
            Units.Add(new(21102, "幽灵水手"));
            Crops.Add(new(181101, "晶化记忆孢子", "锻造物品的材料。", "红杉散发的记忆载体，吸入后可能体验亡者临终经历。"));
            Crops.Add(new(181102, "虚空骨髓", "锻造物品的材料。", "骨骼深处的虚空物质，散发扭曲精神的空间能量。"));
            Crops.Add(new(181103, "神经蕨类", "锻造物品的材料。", "寄生在骸骨上的蕨类植物，根系连接骸骨神经，触碰引发剧烈幻觉。"));
            Crops.Add(new(181104, "龙骨碎片", "锻造物品的材料。", "幽灵船的龙骨半埋于桥底虚空泥沼中，其扭曲的脊椎骨刺穿雾层，船体残骸散发幽蓝磷光。"));
            NPCs.Add("骸骨诗人");
            NPCs.Add("\"船长\"");
            Areas.Add("骸鲸观星台");
            Areas.Add("幽灵船坞");
            ContinuousQuestList.Add("骨桥记忆的精神共鸣", new("进入棱镜骨桥，收集晶体化红杉中储存的亡者记忆，调查空间褶皱对现实结构的影响。"));
            ImmediateQuestList.Add("骨桥幽灵舰队突袭", new("棱镜骨桥下方的幽灵船队突破虚空屏障，对现实维度发动突袭，必须在半小时内建立反制防线。"));
            ImmediateQuestList.Add("棱镜记忆过载危机", new("棱镜骨桥的红杉晶体因记忆过载即将爆发共感冲击波，必须在五分钟内疏散周边观测人员。"));
            ProgressiveQuestList.Add("骨桥记忆共鸣实验", new("在棱镜骨桥提取 {0} 份不同的亡者记忆（晶化记忆孢子）。", item: "晶化记忆孢子"));
            ProgressiveQuestList.Add("幽灵航线能量测绘", new("在棱镜骨桥下方收集 {0} 份幽灵船能量数据（龙骨碎片）。", item: "龙骨碎片"));
        }
    }
}
