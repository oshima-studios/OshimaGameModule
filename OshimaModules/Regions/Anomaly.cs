using Milimoe.FunGame.Core.Library.Constant;
using Oshima.Core.Constant;

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
            Areas.Add("星辉水域");
            Areas.Add("上城区");
            Areas.Add("旧街道");
            ContinuousQuestList.Add("悖论引擎的暗涌之息", new("穿梭于银辉城流淌液态月光的街道，侦测悖论引擎释放的异常能量潮汐，揭开可能撕裂现实结构的危险谜团。"));
            ContinuousQuestList.Add("镜湖星辉的引力观察", new("在银辉城折射星空的水域边缘建立观测点，记录星辉水母群在午夜重构重力法则时的空间涟漪。"));
            ImmediateQuestList.Add("星银警戒协议·弎级响应", new("星银合金守卫在悖论引擎周边暴走，形成包围核心区的杀戮矩阵，必须在三刻钟内解除警戒协议。"));
            ImmediateQuestList.Add("液态月光洪流预警", new("银辉城街道的液态月光进入异常涨潮期，即将淹没悖论引擎控制中枢，必须立即启动排水协议。"));
            ProgressiveQuestList.Add("月光萃取计划", new("前往银辉城，采集 {0} 份液态月光（避开月光洪流的**时段/每夜丑时三刻）。", item: "液态月光"));
            ProgressiveQuestList.Add("湖底重力场测绘", new("在银辉城星辉水域测绘 {0} 个重力异常点（星银合金），注意避开午夜水母重构期。", item: "星银合金"));
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
            ContinuousQuestList.Add("时间琥珀中的战争回响", new("在永霜裂痕建立时滞力场实验室，分析冻结在冰壁中的古代战争幻象，还原时霜药剂对观察者认知体系的扭曲机制。"));
            ImmediateQuestList.Add("时霜逆流救援行动", new("科研小组被困在永霜裂痕加速百倍的时间泡内，其肉体正以肉眼可见的速度衰老，必须校准哨塔时钟恢复时间流速。"));
            ProgressiveQuestList.Add("时霜逆向工程", new("通过时间镜像收集 {0} 份来自永霜裂痕不同历史断片的时霜药剂样本（注意时空回响对记忆的覆盖效应）。", item: "时霜药剂"));
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
            ContinuousQuestList.Add("镜像维度的认知污染", new("佩戴反重力拘束装置潜入千瞳镜湖，测绘镜像城的拓扑结构，警惕瞳孔状传送门对记忆模块的逆向写入现象。"));
            ImmediateQuestList.Add("镜像侵蚀净化指令", new("千瞳镜湖的镜像守卫突破维度屏障入侵现实，携带的认知病毒正在改写物理法则，启动空间净化协议隔离威胁。"));
            ProgressiveQuestList.Add("瞳孔密钥重构计划", new("前往千瞳镜湖，从 {0} 个瞳孔状传送门提取量子纠缠碎片（保持镜像对称操作以避免维度塌缩）。", item: "量子纠缠碎片"));
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
            Areas.Add("沙丘核心");
            ContinuousQuestList.Add("流沙时计的昨日寻踪", new("在时之荒漠中寻找海市蜃楼般的昨日之城，收集散落在其中的时间碎片，还原历史真相。"));
            ContinuousQuestList.Add("时沙陷阱的预警系统", new("在时之荒漠部署时间流沙监测网，预测沙丘重组周期并标记高危区域，保障探索者安全。"));
            ImmediateQuestList.Add("流沙陷阱紧急救援", new("勘探队被困在时之荒漠重组的沙丘核心，必须在沙暴吞噬前开辟逃生通道。"));
            ImmediateQuestList.Add("时之蝎群时空侵染", new("时之荒漠的时之蝎因时凝液泄漏引发群体狂暴，其时间加速能力正在污染周边空间，需立即净化。"));
            ProgressiveQuestList.Add("时间碎片收集", new("在时之荒漠的昨日之城中收集 {0} 份不同的时间碎片（注意时间碎片会随机重组）。", item: "时间碎片"));
            ProgressiveQuestList.Add("时凝液时效研究", new("在时之荒漠提纯 {0} 份时凝液（操作需在时间加速器防护罩内完成）。", item: "时凝液"));
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
            ContinuousQuestList.Add("谵妄梦境的思维隔离", new("在谵妄海市建立认知过滤系统，阻断思维寄生虫对市民意识的寄生感染路径。"));
            ImmediateQuestList.Add("谵妄认知病毒爆发", new("谵妄海市的思维寄生虫释放II型认知病毒，感染市民产生现实扭曲能力，必须一小时内建立精神隔离区。"));
            ProgressiveQuestList.Add("梦境碎片毒性分析", new("在谵妄海市分析 {0} 份不同的梦境碎片（需全程佩戴认知过滤器）。", item: "梦境碎片"));
        }
    }
}
