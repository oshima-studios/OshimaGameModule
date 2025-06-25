using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Regions
{
    public class 齿轮坟场 : OshimaRegion
    {
        public 齿轮坟场()
        {
            Id = 5;
            Name = "齿轮坟场";
            Description = "堆积上古机械文明的金属荒漠，沙粒为微缩齿轮。活体建筑群蔓延其中，齿轮血管输送液态魔力，造物车间不断改造闯入者。";
            Category = "机械";
            Weathers.Add("沙尘", 30);
            Weathers.Add("阴暗", -5);
            ChangeRandomWeather();
            Difficulty = RarityType.TwoStar;
            Characters.Add(new(10501, "报废的构装巨龙"));
            Characters.Add(new(10502, "回廊之心"));
            Units.Add(new(20501, "齿轮傀儡"));
            Units.Add(new(20502, "改造士兵"));
            Crops.Add(new(180501, "机械核心碎片", "锻造物品的材料。", "上古机械文明的能量核心，蕴含强大动力但可能触发自毁程序。"));
            Crops.Add(new(180502, "活体魔力血", "锻造物品的材料。", "具有自我修复能力的液态魔力，接触会导致身体不可预知的异变。"));
        }
    }
}
