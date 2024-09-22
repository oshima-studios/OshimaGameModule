using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Characters
{
    public class NiuNan : Character
    {
        public NiuNan() : base()
        {
            Id = 5;
            Name = "Niu";
            FirstName = "Nan";
            NickName = "牛腩";
            PrimaryAttribute = PrimaryAttribute.INT;
            InitialATK = 16;
            InitialHP = 75;
            InitialMP = 90;
            InitialSTR = 0;
            STRGrowth = 0;
            InitialAGI = 0;
            AGIGrowth = 0;
            InitialINT = 30;
            INTGrowth = 3;
            InitialSPD = 300;
            InitialHR = 4;
            InitialMR = 2;
        }
    }
}
