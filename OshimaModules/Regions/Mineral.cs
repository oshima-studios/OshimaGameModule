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
            Areas.Add("鱿熔血池");
            Areas.Add("活体锻炉");
            ContinuousQuestList.Add("元素裂隙的熵增警告", new("使用抗魔探针扫描永燃坩埚第47层矿道，绘制元素裂缝的扩张轨迹，评估其引发位面坍缩的风险等级。"));
            ContinuousQuestList.Add("坩埚熔岩的生命观测", new("在永燃坩埚的火山灰层建立实验室，监测活体金属苔藓与岩浆鱿鱼的共生进化过程。"));
            ImmediateQuestList.Add("深渊火种收容危机", new("永燃坩埚矿道底层的深渊火钻因元素污染进入链式裂变，引发全矿道魔能过载，立即部署熵减力场遏制反应。"));
            ImmediateQuestList.Add("坩埚金属瘟疫爆发", new("永燃坩埚的活体金属苔藓变异为吞噬性瘟疫，正沿岩浆通道快速扩散，必须立即启动熔断隔离机制。"));
            ProgressiveQuestList.Add("火钻精炼协议", new("在永燃坩埚矿工灵魂烙印的指引下获取 {0} 颗深渊火钻（未烙印者触碰火钻将引发元素爆燃）。", item: "深渊火钻"));
            ProgressiveQuestList.Add("坩埚活体金属培育", new("在永燃坩埚培育 {0} 份活体金属苔藓（注意培育环境需保持600℃以上恒温）。", item: "活体金属苔藓"));
        }
    }
}
