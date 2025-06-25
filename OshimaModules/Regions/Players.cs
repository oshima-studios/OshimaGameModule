namespace Oshima.FunGame.OshimaModules.Regions
{
    public class 铎京城 : OshimaRegion
    {
        public 铎京城()
        {
            Id = 0;
            Name = "铎京城";
            Description = "铎京是大陆的科技中枢，是大陆中的一个城邦国家。其首都铎京市是一座具有独立权势的商业城市，以精密机械和能量研究、强大的现代科技与军事实力闻名。";
            Category = "此地";
            Weathers.Add("酷暑", 39);
            Weathers.Add("炙热", 33);
            Weathers.Add("晴朗", 26);
            Weathers.Add("多云", 20);
            Weathers.Add("暴雨", 14);
            Weathers.Add("寒冷", 6);
            Weathers.Add("严寒", -7);
            Weathers.Add("霜冻", -18);
            ChangeRandomWeather();
        }
    }
}
