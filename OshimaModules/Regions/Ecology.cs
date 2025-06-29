using Milimoe.FunGame.Core.Library.Constant;

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
            Areas.Add("母体神经丛​​");
            Areas.Add("诱捕菌林");
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
            NPCs.Add("星锚祭司​​");
            Areas.Add("永昼庭园​​");
            Areas.Add("永夜墓园");
            Areas.Add("星锚之地");
            Areas.Add("中央之岛");
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
            NPCs.Add("骸骨诗人");
            NPCs.Add("\"船长\"");
            Areas.Add("骸鲸观星台");
            Areas.Add("幽灵船坞");
        }
    }
}
