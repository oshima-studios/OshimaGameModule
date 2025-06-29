using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Regions
{
    public class 泰坦遗迹 : OshimaRegion
    {
        public 泰坦遗迹()
        {
            Id = 12;
            Name = "泰坦遗迹";
            Description = "悬浮岩块组成的三维迷宫，岩层嵌满哀嚎的神经宝石。最高峰「裁决尖碑」在月圆之夜投射泰坦调试世界的符文。";
            Category = "世外";
            Weathers.Add("雷暴", 5);
            Weathers.Add("幽暗", 10);
            ChangeRandomWeather();
            Difficulty = RarityType.ThreeStar;
            Characters.Add(new(11201, "雷霆泰坦"));
            Characters.Add(new(11202, "矿脉之心"));
            Units.Add(new(21201, "雷霆元素"));
            Units.Add(new(21202, "晶簇守卫"));
            Crops.Add(new(181201, "泰坦符文石", "锻造物品的材料。", "裁决尖碑脱落的法则碎片，蕴含世界调试规则，解读需强大精神力。"));
            Crops.Add(new(181202, "神经宝石", "锻造物品的材料。", "与矿脉神经相连的晶体，开采时引发山体剧痛和精神污染。"));
            Crops.Add(new(181203, "矿脉神经纤维", "锻造物品的材料。", "连接神经宝石的活体纤维，触碰导致剧烈痛苦和神经感染。"));
            NPCs.Add("泰坦刻录员");
            NPCs.Add("矿痛共生体");
            Areas.Add("神经矿脉​​");
            Areas.Add("雷霆王座");
            Areas.Add("裁决矩阵");
        }
    }
}
