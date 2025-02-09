using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Regions
{
    public class 瑟兰薇歌林海 : OshimaRegion
    {
        public 瑟兰薇歌林海()
        {
            Id = 2;
            Name = "瑟兰薇歌林海";
            Description = "树木枝干中流淌荧蓝汁液，春季行走重组地貌，冬季化为水晶雕塑。深处沉睡着被精灵封印的「旋律古龙」";
            Category = "生态";
            Weathers.Add("多云", 15);
            ChangeRandomWeather();
            Difficulty = RarityType.FourStar;
            Characters.Add(new(10201, "旋律古龙"));
            Units.Add(new(20201, "荧光精灵"));
            Crops.Add(new(180201, "荧蓝汁液", "锻造物品的材料。", "瑟兰薇歌林海特有树木的汁液，呈现美丽的荧蓝色。采集时需要小心，因为树木在受到威胁时会将汁液转化为神经毒素。"));
        }
    }

    public class 腐萤沼渊 : OshimaRegion
    {
        public 腐萤沼渊()
        {
            Id = 8;
            Name = "腐萤沼渊";
            Description = "荧光毒气沼泽，中心生长直径三公里的脑状肉瘤「共生母体」，菌类模仿动物叫声诱捕猎物";
            Category = "生态";
            Weathers.Add("潮湿", 22);
            ChangeRandomWeather();
            Difficulty = RarityType.OneStar;
            Characters.Add(new(10801, "共生母体"));
            Units.Add(new(20801, "沼泽毒虫"));
            Crops.Add(new(180801, "菌类样本", "锻造物品的材料。", "腐萤沼渊共生母体上生长的奇异菌类，拥有着不同的颜色和形态。采集时需要小心，因为某些菌类会释放麻痹毒素。"));
        }
    }

    public class 永燃坩埚 : OshimaRegion
    {
        public 永燃坩埚()
        {
            Id = 14;
            Name = "永燃坩埚";
            Description = "岩浆海上的球形锻造都市，岩浆鱿鱼游弋街道，火山灰培育活体金属苔藓";
            Category = "生态";
            Weathers.Add("高温", 60);
            ChangeRandomWeather();
            Difficulty = RarityType.FiveStar;
            Characters.Add(new(11401, "岩浆之王"));
            Units.Add(new(21401, "岩浆鱿鱼"));
            Crops.Add(new(181401, "活体金属苔藓", "锻造物品的材料。", "永燃坩埚特有的金属质感的苔藓，能够在火山灰中生长。它拥有着自我修复和繁殖的能力，是研究金属生命的重要材料。"));
        }
    }

    public class 骨桥深渊 : OshimaRegion
    {
        public 骨桥深渊()
        {
            Id = 15;
            Name = "骨桥深渊";
            Description = "巨型骸骨形成的呼吸桥梁，幽灵船在桥底虚空航行，骸骨寄生神经蕨类";
            Category = "生态";
            Weathers.Add("阴森", 8);
            ChangeRandomWeather();
            Difficulty = RarityType.ThreeStar;
            Characters.Add(new(11501, "骸骨巨龙"));
            Units.Add(new(21501, "幽灵"));
            Crops.Add(new(181501, "虚空骨髓", "锻造物品的材料。", "骨桥深渊中巨型骸骨内部的特殊物质，散发着微弱的虚空能量，长期接触可能导致精神错乱。"));
            Crops.Add(new(181502, "神经蕨类", "锻造物品的材料。", "骨桥深渊中寄生在骸骨上的奇异蕨类，其根系与骸骨的神经系统相连，触碰时会引发幻觉和精神冲击。"));
        }
    }

    public class 脉轮圣树 : OshimaRegion
    {
        public 脉轮圣树()
        {
            Id = 17;
            Name = "脉轮圣树";
            Description = "树干直径十公里的螺旋巨树，年轮是立体城市，树液凝结可编程蜜蜡";
            Category = "生态";
            Weathers.Add("晴朗", 24);
            ChangeRandomWeather();
            Difficulty = RarityType.ThreeStar;
            Characters.Add(new(11701, "圣树守护者"));
            Units.Add(new(21701, "蜜蜡蜂"));
            Crops.Add(new(181701, "可编程蜜蜡", "锻造物品的材料。", "脉轮圣树分泌的树脂凝结而成的蜡状物质，拥有着独特的纹路和光泽。据说它能够被编程，用于创造各种奇妙的物品。"));
        }
    }

    public class 双生月崖 : OshimaRegion
    {
        public 双生月崖()
        {
            Id = 19;
            Name = "双生月崖";
            Description = "撕裂的悬浮山脉，永昼侧栖光鹰，永夜侧绽影玫瑰。此地区不定期切换昼夜，在切换窗口期内可安全离开此地，否则，视为跨越界限触发湮灭";
            Category = "生态";
            Weathers.Add("永昼", 15);
            Weathers.Add("永夜", -10);
            ChangeRandomWeather();
            Difficulty = RarityType.FiveStar;
            Characters.Add(new(11901, "昼夜守护者"));
            Units.Add(new(21901, "光鹰", [(r => r.Weather == "永昼")]));
            Crops.Add(new(181901, "影玫瑰", "锻造物品的材料。", "双生月崖永夜侧绽放的奇异玫瑰，散发着幽暗的光芒。", QualityType.White, [(r => r.Weather == "永夜")]));
        }
    }

    public class 回音棱镜林 : OshimaRegion
    {
        public 回音棱镜林()
        {
            Id = 13;
            Name = "回音棱镜林";
            Description = "晶体化红杉储存亡者记忆，荧光孢子引发共感，影狼嚎叫产生空间褶皱";
            Category = "生态";
            Weathers.Add("雾气", 14);
            ChangeRandomWeather();
            Difficulty = RarityType.FourStar;
            Characters.Add(new(11301, "远古影狼"));
            Units.Add(new(21301, "棱镜幽灵"));
            Crops.Add(new(181301, "晶化记忆孢子", "锻造物品的材料。", "回音棱镜林晶体化红杉散发的孢子，蕴含着亡者的记忆，吸入可能引发共感。"));
        }
    }
}
