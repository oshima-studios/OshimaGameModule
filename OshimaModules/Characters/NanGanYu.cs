using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Characters
{
    public class NanGanYu : Character
    {
        public NanGanYu() : base()
        {
            Id = 4;
            Name = "Nan";
            FirstName = "Ganyu";
            NickName = "男甘雨";
            PrimaryAttribute = PrimaryAttribute.INT;
            InitialATK = 17;
            InitialHP = 115;
            InitialMP = 80;
            InitialSTR = 6;
            STRGrowth = 0.6;
            InitialAGI = 7;
            AGIGrowth = 0.7;
            InitialINT = 17;
            INTGrowth = 1.7;
            InitialSPD = 300;
            InitialHR = 4;
            InitialMR = 2;
        }
    }
}
