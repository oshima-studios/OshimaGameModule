using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Regions
{
    public class 赫菲斯托斯之喉 : OshimaRegion
    {
        public 赫菲斯托斯之喉()
        {
            Id = 3;
            Name = "赫菲斯托斯之喉";
            Description = "螺旋向下的火山矿井，底层矿工开采深渊火钻，矿道会突然熔化成通往元素位面的裂缝";
            Category = "矿区";
            Weathers.Add("炎热", 45);
            ChangeRandomWeather();
            Difficulty = RarityType.FourStar;
            Characters.Add(new(10301, "熔岩巨兽"));
            Units.Add(new(20301, "火焰元素"));
            Crops.Add(new(180301, "深渊火钻", "锻造物品的材料。", "赫菲斯托斯之喉深处开采出的珍贵矿石，散发着炙热的红色光芒。据说只有被矿工灵魂烙印认可的人才能安全触碰它。"));
        }
    }

    public class 悲鸣矿脉 : OshimaRegion
    {
        public 悲鸣矿脉()
        {
            Id = 18;
            Name = "悲鸣矿脉";
            Description = "岩层嵌满神经宝石的活体矿山，开采引发山体剧痛，晶簇守卫实体化巡逻";
            Category = "矿区";
            Weathers.Add("幽暗", 10);
            ChangeRandomWeather();
            Difficulty = RarityType.FourStar;
            Characters.Add(new(11801, "矿脉之心"));
            Units.Add(new(21801, "晶簇守卫"));
            Crops.Add(new(181801, "神经宝石", "锻造物品的材料。", "悲鸣矿脉中开采出的特殊宝石，散发着微弱的蓝色光芒。据说它与矿脉的神经系统相连，能够引发剧烈的疼痛。"));
            Crops.Add(new(181802, "矿脉神经纤维", "锻造物品的材料。", "悲鸣矿脉中连接神经宝石的纤维，触碰时会引发山体剧痛，并可能导致精神污染。"));
        }
    }
}
