using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Characters
{
    public class TheGodK : Character
    {
        public TheGodK() : base()
        {
            Id = 18;
            Name = "The God";
            FirstName = "K";
            NickName = "K神";
            PrimaryAttribute = PrimaryAttribute.INT;
            InitialATK = 23;
            InitialHP = 80;
            InitialMP = 70;
            InitialSTR = 6;
            STRGrowth = 0.6;
            InitialAGI = 11;
            AGIGrowth = 1.1;
            InitialINT = 13;
            INTGrowth = 1.3;
            InitialSPD = 300;
            InitialHR = 4;
            InitialMR = 2;
        }
    }
}
