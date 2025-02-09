using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Regions
{
    public class 银辉城 : OshimaRegion
    {
        public 银辉城()
        {
            Id = 1;
            Name = "银辉城";
            Description = "悬浮在云海中的倒三角金属都市，建筑由星银合金铸造，街道流淌着液态月光。核心区藏有能改写现实法则的「悖论引擎」";
            Category = "奇异";
            Weathers.Add("晴朗", 20);
            ChangeRandomWeather();
            Difficulty = RarityType.TwoStar;
            Characters.Add(new(10101, "失控的悖论引擎"));
            Units.Add(new(20101, "星银守卫"));
            Crops.Add(new(180101, "星银合金", "锻造物品的材料。", "银辉城特有的金属材料，拥有着银色的光泽和坚固的质地。据说它能够吸收和储存能量，是建造城市和制造武器的理想材料。"));
            Crops.Add(new(180102, "液态月光", "锻造物品的材料。", "一种在银辉城特有的、散发着柔和光芒的液体，如同月光般清澈。据说它蕴含着悖论引擎的能量，能够影响现实的结构。"));
        }
    }

    public class 永霜裂痕 : OshimaRegion
    {
        public 永霜裂痕()
        {
            Id = 4;
            Name = "永霜裂痕";
            Description = "冰晶峡谷冻结着不同时代的战争残影，哨塔时钟随机倒转/加速，需服用「时霜药剂」保持神智";
            Category = "奇异";
            Weathers.Add("极寒", -25);
            ChangeRandomWeather();
            Difficulty = RarityType.FiveStar;
            Characters.Add(new(10401, "时空扭曲者"));
            Units.Add(new(20401, "冰霜傀儡"));
            Crops.Add(new(180401, "时霜药剂", "锻造物品的材料。", "一种在永霜裂痕中使用的特殊药剂，能够减缓时间流逝，保持人的神智清醒。但长期服用可能导致记忆混乱和时间感知错乱。"));
            Crops.Add(new(180402, "冰封记忆", "锻造物品的材料。", "永霜裂痕中冰封的古代战争幻象碎片，触碰时会引发强烈的记忆回溯，但同时也伴随着认知扭曲的风险。"));
        }
    }

    public class 千瞳镜湖 : OshimaRegion
    {
        public 千瞳镜湖()
        {
            Id = 5;
            Name = "千瞳镜湖";
            Description = "湖面倒影展现平行时空，潜入会进入重力颠倒的镜像城，湖底布满瞳孔状传送门";
            Category = "奇异";
            Weathers.Add("阴沉", 10);
            ChangeRandomWeather();
            Difficulty = RarityType.TwoStar;
            Characters.Add(new(10501, "镜像之主"));
            Units.Add(new(20501, "镜像守卫"));
            Crops.Add(new(180501, "量子纠缠碎片", "锻造物品的材料。", "千瞳镜湖的瞳孔状传送门中提取的微小碎片，拥有着神秘的能量。据说它们与平行时空相连，能够引发量子纠缠现象。"));
        }
    }

    public class 流沙时计荒漠 : OshimaRegion
    {
        public 流沙时计荒漠()
        {
            Id = 7;
            Name = "流沙时计荒漠";
            Description = "沙粒蕴含时间魔法，沙丘每小时重组地形，沙暴中会出现海市蜃楼般的「昨日之城」";
            Category = "奇异";
            Weathers.Add("沙尘暴", 35);
            ChangeRandomWeather();
            Difficulty = RarityType.ThreeStar;
            Characters.Add(new(10701, "时间吞噬者"));
            Units.Add(new(20701, "流沙蝎"));
            Crops.Add(new(180701, "时间碎片", "锻造物品的材料。", "流沙时计荒漠中散落的神秘碎片，拥有着不规则的形状和模糊的纹路。据说它们是过去时光的残余，会随机重组。"));
        }
    }

    public class 穹顶之泪湖 : OshimaRegion
    {
        public 穹顶之泪湖()
        {
            Id = 11;
            Name = "穹顶之泪湖";
            Description = "破碎天穹下的倒影湖泊，折射多维星空，星辉水母群午夜重构水体重力法则";
            Category = "奇异";
            Weathers.Add("星光", 16);
            ChangeRandomWeather();
            Difficulty = RarityType.OneStar;
            Characters.Add(new(11101, "星辉巨母"));
            Units.Add(new(21101, "星辉水母"));
            Crops.Add(new(181101, "星辉凝露", "锻造物品的材料。", "穹顶之泪湖中星辉水母散发出的凝露，蕴含着重构水体重力法则的能量，但可能引发重力波动。"));
        }
    }

    public class 时漏沙漠 : OshimaRegion
    {
        public 时漏沙漠()
        {
            Id = 16;
            Name = "时漏沙漠";
            Description = "时间碎片组成的流沙领域，时之蝎加速局部时间，沙漏仙人掌分泌时凝液";
            Category = "奇异";
            Weathers.Add("不稳定", 38);
            ChangeRandomWeather();
            Difficulty = RarityType.TwoStar;
            Characters.Add(new(11601, "时之君王"));
            Units.Add(new(21601, "时之蝎"));
            Crops.Add(new(181601, "时凝液", "锻造物品的材料。", "时漏沙漠中沙漏仙人掌分泌的液体，拥有着粘稠的质地和淡淡的光泽。据说它能够加速时间的流逝，但使用时需要谨慎。"));
        }
    }

    public class 谵妄海市 : OshimaRegion
    {
        public 谵妄海市()
        {
            Id = 20;
            Name = "谵妄海市";
            Description = "需认知干扰剂进入的幻觉城市，思维寄生虫伪装市民，贩卖可食用梦境碎片";
            Category = "奇异";
            Weathers.Add("迷幻", 20);
            ChangeRandomWeather();
            Difficulty = RarityType.FourStar;
            Characters.Add(new(12001, "梦魇之主"));
            Units.Add(new(22001, "思维寄生虫"));
            Crops.Add(new(182001, "梦境碎片", "锻造物品的材料。", "谵妄海市中流通的特殊商品，拥有着不同的颜色和味道。据说它能够影响人的梦境，甚至改变人的认知。"));
        }
    }
}
