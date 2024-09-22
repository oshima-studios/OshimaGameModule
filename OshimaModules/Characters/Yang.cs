using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Characters
{
    public class Yang : Character
    {
        public Yang() : base()
        {
            Id = 3;
            Name = "Ya";
            FirstName = "Yang";
            NickName = "吖养";
            PrimaryAttribute = PrimaryAttribute.STR;
            InitialATK = 23;
            InitialHP = 105;
            InitialMP = 55;
            InitialSTR = 11;
            STRGrowth = 1.8;
            InitialAGI = 9;
            AGIGrowth = 0.5;
            InitialINT = 10;
            INTGrowth = 0.7;
            InitialSPD = 300;
            InitialHR = 4;
            InitialMR = 2;
        }
    }
}
