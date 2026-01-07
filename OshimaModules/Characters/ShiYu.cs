using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Characters
{
    public class ShiYu : Character
    {
        public ShiYu() : base()
        {
            Id = 13;
            Name = "Shi";
            FirstName = "Yu";
            NickName = "时雨";
            PrimaryAttribute = PrimaryAttribute.AGI;
            InitialATK = 20;
            InitialHP = 105;
            InitialMP = 65;
            InitialSTR = 7;
            STRGrowth = 0.7;
            InitialAGI = 14;
            AGIGrowth = 1.5;
            InitialINT = 9;
            INTGrowth = 0.8;
            InitialSPD = 300;
            InitialHR = 4;
            InitialMR = 2;
        }
    }
}
