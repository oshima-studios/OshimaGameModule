using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Regions
{
    public class 永燃坩埚 : OshimaRegion
    {
        public 永燃坩埚()
        {
            Id = 3;
            Name = "永燃坩埚";
            Description = "螺旋向下的火山锻造都市，岩浆鱿鱼在街道游弋，底层矿工开采深渊火钻。火山灰培育的活体金属苔藓覆盖着球形建筑。";
            Category = "矿区";
            Weathers.Add("高温", 60);
            Weathers.Add("炎热", 45);
            ChangeRandomWeather();
            Difficulty = RarityType.OneStar;
            Characters.Add(new(10301, "岩浆之王"));
            Characters.Add(new(10302, "熔岩巨兽"));
            Units.Add(new(20301, "岩浆鱿鱼"));
            Units.Add(new(20302, "火焰元素"));
            Crops.Add(new(180301, "活体金属苔藓", "锻造物品的材料。", "具有金属质感的生命体，能够自我修复和繁殖，是研究金属生命的珍贵样本。"));
            Crops.Add(new(180302, "深渊火钻", "锻造物品的材料。", "火山深处开采的珍稀矿石，只有被矿工灵魂烙印认可者才能安全触碰。"));
            NPCs.Add("\"铁颚\"巴拉克");
            NPCs.Add("苔丝夫人");
            Areas.Add("鱿熔血池​​");
            Areas.Add("活体锻炉");
        }
    }
}
