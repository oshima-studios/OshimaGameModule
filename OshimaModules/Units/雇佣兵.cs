using Milimoe.FunGame.Core.Entity;

namespace Oshima.FunGame.OshimaModules
{
    public class 雇佣兵 : Unit
    {
        public override string Name => "雇佣兵";

        public override string ToString()
        {
            return NickName;
        }

        public 雇佣兵(Character master, string name) : base()
        {
            NickName = $"雇佣兵{name}（{master}）";
            Master = master;
            ExATR = master.ExATR - master.ATR;
            ExMOV = master.ExMOV - master.MOV;
        }
    }
}
