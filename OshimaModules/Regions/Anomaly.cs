using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Regions
{
    public class 银辉城 : OshimaRegion
    {
        public 银辉城()
        {
            Id = 1;
            Name = "银辉城";
            Description = "悬浮在云海中的倒三角金属都市，建筑由星银合金铸造，街道流淌着液态月光，核心区藏有能改写现实法则的「悖论引擎」。破碎的天穹折射多维星空，星辉水母群在午夜时分重构水体重力法则。";
            Category = "奇异";
            Weathers.Add("晴朗", 20);
            Weathers.Add("星光", 16);
            ChangeRandomWeather();
            Difficulty = RarityType.FiveStar;
            Characters.Add(new(10101, "失控的悖论引擎"));
            Characters.Add(new(10102, "星辉巨母"));
            Units.Add(new(20101, "星银守卫"));
            Units.Add(new(20102, "星辉水母"));
            Crops.Add(new(180101, "星银合金", "锻造物品的材料。", "银辉城特有的金属材料，拥有银色的光泽和坚固的质地。能够吸收和储存能量，是建造城市和制造武器的理想材料。"));
            Crops.Add(new(180102, "液态月光", "锻造物品的材料。", "散发着柔和光芒的液体，如同月光般清澈。蕴含悖论引擎的能量，能够影响现实的结构。"));
            Crops.Add(new(180103, "星辉凝露", "锻造物品的材料。", "星辉水母散发的凝露，蕴含重构重力的能量，能够暂时扭曲局部空间法则。"));
            NPCs.Add("莉娅");
            NPCs.Add("沉默守卫G-7");
            Areas.Add("悖论深井");
            Areas.Add("星屑回廊");
        }
    }

    public class 永霜裂痕 : OshimaRegion
    {
        public 永霜裂痕()
        {
            Id = 4;
            Name = "永霜裂痕";
            Description = "冰晶峡谷冻结着不同时代的战争残影，哨塔时钟随机倒转/加速，需服用「时霜药剂」保持神智。";
            Category = "奇异";
            Weathers.Add("极寒", -25);
            ChangeRandomWeather();
            Difficulty = RarityType.FiveStar;
            Characters.Add(new(10401, "时空扭曲者"));
            Units.Add(new(20401, "冰霜傀儡"));
            Crops.Add(new(180401, "时霜药剂", "锻造物品的材料。", "能够减缓时间流逝的药剂，维持神智清醒但可能导致记忆混乱和时间感知错乱。"));
            Crops.Add(new(180402, "冰封记忆", "锻造物品的材料。", "冰封的古代战争幻象碎片，触碰时引发强烈记忆回溯与认知扭曲。"));
            NPCs.Add("艾萨克");
            NPCs.Add("冻伤的信使");
            Areas.Add("时钟哨塔");
            Areas.Add("记忆回廊");
        }
    }

    public class 千瞳镜湖 : OshimaRegion
    {
        public 千瞳镜湖()
        {
            Id = 6;
            Name = "千瞳镜湖";
            Description = "湖面倒影展现平行时空，潜入会进入重力颠倒的镜像城，湖底布满瞳孔状传送门。";
            Category = "奇异";
            Weathers.Add("阴沉", 10);
            Weathers.Add("微风", 8);
            ChangeRandomWeather();
            Difficulty = RarityType.TwoStar;
            Characters.Add(new(10601, "镜像之主"));
            Units.Add(new(20601, "镜像守卫"));
            Crops.Add(new(180601, "量子纠缠碎片", "锻造物品的材料。", "瞳孔状传送门的微观残片，能够引发量子纠缠现象，连接平行时空。"));
            NPCs.Add("奥尔加");
            NPCs.Add("溺亡观测者");
            Areas.Add("倒影城");
            Areas.Add("千瞳之巢");
        }
    }

    public class 时之荒漠 : OshimaRegion
    {
        public 时之荒漠()
        {
            Id = 7;
            Name = "时之荒漠";
            Description = "蕴含时间魔法的无尽沙海，沙丘每小时重组地形，沙暴中浮现「昨日之城」幻影。时间碎片构成的流沙中，时之蝎在时漏仙人掌间穿行。";
            Category = "奇异";
            Weathers.Add("沙尘暴", 35);
            Weathers.Add("不稳定", 38);
            ChangeRandomWeather();
            Difficulty = RarityType.FourStar;
            Characters.Add(new(10701, "时间吞噬者"));
            Characters.Add(new(10702, "时之君王"));
            Units.Add(new(20701, "流沙蝎"));
            Units.Add(new(20702, "时之蝎"));
            Crops.Add(new(180701, "时间碎片", "锻造物品的材料。", "时间残骸形成的晶体，拥有不规则形状和模糊纹路，会随机重组周围时空。"));
            Crops.Add(new(180702, "时凝液", "锻造物品的材料。", "时漏仙人掌分泌的粘稠液体，能够加速或减缓局部时间流逝，但难以控制。"));
            NPCs.Add("卖沙人");
            NPCs.Add("循环勘探队");
            Areas.Add("昨日之城");
            Areas.Add("时漏绿洲");
        }
    }

    public class 谵妄海市 : OshimaRegion
    {
        public 谵妄海市()
        {
            Id = 8;
            Name = "谵妄海市";
            Description = "需认知干扰剂才能安全进入的幻觉城市，思维寄生虫伪装市民，贩卖可食用梦境碎片。";
            Category = "奇异";
            Weathers.Add("迷幻", 20);
            ChangeRandomWeather();
            Difficulty = RarityType.FourStar;
            Characters.Add(new(10801, "梦魇之主"));
            Units.Add(new(20801, "思维寄生虫"));
            Crops.Add(new(180801, "梦境碎片", "锻造物品的材料。", "能够改变认知的梦境残留物，摄入后会混淆现实与虚幻的边界。"));
            NPCs.Add("认知矫正师");
            NPCs.Add("洛伦佐");
            Areas.Add("梦境交易所");
            Areas.Add("​​幻疡医院");
        }
    }
}
