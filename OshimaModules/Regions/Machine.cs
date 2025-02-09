using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Regions
{
    public class 齿轮坟场 : OshimaRegion
    {
        public 齿轮坟场()
        {
            Id = 12;
            Name = "齿轮坟场";
            Description = "堆积上古机械文明的金属荒漠，沙粒为微缩齿轮，构装巨龙在沙暴中游荡";
            Category = "机械";
            Weathers.Add("沙尘", 30);
            ChangeRandomWeather();
            Difficulty = RarityType.ThreeStar;
            Characters.Add(new(11201, "报废的构装巨龙"));
            Units.Add(new(21201, "齿轮傀儡"));
            Crops.Add(new(181201, "机械核心碎片", "锻造物品的材料。", "齿轮坟场中构装巨龙残骸的动力核心碎片，蕴含着上古机械文明的能量，但可能带有自毁装置。"));
        }
    }

    public class 齿与血回廊 : OshimaRegion
    {
        public 齿与血回廊()
        {
            Id = 10;
            Name = "齿与血回廊";
            Description = "自我扩建的活体建筑群，齿轮血管输送液态魔力，「造物车间」会强制改造闯入者";
            Category = "机械";
            Weathers.Add("阴暗", 12);
            ChangeRandomWeather();
            Difficulty = RarityType.FiveStar;
            Characters.Add(new(11001, "回廊之心"));
            Units.Add(new(21001, "改造士兵"));
            Crops.Add(new(181001, "活体魔力血", "锻造物品的材料。", "齿与血回廊活体建筑中流动的液态魔力，具有自我修复和改造的能力，但接触可能导致身体异变。"));
        }
    }
}
