using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Regions
{
    public class 雷霆王座山脉 : OshimaRegion
    {
        public 雷霆王座山脉()
        {
            Id = 6;
            Name = "雷霆王座山脉";
            Description = "悬浮岩块组成的三维迷宫，最高峰「裁决尖碑」在月圆之夜投射出泰坦调试世界的符文";
            Category = "世外";
            Weathers.Add("雷暴", 5);
            ChangeRandomWeather();
            Difficulty = RarityType.FourStar;
            Characters.Add(new(10601, "雷霆泰坦"));
            Units.Add(new(20601, "雷霆元素"));
            Crops.Add(new(180601, "泰坦符文石", "锻造物品的材料。", "雷霆王座山脉裁决尖碑上脱落的符文石，蕴含着泰坦调试世界的法则，但解读时需要强大的精神力。"));
        }
    }

    public class 苍穹碎屿 : OshimaRegion
    {
        public 苍穹碎屿()
        {
            Id = 9;
            Name = "苍穹碎屿";
            Description = "破碎天穹形成的浮空岛群，「星锚之地」竖立着束缚星空巨兽的引雷柱";
            Category = "世外";
            Weathers.Add("晴朗", 18);
            ChangeRandomWeather();
            Difficulty = RarityType.ThreeStar;
            Characters.Add(new(10901, "星空巨兽"));
            Units.Add(new(20901, "浮空岛灵"));
            Crops.Add(new(180901, "星锚晶石", "锻造物品的材料。", "苍穹碎屿星锚之地用于束缚星空巨兽的晶石，蕴含着强大的雷电能量，但可能引发空间裂缝。"));
        }
    }
}
