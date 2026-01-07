using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Characters
{
    public class XReouni : Character
    {
        public XReouni() : base()
        {
            Id = 14;
            Name = "X";
            FirstName = "Reouni";
            NickName = "雷欧尼";
            PrimaryAttribute = PrimaryAttribute.INT;
            InitialATK = 19;
            InitialHP = 90;
            InitialMP = 75;
            InitialSTR = 9;
            STRGrowth = 0.9;
            InitialAGI = 9;
            AGIGrowth = 0.6;
            InitialINT = 12;
            INTGrowth = 1.5;
            InitialSPD = 300;
            InitialHR = 4;
            InitialMR = 2;
        }
    }
}
