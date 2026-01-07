using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Characters
{
    public class Neptune : Character
    {
        public Neptune() : base()
        {
            Id = 15;
            Name = "Nep";
            FirstName = "tune";
            NickName = "海王星";
            PrimaryAttribute = PrimaryAttribute.STR;
            InitialATK = 26;
            InitialHP = 95;
            InitialMP = 55;
            InitialSTR = 16;
            STRGrowth = 1.4;
            InitialAGI = 7;
            AGIGrowth = 1;
            InitialINT = 7;
            INTGrowth = 0.6;
            InitialSPD = 300;
            InitialHR = 4;
            InitialMR = 2;
        }
    }
}
