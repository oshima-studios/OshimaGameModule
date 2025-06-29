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
            ContinuousQuestList.Add("泰坦遗迹的符文解读", new("攀登泰坦遗迹，记录裁决尖碑在月圆之夜投射的泰坦符文，破译其蕴含的宇宙法则。"));
            ContinuousQuestList.Add("矿脉神经的止痛研究", new("在泰坦遗迹的悲鸣矿脉区开发神经缓冲装置，减轻开采神经宝石引发的山体剧痛反应。"));
            ImmediateQuestList.Add("泰坦遗迹的磁场危机", new("泰坦遗迹的悬浮岩块因神经宝石共振引发磁场紊乱，必须在半小时内稳定能量场防止区域崩塌。"));
            ImmediateQuestList.Add("矿脉神经共鸣失控", new("泰坦遗迹悲鸣矿脉的神经宝石集体共振，引发山体结构崩解，必须立即部署阻尼力场稳定矿脉。"));
            ProgressiveQuestList.Add("泰坦符文拓印", new("在泰坦遗迹的裁决尖碑上拓印 {0} 份不同的泰坦符文石（避开雷暴时段/每逢子时）。", item: "泰坦符文石"));
            ProgressiveQuestList.Add("矿脉神经镇痛研究", new("在泰坦遗迹的悲鸣矿脉区采集 {0} 份神经宝石样本（开采时会引发山体剧痛反应）。", item: "神经宝石"));
        }
    }
}
