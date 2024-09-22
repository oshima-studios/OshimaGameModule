using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Characters
{
    public class OshimaShiya : Character
    {
        public OshimaShiya() : base()
        {
            Id = 1;
            Name = "Oshima";
            FirstName = "Shiya";
            NickName = "大島シヤ";
            PrimaryAttribute = PrimaryAttribute.STR;
            InitialATK = 25;
            InitialHP = 85;
            InitialMP = 10;
            InitialSTR = 35;
            STRGrowth = 3.5;
            InitialAGI = 0;
            AGIGrowth = 0;
            InitialINT = 0;
            INTGrowth = 0;
            InitialSPD = 300;
            InitialHR = 4;
            InitialMR = 2;
        }
    }
}
