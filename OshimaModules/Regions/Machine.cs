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
            NPCs.Add("\"噬罪者\"");
            NPCs.Add("7号改造体");
            Areas.Add("忏悔教堂");
            Areas.Add("造物车间");
            ContinuousQuestList.Add("机械坟场的改造危机", new("潜入齿轮坟场被活体建筑同化的造物车间，研究其机械与生物融合的改造机制，寻找控制方法。"));
            ContinuousQuestList.Add("齿轮活血的污染控制", new("在齿轮坟场建立液态魔力隔离区，防止活体魔力血的生物污染扩散至外部环境。"));
            ImmediateQuestList.Add("活体建筑增殖警报", new("齿轮坟场的活体建筑开始暴走式增殖，必须在三刻钟内切断核心供能管道遏制扩张。"));
            ImmediateQuestList.Add("机械坟场核心熔毁", new("齿轮坟场的上古机械核心因活体魔力血污染进入熔毁倒计时，需立即执行冷却协议防止爆炸。"));
            ProgressiveQuestList.Add("活体建筑解析计划", new("在齿轮坟场研究 {0} 个活体建筑样本（活体魔力血）。", item: "活体魔力血"));
            ProgressiveQuestList.Add("机械核心解密行动", new("在齿轮坟场解析 {0} 个上古机械核心（机械核心碎片）。", item: "机械核心碎片"));
        }
    }
}
