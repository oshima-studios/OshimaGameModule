using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Characters
{
    public class CHAOS : Character
    {
        public CHAOS() : base()
        {
            Id = 16;
            Name = "CHAO";
            FirstName = "SHI";
            NickName = "超市";
            PrimaryAttribute = PrimaryAttribute.STR;
            InitialATK = 27;
            InitialHP = 100;
            InitialMP = 40;
            InitialSTR = 14;
            STRGrowth = 1.8;
            InitialAGI = 12;
            AGIGrowth = 1.1;
            InitialINT = 4;
            INTGrowth = 0.1;
            InitialSPD = 300;
            InitialHR = 4;
            InitialMR = 2;
        }
    }
}
